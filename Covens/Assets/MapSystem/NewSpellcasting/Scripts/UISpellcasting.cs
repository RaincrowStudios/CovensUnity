using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Raincrow.Maps;

public class UISpellcasting : MonoBehaviour
{
    private class SpellGroup
    {
        public SpellData baseSpell;
        public List<SpellData> signatures;
    }

    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private GraphicRaycaster m_InputRaycaster;
    [SerializeField] private RectTransform m_MainPanel;
    [SerializeField] private CanvasGroup m_CanvasGroup;
    [SerializeField] private Button m_CloseButton;

    [Header("School selection")]
    [SerializeField] private CanvasGroup m_SchoolPanel;
    [SerializeField] private Button m_ShadowButton;
    [SerializeField] private Button m_GreyButton;
    [SerializeField] private Button m_LightButton;

    [Header("Spell selection")]
    [SerializeField] private CanvasGroup m_SpellPanel;
    [SerializeField] private Button m_SpellEntryPrefab;
    [SerializeField] private Transform m_SpellContainer;
    [SerializeField] private Button m_BackButton;

    public static UISpellcasting Instance { get; private set; }

    private List<SpellData> m_Spells;
    private IMarker m_Target;

    private int m_TweenId;
    private int m_SpellTweenId;
    private int m_SchoolTweenId;

    private Dictionary<string, SpellGroup> m_SignatureDictionary;

    private void Awake()
    {
        Instance = this;

        //setup initial state
        m_MainPanel.anchoredPosition = new Vector2(m_MainPanel.sizeDelta.x, 0);
        m_CanvasGroup.alpha = 0;
        m_InputRaycaster.enabled = false;
        m_Canvas.enabled = false;
        m_SchoolPanel.alpha = 0;
        m_SpellPanel.alpha = 0;
        m_SchoolPanel.gameObject.SetActive(false);
        m_SpellPanel.gameObject.SetActive(false);
        m_SpellEntryPrefab.gameObject.SetActive(false);
        m_SpellEntryPrefab.transform.SetParent(this.transform);

        //setup buttons
        m_CloseButton.onClick.AddListener(Close);
        m_BackButton.onClick.AddListener(() =>
        {
            HideSpellSelection();
            ShowSchoolSelection();
        });
        m_ShadowButton.onClick.AddListener(() => 
        {
            HideSchoolSelection();
            ShowSpellSelection(-1);
        });
        m_GreyButton.onClick.AddListener(() =>
        {
            HideSchoolSelection();
            ShowSpellSelection(0);
        });
        m_LightButton.onClick.AddListener(() =>
        {
            HideSchoolSelection();
            ShowSpellSelection(1);
        });
    }

    public void Show(IMarker target, List<SpellData> spells)
    {
        LeanTween.cancel(m_TweenId);
        LeanTween.cancel(m_SchoolTweenId);
        LeanTween.cancel(m_SpellTweenId);

        m_Target = target;
        m_Spells = spells;

        ShowSchoolSelection();

        m_Canvas.enabled = true;
        m_InputRaycaster.enabled = true;
        m_TweenId = LeanTween.value(0, 1, 0.5f)
           .setOnUpdate((float t) =>
           {
               m_MainPanel.anchoredPosition = new Vector2((1 - t) * m_MainPanel.sizeDelta.x, 0);
               m_CanvasGroup.alpha = t;
           })
           .setEaseOutCubic()
           .uniqueId;
    }

    public void Close()
    {
        m_InputRaycaster.enabled = false;
        m_TweenId = LeanTween.value(0, 1, 0.5f)
           .setOnUpdate((float t) =>
           {
               m_MainPanel.anchoredPosition = new Vector2(t * m_MainPanel.sizeDelta.x, 0);
               m_CanvasGroup.alpha = 1 - t;
           })
           .setOnComplete(() =>
           {
               m_Canvas.enabled = false;
           })
           .setEaseOutCubic()
           .uniqueId;
    }


    private void ShowSchoolSelection()
    {
        m_SchoolPanel.gameObject.SetActive(true);
        m_SchoolTweenId = LeanTween.value(m_SchoolPanel.alpha, 1, 0.25f)
            .setEaseOutCubic()
            .setOnUpdate((float t) =>
            {
                m_SchoolPanel.alpha = t;
            })
            .uniqueId;
    }
    private void HideSchoolSelection()
    {
        m_SchoolTweenId = LeanTween.value(m_SchoolPanel.alpha, 0, 0.25f)
            .setEaseOutCubic()
            .setOnUpdate((float t) =>
            {
                m_SchoolPanel.alpha = t;
            })
            .setOnComplete(() =>
            {
                m_SchoolPanel.gameObject.SetActive(false);
            })
            .uniqueId;
    }

    public void ShowSpellSelection(int school)
    {
        //setup spells
        m_SignatureDictionary = new Dictionary<string, SpellGroup>();
        for(int i = 0; i < m_Spells.Count; i++)
        {
            if (m_Spells[i].school == school)
            {
                SpellGroup group;
                if (m_SignatureDictionary.ContainsKey(m_Spells[i].baseSpell))
                {
                    group = m_SignatureDictionary[m_Spells[i].baseSpell];
                }
                else
                {
                    group = new SpellGroup
                    {
                        baseSpell = null,
                        signatures = new List<SpellData>()
                    };
                    m_SignatureDictionary.Add(m_Spells[i].baseSpell, group);
                }

                if (m_Spells[i].id == m_Spells[i].baseSpell)
                    group.baseSpell = m_Spells[i];
                else
                    group.signatures.Add(m_Spells[i]);
            }
        }

        int j = 0;
        foreach(SpellGroup group in m_SignatureDictionary.Values)
        {
            //Button m_
        }

        m_SpellPanel.gameObject.SetActive(true);
        m_SpellTweenId = LeanTween.value(m_SpellPanel.alpha, 1, 0.25f)
            .setEaseOutCubic()
            .setOnUpdate((float t) =>
            {
                m_SpellPanel.alpha = t;
            })
            .uniqueId;
    }
    private void HideSpellSelection()
    {
        m_SpellTweenId = LeanTween.value(m_SpellPanel.alpha, 0, 0.25f)
            .setEaseOutCubic()
            .setOnUpdate((float t) =>
            {
                m_SpellPanel.alpha = t;
            })
            .setOnComplete(() =>
            {
                m_SpellPanel.gameObject.SetActive(false);
            })
            .uniqueId;
    }
}
