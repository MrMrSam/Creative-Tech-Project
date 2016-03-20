﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class LineUpShot : GoapAction
{
	public override void Init (GoapCore _core)
	{
		actionName = "Line Up Shot";
		fulfillment = "In Line With Enemy";

		prerequisites = new List<string>(1) {"Enemy Targeted"};

		core = _core;
	}

	public override bool Action(GoapPlan _currentPlan, GoapWorldstate _worldState)
	{
		GameObject goalNode = null;

		//if the path has not already been made
		if (_currentPlan.plannedPath.Value == null || _currentPlan.plannedPath.Key != actionName)
		{
			KeyValuePair<string, List<GameObject>> tempPath = new KeyValuePair<string, List<GameObject>>(actionName, _currentPlan.plannedPath.Value);
			_currentPlan.plannedPath = tempPath;

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

					goalNode = closestTroct.gameObject;
				}
			}
		
			//now plot a route to A* pathfind to
			_currentPlan.plotRoute(core.actor, core.actor.currentTrOct, goalNode);
		}

		//try to follow the A* path
		return ProceedAlongPath(_currentPlan);
	}

	private List<TruncOct> TroctsInDirection(List<TruncOct> _troctsInLine, int _facing, TruncOct _rootTroct, GoapWorldstate _worldState)
	{
		//see if the facing is connected
		if (_rootTroct.connections[_facing] != TruncOct.connectionState.Connected)
		{
			return _troctsInLine;
		}
		else //there is a connection
		{
			TruncOct newTroct = _worldState.topology[_rootTroct.connectionObjects[_facing]].GetComponent<TruncOct>();
			_troctsInLine.Add(newTroct);

			_troctsInLine = (TroctsInDirection(_troctsInLine, _facing, newTroct, _worldState));
		}

		return _troctsInLine;		
	}


	//does this action need to be done.
	public override bool Test(GoapWorldstate _worldState)
	{
		TruncOct currentTroct = core.actor.currentTrOct.GetComponent<TruncOct>();

		//are we already in line with the target?

		if (!core.actor.targetEnemy)
		{
			return true;
		}

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
