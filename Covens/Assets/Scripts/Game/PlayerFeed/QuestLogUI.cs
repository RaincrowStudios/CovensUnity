using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

public class QuestLogUI : UIAnimationManager
{

    public static QuestLogUI Instance { get; set; }

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

    public Text rewardEnergy;
    public Text rewardGold;
    public Text rewardSilver;
    public GameObject buttonTapChest;

    public Text bottomInfo;

    public LogScroller LS;
    public GameObject DescObject;
    public Animator descAnim;
    public Text completeText;
    public Text title;
    public Text subTitle;
    public Text Desc;

    bool isQuest = true;

    public Animator anim;

    private bool isOpen = false;
    private bool questInfoVisible = false;

    //will only be true if reward collected, and haven't closed yet.
    public bool dailiesCompleted = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Open();
        dailiesCompleted = false;
    }

    public void Open()
    {
        UIStateManager.Instance.CallWindowChanged(false);
        if (isOpen)
            return;
        isOpen = true;
        StopAllCoroutines();

        QuestLogContainer.SetActive(true);
        anim.Play("in");
        if (isQuest)
        {
            OnClickQuest();
        }
        else
        {
            OnClickLog();
        }
    }

    public void Close()
    {
        UIStateManager.Instance.CallWindowChanged(true);
        if (isOpen == false)
            return;

        isOpen = false;

        if (questInfoVisible)
        {
            HideInfo();
            Invoke("CloseP2", 0.15f);
        }
        else
        {
            CloseP2();
        }


    }

    private void CloseP2()
    {
        StopCoroutine("NewQuestTimer");
        anim.Play("out");
        //   Disable(QuestLogContainer, 1);

        //so it only executes after opening the chest
        if (dailiesCompleted && ReviewPopupController.IsCorrectConditions())
            StartCoroutine(LoadReviewPopup());

        Destroy(gameObject, 1.5f);
    }

    IEnumerator LoadReviewPopup()
    {
        var request = Resources.LoadAsync<GameObject>("ReviewPopup/ReviewPopup");
        yield return request;
        Instantiate(request.asset);
        dailiesCompleted = false;
    }

    void GetLogs()
    {
        Debug.Log("getting logs");
        APIManager.Instance.GetData("character/event-log",
            (string result, int response) =>
            {
                if (Application.isEditor)
                    Debug.Log(result);

                if (response == 200)
                {
                    LS.log = JsonConvert.DeserializeObject<List<EventLogData>>(result);
                    SetupLogs();
                }
                else
                    Debug.Log(result + response);
            });
    }

    public void OnClickLog()
    {
        if (questInfoVisible)
            HideInfo();

        logObject.SetActive(true);
        questObject.SetActive(false);
        questCG.alpha = .4f;
        logCG.alpha = 1;
        GetLogs();
    }

    public void OnClickQuest()
    {
        logObject.SetActive(false);
        questObject.SetActive(true);
        questCG.alpha = 1;
        logCG.alpha = .4f;
        QuestsController.GetQuests((result, response) =>
        {
            if (result == 200)
                SetupQuest();
            else
                Close();
        });
    }

    public void SetupLogs()
    {
        LS.InitScroll();
    }
    // Use this for initialization
    public void SetupQuest()
    {
        #region SetupGlow
        var questPlayer = PlayerDataManager.currentQuests;
        if (questPlayer.explore.complete)
        {
            exploreGlow.SetActive(true);
        }
        else
        {
            exploreGlow.SetActive(false);
        }

        if (questPlayer.gather.complete)
        {
            gatherGlow.SetActive(true);
        }
        else
        {
            gatherGlow.SetActive(false);
        }

        if (questPlayer.spellcraft.complete)
        {
            spellcraftGlow.SetActive(true);
        }
        else
        {
            spellcraftGlow.SetActive(false);
        }

        if (questPlayer.explore.complete && questPlayer.gather.complete)
        {
            expGathLine.SetActive(true);
        }
        else
        {
            expGathLine.SetActive(false);
        }

        if (questPlayer.spellcraft.complete && questPlayer.gather.complete)
        {
            gathSpellLine.SetActive(true);
        }
        else
        {
            gathSpellLine.SetActive(false);
        }

        if (questPlayer.spellcraft.complete && questPlayer.explore.complete)
        {
            spellExpLine.SetActive(true);
        }
        else
        {
            spellExpLine.SetActive(false);
        }
        #endregion
        DescObject.SetActive(true);
        if (questPlayer.explore.complete && questPlayer.gather.complete && questPlayer.spellcraft.complete)
        {
            if (!questPlayer.collected)
            {
                openChest.SetActive(false);
                closedChest.SetActive(true);
                claimFX.SetActive(true);
				bottomInfo.text = LocalizeLookUp.GetText ("daily_tap_chest");//"Tap the chest to claim rewards";
                buttonTapChest.SetActive(true);
            }
            else
            {
                openChest.SetActive(true);
                closedChest.SetActive(false);
                claimFX.SetActive(false);
                StartCoroutine(NewQuestTimer());
                buttonTapChest.SetActive(false);
            }
        }
        else
        {
            openChest.SetActive(false);
            closedChest.SetActive(true);
            claimFX.SetActive(false);
            StartCoroutine(NewQuestTimer());
            buttonTapChest.SetActive(false);
        }
    }


    public void ClaimRewards()
    {
        APIManager.Instance.GetData("daily/reward",
            (string result, int response) =>
            {
                if (response == 200)
                {
                    var reward = JsonConvert.DeserializeObject<Rewards>(result);
                    StartCoroutine(ShowRewards(reward));
                    dailiesCompleted = true;
                }
                else
                {
                    Debug.Log(result + response);
					bottomInfo.text = LocalizeLookUp.GetText("daily_could_not_claim");//"Couldn't Claim rewards . . .";
                }
            });
    }

    IEnumerator ShowRewards(Rewards reward)
    {
        SoundManagerOneShot.Instance.PlayReward();
        if (reward.silver != 0)
        {
            rewardSilver.gameObject.SetActive(true);
			rewardSilver.text = "+" + reward.silver.ToString () + " " + LocalizeLookUp.GetText ("store_silver");//" Silver!";

        }

        yield return new WaitForSeconds(1.8f);
        if (reward.gold != 0)
        {
            rewardGold.gameObject.SetActive(true);
			rewardGold.text = "+" + reward.gold.ToString () + " " + LocalizeLookUp.GetText ("store_gold");//Gold!";
        }

        yield return new WaitForSeconds(1.8f);
        if (reward.energy != 0)
        {
            rewardEnergy.gameObject.SetActive(true);
			rewardEnergy.text = "+" + reward.energy.ToString () + " " + LocalizeLookUp.GetText ("card_witch_energy");//Energy!";
        }

        PlayerDataManager.playerData.silver += reward.silver;
        PlayerDataManager.playerData.gold += reward.gold;

        PlayerManagerUI.Instance.UpdateDrachs();

        openChest.SetActive(true);
        closedChest.SetActive(false);
        claimFX.SetActive(false);
        StartCoroutine(NewQuestTimer());
    }

    IEnumerator NewQuestTimer()
    {
        //		Debug.Log("Starting");

        while (true)
        {
			bottomInfo.text = LocalizeLookUp.GetText("daily_new_quest") /*"New Quest :*/ + " " + "<color=white>" + Utilities.GetTimeRemaining(PlayerDataManager.currentQuests.expiresOn) + "</color>";
            yield return new WaitForSeconds(1);
        }
    }

    public void ClickExplore()
    {
        questInfoVisible = true;

        subTitle.gameObject.SetActive(true);
        subTitle.text = DownloadedAssets.questsDict[PlayerDataManager.currentQuests.explore.id].title;
        Desc.text = DownloadedAssets.questsDict[PlayerDataManager.currentQuests.explore.id].value;
		title.text = LocalizeLookUp.GetText ("daily_explore");//"Explore";
        if (PlayerDataManager.currentQuests.explore.complete)
        {
            completeText.text = "( 1/1 )";
        }
        else
        {
            completeText.text = "( 0/1 )";
        }
        descAnim.Play("up");
        Desc.fontSize = 68;


    }

    public void ClickGather()
    {
        questInfoVisible = true;
        subTitle.gameObject.SetActive(false);
		Desc.text = LocalizeLookUp.GetText("collect") + " " + /*"Collect "*/ + PlayerDataManager.currentQuests.gather.amount + " " + (PlayerDataManager.currentQuests.gather.type == LocalizeLookUp.GetText("daily_herb") /*"herb"*/ ? LocalizeLookUp.GetText("ingredient_botanicals")/*"botanicals"*/ : PlayerDataManager.currentQuests.gather.type);
        //if (PlayerDataManager.currentQuests.gather.amount > 1)
        //{
            //doing this for more than one tool, herb or gem
       //     Desc.text += "s";
       // }
        if (PlayerDataManager.currentQuests.gather.location != "")
        {
			Desc.text += " " + LocalizeLookUp.GetText("daily_in_location").Replace("{{location}}", DownloadedAssets.countryCodesDict[PlayerDataManager.currentQuests.gather.location].value);
        }
		title.text = LocalizeLookUp.GetText ("daily_gather");//"Gather";
        completeText.text = "( " + PlayerDataManager.playerData.dailies.gather.count.ToString() + "/" + PlayerDataManager.currentQuests.gather.amount.ToString() + " )";
        descAnim.Play("up");
        Desc.fontSize = 75;

    }

    public void ClickSpellCraft()
    {
        questInfoVisible = true;

        subTitle.gameObject.SetActive(false);
        Desc.fontSize = 75;


		Desc.text = LocalizeLookUp.GetText ("daily_casting_upon");//.Replace("{{Spell Name}}", DownloadedAssets.spellDictData [PlayerDataManager.currentQuests.spellcraft.id].spellName).Replace("{{amount}}", PlayerDataManager.currentQuests.spellcraft.amount);
		if (PlayerDataManager.currentQuests.spellcraft.type != "")
		{
			if (PlayerDataManager.currentQuests.spellcraft.relation != "") {
				if (PlayerDataManager.currentQuests.spellcraft.relation == "ally") {
					Desc.text = LocalizeLookUp.GetText ("daily_casting_upon_ally") + " " + PlayerDataManager.currentQuests.spellcraft.type;
				} else if (PlayerDataManager.currentQuests.spellcraft.relation == "enemy") {
					Desc.text = LocalizeLookUp.GetText ("daily_casting_upon_enemy") + " " + PlayerDataManager.currentQuests.spellcraft.type;																
				} else if (PlayerDataManager.currentQuests.spellcraft.relation == "coven") {
					Desc.text = LocalizeLookUp.GetText ("daily_casting_upon_coven").Replace ("{{type}}", " " + PlayerDataManager.currentQuests.spellcraft.type);
				} else if (PlayerDataManager.currentQuests.spellcraft.relation == "own") {
					Desc.text = LocalizeLookUp.GetText ("daily_casting_upon_own") + " " + PlayerDataManager.currentQuests.spellcraft.type;
				} else if (PlayerDataManager.currentQuests.spellcraft.relation == "higher") {
					Desc.text = LocalizeLookUp.GetText ("daily_casting_upon_higher") + " " + PlayerDataManager.currentQuests.spellcraft.type;
				} else if (PlayerDataManager.currentQuests.spellcraft.relation == "lower") {
					Desc.text = LocalizeLookUp.GetText ("daily_casting_upon_lower") + " " + PlayerDataManager.currentQuests.spellcraft.type;
				}
			} else {
				Desc.text = LocalizeLookUp.GetText ("daily_casting_on_a").Replace("{{type}}", " " + PlayerDataManager.currentQuests.spellcraft.type);
			}
		}
		if (PlayerDataManager.currentQuests.spellcraft.location != "") {
			Desc.text += " " + LocalizeLookUp.GetText("daily_casting_location").Replace("{{Location}}", " " + DownloadedAssets.countryCodesDict[PlayerDataManager.currentQuests.spellcraft.location].value);
		} 
		if (PlayerDataManager.currentQuests.spellcraft.ingredient != "") {
			Desc.text += " " + LocalizeLookUp.GetText ("daily_casting_using").Replace ("{{ingredient}}", " " + PlayerDataManager.currentQuests.spellcraft.ingredient);
		}
		Desc.text = Desc.text.Replace("{{Spell Name}}", DownloadedAssets.spellDictData [PlayerDataManager.currentQuests.spellcraft.id].spellName).Replace("{{amount}}", PlayerDataManager.currentQuests.spellcraft.amount.ToString());


//		Desc.text = LocalizeLookUp.GetText ("card_witch_cast") + " " + /*"Cast "*/ +DownloadedAssets.spellDictData [PlayerDataManager.currentQuests.spellcraft.id].spellName + " " + PlayerDataManager.currentQuests.spellcraft.amount + " " + LocalizeLookUp.GetText ("generic_times");//times";
 //       if (PlayerDataManager.currentQuests.spellcraft.type != "")
//        {
 //           if (PlayerDataManager.currentQuests.spellcraft.relation != "")
//            {
 //               if (PlayerDataManager.currentQuests.spellcraft.relation == "ally")
//                {
//					Desc.text += " " + LocalizeLookUp.GetText("daily_on_an_ally") + " " /*on an ally "*/ + PlayerDataManager.currentQuests.spellcraft.type;
 //               }
 //               else if (PlayerDataManager.currentQuests.spellcraft.relation == "enemy")
//                {
//					Desc.text += " " + LocalizeLookUp.GetText("daily_on_an_enemy") + " " /*on an enemy "*/ + PlayerDataManager.currentQuests.spellcraft.type;
 //               }
//                else if (PlayerDataManager.currentQuests.spellcraft.relation == "coven")
//                {
//					Desc.text += " " + LocalizeLookUp.GetText("daily_on_an_coven").Replace("{{type}}", PlayerDataManager.currentQuests.spellcraft.type) + " "; //" on an " + PlayerDataManager.currentQuests.spellcraft.type + " of your coven ";
//                }
//                else if (PlayerDataManager.currentQuests.spellcraft.relation == "own")
//                {
//					Desc.text += " " + LocalizeLookUp.GetText("daily_on_your_own") + " " + /*on your own "*/ + PlayerDataManager.currentQuests.spellcraft.type;
  //              }
  //              else if (PlayerDataManager.currentQuests.spellcraft.relation == "higher")
  //              {
//					Desc.text += " " + LocalizeLookUp.GetText("daily_on_higher")/*(on a higher level*/ + " " + PlayerDataManager.currentQuests.spellcraft.type;
 //               }
  //              else if (PlayerDataManager.currentQuests.spellcraft.relation == "lower")
   //             {
//					Desc.text += " " + LocalizeLookUp.GetText("daily_on_lower") /*on a lower level*/ + " " + PlayerDataManager.currentQuests.spellcraft.type;
  //              }
    //        }
    //        else
    //        {
//				Desc.text += " " + LocalizeLookUp.GetText("generic_on_a") /*on a*/ + " " + PlayerDataManager.currentQuests.spellcraft.type;
//            }
//        }
//        if (PlayerDataManager.currentQuests.spellcraft.location != "")
//        {//
//			Desc.text += " " + LocalizeLookUp.GetText("generic_in") + " " + DownloadedAssets.countryCodesDict[PlayerDataManager.currentQuests.spellcraft.location].value;
 //       }
  //      if (PlayerDataManager.currentQuests.spellcraft.ingredient != "")
   //     {
//			Desc.text += " " + LocalizeLookUp.GetText("generic_using_a") + " " + PlayerDataManager.currentQuests.spellcraft.ingredient;
 //       }
        Desc.text += ".";
		title.text = LocalizeLookUp.GetText ("daily_spell");//"Spellcraft";
        completeText.text = "( " + PlayerDataManager.playerData.dailies.spellcraft.count.ToString() + "/" + PlayerDataManager.currentQuests.spellcraft.amount.ToString() + " )";
        descAnim.Play("up");
    }

    public void HideInfo()
    {
        questInfoVisible = false;
        descAnim.Play("down");
    }
}

public class EventLogData
{

    public string type { get; set; }
    public string spirit { get; set; }
    public string spiritId { get; set; }
    public string spellId { get; set; }
    public int casterDegree { get; set; }
    public int energyChange { get; set; }
    public string casterName { get; set; }
    public double timestamp { get; set; }

    public string witchCreated { get; set; }
}




