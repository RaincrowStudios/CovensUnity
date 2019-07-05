using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnhancedUI.EnhancedScroller;

public class DeckScroller : MonoBehaviour, IEnhancedScrollerDelegate {

	public EnhancedScroller scroller;

	public CardSetup card;

	public float size = 475;
	public List<SpiritInstance> data = new List<SpiritInstance>();
	void Start()
	{
		scroller.Delegate = this;
	}

	public void InitScroll()
	{
		scroller.ReloadData ();
	}

	public float GetCellViewSize(EnhancedScroller scroller, int dataIndex){
		return 475f;
	}

	public int GetNumberOfCells(EnhancedScroller scroller)
	{
		return data.Count; 
	}

	public EnhancedScrollerCellView GetCellView (EnhancedScroller scroller, int dataIndex, int cellIndex)
	{
			SpiritInstance sd = data [dataIndex];
			var sCard = scroller.GetCellView (card) as CardSetup;
			sCard.SetupCard (sd);
			return sCard;
	}
}
