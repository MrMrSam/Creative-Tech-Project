//
//Filename: maxCamera.cs
//
// original: http://www.unifycommunity.com/wiki/index.php?title=MouseOrbitZoom
//
// --01-18-2010 - create temporary target, if none supplied at start

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Camera-Control/3dsMax Camera Style")]
public class MaxCamera : MonoBehaviour
{
	GameObject worldGen, axis, highlight, lastHighlight;
	public GameObject target;
	public Vector3 targetOffset, position;
	public float distance = 5.0f, maxDistance = 20f, minDistance = 0.6f, xSpeed = 200.0f, ySpeed = 200.0f,
	panSpeed = 0.3f, zoomDampening = 5.0f, xDeg = 0.0f, yDeg = 0.0f, currentDistance, desiredDistance;
	public int yMinLimit = -80, yMaxLimit = 80, zoomRate = 40;
	private Quaternion currentRotation, desiredRotation, rotation;
	
	void Start() { Init(); }
	void OnEnable() { Init(); }
	
	public void Init()
	{
		axis = GameObject.FindGameObjectWithTag("AxisObject");

		//axis.transform.position = GetComponent<Camera>().WorldToScreenPoint(new Vector3(Screen.width / 2, Screen.height / 2, 10.0f));

		worldGen = GameObject.FindGameObjectWithTag("WorldGen");

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


	
	/*
     * Camera logic on LateUpdate to only update after all character movement logic has been handled. 
     */
	void LateUpdate()
	{
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

		//raycasts to select a new target
		Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

		RaycastHit[]hits = Physics.RaycastAll(ray, GetComponent<Camera>().nearClipPlane + 2);

		List<RaycastHit> hitsList = new List<RaycastHit>();

		//go through _hits and remove those that are too close, and pull out the otherwise closest
		for (int i = 0; i < hits.Length; i++)
		{
			//if not too close, add to hitsList
			if (Vector3.Distance(hits[i].transform.position, transform.position) >= GetComponent<Camera>().nearClipPlane)
			{
				hitsList.Add(hits[i]);
			}
		}

		RaycastHit closestValidHit = new RaycastHit();

		for (int i = 0; i < hitsList.Count; i++)
		{
			if (i == 0)
			{
				closestValidHit = hitsList[i];
				break;
			}

			//if the distance from the camera to the current hit is less than what closestValid hit is, make it the new closestValidHit
			if (Vector3.Distance(hitsList[i].transform.position, transform.position) < Vector3.Distance(closestValidHit.transform.position, transform.position))
			{
				closestValidHit = hitsList[i];
			}
		}

		#endregion

		//Transform objectHit = _hit.transform;
		if (closestValidHit.transform)
		{
			GameObject troctHit = closestValidHit.transform.gameObject;

			

			//if selecting a new troct, colour it and remove the colouring from lastHighlight
			if (troctHit != lastHighlight)
			{
				if (troctHit != target)
				{
					troctHit.transform.GetChild(0).gameObject.SetActive(true);
					troctHit.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.green;
				}

				if (lastHighlight && (lastHighlight != target))
				{
					lastHighlight.GetComponent<TruncOct>().ReturnToTypeColour();
				}

				lastHighlight = troctHit;
			}

			if (Input.GetMouseButtonUp(0))
			{
				setTarget(troctHit);
			}
		}
		
		////////Orbit Position
		
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

	//called when setting a new target
	public void setTarget (GameObject _target)
	{
		//if the current target has a troct, reset it
		if (target.GetComponent<TruncOct>())
		{
			target.GetComponent<TruncOct>().ReturnToTypeColour();
		}

		_target.transform.GetChild(0).gameObject.SetActive(true);
		_target.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.cyan;

		target = _target;

		//set nearPlane as the distance to the target
		GetComponent<Camera>().nearClipPlane = (float)(Vector3.Distance(transform.position, _target.transform.position) - 2.125);
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