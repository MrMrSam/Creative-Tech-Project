using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Node
{
	//a reference to the Node's gameObject
	public GameObject gameObject;
	
	//distance so far
	public float tentativeDist = float.PositiveInfinity;
	
	//the closest node
	public Node tentDistNode;
	public float heuristic;
}


public class TruncOct : MonoBehaviour
{
	Vector3[] newVertices;
	Vector3[][] faces, pairs;

	public GameObject containedActor;

	public int trOctNo;

	public enum tileType {clear, dead, showPath, showSearch, showEnds};

	public tileType type;

	//holds the node data of the TrOct
	public Node nodeData;

	public enum connectionState {NotConnected, Connected, CantConnect}
	public connectionState[] connections;
	
	//public GameObject tOctCentre, allVertices, allEdges, allFaces;
	public int[] connectionObjects;

	public List<Vector3> Faces;

	//public List<Face> shapeFaces;

	void Start()
	{

	}

	void Awake ()
	{
		nodeData = new Node ();
		nodeData.gameObject = this.gameObject;
	}

	/// <summary>
	/// Determines whether this instance is next to the specified _trOct.
	/// </summary>
	/// <returns><c>true</c> if this instance is next to the specified _trOct; otherwise, <c>false</c>.</returns>
	/// <param name="_trOct">_tr oct.</param>
	public bool IsNextTo(GameObject _trOct)
	{
		//go through each connection of the currently considered trOct
		foreach (int _connectedNo in connectionObjects)
		{
			//if actually connected on this face
			if (_connectedNo != -1)
			{
				//if this connected trOct = _troct, it is connected
				if (GameManager.instance.allTrocts[_connectedNo] == _trOct)
				{
					return true;
				}
			}
		}

		return false;
	}

	//once called by an external alteration, this will return the tile to its type config+colour
	public void ReturnToTypeColour()
	{
		switch (type)
		{
		case tileType.clear:
			transform.GetChild(0).gameObject.SetActive(false);
			transform.GetChild(0).GetComponent<MeshRenderer>().material.color = ((Material)Resources.Load("Materials/staticMat")).color;
			break;

		case tileType.dead:
			transform.GetChild(0).gameObject.SetActive(true);
			transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.white;
			break;

		case tileType.showEnds:
			transform.GetChild(0).gameObject.SetActive(true);
			transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.red;
			break;

		case tileType.showPath:
			transform.GetChild(0).gameObject.SetActive(true);
			transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.blue;
			break;

		case tileType.showSearch:
			transform.GetChild(0).gameObject.SetActive(true);
			transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.yellow;
			break;
		}
	}




	void SetFaceCentre (Vector3[] _vertices, int _faceNo)
	{
//		GameObject faceCentre = GameObject.CreatePrimitive (PrimitiveType.Cube);
//		faceCentre.name = "face " + _faceNo;
//		faceCentre.transform.localScale = new Vector3 (0.25f, 0.01f, 0.25f);
		
		Vector3 outsidePoint = Vector3.zero;
		for (int i = 0; i < _vertices.Length; i++)
		{
			outsidePoint += _vertices [i];
		}
		outsidePoint /= _vertices.Length;
		
//		faceCentre.transform.up = (outsidePoint.normalized);
//		faceCentre.transform.position = outsidePoint;

		Faces.Add(outsidePoint);

//		faceCentre.SetActive(false);
//
//		faceCentre.transform.parent = /*allFaces.*/transform;
	}

	
	void SetEdges (Vector3[] _vertices, int _edgeNo)
	{	

		GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Cylinder),
		edge = new GameObject("edge " + _edgeNo);

		SetMeshRendering(edge, temp);

		Vector3 edgeCentre = (_vertices [0] + _vertices [1]) / 2;
		//up is in dir from first to second
		edge.transform.localScale = new Vector3 (0.125f, 0.7f, 0.125f);
		edge.transform.up = (_vertices [0] - _vertices [1]).normalized;
		edge.transform.position = edgeCentre;
		
		edge.transform.parent = /*allEdges.*/transform;
	}

	void SetMeshRendering(GameObject _gObj, GameObject _temp)
	{
		_gObj.AddComponent<MeshFilter>();
		_gObj.GetComponent<MeshFilter>().mesh = _temp.GetComponent<MeshFilter>().mesh;

		_gObj.AddComponent<MeshRenderer>();
		_gObj.GetComponent<MeshRenderer>().receiveShadows = false;
		_gObj.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		_gObj.GetComponent<MeshRenderer>().useLightProbes = false;
		_gObj.GetComponent<MeshRenderer>().reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
		_gObj.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Materials/StaticMat", typeof(Material)) as Material;
		_gObj.GetComponent<MeshRenderer>().materials[0].shader = Shader.Find("Unlit/Color");

		Destroy(_temp);
	}

	//combines all meshes of the object
	void CombineMeshes ()
	{
		gameObject.AddComponent<MeshFilter>();
		gameObject.AddComponent<MeshRenderer>();

		MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
		CombineInstance[] combine = new CombineInstance[meshFilters.Length-1];
		int index = 0;
		for (int i = 0; i < meshFilters.Length; i++)
		{
			if (meshFilters[i].sharedMesh == null) continue;
			combine[index].mesh = meshFilters[i].sharedMesh;
			combine[index++].transform = meshFilters[i].transform.localToWorldMatrix;
			meshFilters[i].GetComponent<MeshRenderer>().enabled = false;
		}
		GetComponent<MeshFilter>().mesh = new Mesh();
		GetComponent<MeshFilter>().mesh.CombineMeshes (combine);
		GetComponent<MeshRenderer>().material = meshFilters[1].GetComponent<MeshRenderer>().sharedMaterial;
	}
}

//public class Face
//{
//	public int faceNo;
//	public Vector3 localPos;
//}
