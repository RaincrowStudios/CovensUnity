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
    [SerializeField] private GameObject m_leaderBoards;

    [Header("FX")]
    [SerializeField] private GameObject m_ChannelingGlow;

    private bool m_IsChanneled;

    
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
        Utilities.InstantiateUI(m_leaderBoards, null);
    }

    private void OnClickWardrobe()
    {
        ApparelManagerUI.Show();
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
        Utilities.InstantiateUI(m_BookOfShadows, null);
    }

    private void OnClickSettings()
    {
        SettingsManager.OpenUI();
    }

    private void OnClickSummoning()
    {
        UISummoning.Open(
            () => MapsAPI.Instance.HideMap(true),
            (spirit) => 
            {
                UISummoning.Close();
                UISummonSuccess.Instance.Show(spirit.spiritData.id, () => MapCameraUtils.FocusOnPosition(spirit.transform.position, false, 2));
            },
            () => MapsAPI.Instance.HideMap(false));
    }

    private void OnClickStore()
    {
        UIStore.OpenStore();
    }

    private void OnClickChat()
    {
        UIChat.Open();
    }

    private void OnClickNearbyPops()
    {
        UINearbyLocations.Open();
    }

    private void OnApplyStatusEffect(string target, string caster, StatusEffect effect)
    {
        if (target != PlayerDataManager.playerData.instance)
            return;

        //if (PlayerManager.witchMarker.witchToken.)
        //if (PlayerManager.witchMarker.has)
    }
}
