using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;



public class SummonHerbData : EnhancedScrollerCellView
{
	public Text text;

	public void Setup(string st){
        text.text = LocalizeLookUp.GetCollectableName(st);
		gameObject.name = st;
	}
}

