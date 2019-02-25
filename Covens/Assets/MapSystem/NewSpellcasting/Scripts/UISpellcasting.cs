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
    [SerializeField] private UISpellcastingItem m_SpellEntryPrefab;
    [SerializeField] private Transform m_SpellContainer;
    [SerializeField] private Button m_BackButton;
    [SerializeField] private UISpellcastingInfo m_SpellInfo;

    private static UISpellcasting m_Instance;
    public static UISpellcasting Instance
    {
        get
        {
            if (m_Instance == null)
                m_Instance = Instantiate(Resources.Load<UISpellcasting>("UISpellcasting"));
            return m_Instance;
        }
    }

    private List<UISpellcastingItem> m_SpellButtons = new List<UISpellcastingItem>();
    private List<SpellData> m_Spells;
    private IMarker m_Target;

    private int m_TweenId;
    private int m_SpellTweenId;
    private int m_SchoolTweenId;

    private Dictionary<string, SpellGroup> m_SignatureDictionary;

    private void Awake()
    {
        m_Instance = this;

        //setup initial state
        m_MainPanel.anchoredPosition = new Vector2(m_MainPanel.sizeDelta.x, 0);
        m_CanvasGroup.alpha = 0;
        EnableCanvas(false);
        m_SchoolPanel.alpha = 0;
        m_SpellPanel.alpha = 0;
        m_SchoolPanel.gameObject.SetActive(false);
        m_SpellPanel.gameObject.SetActive(false);
        m_SpellEntryPrefab.gameObject.SetActive(false);
        m_SpellEntryPrefab.transform.SetParent(this.transform);

        //setup buttons
        m_CloseButton.onClick.AddListener(OnClickClose);
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

        m_SpellInfo.onConfirmSpellcast += OnConfirmSpellcast;
    }

    public void EnableCanvas(bool enable)
    {
        m_Canvas.enabled = enable;
        m_InputRaycaster.enabled = enable;
    }

    public void Show(IMarker target, List<SpellData> spells)
    {
        LeanTween.cancel(m_TweenId);
        LeanTween.cancel(m_SchoolTweenId);
        LeanTween.cancel(m_SpellTweenId);

        m_Target = target;
        m_Spells = spells;

        ShowSchoolSelection();
        HideSpellSelection();
        m_SpellInfo.Hide();

        ReOpen();
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
               EnableCanvas(false);
           })
           .setEaseOutCubic()
           .uniqueId;
    }
    
    public void ReOpen()
    {
        EnableCanvas(true);
        m_TweenId = LeanTween.value(0, 1, 0.5f)
           .setOnUpdate((float t) =>
           {
               m_MainPanel.anchoredPosition = new Vector2((1 - t) * m_MainPanel.sizeDelta.x, 0);
               m_CanvasGroup.alpha = t;
           })
           .setEaseOutCubic()
           .uniqueId;
    }

    private void ShowSchoolSelection()
    {
        LeanTween.cancel(m_SchoolTweenId);
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
        LeanTween.cancel(m_SchoolTweenId);
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
        LeanTween.cancel(m_SpellTweenId);
        //setup spells
        int i;
        m_SignatureDictionary = new Dictionary<string, SpellGroup>();
        for(i = 0; i < m_Spells.Count; i++)
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

        i = 0;
        foreach(SpellGroup group in m_SignatureDictionary.Values)
        {
            UISpellcastingItem item;
            if (i >= m_SpellButtons.Count)
                m_SpellButtons.Add(Instantiate(m_SpellEntryPrefab, m_SpellContainer));
            item = m_SpellButtons[i];

            if (group.baseSpell != null)
                item.Setup(group.baseSpell, group.baseSpell, group.signatures, OnSelectSpell);
            else
                i--;

            foreach(SpellData _signature in group.signatures)
            {
                i++;
                if (i >= m_SpellButtons.Count)
                    m_SpellButtons.Add(Instantiate(m_SpellEntryPrefab, m_SpellContainer));
                item = m_SpellButtons[i];
                item.Setup(_signature, group.baseSpell, group.signatures, OnSelectSpell);
            }

            i++;
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
        LeanTween.cancel(m_SpellTweenId);
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

    private void OnClickClose()
    {
        UIPlayerInfo.Instance.ReOpen();
        Close();
    }

    private void OnSelectSpell(SpellData spell, SpellData baseSpell, List<SpellData> signatures)
    {
        m_SpellInfo.Show(m_Target, spell, baseSpell, signatures);
    }

    private void OnConfirmSpellcast(SpellData spell, List<spellIngredientsData> ingredients)
    {
        //slowly shake the screen while waiting for the cast response
        StreetMapUtils.ShakeCamera(
            new Vector3(1, -5, 5),
            0.02f,
            1f,
            10f
        );

        //show the casting animted UI and hide this
        UIWaitingCastResult.Instance.Show(m_Target, spell);
        //this.Close();

        //send the cast
        Spellcasting.CastSpell(spell, m_Target, ingredients, (result, response) =>
        {
            if (result == 200)
            {
                //focus on the target when the spell is succesfully cast
                StreetMapUtils.FocusOnTarget(m_Target, UIPlayerInfo.cameraFocusOffset, UIPlayerInfo.cameraFocusZoom);

                //close the UI
                //return to witch info screen
                UIWaitingCastResult.Instance.Close();
                this.Close();
                UIPlayerInfo.Instance.ReOpen();
            }
            else
            {
                //todo: handle error
                //show some feedback
                UIWaitingCastResult.Instance.Close();
            }
        });
    }
}
