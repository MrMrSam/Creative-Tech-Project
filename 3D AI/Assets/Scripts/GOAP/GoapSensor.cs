/// Goap sensors test for changes which may oppose the current plan.
/// Implemented by Sam Endean 29/02/2016

using UnityEngine;
using System.Collections;

public abstract class GoapSensor
{
	protected GoapCore core;

	public void Init (GoapCore _core)
	{
		core = _core;
	}

	public abstract bool Sense(GoapWorldstate _worldState);
}
