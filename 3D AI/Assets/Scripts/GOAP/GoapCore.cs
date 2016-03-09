﻿/// The goal orientated action planning core script that attaches to an actor.
/// Implemented by Sam Endean 16/02/2016

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GoapCore
{
	//all goals this actor has
	private List<GoapGoal> goals;

	private List<GoapSensor> sensors;

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
	public void setWorldState(GoapWorldstate _worldState)
	{
		worldState = _worldState;
	}

	public void Init (AIActor _actor)
	{
		actor = _actor;

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
		actionPool.Add (new TargetEnemy ());
		foreach (GoapAction _action in actionPool)
		{
			_action.Init(this);
		}

		//initialise sensors
		enemySensor = new NewEnemies();
		enemySensor.Init (this);

		obstructionSensor = new BlockedPath ();
		obstructionSensor.Init (this);
		sensors = new List<GoapSensor>();
		sensors.Add(obstructionSensor);
		sensors.Add(enemySensor);
	}

	public void TakeTurn ()
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

		//loop the plan execution until all 
		while (actor.actionPoints > 0)
		{
			//process and execute the plan, setting it to be the new plan
			currentPlan = ExecutePlan (currentPlan, worldState);
		}
	}


	private GoapPlan ExecutePlan(GoapPlan _currentPlan, GoapWorldstate _worldState)
	{
		bool actionSuccess = true;

		//try to carry out the plan until an action attempt fails (or sensors interrupt)
		while (actionSuccess == true)
		{
			//if the action on the top of the stack needs to be done
			if (_currentPlan.actionOrder.Peek ().Test (_worldState))
			{
				//run sensors
				GoapPlan tempPlan = CheckSensors(_currentPlan, _worldState);

				//if the plans differ
				if (!ComparePlans(_currentPlan, tempPlan))
				{
					return tempPlan;
				}

				//try to carry it out and have it set the result of actionSuccess depending on a success or failure
				actionSuccess = _currentPlan.actionOrder.Peek().Action(_worldState);

				if (actionSuccess) //if an action was successfully done, call the sensors to check things
				{
					_currentPlan.actionOrder.Pop ();

					tempPlan = CheckSensors (_currentPlan, getworldState ());

					if (!ComparePlans(_currentPlan, tempPlan))
					{
						return tempPlan;
					}
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

		foreach (GoapSensor _sensor in sensors)
		{
			//if a replan is needed according to the current sensor
			if (_sensor.Sense(_worldState))
			{
				//resort goals, and from this formulate a new plan
				return FormulatePlan(SortGoals(_worldState), _worldState);
			}
		}
		//if all is fine, return the plan back as it was, it is fine.
		return _currentPlan;
	}

	/// <summary>
	/// Compares to see if plans are equal from their planning data (as the reference to the core will always flag them as different otherwise).
	/// </summary>
	/// <returns><c>true</c>, if plans are the same <c>false</c> otherwise.</returns>
	/// <param name="_planA">Plan a.</param>
	/// <param name="_planB">Plan b.</param>
	private bool ComparePlans(GoapPlan _planA, GoapPlan _planB)
	{
		//if the goals differ
		if (_planA.goalBeingFulfilled != _planB.goalBeingFulfilled)
		{
			//return stating this difference
			return false;
		}

		GoapAction[] tempA = _planA.actionOrder.ToArray(),
		tempB = _planB.actionOrder.ToArray();

		for (int i = 0; i < tempA.Length; i++)
		{
			if (tempA[i] != tempB[i])
			{
				return false;
			}
		}

		//else they must be the same
		return true;
	}


	/// <summary>
	/// Formulates a plan from the given goal and current actor state.
	/// </summary>
	/// <returns>The plan.</returns>
	/// <param name="_goal">Goal.</param>
	/// <param name="_worldstate">Worldstate.</param>
	private GoapPlan FormulatePlan (GoapGoal _goal, GoapWorldstate _worldstate)
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

		List<string> prerequisites = newPlan.actionOrder.Peek ().prerequisites;

		newPlan.actionOrder = FulfillPrereqs (newPlan.actionOrder, prerequisites, _worldstate);

		return newPlan;
	}


	private Stack<GoapAction> FulfillPrereqs (Stack<GoapAction> _currentActionList, List<string> _currentPrerequisites, GoapWorldstate _worldstate)
	{
		foreach (string _prereq in _currentPrerequisites)
		{
			//find the item to fulfill the current prereq
			foreach (GoapAction _action in actionPool)
			{
				if (_action.fulfillment == _prereq)
				{
					//if the action does not need to be done, no more need to be added, so just return the actionlist as is
					if (!_action.Test (_worldstate))
					{
						return _currentActionList;
					}
					else
					{
						_currentActionList.Push (_action);

						List<string> prereqs = _action.prerequisites;

						_currentActionList = FulfillPrereqs (_currentActionList, prereqs, _worldstate);
					}
				}
			}
		}

		return _currentActionList;
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

		return newWorldState;
	}

}

/// Goap plans hold the formulated plan of the GOAP system.
class GoapPlan
{
	public Stack<GoapAction> actionOrder;

	public GoapGoal goalBeingFulfilled;
}