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
    [SerializeField] private GameObject m_playerFeed;
    [SerializeField] private Transform m_playerFeedTransform;

    private void Awake()
    {
        Instance = this;
        m_InventoryButton.onClick.AddListener(() =>
        {
            System.Action<UIInventoryWheelItem> onSelectItem = (item) =>
            {
                if (item != null && item.inventoryItem != null)
                    UICollectableInfo.Instance.Show(item.inventoryItem.collectible);
            };

            System.Action onClickClose = () => { UIInventory.Instance.Close(); };
            UIInventory.Instance.Show(onSelectItem, onClickClose, true, true, true);
        });
        m_QuestsButton.onClick.AddListener(() => { Utilities.InstantiateUI(m_playerFeed, m_playerFeedTransform); });
        m_SpellbookButton.onClick.AddListener(() => { Utilities.InstantiateUI(m_BookOfShadows, m_bosTransform); });
        m_LeaderboardButton.onClick.AddListener(CreateLeaderboardsCoven);

        m_SummonButton.onClick.AddListener(() => SummoningController.Instance.Open());

    }

    public void CreateLeaderboardsCoven()
    {
        Utilities.InstantiateUI(m_leaderBoards, m_leaderboardTransform);
    }

}
