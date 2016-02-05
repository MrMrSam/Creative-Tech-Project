﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class ActorBase : MonoBehaviour
{
	#region Public Members
	public string actorName;
	public List<GameObject> m_projectiles = new List<GameObject>(), currentPath;
	public float health = 100.0f;
	public int currentFacing = -1, Team, actionPoints = 2;
	public GameObject startNode, goalNode, currentTrOct;
	#endregion
	
	#region Protected Members
	protected Vector3 shootDir = new Vector3(1, 0, 0);
	#endregion

	void Start ()
	{
		//if no startNode, set one
		if (!startNode)
		{

		}
		currentFacing = FindFacing(currentTrOct);
	}
	
	// Update is called once per frame
	protected void Update ()
	{
		if (!currentTrOct) 
		{
			//set the currentTroct to be the current troct and set it's containedActor to this
			currentTrOct = SetTroct (startNode);
			
			//set the troct's containedActor to be this
			currentTrOct.GetComponent<TruncOct> ().containedActor = gameObject;
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

	public virtual void Init()
	{
	}

	/// <summary>
	/// Tries to rotate (tests to see if it has valid number of action points.
	/// </summary>
	public void TryRotate(Transform _lookAt)
	{
		//if it still has actionPoints
		if (actionPoints != 0)
		{
			transform.LookAt (_lookAt);

			currentFacing = FindFacing(currentTrOct);

			actionPoints--;
		}
		else
		{
			//ech.jpg
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
			if (newTrOct.GetComponent<TruncOct>().containedActor == null || newTrOct.GetComponent<TruncOct>().type != TruncOct.tileType.dead)
			{
				currentTrOct = newTrOct;

				currentTrOct.GetComponent<TruncOct>().containedActor = gameObject;

				//set the new position to be the troct in facing direction
				transform.position = currentTrOct.transform.position;

				actionPoints --;

				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Plots an A* route by calling the central A* plotter
	/// </summary>
	public void plotRoute()
	{
		List<GameObject> route = GameManager.instance.GetComponent<aStar> ().GeneratePath (currentTrOct, goalNode);

		currentPath = route;
	}

	public void InflictDamage()
	{
		if (health > 0)
		{
			health -= Random.Range(8, 15);
			Debug.Log("Health: " + health);
			//StartCoroutine(InflictDamageCol(Color.white, Color.black, 1.0f));
			
			if (health <= 0)
			{
				Debug.Log("explode");
			}
		}
	}
}