using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnhancedUI.EnhancedScroller;
using System.Linq;
public class SummonHerbScroller : MonoBehaviour,IEnhancedScrollerDelegate
{
	public EnhancedScroller scroller;

	public SummonHerbData card;

	public float size = 475;


	List<string> data = new List<string>();


	void Start()
	{
		scroller.Delegate = this;
	}

	public void InitScroll( )
	{
		data = PlayerDataManager.playerData.ingredients.herbsDict.Keys.ToList ();
		scroller.ReloadData ();
	}

	public float GetCellViewSize(EnhancedScroller scroller, int dataIndex){
		return size;
	}

	public int GetNumberOfCells(EnhancedScroller scroller)
	{
		return data.Count; 
	}

	public EnhancedScrollerCellView GetCellView (EnhancedScroller scroller, int dataIndex, int cellIndex)
	{
		var sCard = scroller.GetCellView (card) as SummonHerbData;
		sCard.Setup (data[dataIndex]);
		return sCard;
	}
}

