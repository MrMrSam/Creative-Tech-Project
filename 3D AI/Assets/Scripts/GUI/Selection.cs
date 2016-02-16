using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Selection : MonoBehaviour
{
	GameObject highlight, lastHighlight, selectedActor;
	public GameObject target, axis;
	public Vector3 targetOffset, position;
	public float distance = 5.0f, maxDistance = 20f, minDistance = 0.6f, xSpeed = 200.0f, ySpeed = 200.0f,
	panSpeed = 0.3f, zoomDampening = 5.0f, xDeg = 0.0f, yDeg = 0.0f, currentDistance, desiredDistance;
	public int yMinLimit = -80, yMaxLimit = 80, zoomRate = 40;
	private Quaternion currentRotation, desiredRotation, rotation;

	private List<GameObject> tentPath;

	//the state of selections
	public enum SelectionState {None, ActorSelected, TrOctSelected, PlanRoute, Rotate, Shoot}; 
	public SelectionState selectState = SelectionState.None;

	private static Selection m_instance = null;
	public static Selection instance { get { return m_instance; } }
	
	void Awake()
	{
		//instantiate singleton
		m_instance = this;
	}

	private void Start()
	{
		//If there is no target, create a temporary target at 'distance' from the cameras current viewpoint
		if (!target)
		{
			GameObject go = new GameObject("Cam Target");
			go.transform.position = transform.position + (transform.forward * distance);
			target = go;
		}
		else 
		{
			//set nearPlane as the distance to the target
			GetComponent<Camera>().nearClipPlane = (float)(Vector3.Distance(transform.position, target.transform.position) - 2.125);
		}
		
		distance = Vector3.Distance(transform.position, target.transform.position);
		currentDistance = distance;
		desiredDistance = distance;
		
		//be sure to grab the current rotations as starting points.
		position = transform.position;
		rotation = transform.rotation;
		currentRotation = transform.rotation;
		desiredRotation = transform.rotation;
		
		xDeg = Vector3.Angle(Vector3.right, transform.right );
		yDeg = Vector3.Angle(Vector3.up, transform.up );
	}

	void LateUpdate()
	{
		switch (GameManager.instance.GameState)
		{
		case GameStates.debug:
			break;
		case GameStates.gamepause:
			break;
		case GameStates.gameplay:
			LateTick();
			break;
		case GameStates.menu:
			break;
		}
	}

	void LateTick()
	{
		switch (selectState)
		{
		case SelectionState.None:
			break;
		case SelectionState.TrOctSelected:
			break;
		case SelectionState.ActorSelected:
			break;
		}

		if (Input.GetMouseButton(1) && Input.GetKey(KeyCode.LeftControl))
		{
			desiredDistance -= Input.GetAxis("Mouse Y") * Time.deltaTime * zoomRate*0.125f * Mathf.Abs(desiredDistance);
		}
		else if (Input.GetMouseButton(1))
		{
			xDeg += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
			yDeg -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
			
			////////OrbitAngle
			
			//Clamp the vertical axis for the orbit
			yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);
			// set camera rotation 
			desiredRotation = Quaternion.Euler(yDeg, xDeg, 0);
			currentRotation = transform.rotation;
			
			rotation = Quaternion.Lerp(currentRotation, desiredRotation, Time.deltaTime * zoomDampening);
			transform.rotation = rotation;
		}
		
		#region cast and sort ray
		
		if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject ())
		{
			
			//raycasts to select a new target
			Ray ray = GetComponent<Camera> ().ScreenPointToRay (Input.mousePosition);
			
			RaycastHit[] hits = Physics.RaycastAll (ray, GetComponent<Camera> ().nearClipPlane + 2);
			
			List<RaycastHit> hitsList = new List<RaycastHit> ();
			
			//go through _hits and remove those that are too close, and pull out the otherwise closest
			for (int i = 0; i < hits.Length; i++) {
				//if not too close, add to hitsList
				if (Vector3.Distance (hits [i].transform.position, transform.position) >= GetComponent<Camera> ().nearClipPlane) {
					hitsList.Add (hits [i]);
				}
			}
			
			RaycastHit closestValidHit = new RaycastHit ();
			
			for (int i = 0; i < hitsList.Count; i++) {
				if (i == 0) {
					closestValidHit = hitsList [i];
					break;
				}
				
				//if the distance from the camera to the current hit is less than what closestValid hit is, make it the new closestValidHit
				if (Vector3.Distance (hitsList [i].transform.position, transform.position) < Vector3.Distance (closestValidHit.transform.position, transform.position)) {
					closestValidHit = hitsList [i];
				}
			}
			
			
			#endregion

			if (closestValidHit.transform)
			{
				GameObject troctHit = closestValidHit.transform.gameObject;				

				//first filter to see if in the planRoute state, rotate state, or shoot state

				//if the state is rotate and the hit troct is next to the currently selected actor's containing troct
				if (selectState == SelectionState.Rotate && troctHit.GetComponent<TruncOct>().IsNextTo(selectedActor.GetComponent<ActorBase>().currentTrOct))
				{
					//if selecting a new troct, colour it and remove the colouring from lastHighlight
					if (troctHit != lastHighlight)
					{
						if (troctHit != target)
						{
							troctHit.GetComponent<TruncOct>().interior.gameObject.SetActive (true);
							troctHit.GetComponent<TruncOct>().interior.GetComponent<MeshRenderer> ().material.color = Color.green;
						}
						
						//return last highlight to its previous colour
						if (lastHighlight && (lastHighlight != target))
						{
							lastHighlight.GetComponent<TruncOct> ().ReturnToTypeColour ();
						}
						
						lastHighlight = troctHit;
					}
				}
				//else if currently planning a route, tell the player to plot a route to the current troct
				else if (selectState == SelectionState.PlanRoute)
				{
					//feed the player the goalnode
					selectedActor.GetComponent<PlayerActor>().goalNode = troctHit;

					//create a tentative path for display
					tentPath = GameManager.instance.GetComponent<aStar>().GeneratePath(selectedActor.GetComponent<PlayerActor>().currentTrOct, troctHit);
				}
				else 
				{
					//if selecting a new troct, colour it and remove the colouring from lastHighlight
					if (troctHit != lastHighlight && troctHit.GetComponent<TruncOct>())
					{
						if (troctHit != target)
						{
							troctHit.GetComponent<TruncOct>().interior.gameObject.SetActive (true);
							troctHit.GetComponent<TruncOct>().interior.GetComponent<MeshRenderer> ().material.color = Color.green;
						}
						
						//return last highlight to its previous colour
						if (lastHighlight && (lastHighlight != target))
						{
							lastHighlight.GetComponent<TruncOct> ().ReturnToTypeColour ();
						}
						
						lastHighlight = troctHit;
					}
				}

				//click the currently selected Troct
				if (Input.GetMouseButtonUp (0))
				{
					ClickTroct(troctHit);
				}
			}
		}

		// affect the desired Zoom distance if we roll the scrollwheel
		desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomRate * Mathf.Abs(desiredDistance);
		//clamp the zoom min/max
		desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
		// For smoothing of the zoom, lerp distance
		currentDistance = Mathf.Lerp(currentDistance, desiredDistance, Time.deltaTime * zoomDampening);
		
		// calculate position based on the new currentDistance 
		position = target.transform.position - (rotation * Vector3.forward * currentDistance + targetOffset);
		transform.position = position;

		//recalc the nearplane
		GetComponent<Camera>().nearClipPlane = (float)(Vector3.Distance(transform.position, target.transform.position) - 2.125);
	}

	/// <summary>
	/// Determines what happens when the troct is clicked.
	/// </summary>
	/// <param name="_troctHit">_troct hit.</param>
	private void ClickTroct(GameObject _troctHit)
	{
		switch (selectState)
		{
		case SelectionState.Rotate:
			if (_troctHit.GetComponent<TruncOct>().IsNextTo(selectedActor.GetComponent<ActorBase>().currentTrOct))
			{
				selectedActor.GetComponent<ActorBase>().TryRotate(_troctHit.transform);

				selectState = SelectionState.ActorSelected;
			}
			break;
		case SelectionState.PlanRoute:
			selectedActor.GetComponent<ActorBase>().currentPath = tentPath;
			break;
		default: //if not planning route or rotating, just select the troct as normal

			if (_troctHit.GetComponent<TruncOct> ())
			{
				setTarget (_troctHit);
			}
			break;
		}
	}


	//called when setting a new target
	public void setTarget (GameObject _target)
	{
		//if the current target has a troct, reset it
		if (target.GetComponent<TruncOct>())
		{
			target.GetComponent<TruncOct>().ReturnToTypeColour();
		}

		_target.GetComponent<TruncOct>().interior.gameObject.SetActive(true);
		_target.GetComponent<TruncOct>().interior.GetComponent<MeshRenderer>().material.color = Color.cyan;

		target = _target;

		//set nearPlane as the distance to the target
		GetComponent<Camera>().nearClipPlane = (float)(Vector3.Distance(transform.position, _target.transform.position) - 2.125);

		//set state to troctSelected
		selectState = SelectionState.TrOctSelected;
	}

	/// <summary>
	/// Selects the actor inside the current target trOct. Called by a function in GUIManager
	/// </summary>
	public GameObject SelectActor()
	{
		//tell the slection logic that is has selected an actor
		selectState = SelectionState.ActorSelected;

		selectedActor = target.GetComponent<TruncOct> ().containedActor;

		return selectedActor;
	}
	
	private static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360)
			angle += 360;
		if (angle > 360)
			angle -= 360;
		return Mathf.Clamp(angle, min, max);
	}
}