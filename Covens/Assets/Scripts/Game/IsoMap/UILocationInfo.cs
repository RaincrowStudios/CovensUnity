using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILocationInfo : UIInfoPanel
{
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI locationTitleText;
    [SerializeField] private TextMeshProUGUI locOwnedByText;
    [SerializeField] private TextMeshProUGUI defendedByText;
    [SerializeField] private TextMeshProUGUI timeToTreasureText;    

    [Header("Button")]
    [SerializeField] private Button enterLocationButton;
    [SerializeField] private Button exitLocationButton;

    private TextMeshProUGUI enterLocationText;
    private TextMeshProUGUI exitLocationText;
    private float previousMapZoom;
    private IMarker locationMarker;

    protected override void ReOpenAnimation()
    {
        LeanTween.alphaCanvas(m_CanvasGroup, 1, 0.4f);
    }

    public void Show(IMarker marker)
    {
        if (IsShowing)
        {
            return;
        }

        locationMarker = marker;

        locationTitleText.text = string.Empty;
        locOwnedByText.text = string.Empty;
        defendedByText.text = string.Empty;
        timeToTreasureText.text = string.Empty;

        enterLocationButton.interactable = false;
        enterLocationText = enterLocationButton.GetComponentInChildren<TextMeshProUGUI>();
        enterLocationText.text = "Loading...";

        exitLocationButton.interactable = true;
        exitLocationButton.onClick.AddListener(Close);
        exitLocationText = exitLocationButton.GetComponentInChildren<TextMeshProUGUI>();
        exitLocationText.text = "Close";

        MapsAPI.Instance.allowControl = false;
        MapCameraUtils.FocusOnTargetCenter(locationMarker);

        previousMapPosition = MapsAPI.Instance.GetWorldPosition();
        previousMapZoom = MapsAPI.Instance.normalizedZoom;

        MarkerSpawner.HighlightMarker(new List<IMarker> { PlayerManager.marker, locationMarker }, true);

        Show();
    }

    public void SetupDetails(MarkerDataDetail markerDetail)
    {
        locationTitleText.text = markerDetail.displayName;

        if (!string.IsNullOrEmpty(markerDetail.controlledBy))
        {
            if (markerDetail.isCoven)
            {
                locOwnedByText.text = string.Concat("This Place of Power is owned by the coven <color=#ffffff> ", markerDetail.controlledBy, "</color>.");
            }
            else
            {
                locOwnedByText.text = string.Concat("This Place of Power is owned by <color=#ffffff> ", markerDetail.controlledBy, "</color>.");
            }

            if (markerDetail.spiritCount == 1)
            {
                defendedByText.text = string.Concat("It is defended by ", markerDetail.spiritCount.ToString(), " spirit.");
            }
            else
            {
                defendedByText.text = string.Concat("It is defended by ", markerDetail.spiritCount.ToString(), " spirits.");
            }
        }
        else
        {
            locOwnedByText.text = "This Place of Power is unclaimed.";
            defendedByText.text = "You can own this Place of Power by summoning a spirit inside it.";
        }
        timeToTreasureText.text = GetTime(markerDetail.rewardOn) + "until this Place of Power yields treasure.";
        if (markerDetail.full)
        {
            enterLocationText.text = "Place of power is full.";
        }
        else
        {
            enterLocationButton.interactable = true;
            enterLocationText.text = "Enter the Place of Power";
            enterLocationButton.onClick.AddListener(AttackRelay);

            exitLocationText.text = "Not Today";            
        }

    }

    void SetSilenced(bool isTrue)
    {
        if (isTrue)
        {

            enterLocationText.color = Color.gray;

            enterLocationButton.enabled = false;
            enterLocationText.text = "You are silenced for " + Utilities.GetSummonTime(BanishManager.silenceTimeStamp);
            StartCoroutine(SilenceUpdateTimer());
        }
        else
        {
            enterLocationText.color = Color.white;
            enterLocationButton.enabled = true;
            enterLocationText.text = "Cast";
        }
    }


    IEnumerator SilenceUpdateTimer()
    {
        while (BanishManager.isSilenced)
        {
            enterLocationText.text = "You are silenced for " + Utilities.GetSummonTime(BanishManager.silenceTimeStamp);
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

    public override void Hide()
    {
        //base.Hide();
        enterLocationButton.interactable = false;
        exitLocationButton.interactable = false;

        MarkerSpawner.HighlightMarker(new List<IMarker> { PlayerManager.marker, locationMarker }, false);
        MapCameraUtils.FocusOnPosition(previousMapPosition, previousMapZoom, true);
        LeanTween.alphaCanvas(m_CanvasGroup, 0, .4f).setOnComplete(() => Destroy(gameObject));
    }    
}
