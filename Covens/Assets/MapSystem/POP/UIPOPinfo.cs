using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Raincrow.Maps;

public class UIPOPinfo : MonoBehaviour
{
    private static UIPOPinfo m_Instance;
    public static UIPOPinfo Instance
    {
        get
        {
            if (m_Instance == null)
                m_Instance = Instantiate(Resources.Load<UIPOPinfo>("UIPOPinfo"));
            return m_Instance;
        }
    }

    public static bool isOpen
    {
        get
        {
            if (m_Instance == null)
                return false;
            else
                return m_Instance.m_Canvas.enabled;
        }
    }

    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private GraphicRaycaster m_InputRaycaster;
    [SerializeField] private CanvasGroup m_Loading;
    [SerializeField] private CanvasGroup m_LoadingBlock;


    [Header("PoP Info - Claimed")]
    [SerializeField] private CanvasGroup m_UnclaimedGroup;
    [SerializeField] private TextMeshProUGUI m_UnclaimedTitle;
    [SerializeField] private TextMeshProUGUI m_UnclaimedDefendedBy;
    [SerializeField] private Image m_UnclaimedSpiritArt;
    [SerializeField] private Button m_UnclaimedEnterBtn;
    [SerializeField] private Button m_UnclaimedOfferingBtn;
    [SerializeField] private Button m_UnclaimedCloseBtn;
    
    [Header("PoP Info - Unclaimed")]
    [SerializeField] private CanvasGroup m_ClaimedGroup;
    [SerializeField] private TextMeshProUGUI m_ClaimedTitle;
    [SerializeField] private TextMeshProUGUI m_ClaimedDefendedBy;
    [SerializeField] private TextMeshProUGUI m_ClaimedOwner;
    [SerializeField] private TextMeshProUGUI m_ClaimedRewardOn;
    [SerializeField] private Button m_ClaimedEnterBtn;
    [SerializeField] private Button m_ClaimedCloseBtn;

    public IMarker marker { get; private set; }
    public Token tokenData { get; private set; }
    public LocationMarkerDetail details { get; private set; }

    private int m_TweenId;
    private int m_LoadingBlockTweenId;

    private void Awake()
    {
        m_Canvas.enabled = false;
        m_InputRaycaster.enabled = false;

        m_ClaimedGroup.alpha = m_UnclaimedGroup.alpha = 0;
        m_Loading.alpha = 0;
        m_LoadingBlock.alpha = 0;
        m_Loading.gameObject.SetActive(false);
        m_ClaimedGroup.gameObject.SetActive(false);
        m_UnclaimedGroup.gameObject.SetActive(false);

        m_ClaimedEnterBtn.onClick.AddListener(OnClickEnter);
        m_UnclaimedEnterBtn.onClick.AddListener(OnClickEnter);

        m_ClaimedCloseBtn.onClick.AddListener(OnClickClose);
        m_UnclaimedCloseBtn.onClick.AddListener(OnClickClose);

        m_UnclaimedOfferingBtn.onClick.AddListener(OnClickOffering);
    }

    public void Show(IMarker marker, Token data)
    {
        this.tokenData = data;
        this.marker = marker;

        bool isUnclaimed = string.IsNullOrEmpty(data.owner);

        if (isUnclaimed)
        {
            m_UnclaimedTitle.text = LocalizeLookUp.GetText("pop_title");
            m_UnclaimedDefendedBy.text = "";
            m_UnclaimedSpiritArt.color = new Color(0, 0, 0, 0);

            m_ClaimedGroup.gameObject.SetActive(false);
            m_UnclaimedGroup.gameObject.SetActive(true);
        }
        else
        {
            m_ClaimedTitle.text = LocalizeLookUp.GetText("pop_title");
            m_ClaimedDefendedBy.text = "";
            m_ClaimedOwner.text = m_ClaimedRewardOn.text = "";

            m_ClaimedGroup.gameObject.SetActive(true);
            m_UnclaimedGroup.gameObject.SetActive(false);
        }

        m_Canvas.enabled = true;
        m_InputRaycaster.enabled = true;

        m_Loading.gameObject.SetActive(true);
        m_Loading.alpha = 1;
        
        LeanTween.cancel(m_TweenId);
        m_TweenId = LeanTween.alphaCanvas(isUnclaimed ? m_UnclaimedGroup : m_ClaimedGroup, 1f, 0.3f).setEase(LeanTweenType.easeInCubic).uniqueId;
    }

    /*
        controlled by = "" / null / playerName/ CovenName
        is full
        displayName
        level
        is Coven = is the person owning in Coven or not
     */
    public void Setup(LocationMarkerDetail data)
    {
        details = data;
        SpiritDict spirit = string.IsNullOrEmpty(data.spiritId) ? null : DownloadedAssets.GetSpirit(data.spiritId);

        bool isUnclaimed = string.IsNullOrEmpty(data.controlledBy);

        if (isUnclaimed)
        {
            if (!string.IsNullOrEmpty(data.displayName))
                m_UnclaimedTitle.text = data.displayName;

            m_UnclaimedDefendedBy.text = (spirit == null ? "" : LocalizeLookUp.GetText("pop_defended").Replace("{{spirit}}", spirit.spiritName).Replace("{{tier}}", spirit.spiritTier.ToString()));
            
            if (spirit != null)
                DownloadedAssets.GetSprite(data.spiritId, (spr) =>
                {
                    m_UnclaimedSpiritArt.overrideSprite = spr;
                    LeanTween.color(m_UnclaimedSpiritArt.rectTransform, Color.white, 1f).setEaseOutCubic();
                });
        }
        else
        {
            if (!string.IsNullOrEmpty(data.displayName))
                m_ClaimedTitle.text = data.displayName;

            m_ClaimedDefendedBy.text = (spirit == null ? "" : LocalizeLookUp.GetText("pop_defended").Replace("{{spirit}}", spirit.spiritName).Replace("{{tier}}", spirit.spiritTier.ToString()));

            if (data.isCoven)
                m_ClaimedOwner.text = LocalizeLookUp.GetText("pop_owner_coven").Replace("{{coven}}", data.controlledBy);
            else
                m_ClaimedOwner.text = LocalizeLookUp.GetText("pop_owner_player").Replace("{{player}}", data.controlledBy);

            if (data.rewardOn != 0)
                m_ClaimedRewardOn.text = LocalizeLookUp.GetText("pop_treasure_time").Replace("{{time}}", GetTime(data.rewardOn));
            else
                m_ClaimedRewardOn.text = "";
        }
        
        LeanTween.alphaCanvas(m_Loading, 0f, 1f).setEaseOutCubic().setOnComplete(() => m_Loading.gameObject.SetActive(false));
    }

    private void OnClickEnter()
    {
        ShowLoadingBlock();

        PlaceOfPower.EnterPoP(marker, details, (result, response) =>
        {
            if (result == 200)
            {
                //close the UI
                Close();
            }
            else
            {
                //show error and hide the loading block
                UIGlobalErrorPopup.ShowError(HideLoadingBlock, "Error entering location: " + response);
            }
        });
    }

    private void OnClickOffering()
    {
        PlaceOfPower.StartOffering();
    }

    private void OnClickClose()
    {
        Close();
    }

    private void Close()
    {
        LeanTween.cancel(m_TweenId);

        m_TweenId = LeanTween.alphaCanvas(string.IsNullOrEmpty(details.controlledBy) ? m_UnclaimedGroup : m_ClaimedGroup, 0f, 0.3f)
            .setEase(LeanTweenType.easeOutCubic)
            .setOnComplete(() =>
            {
                m_Canvas.enabled = false;
                m_InputRaycaster.enabled = false;
                m_ClaimedGroup.gameObject.SetActive(false);
                m_UnclaimedGroup.gameObject.SetActive(false);
                m_UnclaimedSpiritArt.overrideSprite = null;
            }).uniqueId;

        HideLoadingBlock();
    }

    private void ShowLoadingBlock()
    {
        m_Loading.gameObject.SetActive(true);
        m_LoadingBlock.gameObject.SetActive(true);

        LeanTween.cancel(m_LoadingBlockTweenId);
        m_LoadingBlockTweenId = LeanTween.value(m_LoadingBlock.alpha, 1f, 0.5f)
            .setOnUpdate((float v) =>
            {
                m_LoadingBlock.alpha = v;
                m_Loading.alpha = v;
            })
            .uniqueId;
    }

    private void HideLoadingBlock()
    {
        LeanTween.cancel(m_LoadingBlockTweenId);
        m_LoadingBlockTweenId = LeanTween.value(m_LoadingBlock.alpha, 0f, 0.5f)
            .setOnUpdate((float v) =>
            {
                m_LoadingBlock.alpha = v;
                m_Loading.alpha = v;
            })
            .setOnComplete(() =>
            {
                m_Loading.gameObject.SetActive(false);
                m_LoadingBlock.gameObject.SetActive(false);
            })
            .uniqueId;
    }

    private string GetTime(double javaTimeStamp)
    {
        if (javaTimeStamp < 159348924)
        {
            string s = "unknown";
            return s;
        }

        System.DateTime dtDateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        dtDateTime = dtDateTime.AddMilliseconds(javaTimeStamp).ToUniversalTime();
        var timeSpan = dtDateTime.Subtract(System.DateTime.UtcNow);
        string stamp = "";

        if (timeSpan.Days > 1)
        {
            stamp = timeSpan.Days.ToString() + " days, ";

        }
        else if (timeSpan.Days == 1)
        {
            stamp = timeSpan.Days.ToString() + " day, ";
        }
        if (timeSpan.Hours > 1)
        {
            stamp += timeSpan.Hours.ToString() + " hours ";
        }
        else if (timeSpan.Hours == 1)
        {
            stamp += timeSpan.Hours.ToString() + " hour ";
        }
        else
        {
            if (timeSpan.Minutes > 1)
            {
                stamp += timeSpan.Minutes.ToString() + " minutes ";
            }
            else if (stamp.Length < 4)
            {
                stamp.Remove(4);
            }
        }
        return stamp;
    }

}