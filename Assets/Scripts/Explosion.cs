using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {

	public float lifeTime = 0.5f;

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Bomb") {
			Bomb b = (Bomb) other.gameObject.GetComponent<Bomb>();
			b.Detonate ();
		}
		if (other.gameObject.tag == "Item") {
			Destroy (other.gameObject);
		}
		else if (other.gameObject.tag == "Player") {
			PlayerController pc = (PlayerController) other.gameObject.GetComponent<PlayerController>();
			pc.PlayerDie ();
		}
	}

	// Update is called once per frame
	void Update () {
		if (LevelManager.GetInstance ().IsGamePause ())
			return;
		lifeTime -= Time.deltaTime;
		if (lifeTime <= 0)
			Destroy (gameObject);
	}
}
