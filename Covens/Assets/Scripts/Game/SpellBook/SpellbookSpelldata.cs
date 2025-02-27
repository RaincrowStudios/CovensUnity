﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;


public class SpellbookSpelldata : MonoBehaviour
{
	public Text energy;
	public Text desc;
	public Text baseDesc;
	public GameObject knowSigs;
	public GameObject sig;
	public Transform sigContainer;
	public SpellData data;
	public void Setup(SpellData sd)
	{
		data = sd; 
		energy.text = "Cost: <color=#000000>" + sd.cost.ToString() + " Energy </color>";
		baseDesc.text = DownloadedAssets.spellDictData [sd.id].spellLore;
		desc.text = DownloadedAssets.spellDictData [sd.id].spellDescription;
		var sigList = new List<string> ();  

//		foreach (var item in PlayerDataManager.playerData.spells) {
//			if (item.baseSpell == sd.id) {
//				sigList.Add (DownloadedAssets.spellDictData [item.id].spellName);
//			}
//		}

		if (sigList.Count == 0) {
			knowSigs.SetActive(false);
			return;
		}

		foreach (var item in sigList) {
			var g = Utilities.InstantiateObject (sig, sigContainer);
			g.GetComponent<Text> ().text = item;
		}
	}
	
}

