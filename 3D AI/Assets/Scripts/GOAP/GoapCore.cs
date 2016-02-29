/// The goal orientated action planning core script that attaches to an actor.
/// Implemented by Sam Endean 16/02/2016

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GoapCore : MonoBehaviour
{
	//all goals this actor has
	private List<GoapGoal> goals;

	public AIActor actor;

	//the pool of actions avaliable to the actor
	private List<GoapAction> actionPool;

	//the collection of sensors belonging to this object
	private NewEnemies enemySensor;
	private BlockedPath obstructionSensor;

	//The current world state as seen by the actor
	private GoapWorldstate worldState;

	private GoapPlan currentPlan;
	private GoapGoal currentGoal;

	public GoapWorldstate getworldState()
	{
		return worldState;
	}

	void Start ()
	{
		//initialise goals
		goals = new List<GoapGoal> ();
		goals.Add (new KillEnemy ());
		goals.Add (new Heal ());
		foreach (GoapGoal _goal in goals)
		{
			_goal.Init(this);
		}

		//initialise actions
		actionPool = new List<GoapAction> ();
		actionPool.Add (new FaceEnemy ());
		actionPool.Add (new FindEnemy ());
		actionPool.Add (new LineUpShot ());
		actionPool.Add (new ShootEnemy ());
		foreach (GoapAction _action in actionPool)
		{
			_action.Init(this);
		}

		//initialise sensors
		enemySensor = new NewEnemies();
		enemySensor.Init (this);

		obstructionSensor = new BlockedPath ();
		obstructionSensor.Init (this);



	}

	public void Tick ()
	{
		worldState = UpdateWorldstate();

		//sort the goal priorities for this turn.
		GoapGoal tempGoal = SortGoals (worldState);

		//if the goal has changed, or the sensors demand it, a new plan must be formulated
		if (tempGoal != currentGoal || currentPlan == null)
		{
			currentGoal = tempGoal;

			currentPlan = FormulatePlan(currentGoal, worldState);
		}

		currentPlan = ExecutePlan (currentPlan, worldState);
	}


	private GoapPlan ExecutePlan(GoapPlan _currentPlan, GoapWorldstate _worldState)
	{
		bool sensorInterruption = false,
		actionSuccess = true;

		//try to carry out the plan until an action attempt fails (or sensors interrupt)
		while (actionSuccess == true)
		{
			//if the action on the top of the stack needs to be done
			if (_currentPlan.actionOrder.Peek ().Test (_worldState))
			{
				//try to carry it out and have it set the result of actionSuccess depending on a success or failure
				actionSuccess = _currentPlan.actionOrder.Peek().Action(_worldState);

				if (actionSuccess) //if an action was successfully done, call the sensors to check things
				{
					_currentPlan = CheckSensors (_currentPlan, getworldState ());
				}
			}
			else//remove it from the plan, as it doesnt need to be carried out
			{
				_currentPlan.actionOrder.Pop ();
			}
		}

		//return the plan now that it has been processed and attempted
		return _currentPlan;
	}

	/// <summary>
	/// Calls the sensors one at a time in order to see if the plan needs changing in accordance to worldstate changes
	/// </summary>
	/// <returns>The altered goal if the sensors call for a replan</returns>
	/// <param name="">.</param>
	private GoapPlan CheckSensors (GoapPlan _currentPlan, GoapWorldstate _worldState)
	{
		//loop through sensors


		return null;

		//if all is fine, return the plan back as it was, it is fine.
		return _currentPlan;
	}

	/// <summary>
	/// Formulates a plan from the given goal and current actor state.
	/// </summary>
	/// <returns>The plan.</returns>
	/// <param name="_goal">Goal.</param>
	/// <param name="_worldstate">Worldstate.</param>
	GoapPlan FormulatePlan (GoapGoal _goal, GoapWorldstate _worldstate)
	{
		GoapPlan newPlan = new GoapPlan();

		newPlan.actionOrder = new Stack<GoapAction> ();

		//according to the goal form an initial prerequisite of what we want to do
		newPlan.goalBeingFulfilled = _goal;

		string initPrereq = _goal.initPrerequisite;

		//add the initial action onto the stack to start constructing the plan
		foreach (GoapAction _action in actionPool)
		{
			if (_action.fulfillment == initPrereq)
			{
				newPlan.actionOrder.Push (_action);
				break;
			}
		}

		//loop through all action fulfillments and finds one to fulfill the last action pushed to the stack
		List<String> actionsRequired = newPlan.actionOrder.Peek().prerequisites,
		actionsFulfilled = new List<string>();

		//while there are still prerequisites that need to be accounted for:
		while (!CompareRequiredAndFulfilled(actionsRequired, actionsFulfilled))
		{
			//find the action to fulfill the first action encountered wich does not have a fulfillment
			foreach (string _prereq in actionsRequired)
			{
				if (!actionsFulfilled.Contains (_prereq))
				{
					//find an action that fulfills this from the pool and add it to the stack
					foreach (GoapAction _action in actionPool)
					{
						if (_action.fulfillment == _prereq)
						{
							newPlan.actionOrder.Push (_action);

							//add the prereqs + fulfillments of this action to the lists respectively
							actionsRequired.AddRange(_action.prerequisites);
							actionsFulfilled.Add (_action.fulfillment);
						}
					}
				}
			}
		}

		return newPlan;
	}

	/// <summary>
	/// Compares the required and fulfilled lists added, seeing if they match up.
	/// </summary>
	/// <returns><c>true</c>, if required and fulfilled was compared, <c>false</c> otherwise.</returns>
	/// <param name="_req">Req.</param>
	/// <param name="_ful">Ful.</param>
	private bool CompareRequiredAndFulfilled (List<String> _req, List<String> _ful)
	{
		List<String> accountedFor = new List<string> ();

		//go through _req and see if each is fulfilled by something from _ful
		foreach (string _prereq in _req)
		{
			//if already accounted for, there is no point checking
			if (!accountedFor.Contains (_prereq))
			{
				accountedFor.Add (_prereq);

				if (!_ful.Contains (_prereq))
				{
					return false;
				}
			}
		}

		//if here is reached, then all _reqs were accounted for in _ful
		return true;
	}


	private GoapGoal SortGoals (GoapWorldstate _worldState)
	{
		GoapGoal newGoal = new GoapGoal();
		float highestWeight = float.MinValue,
		testWeight;

		for (int i = 0; i < goals.Count; i++)
		{
			testWeight = goals[i].calcWeight(worldState);

			if (testWeight > highestWeight)
			{
				highestWeight = testWeight;
				newGoal = goals[i];
			}
		}

		return newGoal;
	}

	/// <summary>
	/// Updates the worldstate.
	/// </summary>
	/// <returns>The new worldstate.</returns>
	private GoapWorldstate UpdateWorldstate ()
	{
		GoapWorldstate newWorldState = new GoapWorldstate();

		newWorldState.generateWorldState (actor);

		return worldState;
	}

}

/// Goap plans hold the formulated plan of the GOAP system.
class GoapPlan
{
	public Stack<GoapAction> actionOrder;

	public GoapGoal goalBeingFulfilled;
}