using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class LineUpShot : GoapAction
{
	void Start ()
	{
		actionName = "Line Up Shot";
		fulfillment = "In line with enemy";

		prerequisites = new List<string>(1) {"Enemy Found"};
	}

	public override bool Action(GoapWorldstate _worldState)
	{
		//if the path has not already been made
		if (core.actor.currentPath.Count == 0)
		{
			TruncOct enemyTroct= core.actor.targetEnemy.currentTrOct.GetComponent<TruncOct>();

			List<TruncOct> inLineTrocts = new List<TruncOct>();

			//List all trocts leading from enemy location
			//loop through all faces of the troct
			for (int i = 0; i < enemyTroct.Faces.Count; i++)
			{
				//if the current face has no connection, continue to the next face.
				if (enemyTroct.connections[i] != TruncOct.connectionState.Connected)
				{
					continue;
				}
				else //else it it connected and should be pushed as far as it can go outwards
				{
					//get the next troct
					TruncOct newTroct = _worldState.topology[enemyTroct.connectionObjects[i]].GetComponent<TruncOct>();

					List<TruncOct> direction = new List<TruncOct>();
					direction.Add(newTroct);

					inLineTrocts.AddRange (TroctsInDirection(direction, i, newTroct, _worldState));
				}
			}

			//find the closest one that is avaliable
			float closestDistance = float.MaxValue;
			TruncOct closestTroct;

			for (int i = 0; i < inLineTrocts.Count; i++)
			{
				float tempDist = Vector3.Distance(core.actor.currentTrOct.transform.position, inLineTrocts[i].transform.position);
				if (tempDist < closestDistance)
				{
					closestDistance = tempDist;
					closestTroct = inLineTrocts[i];

					core.actor.goalNode = closestTroct.gameObject;
				}
			}
		
			//now plot a route to A* pathfind to
			core.actor.plotRoute();
		}

		//try to follow the A* path
		return ProceedAlongPath();
	}

	private List<TruncOct> TroctsInDirection(List<TruncOct> _troctsInLine, int _facing, TruncOct _rootTroct, GoapWorldstate _worldState)
	{
		//filter the list so that trocts are only added once
		List<TruncOct> filterList = _troctsInLine;

		//see if the facing is connected
		if (_rootTroct.connections[_facing] != TruncOct.connectionState.Connected)
		{
			return filterList;
		}
		else //there is a connection
		{
			TruncOct newTroct = _worldState.topology[_rootTroct.connectionObjects[_facing]].GetComponent<TruncOct>();
			_troctsInLine.Add(newTroct);

			_troctsInLine.AddRange (TroctsInDirection(_troctsInLine, _facing, newTroct, _worldState));
		}

		for (int i = 0; i < _troctsInLine.Count; i++)
		{
			if (!filterList.Contains(_troctsInLine[i]))
			{
				filterList.Add(_troctsInLine[i]);
			}
		}

		return filterList;		
	}


	//does this action need to be done.
	public override bool Test(GoapWorldstate _worldState)
	{
		TruncOct currentTroct = core.actor.currentTrOct.GetComponent<TruncOct>();

		//are we already in line with the target?

		//do any enemy directions line up with the current troct faces?
		TruncOct enemyTroct = core.actor.targetEnemy.currentTrOct.GetComponent<TruncOct>();

		foreach (Vector3 _facing in currentTroct.Faces)
		{

			if ((enemyTroct.transform.position - currentTroct.transform.position).normalized == _facing.normalized)
			{
				//they line up and so we do not need to line up a shot
				return false;
			}
		}

		return true;
	}
}
