using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class LocalizeLookUp : MonoBehaviour {
	Text t;
	void Awake()
	{
		t = GetComponent<Text> ();
		LocalizationManager.OnChangeLanguage += RefreshText;
	}

	void OnDestroy(){
		LocalizationManager.OnChangeLanguage -= RefreshText;
	}


	void RefreshText()
	{
		
	}
	// Update is called once per frame
	void Update () {
		
	}
}
