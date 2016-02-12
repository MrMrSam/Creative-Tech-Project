using UnityEngine;
using System.Collections;

public class Torpedo : MonoBehaviour
{	
	Vector3 startPos;


	void Start ()
	{
		startPos = transform.localPosition;
	}

	/// <summary>
	/// whilst active, will travel forwards in the forwards direction, once it hits a target or has travelled 20uu, it will disable and reset
	/// </summary>
	void Update ()
	{
		transform.Translate(transform.parent.forward * Time.deltaTime * 5, Space.World);


		if (Vector3.Distance(transform.position, transform.parent.position) > 20)
		{
			transform.localPosition = startPos;
			gameObject.SetActive(false);
		}
	}

	void OnTriggerEnter(Collider _other)
	{
		//if it hits and actor
		if (_other.gameObject.GetComponent<ActorBase>())
		{
			//if it isnt this torpedo's parent
			if (_other.gameObject != this.transform.parent.gameObject)
			{
				_other.gameObject.GetComponent<ActorBase>().TakeDamage();

				//reset position and disable for further use
				transform.localPosition = startPos;
				gameObject.SetActive(false);
			}
		}
	}
}
