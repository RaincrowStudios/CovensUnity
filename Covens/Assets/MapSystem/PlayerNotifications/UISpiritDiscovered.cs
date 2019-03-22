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
    [SerializeField] private TextMeshProUGUI m_Title;
    [SerializeField] private TextMeshProUGUI m_Description;
    [SerializeField] private Animator m_Animator;
    [SerializeField] private Image m_SpiritArt;

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

    private int m_TweenId;

    private void Awake()
    {
        m_CloseButton.onClick.AddListener(Close);
        m_Content.SetActive(false);
        m_Content.transform.localScale = Vector3.zero;
        m_CanvasGroup.alpha = 0;
        m_Animator.enabled = false;
    }

    public void Show(string spiritId)
    {
        DownloadedAssets.GetSprite(spiritId, (sprite) =>
        {
            StartCoroutine(ShowCoroutine(spiritId, sprite));
        });
    }

    private void Close()
    {
        m_Animator.enabled = false;
        m_CloseButton.interactable = false;

        LeanTween.cancel(m_TweenId);
        m_TweenId = LeanTween.value(1, 0, 0.4f)
            .setEaseOutCubic()
            .setOnUpdate((float t) =>
            {
                m_Content.transform.localScale = new Vector3(t, t, t);
                m_CanvasGroup.alpha = t;
            })
            .setOnComplete(() => 
            {
                Destroy(this.gameObject);
            })
            .uniqueId;
    }

    private IEnumerator ShowCoroutine(string id, Sprite sprite)
    {
        //wait for the spirit banished ui to close
        while (UISpiritBanished.IsOpen)
            yield return 1;
                
        SpiritDict spiritData = DownloadedAssets.GetSpirit(id);

        if (spiritData != null)
        {
            m_Title.text = spiritData.spiritName + " Discovered!";
            m_Description.text = "You now have the knowledge to summon " + spiritData.spiritName;
        }

        m_SpiritArt.sprite = sprite;
        
        m_Animator.enabled = true;
        m_Content.SetActive(true);

        m_CloseButton.interactable = false;
        yield return new WaitForSeconds(0.1f);
        m_CloseButton.interactable = true;
    }
}
