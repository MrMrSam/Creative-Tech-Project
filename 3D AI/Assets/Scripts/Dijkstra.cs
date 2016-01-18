using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Dijkstra : MonoBehaviour
{
	List<Node> unvisited, closed, open;

	Node currentNode, startNode, goalNode;

	public List<int> path;

	bool complete;

	public int startNodeNo = int.MinValue, goalNodeNo = int.MinValue, currentNodeNo;

	GameObject worldGen;

	//the number of seconds between each progression/update
	public float timer = 2f;
	//stores the initial timer value
	float timerVal;

	void Start ()
	{
		timerVal = timer;

		worldGen = GameObject.FindGameObjectWithTag("WorldGen");

		closed = new List<Node>();
		open = new List<Node>();

		//only randomly gen if nodes arent set
		if (startNodeNo == int.MinValue)
		{
			Random.seed = System.DateTime.Now.Millisecond;
			startNodeNo = Random.Range(0, worldGen.GetComponent<WorldGen>().truncOcts.Count);
		}
		if (goalNodeNo == int.MinValue)
		{
			Random.seed = System.DateTime.Now.Millisecond;
			goalNodeNo = Random.Range(0, worldGen.GetComponent<WorldGen>().truncOcts.Count);
		}

		//pull list from worldGen and compile list of Nodes
		for (int i = 0; i < worldGen.GetComponent<WorldGen>().truncOcts.Count; i++)
		{
			Node pathNode = new Node();

			pathNode.trOct = worldGen.GetComponent<WorldGen>().truncOcts[i];
			pathNode.tentativeDist = float.PositiveInfinity;

			//check if the current is the start
			if (i == startNodeNo)
			{
				//take pathnode to be current and change colour
				currentNode = pathNode;
				pathNode.tentativeDist = 0;
				pathNode.tentDistNode = null;
				startNode = pathNode;

				currentNode.trOct.GetComponent<TruncOct>().type = TruncOct.tileType.showEnds;
				currentNode.trOct.GetComponent<TruncOct>().ReturnToTypeColour();
			}
			else if ( i == goalNodeNo)
			{
				goalNode = pathNode;
				pathNode.trOct.GetComponent<TruncOct>().type = TruncOct.tileType.showEnds;
				pathNode.trOct.GetComponent<TruncOct>().ReturnToTypeColour();
			}

			open.Add (pathNode);

		}//all nodes compiled and set
	
	}

	void Update ()
	{
		if (!complete)
		{
			timer -= Time.deltaTime;

			//wait for timer*seconds between movements/calculations
			if (timer <= 0f )
			{
				//if the goal node is in the closed list, STOP
				//i goes through the closed list to find the goal node
				for (int i = 0; i < closed.Count; i++)
				{
					if (closed[i] == goalNode)
					{
						path = new List<int>();
						complete = true;

						path.Add(goalNode.trOct.GetComponent<TruncOct>().trOctNo);

						DisplayPath(goalNode);
						//clear search space the path
						RemoveSearchSpace(true);

						return;
					}
				}

				//find the smallest tentative distance on the open list
				float dist = float.MaxValue;
				//i goes through each node in the open list
				for (int i = 0; i < open.Count; i++)
				{
					//if the tentative dist of the node is smaller than previous, 
					if (open[i].tentativeDist < dist)
					{
						currentNode = open[i];
						currentNodeNo = open[i].trOct.GetComponent<TruncOct>().trOctNo;
						dist = open[i].tentativeDist;
					}
				}

				currentNode.trOct.GetComponent<TruncOct>().type = TruncOct.tileType.showSearch;
				currentNode.trOct.GetComponent<TruncOct>().ReturnToTypeColour();

				//calculate the tentative distance between the current node and all of it's neighbors
				//i goes through the connections of the current object
				for (int i = 0; i < 14; i++)
				{
					//check if the connection is marked as "Cannot connect", if so skip to next connection
					if (currentNode.trOct.GetComponent<TruncOct>().connections[i] == TruncOct.connectionState.CantConnect)
					{
						continue;
					}

					//check if the object in this direction is already closed by searching the open list for it, if so skip to next connection
					Node neighbor = null;
					//j goes through the nodes in the open list
					for (int j = 0; j < open.Count; j++)
					{
						if (currentNode.trOct.GetComponent<TruncOct>().connectionObjects[i] == open[j].trOct.GetComponent<TruncOct>().trOctNo)
						{
							neighbor = open[j];
						}
					}
					//if neighbor was not found on the open list, it must be closed, so skip to the next connection.
					if (neighbor == null)
					{
						continue;
					}

					//calculate new tentative distance for the object on this connection, if it is smaller than it's current, replace it
					float newDist = currentNode.trOct.GetComponent<TruncOct>().Faces[i].sqrMagnitude + currentNode.tentativeDist;
					if (newDist < neighbor.tentativeDist)
					{
						neighbor.tentDistNode = currentNode;
						neighbor.tentativeDist = newDist;
					}
				}

				//mark current node as closed and remove from open list
				if (currentNode == startNode || currentNode == goalNode)
				{
					currentNode.trOct.GetComponent<TruncOct>().type = TruncOct.tileType.showEnds;
					currentNode.trOct.GetComponent<TruncOct>().ReturnToTypeColour();
				}

				open.Remove(currentNode);
				closed.Add(currentNode);

				//reset the time
				timer = timerVal;
			}
		}
	}

	//takes the goal node's tentDistNode and moves backwards through them to the start node changing colour to display the path
	void DisplayPath(Node dispNode)
	{
		Node newNode = dispNode.tentDistNode;

		path.Insert(0, newNode.trOct.GetComponent<TruncOct>().trOctNo);

		//if the new node is not the start node
		if (newNode != startNode)
		{
			newNode.trOct.GetComponent<TruncOct>().type = TruncOct.tileType.showPath;
			newNode.trOct.GetComponent<TruncOct>().ReturnToTypeColour();

			//calls the function for said node
			DisplayPath(newNode);
		}
	}

	//this removes the yellow search space by checking the closed list 
	void RemoveSearchSpace(bool _all)
	{
		//i goes through the closed list to find nodes not on the path
		for (int i = 0; i < closed.Count; i++)
		{
			//only clear those not on the path
			if (!_all)
			{
				if (!path.Contains(closed[i].trOct.GetComponent<TruncOct>().trOctNo))
				{
					closed[i].trOct.GetComponent<TruncOct>().type = TruncOct.tileType.clear;
					closed[i].trOct.GetComponent<TruncOct>().ReturnToTypeColour();
				}
			}
			else //clear all objects, even those on the path
			{
				closed[i].trOct.GetComponent<TruncOct>().type = TruncOct.tileType.clear;
				closed[i].trOct.GetComponent<TruncOct>().ReturnToTypeColour();
			}
		}
	}

	class Node
	{
		public GameObject trOct;
		public float tentativeDist;
		public Node tentDistNode;
	}
}
