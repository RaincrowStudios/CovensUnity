using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnhancedUI.EnhancedScroller;

public class CardSetup : EnhancedScrollerCellView {

	public SetupDeckCard spirit;
	public SetupDeckCard whiteP;
	public SetupDeckCard shadowP;
	public SetupDeckCard greyP;

	public void SetupCard(SpiritInstance SD){
		spirit.gameObject.SetActive (false);
		whiteP.gameObject.SetActive (false);
		shadowP.gameObject.SetActive (false);
		greyP.gameObject.SetActive (false);

		if (SD.deckCardType == SpiritDeckUIManager.type.active || SD.deckCardType == SpiritDeckUIManager.type.known) {
			spirit.gameObject.SetActive (true);
			spirit.SetupCard (SD);
		} else {
			if (SD.degree > 0) {
				whiteP.gameObject.SetActive (true);	
				whiteP.SetupCard (SD);
				 
			} else if (SD.degree < 0) {
				shadowP.gameObject.SetActive (true);	
				shadowP.SetupCard (SD);
			
			} else {
				greyP.gameObject.SetActive (true);	
				greyP.SetupCard (SD);
			
			}
		}
	}
}
