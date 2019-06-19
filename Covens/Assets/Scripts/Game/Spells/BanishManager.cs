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

    public static event System.Action OnBanished;

    public void Awake()
    {
        Instance = this;
    }

    public void Banish(double lng, double lat, string caster)
    {
        StartCoroutine(BanishHelper(caster, lng, lat));
    }

    IEnumerator BanishHelper(string caster, double lng, double lat)
    {
        if (PlaceOfPower.IsInsideLocation)
        {
            //dont send the leave request (server already removed the player from the pop)
            PlaceOfPower.LeavePoP(false);
            yield return new WaitForSeconds(1f);

            OnBanished?.Invoke();
            UIPlayerBanished.Show(caster);
            yield return 1;
            yield return new WaitForSeconds(2f);

            //load the map at the new position
            MapsAPI.Instance.InitMap(lng, lat, MapsAPI.Instance.normalizedZoom, null, true);
        }
        else
        {
            OnBanished?.Invoke();
            UIPlayerBanished.Show(caster);

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
        }

        yield return 0;
        yield return new WaitForSeconds(2f);
    }

    public void Bind(Conditions condition)
    {
        isBind = true;
        bindTimeStamp = condition.expiresOn;
        
        flyButton.SetActive(false);
        bindLock.SetActive(true);
        recallButton.interactable = false;

        StartCoroutine(BindHelper());
    }

    private IEnumerator BindHelper()
    {
        PlayerManager.Instance.CancelFlight();
        yield return new WaitForSeconds(1f);
        yield return 0;

        while (isBind)
        {
            yield return 0;
        }
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

