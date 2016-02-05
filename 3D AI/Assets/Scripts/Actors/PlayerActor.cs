using UnityEngine;
using System.Collections;

public class PlayerActor : ActorBase
{
	//public ActorBase actor;
	public int PlayerActorID;

	protected bool grounded, shooting, allowShoot = true;    

	protected virtual void Start()
	{
		Team = 0;
		actorName = "TestActor";
		base.Init();
	}

	
//	protected virtual void Update()
//	{
//		//check current game state
//		switch (GameManager.instance.GameState)
//		{
//		case GameStates.gameplay:
//			Tick();
//			break;
//		case GameStates.gamepause:
//			break;
//		case GameStates.debug:
//			Tick();
//			break;
//		case GameStates.menu:
//			break;
//		}
//		base.Update ();
//	}

	public virtual void Init(PlayerActor prevPlayer)
	{
		base.Init();
	}

	protected override void Tick()
	{
	}
}
