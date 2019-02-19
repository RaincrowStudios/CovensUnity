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


    [Header("Screens")]
    [SerializeField] private GameObject m_BookOfShadows;
    [SerializeField] private Transform m_bosTransform;
    [SerializeField] private GameObject m_leaderBoards;
    [SerializeField] private Transform m_leaderboardTransform;
    [SerializeField] private GameObject m_summoning;
    [SerializeField] private Transform m_summoningTransform;
    [SerializeField] private GameObject m_inventory;
    [SerializeField] private Transform m_inventoryTransform;
    [SerializeField] private GameObject m_playerFeed;
    [SerializeField] private Transform m_playerFeedTransform;

    private void Awake()
    {
        Instance = this;
        //  m_WardrobeButton.onClick.AddListener(OnClickWardrobe);
        //   m_ConditionsButton.onClick.AddListener(OnClickConditions);
        //   m_MoonphaseButton.onClick.AddListener(()=>{}));
        //  m_RecallButton.onClick.AddListener(OnClickRecall);
        //   m_QuestsButton.onClick.AddListener(OnClickQuests);
        m_InventoryButton.onClick.AddListener(() => { Utilities.InstantiateUI(m_inventory, m_inventoryTransform); });
        m_QuestsButton.onClick.AddListener(() => { Utilities.InstantiateUI(m_playerFeed, m_playerFeedTransform); });
        //  m_CovenButton.onClick.AddListener(OnClickCoven);
        //    m_FlyButton.onClick.AddListener(OnClickFly);
        m_SpellbookButton.onClick.AddListener(() => { Utilities.InstantiateUI(m_BookOfShadows, m_bosTransform); });
        //     m_SettingsButton.onClick.AddListener(OnClickSettings);
        //    m_SummonButton.onClick.AddListener(OnClickSummon);
        //   m_SpiritDeckButton.onClick.AddListener(OnClickSpiritDeck);
        //   m_StoreButton.onClick.AddListener(OnClickStore);
        m_LeaderboardButton.onClick.AddListener(CreateLeaderboardsCoven);
        //   m_SummonButton.onClick.AddListener(() => { Utilities.InstantiateUI(m_summoning, m_summoningTransform); });
    }

    public void CreateLeaderboardsCoven()
    {
        Utilities.InstantiateUI(m_leaderBoards, m_leaderboardTransform);
    }

}
