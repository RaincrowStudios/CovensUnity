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

    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private GraphicRaycaster m_InputRaycaster;
    [SerializeField] private CanvasGroup m_CanvasGroup;

    [Header("PoP Info")]
    [SerializeField] private TextMeshProUGUI m_Title;
    [SerializeField] private TextMeshProUGUI m_DefendedBy;
    [SerializeField] private TextMeshProUGUI m_RewardOn;
    [SerializeField] private TextMeshProUGUI m_Status;
    [SerializeField] private Button m_EnterBtn;
    [SerializeField] private Button m_CloseBtn;
    

    private void Awake()
    {
        m_Canvas.enabled = false;
        m_EnterBtn.onClick.AddListener(Enter);
        m_CloseBtn.onClick.AddListener(Close);
    }

    public void Show(IMarker marker)
    {

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
        m_Title.text = data.displayName;
        if (!string.IsNullOrEmpty(data.controlledBy))
        {
            m_RewardOn.text = GetTime(data.rewardOn) + "until this Place of Power yields treasure.";
        }
        else
        {
            m_Status.text = LocalizeLookUp.GetText("pop_unclaimed");
            m_DefendedBy.text = LocalizeLookUp.GetText("pop_hint");
        }
    }

    private void Enter()
    {

    }

    private void Close()
    {

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