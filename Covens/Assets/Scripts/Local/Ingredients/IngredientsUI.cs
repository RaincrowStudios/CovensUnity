using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class IngredientsUI : MonoBehaviour
{

	public static IngredientsUI Instance { get; set;}

	public GameObject Ingredients;
	public GameObject ingInfo;
	public GameObject skipIngredients;
	public GameObject ListObject;
	public Text CastSpell;

	SpellSelect Sp;

	void Awake()
	{
		Instance = this;
	}

	public void OnClick(SpellSelect sp)
	{
		Sp = sp;
		Ingredients.SetActive (true);
		ListObject.SetActive (false);
		ingInfo.SetActive (true);
		skipIngredients.SetActive (true);
		CastSpell.gameObject.SetActive (false);
		CastSpell.text = "Cast : " + Sp.name;
	}

	public void CloseIngredients ()
	{
		Ingredients.GetComponent<Fade> ().FadeOutHelper();
		Sp.onClick ();
	}

	public void CloseIngredientsSummon ()
	{
		Ingredients.GetComponent<Fade> ().FadeOutHelper();
	}
}

