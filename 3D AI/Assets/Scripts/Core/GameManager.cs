/// <summary>
/// GameManager controls the game state machine and drives the
/// game pending on the current game state
/// Created and implemented by Sam Endean - 18/01/16
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour 
{
    private static GameManager m_instance = null;
    public static GameManager instance { get { return m_instance; } }

	public float genDistance = 10;

	public List<GameObject> allTrocts;

    private GameStates m_GameState;
    public GameStates GameState
    {
        get { return m_GameState; }
        set { m_GameState = value; }
    }
    
    void Awake()
    {
        //instantiate singleton
        m_instance = this;

        //set up initial GameState
        m_GameState = GameStates.gameplay;

		foreach (GameObject _troct in allTrocts)
		{
			_troct.transform.parent = transform;
		}
    }

	/// <summary>
	/// Pushes the game into the next the turn.
	/// </summary>
	public void TurnEnd()
	{
		//switch the active team and tell the ActorManager
	}


    void Start()
    {
		allTrocts = WorldGen.instance.GenWorld (genDistance);
	}
}
