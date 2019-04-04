using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
public class BanishManager : MonoBehaviour
{
    public static BanishManager Instance { get; set; }
    public GameObject banishObject;
    public TextMeshProUGUI banishInfoText;
    public static string banishCasterID;

    public GameObject bindObject;
    public CanvasGroup recallButton;
    public TextMeshProUGUI bindInfoText;
    public GameObject flyButton;
    public GameObject bindLock;
    public Text countDown;

    public GameObject silencedObject;
    public TextMeshProUGUI silencedInfo;

    public static double bindTimeStamp;
    public static double silenceTimeStamp;

    public static bool isSilenced;
    public static bool isBind;

    public void Awake()
    {
        Instance = this;
    }

    public void Banish(double lng, double lat)
    {
        banishInfoText.text = "You have been banished by " + banishCasterID;
        banishObject.SetActive(true);
        StartCoroutine(BanishHelper(lng, lat));
    }

    IEnumerator BanishHelper(double lng, double lat)
    {
        bool getMarkerResponse = false;

        //get markers
        MarkerManagerAPI.GetMarkers((float)lng, (float)lat, false, () => 
        {
            //load/move the map
            //MapsAPI.Instance.ShowStreetMap(lng, lat, null, true);
            getMarkerResponse = true;
        });

        while (!getMarkerResponse)
            yield return 1;

        yield return new WaitForSeconds(2f);

        banishObject.SetActive(false);
    }

    public void Bind(WSData data)
    {
        flyButton.SetActive(false);
        bindLock.SetActive(true);
        recallButton.interactable = false;
        if (MapSelection.currentView == CurrentView.MapView)
        {
            ShowBindFX(data);
        }
        PlayerManager.Instance.CancelFlight();
    }

    public void ShowBindFX(WSData data)
    {
        bindObject.SetActive(true);
        if (data.casterType == "witch")
        {
            bindInfoText.text = "You have been bound by " + data.caster;
        }
        else if (data.casterType == "spirit")
        {
            bindInfoText.text = "You have been bound by " + DownloadedAssets.spiritDictData[data.caster].spiritName;
        }
        this.CancelInvoke();
        Invoke("DisableBind", 3.5f);
    }

    void DisableBind()
    {
        bindObject.SetActive(false);
        recallButton.interactable = true;

    }

    public void Unbind()
    {
        flyButton.SetActive(true);
        bindLock.SetActive(false);
        PlayerNotificationManager.Instance.ShowNotification("You are no longer bound. You are now able to fly.", PlayerNotificationManager.Instance.spellBookIcon);
    }

    public void Silenced(WSData data)
    {
        if (silencedObject.activeInHierarchy)
        {
            silencedObject.SetActive(false);
        }
        isSilenced = true;

        //ShowSelectionCard.Instance.SetSilenced (true);
        if (data.casterType == "witch")
        {
            silencedInfo.text = "You have been silenced by " + data.caster;
        }
        else if (data.casterType == "spirit")
        {
            silencedInfo.text = "You have been silenced by " + DownloadedAssets.spiritDictData[data.caster].spiritName;
        }

        silencedObject.SetActive(true);
    }

    public void unSilenced()
    {
        Debug.Log("Not Silenced");
        isSilenced = false;
        PlayerNotificationManager.Instance.ShowNotification("You have been unsilenced. You are now able to cast spells.", PlayerNotificationManager.Instance.spellBookIcon);
        //ShowSelectionCard.Instance.SetSilenced (false);
        silencedObject.SetActive(false);
    }
}

