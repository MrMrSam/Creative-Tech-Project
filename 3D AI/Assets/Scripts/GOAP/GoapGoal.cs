/// The overarching goal of an actor, this contains prerequisites which must be met by searching in the actionpool

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GoapGoal : MonoBehaviour
{
	public float baseWeight;

	protected GoapCore core;

	//The prerequisite that the goal wants to achieve.
	public string initPrerequisite;

	public void Init (GoapCore _core)
	{
		core = _core;
	}

	public float getWeight()
	{
		return baseWeight;
	}

	/// <summary>
	/// Calculates the total weight of this goal.
	/// </summary>
	/// <returns>The weight.</returns>
	/// <param name="_actorWorldState">Actor world state.</param>
	public virtual float calcWeight (GoapWorldstate _actorWorldState) {return 0f;}
}
