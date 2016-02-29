/// The GoapSensor responsible for detecting an immediate obstruction on the current path (if avaliable)
/// Implemented by Sam Endean 29/02/2016

using UnityEngine;
using System.Collections;

public class BlockedPath : GoapSensor
{
	/// <summary>
	/// If the actor is facing the next troct on the path and the path is obstructed and the current actor has action points left, replan
	/// </summary>
	/// <param name="_worldState">World state.</param>
	public override bool Action(GoapWorldstate _worldState)
	{


		return false;
	}
}
