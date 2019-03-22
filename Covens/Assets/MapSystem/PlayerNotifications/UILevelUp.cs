using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UILevelUp : MonoBehaviour
{
    [SerializeField] private GameObject m_Content;
    [SerializeField] private CanvasGroup m_CanvasGroup;
    [SerializeField] private TextMeshProUGUI m_Title;
    [SerializeField] private TextMeshProUGUI m_Description;
    [SerializeField] private Button m_CloseButton;
    [SerializeField] private Animator m_Animator;

    private static UILevelUp m_Instance;
    public static UILevelUp Instance
    {
        get
        {
            if (m_Instance == null)
                m_Instance = Instantiate(Resources.Load<UILevelUp>("UILevelUp"));
            return m_Instance;
        }
    }

    private int m_TweenId;

    private void Awake()
    {
        m_CloseButton.onClick.AddListener(Close);
        m_Content.SetActive(false);
    }

    public void Show()
    {
        m_Title.text = "You Leveled up!";
        m_Description.text = "Level " + PlayerDataManager.playerData.level + "!";

        m_Animator.enabled = true;
        m_Content.SetActive(true);

        m_CloseButton.interactable = false;
        LeanTween.value(0, 0, 0).setDelay(0.1f).setOnStart(() => { m_CloseButton.interactable = true; });
        SoundManagerOneShot.Instance.PlayLevel();
    }

    private void Close()
    {
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
