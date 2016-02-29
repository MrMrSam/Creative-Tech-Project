/// The Heal goal represents agents wanting to be able to repair themselves.
/// Implemented by Sam Endean 17/02/2016
 
using UnityEngine;
using System.Collections;

public class Heal : GoapGoal 
{

	void start ()
	{
		baseWeight = 0f;

		initPrerequisite = "Heal";
	}

	public override float calcWeight (GoapWorldstate _actorWorldState)
	{
		float totalWeight = 1 - (core.actor.health / 100f);

		return totalWeight;
	}
}
