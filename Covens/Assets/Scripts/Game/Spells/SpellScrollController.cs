using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnhancedUI.EnhancedScroller;
using System.Linq;

public class SpellScrollController : MonoBehaviour, IEnhancedScrollerDelegate {

	// Use this for initialization
	public SpellCellView spellButton;
	public EnhancedScroller scroller;

	public float size = 200;


	 IEnumerator Start()
	{
		scroller.Delegate = this;
			yield return new WaitForSeconds (1f);
		print ("___loading data with item total of " + SpellCastAPI.validSpells.Count);
	
			scroller.ReloadData ();
			scroller.Snap ();
	}



	#region IEnhancedScrollerDelegate implementation

	public int GetNumberOfCells (EnhancedScroller scroller)
	{
		return SpellCastAPI.validSpells.Count;
	}

	public float GetCellViewSize (EnhancedScroller scroller, int dataIndex)
	{
		return size;
	}

	public EnhancedScrollerCellView GetCellView (EnhancedScroller scroller, int dataIndex, int cellIndex)
	{

		SpellCellView cellView = scroller.GetCellView (spellButton) as SpellCellView;

		if (SpellCastAPI.validSpells [dataIndex] != "null") {
			cellView.SetData (SpellCastAPI.spells [SpellCastAPI.validSpells [dataIndex]]);
		} else {
			cellView.SetData (null, true);
		}
			return cellView;
		
	}

	#endregion
}


