/// The Heal goal represents agents wanting to be able to repair themselves.
/// Implemented by Sam Endean 29/02/2016

using UnityEngine;
using System.Collections;

public class Replan : GoapGoal 
{
	public override float calcWeight (GoapWorldstate _actorWorldState)
	{
		return float.MinValue;
	}
}
