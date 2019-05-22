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
	[SerializeField] private GameObject o_Preview;



    [Header("PoP Info")]
    [SerializeField] private TextMeshProUGUI m_Title;
    [SerializeField] private TextMeshProUGUI m_DefendedBy;
    [SerializeField] private TextMeshProUGUI m_RewardOn;
    [SerializeField] private TextMeshProUGUI m_Status;
    [SerializeField] private Button m_EnterBtn;
    [SerializeField] private Button m_CloseBtn;

    public IMarker marker { get; private set; }
    public Token tokenData { get; private set; }
    

    private void Awake()
    {
		o_Preview.GetComponent<CanvasGroup> ().alpha = 0f;
		m_CanvasGroup.alpha = 0f;
        m_Canvas.enabled = false;
        m_InputRaycaster.enabled = false;
        m_EnterBtn.onClick.AddListener(Enter);
        m_CloseBtn.onClick.AddListener(Close);
    }

    public void Show(IMarker marker, Token data)
    {
		LeanTween.alphaCanvas (m_CanvasGroup, 1f, 0.3f).setEase (LeanTweenType.easeInCubic);
        this.tokenData = data;
        this.marker = marker;


        m_Canvas.enabled = true;
        m_InputRaycaster.enabled = true;
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
		if (string.IsNullOrEmpty(data.displayName)) {
			m_Title.text = "Place of Power";
		}
		else {
        m_Title.text = data.displayName;
		}

        if (!string.IsNullOrEmpty(data.controlledBy))
        {
			m_RewardOn.text = GetTime (data.rewardOn) + LocalizeLookUp.GetText ("pop_treasure_time");// "until this Place of Power yields treasure.";
        }
        else
        {
            m_Status.text = LocalizeLookUp.GetText("pop_unclaimed");
            m_DefendedBy.text = LocalizeLookUp.GetText("pop_hint");
        }
    }

    private void Enter()
    {
		Preview ();
    }
	private void Preview()
	{
		o_Preview.SetActive (true); //preview 
		LeanTween.alphaCanvas(o_Preview.GetComponent<CanvasGroup>(), 1f, 0.4f).setEase(LeanTweenType.easeInCubic);
		o_Preview.transform.GetChild (2).GetComponent<Button> ().onClick.AddListener (Close);

	}
    private void Close()
    {
		
		LeanTween.alphaCanvas (m_CanvasGroup, 0f, 0.3f).setEase (LeanTweenType.easeOutCubic).setOnComplete(() => {
			m_Canvas.enabled = false;
			m_InputRaycaster.enabled = false;
			o_Preview.SetActive(false);
			o_Preview.GetComponent<CanvasGroup> ().alpha = 0f;
		});
		//m_CanvasGroup.alpha = 0f;
       
       
		//transform.GetChild (8).gameObject.SetActive (false);
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