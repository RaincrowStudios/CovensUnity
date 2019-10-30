using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UISpiritDiscovered : MonoBehaviour
{
    [SerializeField] private GameObject m_Content;
    [SerializeField] private CanvasGroup m_CanvasGroup;
    [SerializeField] private Button m_CloseButton;
    [SerializeField] protected TextMeshProUGUI m_Title;
    [SerializeField] protected TextMeshProUGUI m_Description;
    [SerializeField] private Animator m_Animator;
    [SerializeField] protected Image m_SpiritArt;
    
    private static UISpiritDiscovered m_Instance;
    public static UISpiritDiscovered Instance
    {
        get
        {
            if (m_Instance == null)
                m_Instance = Instantiate(Resources.Load<UISpiritDiscovered>("UISpiritDiscovered"));
            return m_Instance;
        }
    }

    public static bool IsOpen
    {
        get
        {
            if (m_Instance == null)
                return false;
            else
                return string.IsNullOrEmpty(m_Instance.m_SpiritId) == false;
        }
    }

    private string m_SpiritId = null;
    private int m_TweenId;
    private System.Action m_OnClose;

    private void Awake()
    {
        m_CloseButton.onClick.AddListener(Close);
        m_Content.SetActive(false);
        m_CanvasGroup.alpha = 0;
        m_Animator.enabled = false;
    }

    public void Show(string spiritId, System.Action onClose = null)
    {
        LeanTween.cancel(m_TweenId);

        m_SpiritId = spiritId;
        m_OnClose = onClose;

        BackButtonListener.AddCloseAction(null);
        DownloadedAssets.GetSprite(spiritId, (sprite) =>
        {
            m_TweenId = LeanTween.alphaCanvas(m_CanvasGroup, 0, 0.5f).uniqueId;
            StartCoroutine(ShowCoroutine(spiritId, sprite));
        });
    }

    private void Close()
    {
        BackButtonListener.RemoveCloseAction();

        m_SpiritId = null;
        m_Animator.enabled = false;
        m_CloseButton.interactable = false;
        m_OnClose?.Invoke();
        m_OnClose = null;
        m_Instance = null;

        LeanTween.cancel(m_TweenId);
        LeanTween.scale(m_Content.gameObject, m_Content.transform.localScale * 0.8f, 0.5f).setEaseOutCubic(); ;
        m_TweenId = LeanTween.alphaCanvas(m_CanvasGroup, 0, 0.25f).setOnComplete(() => Destroy(this.gameObject)).uniqueId;
    }

    private IEnumerator ShowCoroutine(string id, Sprite sprite)
    {
        Setup(id, sprite);
        
        m_Animator.enabled = true;
        m_Content.SetActive(true);

        m_CloseButton.interactable = false;

        yield return new WaitForSeconds(0.2f);

        m_CloseButton.interactable = true;

        BackButtonListener.RemoveCloseAction();
        BackButtonListener.AddCloseAction(Close);
    }

    protected virtual void Setup(string id,Sprite sprite)
    {
        SpiritData data = DownloadedAssets.GetSpirit(id);

        m_Title.text = data.Name + " Discovered!";
        m_Description.text = "You now have the knowledge to summon " + data.Name;
        m_SpiritArt.overrideSprite = sprite;
    }
}
