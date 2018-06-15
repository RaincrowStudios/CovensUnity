using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellCarousel : MonoBehaviour {

	public static SpellCarousel Instance { get; set;}

	public Text spellTitle;
	public Text spellCost;
	public Text spellDiscription;
	public static string currentSpell;
	public GameObject spellButton;

	public GameObject shadowP;
	public GameObject whiteP;
	public GameObject greyP;
	CanvasGroup CG;

	void Awake()
	{
		Instance = this;
	}

	void Start () {
		CG = GetComponentInParent<CanvasGroup> ();
		Invoke ("enableSpellButtons", 1f);
	}

	void enableSpellButtons()
	{
		if (spellButton == null)
			return;
		spellButton.SetActive (true);	
	}
	// Update is called once per frame
	void Update () {
		
	}

	public void SpellInfo(string spellName)
	{
		SpellData sd = SpellCastAPI.spells [spellName];
		spellTitle.text = sd.id; 
		ChangeColor (sd.school);
		if (spellName == "spell_attack" || spellName == "spell_ward") {
			spellCost.text = "Cost: Channel Energy";
		}else
		spellCost.text = "Cost: " + sd.cost.ToString () + " Energy";
		spellDiscription.text = sd.id + " : Spell Description here";
	}

	public void ChangeColor(int i)
	{
		if (CG.alpha != 1)
			return;
		whiteP.SetActive (false);
		shadowP.SetActive (false);
		greyP.SetActive (false);

		if (i == 1) {
//			spellTitle.color = Utilities.Orange;
			whiteP.SetActive (true);
		} else if (i == -1) {
//			spellTitle.color = Utilities.Purple;
			shadowP.SetActive (true);
		} else {
//			spellTitle.color = Utilities.Blue;
			greyP.SetActive (true);
		}
	}
}
