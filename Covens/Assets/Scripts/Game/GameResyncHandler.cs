using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameResyncHandler : MonoBehaviour
{
    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private GraphicRaycaster m_InputRaycaster;
    [SerializeField] private CanvasGroup m_CanvasGroup;

    [SerializeField] private Image m_SpiritArt;
    [SerializeField] private Text m_SpiritName;
    [SerializeField] private Text m_Subtitle;

    public static event System.Action OnResyncStart;
    public static event System.Action OnResyncFinish;

    public static bool IsBackground { get; private set; }
    public static GameResyncHandler Instance { get; private set; }
    public static bool IsResyncing { get; private set; }

    private const float MAX_BG_TIME = 300;
    private float m_LostFocusTime;
    private int m_MainTweenId;
    private int m_SpiritTweenId;
    
    private void Awake()
    {
        Instance = this;

        m_Canvas.enabled = false;
        m_InputRaycaster.enabled = false;
        m_CanvasGroup.alpha = 0;
    }
    
    //private void OnApplicationFocus(bool hasFocus)
    //{
    //    IsBackground = !hasFocus;

    //    if (hasFocus)
    //    {
    //        float timeInBg = Time.unscaledTime - m_LostFocusTime;
    //        Debug.Log("Game was in background for " + timeInBg.ToString() + " seconds");

    //        if (timeInBg > MAX_BG_TIME)
    //        {
    //            ResyncGame();
    //        }
    //    }
    //    else
    //    {
    //        m_LostFocusTime = Time.unscaledTime;
    //    }
    //}

    private void ShowScreen(string title)
    {
        LeanTween.cancel(m_MainTweenId);
        LeanTween.cancel(m_SpiritTweenId);

        //hide
        m_SpiritName.text = "";
        m_SpiritArt.overrideSprite = null;
        m_SpiritArt.color = new Color(m_SpiritArt.color.r, m_SpiritArt.color.g, m_SpiritArt.color.b, 0);

        //load new spirit
        SpiritData spirit = DownloadedAssets.spiritDict.ElementAt(UnityEngine.Random.Range(0, DownloadedAssets.spiritDict.Count)).Value;
        m_SpiritName.text = spirit.Name;
        DownloadedAssets.GetSprite(spirit.id, (spr) =>
        {
            m_SpiritArt.overrideSprite = spr;
            m_SpiritTweenId = LeanTween.alpha(m_SpiritArt.rectTransform, 1f, 1f).uniqueId;
        });

        //fade the screen
        m_Canvas.enabled = true;
        m_InputRaycaster.enabled = true;
        m_MainTweenId = LeanTween.alphaCanvas(m_CanvasGroup, 1f, .5f).setEaseOutCubic().uniqueId;

        m_Subtitle.text = title;// "Syncing with server . . .";
    }

    private void HideScreen()
    {
        m_InputRaycaster.enabled = false;

        LeanTween.cancel(m_MainTweenId);
        m_MainTweenId = LeanTween.alphaCanvas(m_CanvasGroup, 0f, 1f)
            .setEaseOutCubic()
            .setOnComplete(() => m_Canvas.enabled = false)
            .uniqueId;
    }


    public static void ResyncGame()
    {
#if LOCAL_API
        return;
#endif
        Debug.Log("Resync game");
        Instance.ShowScreen(LocalizeLookUp.GetText("server_syncing"));
        Instance.StartCoroutine(Instance.ResyncCoroutine());
    }

    private IEnumerator ResyncCoroutine()
    {
        if (IsResyncing)
            yield break;

        IsResyncing = true;

        yield return new WaitForSeconds(1);

        //wait reconection if internet not reachable
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            ShowScreen(LocalizeLookUp.GetText("server_connect"));

            while (Application.internetReachability == NetworkReachability.NotReachable)
                yield return new WaitForSeconds(0.1f);
        }

        Debug.Log("Resync started");
        OnResyncStart?.Invoke();
        
        Debug.Log("clearing coven data");
        TeamManager.MyCovenData = null;

        Debug.Log("updating player data");
        LoginAPIManager.GetCharacter((result, response) =>
        {
            Debug.Log("retrieving nearby markers");
            MarkerManagerAPI.GetMarkers(
                PlayerDataManager.playerData.longitude,
                PlayerDataManager.playerData.latitude,
                () =>
                {
                    OnResyncFinish?.Invoke();
                    Debug.Log("Resync finished");

                    //reset timer
                    m_LostFocusTime = Time.unscaledTime;

                    //close screen
                    IsResyncing = false;
                    HideScreen();
                },
                true,
                false);
        });
    }
}
