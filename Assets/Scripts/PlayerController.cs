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
	ControlKeySet m_myControlKeySet;

	static float timeLifeBombWithRemote = 10f;
	float timeToDetonateNextBomb = 1.0f;
	float countdownRemote;
	float timeToSetNextBomb = 0.2f;
	float countdownSetBomb;

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
		if (index == 0)
			m_myControlKeySet = new LeftHandControlKeySet ();
		else
			m_myControlKeySet = new RightHandControlKeySet ();
	}

	public int GetPlayerIndex()
	{
		return indexPlayer;
	}

	void FixedUpdate()
	{
		//if (LevelManager.GetInstance ().IsGamePause ())
		//	return;
		if (!isAlive)
			return;

		m_myControlKeySet.UpdateKey();
		

		float h = 0;
		float v = 0;
		if (m_myControlKeySet.IsMoveUp())
			v = 1;
		if (m_myControlKeySet.IsMoveDown())
			v = -1;
		if (v == 0) {
			if (m_myControlKeySet.IsMoveLeft())
				h = -1;
			if (m_myControlKeySet.IsMoveRight())
				h = 1;
		}

		// Move the player around the scene.
		if (isFastRunner && countdownSuperSpeed > 0) {
			countdownSuperSpeed -= Time.deltaTime;

		} else
			isFastRunner = false;

		if (countdownRemote > 0)
			countdownRemote -= Time.deltaTime;

		if (countdownSetBomb > 0)
			countdownSetBomb -= Time.deltaTime;
	
		Move (h, v);

		if (m_myControlKeySet.IsSetBomb() && !isSettingABomb && countdownSetBomb <= 0) 
		{		
			countdownSetBomb = timeToSetNextBomb;
			SetBomb ();
		}
		if (isGotRemoteController && m_myControlKeySet.IsDectonated() && countdownRemote <= 0) {
			countdownRemote = timeToDetonateNextBomb;
			DetonateABomb ();
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
			//Debug.Log ("BBB - [" + i + "] = " + myBombs [i] != null );
			if (countBomb < totalBomb && myBombs [i] == null) {
				//Debug.Log ("BBB - oki at i = " + i);
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
		case 
			PickUps.TypePickUp.LonggerBombBlast:
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

class ControlKeySet
{
	int m_iFlagKeyControl = 0;
	const int c_iFlagMoveUp = 		1;
	const int c_iFlagMoveDown = 	1 << 1;
	const int c_iFlagMoveLeft = 	1 << 2;
	const int c_iFlagMoveRight = 	1 << 3;
	const int c_iFlagSetBomb = 		1 << 4;
	const int c_iFlagDetonate = 	1 << 5;

	protected KeyCode m_KeyMoveUp;
	protected KeyCode m_KeyMoveDown;
	protected KeyCode m_KeyMoveLeft;
	protected KeyCode m_KeyMoveRight;
	protected KeyCode m_KeySetBomb;
	protected KeyCode m_KeyDectonate;

	public ControlKeySet()
	{
		m_iFlagKeyControl = 0;
		SetUpKeySet ();
	}

	protected virtual void SetUpKeySet()
	{
		m_KeyMoveUp = KeyCode.UpArrow;
		m_KeyMoveDown = KeyCode.DownArrow;
		m_KeyMoveLeft = KeyCode.LeftArrow;
		m_KeyMoveRight = KeyCode.RightArrow;
		m_KeySetBomb = KeyCode.RightControl;
		m_KeyDectonate = KeyCode.RightAlt;
	}

	public bool IsMoveUp()
	{
		return (m_iFlagKeyControl & c_iFlagMoveUp) != 0;
	}
	public bool IsMoveDown()
	{
		return (m_iFlagKeyControl & c_iFlagMoveDown) != 0;
	}
	public bool IsMoveLeft()
	{
		return (m_iFlagKeyControl & c_iFlagMoveLeft) != 0;
	}
	public bool IsMoveRight()
	{
		return (m_iFlagKeyControl & c_iFlagMoveRight) != 0;
	}
	public bool IsSetBomb()
	{
		return (m_iFlagKeyControl & c_iFlagSetBomb) != 0;
	}
	public bool IsDectonated()
	{
		return (m_iFlagKeyControl & c_iFlagDetonate) != 0;
	}
		

	public void UpdateKey()
	{
		m_iFlagKeyControl = 0;
		if (Input.GetKey(m_KeyMoveUp))
			m_iFlagKeyControl = m_iFlagKeyControl | c_iFlagMoveUp;
		if (Input.GetKey(m_KeyMoveDown))
			m_iFlagKeyControl = m_iFlagKeyControl | c_iFlagMoveDown;
		if (Input.GetKey(m_KeyMoveLeft))
			m_iFlagKeyControl = m_iFlagKeyControl | c_iFlagMoveLeft;
		if (Input.GetKey(m_KeyMoveRight))
			m_iFlagKeyControl = m_iFlagKeyControl | c_iFlagMoveRight;
		if (Input.GetKey(m_KeySetBomb))
			m_iFlagKeyControl = m_iFlagKeyControl | c_iFlagSetBomb;
		if (Input.GetKey(m_KeyDectonate))
			m_iFlagKeyControl = m_iFlagKeyControl | c_iFlagDetonate;
	}
}

class LeftHandControlKeySet: ControlKeySet
{
	protected override void SetUpKeySet()
	{
		m_KeyMoveUp = KeyCode.W;
		m_KeyMoveDown = KeyCode.S;
		m_KeyMoveLeft = KeyCode.A;
		m_KeyMoveRight = KeyCode.D;
		m_KeySetBomb = KeyCode.B;
		m_KeyDectonate = KeyCode.N;
	}
}

class RightHandControlKeySet: ControlKeySet
{
	protected override void SetUpKeySet()
	{
		m_KeyMoveUp = KeyCode.Keypad8;
		m_KeyMoveDown = KeyCode.Keypad5;
		m_KeyMoveLeft = KeyCode.Keypad4;
		m_KeyMoveRight = KeyCode.Keypad6;
		m_KeySetBomb = KeyCode.Keypad7;
		m_KeyDectonate = KeyCode.Keypad9;
	}
}