using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Raincrow.Maps;
using TMPro;

public class UISpellcasting : UIInfoPanel
{
    [SerializeField] private Button m_BackButton;
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

    public static bool isOpen
    {
        get
        {
            if (m_Instance == null)
                return false;
            else
                return m_Instance.IsShowing;
        }
    }

    private List<UISpellcastingItem> m_SpellButtons = new List<UISpellcastingItem>();
    private List<SpellData> m_Spells;
    private MarkerDataDetail m_Target;
    private IMarker m_Marker;
    private System.Action m_OnFinishSpellcasting;
    private System.Action m_OnBack;
    private System.Action m_OnClose;
    private bool m_CloseOnFinish;
    private int m_SelectedSchool = -999;
    private int m_PreviousSchool = -999;
    private int m_PreviousSpell = 0;

    protected override void Awake()
    {
        base.Awake();

        m_Instance = this;

        //setup initial state
        m_SpellEntryPrefab.gameObject.SetActive(false);
        m_SpellEntryPrefab.transform.SetParent(this.transform);
        m_SelectedSpellOverlay.gameObject.SetActive(false);
        m_SelectedSpellOverlay.SetParent(transform);

        //setup buttons
        m_BackButton.onClick.AddListener(OnClickBack);
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
    }

    public void Show(MarkerDataDetail target, IMarker marker, List<SpellData> spells, System.Action onFinishSpellcasting, System.Action onBack = null, System.Action onClose = null, bool closeOnFinish = false)
    {
        m_Target = target;
        m_Marker = marker;
        m_Spells = spells;
        m_OnFinishSpellcasting = onFinishSpellcasting;
        m_OnBack = onBack;
        m_OnClose = onClose;
        m_CloseOnFinish = closeOnFinish;

        int school = m_PreviousSchool;
        if (school == -999)
            school = PlayerDataManager.playerData.degree == 0 ? 0 : (int)Mathf.Sign(PlayerDataManager.playerData.degree);

        SetupSpellSelection(school);

        base.Show();
    }

    public void SetupSpellSelection(int school)
    {
        if (m_SelectedSchool != school)
        {
            m_SpellInfo.Hide();

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
            StopAllCoroutines();

            //disable buttons
            for (int i = 0; i < m_SpellButtons.Count; i++)
                m_SpellButtons[i].Hide();

            m_SelectedSpellOverlay.gameObject.SetActive(false);

            List<SpellData> spells = new List<SpellData>();
            for (int i = 0; i < m_Spells.Count; i++)
            {
                if (m_Spells[i].school == school)
                    spells.Add(m_Spells[i]);
            }
            StartCoroutine(SetupSpellList(spells));
        }

        m_PreviousSchool = m_SelectedSchool = school;
    }

    private IEnumerator SetupSpellList(List<SpellData> spells)
    {
        for (int i = 0; i < spells.Count; i++)
        {
            UISpellcastingItem item;
            if (i >= m_SpellButtons.Count)
                m_SpellButtons.Add(Instantiate(m_SpellEntryPrefab, m_SpellContainer));

            item = m_SpellButtons[i];

            item.Setup(m_Target, m_Marker, spells[i], OnSelectSpell);
        }
        yield return 0;

        LayoutRebuilder.ForceRebuildLayoutImmediate(m_SpellContainer.parent.GetComponent<RectTransform>());

        m_SpellButtons[0].OnClick();

        for (int i = 0; i < spells.Count; i++)
        {
            m_SpellButtons[i].Show();
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void FinishSpellcastingFlow()
    {
        m_OnFinishSpellcasting?.Invoke();
        if (m_CloseOnFinish)
            Close();
        else
            ReOpen();
    }

    private void OnClickBack()
    {
        m_OnBack?.Invoke();
        Close();
    }

    private void OnClickClose()
    {
        m_OnClose?.Invoke();
        Close();
    }

    protected override void Close()
    {
        base.Close();

        m_SelectedSchool = -999;
        m_OnFinishSpellcasting = null;
        m_OnBack = null;
        m_OnClose = null;
    }

    private void OnSelectSpell(UISpellcastingItem item, SpellData spell)
    {
        m_SelectedSpellOverlay.SetParent(item.transform);
        m_SelectedSpellOverlay.localPosition = Vector2.zero;
        m_SelectedSpellOverlay.gameObject.SetActive(true);
        m_SpellInfo.Show(m_Target, m_Marker, spell, OnConfirmSpellcast);
    }

    private void OnConfirmSpellcast(SpellData spell)
    {
        Hide();

        //send the cast
        Spellcasting.CastSpell(spell, m_Marker, new List<spellIngredientsData>(), 
            (result) => //ON CLICK CONTINUE
            {
                //if success, return to player info
                if (result != null && (result.effect == "success" || result.effect == "fizzle"))
                {
                    if (result.effect == "success")
                    {
                        print("playing fx");
                    }
                    FinishSpellcastingFlow();
                }
                else //reopen the UI for a possible retry
                {
                    if (m_Marker.customData != null)
                    {
                        IMarker marker = MarkerManager.GetMarker((m_Marker.customData as Token).instance);
                        if (marker != null)
                            StreetMapUtils.FocusOnTarget(marker);
                    }
                    ReOpen();
                }

                m_SpellInfo.UpdateCanCast();
            },
            () => //ON CLICK CLOSE
            {
                OnClickClose();
            });
    }
}