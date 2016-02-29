using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShootEnemy : GoapAction
{
	void Start ()
	{
		actionName = "Shoot Enemy";
		fulfillment = actionName;

		prerequisites = new List<string>(2) {"Enemy Found", "Facing Enemy"};
	}

	public override bool Action(GoapWorldstate _worldState)
	{
		//SHOOT FORWARDS
		return core.actor.Shootforwards();
	}

	//does this action need to be done.
	public override bool Test(GoapWorldstate _worldState)
	{
		//this is the end action so it always need to be attempted.
		return true;
	}
}
