using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundMaganger : MonoBehaviour {

	public int height = 20;
	public int width = 40;
	public float densityBrick = 0.5f;

	GameObject[] mapObjects;

	public GameObject prefBrick;
	public GameObject prefMetal;
	public GameObject perfItem;

	public float rateDropItem = 0.3f;

	public enum mapValue {
		empty = 0,
		brick,
		metal,
		bomb,
		item,
		spawnPoint
	};

	mapValue[] mapGround;
	static GroundMaganger instGroundManager = null;
	Vector3[] spawnPoints;
	int maxSpawnPoint = 2;
	void Awake()
	{
		instGroundManager = this;
		mapGround = new mapValue[height * width];
		mapObjects = new GameObject[height * width];
		for (int i = 0; i < mapObjects.Length; i++) {
			mapObjects [i] = null;
		}

		spawnPoints = new Vector3[maxSpawnPoint];
		spawnPoints [0] = new Vector3 (0f, 0.5f, 0f);
		spawnPoints [1] = new Vector3 (width - 1, 0.5f, height - 1);
	}


	public void NewMap()
	{

		GenerateRandomMap ();
		BuildMap ();
	}

	public void ClearMap()
	{
		for (int i = 0; i < height; i++) {
			for (int j = 0; j < width; j++) {
				int t = i * width + j;
					if (mapObjects [t] != null)
						Destroy (mapObjects [t]);
					mapGround [t] = mapValue.empty;
			}
		}
	}

	void GenerateRandomMap()
	{
		for (int i = 0; i < height; i++) {
			for (int j = 0; j < width; j++)
			{
				
				if (i % 2 == 1 && j % 2 == 1) {
					mapGround [i * width + j] = mapValue.metal;
				} else {
					if (Random.value < densityBrick)
						mapGround [i * width + j] = mapValue.brick;
					else
						mapGround [i * width + j] = mapValue.empty;
				}
			}
		}

		//reserve space to spawn player 1
		mapGround [0] = mapValue.empty;
		mapGround [1] = mapValue.empty;
		mapGround [width] = mapValue.empty;

		//reserve space to spawn player 2
		mapGround [height * width - 1] = mapValue.empty;
		mapGround [height * width - 2] = mapValue.empty;
		mapGround [height * width - width - 1] = mapValue.empty;
	}

	void BuildMap()
	{
		for (int i = 0; i < height; i++) {
			for (int j = 0; j < width; j++) 
			{
				int t = i * width + j;
				//Vector3 a = {}
				if (mapGround [t] == mapValue.empty) {
					if (mapObjects [t] != null)
						Destroy (mapObjects [t]);
				} else if (mapGround [t] == mapValue.brick) {
					if (mapObjects [t] == null)
						mapObjects [t] = Instantiate (prefBrick, new Vector3 (j, 0.5f, i), Quaternion.identity);
				}
				else if (mapGround [t] == mapValue.metal) {
					if (mapObjects [t] == null)
						mapObjects [t] = Instantiate (prefMetal, new Vector3 (j, 0.5f, i), Quaternion.identity);
				}
			}
		}
	}

	void SpawnPlayers()
	{
	}

	public mapValue GetMapValueAt(int i, int j)
	{
		if (i < 0 || j < 0 || i >= height || j >= width)
			return mapValue.metal;
		else 
			return mapGround [i * width + j];
	}

	public void DestroyAt (int i, int j)
	{
		Destroy (mapObjects [i * width + j], 0.5f);
		mapGround [i * width + j] = mapValue.empty;
		if (Random.value < rateDropItem)
			Instantiate (perfItem, new Vector3 (j, 0.5f, i), Quaternion.identity);
	}


	public Vector3 GetSpawnPoint(int index)
	{
		if (index < 0 || index >= maxSpawnPoint)
			return spawnPoints [0];
		return spawnPoints [index];
	}

	public static GroundMaganger GetInstance()
	{
		return instGroundManager;
	}
}
