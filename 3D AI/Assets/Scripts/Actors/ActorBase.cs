using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class ActorBase : MonoBehaviour
{
	#region Public Members
	public string actorName;
	public List<GameObject> m_projectiles = new List<GameObject>(), currentPath;
	public float health = 10.0f,
	viewDistance;
	public int progressAlongPath = 0;
	public int currentFacing = -1, Team, actionPoints = 3;
	public GameObject Torpedo, startNode, goalNode, currentTrOct;
	#endregion

	#region Protected Members
	protected Vector3 shootDir = new Vector3(1, 0, 0);
	#endregion

	void Start ()
	{
		currentPath = new List<GameObject>();

		currentFacing = FindFacing(currentTrOct);

		viewDistance = TeamManager.instance.fowDistance;

	}
	
	// Update is called once per frame
	protected void Update ()
	{
		if (!currentTrOct) {
			//set the currentTroct to be the current troct and set it's containedActor to this
			currentTrOct = SetTroct (startNode);
			
			//set the troct's containedActor to be this
			currentTrOct.GetComponent<TruncOct> ().containedActor = gameObject;
		}
		else //check if the current troct's contained actor = this
		{
			if (currentTrOct.GetComponent<TruncOct> ().containedActor != gameObject)
			{
				currentTrOct.GetComponent<TruncOct> ().containedActor = gameObject;
			}
		}

		if (currentFacing == -1)
		{
			currentFacing = FindFacing(currentTrOct);
		}
		
		switch (GameManager.instance.GameState)
		{
		case GameStates.menu:
			break;
		case GameStates.debug:
			break;
		case GameStates.gameplay:
			Tick ();
			break;
		case GameStates.gamepause:
			break;
		}
	}
	
	protected abstract void Tick ();
	
	/// <summary>
	/// Sets the troct to be the startNode, wooooooo
	/// </summary>
	/// <returns>The troct.</returns>
	/// <param name="_startNode">The start node.</param>
	GameObject SetTroct(GameObject _node)
	{
		foreach (GameObject _trOct in GameManager.instance.allTrocts)
		{
			if (_trOct == _node)
			{
				return _trOct;
			}
		}
		
		//if unable to be found, call FindCurrentTrOct()
		return FindCurrentTrOct ();
	}
	
	/// <summary>
	/// a backup function that will find the troct that this actor is within	/// </summary>
	/// <returns>The current troct</returns>
	GameObject FindCurrentTrOct()
	{
		//find the troct with the same position vector
		foreach (GameObject _trOct in GameManager.instance.allTrocts)
		{
			if (_trOct.transform.position == this.transform.position)
			{
				return _trOct;
			}
		}
		//If it reaches here, OH DEARS
		return null;
	}

	/// <summary>
	/// Finds the troct face being pointed to by the actor.
	/// </summary>
	/// <returns>The facing.</returns>
	private int FindFacing(GameObject _trOct)
	{
		List<Vector3> facings = _trOct.GetComponent<TruncOct> ().Faces;

		for (int i = 0; i < facings.Count; i++)
		{
			if (transform.forward == facings[i].normalized)
			{
				//return the number of the face being faced
				return i;
			}
		}
		//if it reaches here, oh dear
		return -1;
	}

	/// <summary>
	/// Called by TeamManager upon placing actors, this sets the facing.
	/// </summary>
	/// <param name="_face">The facing to face</param>
	public void setFacing(GameObject _trOct, int _face)
	{
		List<Vector3> facings = _trOct.GetComponent<TruncOct> ().Faces;

		transform.forward = facings[_face].normalized;

		currentFacing = _face;
	}

	public virtual void Init()
	{
	}

	/// <summary>
	/// Tries to rotate (tests to see if it has valid number of action points.
	/// </summary>
	public bool TryRotate(Transform _lookAt)
	{
		//if it still has actionPoints
		if (actionPoints != 0)
		{
			transform.LookAt (_lookAt);

			currentFacing = FindFacing(currentTrOct);

			actionPoints--;

			return true;
		}
		else
		{
			return false;
		}

	}

	/// <summary>
	/// Tries to move forwards.
	/// </summary>
	public bool TryMoveForwards()
	{
		//if it still has actionPoints
		if (actionPoints != 0)
		{
			//reset currentTroct's contained actor
			currentTrOct.GetComponent<TruncOct>().containedActor = null;

			//find the troct being faced
			GameObject newTrOct = GameManager.instance.allTrocts[currentTrOct.GetComponent<TruncOct>().connectionObjects[currentFacing]];

			//see if it is clear
			if (!newTrOct.GetComponent<TruncOct> ().containedActor && newTrOct.GetComponent<TruncOct> ().type != TruncOct.tileType.dead) {
				currentTrOct = newTrOct;

				currentTrOct.GetComponent<TruncOct> ().containedActor = gameObject;

				//set the new position to be the troct in facing direction
				transform.position = currentTrOct.transform.position;

				actionPoints--;

				return true;
			}
			else
			{
				currentTrOct.GetComponent<TruncOct> ().containedActor = gameObject;
			}
		}
		return false;
	}

	public void TakeDamage()
	{
		health -= 50;

		//check if now dead
		if (health < 0)
		{
			//tell the team Manager to kill this actor
			TeamManager.instance.Kill(gameObject);
		}
	}

	/// <summary>
	/// Attempt to shoot forwards
	/// </summary>
	public bool Shootforwards()
	{

		if (actionPoints != 0)
		{
			Torpedo.SetActive(true);

			actionPoints--;

			//tell the AI system that the actor shot successfully (has actionpoints)
			return true;
		}

		//tell the AI system that the actor could not move at this time.
		return false;
	}

	/// <summary>
	/// Plots an A* route by calling the central A* plotter
	/// </summary>
	public void plotRoute()
	{
		List<GameObject> route = GameManager.instance.GetComponent<aStar> ().GeneratePath (currentTrOct, goalNode);

		progressAlongPath = 0;

		currentPath = route;
	}
}
