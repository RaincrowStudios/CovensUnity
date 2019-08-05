using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using Raincrow;
using Raincrow.GameEventResponses;

public class QuestLogUI : UIAnimationManager
{
    [SerializeField] private Button m_CloseButton;

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
    
    private static QuestLogUI m_Instance;
    private int m_TweenId;

    public static void Open()
    {
        if (m_Instance != null)
        {
            m_Instance.Show();
        }
        else
        {
            //load the coven scene
            LoadingOverlay.Show();
            SceneManager.LoadSceneAsync(
                SceneManager.Scene.DAILY_QUESTS,
                UnityEngine.SceneManagement.LoadSceneMode.Additive,
                (progress) =>
                {
                },
                () =>
                {
                    LoadingOverlay.Hide();
                    m_Instance.Show();
                });
        }
    }

    void Awake()
    {
        m_Instance = this;
        gameObject.SetActive(false);
        m_CloseButton.onClick.AddListener(Hide);
    }

    [ContextMenu("Show")]
    public void Show()
    {
        if (isOpen)
            return;

        LeanTween.cancel(m_TweenId);
        isOpen = true;

        StopAllCoroutines();
        gameObject.SetActive(true);        
        QuestLogContainer.SetActive(true);

        anim.Play("in");

        if (isQuest)
            OnClickQuest();
        else
            OnClickLog();

        DailyProgressHandler.OnDailyProgress += DailyProgressHandler_OnDailyProgress;
    }

    [ContextMenu("Hide")]
    public void Hide()
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

        DailyProgressHandler.OnDailyProgress -= DailyProgressHandler_OnDailyProgress;
    }

    private void DailyProgressHandler_OnDailyProgress(DailyProgressHandler.DailyProgressEventData data)
    {

    }

    private void CloseP2()
    {
        StopCoroutine("NewQuestTimer");
        anim.Play("out");

        //if (dailiesCompleted && ReviewPopupController.IsCorrectConditions())
        //    StartCoroutine(LoadReviewPopup());

        LeanTween.cancel(m_TweenId);
        m_TweenId = LeanTween.value(0, 0, 1.5f)
            .setOnComplete(() => gameObject.SetActive(false))
            .uniqueId;
    }

    //IEnumerator LoadReviewPopup()
    //{
    //    var request = Resources.LoadAsync<GameObject>("ReviewPopup/ReviewPopup");
    //    yield return request;
    //    Instantiate(request.asset);
    //    dailiesCompleted = false;
    //}

    void GetLogs()
    {
        Debug.LogError("TODO: GET EVENT LOGS");
        //APIManager.Instance.Get("character/event-log",
        //    (string result, int response) =>
        //    {
        //        if (Application.isEditor)
        //            Debug.Log(result);

        //        if (response == 200)
        //        {
        //            LS.log = JsonConvert.DeserializeObject<List<EventLogData>>(result);
        //            SetupLogs();
        //        }
        //        else
        //            Debug.Log(result + response);
        //    });
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
        QuestsController.GetQuests((error) =>
        {
            if (string.IsNullOrEmpty(error))
            {
                SetupQuest();
            }
            else
            {
                UIGlobalPopup.ShowError(Hide, error);
            }
        });
    }

    public void SetupLogs()
    {
        LS.InitScroll();
    }
    // Use this for initialization
    public void SetupQuest()
    {
        bool claimedRewards = PlayerDataManager.playerData.quest.completed;
        bool spellcraft = PlayerDataManager.playerData.quest.spell.completed;
        bool gather = PlayerDataManager.playerData.quest.gather.completed;
        bool explore = PlayerDataManager.playerData.quest.explore.completed;

        exploreGlow.SetActive(explore);
        gatherGlow.SetActive(gather);
        spellcraftGlow.SetActive(spellcraft);

        expGathLine.SetActive(explore && gather);
        gathSpellLine.SetActive(gather && spellcraft);
        spellExpLine.SetActive(spellcraft && explore);

        DescObject.SetActive(true);

        if (spellcraft && explore && gather)
        {
            if (claimedRewards == false)
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
        
        openChest.SetActive(true);
        closedChest.SetActive(false);
        claimFX.SetActive(false);
        StartCoroutine(NewQuestTimer());
    }

    IEnumerator NewQuestTimer()
    {
        while (true)
        {
			bottomInfo.text = LocalizeLookUp.GetText("daily_new_quest") + " " + "<color=white>" + Utilities.GetTimeRemaining(QuestsController.Quests.endDate) + "</color>";
            yield return new WaitForSeconds(1);
        }
    }

    public void ClickExplore()
    {
        questInfoVisible = true;

        subTitle.gameObject.SetActive(true);
        subTitle.text = LocalizeLookUp.GetExploreTitle(QuestsController.Quests.explore.type);
        Desc.text = LocalizeLookUp.GetExploreLore(QuestsController.Quests.explore.type);
		title.text = LocalizeLookUp.GetText ("daily_explore");//"Explore";

        if (PlayerDataManager.playerData.quest.explore.completed)
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

        var gather = QuestsController.Quests.gather;
        var progress = PlayerDataManager.playerData.quest.gather;
        
        Desc.text = 
            LocalizeLookUp.GetText("collect") + " " +
            gather.amount + " " + 
            (gather.type == LocalizeLookUp.GetText("daily_herb") ? LocalizeLookUp.GetText("ingredient_botanicals") : gather.type);
       
        if (string.IsNullOrEmpty(gather.country) == false)
			Desc.text += " " + LocalizeLookUp.GetText("daily_in_location").Replace("{{location}}", LocalizeLookUp.GetCountryName(gather.country));

		title.text = LocalizeLookUp.GetText ("daily_gather");//"Gather";
        completeText.text = "( " + progress.count + "/" + gather.amount.ToString() + " )";

        descAnim.Play("up");
        Desc.fontSize = 75;
    }

    public void ClickSpellCraft()
    {
        questInfoVisible = true;

        subTitle.gameObject.SetActive(false);
        Desc.fontSize = 75;

        var spellcraft = QuestsController.Quests.spellcraft;
        var progress = PlayerDataManager.playerData.quest.spell;

        Desc.text = LocalizeLookUp.GetText("daily_casting_upon");
        if (spellcraft.type != "")
        {
            if (spellcraft.relation != "")
            {
                if (spellcraft.relation == "ally")
                {
                    Desc.text = LocalizeLookUp.GetText("daily_casting_upon_ally") + " " + spellcraft.type;
                }
                else if (spellcraft.relation == "enemy")
                {
                    Desc.text = LocalizeLookUp.GetText("daily_casting_upon_enemy") + " " + spellcraft.type;
                }
                else if (spellcraft.relation == "coven")
                {
                    Desc.text = LocalizeLookUp.GetText("daily_casting_upon_coven").Replace("{{type}}", " " + spellcraft.type);
                }
                else if (spellcraft.relation == "own")
                {
                    Desc.text = LocalizeLookUp.GetText("daily_casting_upon_own") + " " + spellcraft.type;
                }
                else if (spellcraft.relation == "higher")
                {
                    Desc.text = LocalizeLookUp.GetText("daily_casting_upon_higher") + " " + spellcraft.type;
                }
                else if (spellcraft.relation == "lower")
                {
                    Desc.text = LocalizeLookUp.GetText("daily_casting_upon_lower") + " " + spellcraft.type;
                }
            }
            else
            {
                Desc.text = LocalizeLookUp.GetText("daily_casting_on_a").Replace("{{type}}", " " + spellcraft.type);
            }
        }

        if (string.IsNullOrEmpty(spellcraft.country) == false)
        {
            Desc.text += " " + LocalizeLookUp.GetText("daily_casting_location").Replace("{{Location}}", " " + LocalizeLookUp.GetCountryName(spellcraft.country));
        }
        if (spellcraft.ingredient != "")
        {
            Desc.text += " " + LocalizeLookUp.GetText("daily_casting_using").Replace("{{ingredient}}", " " + spellcraft.ingredient);
        }
        Desc.text = Desc.text.Replace("{{Spell Name}}", LocalizeLookUp.GetSpellName(spellcraft.spell)).Replace("{{amount}}", spellcraft.amount.ToString());

        Desc.text += ".";
        title.text = LocalizeLookUp.GetText("daily_spell");//"Spellcraft";
        completeText.text = "( " + progress.count + "/" + spellcraft.amount.ToString() + " )";
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




