using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextScore : MonoBehaviour {

	//static string textToDisplay;
	Text text;
	static TextScore instTextScore;
	public static TextScore getInstance()
	{
		return instTextScore;
	}

	void Awake()
	{
		instTextScore = this;
		text = GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetScore(int score1, int score2)
	{
		text.text = "" + score1 + ":" + score2;
	}


}
