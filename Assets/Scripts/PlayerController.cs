using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float speed = 6f;            // The speed that the player will move at.
	public float speedSuper = 18f;
	public float timeSuperSpeed = 5.0f;
	float countdownSuperSpeed;

	Vector3 movement;                   // The vector to store the direction of the player's movement.
	Animator anim;                      // Reference to the animator component.
	Rigidbody playerRigidbody;          // Reference to the player's rigidbody.

	public GameObject bombPrefab;

	GameObject[] myBombs;
	public int maxBomb = 20;

	bool isFastRunner;
	bool isGotRemoteController;
	int sizeBombBlast;
	int totalBomb;

	int indexPlayer;

	static float timeLifeBombWithRemote = 10f;
	float timeToDetonateNextBomb = 1.0f;
	float countdownRemote;

	bool isSettingABomb;
	bool isAlive;

	void Awake ()
	{
		playerRigidbody = GetComponent <Rigidbody> ();
		myBombs = new GameObject[maxBomb];
	}

	// Use this for initialization
	void Start () {
		isFastRunner = false;
		isGotRemoteController = false;
		sizeBombBlast = 2;
		totalBomb = 1;
		isSettingABomb = false;
		countdownSuperSpeed = 0;
		countdownRemote = 0;
		isAlive = true;
	}
	
	public void SetPlayerIndex(int index)
	{
		indexPlayer = index;
	}

	public int GetPlayerIndex()
	{
		return indexPlayer;
	}

	void FixedUpdate()
	{
		if (LevelManager.GetInstance ().IsGamePause ())
			return;
		if (!isAlive)
			return;
		
		if (indexPlayer == 0) {
			// Store the input axes.
			float h = 0;
			float v = 0;
			if (Input.GetKey(KeyCode.W))
				v = 1;
			if (Input.GetKey (KeyCode.S))
				v = -1;

			if (Input.GetKey (KeyCode.A))
				h = -1;
			if (Input.GetKey (KeyCode.D))
				h = 1;

			// Move the player around the scene.
			if (isFastRunner && countdownSuperSpeed > 0) {
				countdownSuperSpeed -= Time.deltaTime;

			} else
				isFastRunner = false;

			if (countdownRemote > 0)
				countdownRemote -= Time.deltaTime;
		
			Move (h, v);

			if (Input.GetKey (KeyCode.B) && !isSettingABomb) {
				SetBomb ();
			}
			if (isGotRemoteController && Input.GetKey (KeyCode.N) && countdownRemote <= 0) {
				countdownRemote = timeToDetonateNextBomb;
				DetonateABomb ();
			}
		} else if (indexPlayer == 1) {
			// Store the input axes.
			float h = 0;
			float v = 0;
			if (Input.GetKey (KeyCode.Keypad8))
				v = 1;
			if (Input.GetKey (KeyCode.Keypad5))
				v = -1;

			if (Input.GetKey (KeyCode.Keypad4))
				h = -1;
			if (Input.GetKey (KeyCode.Keypad6))
				h = 1;

			// Move the player around the scene.
			if (isFastRunner && countdownSuperSpeed > 0) {
				countdownSuperSpeed -= Time.deltaTime;

			} else
				isFastRunner = false;

			if (countdownRemote > 0)
				countdownRemote -= Time.deltaTime;

			Move (h, v);

			if (Input.GetKey (KeyCode.Keypad7) && !isSettingABomb) {
				SetBomb ();
			}
			if (isGotRemoteController && Input.GetKey (KeyCode.Keypad9) && countdownRemote <= 0) {
				countdownRemote = timeToDetonateNextBomb;
				DetonateABomb ();
			}
		}
	}

	void Move (float h, float v)
	{
		if (h == 0 && v == 0)
			return;

		// Set the movement vector based on the axis input.
		movement.Set (h, 0f, v);

		if (isFastRunner)
			movement = movement.normalized * speedSuper * Time.deltaTime;
		else
			movement = movement.normalized * speed * Time.deltaTime;

		// Move the player to it's current position plus the movement.
		playerRigidbody.MovePosition (transform.position + movement);

		// Create a quaternion (rotation) based on looking down the vector from the player to the mouse.
		Quaternion newRotation = Quaternion.LookRotation (movement);

		// Set the player's rotation to this new rotation.
		playerRigidbody.MoveRotation (newRotation);
	}

	void SetBomb()
	{
		if (totalBomb > maxBomb)
			totalBomb = maxBomb;
		int countBomb = 0;
		for (int i = 0; i < maxBomb; i++) {
			if (countBomb < totalBomb && myBombs [i] == null) {
				myBombs [i] = Instantiate (bombPrefab, new Vector3 ((float)System.Math.Round (transform.position.x, 0), 0.5f, (float)System.Math.Round (transform.position.z, 0)), Quaternion.identity);
				Bomb bs = myBombs [i].GetComponent<Bomb> ();
				bs.setSizeBombBlast (sizeBombBlast);
				if (isGotRemoteController)
					bs.setTimeLife (timeLifeBombWithRemote);
				
				break;
			} else {
				countBomb++;
			}
		}
	}

	void DetonateABomb()
	{
		//int countBomb = 0;
		int indexOldestBomb = -1;
		float shortestTime = timeLifeBombWithRemote;
		for (int i = 0; i < maxBomb; i++) {
			if (myBombs [i] != null) {
				Bomb bs = myBombs [i].GetComponent<Bomb> ();
				float t = bs.getTimeLife ();
				if (t < shortestTime) {
					indexOldestBomb = i;
					shortestTime = t;
				}
			} 
		}

		if (indexOldestBomb < 0)
			return;
		
		Bomb oldestBomb = myBombs [indexOldestBomb].GetComponent<Bomb> ();
		oldestBomb.Detonate ();
	}

	public void AccquireItem(PickUps.TypePickUp typeItem)
	{
		switch (typeItem) {
		case PickUps.TypePickUp.LonggerBombBlast:
			sizeBombBlast++;
			break;
		case PickUps.TypePickUp.MoreBomb:
			totalBomb++;
			break;
		case PickUps.TypePickUp.FastRunner:
			countdownSuperSpeed = timeSuperSpeed;
			isFastRunner = true;
			break;
		case PickUps.TypePickUp.RemoteControl:
			isGotRemoteController = true;
			break;
		}
	}

	public void DoneWithTheBomb()
	{
		isSettingABomb = false;
	}
	public void SettingTheBomb()
	{
		isSettingABomb = true;
	}


	public void PlayerDie()
	{
		if (!isAlive)
			return;
		isAlive = false;
		LevelManager.GetInstance ().PlayerDie (indexPlayer);
	}
}
