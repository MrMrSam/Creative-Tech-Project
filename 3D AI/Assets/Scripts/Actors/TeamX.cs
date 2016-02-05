using UnityEngine;
using System.Collections;

public class TeamX : TeamManager
{
	private static TeamX m_instance = null;
	public static TeamX instance { get { return m_instance; } }
	
	void Awake()
	{
		//instantiate singleton
		m_instance = this;
	}


	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
