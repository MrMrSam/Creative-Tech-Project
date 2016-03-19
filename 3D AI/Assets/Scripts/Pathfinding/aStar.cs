/// <summary>
/// aStar
/// Created and implemented by Sam Endean - 18/01/16
/// </summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class aStar : MonoBehaviour
{
//	public List<GameObject> unvisited, closed, open;
	
	//GameObject currentNode, startNode, goalNode;
	
	public enum Heuristic {Euclidian, ManhattanBig, ManhattanSmall, ManhattanAverage, NoneDijkstra}; 
	
	public Heuristic HeuristicType = Heuristic.Euclidian;
	
	public List<GameObject> path;
	
	public float pathTime;
	
	bool complete;
	
	public int startNodeNo = 0, goalNodeNo = 0, currentNodeNo;
	
	WorldGen worldGen;
	
	//the number of seconds between each progression/update
	public float timer = -1f, resetTimer = 2f;
	//stores the initial timer value
	float timerVal;

	private static aStar m_instance = null;
	public static aStar instance { get { return m_instance; } }
	
	void Awake ()
	{
		//instantiate singleton
		m_instance = this;
	}

	void Start ()
	{
		worldGen = WorldGen.instance;
		//AStarSetup();
	}

	/// <summary>
	/// Sets up the searchSpace for using A*.
	/// </summary>
	/// <returns> the modified searchspace, the startnode, and the end node.</returns>
	/// <param name="_searchSpace">The searchspace of the function.</param>
	/// <param name="_start"> The start node if known, otherwise this is generated.</param>
	/// <param name="_goal">The goal node if known, otherwise this is generated.</param>
	private AStarData AStarSetup (List<GameObject> _searchSpace, GameObject _start, GameObject _goal)
	{
		//List<List<GameObject>> aStarData = new List<List<GameObject>> ();

		GameObject start, goal;
		//if start is not provided, generate one
		if (_start) 
		{
			start = _start;
		}
		else
		{
			start = GenerateRandomNode (_searchSpace);
		}
		//if goal is not provided, generate one
		if (_goal)
		{
			goal = _goal;
		}
		else
		{
			goal = GenerateRandomNode (_searchSpace);
		}

		//set start's cost so far to 0
		start.GetComponent<TruncOct> ().nodeData.tentativeDist = 0;

		//setHeuristics
		_searchSpace = SetHeuristics (_searchSpace, start, goal);

		//wrap start and goal nodes into mono-member lists for packing into aStarData
		List<GameObject> startHolder = new List<GameObject> (1) {start},
		goalHolder = new List<GameObject> (1) {goal};


		AStarData dataHolder = new AStarData ();

		dataHolder.searchSpace = _searchSpace;
		dataHolder.startObject = start;
		dataHolder.goalObject = goal;

		return dataHolder;
	}




	/// <summary>
	/// Sets the heuristics of all trocts in the searchspace.
	/// </summary>
	/// <returns>The searchspace with heuristics set.</returns>
	/// <param name="_searchSpace">The search space.</param>
	/// <param name="_start">The start node.</param>
	/// <param name="_goal">The goal node.</param>
	private List <GameObject> SetHeuristics(List<GameObject> _searchSpace, GameObject _start, GameObject _goal)
	{
		for (int i = 0; i < _searchSpace.Count; i++) 
		{
			if (_searchSpace[i] != _start)
			{
				int steps;

				//reset the tentative distance of the node data of the nodes
				_searchSpace[i].GetComponent<TruncOct>().nodeData.tentativeDist = float.PositiveInfinity;
				
				switch (HeuristicType)
				{
				case Heuristic.Euclidian:
					//(distance from this node in space to the goal node (SQUARE DISTANCE)
					_searchSpace[i].GetComponent<TruncOct>().nodeData.heuristic = (_goal.transform.position - _searchSpace[i].transform.position).sqrMagnitude;
					break;
					
				case Heuristic.ManhattanAverage:
					//6 square faces of 16 = 96
					//8 hexagonal faces of 12 = 96
					float avrgLength = 192/14;
					
					//(distance from this node in space to the goal node (SQUARE DISTANCE)
					steps = (int)((_goal.transform.position - _searchSpace[i].transform.position).sqrMagnitude / avrgLength);
					
					steps = (int)(steps * avrgLength);
					
					_searchSpace[i].GetComponent<TruncOct>().nodeData.heuristic = steps;
					break;
					
				case Heuristic.ManhattanBig:
					//(distance from this node in space to the goal node (SQUARE DISTANCE)
					steps = (int)((_goal.transform.position - _searchSpace[i].transform.position).sqrMagnitude / 16);
					
					steps *= 16;
					
					_searchSpace[i].GetComponent<TruncOct>().nodeData.heuristic = steps;
					break;
					
				case Heuristic.ManhattanSmall:						
					//(distance from this node in space to the goal node (SQUARE DISTANCE)
					steps = (int)((_goal.transform.position - _searchSpace[i].transform.position).sqrMagnitude / 12);
					
					steps *= 12;
					
					_searchSpace[i].GetComponent<TruncOct>().nodeData.heuristic = steps;
					break;
					
				case Heuristic.NoneDijkstra:
					_searchSpace[i].GetComponent<TruncOct>().nodeData.heuristic = 0f;
					break;
				}
			}
		}

		return _searchSpace;
	}


	/// <summary>
	/// Generates a random node from the searchSpace
	/// </summary>
	/// <returns>The random node.</returns>
	/// <param name="_searchSpace">The full search space.</param>
	private GameObject GenerateRandomNode(List<GameObject> _searchSpace)
	{
		int rand;
		//generate random number until not on a dead space 
		do 
		{
			rand = Random.Range(0, _searchSpace.Count);
		} while (_searchSpace[rand].GetComponent<TruncOct>().type != TruncOct.tileType.clear);
		
		return _searchSpace[rand];
	}


	/// <summary>
	/// Called by each actor when needed, generates a path from the currentnode to the supplied goal node, else it generates one
	/// </summary>
	/// <returns>The path</returns>
	/// <param name="_start">The start node</param>
	/// <param name="_goal">The goal node</param>
	public List<GameObject> GeneratePath (GameObject _start, GameObject _goal)
	{
		GameObject start = _start, goal, current;

		//if a goal was supplied, set it as the goal, otherwise generate a random one
		if (_goal != null)
		{
			goal = _goal;
		}
		else
		{
			goal = GenerateRandomNode(GameManager.instance.allTrocts);
		}

		startNodeNo = _start.GetComponent<TruncOct>().trOctNo;

		//setup the world for Astart use
		AStarData asData = AStarSetup(GameManager.instance.allTrocts, _start, null);

		//unpack AstarData into thangs
		List<GameObject>  open, closed, searchSpace = asData.searchSpace;
		start = asData.startObject;
		goal = asData.goalObject;

		//current node is the start
		current = start;

		open = new List<GameObject> ();

		for (int i = 0; i < searchSpace.Count; i++)
		{
			open.Add (searchSpace [i]);
		}

		//before pathfinding occurs, all nodes are still open
		closed = new List<GameObject> ();

		//actual pathfinding begins here
		while (current != goal)
		{
			//calculate the tentative distance between the current node and all of it's neighbors
			//i goes through the connections of the current object
			for (int i = 0; i < 14; i++) {
				//check if the connection is marked as "Cannot connect", if so skip to next connection
				if (current.GetComponent<TruncOct> ().connections [i] == TruncOct.connectionState.CantConnect)
				{
					continue;
				}
				
				//check if the object in this direction is already closed by searching the open list for it, if so skip to next connection
				GameObject neighbor = null;
				//j goes through the nodes in the open list
				for (int j = 0; j < open.Count; j++)
				{
					if (current.GetComponent<TruncOct> ().connectionObjects [i] == open [j].GetComponent<TruncOct> ().trOctNo)
					{
						neighbor = open [j];
					}
				}

				//if neighbor was not found on the open list, it must be closed, so skip to the next connection.
				if (neighbor == null)
				{
					continue;
				}
				
				//calculate new tentative distance for the object on this connection, if it is smaller than it's current, replace it
				float newDist = (current.GetComponent<TruncOct> ().Faces [i].sqrMagnitude * 2) + current.GetComponent<TruncOct>().nodeData.tentativeDist + current.GetComponent<TruncOct>().nodeData.heuristic;
				if (newDist < neighbor.GetComponent<TruncOct>().nodeData.tentativeDist)
				{
					neighbor.GetComponent<TruncOct>().nodeData.tentDistNode = current.GetComponent<TruncOct>().nodeData;
					neighbor.GetComponent<TruncOct>().nodeData.tentativeDist = newDist;
				}
			}

			//find the smallest tentative distance on the open list
			float dist = float.PositiveInfinity;

			//i goes through each node in the open list to find the closest
			for (int i = 0; i < open.Count; i++)
			{
				//if the tentative dist of the node is smaller than previous,
				if ((open [i].GetComponent<TruncOct>().nodeData.tentativeDist + open [i].GetComponent<TruncOct>().nodeData.heuristic) < dist)
				{
					//it becomes the current node
					current = open [i];
					dist = open [i].GetComponent<TruncOct>().nodeData.tentativeDist;
				}
			}

			open.Remove (current);
			closed.Add (current);

		}

		path = new List<GameObject> ();

		path = CompilePath(start, goal, path);

		return path;
	}

	List<GameObject> CompilePath(GameObject _start, GameObject _currentNode, List<GameObject> _list)
	{
		_list.Insert (0, _currentNode);

		//if not at the end of the path
		if (_currentNode != _start)
		{
			//recurse
			_list = (CompilePath (_start, _currentNode.GetComponent<TruncOct> ().nodeData.tentDistNode.gameObject, _list));
		}
		//else return
		return _list;
	}

//	//takes the goal node's tentDistNode and moves backwards through them to the start node changing colour to display the path
//	void DisplayPath(GameObject _dispNode, GameObject _start)
//	{
//		GameObject newNode = _dispNode.GetComponent<TruncOct>().nodeData.tentDistNode.gameObject;
//		
//		path.Insert(0, newNode);
//		
//		//if the new node is not the start node
//		if (newNode != _start)
//		{
//			newNode.GetComponent<TruncOct>().type = TruncOct.tileType.showPath;
//			newNode.GetComponent<TruncOct>().ReturnToTypeColour();
//			
//			//calls the function for said node
//			DisplayPath(newNode, _start);
//		}
//	}
//
//	/// <summary>
//	/// Faces the next TrOct on the path, called by AIActors
//	/// </summary>
	/// <returns>The number of action points used</returns>
	/// <param name="_path">the complete path being followed</param>
	void faceNextPath(List<GameObject> _path, int _pathPos)
	{
		//int actionPoints = 0;

		//find the direction from the current troct to the next, set rotation to 
		Vector3 lookDir = (path[_pathPos + 1].transform.position - path[_pathPos].transform.position).normalized;

		//set look rotation
		transform.rotation.SetLookRotation (lookDir);

		//return ++actionPoints;
	}

	/// <summary>
	/// Moves forwards to the next TrOct in path
	/// </summary>
	/// <returns>The number of action points used</returns>
	/// <param name="_path">the complete path being followed</param>
	//void moveToNextPath(List<GameObject> _path, int _pathPos)
//	{
//		//int actionPoints = 0;
//
//
//		//transform.rotation.SetLookRotation (lookDir);
//	
//		//return ++actionPoints;
//	}

	/// <summary>
	/// Removes the coloured search space.
	/// </summary>
	/// <param name="_searched">The searched space.</param>
	/// <param name="_all">If set to <c>true</c> then remove the path's colour too.</param>
	private void RemoveSearchSpace(List<GameObject> _searched, bool _all)
	{
		//i goes through the closed list to find nodes not on the path
		for (int i = 0; i < _searched.Count; i++)
		{
			if (!_all)
			{
				if (!path.Contains(_searched[i]))
				{
					_searched[i].GetComponent<TruncOct>().type = TruncOct.tileType.clear;
					_searched[i].GetComponent<TruncOct>().ReturnToTypeColour();
				}
			}
			else 
			{
				_searched[i].GetComponent<TruncOct>().type = TruncOct.tileType.clear;
				_searched[i].GetComponent<TruncOct>().ReturnToTypeColour();
			}
		}
	}
}

public struct AStarData
{
	public List<GameObject> searchSpace;
	public GameObject startObject,
	goalObject;
}