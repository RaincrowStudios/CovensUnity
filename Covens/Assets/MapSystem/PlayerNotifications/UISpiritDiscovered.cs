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
    [Space()]
    [SerializeField] private RectTransform m_RewardLayout;
    [SerializeField] private TextMeshProUGUI m_SilverReward;
    [SerializeField] private TextMeshProUGUI m_ExpReward;

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
    private System.Action m_OnClose;

    private void Awake()
    {
        m_CloseButton.onClick.AddListener(Close);
        m_Content.SetActive(false);
        m_Content.transform.localScale = Vector3.zero;
        m_CanvasGroup.alpha = 0;
        m_Animator.enabled = false;
    }

    public void Show(string spiritId, System.Action onClose = null)
    {
        m_OnClose = onClose;

        BackButtonListener.AddCloseAction(null);
        DownloadedAssets.GetSprite(spiritId, (sprite) =>
        {
            StartCoroutine(ShowCoroutine(spiritId, sprite));
        });
    }

    private void Close()
    {
        BackButtonListener.RemoveCloseAction();

        m_Animator.enabled = false;
        m_CloseButton.interactable = false;
        m_OnClose?.Invoke();
        m_OnClose = null;

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
        Setup(id, sprite);
        
        m_Animator.enabled = true;
        m_Content.SetActive(true);

        m_CloseButton.interactable = false;

        yield return new WaitForSeconds(0.2f);

        m_CloseButton.interactable = true;

        BackButtonListener.RemoveCloseAction();
        BackButtonListener.AddCloseAction(Close);
    }

    protected virtual void Setup(string id, Sprite sprite)
    {
        SpiritData data = DownloadedAssets.GetSpirit(id);
        long exp = PlayerDataManager.spiritRewardExp[data.tier - 1];
        int silver = PlayerDataManager.spiritRewardSilver[data.tier - 1];

        m_Title.text = data.Name + " Discovered!";
        m_Description.text = "You now have the knowledge to summon " + data.Name;
        m_ExpReward.text = $"+{exp} XP";
        m_SilverReward.text = $"+{silver} {LocalizeLookUp.GetText("store_silver")}";
        m_SpiritArt.overrideSprite = sprite;
    }
}
