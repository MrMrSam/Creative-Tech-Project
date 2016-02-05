using UnityEngine;
using System.Collections;

public class TeamY : TeamManager
{
	private static TeamY m_instance = null;
	public static TeamY instance { get { return m_instance; } }
	
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
