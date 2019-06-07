using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
public class BanishManager : MonoBehaviour
{
    public static BanishManager Instance { get; set; }

    public CanvasGroup recallButton;
    public GameObject flyButton;
    public GameObject bindLock;
    public Text countDown;
    
    public static double bindTimeStamp { get; private set; }
    public static double silenceTimeStamp { get; private set; }

    public static bool isSilenced { get; private set; }
    public static bool isBind { get; private set; }

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

    public void Bind(Conditions condition)
    {
        isBind = true;
        bindTimeStamp = condition.expiresOn;

        PlayerManager.Instance.CancelFlight();

        flyButton.SetActive(false);
        bindLock.SetActive(true);
        recallButton.interactable = false;
    }

    public void ShowBindScreen(WSData data)
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
    }

    public void Unbind()
    {
        isBind = false;

        recallButton.interactable = true;
        flyButton.SetActive(true);
        bindLock.SetActive(false);
        PlayerNotificationManager.Instance.ShowNotification("You are no longer bound. You are now able to fly.", PlayerNotificationManager.Instance.spellBookIcon);
    }

    public void Silenced(WSData data)
    {
        isSilenced = true;

        if (UISpellcasting.isOpen)
            UISpellcasting.Instance.UpdateCanCast();

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

