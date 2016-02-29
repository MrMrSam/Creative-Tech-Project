/// The GoapSensor responsible for detecting an immediate obstruction on the current path (if avaliable)
/// Implemented by Sam Endean 29/02/2016

using UnityEngine;
using System.Collections;

public class NewEnemies : GoapSensor
{
	/// <summary>
	/// compare the fed worldstate to a newly generated one, if they have different enemy data, replan.
	/// </summary>
	/// <param name="_worldState">World state.</param>
	public override bool Action(GoapWorldstate _worldState)
	{
		return false;
	}
}
