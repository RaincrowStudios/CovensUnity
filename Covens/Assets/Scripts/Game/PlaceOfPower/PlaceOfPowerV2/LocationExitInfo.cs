using UnityEngine;
using UnityEngine.UI;

public class LocationExitInfo : UIInfoPanel
{
    private static LocationExitInfo m_Instance;
    [SerializeField] private Button m_CloseBtn;
    [SerializeField] private Image m_LocationIcon;

    public static LocationExitInfo Instance
    {
        get
        {
            if (m_Instance == null)
                m_Instance = Instantiate(Resources.Load<LocationExitInfo>("LocationExitInfo"));
            return m_Instance;

        }
    }

    public static bool isShowing
    {
        get
        {
            if (m_Instance == null) return false;
            else return m_Instance.m_IsShowing;
        }
    }

    protected override void Awake()
    {
        m_Instance = this;
        base.Awake();
        m_CloseBtn.onClick.AddListener(Close);
    }

    public void ShowUI()
    {
        base.Show();
        m_LocationIcon.sprite = LocationPOPInfo.selectedPopSprite;
        m_LocationIcon.color = Color.white;
    }

}