/// The worldstate class contains the world state knowledge for a GOAP agent (the tiles within sight range of the actor + enemy positions known by the other actors
/// Implemented by Sam Endean 16/02/2016

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GoapWorldstate
{
	//the actual world/map info
	public List<GameObject> topology;

	//position and id of all enemies known about (within sight range AND within sight range of allies in sight range)
	public List<EnemyPosition> enemyData;

	public List<ActorBase> allies;

	//the troct being occupied by the actor
	public TruncOct selfTroct;

	//the facing of the actor
	public int selfFacing;

	//alters the calling worldstate to be updated according to it's actor
	public void generateWorldState (AIActor _goapActor)
	{
		//fetch the troct info from the game manager
		topology = GameManager.instance.allTrocts;

		//scan for enemies and allies within sight range
		enemyData = EnemyScan(_goapActor, new List<EnemyPosition> (), new List<ActorBase> ());

		//Go into actor and get current troct
		selfTroct = _goapActor.currentTrOct.GetComponent<TruncOct>();

		//go into actor and get the current facing
		selfFacing = _goapActor.currentFacing;

		allies = TeamManager.instance.GetTeam(_goapActor.Team);
	}

	/// <summary>
	/// Scans to find enemies and alliew within sight range, all enemies found are then added to the list and then any allies found are also scanned from
	/// </summary>
	/// <returns>The found enemies</returns>
	/// <param name="_actor">the actor scanning</param>
	private List<EnemyPosition> EnemyScan (AIActor _actor, List<EnemyPosition> _enemies, List<ActorBase> _allies)
	{
		List<EnemyPosition> enemyData = new List<EnemyPosition> ();

		//search all trocts within sight range of the actor.
		for (int i = 0; i < topology.Count; i++)
		{
			//if within range and not inFow & in the active list
			if (Vector3.Distance (_actor.gameObject.transform.position, topology [i].gameObject.transform.position) < _actor.viewDistance && !topology [i].GetComponent<TruncOct> ().inFow &&
				topology [i].GetComponent<TruncOct> ().containedActor != null)
			{
				ActorBase actorFound = topology [i].GetComponent<TruncOct> ().containedActor.GetComponent<ActorBase> ();

				//Test if the actor is on this team
				if (topology [i].GetComponent<TruncOct> ().containedActor.GetComponent<ActorBase> ().Team == _actor.Team)
				{
					//if so they are added to the allies list after seeing whether they have already been scanned
					if (!_allies.Contains (actorFound))
					{
						_allies.Add (actorFound);

						//scan the actorfound in this case
						_enemies.AddRange(EnemyScan(actorFound.gameObject.GetComponent<AIActor>(), _enemies, _allies));
					}
				}
				else
				{
					EnemyPosition enemy = new EnemyPosition ();

					enemy.enemy = actorFound;
					enemy.enemyLocation = actorFound.currentTrOct.GetComponent<TruncOct>();

					//if the found enemy is not currently in the list, add it in
					if (!_enemies.Contains(enemy))
					{
						_enemies.Add (enemy);
					}
				}
			}
		}
			
		return _enemies;
	}

	/// <summary>
	/// Compares the presence of the actor's team compared to the enemy team.
	/// </summary>
	/// <returns>The relative presence.</returns>
	public float ComparePresence(ActorBase _actor)
	{
		//if there are no enemies
		if (enemyData.Count == 0)
		{
			return 1;
		}

		//take own health proportion
		float ally = _actor.health / _actor.maxHealth;
		float enemy = 0f;

		//take ally health proportion / distance
		foreach (ActorBase _ally in allies)
		{
			ally += _ally.health / _ally.maxHealth;
		}

		if (ally != 0f)
		{
			ally /= allies.Count + 1;
		}	

		foreach (EnemyPosition _enemyDat in enemyData)
		{
			enemy += _enemyDat.enemy.health / _enemyDat.enemy.maxHealth;
		}

		enemy /= enemyData.Count;

		return 1 - ((ally /  enemy) / (allies.Count + enemyData.Count));
	}

}


public class EnemyPosition
{
	//the actual enemy
	public ActorBase enemy;

	//the Troct location of the enemy.
	public TruncOct enemyLocation;
}
