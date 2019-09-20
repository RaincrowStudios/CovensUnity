using Raincrow;
using Raincrow.FTF;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Raincrow.Maps;

public class UISummoning : MonoBehaviour
{
    private static UISummoning m_Instance;

    public static bool IsOpen { get; private set; }

    private static int m_PoPPosition;
    private static int m_PoPIsland;

    private static System.Action m_OnOpen;
    private static System.Action<SpiritMarker> m_OnSummon;
    private static System.Action<string> m_OnSummonPoP;
    private static System.Action m_OnClose;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="onOpen">triggered when the open animation is complete</param>
    /// <param name="onSummon">triggered when a summon is succesful</param>
    /// <param name="onClose">triggered when the close animation starts </param>
    public static void Open(System.Action onOpen, System.Action<SpiritMarker> onSummon, System.Action onClose)
    {
        m_OnOpen = onOpen;
        m_OnSummon = onSummon;
        m_OnClose = onClose;
        m_PoPPosition = -1;
        m_PoPIsland = -1;
        m_OnSummonPoP = null;

        if (m_Instance != null)
        {
            m_Instance._Open();
        }
        else
        {
            LoadingOverlay.Show();
            SceneManager.LoadSceneAsync(SceneManager.Scene.SUMMONING, UnityEngine.SceneManagement.LoadSceneMode.Additive, null, () =>
            {
                LoadingOverlay.Hide();
                m_Instance._Open();
            });
        }
    }

    public static void Open(int position, int island, System.Action onOpen, System.Action<string> onSummon, System.Action onClose)
    {
        Open(onOpen, null, onClose);
        m_PoPPosition = position;
        m_PoPIsland = island;
        m_OnSummonPoP = onSummon;
    }

    public static void Close()
    {
        if (m_Instance == null)
            return;

        m_Instance._Close();
    }

    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private GraphicRaycaster m_InputRaycaster;
    [SerializeField] private CanvasGroup m_CanvasGroup;
    [SerializeField] private RectTransform m_MainRect;
    [SerializeField] private SwipeDetector m_SwipeDetector;
    [SerializeField] private UISummoningSpiritInfo m_InfoPopup;

    [Header("header")]
    [SerializeField] private List<Button> m_TierButtons;
    [SerializeField] private Button m_CloseButton;

    [Header("spirit")]
    [SerializeField] private CanvasGroup m_PageCanvasGroup;
    [SerializeField] private RectTransform m_PageRect;
    [SerializeField] private GameObject m_SpiritContainer;
    [SerializeField] private GameObject m_NoSpiritContainer;
    [SerializeField] private Image m_SpiritArt;
    [SerializeField] private TextMeshProUGUI m_SpiritName;
    [SerializeField] private TextMeshProUGUI m_SpiritBehavior;
    [SerializeField] private TextMeshProUGUI m_SummonCost;
    [SerializeField] private Button m_SummonButton;
    [SerializeField] private Button m_InfoButton;

    [Header("")]
    [SerializeField] private TextMeshProUGUI m_PageCounter;
    [SerializeField] private Button m_PrevPageButton;
    [SerializeField] private Button m_NextPageButton;

    private int m_TweenId;
    private int m_PageTweenId;
    private int m_ArtTweenId;
    private int m_CurrentIndex = 0;
    private int m_CurrentTier = 1;
    private RectTransform m_CanvasRect;
    private List<List<SpiritData>> m_SpiritsByTier;
    
    private void Awake()
    {
        m_Instance = this;
        m_CanvasRect = m_Canvas.GetComponent<RectTransform>();

        m_Canvas.enabled = false;
        m_InputRaycaster.enabled = false;
        m_CanvasGroup.alpha = 0;
        m_NoSpiritContainer.gameObject.SetActive(false);
        m_InfoPopup.gameObject.SetActive(false);

        m_SummonButton.onClick.AddListener(OnClickSummon);
        m_InfoButton.onClick.AddListener(OnClickInfo);
        m_CloseButton.onClick.AddListener(OnClickClose);
        m_NextPageButton.onClick.AddListener(NextPage);
        m_PrevPageButton.onClick.AddListener(PrevPage);

        for (int i = 0; i < m_TierButtons.Count; i++)
        {
            int idx = i + 1;
            m_TierButtons[i].onClick.AddListener(() => OnClickTier(idx));
        }

        DownloadedAssets.OnWillUnloadAssets += DownloadedAssets_OnWillUnloadAssets;
    }

    private void DownloadedAssets_OnWillUnloadAssets()
    {
        if (IsOpen)
            return;

        DownloadedAssets.OnWillUnloadAssets -= DownloadedAssets_OnWillUnloadAssets;

        LeanTween.cancel(m_TweenId);
        SceneManager.UnloadScene(SceneManager.Scene.SUMMONING, null, null);
    }

    [ContextMenu("Open")]
    private void _Open()
    {
        IsOpen = true;

        if (FirstTapManager.IsFirstTime("summoning"))
            FirstTapManager.Show("summoning", null);

        m_InputRaycaster.enabled = true;
        m_Canvas.enabled = true;

        //animate
        LeanTween.cancel(m_TweenId);
        m_TweenId = LeanTween.value(m_CanvasGroup.alpha, 1f, 0.6f)
            .setEaseOutCubic()
            .setOnUpdate((float t) =>
            {
                m_CanvasGroup.alpha = t;
                m_MainRect.localScale = new Vector3(t, t, t);
            })
            .setOnComplete(m_OnOpen)
            .uniqueId;

        //setup
        m_SwipeDetector.SwipeRight += PrevPage;
        m_SwipeDetector.SwipeLeft += NextPage;

        m_SpiritsByTier = new List<List<SpiritData>>();
        for (int i = 0; i < m_TierButtons.Count; i++)
        {
            m_SpiritsByTier.Add(new List<SpiritData>());
            foreach(KnownSpirits item in PlayerDataManager.playerData.knownSpirits)
            {
                SpiritData data = DownloadedAssets.GetSpirit(item.spirit);

                if (LocationIslandController.isInBattle && data.type == "forbidden")
                    continue;

                if (data.tier == i + 1)
                    m_SpiritsByTier[i].Add(data);
            }
        }

        SetupPage();
        SetupPageCounter();
    }

    [ContextMenu("Close")]
    private void _Close()
    {
        m_OnClose?.Invoke();
        m_OnClose = null;

        IsOpen = false;
        m_SwipeDetector.SwipeRight -= PrevPage;
        m_SwipeDetector.SwipeLeft -= NextPage;
        m_InputRaycaster.enabled = false;

        //animate
        LeanTween.cancel(m_TweenId);
        m_TweenId = LeanTween.value(m_CanvasGroup.alpha, 0, 1f)
            .setEaseOutCubic()
            .setOnUpdate((float t) =>
            {
                m_CanvasGroup.alpha = t;
                m_MainRect.localScale = new Vector3(t, t, t);
            })
            .setOnComplete(() =>
            {
                m_Canvas.enabled = false;
            })
            .uniqueId;
    }

    private void AnimateLeft(System.Action onComplete)
    {
        LeanTween.cancel(m_PageTweenId);
        m_PageTweenId = LeanTween.value(1 - m_PageCanvasGroup.alpha, 1, m_PageCanvasGroup.alpha / 4f)
            .setEaseOutCubic()
            .setOnUpdate((float t) =>
            {
                m_PageCanvasGroup.alpha = 1 - t;
                m_PageRect.anchoredPosition = new Vector2(Mathf.Lerp(0, -m_CanvasRect.sizeDelta.x / 4f, t), 0);
            })
            .setOnComplete(onComplete)
            .uniqueId;
    }

    private void AnimateRight(System.Action onComplete)
    {
        LeanTween.cancel(m_PageTweenId);
        m_PageTweenId = LeanTween.value(1 - m_PageCanvasGroup.alpha, 1, m_PageCanvasGroup.alpha / 4f)
           .setEaseOutCubic()
           .setOnUpdate((float t) =>
           {
               m_PageCanvasGroup.alpha = 1 - t;
               m_PageRect.anchoredPosition = new Vector2(Mathf.Lerp(0, m_CanvasRect.sizeDelta.x / 5f, t), 0);
           })
           .setOnComplete(onComplete)
           .uniqueId;
    }

    private void AnimateCenter(System.Action onComplete)
    {
        LeanTween.cancel(m_PageTweenId);

        float start = -m_PageRect.anchoredPosition.x;
        float end = 0;

        m_PageTweenId = LeanTween.value(0 - m_PageCanvasGroup.alpha, 1, 0.25f)
            .setEaseInOutCubic()
            .setOnUpdate((float t) =>
            {
                m_PageCanvasGroup.alpha = t;
                m_PageRect.anchoredPosition = new Vector2(Mathf.Lerp(start, end, t), 0);
            })
            .setOnComplete(onComplete)
            .uniqueId;
    }

    private void NextPage()
    {
        m_CurrentIndex += 1;
        if (m_CurrentIndex >= m_SpiritsByTier[m_CurrentTier-1].Count)
            m_CurrentIndex = 0;
        
        //pan and fade out
        AnimateLeft(() =>
        {
            SetupPage();
            AnimateCenter(null);
        });

        SetupPageCounter();
    }

    private void PrevPage()
    {
        m_CurrentIndex -= 1;
        if (m_CurrentIndex < 0)
            m_CurrentIndex = m_SpiritsByTier[m_CurrentTier - 1].Count - 1;

        AnimateRight(() =>
        {
            SetupPage();
            AnimateCenter(null);
        });

        SetupPageCounter();
    }

    private void SetupPage()
    {
        if (m_SpiritsByTier[m_CurrentTier - 1].Count == 0)
        {
            m_SpiritContainer.SetActive(false);
            m_NoSpiritContainer.SetActive(true);
            return;
        }

        m_SpiritContainer.SetActive(true);
        m_NoSpiritContainer.SetActive(false);

        int spiritIdx = m_CurrentIndex;
        SpiritData spirit = m_SpiritsByTier[m_CurrentTier-1][spiritIdx];
                
        m_SpiritName.text = spirit.Name;
        m_SpiritBehavior.text = spirit.Behavior;
        m_SpiritArt.overrideSprite = null;
        m_SpiritArt.color = Color.black;
        m_SummonButton.interactable = SummoningManager.CanSummon(spirit.id);
        int summonCost = PlayerDataManager.SummoningCosts[spirit.tier - 1] * (LocationIslandController.isInBattle ? 3 : 1);
        m_SummonCost.text = LocalizeLookUp.GetText("spell_data_cost").Replace("{{Energy Cost}}", summonCost.ToString());

        DownloadedAssets.GetSprite(spirit.id, spr =>
        {
            if (m_SpiritArt == null)
                return;
            if (spiritIdx != m_CurrentIndex)
                return;

            LeanTween.cancel(m_ArtTweenId);
            m_SpiritArt.overrideSprite = spr;
            m_ArtTweenId = LeanTween.color(m_SpiritArt.rectTransform, Color.white, 1f).uniqueId;
        });
    }

    private void SetupPageCounter()
    {
        m_PageCounter.text = $"{m_CurrentIndex+1}/{m_SpiritsByTier[m_CurrentTier-1].Count}";
    }

    private void OnClickClose()
    {
        _Close();
    }

    private void OnClickSummon()
    {
        string spiritId = m_SpiritsByTier[m_CurrentTier-1][m_CurrentIndex].id;

        if (m_PoPPosition < 0 || m_PoPIsland < 0)
        {
            LoadingOverlay.Show();
            SummoningManager.Summon(spiritId, (marker, error) =>
            {
                LoadingOverlay.Hide();
                if (string.IsNullOrEmpty(error))
                {
                    SoundManagerOneShot.Instance.SpiritSummon();
                    m_OnSummon?.Invoke(marker);
                }
                else
                {
                    UIGlobalPopup.ShowError(_Close, APIManager.ParseError(error));
                }
            });
        }
        else
        {
            LoadingOverlay.Show();
            SummoningManager.SummonPoP(spiritId, m_PoPPosition, m_PoPIsland, (error) =>
            {
                LoadingOverlay.Hide();
                if (string.IsNullOrEmpty(error))
                {
                    SoundManagerOneShot.Instance.SpiritSummon();
                    m_OnSummonPoP?.Invoke(null);
                }
                else
                {
                    m_OnSummonPoP?.Invoke(error);
                }
            });
        }
    }

    private void OnClickInfo()
    {
        string spiritId = m_SpiritsByTier[m_CurrentTier - 1][m_CurrentIndex].id;
        SpiritData spirit = DownloadedAssets.GetSpirit(spiritId);
        m_InfoPopup.Show(spirit);
    }

    private void OnClickTier(int tier)
    {
        if (m_CurrentTier == tier)
            return;

        for (int i = 0; i < m_TierButtons.Count; i++)
        {
            if (i + 1 == tier)
                m_TierButtons[i].targetGraphic.color = Color.white;
            else
                m_TierButtons[i].targetGraphic.color = Color.grey;
        }

        if (tier > m_CurrentTier)
        {
            AnimateLeft(() =>
            {
                SetupPage();
                SetupPageCounter();
                AnimateCenter(null);
            });
        }
        else
        {
            AnimateRight(() =>
            {
                SetupPage();
                SetupPageCounter();
                AnimateCenter(null);
            });
        }

        m_CurrentIndex = 0;
        m_CurrentTier = tier;
    }
}
