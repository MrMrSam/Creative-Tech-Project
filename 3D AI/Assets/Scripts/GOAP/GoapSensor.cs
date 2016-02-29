/// Goap sensors test for changes which may oppose the current plan.
/// Implemented by Sam Endean 29/02/2016

using UnityEngine;
using System.Collections;

public abstract class GoapSensor : MonoBehaviour
{
	protected GoapCore core;

	public void Init (GoapCore _core)
	{
		core = _core;
	}

	public abstract bool Action(GoapWorldstate _worldState);
}
