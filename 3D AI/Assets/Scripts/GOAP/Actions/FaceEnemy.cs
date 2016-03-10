using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class FaceEnemy : GoapAction
{
	public override void Init (GoapCore _core)
	{
		actionName = "Face Enemy";
		fulfillment = "Facing Enemy";

		prerequisites = new List<string>(1) {"In Line With Enemy"};

		core = _core;
	}

	public override bool Action(GoapPlan _currentPlan, GoapWorldstate _worldState)
	{
		//try to rotate towards the target enemy
		return core.actor.TryRotate(core.actor.targetEnemy.transform);
	}

	//does this action need to be done?
	public override bool Test(GoapWorldstate _worldState)
	{
		//are we currently facing the enemy already?

		if (!core.actor.targetEnemy)
		{
			return true;
		}

		//get the enemy's troct
		TruncOct enemyTroct = core.actor.targetEnemy.currentTrOct.GetComponent<TruncOct>();

		//if the direction towards an enemy is the same as the direction of facing
		if ((enemyTroct.transform.position - core.actor.currentTrOct.transform.position).normalized == core.actor.transform.forward)
		{
			float distance = Vector3.Distance(enemyTroct.transform.position , core.actor.currentTrOct.transform.position);

			for (int j = 0; j < _worldState.allies.Count; j++)
			{
				TruncOct allyTroct = _worldState.allies[j].currentTrOct.GetComponent<TruncOct>();
				//check there are no allies in the way.

				//is the ally in the same direction as the enemy but closer?
				if (((allyTroct.transform.position - core.actor.currentTrOct.transform.position).normalized == core.actor.transform.forward) &&
					Vector3.Distance(allyTroct.transform.position , core.actor.currentTrOct.transform.position) < distance)
				{
					//an ally is in the way, move to a different position
					return true;
				}
				else 
				{
					//no allies in the way and enemy a head
					return false;
				}
			}
		}

		//if here is reached there are no enemies within current sight.
		return true;
	}
}
