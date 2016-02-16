/// The worldstate class contains the world state knowledge for a GOAP agent (the tiles within sight range of the actor + enemy positions known by the other actors
/// Implemented by Sam Endean 16/02/2016

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GoapWorldstate : MonoBehaviour 
{
	List<TruncOct> topology;

	List<EnemyPosition> combat;
}

public class EnemyPosition
{
	ActorBase enemy;
	TruncOct enemyLocation;
}
