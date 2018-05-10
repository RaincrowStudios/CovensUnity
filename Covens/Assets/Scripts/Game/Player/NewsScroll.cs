using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewsScroll : MonoBehaviour {
	public static NewsScroll Instance { get; set;}
	public Text Previous;
	public Text New;
	public GameObject scrollContainer;
	string curText ="";
	void Awake()
	{
		Instance = this;
		Previous.text = "";
		New.text = "";
	}

	public void ShowText(string text)
	{
		this.CancelInvoke ();
		Animate ();
		New.text = text;
		curText = text;
		Invoke ("AddOldText", .28f);
	}

	void AddOldText()
	{
		Previous.text = curText;
	}

	void Animate()
	{
		scrollContainer.SetActive (false);
		scrollContainer.SetActive (true);
	}
}
