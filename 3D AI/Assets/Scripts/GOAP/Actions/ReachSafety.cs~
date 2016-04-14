using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class ReachSafety : GoapAction
{
	public override void Init (GoapCore _core)
	{
		actionName = "Reach Safety";
		fulfillment = actionName;

		prerequisites = new List<string>();

		core = _core;
	}

	/// <summary>
	/// Carry out the GOAP action, in this case, move to a safer area.
	/// </summary>
	/// <param name="_currentPlan">Current plan.</param>
	/// <param name="_worldState">World state.</param>
	public override bool Action(GoapPlan _currentPlan, GoapWorldstate _worldState)
	{
		GameObject goalNode = null;

		//if the path has not already been made
		if (_currentPlan.plannedPath.Value == null || _currentPlan.plannedPath.Key != actionName)
		{
			KeyValuePair<string, List<GameObject>> tempPath = new KeyValuePair<string, List<GameObject>>(actionName, _currentPlan.plannedPath.Value);
			_currentPlan.plannedPath = tempPath;

			List<TruncOct> inLineTrocts = new List<TruncOct>();

			//compile a list of trocts from all visible enemies
			for (int i = 0; i < _worldState.enemyData.Count; i++)
			{
				TruncOct enemyTroct = _worldState.enemyData[i].enemyLocation;

				//List all trocts leading from enemy location
				//loop through all faces of the troct
				for (int j = 0; j < enemyTroct.Faces.Count; j++)
				{
					//if the current face has no connection, continue to the next face.
					if (enemyTroct.connections[j] != TruncOct.connectionState.Connected)
					{
						continue;
					}
					else //else it it connected and should be pushed as far as it can go outwards
					{
						//get the next troct
						TruncOct newTroct = _worldState.topology[enemyTroct.connectionObjects[j]].GetComponent<TruncOct>();

						List<TruncOct> direction = new List<TruncOct>();
						direction.Add(newTroct);

						inLineTrocts.AddRange (TroctsInDirection(direction, j, newTroct, _worldState));
					}
				}
			}

			//find the closest troct in line that is not in line with the enemy
			List<TruncOct> inLineSelf = new List<TruncOct>();

			TruncOct thisTroct = core.actor.currentTrOct.GetComponent<TruncOct>();

			//loop through all faces of the troct
			for (int i = 0; i < thisTroct.Faces.Count; i++)
			{
				//if the current face has no connection, continue to the next face.
				if (thisTroct.connections[i] != TruncOct.connectionState.Connected)
				{
					continue;
				}
				else //else it it connected and should be pushed as far as it can go outwards
				{
					//get the next troct
					TruncOct newTroct = _worldState.topology[thisTroct.connectionObjects[i]].GetComponent<TruncOct>();

					List<TruncOct> direction = new List<TruncOct>();
					direction.Add(newTroct);

					inLineSelf.AddRange (TroctsInDirection(direction, i, newTroct, _worldState));
				}
			}

			TruncOct targetTroct = null;

			//compare to find a troct that is not inline with an enemy
			for (int i = 0; i < inLineSelf.Count; i++)
			{
				if (!inLineTrocts.Contains(inLineSelf[i]))
				{
					targetTroct = inLineSelf[i];
					break;
				}			
			}

			//if a new location still needs to be found
			if (targetTroct == null)
			{
				//else find a close troct that is not in line
				List<TruncOct> closeTrocts = new List<TruncOct>();

				foreach(GameObject _tObject in GameManager.instance.allTrocts)
				{
					if (Vector3.Distance(core.actor.transform.position, _tObject.transform.position) <= (core.actor.viewDistance / 1.5f))
					{
						if (!inLineTrocts.Contains(_tObject.GetComponent<TruncOct>()))
						{
							targetTroct = _tObject.GetComponent<TruncOct>();
						}
					}
				}

				//if a close one cant be found, just generate a random one by force, we must move!
				while ((!inLineTrocts.Contains(targetTroct) && targetTroct == null) || targetTroct == null)
				{
					TruncOct rand = GameManager.instance.allTrocts[Random.Range(0, GameManager.instance.allTrocts.Count)].GetComponent<TruncOct>();

					if (!inLineTrocts.Contains(rand))
					{
						targetTroct = rand;
						break;
					}
				}
			}

			goalNode = targetTroct.gameObject;

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
		//first check if there are any enemies at all
		if (_worldState.enemyData.Count == 0)
		{
			return false;
		}

		bool farOut = true,
		inLine = false;
		//now if the visible enemies are all out of sight range
		for (int i = 0; i < _worldState.enemyData.Count; i++)
		{
			if (Vector3.Distance(_worldState.enemyData[i].enemy.transform.position, core.actor.transform.position) < core.actor.viewDistance)
			{
				farOut = false;
			}
			else //check if in line
			{
				//go through each face of the enemy troct
				foreach (Vector3 _facing in _worldState.enemyData[i].enemyLocation.Faces)
				{
					//if the direction lines up then it must be in line
					if ((_worldState.enemyData[i].enemyLocation.transform.position - core.actor.currentTrOct.transform.position).normalized == _facing.normalized)
					{
						//we are in line
						inLine = true;
					}
				}
			}
			//if the visible enemies are out of sight range, it is safe.
			if (farOut == true)
			{
				return false;
			}
		}

		//now check if an enemy is in line with us 
		if (inLine != true)
		{
			return false;
		}

		return true;
	}
}
