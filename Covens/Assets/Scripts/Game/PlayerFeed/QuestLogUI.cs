using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

public class QuestLogUI : UIAnimationManager {
	public GameObject QuestLogContainer;
	public GameObject logObject;
	public GameObject questObject;

	public CanvasGroup questCG;
	public CanvasGroup logCG;

	public GameObject gatherGlow;
	public GameObject exploreGlow;
	public GameObject spellcraftGlow;

	public GameObject expGathLine;
	public GameObject gathSpellLine;
	public GameObject spellExpLine;

	public GameObject claimFX;
	public GameObject openChest;
	public GameObject closedChest;

	public Text bottomInfo;

	bool isQuest = true;

	public Animator anim;

	public void Open()
	{
		QuestLogContainer.SetActive (true);
		anim.Play ("in");
		if (isQuest) {
			logObject.SetActive (false);
			questObject.SetActive (true);
			questCG.alpha = 1;
			logCG.alpha = .4f;
			SetupQuest ();
		} else {
			logObject.SetActive (true);
			questObject.SetActive (false);
			questCG.alpha = .4f;
			logCG.alpha = 1;
		}
	}

	public void Close()
	{
		anim.Play ("out");
		Disable (QuestLogContainer, 1);
	}

	void getQuests()
	{
		APIManager.Instance.GetData ("quest/get",
			(string result, int response) => {
				if(response == 200)
				print(result);	
				else
				print(result + response);
			});
	}
	// Use this for initialization
	public void SetupQuest()
	{
		getQuests ();
	}
}
