using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.Networking;

public class witchSchoolData : MonoBehaviour
{
    [SerializeField] private CanvasGroup m_CanvasGroup;
    [SerializeField] private Image m_Thumbnail;
    [SerializeField] private TextMeshProUGUI m_VideoTitle;
    [SerializeField] private TextMeshProUGUI m_VideoDesc;
    [SerializeField] private Button m_ThumbButton;

    private string m_VideoId;
    private System.Action m_OnClick;

    private void Awake()
    {
        m_ThumbButton.onClick.AddListener(OnClickPlayVideo);
        m_Thumbnail.color = Color.black;
        //m_CanvasGroup.alpha = 0;
    }

    public void Setup(string id, System.Action onClick)
    {
        this.m_VideoId = id;
        this.m_OnClick = onClick;

        m_VideoTitle.text = LocalizeLookUp.GetText(id + "_title").ToUpper();
        m_VideoDesc.text = LocalizeLookUp.GetText(id + "_desc");

        StopCoroutine("DownloadThumb");
        StartCoroutine(DownloadThumb(id));

        //LeanTween.alphaCanvas(m_CanvasGroup, 1f, 1f).setDelay(0.1f).setEaseOutCubic();
    }

    IEnumerator DownloadThumb(string id)
    {
        m_Thumbnail.color = Color.black;
        string url = DownloadAssetBundle.baseURL + "witch-school-new/thumbs/" + id + ".png";
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            yield return new WaitForSeconds(1f);
            StartCoroutine(DownloadThumb(id));
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
                    m_Thumbnail.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
                    LeanTween.value(0, 1, 1f)
                        .setEaseOutCubic()
                        .setOnUpdate((float t) =>
                        {
                            m_Thumbnail.color = Color.Lerp(Color.black, Color.white, t);
                        });
                }
            }
        }
    }

    void OnClickPlayVideo()
    {
        m_OnClick?.Invoke();
    }

}

