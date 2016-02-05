/// <summary>
/// Copies the rotation of the camera and applies it
/// Created and implemented by Sam Endean - 18/01/16
/// </summary>

using UnityEngine;
using System.Collections;

public class AxisObject : MonoBehaviour
{
	public GameObject cam;

	void Update ()
	{
		transform.rotation = cam.transform.rotation;
	}
}
