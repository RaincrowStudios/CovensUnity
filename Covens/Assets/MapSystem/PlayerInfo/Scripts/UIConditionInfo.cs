using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIConditionInfo : MonoBehaviour
{
    private static UIConditionInfo m_Instance;
    public static UIConditionInfo Instance
    {
        get
        {
            if (m_Instance == null)
                m_Instance = Instantiate(Resources.Load<UIConditionInfo>("UIConditionInfo"));
            return m_Instance;
        }
    }

    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private GraphicRaycaster m_InputRaycaster;

    [SerializeField] private CanvasGroup m_CanvasGroup;
    [SerializeField] private RectTransform m_Panel;

    [SerializeField] private TextMeshProUGUI m_Title;
    [SerializeField] private TextMeshProUGUI m_Description;
    [SerializeField] private Button m_CloseButton;

    private int m_TweenId;
    private RectTransform m_ReferencePosition;

    private void Awake()
    {
        m_Canvas.enabled = false;
        m_InputRaycaster.enabled = false;
        m_CanvasGroup.alpha = 0;
        m_CloseButton.onClick.AddListener(Close);
    }

    public void Show(string conditionId, RectTransform referencePosition)
    {
        ConditionDict condition = DownloadedAssets.GetCondition(conditionId);

        if (condition == null)
        {
            Close();
            return;
        }

        LeanTween.cancel(m_TweenId, true);

        SpellDict spell = DownloadedAssets.GetSpell(condition.spellID);

        m_Title.text = spell.spellName;
        m_Description.text = condition.conditionDescription;
        m_ReferencePosition = referencePosition;

        LayoutRebuilder.ForceRebuildLayoutImmediate(m_Panel);

        m_TweenId = LeanTween.value(0, 1, 0.5f)
            .setEaseOutCubic()
            .setOnStart(() =>
            {
                m_Panel.localScale = new Vector3(1.2f, 1.2f, 1.2f);
                m_CanvasGroup.alpha = 0;

                m_Canvas.enabled = true;
                m_InputRaycaster.enabled = true;

                Update();

                this.enabled = true;
            })
            .setOnUpdate((float t) =>
            {
                m_Panel.localScale = Vector3.one * (t * 1f + 1.2f * (1 - t));
                m_CanvasGroup.alpha = t;
            })
            .uniqueId;        
    }

    private void Close()
    {
        this.enabled = false;
        m_InputRaycaster.enabled = false;

        LeanTween.cancel(m_TweenId, true);
        m_TweenId = LeanTween.value(1, 0, 0.25f)
            .setEaseOutCubic()
            .setOnUpdate((float t) =>
            {
                m_Panel.localScale = Vector3.one * (t * 1f + 1.2f * (1 - t));
                m_CanvasGroup.alpha = t;
            })
            .setOnComplete(() =>
            {
                m_Canvas.enabled = false;
            })
            .uniqueId;
    }

    private void Update()
    {
        if (m_ReferencePosition == null)
        {
            Debug.LogError("null condition item");
            Close();
            return;
        }
        m_Panel.position = m_ReferencePosition.position;
    }
}
