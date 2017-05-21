using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextHighScore : MonoBehaviour {
	//static string textToDisplay;
	Text text;
	static TextHighScore inst;

	void Awake()
	{
		inst = this;
		text = GetComponent<Text> ();
	}

	public static TextHighScore getInstance()
	{
		return inst;
	}

	public void SetDisplayText(string str)
	{
		text.text = str;
	}
}
