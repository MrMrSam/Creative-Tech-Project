/// <summary>
/// AIActors are owned by the Team managers and controlled using a personal GOAL system
/// Created and implemented by Sam Endean - 18/01/16
/// </summary>

using UnityEngine;
using System.Collections;

public class AIActor : ActorBase
{
	public GoapCore goapBrain;

	public ActorBase targetEnemy;

	void Start ()
	{
		targetEnemy = null;

		goapBrain = new GoapCore();
		goapBrain.Init(this);
	}

	override protected void Tick()
	{}


}
