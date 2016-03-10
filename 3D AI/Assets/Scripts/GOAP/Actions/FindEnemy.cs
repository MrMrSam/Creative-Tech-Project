using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class FindEnemy : GoapAction
{

	public override void Init (GoapCore _core)
	{
		actionName = "Find Enemy";
		fulfillment = "Enemy Found";

		prerequisites = new List<string>();

		core = _core;
	}

	public override bool Action(GoapPlan _currentPlan, GoapWorldstate _worldState)
	{
		GameObject goalNode = null;

		//if there is not currently a path, plot a course towards a random undiscovered area
		if (_currentPlan.plannedPath == null)
		{
			//compile a list of all trocts concealed by the fow
			List<GameObject> undiscoveredTrocts = new List<GameObject>();
			foreach (GameObject _trOctObj in _worldState.topology)
			{
				if (_trOctObj.GetComponent<TruncOct>().inFow)
				{
					undiscoveredTrocts.Add(_trOctObj);
				}
			}

			//find the one with the largest combined distance from all allies
			float biggestDistance = 0f;
			GameObject furthestTroct;
			for (int i = 0; i < undiscoveredTrocts.Count; i++)
			{
				float totalDistance = 0f;

				for (int j = 0; j < _worldState.allies.Count; j++)
				{
					totalDistance += Vector3.Distance(undiscoveredTrocts[i].transform.position, _worldState.allies[j].currentTrOct.transform.position);
				}

				if (totalDistance > biggestDistance)
				{
					furthestTroct = undiscoveredTrocts[i];
					biggestDistance = totalDistance;

					//this will be the goal TrOct
					goalNode = furthestTroct;
				}
			}

			//plot a route using the central A* plotter through the current plan
			_currentPlan.plotRoute(core.actor, core.actor.currentTrOct, goalNode);
		}

		// attempt to follow the path that was either preexisting or was just generated
		return ProceedAlongPath(_currentPlan);
	}


	//does this action need to be done.
	public override bool Test(GoapWorldstate _worldState)
	{
		GoapWorldstate worldState = core.getworldState ();

		//if enemies are in this actor's worldstate, they dont need to find enemies.
		if (worldState.enemyData.Count != 0)
		{
			return false;
		}
		else //else this does need to be done and enemies need to be found.
		{
			return true;
		}
	}
}
