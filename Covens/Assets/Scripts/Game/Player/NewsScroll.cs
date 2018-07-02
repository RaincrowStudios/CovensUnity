using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewsScroll : MonoBehaviour {
	public static NewsScroll Instance { get; set;}
	public Text Previous;
	public Text New;
	public GameObject scrollContainer;
	public Image IconOld;
	public Image IconNew;

	public Sprite InfoIcon;
	public Sprite ChatIcon;

	string curText ="";
	Sprite curSp ;
	void Awake()
	{
		Instance = this;
		Previous.text = "";
		New.text = "";
	}

	public void ShowText(string text, bool isChat = false)
	{
		this.CancelInvoke ();
		Animate ();
		New.text = text;
		curText = text;
		if (!isChat) {
			IconNew.sprite = InfoIcon;
			curSp = InfoIcon; 
		} else {
			IconNew.sprite = ChatIcon;
			curSp = ChatIcon; 
		}
		Invoke ("AddOldText", .28f);
	}

	void AddOldText()
	{
		Previous.text = curText;
		IconOld.sprite = curSp; 
	}

	void Animate()
	{
		scrollContainer.SetActive (false);
		scrollContainer.SetActive (true);
	}
}
