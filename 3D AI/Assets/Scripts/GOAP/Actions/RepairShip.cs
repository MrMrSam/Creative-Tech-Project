using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RepairShip : GoapAction
{
	public override void Init (GoapCore _core)
	{
		actionName = "Repair Ship";
		fulfillment = "Heal";

		prerequisites = new List<string>(1) {"Reach Safety"};

		core = _core;
	}

	public override bool Action(GoapPlan _currentPlan, GoapWorldstate _worldState)
	{
		//if we have 2 action points left, heal otherwise remove last action points and continue
		if (!core.actor.TryToRepair())
		{
			core.actor.actionPoints = 0;
			return false;
		}

		return true;
	}

	//does this action need to be done.
	public override bool Test(GoapWorldstate _worldState)
	{
		//this is the end action so it always need to be attempted.
		return true;
	}
}
