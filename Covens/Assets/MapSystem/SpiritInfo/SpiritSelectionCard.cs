using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Raincrow.Maps;

public class SpiritSelectionCard : MonoBehaviour
{
    private static SpiritSelectionCard m_Instance;
    public static SpiritSelectionCard Instance
    {
        get
        {
            if (m_Instance == null)
                m_Instance = Resources.Load<SpiritSelectionCard>("UISpiritInfo");
            return m_Instance;
        }
    }

    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private GraphicRaycaster m_InputRaycaster;
    [SerializeField] private CanvasGroup m_CanvasGroup;

    [Header("SpiritCard")]
    //public GameObject SpiritCard;
    public Image spiritSprite;
    public TextMeshProUGUI title;
    public TextMeshProUGUI legend;
    public TextMeshProUGUI desc;
    public TextMeshProUGUI tier;
    public GameObject wild;
    public GameObject owned;
    public TextMeshProUGUI ownedBy;
    public TextMeshProUGUI covenBy;
    public TextMeshProUGUI behaviorOwned;
    public TextMeshProUGUI behaviorWild;
    public TextMeshProUGUI atkText;

    public Button atkButton;
    public Button close;

    private Token m_Spirit;
    private MarkerDataDetail m_Details;
    private IMarker m_Marker;
    private SpiritDict m_SpiritData;

    public Token Spirit { get { return m_Spirit; } }
    
    private void Awake()
    {
        m_CanvasGroup.alpha = 0;
        m_Canvas.enabled = false;
        m_InputRaycaster.enabled = false;

        atkButton.onClick.AddListener(OnClickAttack);
        close.onClick.AddListener(OnClickClose);
    }

    public void Show(IMarker marker, Token token)
    {
        m_Spirit = token;
        m_Marker = marker;
        m_Details = null;

        m_SpiritData = DownloadedAssets.spiritDictData[token.spiritId];

        //spirit info
        title.text = m_SpiritData.spiritName;
        if (m_SpiritData.spiritTier == 1)
            tier.text = "Lesser Spirit";
        else if (m_SpiritData.spiritTier == 2)
            tier.text = "Greater Spirit";
        else if (m_SpiritData.spiritTier == 3)
            tier.text = "Superior Spirit";
        else
            tier.text = "Legendary Spirit";

        legend.text = m_SpiritData.spiritLegend;
        desc.text = m_SpiritData.spiritDescription;
        DownloadedAssets.GetSprite(token.spiritId, spiritSprite);

        //behavior and owner info
        //if (token.owner == "")
        //{
        //    wild.SetActive(true);
        //    owned.SetActive(false);
        //    behaviorWild.text = m_SpiritData.spriitBehavior;
        //}
        //else
        //{
        //    wild.SetActive(false);
        //    owned.SetActive(true);
        //    ownedBy.text = "Summoned By: " + details.owner;
        //    covenBy.text = (details.covenName == "" ? "Coven: None" : "Coven: " + details.covenName);
        //    behaviorOwned.text = m_SpiritData.spriitBehavior;
        //}

        SetSilenced(BanishManager.isSilenced);

        m_Canvas.enabled = true;
        m_InputRaycaster.enabled = true;

        Reopen();
    }

    public void SetupDetails(MarkerDataDetail details)
    {
        m_Details = details;

        if (details.owner == "")
        {
            wild.SetActive(true);
            owned.SetActive(false);
            behaviorWild.text = m_SpiritData.spriitBehavior;
        }
        else
        {
            wild.SetActive(false);
            owned.SetActive(true);
            ownedBy.text = "Summoned By: " + details.owner;
            covenBy.text = (details.covenName == "" ? "Coven: None" : "Coven: " + details.covenName);
            behaviorOwned.text = m_SpiritData.spriitBehavior;
        }
    }

    public void Reopen()
    {
        LeanTween.alphaCanvas(m_CanvasGroup, 1, .4f);
    }

    void SetSilenced(bool isTrue)
    {
        var castingButton = atkButton.GetComponent<Button>();
        if (isTrue)
        {

            atkText.color = Color.gray;

            castingButton.enabled = false;
            atkText.text = "You are silenced for " + Utilities.GetSummonTime(BanishManager.silenceTimeStamp);
            StartCoroutine(SilenceUpdateTimer());
        }
        else
        {
            atkText.color = Color.white;
            castingButton.enabled = true;
            atkText.text = "Cast";
        }
    }


    IEnumerator SilenceUpdateTimer()
    {
        while (BanishManager.isSilenced)
        {
            atkText.text = "You are silenced for " + Utilities.GetSummonTime(BanishManager.silenceTimeStamp);
            yield return new WaitForSeconds(1);
        }
    }


    void OnClickAttack()
    {
        Close();
    }


    void OnClickClose()
    {
        Close();
    }

    void Close()
    {
        m_InputRaycaster.enabled = false;
        LeanTween.alphaCanvas(m_CanvasGroup, 0, .4f).setOnComplete(() =>
        {
            m_Canvas.enabled = false;
        });
    }

}