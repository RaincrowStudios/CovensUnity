using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Raincrow.Maps;
using TMPro;

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
    [SerializeField] private Button m_ShadowButton;
    [SerializeField] private Button m_GreyButton;
    [SerializeField] private Button m_LightButton;
    [SerializeField] private TextMeshProUGUI m_ShadowText;
    [SerializeField] private TextMeshProUGUI m_GreyText;
    [SerializeField] private TextMeshProUGUI m_WhiteText;

    [Header("Spell selection")]
    [SerializeField] private UISpellcastingItem m_SpellEntryPrefab;
    [SerializeField] private Transform m_SpellContainer;
    [SerializeField] private UISpellcastingInfo m_SpellInfo;
    [SerializeField] private RectTransform m_SelectedSpellOverlay;
    [SerializeField] private Image m_SelectedSpell_Glow;

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
    //private MarkerDataDetail m_TargetDetail;
    private System.Action m_OnFinish;
    private int m_SelectedSchool = -999;

    private int m_TweenId;

    private Dictionary<string, SpellGroup> m_SignatureDictionary;

    private void Awake()
    {
        m_Instance = this;

        //setup initial state
        m_MainPanel.anchoredPosition = new Vector2(m_MainPanel.sizeDelta.x, 0);
        m_CanvasGroup.alpha = 0;
        EnableCanvas(false);
        m_SpellEntryPrefab.gameObject.SetActive(false);
        m_SpellEntryPrefab.transform.SetParent(this.transform);
        m_SelectedSpellOverlay.gameObject.SetActive(false);
        m_SelectedSpellOverlay.SetParent(transform);

        //setup buttons
        m_CloseButton.onClick.AddListener(OnClickClose);
        m_ShadowButton.onClick.AddListener(() =>
        {
            SetupSpellSelection(-1);
        });
        m_GreyButton.onClick.AddListener(() =>
        {
            SetupSpellSelection(0);
        });
        m_LightButton.onClick.AddListener(() =>
        {
            SetupSpellSelection(1);
        });

        m_SpellInfo.onConfirmSpellcast += OnConfirmSpellcast;
    }

    public void EnableCanvas(bool enable)
    {
        m_Canvas.enabled = enable;
        m_InputRaycaster.enabled = enable;
    }

    public void Show(IMarker target, List<SpellData> spells, System.Action onFinishSpellcasting)
    {
        LeanTween.cancel(m_TweenId);

        m_Target = target;
        m_Spells = spells;
        m_OnFinish += onFinishSpellcasting;

        int school = m_SelectedSchool;
        if (school < 0)
            school = PlayerDataManager.playerData.degree == 0 ? 0 : (int)Mathf.Sign(PlayerDataManager.playerData.degree);

        SetupSpellSelection(school);
        
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

    public void FinishSpellcastingFlow()
    {
        Close();
        
        m_OnFinish?.Invoke();
        m_OnFinish = null;
    }
    
    public void SetupSpellSelection(int school)
    {
        if (m_SelectedSchool != school)
        {
            m_SelectedSpellOverlay.gameObject.SetActive(false);

            m_ShadowText.text = "Shadow";
            m_GreyText.text = "Grey";
            m_WhiteText.text = "White";
            Color color;
            if (school < 0)
            {
                m_ShadowText.text = "<u>Shadow</u>";
                color = Utilities.Purple;
            }
            else if (school > 0)
            {
                m_WhiteText.text = "<u>White</u>";
                color = Utilities.Orange;
            }
            else
            {
                m_GreyText.text = "<u>Grey</u>";
                color = Utilities.Blue;
            }
            color.a = 0.3f;
            m_SelectedSpell_Glow.color = color;

            //setup spells
            m_SignatureDictionary = new Dictionary<string, SpellGroup>();
            for (int i = 0; i < m_Spells.Count; i++)
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
                    else if (m_Spells[i].unlocked)
                        group.signatures.Add(m_Spells[i]);
                }
            }

            StartCoroutine(SetupSpells(m_SignatureDictionary));
        }

        m_SelectedSchool = school;
    }

    private IEnumerator SetupSpells(Dictionary<string, SpellGroup> spells)
    {
        m_SelectedSpellOverlay.gameObject.SetActive(false);

        int i = 0;
        foreach (SpellGroup group in m_SignatureDictionary.Values)
        {
            UISpellcastingItem item;
            if (i >= m_SpellButtons.Count)
                m_SpellButtons.Add(Instantiate(m_SpellEntryPrefab, m_SpellContainer));
            item = m_SpellButtons[i];

            if (group.baseSpell != null)
            {
                item.Setup(m_Target, group.baseSpell, group.baseSpell, group.signatures, OnSelectSpell);
            }
            else
            {
                i--;
            }

            foreach (SpellData _signature in group.signatures)
            {
                i++;
                if (i >= m_SpellButtons.Count)
                    m_SpellButtons.Add(Instantiate(m_SpellEntryPrefab, m_SpellContainer));
                item = m_SpellButtons[i];
                item.Setup(m_Target, _signature, group.baseSpell, group.signatures, OnSelectSpell);
            }

            i++;
        }

        //disable unused button
        for (; i < m_SpellButtons.Count; i++)
            m_SpellButtons[i].gameObject.SetActive(false);

        yield return 1;

        m_SpellButtons[0].OnClick();
    }

    private void OnClickClose()
    {
        FinishSpellcastingFlow();
    }

    private void OnSelectSpell(UISpellcastingItem item, SpellData spell, SpellData baseSpell, List<SpellData> signatures)
    {
        m_SelectedSpellOverlay.SetParent(item.transform);
        m_SelectedSpellOverlay.localPosition = Vector2.zero;
        m_SelectedSpellOverlay.gameObject.SetActive(true);
        m_SpellInfo.Show(m_Target, spell, baseSpell, signatures);
    }

    private void OnConfirmSpellcast(SpellData spell, List<spellIngredientsData> ingredients)
    {
        Close();

        //send the cast
        Spellcasting.CastSpell(spell, m_Target, ingredients, (result) =>
        {
            //if success, return to player info
            if (result.effect == "success" || result.effect == "fizzle")
            {
                if (result.effect == "success")
                {
                    print("playing fx");
                }
                FinishSpellcastingFlow();
            }
            else //reopen the UI for a possible retry
            {
                ReOpen();
            }
        });
    }
}
