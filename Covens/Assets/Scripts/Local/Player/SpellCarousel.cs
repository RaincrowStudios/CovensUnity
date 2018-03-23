using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellCarousel : MonoBehaviour {


	public Text spellTitle;
	public Text spellCost;
	public Text spellDiscription;
	public static string currentSpell;
	public GameObject spellButton;

	public GameObject shadowP;
	public GameObject whiteP;
	public GameObject greyP;
	CanvasGroup CG;
	void Start () {
		CG = GetComponentInParent<CanvasGroup> ();
		Invoke ("enableSpellButtons", 1f);
	}

	void enableSpellButtons()
	{
		spellButton.SetActive (true);	
	}
	// Update is called once per frame
	void Update () {
		
	}

	public void SpellInfo(string spellName)
	{
		spellTitle.text = spellName; 

		if (spellName == "Hex") {
			spellDiscription.text = Constants.hexDiscription;
			spellCost.text = "Cost : " + Constants.hexCost.ToString() +" Energy";
			ChangeColor (2);
		}

		if (spellName == "Bless") {
			spellDiscription.text = Constants.blessDiscription;
			spellCost.text = "Cost : " + Constants.hexCost.ToString() +" Energy";
			ChangeColor (1);
		}

		if (spellName == "Grace") {
			spellDiscription.text = Constants.graceiscription;
			spellCost.text = "Cost : " + Constants.graceCost.ToString() +" Energy";
			ChangeColor (1);
		}

		if (spellName == "Ressurect") {
			spellDiscription.text = Constants.ressurectiscription;
			spellCost.text = "Cost : " + Constants.ressurectCost.ToString() +" Energy";
			ChangeColor (2);
		}

		if (spellName == "WhiteFlame") {
			spellDiscription.text = Constants.whiteFlameDiscription;
			spellCost.text = "Cost : " + Constants.whiteFlameCost.ToString() +" Energy";
			ChangeColor (1);
		}

		if (spellName == "SunEater") {
			spellDiscription.text = Constants.sunEaterDiscription;
			spellCost.text = "Cost : " + Constants.sunEaterCost.ToString() +" Energy";
			ChangeColor (2);
		}

		if (spellName == "Bind") {
			spellDiscription.text = Constants.bindDiscription;
			spellCost.text = "Cost : " + Constants.bindCost.ToString() +" Energy";
			ChangeColor (0);
		}

		if (spellName == "Banish") {
			spellDiscription.text = Constants.banishDiscription;
			spellCost.text = "Cost : " + Constants.banishCost.ToString() +" Energy";
			ChangeColor (2);
		}

		if (spellName == "Silence") {
			spellDiscription.text = Constants.silenceDiscription;
			spellCost.text = "Cost : " + Constants.silenceCost.ToString() +" Energy";
			ChangeColor (1);
		}

		if (spellName == "Waste") {
			spellDiscription.text = Constants.wasteDiscription;
			spellCost.text = "Cost : " + Constants.wasteCost.ToString() +" Energy";
			ChangeColor (2);
		}


	}

	public void ChangeColor(int i)
	{
		if (CG.alpha != 1)
			return;
		whiteP.SetActive (false);
		shadowP.SetActive (false);
		greyP.SetActive (false);

		if (i == 1) {
			spellTitle.color = Utilities.Orange;
			whiteP.SetActive (true);
		} else if (i == 2) {
			spellTitle.color = Utilities.Purple;
			shadowP.SetActive (true);
		} else {
			spellTitle.color = Utilities.Blue;
			greyP.SetActive (true);
		}
	}
}
