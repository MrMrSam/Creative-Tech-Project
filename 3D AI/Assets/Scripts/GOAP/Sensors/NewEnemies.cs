/// The GoapSensor responsible for detecting an immediate obstruction on the current path (if avaliable)
/// Implemented by Sam Endean 29/02/2016

using UnityEngine;
using System.Collections;

public class NewEnemies : GoapSensor
{
	/// <summary>
	/// compare the fed worldstate to a newly generated one, if they have different enemy data, replan.
	/// </summary>
	/// <param name="_worldState">World state.</param>
	public override bool Sense(GoapWorldstate _worldState)
	{
		bool changeSensed = false;

		GoapWorldstate oldState = _worldState,
		newState = new GoapWorldstate();

		//generate a new worldstate
		newState.generateWorldState(core.actor);

		//if the enemy data is different, the enemies have changed and a new plan should be forged
		if (oldState.enemyData != newState.enemyData)
		{
			//drop the current target enemy
			core.actor.targetEnemy = null;

			changeSensed = true;
		}

		//push the new worldstate into the core
		core.setWorldState(newState);

		//return the verdict
		return changeSensed;
	}
}
