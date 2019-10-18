using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDegreeChanged : MonoBehaviour
{
    [SerializeField] private GameObject m_Content;
    [SerializeField] private CanvasGroup m_CanvasGroup;
    [SerializeField] private TextMeshProUGUI m_Title;
    [SerializeField] private TextMeshProUGUI m_Description;
    [SerializeField] private Button m_CloseButton;
    [SerializeField] private Animator m_Animator;

    private static UIDegreeChanged m_Instance;
    public static UIDegreeChanged Instance
    {
        get
        {
            if (m_Instance == null)
                m_Instance = Instantiate(Resources.Load<UIDegreeChanged>("UIDegreeChanged"));
            return m_Instance;
        }
    }

    private int m_TweenId;

    private void Awake()
    {
        m_CloseButton.onClick.AddListener(Close);
        m_Content.SetActive(false);
    }

    public void Show(int oldDegree, int newDegree)
    {
        m_Title.text = "Your Alignment Changed!";
        m_Description.text = Utilities.WitchTypeControlSmallCaps(PlayerDataManager.playerData.degree);

        m_Animator.enabled = true;
        m_Content.SetActive(true);

        m_CloseButton.interactable = false;
        LeanTween.value(0, 0, 0).setDelay(0.1f).setOnStart(() => { m_CloseButton.interactable = true; });

        if (oldDegree < newDegree)
            SoundManagerOneShot.Instance.PlayWhite();
        else
            SoundManagerOneShot.Instance.PlayShadow();

        BackButtonListener.AddCloseAction(Close);
    }

    private void Close()
    {
        BackButtonListener.RemoveCloseAction();

        m_Animator.enabled = false;
        m_CloseButton.interactable = false;

        LeanTween.cancel(m_TweenId);
        m_TweenId = LeanTween.value(m_CanvasGroup.alpha, 0, 0.4f)
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
}
