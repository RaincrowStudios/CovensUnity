using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using Raincrow;
using Raincrow.GameEventResponses;
using Raincrow.FTF;

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
    public CanvasGroup claimLoadingFx;
    public GameObject openChest;
    public GameObject closedChest;

    public Text rewardEnergy;
    public Text rewardGold;
    public Text rewardSilver;
    public Button buttonTapChest;

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
    public CanvasGroup eventLogLoading;

    private bool isOpen = false;
    private bool questInfoVisible = false;

    private static QuestLogUI m_Instance;
    private int m_TweenId;

    private List<EventLog> m_Logs;
    private float m_LastLogRequest;

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
        buttonTapChest.onClick.AddListener(OnClickClaimChest);

        claimLoadingFx.gameObject.SetActive(false);
        eventLogLoading.alpha = 0;
    }

    [ContextMenu("Show")]
    public void Show()
    {
        if (isOpen)
            return;

        BackButtonListener.AddCloseAction(Hide);

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

        if (FirstTapManager.IsFirstTime("daily"))
            FirstTapManager.Show("daily", null);


    }

    [ContextMenu("Hide")]
    public void Hide()
    {
        if (isOpen == false)
            return;

        BackButtonListener.RemoveCloseAction();

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

    void GetLogs(System.Action<List<EventLog>, string> callback)
    {
        if (m_Logs != null && Time.realtimeSinceStartup - m_LastLogRequest < 60)
        {
            callback?.Invoke(m_Logs, null);
            return;
        }

        m_LastLogRequest = Time.realtimeSinceStartup;

        APIManager.Instance.Get("character/eventLog", (response, result) =>
        {
            if (result == 200)
            {
                m_Logs = JsonConvert.DeserializeObject<List<EventLog>>(response);
                callback?.Invoke(m_Logs, null);
            }
            else
            {
                Debug.LogError("eventlog request error\n" + response);
                callback?.Invoke(null, APIManager.ParseError(response));
            }
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

        LeanTween.alphaCanvas(eventLogLoading, 1f, 0.5f).setEaseOutCubic();
        GetLogs((logs, error) =>
        {
            LeanTween.alphaCanvas(eventLogLoading, 0f, 1f).setEaseOutCubic();
            LS.log = logs;
            if (string.IsNullOrEmpty(error))
                SetupLogs();
            else
                UIGlobalPopup.ShowError(null, error);
        });
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
                bottomInfo.text = LocalizeLookUp.GetText("daily_tap_chest");//"Tap the chest to claim rewards";
                buttonTapChest.gameObject.SetActive(true);
            }
            else
            {
                openChest.SetActive(true);
                closedChest.SetActive(false);
                claimFX.SetActive(false);
                StartCoroutine(NewQuestTimer());
                buttonTapChest.gameObject.SetActive(false);
            }
        }
        else
        {
            openChest.SetActive(false);
            closedChest.SetActive(true);
            claimFX.SetActive(false);
            StartCoroutine(NewQuestTimer());
            buttonTapChest.gameObject.SetActive(false);
        }
    }

    IEnumerator ShowRewards(DailyRewards reward)
    {
        SoundManagerOneShot.Instance.PlayReward();
        if (reward.silver != 0)
        {
            rewardSilver.gameObject.SetActive(true);
            rewardSilver.text = "+" + reward.silver.ToString() + " " + LocalizeLookUp.GetText("store_silver");//" Silver!";
        }

        yield return new WaitForSeconds(1f);
        if (reward.gold != 0)
        {
            rewardGold.gameObject.SetActive(true);
            rewardGold.text = "+" + reward.gold.ToString() + " " + LocalizeLookUp.GetText("store_gold");//Gold!";
        }

        yield return new WaitForSeconds(1.8f);
        if (reward.energy != 0)
        {
            rewardEnergy.gameObject.SetActive(true);
            rewardEnergy.text = "+" + reward.energy.ToString() + " " + LocalizeLookUp.GetText("card_witch_energy");//Energy!";
        }

        openChest.SetActive(true);
        closedChest.SetActive(false);
        claimFX.SetActive(false);
        PlayerManagerUI.Instance.UpdateDrachs();
        PlayerManagerUI.Instance.UpdateEnergy();
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

    private void OnClickClaimChest()
    {
        if (PlayerDataManager.playerData.quest.completed)
            return;

        claimLoadingFx.alpha = 0;
        claimLoadingFx.gameObject.SetActive(true);
        LeanTween.alphaCanvas(claimLoadingFx, 0.7f, 0.25f).setEaseOutCubic();

        QuestsController.ClaimRewards((rewards, error) =>
        {
            LeanTween.alphaCanvas(claimLoadingFx, 0f, 2)
                .setEaseOutCubic()
                .setOnComplete(() => claimLoadingFx.gameObject.SetActive(false));

            if (string.IsNullOrEmpty(error))
            {
                StartCoroutine(ShowRewards(rewards));
            }
            else
            {
                UIGlobalPopup.ShowError(null, error);
            }
        });
    }

    public void ClickExplore()
    {
        questInfoVisible = true;

        subTitle.gameObject.SetActive(true);
        subTitle.text = LocalizeLookUp.GetExploreTitle(QuestsController.Quests.explore.type);
        Desc.text = LocalizeLookUp.GetExploreDesc(QuestsController.Quests.explore.type);
        title.text = LocalizeLookUp.GetText("daily_explore");//"Explore";

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

        title.text = LocalizeLookUp.GetText("daily_gather");//"Gather";
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
        if (string.IsNullOrEmpty(spellcraft.type) == false)
        {
            if (string.IsNullOrEmpty(spellcraft.relation) == false)
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
                Desc.text = LocalizeLookUp.GetText("daily_casting_on_a").Replace("{{type}}", " " + spellcraft.type).Replace("{{amount}}", spellcraft.amount.ToString());
            }
        }

        if (string.IsNullOrEmpty(spellcraft.country) == false)
        {
            Desc.text += " " + LocalizeLookUp.GetText("daily_casting_location").Replace("{{Location}}", " " + LocalizeLookUp.GetCountryName(spellcraft.country));
        }
        if (string.IsNullOrEmpty(spellcraft.ingredient) == false)
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

public struct EventLog
{
    public struct Data
    {
        public string spirit;
        public string spiritId;
        public string spellId;
        public int casterDegree;
        public int energyChange;
        public string casterName;
        //public string witchCreated;
    }

    public string character;
    public string type;
    public double createdOn;
    public Data data;
}




