using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class IngredientButtonData : MonoBehaviour
{
	[HideInInspector]
	public MarkerSpawner.MarkerType type;

	public ParticleSystem pS;

	public void AddItem()
	{
		IngredientsManager.Instance.AddItem (name,type,GetComponent<Text>(),pS);
		transform.GetChild (0).gameObject.SetActive (true);
	}
}

