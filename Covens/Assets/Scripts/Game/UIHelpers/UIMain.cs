using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Raincrow.Chat;
using Raincrow.Chat.UI;
using Raincrow.FTF;

public class UIMain : MonoBehaviour
{
    public static UIMain Instance { get; private set; }
    
    [Header("Buttons")]
    [SerializeField] private Button m_WardrobeButton;
    [SerializeField] private Button m_MoonphaseButton;
    [SerializeField] private Button m_RecallButton;
    [SerializeField] private Button m_QuestsButton;
    [SerializeField] private Button m_InventoryButton;
    [SerializeField] private Button m_CovenButton;
    [SerializeField] private Button m_FlyButton;
    [SerializeField] private Button m_SpellbookButton;
    [SerializeField] private Button m_SettingsButton;
    [SerializeField] private Button m_SummonButton;
    [SerializeField] private Button m_StoreButton;
    [SerializeField] private Button m_LeaderboardButton;
    [SerializeField] private Button m_ChatButton;
    [SerializeField] private Button m_NearbyPopsButton;
    
    [Header("Screens")]
    [SerializeField] private GameObject m_BookOfShadows;
    [SerializeField] private Transform m_bosTransform;
    [SerializeField] private GameObject m_leaderBoards;
    [SerializeField] private Transform m_leaderboardTransform;

    
    private void Awake()
    {
        Instance = this;

        m_WardrobeButton.onClick.AddListener(OnClickWardrobe);
        m_MoonphaseButton.onClick.AddListener(OnClickMoonphase);
        m_RecallButton.onClick.AddListener(OnClickRecall);
        m_QuestsButton.onClick.AddListener(OnClickQuests);
        m_InventoryButton.onClick.AddListener(OnClickInventory);
        m_CovenButton.onClick.AddListener(OnClickCoven);
        m_FlyButton.onClick.AddListener(OnClickFly);
        m_SpellbookButton.onClick.AddListener(OnClickBookOfShadows);
        m_SettingsButton.onClick.AddListener(OnClickSettings);
        m_SummonButton.onClick.AddListener(OnClickSummoning);
        m_StoreButton.onClick.AddListener(OnClickStore);
        m_LeaderboardButton.onClick.AddListener(OnClickLeaderboards);
        m_ChatButton.onClick.AddListener(OnClickChat);
        m_NearbyPopsButton.onClick.AddListener(OnClickNearbyPops);
    }

    private void OnClickLeaderboards()
    {
        Utilities.InstantiateUI(m_leaderBoards, m_leaderboardTransform);
    }

    private void OnClickWardrobe()
    {
        ApparelManagerUI.Instance.Show();
    }

    private void OnClickMoonphase()
    {
        MoonManager.Instance.Open();
    }

    private void OnClickRecall()
    {
        if (FirstTapManager.IsFirstTime("recall"))
        {
            FirstTapManager.Show("recall", OnClickRecall);
            return;
        }

        PlayerManager.Instance.RecallHome();
    }

    private void OnClickQuests()
    {
        QuestLogUI.Open();
    }

    private void OnClickInventory()
    {
        System.Action<UIInventoryWheelItem> onSelectItem = (item) =>
        {
            if (item != null && item.inventoryItemId != null && item.inventoryItemId != UICollectableInfo.Instance.CollectableId)
                UICollectableInfo.Instance.Show(item.inventoryItemId);
            else
                UICollectableInfo.Instance.Close();
        };

        System.Action onClickClose = () => 
        {
            
        };

        UIInventory.Instance.Show(onSelectItem, onClickClose, true, true);
    }

    private void OnClickCoven()
    {
        TeamManagerUI.Open(TeamManager.MyCovenId);
    }

    private void OnClickFly()
    {
        if (FirstTapManager.IsFirstTime("flight"))
        {
            FirstTapManager.Show("flight", OnClickFly);
            return;
        }
        MapFlightTransition.Instance.FlyOut();
    }

    private void OnClickBookOfShadows()
    {
        Utilities.InstantiateUI(m_BookOfShadows, m_bosTransform);
    }

    private void OnClickSettings()
    {
        SettingsManager.OpenUI();
    }

    private void OnClickSummoning()
    {
        SummoningManager.Open();
    }

    private void OnClickStore()
    {
        ShopManager.OpenStore();
    }

    private void OnClickChat()
    {
        UIChat.Open();
    }

    private void OnClickNearbyPops()
    {
        UINearbyLocations.Open();
    }
}
