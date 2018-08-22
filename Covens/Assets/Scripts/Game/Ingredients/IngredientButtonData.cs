using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class IngredientButtonData : MonoBehaviour
{

	public IngredientType ingType;
	public string ID;
	public Text text;

	public void Setup(string title, int count,string id)
	{
		if (title == "")
			return;
		ID = id;
		if (count == 0)
			Destroy (gameObject);
		
		if(count>1)
		text.text = title + " (" + count.ToString () + ")";
		else
			text.text = title;
	}

}

