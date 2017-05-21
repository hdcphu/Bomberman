using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelCountdown : MonoBehaviour {

	float m_fTimeCountdown;
	public Text m_textCountdown;
	static Vector3 v3MaxScale = new Vector3 (1f, 1f );
	static Vector3 v3MinScale = new Vector3 (1f, 0f );
	enum State {
		Idle,
		FlyIn,
		Countdown,
		FlyOut
	};

	State m_stateCurrent;

	void Awake()
	{
		m_fTimeCountdown = 0;
		m_stateCurrent = State.Idle;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (m_fTimeCountdown > 0) {
			m_fTimeCountdown -= Time.deltaTime;
		}

		switch (m_stateCurrent) {
		case State.FlyIn:
			UpdateFlyin ();
			break;
		case State.Countdown:
			UpdateCountdown ();
			break;
		case State.FlyOut:
			UpdateFlyout ();
			break;
		case State.Idle:
		default:
			break;
		}

	}

	void UpdateFlyin()
	{
		if (gameObject.transform.localScale.y < 0.9) {
			gameObject.transform.localScale = Vector3.Lerp (gameObject.transform.localScale, v3MaxScale, Time.deltaTime * 20);

		} else {
			gameObject.transform.localScale = v3MaxScale;
			m_stateCurrent = State.Countdown;
		}
	}

	void UpdateFlyout()
	{
		if (gameObject.transform.localScale.y > 0.1) {
			gameObject.transform.localScale = Vector3.Lerp (gameObject.transform.localScale, v3MinScale, Time.deltaTime * 20);

		} else {
			gameObject.transform.localScale = v3MinScale;
			m_stateCurrent = State.Idle;
			gameObject.SetActive (false);
		}
	}

	void UpdateCountdown()
	{

		if (m_fTimeCountdown > 3)
			m_textCountdown.text = "Ready";
		else if (m_fTimeCountdown > 1)
			m_textCountdown.text = "" + Mathf.RoundToInt (m_fTimeCountdown);
		else if (m_fTimeCountdown > 0)
			m_textCountdown.text = "GO";
		else { // <= 0
			m_stateCurrent = State.FlyOut;
		}

	}

	public void StartCountDown(float time)
	{
		gameObject.SetActive (true);
		m_fTimeCountdown = time;
		m_stateCurrent = State.FlyIn;
	}

}
