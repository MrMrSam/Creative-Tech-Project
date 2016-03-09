using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AttackEnemy : GoapAction
{
	public override void Init (GoapCore _core)
	{
		actionName = "Attack Enemy";
		fulfillment = actionName;

		prerequisites = new List<string>(1) {"Enemy Found"};

		core = _core;
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public override bool Action(GoapWorldstate _worldState)
	{
		return false;
	}

	//does this action need to be done.
	public override bool  Test(GoapWorldstate _worldState)
	{

		return false;

	}
}
