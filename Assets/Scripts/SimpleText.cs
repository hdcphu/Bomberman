using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class is designed to help us access directly to the Text UI to which its attached.
/// </summary>

public class SimpleText: MonoBehaviour {

	protected Text text;
	protected static SimpleText inst;

	protected void Awake()
	{
		text = GetComponent<Text> ();
	}

	public SimpleText()
	{
		inst = this;
	}

	public static SimpleText getInstance()
	{
		if (inst == null)
			inst = new SimpleText ();
		return inst;
	}

	public void SetDisplayText(string str)
	{
		text.text = str;
	}
}
