using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class summonIngredientData : MonoBehaviour
{
	[HideInInspector]
	public string id;
	[HideInInspector]
	public IngredientType type;
	public Text title;
	public Action<summonIngredientData> onSelectItem;

	public void OnClick()
	{
		onSelectItem (this);
	}
}

