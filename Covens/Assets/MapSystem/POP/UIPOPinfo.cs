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
    [SerializeField] private CanvasGroup m_CanvasGroup;
    [SerializeField] private CanvasGroup m_Loading;
    [SerializeField] private CanvasGroup m_LoadingBlock;


    [Header("PoP Info")]
    [SerializeField] private TextMeshProUGUI m_Title;
    [SerializeField] private TextMeshProUGUI m_DefendedBy;
    [SerializeField] private TextMeshProUGUI m_RewardOn;
    [SerializeField] private TextMeshProUGUI m_Status;
    [SerializeField] private Button m_EnterBtn;
    [SerializeField] private Button m_CloseBtn;

    public IMarker marker { get; private set; }
    public Token tokenData { get; private set; }
    public MarkerDataDetail details { get; private set; }

    private int m_TweenId;
    private int m_LoadingBlockTweenId;

    private void Awake()
    {
        m_Canvas.enabled = false;
        m_InputRaycaster.enabled = false;

        m_CanvasGroup.alpha = 0f;
        m_Loading.alpha = 0;
        m_LoadingBlock.alpha = 0;
        m_Loading.gameObject.SetActive(false);
        m_Loading.gameObject.SetActive(false);

        m_EnterBtn.onClick.AddListener(OnClickEnter);
        m_CloseBtn.onClick.AddListener(OnClickClose);
    }

    public void Show(IMarker marker, Token data)
    {
        this.tokenData = data;
        this.marker = marker;

        m_Title.text = "Place of Power";
        m_Status.text = "";
        m_RewardOn.text = "";
        m_DefendedBy.text = "";

        m_Canvas.enabled = true;
        m_InputRaycaster.enabled = true;

        m_Loading.gameObject.SetActive(true);
        m_Loading.alpha = 1;
        
        LeanTween.cancel(m_TweenId);
        m_TweenId = LeanTween.alphaCanvas(m_CanvasGroup, 1f, 0.3f).setEase(LeanTweenType.easeInCubic).uniqueId;
    }

    /*
        controlled by = "" / null / playerName/ CovenName
        is full
        displayName
        level
        is Coven = is the person owning in Coven or not
     */
    public void Setup(MarkerDataDetail data)
    {
        details = data;
        
        if (string.IsNullOrEmpty(data.displayName))
        {
            m_Title.text = "Place of Power";
        }
        else
        {
            m_Title.text = data.displayName;
        }

        if (!string.IsNullOrEmpty(data.controlledBy))
        {
            m_Status.text = "";
            m_DefendedBy.text = "Defended by " + data.controlledBy;

            m_RewardOn.text = GetTime(data.rewardOn) + LocalizeLookUp.GetText("pop_treasure_time");// "until this Place of Power yields treasure.";
        }
        else
        {
            m_RewardOn.text = "";
            m_Status.text = LocalizeLookUp.GetText("pop_unclaimed");
            m_DefendedBy.text = LocalizeLookUp.GetText("pop_hint");
        }


        LeanTween.alphaCanvas(m_Loading, 0f, 1f).setEaseOutCubic().setOnComplete(() => m_Loading.gameObject.SetActive(false));
    }

    private void OnClickEnter()
    {
        ShowLoadingBlock();

        PlaceOfPower.EnterPoP(tokenData.instance, (result, response) =>
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

    private void OnClickClose()
    {
        Close();
    }

    private void Close()
    {
        LeanTween.cancel(m_TweenId);

		m_TweenId = LeanTween.alphaCanvas (m_CanvasGroup, 0f, 0.3f).setEase (LeanTweenType.easeOutCubic).setOnComplete(() => {
			m_Canvas.enabled = false;
			m_InputRaycaster.enabled = false;
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