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
	void Start()
	{
		scroller.Delegate = this;
		scroller.ReloadData ();
		scroller.Snap ();
	}



	// Update is called once per frame
	void Update () {
		
	}

	#region IEnhancedScrollerDelegate implementation

	public int GetNumberOfCells (EnhancedScroller scroller)
	{
		return SpellCastAPI.spells.Count;
	}

	public float GetCellViewSize (EnhancedScroller scroller, int dataIndex)
	{
		return size;
	}

	public EnhancedScrollerCellView GetCellView (EnhancedScroller scroller, int dataIndex, int cellIndex)
	{
		
		SpellCellView cellView = scroller.GetCellView (spellButton) as SpellCellView;
		cellView.SetData (SpellCastAPI.spells.Values.ToList() [dataIndex]);
		return cellView;
	}

	#endregion
}


