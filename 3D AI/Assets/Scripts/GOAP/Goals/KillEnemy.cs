using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KillEnemy : GoapGoal
{

	void Start ()
	{
		baseWeight = 0.5f;

		initPrerequisite = "Shoot Enemy";
	}


	public override float calcWeight (GoapWorldstate _actorWorldState)
	{
		float totalWeight = baseWeight;

		//IF THERE IS AN ENEMY VISIBLE, ADD localstrength() ((1 / (localStrength(_actorWorldState))) / 2);

		return totalWeight;
	}

	/// <summary>
	/// calculate the difference in local strength between the friendlies and enemies.
	/// </summary>
	/// <returns>The strength.</returns>
	/// <param name="_actorWorldState">Actor world state.</param>
	private float localStrength (GoapWorldstate _actorWorldState)
	{
		float comparativeStrength = 0f;

		//calculate enemy combat strength (total visible (to this actor) enemy strength within sight range)

		//calculate friendly combat strength (total health of allies within sight range of target)
		return comparativeStrength;
	}
}
