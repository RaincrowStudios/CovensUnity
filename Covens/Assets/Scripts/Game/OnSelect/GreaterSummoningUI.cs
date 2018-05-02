using UnityEngine;
using UnityEngine.UI;

public class GreaterSummoningUI : MonoBehaviour {
	
	public static GreaterSummoningUI Instance {get; set;}
	public GameObject[] TurnOff;
	public GameObject closeSummon;
	public GameObject closeSummonRes;
	public GameObject spellCastCanvas;
	public GameObject Ingredients;
	public GameObject SummonSpellCast;
	public Text resultText; 
	public GameObject resultScreen;
	public GameObject GreaterSummonIngCast;
	public CanvasGroup fadeBlack;
	public SpellTraceManager STM;

	public CanvasGroup castSpell;
	public CanvasGroup spellCastBG;


	void Awake(){
		Instance = this;
	}

	public void OnStartSummon()
	{
		foreach (var item in TurnOff) {
			item.SetActive (false);
		}
		fadeBlack.alpha = 1;
		spellCastBG.alpha = 1;
		castSpell.alpha = 0;
		castSpell.interactable = false;
		closeSummonRes.SetActive (true);
		spellCastCanvas.SetActive (true);
		Ingredients.SetActive (true);
		closeSummon.SetActive (true);
		GreaterSummonIngCast.SetActive (true);
		Utilities.allowMapControl (false);
		SummonSpellCast.SetActive (false);
	}

	public void OnFinishAddIng()
	{
		STM.enabled = true;
		Ingredients.SetActive (false);

		SummonSpellCast.SetActive (true);
	}

	public void SpellCastDone()
	{
		STM.enabled = false;
		spellCastCanvas.SetActive (false);
		resultScreen.SetActive (true);
		resultText.text = "Portal Created";
		GreaterSummonIngCast.SetActive (false);
		SpellCastAPI.CastSummon ();
	}

	public void close()
	{
		foreach (var item in TurnOff) {
			item.SetActive (true);
		}
		SummonSpellCast.SetActive (false);
		closeSummonRes.SetActive (false);
		closeSummon.SetActive (false);
		Utilities.allowMapControl (true);
		fadeBlack.alpha = 0;
		castSpell.alpha = 1;
		spellCastBG.alpha = 1;
		castSpell.interactable = true;
	}
}
