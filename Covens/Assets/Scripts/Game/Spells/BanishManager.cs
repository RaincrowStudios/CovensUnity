using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
public class BanishManager : MonoBehaviour
{
    public static BanishManager Instance { get; set; }
    public static string banishCasterID;

    public CanvasGroup recallButton;
    public GameObject flyButton;
    public GameObject bindLock;
    public Text countDown;
    
    public static double bindTimeStamp;
    public static double silenceTimeStamp;

    public static bool isSilenced;
    public static bool isBind;

    public void Awake()
    {
        Instance = this;
    }

    public void Banish(double lng, double lat, string caster)
    {
        UIPlayerBanished.Show(caster);
        StartCoroutine(BanishHelper(lng, lat));
    }

    IEnumerator BanishHelper(double lng, double lat)
    {
        yield return 1;
        yield return new WaitForSeconds(2);

        //get markers
        MarkerManagerAPI.GetMarkers(
            (float)lng, 
            (float)lat, 
            false, 
            null,
            true,
            false,
            true
        );

        yield return 0;
        yield return new WaitForSeconds(2f);
    }

    public void Bind(WSData data)
    {
        flyButton.SetActive(false);
        bindLock.SetActive(true);
        recallButton.interactable = false;
        ShowBindFX(data);
        PlayerManager.Instance.CancelFlight();
    }

    public void ShowBindFX(WSData data)
    {
        string caster = "";
        if (data.casterType == "witch")
        {
            caster = data.caster;
        }
        else if (data.casterType == "spirit")
        {
            SpiritDict spiritDict = DownloadedAssets.GetSpirit(data.caster);
            if (spiritDict != null)
                caster = spiritDict.spiritName;
        }

        UIPlayerBound.Show(caster);

        this.CancelInvoke();
        Invoke("DisableBind", 3.5f);
    }

    void DisableBind()
    {
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
        isSilenced = true;

        string caster = "";
        if (data.casterType == "witch")
        {
            caster = data.caster;
        }
        else if (data.casterType == "spirit")
        {
            SpiritDict spiritDict = DownloadedAssets.GetSpirit(data.caster);
            if (spiritDict != null)
                caster = spiritDict.spiritName;
        }

        UIPlayerSilenced.Show(caster);
    }

    public void unSilenced()
    {
        isSilenced = false;
        PlayerNotificationManager.Instance.ShowNotification("You have been unsilenced. You are now able to cast spells.", PlayerNotificationManager.Instance.spellBookIcon);
    }
}

