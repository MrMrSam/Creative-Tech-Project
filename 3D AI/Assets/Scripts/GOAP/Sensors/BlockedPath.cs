/// The GoapSensor responsible for detecting an immediate obstruction on the current path (if avaliable)
/// Implemented by Sam Endean 29/02/2016

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlockedPath : GoapSensor
{
	/// <summary>
	/// If the actor is facing the next troct on the path and the path is obstructed and the current actor has action points left, replan
	/// </summary>
	/// <param name="_worldState">World state.</param>
	public override bool Sense(GoapWorldstate _worldState)
	{
		//make a copy of the current path
		List<GameObject> path = core.actor.currentPath;

		//if the path is empty or we are at the end of the path, return false as no replanning has to be done as a result of the sensed state
		if (path.Count == 0 || core.actor.progressAlongPath == path.Count)
		{
			return false;
		}

		//find the next GO on the currentPath
		GameObject targetTroct = path[core.actor.progressAlongPath + 1];

		Vector3 towardsTarget = (targetTroct.transform.position - core.actor.currentTrOct.transform.position).normalized;

		//is this actor facing the troct?
		if (towardsTarget == core.actor.transform.forward)
		{
			//is the target troct occupied?
			if (targetTroct.GetComponent<TruncOct>().containedActor != null)
			{
				//if so, replan
				return true;
			}
		}

		return false;
	}
}
