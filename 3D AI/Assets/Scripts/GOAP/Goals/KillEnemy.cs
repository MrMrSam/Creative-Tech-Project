﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KillEnemy : GoapGoal
{

	public override void Init (GoapCore _core)
	{
		baseWeight = 0.5f;

		initPrerequisite = "Shoot Enemy";

		base.Init(_core);
	}


	public override float calcWeight (GoapWorldstate _actorWorldState)
	{
		float totalWeight = baseWeight;

		return totalWeight;
	}
}
