using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUps : MonoBehaviour {

	public float rotateSpeed = 40.0f;
	public enum TypePickUp {
		LonggerBombBlast,
		MoreBomb,
		FastRunner,
		RemoteControl,
	};

	protected TypePickUp myType;

	/// <summary>
	/// It's posible to apply inheritance here by create a class Pickup with the common functions and variables 
	/// then let it be inherited by child class such as Morebobm, FastRunner ...
	/// Btw, applying inheritance for such a simple class/simple task like this is quite redundant.
	/// We can replace it by a simple switch statement in Start()
	/// </summary>
	void Start () {
		myType = (TypePickUp)Random.Range (0, 4);
		Color c = Color.red;
		switch (myType) {
		case TypePickUp.LonggerBombBlast:
			c = Color.red;
			break;
		case TypePickUp.MoreBomb:
			c = Color.black;
			break;
		case TypePickUp.FastRunner:
			c = Color.yellow;
			break;
		case TypePickUp.RemoteControl:
			c = Color.blue;
			break;
		}
			
		Renderer r = GetComponent<Renderer> ();
		r.material.color = c;

	}
	
	// Update is called once per frame
	void Update () {
		if (LevelManager.GetInstance ().IsGamePause ())
			return;
		if (LevelManager.GetInstance ().IsCountingForNewMatch())
		{
			Destroy (gameObject, 0.1f);
			return;
		}
		else
			transform.Rotate (new Vector3(0, rotateSpeed * Time.deltaTime, rotateSpeed * Time.deltaTime));
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player") {
			PlayerController pc = other.gameObject.GetComponent<PlayerController>();
			pc.AccquireItem (myType);
			Destroy (gameObject);
		}
	}
}