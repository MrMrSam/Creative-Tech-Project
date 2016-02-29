/// <summary>
/// AIActors are owned by the Team managers and controlled using a personal GOAL system
/// Created and implemented by Sam Endean - 18/01/16
/// </summary>

using UnityEngine;
using System.Collections;

public class AIActor : ActorBase
{
	GoapCore goapBrain;

	public ActorBase targetEnemy;

	void Start ()
	{
		goapBrain = new GoapCore();

		goapBrain.actor = this;
	}

	override protected void Tick()
	{}


}
