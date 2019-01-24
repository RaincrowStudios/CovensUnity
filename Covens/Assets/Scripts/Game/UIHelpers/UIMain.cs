using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIMain : MonoBehaviour
{
    public static UIMain Instance { get; private set; }

    [Header("Anim")]
    [SerializeField] private CanvasGroup m_CanvasGroup;

    [Header("Buttons")]
    [SerializeField] private Button m_WardrobeButton;
    [SerializeField] private Button m_ConditionsButton;
    [SerializeField] private Button m_MoonphaseButton;
    [SerializeField] private Button m_RecallButton;
    [SerializeField] private Button m_QuestsButton;
    [SerializeField] private Button m_InventoryButton;
    [SerializeField] private Button m_CovenButton;
    [SerializeField] private Button m_FlyButton;
    [SerializeField] private Button m_SpellbookButton;
    [SerializeField] private Button m_SettingsButton;
    [SerializeField] private Button m_SummonButton;
    [SerializeField] private Button m_SpiritDeckButton;
    [SerializeField] private Button m_StoreButton;
    [SerializeField] private Button m_LeaderboardButton;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI m_LevelText;
    [SerializeField] private TextMeshProUGUI m_ConditionCountText;
    [SerializeField] private TextMeshProUGUI m_ExperienceText;
    [SerializeField] private TextMeshProUGUI m_EnergyText;

    private void Awake()
    {
        Instance = this;

        m_CanvasGroup.alpha = 0;
        m_CanvasGroup.interactable = false;

        m_WardrobeButton.onClick.AddListener(OnClickWardrobe);
        m_ConditionsButton.onClick.AddListener(OnClickConditions);
        m_MoonphaseButton.onClick.AddListener(OnClickMoonphase);
        m_RecallButton.onClick.AddListener(OnClickRecall);
        m_QuestsButton.onClick.AddListener(OnClickQuests);
        m_InventoryButton.onClick.AddListener(OnClickInventory);
        m_CovenButton.onClick.AddListener(OnClickCoven);
        m_FlyButton.onClick.AddListener(OnClickFly);
        m_SpellbookButton.onClick.AddListener(OnClickSpellbook);
        m_SpellbookButton.onClick.AddListener(OnClickSettings);
        m_SummonButton.onClick.AddListener(OnClickSummon);
        m_SpiritDeckButton.onClick.AddListener(OnClickSpiritDeck);
        m_StoreButton.onClick.AddListener(OnClickStore);
        m_LeaderboardButton.onClick.AddListener(OnClickLeaderboard);
    }

    private void Start()
    {
        
    }

    public void Show()
    {
        m_CanvasGroup.interactable = true;
        LeanTween.value(m_CanvasGroup.alpha, 1, 0.25f)
            .setOnUpdate((float t) =>
            {
                m_CanvasGroup.alpha = t;
            });
    }

    public void Hide()
    {
        m_CanvasGroup.interactable = false;
        LeanTween.value(m_CanvasGroup.alpha, 0, 0.25f)
            .setOnUpdate((float t) =>
            {
                m_CanvasGroup.alpha = t;
            });
    }

    private void OnClickWardrobe()
    {
        ApparelManagerUI.Instance.Show();
    }

    private void OnClickConditions()
    {
        ConditionsManager.Instance.Animate();
    }

    private void OnClickMoonphase()
    {
        MoonManager.Instance.Open();
    }

    private void OnClickRecall()
    {
        Recall.Instance.RecallHome();
    }

    private void OnClickQuests()
    {
        QuestLogUI.Instance.Open();
    }

    private void OnClickInventory()
    {
        //enable inventory object instance
        // call InventoryTransitionControl.Instance.OnAnimateIn();
        //enable inventory object instance's child "InputBlocker"
    }

    private void OnClickCoven()
    {
        TeamManagerUI.Instance.Show();
    }

    private void OnClickFly()
    {
        PlayerManager.Instance.Fly();
    }

    private void OnClickSpellbook()
    {
        SpellBookScrollController.Instance.Open();
    }

    private void OnClickSettings()
    {
        SettingsManager.Instance.Show();
    }

    private void OnClickSummon()
    {
        SummoningManager.Instance.Open();
    }

    private void OnClickSpiritDeck()
    {
        SpiritDeckUIManager.Instance.TurnOn();
    }

    private void OnClickStore()
    {
        StoreUIManager.Instance.GetStore();
    }
    
    private void OnClickLeaderboard()
    {
        Leaderboards.Instance.Show();
    }
}
