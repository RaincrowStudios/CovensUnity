using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LocationSelectionCard : MonoBehaviour
{
    [Header("Location")]
    public TextMeshProUGUI locationTitle;
    public TextMeshProUGUI locOwnedBy;
    public TextMeshProUGUI defendedBy;
    public TextMeshProUGUI timeToTreasure;
    public TextMeshProUGUI ExitLocation;
    public Button EnterLocation;
    TextMeshProUGUI EnterLocationText;
    public Button close;

    public void Start()
    {
        var cg = GetComponent<CanvasGroup>();
        cg.alpha = 0;
        LeanTween.alphaCanvas(cg, 1, .4f);
        EnterLocationText = EnterLocation.GetComponent<TextMeshProUGUI>();

        locationTitle.text = string.Empty;
        locOwnedBy.text = string.Empty;
        defendedBy.text = string.Empty;
        timeToTreasure.text = string.Empty;

        EnterLocation.interactable = false;
        EnterLocationText.text = "Loading...";

        close.interactable = true;
        close.onClick.AddListener(Close);
        ExitLocation.text = "Close";
    }

    public void SetupDetails(MarkerDataDetail markerDetail)
    {
        locationTitle.text = markerDetail.displayName;
        //Debug.Log(markerDetail.controlledBy);

        if (!string.IsNullOrEmpty(markerDetail.controlledBy))
        {
            if (markerDetail.isCoven)
            {
                locOwnedBy.text = string.Concat("This Place of Power is owned by the coven <color=#ffffff> ", markerDetail.controlledBy, "</color>.");
            }
            else
            {
                locOwnedBy.text = string.Concat("This Place of Power is owned by <color=#ffffff> ", markerDetail.controlledBy, "</color>.");
            }

            if (markerDetail.spiritCount == 1)
            {
                defendedBy.text = string.Concat("It is defended by ", markerDetail.spiritCount.ToString(), " spirit.");
            }
            else
            {
                defendedBy.text = string.Concat("It is defended by ", markerDetail.spiritCount.ToString(), " spirits.");
            }
        }
        else
        {
            locOwnedBy.text = "This Place of Power is unclaimed.";
            defendedBy.text = "You can own this Place of Power by summoning a spirit inside it.";
        }
        timeToTreasure.text = GetTime(markerDetail.rewardOn) + "until this Place of Power yields treasure.";
        if (markerDetail.full)
        {
            EnterLocationText.text = "Place of power is full.";
        }
        else
        {
            EnterLocation.interactable = true;
            EnterLocationText.text = "Enter the Place of Power";
            ExitLocation.text = "Not Today";
            EnterLocation.onClick.AddListener(AttackRelay);
        }

    }

    void SetSilenced(bool isTrue)
    {
        if (isTrue)
        {

            EnterLocationText.color = Color.gray;

            EnterLocation.enabled = false;
            EnterLocationText.text = "You are silenced for " + Utilities.GetSummonTime(BanishManager.silenceTimeStamp);
            StartCoroutine(SilenceUpdateTimer());
        }
        else
        {
            EnterLocationText.color = Color.white;
            EnterLocation.enabled = true;
            EnterLocationText.text = "Cast";
        }
    }


    IEnumerator SilenceUpdateTimer()
    {
        while (BanishManager.isSilenced)
        {
            EnterLocationText.text = "You are silenced for " + Utilities.GetSummonTime(BanishManager.silenceTimeStamp);
            yield return new WaitForSeconds(1);
        }
    }

    string GetTime(double javaTimeStamp)
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