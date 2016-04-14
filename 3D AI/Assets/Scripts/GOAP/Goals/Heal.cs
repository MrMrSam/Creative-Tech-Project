﻿/// The Heal goal represents agents wanting to be able to repair themselves.
/// Implemented by Sam Endean 17/02/2016
 
using UnityEngine;
using System.Collections;

public class Heal : GoapGoal 
{

	public override void Init (GoapCore _core)
	{
		baseWeight = 0f;

		initPrerequisite = "Heal";

		base.Init(_core);
	}

	public override float calcWeight (GoapWorldstate _actorWorldState)
	{
		float totalWeight = 1 - (core.actor.health / 100f);

		//if not at full health and there are no visible enemies, it may as well heal
		if (core.actor.health != core.actor.maxHealth && _actorWorldState.enemyData.Count == 0)
		{
			baseWeight = 1f;
		}

		return totalWeight;
	}
}
