/// <summary>
/// AIDemo is a temp class for the prototype demonstration
/// Created and implemented by Sam Endean - 18/01/16
/// </summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIDemo : MonoBehaviour
{
	enum TempStates {idle, path};
	
	private GameObject startNode, goalNode, currentTrOct;
	//the TrOct face being...faced by the actor
	private Vector3 face;
	private float initPathTimerVal;

	//the time between a new path being searched for
	public float pathTimer = 5f, moveTimer;
	//should the starting pos be random?
	public bool randomStart;

	enum AIDemoStates {Setup, Pathfind};

	AIDemoStates state = AIDemoStates.Setup;

	//will have the top popped off each time
	public List<GameObject> path;
	
	// Update is called once per frame
	void Update ()
	{
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

	void setup()
	{
		moveTimer = 0;
		
		path = new List<GameObject> ();
		
		initPathTimerVal = pathTimer;
		
		//if random start, find a random troct from world list and set as current
		//if (randomStart)
		{
			//do 
			{
				currentTrOct = GameManager.instance.allTrocts[Random.Range(0, GameManager.instance.allTrocts.Count)];
			} //while (currentTrOct.GetComponent<TruncOct>().type != TruncOct.tileType.clear);
		}
		//else
		{
			//set current troct to be the origin troct
			//currentTrOct = aStar.instance.open[0];
		}
		
		transform.position = currentTrOct.transform.position;
		
		face = currentTrOct.GetComponent<TruncOct> ().Faces [1];

		state = AIDemoStates.Pathfind;
	}

	void Pathfind()
	{
		//if path is empty
		if (path.Count <= 1)
		{
			//if pathTimer has hit 0
			//if (pathTimer <= 0)
			{
				//create one by calling A* functions
				//path = aStar.instance.GeneratePath(currentTrOct, );
				
				pathTimer = initPathTimerVal;
			}
			//else //else reduce pathTimer
			{
			//	pathTimer -= Time.deltaTime;
			}
		}
		else //go to the next pathNode
		{
			currentTrOct = path[0];
			
			//if forward is the same direction as the Dir to the next PathNode
			if (transform.forward == (path[1].transform.position - path[0].transform.position).normalized)
			{
				//move forward to it instead
				//move
				if (moveTimer < 1)
				{
					Vector3.Lerp(transform.position, path[1].transform.position, moveTimer);
				}

				moveTimer += Time.deltaTime;

				if (moveTimer == 1)
				{
					moveTimer = 0;
				}
				//pop top of list off
				path.RemoveAt(0);
			}
			else //look at next pathNode
			{
				transform.forward = (path[1].transform.position - path[0].transform.position).normalized;
			}
		}
	}

	void Tick ()
	{
		switch (state)
		{
		case AIDemoStates.Setup:
			setup ();
			break;
		case AIDemoStates.Pathfind:
			Pathfind();
			break;
		}
	}
}
