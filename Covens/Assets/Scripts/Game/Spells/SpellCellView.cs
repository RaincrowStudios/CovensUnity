using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;
public class SpellCellView : EnhancedScrollerCellView {

	public Image spellGlyph;

	public void SetData(SpellData sd)
	{
		gameObject.name = sd.id;
		spellGlyph.sprite = SpellGlyphs.glyphs [sd.id];
	}

}
