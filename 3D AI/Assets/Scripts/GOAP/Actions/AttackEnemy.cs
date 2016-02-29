using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AttackEnemy : GoapAction
{
	void Start ()
	{
		actionName = "Attack Enemy";
		fulfillment = actionName;

		prerequisites = new List<string>(1) {"Enemy Found"};
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
