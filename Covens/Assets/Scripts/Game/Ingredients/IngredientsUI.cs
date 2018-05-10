using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class IngredientsUI : MonoBehaviour
{

	public static IngredientsUI Instance { get; set;}
	public Image selectedSpell;
	public GameObject Ingredients;
	public Text ingInfo;
	public GameObject skipIngredients;
	public GameObject ListObject;
	public Text CastSpell;
	bool hasFlashed;

	 SpellSelect Sp;

	void Awake()
	{
		Instance = this;
	}

	public void HideSkipIng(bool show)
	{
		skipIngredients.SetActive (show);
	}

	public void OnClick(SpellSelect sp)
	{
		EventManager.Instance.CallCastingStateChange (SpellCastStates.casting);
		Sp = sp;
		IngredientsManager.Instance.init ();
		Ingredients.SetActive (true);
		ListObject.SetActive (false);
		skipIngredients.SetActive (true);
		selectedSpell.gameObject.SetActive (true);
		selectedSpell.sprite = SpellGlyphs.glyphs [SpellCarousel.currentSpell];
	}

	public void SpellAdded()
	{
		ingInfo.text = "Cast : " + SpellCastAPI.spells[SpellCarousel.currentSpell].displayName;
		ingInfo.GetComponent<Button> ().enabled = true;
		if (!hasFlashed) {
			ingInfo.transform.GetChild (0).gameObject.SetActive (true);
			hasFlashed = true;
		}
	}

	public void SpellInit()
	{
		hasFlashed = false;
		ingInfo.text = "Adding ingredients empowers spells.";
		ingInfo.GetComponent<Button> ().enabled = false;
	}

	public void CloseIngredients ()
	{
		Ingredients.GetComponent<Fade> ().FadeOutHelper();
		Sp.onClick ();
		SpellSelectParent.Instance.ManageScroll (false);
		SpellSelectParent.Instance.CarouselFadeIn ();
	}

	public void CloseIngredientsSummon ()
	{
		Ingredients.GetComponent<Fade> ().FadeOutHelper();
		selectedSpell.gameObject.SetActive (false);
	}
}

