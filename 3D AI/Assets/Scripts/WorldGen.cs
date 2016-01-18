using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class WorldGen : MonoBehaviour
{
	public GameObject trOctPrefab, trOctShapePrefab;
	public List<GameObject> truncOcts;

	public GameObject Pathfinding, Cam;

	GameObject allTrOcts;

	public int genDistance = 10;

	public bool deadSpace, seeThroughTrOcts;

	List<GameObject> deadTrocts;

	bool updated;

	void Start ()
	{
		allTrOcts = new GameObject("AllTrOcts");

		truncOcts = new List<GameObject> ();

		if (deadSpace)
		{
			deadTrocts = new List<GameObject>();
		}

		//create initial troct
		GameObject tOct = (GameObject) GameObject.Instantiate(trOctPrefab, Vector3.zero, Quaternion.identity);//new GameObject ();
		//tOct.AddComponent<TruncOct> ();
		tOct.name = "TruncOct 0";

		//create interior troct
		GameObject intTrOct = (GameObject) GameObject.Instantiate(trOctShapePrefab, Vector3.zero, Quaternion.identity);
		intTrOct.name = tOct.name + " Interior";
		intTrOct.transform.parent = tOct.transform;
		intTrOct.SetActive(false);

		tOct.transform.parent = allTrOcts.transform;
		truncOcts.Add (tOct);

		//GENERATE + CONNECT

		//MAKE A TEST
		while (!connectionsFull())
		{
			//for loop through the list and find a non-connection
			
			//loop through all truncOcts
			for (int i = 0; i < truncOcts.Count; i++)
			{
				//loop trough faces
				for (int j = 0; j < 14; j++)
				{
					//if not already connected, try to connect by calling SpawnTrunc
					if (truncOcts[i].GetComponent<TruncOct>().connections[j] == TruncOct.connectionState.NotConnected)
					{
						SpawnTrunc(truncOcts[i], j);
					}
				}
			}
		}

		//if dead trocts are present, set their connections to "CANT CONNECT" along with those of their neighbors, also make their interior the blocking colour
		if (deadSpace)
		{
			//i goes through each deadtroct
			for(int i = 0; i < deadTrocts.Count; i++)
			{
				//set interior to visible and white
				deadTrocts[i].GetComponent<TruncOct>().type = TruncOct.tileType.dead;
				deadTrocts[i].GetComponent<TruncOct>().ReturnToTypeColour();

				//set all connections and shared connections in neighboring trocts to CANT CONNECT
				//j goes through each connection of the current deadtroct (deadTrocts[i])
				for (int j = 0; j < 14; j++)
				{

					if (deadTrocts[i].GetComponent<TruncOct>().connections[j] == TruncOct.connectionState.Connected)
					{
						//find the neighboring troct by finding the object in truncOcts at the index of the current connection's int value
						GameObject neighbor = truncOcts[deadTrocts[i].GetComponent<TruncOct>().connectionObjects[j]];

						//find the shared connections in neighbor by going through their connections to find the current troctNumber
						//s goes through all of the neighbors connections and finds the one connecting to this
						for (int s = 0; s < 14; s++)
						{
							//if the currently considered connection object == deadTroct
							if (neighbor.GetComponent<TruncOct>().connectionObjects[s] == deadTrocts[i].GetComponent<TruncOct>().trOctNo)
							{
								//set to cannot Connect, cutting the dead troct off
								neighbor.GetComponent<TruncOct>().connections[s] = TruncOct.connectionState.CantConnect;
							}
						}//end of s loop

						deadTrocts[i].GetComponent<TruncOct>().connections[j] = TruncOct.connectionState.CantConnect;
					}
				}//end of j loop
			}//end of i loop
		}

//		//disable the vertices and edges surrounding applicable faces
//		for (int i = 0; i < truncOcts.Count; i++)
//		{
//			for (int j = 0; j < 14; j++)
//			{
//				if (truncOcts[i].GetComponent<TruncOct>().connections[j] == TruncOct.connectionState.Connected &&
//				    (j == 0 || j == 1 || j == 2 || j == 6 || j == 7 || j == 8 || j == 9))
//				{
//
//					switch (j)
//					{
//					case 0:
//						//disable edges 12, 14, 15, 17 and vertices 4, 5, 12, 14
//						truncOcts[i].GetComponent<TruncOct>().allEdges.transform.GetChild(12).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allEdges.transform.GetChild(14).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allEdges.transform.GetChild(15).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allEdges.transform.GetChild(17).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allVertices.transform.GetChild(4).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allVertices.transform.GetChild(5).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allVertices.transform.GetChild(12).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allVertices.transform.GetChild(14).gameObject.SetActive(false);
//						break;
//
//					case 1:
//						truncOcts[i].GetComponent<TruncOct>().allEdges.transform.GetChild(24).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allEdges.transform.GetChild(25).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allEdges.transform.GetChild(31).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allEdges.transform.GetChild(32).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allVertices.transform.GetChild(8).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allVertices.transform.GetChild(10).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allVertices.transform.GetChild(16).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allVertices.transform.GetChild(18).gameObject.SetActive(false);
//						break;
//
//					case 2:
//						truncOcts[i].GetComponent<TruncOct>().allEdges.transform.GetChild(0).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allEdges.transform.GetChild(1).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allEdges.transform.GetChild(5).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allEdges.transform.GetChild(3).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allVertices.transform.GetChild(0).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allVertices.transform.GetChild(1).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allVertices.transform.GetChild(20).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allVertices.transform.GetChild(21).gameObject.SetActive(false);
//						break;
//
//					case 6:
//						truncOcts[i].GetComponent<TruncOct>().allEdges.transform.GetChild(1).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allEdges.transform.GetChild(2).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allEdges.transform.GetChild(15).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allEdges.transform.GetChild(16).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allEdges.transform.GetChild(27).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allEdges.transform.GetChild(29).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allVertices.transform.GetChild(0).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allVertices.transform.GetChild(21).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allVertices.transform.GetChild(9).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allVertices.transform.GetChild(17).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allVertices.transform.GetChild(5).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allVertices.transform.GetChild(12).gameObject.SetActive(false);
//						break;
//
//					case 7:
//						truncOcts[i].GetComponent<TruncOct>().allEdges.transform.GetChild(17).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allEdges.transform.GetChild(16).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allEdges.transform.GetChild(33).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allEdges.transform.GetChild(35).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allEdges.transform.GetChild(7).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allEdges.transform.GetChild(6).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allVertices.transform.GetChild(14).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allVertices.transform.GetChild(5).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allVertices.transform.GetChild(17).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allVertices.transform.GetChild(11).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allVertices.transform.GetChild(23).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allVertices.transform.GetChild(2).gameObject.SetActive(false);
//						break;
//
//					case 8:
//						truncOcts[i].GetComponent<TruncOct>().allEdges.transform.GetChild(12).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allEdges.transform.GetChild(6).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allEdges.transform.GetChild(8).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allEdges.transform.GetChild(30).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allEdges.transform.GetChild(32).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allEdges.transform.GetChild(13).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allVertices.transform.GetChild(4).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allVertices.transform.GetChild(14).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allVertices.transform.GetChild(2).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allVertices.transform.GetChild(22).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allVertices.transform.GetChild(10).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allVertices.transform.GetChild(16).gameObject.SetActive(false);
//						break;
//
//					case 9:
//						truncOcts[i].GetComponent<TruncOct>().allEdges.transform.GetChild(14).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allEdges.transform.GetChild(13).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allEdges.transform.GetChild(24).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allEdges.transform.GetChild(26).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allEdges.transform.GetChild(0).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allEdges.transform.GetChild(2).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allVertices.transform.GetChild(12).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allVertices.transform.GetChild(4).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allVertices.transform.GetChild(16).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allVertices.transform.GetChild(8).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allVertices.transform.GetChild(20).gameObject.SetActive(false);
//						truncOcts[i].GetComponent<TruncOct>().allVertices.transform.GetChild(0).gameObject.SetActive(false);
//						break;
//					}
//				}
//			}
//		}
	}

	void Update()
	{
		if (!updated)
		{
			Pathfinding.SetActive(true);
			Cam.SetActive(true);
			updated = true;
		}
		else
		{
		}
	}

	//this spawns a trunc and tries to spawn a trunk on all of the connections
	void SpawnTrunc (GameObject _trOct, int _connection)
	{
		//_trOct is the root troct and the _connection is root's connection
		GameObject newTrOct = (GameObject) GameObject.Instantiate(trOctPrefab, _trOct.transform.position, Quaternion.identity);//new GameObject ();
		newTrOct.name = "TruncOct " + truncOcts.Count;

		newTrOct.GetComponent<TruncOct>().trOctNo = truncOcts.Count;

		//set pos to the pos of the older trOct, then add the distance from the mid to the connection twice
		newTrOct.transform.position = _trOct.transform.position + (_trOct.GetComponent<TruncOct>().Faces[_connection] * 2);

		//create interior troct
		GameObject intTrOct = (GameObject) GameObject.Instantiate(trOctShapePrefab, Vector3.zero, Quaternion.identity);
		intTrOct.name = newTrOct.name + " Interior";
		intTrOct.transform.position = newTrOct.transform.position;
		intTrOct.transform.parent = newTrOct.transform;
		intTrOct.SetActive(false);

		int newConnection = ConvertConnecton(_connection);

		//test if valid, if can spawn: spawn and set to connected and previous oct's connections and connections objects accordingly 
		//if valid (within the genDistance)
		if (Vector3.Distance(newTrOct.transform.position, Vector3.zero) < genDistance)
		{
			//set old connection to connected
			_trOct.GetComponent<TruncOct>().connections[_connection/*newConnection*/] = TruncOct.connectionState.Connected;
			_trOct.GetComponent<TruncOct>().connectionObjects[_connection/*newConnection*/] = newTrOct.GetComponent<TruncOct>().trOctNo;

			newTrOct.GetComponent<TruncOct>().connections[newConnection/*_connection*/] = TruncOct.connectionState.Connected;
			newTrOct.GetComponent<TruncOct>().connectionObjects[newConnection/*_connection*/] = _trOct.GetComponent<TruncOct>().trOctNo;

			//link up to pre-existing TrOcts
			LinkTrOcts(newTrOct);

			//add to the global list
			newTrOct.transform.parent = allTrOcts.transform;
			truncOcts.Add(newTrOct);
		}
		else
		{
			//if not valid
			_trOct.GetComponent<TruncOct>().connections[_connection/*newConnection*/] = TruncOct.connectionState.CantConnect;
			Destroy(newTrOct);
		}

		//if deadtrocts, then perhaps make a dead troct, but there cannot be more than 3 dead trocs consecutively
		if (deadSpace)
		{
			//1 in 20 chance of being a dead troct
			int randTest, random = Random.Range(0, 20);
			Random.seed = System.DateTime.Now.Millisecond;
			randTest = Random.Range (0,20);
			
			if (random == randTest)
			{
				//make sure that this is not connected to more than 3 other dead trocts
				if (linkedToDead(newTrOct, 0) <= 4)
				{
					newTrOct.GetComponent<TruncOct>().type = TruncOct.tileType.dead;
					
					//add to the list to be altered further later
					deadTrocts.Add(newTrOct);
				}
			}
		}
		return;
	}


	void LinkTrOcts(GameObject _trOct)
	{
		for (int i = 0; i < truncOcts.Count; i++)
		{
			float dist = Vector3.Distance(_trOct.transform.position, truncOcts[i].transform.position);

			if (dist <= 4.1f)
			{
				Vector3 direction = truncOcts[i].transform.position - _trOct.transform.position;
				//find out if it is U||D||L||R||F||B

				//thisOne is the connection of the current (_trOct), and thatOne is from the list
				int thisOne = 15, thatOne = 15;

				//up (this face0, that face 5)
				if (direction == new Vector3(0f, 4f, 0f))
				{
					thisOne = 0;
					thatOne = 5;
				}
				else if (direction == new Vector3(0f, -4f, 0f))
				{
					thisOne = 5;
					thatOne = 0;
				}
				//forward (this face 1, that face 3)
				else if (direction == new Vector3(0f, 0f, 4f))
				{
					thisOne = 1;
					thatOne = 3;
				}
				else if (direction == new Vector3(0f, 0f, -4f))
				{
					thisOne = 3;
					thatOne = 1;
				}
				//right (this face 2, that face 4)
				else if (direction == new Vector3(4f, 0f, 0f))
				{
					thisOne = 2;
					thatOne = 4;
				}
				else if (direction == new Vector3(-4f, 0f, 0f))
				{
					thisOne = 4;
					thatOne = 2;
				}
				//others
				else if (direction == new Vector3(-2f, 2f, 2f))
				{
					thisOne = 8;
					thatOne = 11;
				}
				else if (direction == new Vector3(2f, -2f, -2f))
				{
					thisOne = 11;
					thatOne = 8;
				}
				else if (direction == new Vector3(2f, 2f, 2f))
				{
					thisOne = 9;
					thatOne = 12;
				}
				else if (direction == new Vector3(-2f, -2f, -2f))
				{
					thisOne = 12;
					thatOne = 9;
				}
				else if (direction == new Vector3(2f, 2f, -2f))
				{
					thisOne = 6;
					thatOne = 13;
				}
				else if (direction == new Vector3(-2f, -2f, 2f))
				{
					thisOne = 13;
					thatOne = 6;
				}
				else if (direction == new Vector3(2f, -2f, 2f))
				{
					thisOne = 10;
					thatOne = 7;
				}
				else if (direction == new Vector3(-2f, 2f, -2f))
				{
					thisOne = 7;
					thatOne = 10;
				}

				_trOct.GetComponent<TruncOct>().connections[thisOne] = TruncOct.connectionState.Connected;
				_trOct.GetComponent<TruncOct>().connectionObjects[thisOne] = truncOcts[i].GetComponent<TruncOct>().trOctNo;

				truncOcts[i].GetComponent<TruncOct>().connections[thatOne] = TruncOct.connectionState.Connected;
				truncOcts[i].GetComponent<TruncOct>().connectionObjects[thatOne] = _trOct.GetComponent<TruncOct>().trOctNo;
			}
		}
	}

	//takes the input of a connection and returns the connection of the adjoining trOct at the same pos.
	int ConvertConnecton(int _connection)
	{
		int connecting = 15;

		switch (_connection)
		{
		case 0:
			connecting = 5;
			break;

		case 1:
			connecting = 3;
			break;

		case 2:
			connecting = 4;
			break;

		case 3:
			connecting = 1;
			break;

		case 4:
			connecting = 2;
			break;

		case 5:
			connecting = 0;
			break;

		case 6:
			connecting = 13;
			break;

		case 7:
			connecting = 10;
			break;

		case 8:
			connecting = 11;
			break;

		case 9:
			connecting = 12;
			break;

		case 10:
			connecting = 7;
			break;

		case 11:
			connecting = 8;
			break;

		case 12:
			connecting = 9;
			break;

		case 13:
			connecting = 6;
			break;
		}

		return connecting;
	}

	//loops through ALL trOcts and if a null connection is found, false is returned.
	bool connectionsFull ()
	{
		for (int i = 0; i < truncOcts.Count; i++)
		{
			for (int j = 0; j < 14; j++)
			{
				if (truncOcts[i].GetComponent<TruncOct>().connections[j] == TruncOct.connectionState.NotConnected)
				{
					return false;
				}
			}
		}

		//if code reaches here, then all connections are full
		return true;
	}

	//takes the input troct and tests if it connects to a deadtroct, if it does then increment the value and call linkedToDead for that deadtroct
	int linkedToDead(GameObject _trOct, int _value)
	{
		//i goes through each connection searching for a dead troct
		for (int i = 0; i < 14; i++)
		{
			//first test if the connection is even connected
			if (_trOct.GetComponent<TruncOct>().connections[i] == TruncOct.connectionState.Connected)
			{
				//if the truncOct that corresponds to this connection is dead
				if (truncOcts[_trOct.GetComponent<TruncOct>().connectionObjects[i]].GetComponent<TruncOct>().type == TruncOct.tileType.dead)
				{
					if (_value <= 4)
					{
						//call this function and add value
						_value = linkedToDead(truncOcts[_trOct.GetComponent<TruncOct>().connectionObjects[i]], ++_value);
					}
				}
			}
		}

		return _value;

	}

	public void ChangeAlpha(float _alpha)
	{
		foreach (GameObject troct in truncOcts)
		{
			Color tempCol = troct.GetComponent<MeshRenderer> ().material.color;
			tempCol.a = _alpha;
			
			troct.GetComponent<MeshRenderer> ().material.color = tempCol;
		}
	}
}