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


	public GameObject DescObject;
	public Animator descAnim;
	public Text completeText;
	public Text title;
	public Text subTitle;
	public Text Desc;

	bool isQuest = true;

	public Animator anim;

	Quests currentQuests;

	public void Open()
	{
		QuestLogContainer.SetActive (true);
		anim.Play ("in");
		if (isQuest) {
			logObject.SetActive (false);
			questObject.SetActive (true);
			questCG.alpha = 1;
			logCG.alpha = .4f;
			GetQuests ();
		} else {
			logObject.SetActive (true);
			questObject.SetActive (false);
			questCG.alpha = .4f;
			logCG.alpha = 1;
		}
	}

	public void Close()
	{
		StopCoroutine ("NewQuestTimer");
		anim.Play ("out");
		Disable (QuestLogContainer, 1);
		DescObject.SetActive (false);
	}

	void GetQuests()
	{
		APIManager.Instance.GetData ("quest/get",
			(string result, int response) => {
				if(response == 200){
					currentQuests = JsonConvert.DeserializeObject<Quests>(result);	
					SetupQuest();
				}
				else
				print(result + response);
			});
	}
	// Use this for initialization
	public void SetupQuest()
	{
		#region SetupGlow
		var questPlayer = PlayerDataManager.playerData.quests;
		if (currentQuests.explore.complete) {
			exploreGlow.SetActive (true);
		} else {
			exploreGlow.SetActive (false);
		} 

		if (questPlayer.gather.complete) {
			gatherGlow.SetActive (true);
		} else {
			gatherGlow.SetActive (false);
		}

		if (questPlayer.spellcraft.complete) {
			spellcraftGlow.SetActive (true);
		} else {
			spellcraftGlow.SetActive (false);
		}

		if (currentQuests.explore.complete && questPlayer.gather.complete) {
			expGathLine.SetActive (true);
		} else {
			expGathLine.SetActive (false);
		}

		if (currentQuests.spellcraft.complete && questPlayer.gather.complete) {
			gathSpellLine.SetActive (true);
		} else {
			gathSpellLine.SetActive (false);
		}

		if (currentQuests.spellcraft.complete && questPlayer.explore.complete) {
			spellExpLine.SetActive (true);
		} else {
			spellExpLine.SetActive (false);
		}
		#endregion
		DescObject.SetActive (true);
		if (currentQuests.explore.complete && currentQuests.gather.complete && currentQuests.spellcraft.complete) {
			if (!currentQuests.collected) {
				openChest.SetActive (false);
				closedChest.SetActive (true);
				claimFX.SetActive (true);
				bottomInfo.text = "Tap the chest to claim rewards";
			} else {
				openChest.SetActive (true);
				closedChest.SetActive (false);
				claimFX.SetActive (false);
				StartCoroutine (NewQuestTimer ());
			}
		} else {
			openChest.SetActive (false);
			closedChest.SetActive (true);
			claimFX.SetActive (false);
			StartCoroutine (NewQuestTimer ());
		}
	}

	IEnumerator NewQuestTimer()
	{
		print("Starting");

		while (true) {
			bottomInfo.text = "New Quest : <color=white>" + Utilities.GetTimeRemaining(currentQuests.expiresOn)+"</color>";
			yield return new WaitForSeconds (1);
		}
	}

	public void ClickExplore()
	{
		subTitle.gameObject.SetActive (true);
		subTitle.text = DownloadedAssets.questsDict [currentQuests.explore.id].title;
		Desc.text = DownloadedAssets.questsDict [currentQuests.explore.id].value;
		title.text = "Explore";
		if (currentQuests.explore.complete) {
			completeText.text = "( 1/1 )";
		} else {
			completeText.text = "( 0/1 )";
		}
		descAnim.Play ("up");

			
	}

	public void ClickGather()
	{
		subTitle.gameObject.SetActive (false);
		Desc.text = "Collect " + currentQuests.gather.amount + " " + currentQuests.gather.type;
		if (currentQuests.gather.location != "") {
			Desc.text += " in " + DownloadedAssets.countryCodesDict [currentQuests.gather.location]+ ".";
		}
		title.text = "Gather";
		completeText.text = "( " + PlayerDataManager.playerData.quests.gather.count.ToString() + "/" + currentQuests.gather.complete.ToString() + " )";
		descAnim.Play ("up");

	}

	public void ClickSpellCraft()
	{
		subTitle.gameObject.SetActive (false);
		Desc.text = "Cast " + DownloadedAssets.spellDictData[currentQuests.spellcraft.id].spellName + " " + currentQuests.spellcraft.amount + " times" ;
		if (currentQuests.spellcraft.target != "") {
			Desc.text += " on a " + currentQuests.spellcraft.type;
		}
		if (currentQuests.gather.location != "") {
			Desc.text += " in " + DownloadedAssets.countryCodesDict [currentQuests.spellcraft.location];
		}
		if (currentQuests.spellcraft.type != "") {
			Desc.text += " using " + currentQuests.spellcraft.type;
		}
		Desc.text += ".";
		title.text = "Spellcraft";
		completeText.text = "( " + PlayerDataManager.playerData.quests.spellcraft.count.ToString() + "/" + currentQuests.spellcraft.complete.ToString() + " )";
		descAnim.Play ("up");

	}

	public void HideInfo()
	{
		descAnim.Play ("down");
	}
}





