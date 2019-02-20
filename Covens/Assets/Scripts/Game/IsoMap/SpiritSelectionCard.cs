using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SpiritSelectionCard : MonoBehaviour
{
    [Header("SpiritCard")]
    //public GameObject SpiritCard;
    public Image spiritSprite;
    public TextMeshProUGUI title;
    public TextMeshProUGUI legend;
    public TextMeshProUGUI desc;
    public TextMeshProUGUI tier;
    public TextMeshProUGUI atkButton;
    public GameObject wild;
    public GameObject owned;
    public TextMeshProUGUI ownedBy;
    public TextMeshProUGUI covenBy;
    public TextMeshProUGUI behaviorOwned;
    public TextMeshProUGUI behaviorWild;
    public Button close;

    void Start()
    {
        var cg = GetComponent<CanvasGroup>();
        cg.alpha = 0;
        LeanTween.alphaCanvas(cg, 1, .4f);

        var data = MarkerSpawner.SelectedMarker;

        var sData = DownloadedAssets.spiritDictData[data.id];
        title.text = sData.spiritName;
        string r = "";
        if (DownloadedAssets.spiritDictData[data.id].spiritTier == 1)
        {
            r = "Lesser Spirit";
        }
        else if (DownloadedAssets.spiritDictData[data.id].spiritTier == 2)
        {
            r = "Greater Spirit";
        }
        else if (DownloadedAssets.spiritDictData[data.id].spiritTier == 3)
        {
            r = "Superior Spirit";
        }
        else
        {
            r = "Legendary Spirit";
        }
        tier.text = r;

        if (data.owner == "")
        {
            wild.SetActive(true);
            owned.SetActive(false);
            behaviorWild.text = DownloadedAssets.spiritDictData[data.id].spriitBehavior;
        }
        else
        {
            wild.SetActive(false);
            owned.SetActive(true);
            ownedBy.text = "Summoned By: " + data.owner;
            covenBy.text = (data.covenName == "" ? "Coven: None" : "Coven: " + data.covenName);
            behaviorOwned.text = DownloadedAssets.spiritDictData[data.id].spriitBehavior;
        }

        legend.text = sData.spiritLegend;
        desc.text = sData.spiritDescription;

        DownloadedAssets.GetSprite(data.id, spiritSprite);

        SetSilenced(BanishManager.isSilenced);

        atkButton.GetComponent<Button>().onClick.AddListener(AttackRelay);
        close.onClick.AddListener(Close);
    }

    void SetSilenced(bool isTrue)
    {
        var castingButton = atkButton.GetComponent<Button>();
        if (isTrue)
        {

            atkButton.color = Color.gray;

            castingButton.enabled = false;
            atkButton.text = "You are silenced for " + Utilities.GetSummonTime(BanishManager.silenceTimeStamp);
            StartCoroutine(SilenceUpdateTimer());
        }
        else
        {
            atkButton.color = Color.white;
            castingButton.enabled = true;
            atkButton.text = "Cast";
        }
    }


    IEnumerator SilenceUpdateTimer()
    {
        while (BanishManager.isSilenced)
        {
            atkButton.text = "You are silenced for " + Utilities.GetSummonTime(BanishManager.silenceTimeStamp);
            yield return new WaitForSeconds(1);
        }
    }


    void AttackRelay()
    {
        ShowSelectionCard.Instance.Attack();
        Close();
    }

    void Close()
    {
        LeanTween.alphaCanvas(GetComponent<CanvasGroup>(), 0, .4f).setOnComplete(() => Destroy(gameObject));
    }

}