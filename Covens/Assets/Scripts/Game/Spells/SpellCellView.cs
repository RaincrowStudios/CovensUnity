using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;
public class SpellCellView : EnhancedScrollerCellView {

	public Image spellGlyph;
	public GameObject container;

	public void SetData (SpellData sd, bool isNull = false)
	{
		if (!isNull) {
			GetComponent<BoxCollider> ().enabled = true;
			gameObject.name = sd.id;
			spellGlyph.sprite = SpellGlyphs.glyphs [sd.id];
			spellGlyph.color =Color.white;
			container.SetActive (true);
		} else {
//			gameObject.name = "null";
			GetComponent<BoxCollider> ().enabled = false;
			spellGlyph.color = new Color (0, 0, 0, 0);
			container.SetActive (false);
		}
	}

}
