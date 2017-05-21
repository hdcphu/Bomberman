using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour {

	public float minSize = 0.8f;
	public float maxSize = 1.0f;

	public float timeLife = 3.0f;

	public float growSpeed = 0.25f;

	public GameObject prefExplotion; 
	int sizeExplosion = 1;

	GroundMaganger groundManager;

	public GameObject gameMangagers;
	static GameObject gm = null;

	bool isDetonated;

	void Awake()
	{
		if (gm == null && gameMangagers != null)
			gm = gameMangagers;
	}

	// Use this for initialization
	void Start () {
		isDetonated = false;
		
	}
	
	// Update is called once per frame
	void Update () {
		if (LevelManager.GetInstance ().IsGamePause ())
			return;
		if (timeLife <= 0)
			return;
			
		if (transform.localScale.x > maxSize || transform.localScale.x < minSize)
			growSpeed = -growSpeed;
		float t = transform.localScale.x + growSpeed * Time.deltaTime;
		transform.localScale = new Vector3 (t, t, t);

		timeLife -= Time.deltaTime;
		if (timeLife <= 0)
			Detonate ();
	}

	public void Detonate()
	{
		if (isDetonated)
			return;
		isDetonated = true;

		Destroy (gameObject, 0.1f);
		int i = (int)System.Math.Round (transform.position.z, 0);
		int j = (int)System.Math.Round (transform.position.x, 0);

		Explose (i, j, new Vector2 (1, 0), sizeExplosion);
		Explose (i, j, new Vector2 (-1, 0), sizeExplosion);
		Explose (i, j, new Vector2 (0, 1), sizeExplosion);
		Explose (i, j, new Vector2 (0, -1), sizeExplosion);
	}

	void Explose(int i, int j, Vector2 dir, int size)
	{		
		Instantiate (prefExplotion, new Vector3 (j, 0.5f, i), Quaternion.identity);
		if (size > 1) {
			int ni = (int)System.Math.Round (i + dir.x, 0);
			int nj = (int)System.Math.Round (j + dir.y, 0);
			GroundMaganger.mapValue v = GroundMaganger.GetInstance ().GetMapValueAt (ni, nj);
			if (v != GroundMaganger.mapValue.metal && v != GroundMaganger.mapValue.brick) {
				Explose (ni, nj, dir, size - 1);
			}
			if (v == GroundMaganger.mapValue.brick) {
				GroundMaganger.GetInstance ().DestroyAt (ni, nj);
			}
		}
	}
	void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag == "Player") {
			gameObject.GetComponent<Collider> ().isTrigger = false;
			PlayerController pc = other.gameObject.GetComponent<PlayerController>();
			pc.DoneWithTheBomb ();
		}
	}
	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player") {
			PlayerController pc = other.gameObject.GetComponent<PlayerController>();
			pc.DoneWithTheBomb ();
		}
	}

	public void setSizeBombBlast(int size)
	{
		sizeExplosion = size;
	}

	public void setTimeLife(float time)
	{
		timeLife = time;
	}

	public float getTimeLife()
	{
		return timeLife;
	}
}
