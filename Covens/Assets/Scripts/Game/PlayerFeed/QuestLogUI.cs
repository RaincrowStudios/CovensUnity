﻿using System.Collections;
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

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Open();
    }

    public void Open()
    {
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
        Destroy(gameObject, 1.5f);
        //DescObject.SetActive(false);
    }

    void GetLogs()
    {
        print("getting logs");
        APIManager.Instance.GetData("character/event-log",
            (string result, int response) =>
            {
                if (Application.isEditor)
                    print(result);

                if (response == 200)
                {
                    LS.log = JsonConvert.DeserializeObject<List<EventLogData>>(result);
                    SetupLogs();
                }
                else
                    print(result + response);
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
                bottomInfo.text = "Tap the chest to claim rewards";
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
                }
                else
                {
                    print(result + response);
                    bottomInfo.text = "Couldn't Claim rewards . . .";
                }
            });
    }

    IEnumerator ShowRewards(Rewards reward)
    {
        SoundManagerOneShot.Instance.PlayReward();
        if (reward.silver != 0)
        {
            rewardSilver.gameObject.SetActive(true);
            rewardSilver.text = "+" + reward.silver.ToString() + " Silver!";
        }

        yield return new WaitForSeconds(1.8f);
        if (reward.gold != 0)
        {
            rewardGold.gameObject.SetActive(true);
            rewardGold.text = "+" + reward.gold.ToString() + " Gold!";
        }

        yield return new WaitForSeconds(1.8f);
        if (reward.energy != 0)
        {
            rewardEnergy.gameObject.SetActive(true);
            rewardEnergy.text = "+" + reward.energy.ToString() + " Energy!";
        }

        openChest.SetActive(true);
        closedChest.SetActive(false);
        claimFX.SetActive(false);
        StartCoroutine(NewQuestTimer());
    }

    IEnumerator NewQuestTimer()
    {
        //		print("Starting");

        while (true)
        {
            bottomInfo.text = "New Quest : <color=white>" + Utilities.GetTimeRemaining(PlayerDataManager.currentQuests.expiresOn) + "</color>";
            yield return new WaitForSeconds(1);
        }
    }

    public void ClickExplore()
    {
        questInfoVisible = true;

        subTitle.gameObject.SetActive(true);
        subTitle.text = DownloadedAssets.questsDict[PlayerDataManager.currentQuests.explore.id].title;
        Desc.text = DownloadedAssets.questsDict[PlayerDataManager.currentQuests.explore.id].value;
        title.text = "Explore";
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
        Desc.text = "Collect " + PlayerDataManager.currentQuests.gather.amount + " " + (PlayerDataManager.currentQuests.gather.type == "herb" ? "botanicals" : PlayerDataManager.currentQuests.gather.type);
        if (PlayerDataManager.currentQuests.gather.location != "")
        {
            Desc.text += " in " + DownloadedAssets.countryCodesDict[PlayerDataManager.currentQuests.gather.location].value + ".";
        }
        title.text = "Gather";
        completeText.text = "( " + PlayerDataManager.playerData.dailies.gather.count.ToString() + "/" + PlayerDataManager.currentQuests.gather.amount.ToString() + " )";
        descAnim.Play("up");
        Desc.fontSize = 75;

    }

    public void ClickSpellCraft()
    {
        questInfoVisible = true;

        subTitle.gameObject.SetActive(false);
        Desc.fontSize = 75;
        Desc.text = "Cast " + DownloadedAssets.spellDictData[PlayerDataManager.currentQuests.spellcraft.id].spellName + " " + PlayerDataManager.currentQuests.spellcraft.amount + " times";
        if (PlayerDataManager.currentQuests.spellcraft.type != "")
        {
            if (PlayerDataManager.currentQuests.spellcraft.relation != "")
            {
                if (PlayerDataManager.currentQuests.spellcraft.relation == "ally")
                {
                    Desc.text += " on an ally " + PlayerDataManager.currentQuests.spellcraft.type;
                }
                else if (PlayerDataManager.currentQuests.spellcraft.relation == "enemy")
                {
                    Desc.text += " on an enemy " + PlayerDataManager.currentQuests.spellcraft.type;
                }
                else if (PlayerDataManager.currentQuests.spellcraft.relation == "coven")
                {
                    Desc.text += " on an " + PlayerDataManager.currentQuests.spellcraft.type + " of your coven ";
                }
                else if (PlayerDataManager.currentQuests.spellcraft.relation == "own")
                {
                    Desc.text += " on your own " + PlayerDataManager.currentQuests.spellcraft.type;
                }
                else if (PlayerDataManager.currentQuests.spellcraft.relation == "higher")
                {
                    Desc.text += " on a higher level " + PlayerDataManager.currentQuests.spellcraft.type;
                }
                else if (PlayerDataManager.currentQuests.spellcraft.relation == "lower")
                {
                    Desc.text += " on a lower level " + PlayerDataManager.currentQuests.spellcraft.type;
                }
            }
            else
            {
                Desc.text += " on a " + PlayerDataManager.currentQuests.spellcraft.type;
            }
        }
        if (PlayerDataManager.currentQuests.spellcraft.location != "")
        {
            Desc.text += " in " + PlayerDataManager.currentQuests.spellcraft.location;
        }
        if (PlayerDataManager.currentQuests.spellcraft.ingredient != "")
        {
            Desc.text += " using a " + PlayerDataManager.currentQuests.spellcraft.ingredient;
        }
        Desc.text += ".";
        title.text = "Spellcraft";
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
}




