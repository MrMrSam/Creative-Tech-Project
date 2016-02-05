//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//
//public class aStarPerUpdate : MonoBehaviour
//{
//	List<Node> unvisited, closed, open;
//	
//	Node currentNode, startNode, goalNode;
//
//	public enum Heuristic {Euclidian, ManhattanBig, ManhattanSmall, ManhattanAverage, NoneDijkstra}; 
//
//	public Heuristic HeuristicType = Heuristic.Euclidian;
//
//	public List<int> path;
//
//	public float pathTime;
//	
//	bool complete;
//	
//	public int startNodeNo = 0, goalNodeNo = 0, currentNodeNo;
//	
//	WorldGen worldGen;
//	
//	//the number of seconds between each progression/update
//	public float timer = -1f, resetTimer = 2f;
//	//stores the initial timer value
//	float timerVal;
//	
//	void Start ()
//	{
//		worldGen = WorldGen.instance;
//
//		AStarSetup ();
//	}
//
//	//sets the system off with a new 
//	public void AStarSetup ()
//	{
//		//if already been completed, set the old goal to the start and gen a new goal AMONG OTHER THINGS
//		if (complete)
//		{
//			//reset all closed nodes
//			RemoveSearchSpace(true);
//
//			startNodeNo = goalNodeNo;
//			
//			goalNodeNo = GenerateRandomNode();
//		}
//
//		pathTime = 0;
//		
//		timerVal = timer;
//		
//		closed = new List<Node>();
//		open = new List<Node>();
//		
//		
//		//only randomly gen if nodes arent set
//		if (startNodeNo == 0) {
//			startNodeNo = GenerateRandomNode ();
//		}
//		if (goalNodeNo == 0) {
//			goalNodeNo = GenerateRandomNode();
//		}
//
//
//		
//		//pull list from worldGen and compile list of Nodes
//		for (int i = 0; i < worldGen.GetComponent<WorldGen>().truncOcts.Count; i++)
//		{
//			Node pathNode = new Node();
//			
//			pathNode.trOct = worldGen.GetComponent<WorldGen>().truncOcts[i];
//			pathNode.tentativeDist = float.PositiveInfinity;
//			
//			//check if the current is the start
//			if (i == startNodeNo)
//			{
//				//take pathnode to be current and change colour
//				currentNode = pathNode;
//				pathNode.tentativeDist = 0;
//				pathNode.tentDistNode = null;
//				startNode = pathNode;
//				
//				currentNode.trOct.GetComponent<TruncOct>().type = TruncOct.tileType.showEnds;
//				currentNode.trOct.GetComponent<TruncOct>().ReturnToTypeColour();
//			}
//			else if ( i == goalNodeNo)
//			{
//				goalNode = pathNode;
//				pathNode.trOct.GetComponent<TruncOct>().type = TruncOct.tileType.showEnds;
//				pathNode.trOct.GetComponent<TruncOct>().ReturnToTypeColour();
//			}
//			
//			open.Add (pathNode);
//			
//		}//all nodes compiled and set
//
//		//calculate and set the heuristic in accordance to the selection distance to the goal node for each node
//		switch (HeuristicType)
//		{
//			//simply the direct 3D distance from the node to the goal
//		case Heuristic.Euclidian:
//			//i goes through each element of open
//			for (int i = 0; i < open.Count; i++)
//			{
//				//if the node is not the start node
//				if (open[i] != startNode)
//				{
//					//(distance from this node in space to the goal node (SQUARE DISTANCE)
//					open[i].heuristic = (goalNode.trOct.transform.position - open[i].trOct.transform.position).sqrMagnitude;
//				}
//			}
//			break;
//			
//			//the manhattan distance to the goal (using longer face distances)
//		case Heuristic.ManhattanBig:
//			//do manhattan by dividing the distance by the longer face distance(squared), then multiply the face distance by the int number 
//			//i goes through each element of open
//			for (int i = 0; i < open.Count; i++)
//			{
//				//if the node is not the start node
//				if (open[i] != startNode)
//				{
//					
//					//(distance from this node in space to the goal node (SQUARE DISTANCE)
//					int steps = (int)((goalNode.trOct.transform.position - open[i].trOct.transform.position).sqrMagnitude / 16);
//					
//					steps *= 16;
//					
//					open[i].heuristic = steps;
//				}
//			}
//			break;
//			
//			//the manhattan distance to the goal (using shorter face distances)
//		case Heuristic.ManhattanSmall:
//			//do manhattan by dividing the distance by the shorter face distance(squared), then multiply the face distance by the int number 
//			//i goes through each element of open
//			for (int i = 0; i < open.Count; i++)
//			{
//				//if the node is not the start node
//				if (open[i] != startNode)
//				{
//					
//					//(distance from this node in space to the goal node (SQUARE DISTANCE)
//					int steps = (int)((goalNode.trOct.transform.position - open[i].trOct.transform.position).sqrMagnitude / 12);
//					
//					steps *= 12;
//					
//					open[i].heuristic = steps;
//				}
//			}
//			break;
//			
//			//manhattan average finds the average distance to another face through the distance and number of said faces
//		case Heuristic.ManhattanAverage:
//			//do manhattan by dividing the distance by the average face distance(squared), then multiply the face distance by the int number 
//			//i goes through each element of open
//			for (int i = 0; i < open.Count; i++)
//			{
//				//if the node is not the start node
//				if (open[i] != startNode)
//				{
//					//6 square faces of 16 = 96
//					//8 hexagonal faces of 12 = 96
//					float avrgLength = 192/14;
//					
//					//(distance from this node in space to the goal node (SQUARE DISTANCE)
//					int steps = (int)((goalNode.trOct.transform.position - open[i].trOct.transform.position).sqrMagnitude / avrgLength);
//					
//					steps = (int)(steps * avrgLength);
//					
//					open[i].heuristic = steps;
//				}
//			}
//			break;
//
//			//No heuristic
//		case Heuristic.NoneDijkstra:
//			for (int i = 0; i < open.Count; i++)
//			{
//				//if the node is not the start node
//				if (open[i] != startNode)
//				{					
//					open[i].heuristic = 0f;
//				}
//			}
//			break;
//		}
//
//		complete = false;
//	}
//
//	//generates a random goal or start node 
//	int GenerateRandomNode()
//	{
//		int rand;
//			//generate random number until not on a dead space 
//		do 
//		{
//			rand = Random.Range(0, worldGen.GetComponent<WorldGen>().truncOcts.Count);
//		} while (worldGen.GetComponent<WorldGen>().truncOcts[rand].GetComponent<TruncOct>().type != TruncOct.tileType.clear);
//
//		return rand;
//	}
//	
//	void Update ()
//	{
//		if (!complete) {
//			pathTime += Time.deltaTime;
//
//			timer -= Time.deltaTime;
//			
//			//wait for timer*seconds between movements/calculations
//			if (timer <= 0f) {				
//				//find the smallest tentative distance on the open list
//				float dist = float.MaxValue;
//				//i goes through each node in the open list
//				for (int i = 0; i < open.Count; i++) {
//					//if the tentative dist of the node is smaller than previous, 
//					if (open [i].tentativeDist + open [i].heuristic < dist) {
//						currentNode = open [i];
//						currentNodeNo = open [i].trOct.GetComponent<TruncOct> ().trOctNo;
//						dist = open [i].tentativeDist;
//					}
//				}
//
//				//if the goal node is the smallest in the open list, STOP
//				if (currentNode == goalNode) {
//					path = new List<int> ();
//					complete = true;
//
//					timer = 0f;
//					
//					path.Add (goalNode.trOct.GetComponent<TruncOct> ().trOctNo);
//					
//					DisplayPath (goalNode);
//					RemoveSearchSpace (false);
//					
//					return;
//				}	
//
//
//				currentNode.trOct.GetComponent<TruncOct> ().type = TruncOct.tileType.showSearch;
//				currentNode.trOct.GetComponent<TruncOct> ().ReturnToTypeColour ();
//				
//				//calculate the tentative distance between the current node and all of it's neighbors
//				//i goes through the connections of the current object
//				for (int i = 0; i < 14; i++) {
//					//check if the connection is marked as "Cannot connect", if so skip to next connection
//					if (currentNode.trOct.GetComponent<TruncOct> ().connections [i] == TruncOct.connectionState.CantConnect) {
//						continue;
//					}
//					
//					//check if the object in this direction is already closed by searching the open list for it, if so skip to next connection
//					Node neighbor = null;
//					//j goes through the nodes in the open list
//					for (int j = 0; j < open.Count; j++) {
//						if (currentNode.trOct.GetComponent<TruncOct> ().connectionObjects [i] == open [j].trOct.GetComponent<TruncOct> ().trOctNo) {
//							neighbor = open [j];
//						}
//					}
//					//if neighbor was not found on the open list, it must be closed, so skip to the next connection.
//					if (neighbor == null) {
//						continue;
//					}
//					
//					//calculate new tentative distance for the object on this connection, if it is smaller than it's current, replace it
//					float newDist = currentNode.trOct.GetComponent<TruncOct> ().Faces [i].sqrMagnitude + currentNode.tentativeDist + currentNode.heuristic;
//					if (newDist < neighbor.tentativeDist) {
//						neighbor.tentDistNode = currentNode;
//						neighbor.tentativeDist = newDist;
//					}
//				}
//				
//				//mark current node as closed and remove from open list
//				if (currentNode == startNode || currentNode == goalNode) {
//					currentNode.trOct.GetComponent<TruncOct> ().type = TruncOct.tileType.showEnds;
//					currentNode.trOct.GetComponent<TruncOct> ().ReturnToTypeColour ();
//				}
//				
//				open.Remove (currentNode);
//				closed.Add (currentNode);
//				
//				//reset the time
//				timer = timerVal;
//			}
//		}
//		else //countdown a number of seconds until the next Pathfinding
//		{
//			timer += Time.deltaTime;
//
//			if (timer >= resetTimer)
//			{
//				timer = 0;
//
//				AStarSetup();
//			}
//		}
//
//
//	}
//	
//	//takes the goal node's tentDistNode and moves backwards through them to the start node changing colour to display the path
//	void DisplayPath(Node dispNode)
//	{
//		Node newNode = dispNode.tentDistNode;
//		
//		path.Insert(0, newNode.trOct.GetComponent<TruncOct>().trOctNo);
//		
//		//if the new node is not the start node
//		if (newNode != startNode)
//		{
//			newNode.trOct.GetComponent<TruncOct>().type = TruncOct.tileType.showPath;
//			newNode.trOct.GetComponent<TruncOct>().ReturnToTypeColour();
//			
//			//calls the function for said node
//			DisplayPath(newNode);
//		}
//	}
//	
//	//this removes the yellow search space by checking the closed list 
//	void RemoveSearchSpace(bool _all)
//	{
//		//i goes through the closed list to find nodes not on the path
//		for (int i = 0; i < closed.Count; i++)
//		{
//			if (!_all)
//			{
//				if (!path.Contains(closed[i].trOct.GetComponent<TruncOct>().trOctNo))
//				{
//					closed[i].trOct.GetComponent<TruncOct>().type = TruncOct.tileType.clear;
//					closed[i].trOct.GetComponent<TruncOct>().ReturnToTypeColour();
//				}
//			}
//			else 
//			{
//				closed[i].trOct.GetComponent<TruncOct>().type = TruncOct.tileType.clear;
//				closed[i].trOct.GetComponent<TruncOct>().ReturnToTypeColour();
//			}
//		}
//	}
//	
//	class Node
//	{
//		public GameObject trOct;
//		public float tentativeDist;
//		public Node tentDistNode;
//		public float heuristic;
//	}
//}
