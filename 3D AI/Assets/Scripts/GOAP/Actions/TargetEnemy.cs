using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class TargetEnemy : GoapAction
{
	public override void Init (GoapCore _core)
	{
		actionName = "Target Enemy";
		fulfillment = "Enemy Targeted";

		prerequisites = new List<string>(1) {"Enemy Found"};

		core = _core;
	}

	public override bool Action(GoapWorldstate _worldState)
	{
		//target a visible enemy

		//if there is only one enemy visible, target it
		if (_worldState.enemyData.Count == 1)
		{
			core.actor.targetEnemy = _worldState.enemyData[0].enemy;
		}
		else //if there are more, choose the closest
		{
			ActorBase closest = null;
			float closestDistance = float.PositiveInfinity;

			foreach (EnemyPosition _enemyData in _worldState.enemyData)
			{
				float testDistance = Vector3.Distance(core.actor.transform.position, _enemyData.enemy.transform.position);

				if (testDistance < closestDistance)
				{
					closest = _enemyData.enemy;
					closestDistance = testDistance;
				}
			}
			core.actor.targetEnemy = closest;
		}
		return true;
	}

	//does this action need to be done.
	public override bool Test(GoapWorldstate _worldState)
	{
		//if there is not a target enemy
		if (core.actor.targetEnemy == null)
		{
			//the we need to target one
			return true;
		}

		//there is already a target
		return false;
	}
}
