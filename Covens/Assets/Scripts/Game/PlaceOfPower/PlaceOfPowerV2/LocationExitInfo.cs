using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LocationExitInfo : UIInfoPanel
{
    [SerializeField] private Button m_CloseBtn;
    [SerializeField] private Image m_LocationIcon;

    private System.Action m_OnClose;

    private static LocationExitInfo m_Instance;

    public static LocationExitInfo Instance
    {
        get
        {
            if (m_Instance == null)
                m_Instance = Instantiate(Resources.Load<LocationExitInfo>("LocationExitInfo"));
            return m_Instance;

        }
    }

    public static bool isShowing
    {
        get
        {
            if (m_Instance == null) return false;
            else return m_Instance.m_IsShowing;
        }
    }

    protected override void Awake()
    {
        m_Instance = this;
        base.Awake();
        m_CloseBtn.onClick.AddListener(Close);
    }

    public override void Close()
    {
        base.Close();
        m_OnClose?.Invoke();
    }

    public void ShowUI(System.Action onClose)
    {
        m_OnClose = onClose;
        base.Show();
        if (m_LocationIcon.sprite == null)
        {
            m_LocationIcon.sprite = LocationPOPInfo.selectedPopSprite;
            m_LocationIcon.color = Color.white;
        }
        else
        {
            StartCoroutine(DownloadThumb());
        }

        m_CloseBtn.interactable = false;
        LeanTween.value(0, 0, 0).setDelay(2f).setOnComplete(() => m_CloseBtn.interactable = true);
    }

    IEnumerator DownloadThumb()
    {
        m_LocationIcon.color = new Color(1, 1, 1, 0);
        string url = DownloadAssetBundle.baseURL + "pop-circle/" + LocationIslandController.popName + ".png";
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            yield return new WaitForSeconds(1f);
            StartCoroutine(DownloadThumb());
        }
        else
        {
            if (www.isHttpError)
            {
                Debug.LogError($"failed to download \"{url}\"");
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(www);
                if (texture != null)
                {
                    m_LocationIcon.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
                    m_LocationIcon.color = Color.white;
                }
            }
        }
    }

}