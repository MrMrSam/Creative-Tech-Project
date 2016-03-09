using UnityEngine;
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

	public abstract bool Action(GoapWorldstate _worldState);

	public abstract bool Test(GoapWorldstate _worldState);

	public abstract void Init (GoapCore _core);


	/// <summary>
	/// Proceeds the along path.
	/// </summary>
	/// <returns><c>true</c>, if progress along the path was made, <c>false</c> otherwise.</returns>
	protected bool ProceedAlongPath()
	{
		//make a copy of the current path
		List<GameObject> path = core.actor.currentPath;

		//find the next GO on the currentPath
		GameObject targetTroct = path[core.actor.progressAlongPath + 1];

		Vector3 towardsTarget = (targetTroct.transform.position - core.actor.currentTrOct.transform.position).normalized;

		//is this actor facing the troct?
		if (towardsTarget == core.actor.transform.forward)
		{
			//if so, try move forwards
			if (core.actor.TryMoveForwards())
			{
				core.actor.progressAlongPath ++;
				//see if the target node has been reached
				if (core.actor.currentTrOct == core.actor.goalNode)
				{
					core.actor.currentPath.Clear();
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
