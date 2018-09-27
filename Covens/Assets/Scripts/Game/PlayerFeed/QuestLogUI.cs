using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

public class QuestLogUI : UIAnimationManager {

	public static QuestLogUI Instance { get; set;}

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

	public LogScroller LS;
	public GameObject DescObject;
	public Animator descAnim;
	public Text completeText;
	public Text title;
	public Text subTitle;
	public Text Desc;
	public GameObject Notification;
	public Text notiTitle;
	public Text notiProgress;
	bool isQuest = true;

	public Animator anim;

	public static Quests currentQuests;

	void Awake()
	{
		Instance = this;
	}

	public void OnProgress(string quest, int count, int silver){
		StartCoroutine (OnProgressHelper (quest, count, silver));
	}

	 IEnumerator OnProgressHelper(string quest, int count, int silver)
	{
		var pQuest = PlayerDataManager.playerData.quests;
		Notification.SetActive (true);
		yield return new WaitForSeconds (3);
		if (silver == 0) {
			if (quest == "gather") {
				notiTitle.text = "Quest Progress : Gather"; 
				notiProgress.text = "Completed : " + count.ToString () + "/" + currentQuests.gather.amount.ToString ();
				pQuest.gather.count = count;
			} else if (quest == "spellcraft") {
				notiTitle.text = "Quest Progress : Spellcraft"; 
				notiProgress.text = "Completed : " + count.ToString () + "/" + currentQuests.spellcraft.amount.ToString ();
				pQuest.spellcraft.count = count;
			} 
		} else {
			if (quest == "gather") {
				notiTitle.text = "Gather Quest Completed!"; 
				notiProgress.text = "+ " + silver.ToString () + " Silver";
				pQuest.gather.count = count;
				pQuest.explore.complete = true;

			} else if (quest == "spellcraft") {
				notiTitle.text = "Spellcraft Quest Completed!"; 
				notiProgress.text = "+ " + silver.ToString () + " Silver";
				pQuest.spellcraft.count = count;
				pQuest.spellcraft.complete = true;
			} else {
				notiTitle.text = "Explore Quest Completed!"; 
				notiProgress.text = "+ " + silver.ToString () + " Silver";
				pQuest.explore.count = 1;
				pQuest.explore.complete = true;
			}
		}
	}

	public void Open()
	{
		QuestLogContainer.SetActive (true);
		anim.Play ("in");
		if (isQuest) {
			OnClickQuest ();
		} else {
			OnClickLog ();
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

	void GetLogs()
	{
		APIManager.Instance.GetData ("character/event-log",
			(string result, int response) => {
				if(response == 200){
					LS.log = JsonConvert.DeserializeObject<List<EventLogData>>(result);	
					SetupLogs();
				}
				else
					print(result + response);
			});
	}

	public void OnClickLog()
	{
		logObject.SetActive (true);
		questObject.SetActive (false);
		questCG.alpha = .4f;
		logCG.alpha = 1;
		GetLogs ();
	}

	public void OnClickQuest()
	{
		logObject.SetActive (false);
		questObject.SetActive (true);
		questCG.alpha = 1;
		logCG.alpha = .4f;
		GetQuests ();
	}

	public void SetupLogs()
	{
		LS.InitScroll ();
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
		Desc.fontSize = 55;

			
	}

	public void ClickGather()
	{
		subTitle.gameObject.SetActive (false);
		Desc.text = "Collect " + currentQuests.gather.amount + " " + currentQuests.gather.type;
		if (currentQuests.gather.location != "") {
			Desc.text += " in " + DownloadedAssets.countryCodesDict [currentQuests.gather.location].value+ ".";
		}
		title.text = "Gather";
		completeText.text = "( " + PlayerDataManager.playerData.quests.gather.count.ToString() + "/" + currentQuests.gather.amount.ToString() + " )";
		descAnim.Play ("up");
		Desc.fontSize = 65;

	}

	public void ClickSpellCraft()
	{
		subTitle.gameObject.SetActive (false);
		Desc.fontSize = 65;
		Desc.text = "Cast " + DownloadedAssets.spellDictData[currentQuests.spellcraft.id].spellName + " " + currentQuests.spellcraft.amount + " times" ;
		if (currentQuests.spellcraft.type != "") {
			if (currentQuests.spellcraft.relation != "") {
				if (currentQuests.spellcraft.relation == "ally") {
					Desc.text += " on an ally " + currentQuests.spellcraft.type; 
				} else if (currentQuests.spellcraft.relation == "enemy") {
					Desc.text += " on an enemy " + currentQuests.spellcraft.type; 
				} else if (currentQuests.spellcraft.relation == "coven") {
					Desc.text += " on an " + currentQuests.spellcraft.type + " of your coven "; 
				} else if (currentQuests.spellcraft.relation == "own") {
					Desc.text += " on your own " + currentQuests.spellcraft.type; 
				} else if (currentQuests.spellcraft.relation == "higher") {
					Desc.text += " on a higher level " + currentQuests.spellcraft.type; 
				} else if (currentQuests.spellcraft.relation == "lower") {
					Desc.text += " on a lower level " + currentQuests.spellcraft.type; 
				}
			} else {
				Desc.text += " on a " + currentQuests.spellcraft.type; 
			}
		}
		if (currentQuests.spellcraft.location != "") {
			Desc.text += " in " + currentQuests.spellcraft.location;
		}
		if (currentQuests.spellcraft.ingredient != "") {
			Desc.text += " using a " + currentQuests.spellcraft.ingredient;
		}
		Desc.text += ".";
		title.text = "Spellcraft";
		completeText.text = "( " + PlayerDataManager.playerData.quests.spellcraft.count.ToString() + "/" + currentQuests.spellcraft.amount.ToString() + " )";
		descAnim.Play ("up");

	}

	public void HideInfo()
	{
		descAnim.Play ("down");
	}
}

public class EventLogData{

	public string type{ get; set;}
	public int energyChange { get; set;}
	public double timestamp{ get; set;}
}




