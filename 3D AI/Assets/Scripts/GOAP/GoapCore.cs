/// The goal orientated action planning core script that attaches to an actor.
/// Implemented by Sam Endean 16/02/2016

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GoapCore : MonoBehaviour
{
	//all goals this actor has
	List<GoapGoal> goals;

	//the pool of actions avaliable to the actor
	List<GoapAction> actionPool;

	//The current world state as seen by the actor
	GoapWorldstate worldState;

	GoapPlan currentPlan;
	GoapGoal currentGoal;

	public void Tick ()
	{
		if (!currentGoal)
		{
			//if the goal has been set to null, generate a new one.
			currentGoal = SelectGoal (worldState);

			currentPlan = FormulatePlan (currentGoal);
		}

		if (!currentPlan)
		{
			//if there is currently no plan, generate a new one.


		}
	}

	GoapPlan FormulatePlan (GoapGoal _goal)
	{
		GoapPlan newPlan;



		return newPlan;
	}

	GoapGoal SelectGoal (GoapWorldstate _worldState)
	{
		GoapGoal newGoal;


		return newGoal;
	}

}

/// Goap plans hold the formulated plan of the GOAP system.
class GoapPlan
{
	Stack<GoapAction> actionOrder;

	GoapGoal goalBeingFulfilled;
}