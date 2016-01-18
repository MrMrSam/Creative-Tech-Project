using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TruncOct : MonoBehaviour
{
	Vector3[] newVertices;
	Vector3[][] faces, pairs;

	public int trOctNo;

	public enum tileType {clear, dead, showPath, showSearch, showEnds};

	public tileType type;

	public enum connectionState {NotConnected, Connected, CantConnect}
	public connectionState[] connections;
	
	//public GameObject tOctCentre, allVertices, allEdges, allFaces;
	public int[] connectionObjects;

	public List<Vector3> Faces;

	void Awake ()
	{
//		gameObject.tag = "TrOct";
//
//		connections = new connectionState[14];
//		connectionObjects = new int[14];
//		
//		//allVertices = new GameObject ("Vertices");
//		//allEdges = new GameObject ("Edges");
//		//allFaces = new GameObject ("Faces");
//
//		Faces = new List<Vector3>();
//		
//		//allVertices.transform.parent = transform;
//		//allEdges.transform.parent = transform;
//		//allFaces.transform.parent = transform;
//		
//		//declare that the shape is made of 14 faces
//		faces = new Vector3[14][];
//		pairs = new Vector3[36][];
//		
//		newVertices = new Vector3[24];
//		
//		//define all 24 vertices
//		
//		newVertices [0] = new Vector3 (2, 1, 0);
//		newVertices [1] = new Vector3 (2, -1, 0);
//		newVertices [2] = new Vector3 (-2, 1, 0);
//		newVertices [3] = new Vector3 (-2, -1, 0);
//		
//		newVertices [4] = new Vector3 (0, 2, 1);
//		newVertices [5] = new Vector3 (0, 2, -1);
//		newVertices [6] = new Vector3 (0, -2, 1);
//		newVertices [7] = new Vector3 (0, -2, -1);
//		
//		newVertices [8] = new Vector3 (1, 0, 2);
//		newVertices [9] = new Vector3 (1, 0, -2);
//		newVertices [10] = new Vector3 (-1, 0, 2);
//		newVertices [11] = new Vector3 (-1, 0, -2);
//		
//		newVertices [12] = new Vector3 (1, 2, 0);
//		newVertices [13] = new Vector3 (1, -2, 0);
//		newVertices [14] = new Vector3 (-1, 2, 0);
//		newVertices [15] = new Vector3 (-1, -2, 0);
//
//		newVertices [16] = new Vector3 (0, 1, 2);
//		newVertices [17] = new Vector3 (0, 1, -2);
//		newVertices [18] = new Vector3 (0, -1, 2);
//		newVertices [19] = new Vector3 (0, -1, -2);
//
//		newVertices [20] = new Vector3 (2, 0, 1);
//		newVertices [21] = new Vector3 (2, 0, -1);
//		newVertices [22] = new Vector3 (-2, 0, 1);
//		newVertices [23] = new Vector3 (-2, 0, -1);
//		
//		int pointNumber = 0;
//
//		//draw spheres
//		for (int i = 0; i < newVertices.Length; i++)
//		{
//			GameObject sphereTemp = GameObject.CreatePrimitive(PrimitiveType.Sphere),
//			Sphere = new GameObject("Point " + pointNumber);
//
//			SetMeshRendering(Sphere, sphereTemp);
//
//			Sphere.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
//			Sphere.transform.position = newVertices[i];
//			
//			pointNumber ++;
//			
//			Sphere.transform.parent = /*allVertices.*/transform;
//		}
//		
//		//compile vertices into faces
//		faces [0] = new Vector3[4] {newVertices [4], newVertices [12], newVertices [5], newVertices [14]};
//		faces [1] = new Vector3[4] {newVertices [16], newVertices [10], newVertices [18], newVertices [8]};
//		faces [2] = new Vector3[4] {newVertices [0], newVertices [20], newVertices [1], newVertices [21]};
//		faces [3] = new Vector3[4] {newVertices [17], newVertices [9], newVertices [19], newVertices [11]};
//		faces [4] = new Vector3[4] {newVertices [2], newVertices [23], newVertices [3], newVertices [22]};
//		faces [5] = new Vector3[4] {newVertices [6], newVertices [15], newVertices [7], newVertices [13]};
//		faces [6] = new Vector3[6] {newVertices [12], newVertices [0], newVertices [21], newVertices [9], newVertices [17], newVertices [5]};
//		faces [7] = new Vector3[6] {newVertices [5], newVertices [14], newVertices [17], newVertices [11], newVertices [23], newVertices [2]};
//		faces [8] = new Vector3[6] {newVertices [14], newVertices [2], newVertices [22], newVertices [10], newVertices [16], newVertices [4]};
//		faces [9] = new Vector3[6] {newVertices [4], newVertices [16], newVertices [8], newVertices [20], newVertices [0], newVertices [12]};
//		faces [10] = new Vector3[6] {newVertices [8], newVertices [18], newVertices [6], newVertices [13], newVertices [1], newVertices [20]};
//		faces [11] = new Vector3[6] {newVertices [21], newVertices [1], newVertices [13], newVertices [7], newVertices [19], newVertices [9]};
//		faces [12] = new Vector3[6] {newVertices [11], newVertices [19], newVertices [7], newVertices [15], newVertices [3], newVertices [23]};
//		faces [13] = new Vector3[6] {newVertices [22], newVertices [3], newVertices [15], newVertices [6], newVertices [18], newVertices [10]};
//
//		
//		pairs [0] = new Vector3[2] {newVertices [0], newVertices [20]};
//		pairs [1] = new Vector3[2] {newVertices [0], newVertices [21]};
//		pairs [2] = new Vector3[2] {newVertices [0], newVertices [12]};
//		pairs [3] = new Vector3[2] {newVertices [1], newVertices [20]};
//		pairs [4] = new Vector3[2] {newVertices [1], newVertices [13]};
//		pairs [5] = new Vector3[2] {newVertices [1], newVertices [21]};
//		pairs [6] = new Vector3[2] {newVertices [2], newVertices [14]};
//		pairs [7] = new Vector3[2] {newVertices [2], newVertices [23]};
//		pairs [8] = new Vector3[2] {newVertices [2], newVertices [22]};
//		pairs [9] = new Vector3[2] {newVertices [3], newVertices [23]};
//		pairs [10] = new Vector3[2] {newVertices [3], newVertices [15]};
//		pairs [11] = new Vector3[2] {newVertices [3], newVertices [22]};		
//		pairs [12] = new Vector3[2] {newVertices [4], newVertices [14]};
//		pairs [13] = new Vector3[2] {newVertices [4], newVertices [16]};
//		pairs [14] = new Vector3[2] {newVertices [4], newVertices [12]};
//		pairs [15] = new Vector3[2] {newVertices [5], newVertices [12]};
//		pairs [16] = new Vector3[2] {newVertices [5], newVertices [17]};
//		pairs [17] = new Vector3[2] {newVertices [5], newVertices [14]};		
//		pairs [18] = new Vector3[2] {newVertices [6], newVertices [18]};
//		pairs [19] = new Vector3[2] {newVertices [6], newVertices [15]};
//		pairs [20] = new Vector3[2] {newVertices [6], newVertices [13]};
//		pairs [21] = new Vector3[2] {newVertices [7], newVertices [19]};		
//		pairs [22] = new Vector3[2] {newVertices [7], newVertices [13]};
//		pairs [23] = new Vector3[2] {newVertices [7], newVertices [15]};
//		pairs [24] = new Vector3[2] {newVertices [8], newVertices [16]};
//		pairs [25] = new Vector3[2] {newVertices [8], newVertices [18]};
//		pairs [26] = new Vector3[2] {newVertices [8], newVertices [20]};
//		pairs [27] = new Vector3[2] {newVertices [9], newVertices [21]};
//		pairs [28] = new Vector3[2] {newVertices [9], newVertices [19]};
//		pairs [29] = new Vector3[2] {newVertices [9], newVertices [17]};		
//		pairs [30] = new Vector3[2] {newVertices [10], newVertices [22]};
//		pairs [31] = new Vector3[2] {newVertices [10], newVertices [18]};
//		pairs [32] = new Vector3[2] {newVertices [10], newVertices [16]};
//		pairs [33] = new Vector3[2] {newVertices [11], newVertices [17]};
//		pairs [34] = new Vector3[2] {newVertices [11], newVertices [19]};
//		pairs [35] = new Vector3[2] {newVertices [11], newVertices [23]};		
//		
//		//find average of each face and set vector 3 face
//		int faceNo = 0;
//		for (int i = 0; i < faces.Length; i++)
//		{
//			SetFaceCentre (faces[i], faceNo);
//			//increment faceNo
//			faceNo++;
//		}
//		
//		//set edges (cylinders)
//		int edgeNo = 0;
//		for (int i = 0; i < pairs.Length; i++)
//		{
//			SetEdges (pairs[i], edgeNo);
//			edgeNo++;
//		}
//
//		CombineMeshes();
//
//		//destroy children
//		int children = transform.childCount;
//
//		for (int i = 0; i < children; i ++)
//		{
//			Destroy(transform.GetChild(i).gameObject);
//		}
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
