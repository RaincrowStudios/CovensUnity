using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PortalSelectionCard : MonoBehaviour
{


    [Header("PortalCard")]
    public GameObject[] portalType;
    public TextMeshProUGUI creator;
    public TextMeshProUGUI portalTitle;
    public TextMeshProUGUI portalEnergy;
    public TextMeshProUGUI summonsIn;
    public Button castButton;
    public TextMeshProUGUI castingText;
    public Button closeButton;
    //public TextMeshProUGUI castText;

    void Start()
    {
        var cg = GetComponent<CanvasGroup>();
        cg.alpha = 0;
        LeanTween.alphaCanvas(cg, 1, .4f);
        var data = MarkerSpawner.SelectedMarker;

        portalTitle.text = "Portal";
        foreach (var item in portalType)
        {
            item.SetActive(false);
        }
        if (data.degree > 0)
        {
            portalType[0].SetActive(true);
        }
        else if (data.degree == 0)
        {
            portalType[1].SetActive(true);
        }
        else
        {
            portalType[2].SetActive(true);
        }

        creator.text = data.owner;
        portalEnergy.text = "Energy : " + data.energy.ToString();
        summonsIn.text = "Summon in " + Utilities.EpocToDateTime(data.summonOn);
        SetSilenced(BanishManager.isSilenced);
        castButton.onClick.AddListener(AttackRelay);
        closeButton.onClick.AddListener(Close);
    }

    void SetSilenced(bool isTrue)
    {
        Button castingButton = castButton.GetComponent<Button>();
        if (isTrue)
        {

            castingText.color = Color.gray;

            castingButton.enabled = false;
            castingText.text = "You are silenced for " + Utilities.GetSummonTime(BanishManager.silenceTimeStamp);
            StartCoroutine(SilenceUpdateTimer());
        }
        else
        {
            castingText.color = Color.white;
            castingButton.enabled = true;
            castingText.text = "Cast";
        }
    }

    IEnumerator SilenceUpdateTimer()
    {
        while (BanishManager.isSilenced)
        {
            castingText.text = "You are silenced for " + Utilities.GetSummonTime(BanishManager.silenceTimeStamp);
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
        Debug.Log("closing");
        LeanTween.alphaCanvas(GetComponent<CanvasGroup>(), 0, .4f).setOnComplete(() => Destroy(gameObject));
    }
}