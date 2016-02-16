/// <summary>
/// TeamManager is the base class for team management
/// Created and implemented by Sam Endean - 21/01/16
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TeamManager : MonoBehaviour 
{
	private Team teamA, teamB;
	private int teamMember;

	public List<GameObject> activeActors;

	private static TeamManager m_instance = null;
	public static TeamManager instance { get { return m_instance; } }

	public GameObject teamASpawn, teamBSpawn;
	public float fowDistance = 10;

	void Awake()
	{
		//instantiate singleton
		m_instance = this;
	}

	/// <summary>
	/// Sets teams using data passed from GameManager.
	/// </summary>
	/// <param name="_world">The world Generated</param>
	/// <param name="_teamAType">If set to <c>true</c> team A is AI controlled</param>
	/// <param name="_teamBType">If set to <c>true</c> team B is AI controlled</param>
	/// <param name="_noOfVessels">Number of vessels to create on each team.</param>
	public void SetTeams(List<GameObject> _world, bool _teamAType, bool _teamBType, int _noOfVessels)
	{
		teamA = new Team();
		teamB = new Team();

		teamA.members = new List<GameObject>();
		teamB.members = new List<GameObject>();

		teamA.team = 0;
		teamB.team = 1;

		if (_teamAType)
		{
			teamA.controlType = Team.TeamType.AI;
		}
		else
		{
			teamA.controlType = Team.TeamType.PC;
		}

		if (_teamBType)
		{
			teamB.controlType = Team.TeamType.AI;
		}
		else
		{
			teamB.controlType = Team.TeamType.PC;
		}

		activeActors = new List<GameObject>();

		//i is also the team number
		for (int i = 0; i < 2; i++)
		{
			//j is the vessel within team i
			for (int j = 0; j < _noOfVessels; j++)
			{
				if (i == 0)
				{
					//if this team is player controlled, make a player
					if (teamA.controlType == Team.TeamType.PC)
					{
						GameObject newActor = transform.GetChild(i).GetChild(j).gameObject;
						newActor.GetComponent<PlayerActor>().PlayerActorID = j;
						newActor.GetComponent<PlayerActor>().Team = i;
						newActor.SetActive(true);

						activeActors.Add(newActor);
						teamA.members.Add(newActor);

						//transform.GetChild
					}
					else //make it an AI actor
					{
					}
				}
				else
				{
					//if this team is player controlled, make a player
					if (teamB.controlType == Team.TeamType.PC)
					{
						GameObject newActor = transform.GetChild(i).GetChild(j).gameObject;
						newActor.GetComponent<PlayerActor>().PlayerActorID = j + (i * _noOfVessels);
						newActor.GetComponent<PlayerActor>().Team = i;
						newActor.SetActive(true);
						
						activeActors.Add(newActor);
						teamB.members.Add(newActor);

						//transform.GetChild
					}
					else //make it an AI actor
					{
					}
				}
			}
		}

		SpawnTeams(_world, _noOfVessels);
	}

	/// <summary>
	/// Reset the member selection int, clear the fow for the new team.
	/// </summary>
	/// <param name="_team">The team being switched to.</param>
	public void SwitchTeams(int _team)
	{
		teamMember = 0;

		//clears the FOW for the team being switched to.
		ClearFOW(_team);

		//set the new cam target to the next teams first player entry
		if (_team == 0)
		{
			//reset actionPoints
			foreach (GameObject _actor in teamA.members)
			{
				_actor.GetComponent<MeshRenderer>().enabled = true;
				_actor.GetComponent<ActorBase>().actionPoints = 3;
			}

			//set the last team's mesh renderers to false if they are now in the FOW
			foreach (GameObject _actor in teamB.members)
			{
				//if in the FOW
				if (_actor.GetComponent<ActorBase>().currentTrOct.GetComponent<TruncOct>().fogOfWar.activeSelf == true)
				{
					//be invisible
					_actor.GetComponent<MeshRenderer>().enabled = false;
				}
			}


			Selection.instance.setTarget(teamA.members[0].GetComponent<ActorBase>().currentTrOct);
		}
		else
		{
			//reset actionPoints
			foreach (GameObject _actor in teamB.members)
			{
				_actor.GetComponent<MeshRenderer>().enabled = true;
				_actor.GetComponent<ActorBase>().actionPoints = 3;
			}

			//set the last team's mesh renderers to false if they are now in the FOW
			foreach (GameObject _actor in teamA.members)
			{
				//if in the FOW
				if (_actor.GetComponent<ActorBase>().currentTrOct.GetComponent<TruncOct>().fogOfWar.activeSelf == true)
				{
					//be invisible
					_actor.GetComponent<MeshRenderer>().enabled = false;
				}
			}

			Selection.instance.setTarget(teamB.members[0].GetComponent<ActorBase>().currentTrOct);
		}
	}

	/// <summary>
	/// Kill the specified _actor.
	/// </summary>
	/// <param name="_actor">_actor.</param>
	public void Kill(GameObject _actor)
	{
		int team = _actor.GetComponent<ActorBase>().Team;

		_actor.GetComponent<ActorBase> ().currentTrOct.GetComponent<TruncOct> ().containedActor = null;

		_actor.SetActive(false);

		//remove from the team roster and set to disabled
		if (team == 0)
		{
			teamA.members.Remove(_actor);

			if (teamA.members.Count == 0)
			{
				GameManager.instance.TeamWin(team + 1);
			}
		}
		else
		{
			teamB.members.Remove(_actor);

			if (teamB.members.Count == 0)
			{
				GameManager.instance.TeamWin(team - 1);
			}
		}

		activeActors.Remove(_actor);
	}

	/// <summary>
	/// Clears the FOW for the team being switched to by going outwards from each face of the team member's current face and clearing 3 trocts.
	/// </summary>
	/// <param name="_team">_team.</param>
	public void ClearFOW(int _team)
	{
		//iterate through each team member
		Team currentTeam = teamA;

		if (_team == 1)
		{
			currentTeam = teamB;
		}

		for (int i = 0; i < currentTeam.members.Count; i++)
		{
			//go through trocts and disble the fow on those that are within the fowDistance
			foreach(GameObject _trOct in GameManager.instance.allTrocts)
			{
				if (Vector3.Distance(currentTeam.members[i].GetComponent<ActorBase>().currentTrOct.transform.position, _trOct.transform.position) <= fowDistance)
				{
					_trOct.GetComponent<TruncOct>().fogOfWar.SetActive(false);
				}
			}


//			//go through each face of the current Troct and clear 3 trocts in each direction
//			for (int j = 0; j < 14; j++)
//			{
//				TruncOct trOct = currentTeam.members[i].GetComponent<ActorBase>().currentTrOct.GetComponent<TruncOct>(),
//				firstTrOct = GameManager.instance.allTrocts[trOct.connectionObjects[j]].GetComponent<TruncOct>();
//
//				trOct.fogOfWar.SetActive(false);
//
//				ClearTrOct(firstTrOct, j, 0);
//			}

		}
	}

	private void ClearTrOct(TruncOct _trOct, int _faceNo, int _numberCleared)
	{
		//clear the given troct and increment _numbercleared
		_trOct.fogOfWar.SetActive(false);

		_numberCleared++;

		//if number cleared is now 3, return, else do the next troct in this direction
		if (_numberCleared < 3)
		{
			TruncOct fedTrOct = GameManager.instance.allTrocts[_trOct.connectionObjects[_faceNo]].GetComponent<TruncOct>();

			ClearTrOct(fedTrOct, _faceNo, _numberCleared);
		}

		return;		
	}


	/// <summary>
	/// Spawns the teams in the world by choosing a face from either side of the starting troct and going outwards to the end.
	/// </summary>
	/// <param name="_world">the World in which to spawn the players</param>
	private void SpawnTeams(List<GameObject> _world, int _noOfVessels)
	{

		KeyValuePair<GameObject, int> spawnInfoA = SelectSpawnTrOct(0, _world),
		spawnInfoB = SelectSpawnTrOct(1, _world);

		teamASpawn = spawnInfoA.Key;
		teamBSpawn = spawnInfoB.Key;

		//find the correct direction to start each actor facing
		int startFacingA = WorldGen.instance.ConvertConnecton(spawnInfoA.Value),
		startFacingB = WorldGen.instance.ConvertConnecton(spawnInfoB.Value);

		//set first actor of first team to spawn on the spawnPoint
		teamA.members[0].transform.position = teamASpawn.transform.position;
		LinkTrOctContaining(teamASpawn, teamA.members[0]);
		teamA.members[0].GetComponent<ActorBase>().setFacing(teamASpawn, startFacingA);

		//first actor of teamB
		teamB.members[0].transform.position = teamBSpawn.transform.position;
		LinkTrOctContaining(teamBSpawn, teamB.members[0]);
		teamB.members[0].GetComponent<ActorBase>().setFacing(teamBSpawn, startFacingB);

		//spawn the rest of the 
		for (int i = 1; i < _noOfVessels; i++)
		{
			//try to spawn in order around the starting troct, checking for it beign the clear type, and it not containing an actor
			TryPlaceActorAround(teamA.members[i], _world, teamASpawn, startFacingA); 
			TryPlaceActorAround(teamB.members[i], _world, teamBSpawn, startFacingB); 
		}
	}

	/// <summary>
	/// Tries the place actor around _troct, looking in _facing direction.
	/// </summary>
	/// <param name="_world">The worldspace.</param>
	/// <param name="_trOct">The trOct to be spawned around.</param>
	/// <param name="_facing">The face to look towards.</param>
	private bool TryPlaceActorAround(GameObject _actor, List<GameObject> _world, GameObject _trOct, int _facing)
	{
		//try to place actor in each direction
		for (int i = 0; i < 14; i++)
		{
			TruncOct temp = _world[_trOct.GetComponent<TruncOct>().connectionObjects[i]].GetComponent<TruncOct>();

			//if clear and has no actor, spawn here
			if (temp.type == TruncOct.tileType.clear && !temp.containedActor && temp.trOctNo != 15)
			{
				//place/spawn
				_actor.transform.position = temp.gameObject.transform.position;
				LinkTrOctContaining(temp.gameObject, _actor);
				_actor.GetComponent<ActorBase>().setFacing(temp.gameObject, _facing);

				return true;
			}
		}

		//if by here, spawnage has not occured, repeat this fuction for each connecting _troct
		for (int i = 0; i < 14; i++)
		{
			GameObject newTroct = _world[_trOct.GetComponent<TruncOct>().connectionObjects[i]];

			return TryPlaceActorAround(_actor, _world, _trOct, _facing); 
		}

		return false;
	}
		



	/// <summary>
	/// Links the trOct and the containing actor.
	/// </summary>
	private void LinkTrOctContaining(GameObject _trOct, GameObject _actor)
	{
		_trOct.GetComponent<TruncOct>().containedActor = _actor;
		_actor.GetComponent<ActorBase>().currentTrOct = _trOct;
	}

	private KeyValuePair<GameObject, int> SelectSpawnTrOct(int _team, List<GameObject> _world)
	{
		//the faces to chose from randomly for each side
		int[] spawnFacesA = new int[6] {1, 2, 8, 9, 10, 13},
		spawnFacesB = new int[6] {3, 4, 6, 7, 11, 12};

		int faceChosen = 0;

		if (_team == 0)
		{
			faceChosen = spawnFacesA[Random.Range(0, 6)];
		}
		else
		{
			faceChosen = spawnFacesB[Random.Range(0, 6)];
		}
		
			//use FaceA and FaceB to travel to the furthest clear troct from the centre in that direction
		
		TruncOct currentTrOct = _world[0].GetComponent<TruncOct>();
		while (currentTrOct.connectionObjects[faceChosen] != 15)
		{
			currentTrOct = _world[currentTrOct.connectionObjects[faceChosen]].GetComponent<TruncOct>();
		}
		//sanity check to make sure the troct is clear
		//if not clear, choose a face from this one and check again
		if (currentTrOct.type != TruncOct.tileType.clear)
		{
			for (int i = 0; i < 14; i++)
			{
				if (_world[currentTrOct.connectionObjects[i]].GetComponent<TruncOct>().type == TruncOct.tileType.clear)
				{
					currentTrOct = _world[currentTrOct.connectionObjects[i]].GetComponent<TruncOct>();
					break;
				}
			}
		}
		
		return new KeyValuePair<GameObject, int>(currentTrOct.gameObject, faceChosen);
	}
}


public class Team
{
	public List<GameObject> members;

	public int team;

	//TeamType can be AI Controlled or Player Controlled
	public enum TeamType {AI, PC};

	public TeamType controlType;
}