using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Raincrow.Chat;
using Raincrow.Chat.UI;
using Raincrow.FTF;
using Raincrow.GameEventResponses;

public class UIMain : MonoBehaviour
{
    [System.Serializable]
    public struct TextPanel
    {
        [SerializeField] public TextMeshProUGUI m_Title;
        [SerializeField] public TextMeshProUGUI m_Content;
        [SerializeField] public Button m_Button;

        [SerializeField] private RectTransform m_Panel;
        [SerializeField] private CanvasGroup m_CanvasGroup;

        private int m_TweenId;

        public bool IsOpen { get; private set; }
        
        public void Show(bool show)
        {
            IsOpen = show;

            LeanTween.cancel(m_TweenId);

            CanvasGroup cg = m_CanvasGroup;
            RectTransform rt = m_Panel;
            RectTransform contentRT = m_Content.GetComponent<RectTransform>();
            float topOffset = m_Title.fontSize * 1f;

            float startAlpha = cg.alpha;
            float endAlpha = show ? 1 : 0;

            Vector2 startPosition = rt.anchoredPosition;
            Vector2 endPosition = show ? new Vector2(0, -70) : new Vector2(0, rt.sizeDelta.y);
            
            if (show)
                rt.gameObject.SetActive(true);

            m_TweenId = LeanTween.value(0, 1, 0.5f)
                .setEaseOutCubic()
                .setOnUpdate((float t) =>
                {
                    cg.alpha = Mathf.Lerp(startAlpha, endAlpha, t);
                    rt.anchoredPosition = Vector2.Lerp(startPosition, endPosition, t);
                    rt.sizeDelta = contentRT.sizeDelta + new Vector2(0, topOffset);
                })
                .setOnComplete(() =>
                {
                    rt.gameObject.SetActive(show);
                })
                .uniqueId;
        }
    }

    public static UIMain Instance { get; private set; }

    [SerializeField] private UIMain_StateAnim m_StateAnim;
    
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

    [Header("Buttons2")]
    [SerializeField] private Button m_EnergyButton;
    
    [Header("Screens")]
    [SerializeField] private GameObject m_BookOfShadows;
    [SerializeField] private GameObject m_leaderBoards;

    [Header("FX")]
    [SerializeField] private GameObject m_ChannelingGlow;
    [SerializeField] private TextPanel m_EnergyTextPanel;

    private bool m_ChanneledFX;
    
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

        m_EnergyButton.onClick.AddListener(OnClickEnergy);
        m_EnergyTextPanel.m_Button.onClick.AddListener(() => m_EnergyTextPanel.Show(false));

        SpellCastHandler.OnApplyEffect += OnApplyStatusEffect;
        SpellCastHandler.OnExpireEffect += OnExpireStatusEffect;

        m_EnergyTextPanel.Show(false);
        m_ChannelingGlow.SetActive(false);
        
        m_EnergyTextPanel.m_Title.text = LocalizeLookUp.GetText("lt_none");
        m_EnergyTextPanel.m_Content.text = " ";
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
        MoonManager.Open();
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
            //BackButtonListener.RemoveCloseAction();
        };

        UIInventory.Instance.Show(onSelectItem, onClickClose, true, true);
        //BackButtonListener.AddCloseAction(UIInventory.Instance.Close);
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

    private void OnClickEnergy()
    {
        m_EnergyTextPanel.Show(!m_EnergyTextPanel.IsOpen);
    }

    ///////////////////////////////////////////

    private void OnApplyStatusEffect(string target, string caster, StatusEffect effect)
    {
        if (target != PlayerDataManager.playerData.instance)
            return;

        if (effect.HasStatus(SpellData.CHANNELED_STATUS))
        {
            if (m_ChanneledFX == false)
            {
                //enable channeled fx
                m_ChannelingGlow.gameObject.SetActive(true);

                //setup channeling panel
                m_EnergyTextPanel.m_Title.text = LocalizeLookUp.GetSpellName("spell_channeling");
                m_EnergyTextPanel.m_Content.text =
                       "+" + effect.modifiers.power + " " + LocalizeLookUp.GetText("generic_power") +
                       "\n+" + effect.modifiers.resilience + " " + LocalizeLookUp.GetText("generic_resilience") +
                       "\n+" + effect.modifiers.toCrit + "% " + LocalizeLookUp.GetText("generic_critical_cast_rate");

            }
        }
    }

    private void OnExpireStatusEffect(string target, StatusEffect effect)
    {
        if (target != PlayerDataManager.playerData.instance)
            return;

        if (effect.HasStatus(SpellData.CHANNELED_STATUS))
        {
            //disable channeled fx
            m_ChannelingGlow.gameObject.SetActive(false);

            //setup channeling panel
            m_EnergyTextPanel.m_Title.text = LocalizeLookUp.GetText("lt_none");
            m_EnergyTextPanel.m_Content.text = " ";
            m_EnergyTextPanel.Show(false);
        }
    }
}
