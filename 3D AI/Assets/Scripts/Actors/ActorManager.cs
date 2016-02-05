/// <summary>
/// ActorManager controls the
/// Created and implemented by Sam Endean - 18/01/16
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActorManager : MonoBehaviour 
{
	private static ActorManager m_instance = null;
	public static ActorManager instance { get { return m_instance; } }

	public List<GameObject> Team0, Team1;

	//the starting amount of ships on each team
	public int startShipNo = 1;

	void Awake()
	{
		//instantiate singleton
		m_instance = this;
	}

	/// <summary>
	/// Setup the teams on start/restart.
	/// </summary>
	void Setup()
	{

	}
}