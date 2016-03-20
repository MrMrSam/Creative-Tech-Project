﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public abstract class GoapAction
{
	public List<string> prerequisites;

	protected GoapCore core;

	public string actionName, 	//
	fulfillment, 				//the prerequisites this will fulfill
	calls; 						//the base function that is used to change the worldstate

	public abstract bool Action(GoapPlan _currentPlan, GoapWorldstate _worldState);

	public abstract bool Test(GoapWorldstate _worldState);

	public abstract void Init (GoapCore _core);


	/// <summary>
	/// Proceeds the along path.
	/// </summary>
	/// <returns><c>true</c>, if progress along the path was made, <c>false</c> otherwise.</returns>
	protected bool ProceedAlongPath(GoapPlan _currentPlan)
	{
		//find the next GO on the currentPath
		GameObject targetTroct = _currentPlan.plannedPath.Value[0];

		Vector3 towardsTarget = (targetTroct.transform.position - core.actor.currentTrOct.transform.position).normalized;

		//is this actor facing the troct?
		if (towardsTarget == core.actor.transform.forward)
		{
			//if so, try move forwards
			if (core.actor.TryMoveForwards())
			{
				_currentPlan.plannedPath.Value.RemoveAt(0);

				//see if the target node has been reached
				if (_currentPlan.plannedPath.Value.Count == 0)
				{
					KeyValuePair<string, List<GameObject>> tempPath = new KeyValuePair<string, List<GameObject>>(_currentPlan.plannedPath.Key, null);
					_currentPlan.plannedPath = tempPath;
				}

				return true; //return a successful progression along the path
			}
			else
			{
				return false; // return an unsuccessful progression along the path
			}
		}
		else //turn to face the next TrOct
		{
			return core.actor.TryRotate(targetTroct.transform); //return the result of attempting to look at the next troct.
		}
	}
}
