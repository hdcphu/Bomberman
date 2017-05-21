using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextCountDown : MonoBehaviour {

	Text text;
	static TextCountDown inst;

	void Awake()
	{
		inst = this;
		text = GetComponent<Text> ();
	}

	public static TextCountDown getInstance()
	{
		return inst;
	}

	public void SetDisplayText(string str)
	{
		text.text = str;
	}
}
