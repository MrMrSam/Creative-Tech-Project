using UnityEngine;
using System.Collections;

public class CombineMeshes : MonoBehaviour {

	void Start ()
	{	
		for (int i = 0; i < transform.childCount; i++)
		{
			MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
			CombineInstance[] combine = new CombineInstance[meshFilters.Length-1];
			int index = 0;
			for (int j = 0; j < meshFilters.Length; j++)
			{
				if (meshFilters[j].GetComponent<MeshFilter>().sharedMesh == null) continue;
				combine[index].mesh = meshFilters[j].GetComponent<MeshFilter>().sharedMesh;
				combine[index++].transform = meshFilters[j].transform.localToWorldMatrix;
				meshFilters[j].GetComponent<MeshRenderer>().enabled = false;
			}
			GetComponent<MeshFilter>().mesh = new Mesh();
			GetComponent<MeshFilter>().mesh.CombineMeshes (combine);
		}
	}
}
