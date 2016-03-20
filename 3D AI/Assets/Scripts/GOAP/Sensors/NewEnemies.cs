﻿/// The GoapSensor responsible for detecting an immediate obstruction on the current path (if avaliable)
/// Implemented by Sam Endean 29/02/2016

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NewEnemies : GoapSensor
{
	/// <summary>
	/// compare the fed worldstate to a newly generated one, if they have different enemy data, replan.
	/// </summary>
	/// <param name="_worldState">World state.</param>
	public override bool Sense(GoapPlan _currentPlan, GoapWorldstate _worldState)
	{
		bool changeSensed = false;

		GoapWorldstate oldState = _worldState,
		newState = new GoapWorldstate();

		//generate a new worldstate
		newState.generateWorldState(core.actor);

		//if the enemy data is different, the enemies have changed and a new plan should be forged
		if (CompareEnemyData(oldState.enemyData, newState.enemyData))
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

	/// <summary>
	/// Compares the enemy data to determine equivilancy.
	/// </summary>
	/// <returns><c>true</c>, if enemy data was different, <c>false</c> otherwise.</returns>
	/// <param name="_oldEnemyData">Old enemy data.</param>
	/// <param name="_newEnemyData">New enemy data.</param>
	private bool CompareEnemyData (List<EnemyPosition> _oldEnemyData, List<EnemyPosition> _newEnemyData)
	{
		//if the count if different then return a difference
		if (_oldEnemyData.Count != _newEnemyData.Count)
		{
			return true;
		}

		//if an enemy entered and another left the counts may be the same but with different enemies, necessitating another comparison
		for (int i = 0; i < _oldEnemyData.Count; i++)
		{
			if (_oldEnemyData[i].enemy != _newEnemyData[i].enemy)
			{
				return true;
			}
		}

		//if this is reached, they are equal
		return false;
	}


}
