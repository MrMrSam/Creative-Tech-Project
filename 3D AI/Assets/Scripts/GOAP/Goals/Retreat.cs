/// The Retreal goal represents agents wanting to escape the current area towards (hopeful) safety.
/// Implemented by Sam Endean 17/03/2016

using UnityEngine;
using System.Collections;

public class Retreat : GoapGoal 
{

	public override void Init (GoapCore _core)
	{
		baseWeight = 0f;

		initPrerequisite = "Reach Safety";

		base.Init(_core);
	}

	public override float calcWeight (GoapWorldstate _actorWorldState)
	{
		float totalWeight = 1 - _actorWorldState.ComparePresence(core.actor);

		return totalWeight;
	}
}
