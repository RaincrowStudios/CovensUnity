using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIWaitingCastResult : UIInfoPanel
{
    [Header("Generic")]
    [SerializeField] private TextMeshProUGUI m_TitleText;

    [Header("Loading")]
    [SerializeField] private Color m_IngredientColor;
    [SerializeField] private CanvasGroup m_LoadingGroup;
    [SerializeField] private Image m_SchoolCrest;
    [SerializeField] private Image m_LodingSpellGlyph;
    [SerializeField] private Image m_ToolsFill;
    [SerializeField] private Image m_GemsFill;
    [SerializeField] private Image m_HerbsFill;
    [SerializeField] private Image m_ToolsIcon;
    [SerializeField] private Image m_GemsIcon;
    [SerializeField] private Image m_HerbsIcon;
    [SerializeField] private Button m_CloseButton;

    [Header("OnCast")]
    [SerializeField] private CanvasGroup m_ResultGroup;
    [SerializeField] private TextMeshProUGUI m_ResultSpellTitle;
    [SerializeField] private Image m_ResultSpellGlyph;
    [SerializeField] private TextMeshProUGUI m_DamageDealt;
    [SerializeField] private TextMeshProUGUI m_XPGained;
    [SerializeField] private TextMeshProUGUI m_ResultText;
    [SerializeField] private Button m_ContinueButton;

    [Header("External")]
    [SerializeField] private Sprite m_ShadowCrest;
    [SerializeField] private Sprite m_GreyCrest;
    [SerializeField] private Sprite m_WhiteCrest;

    private static UIWaitingCastResult m_Instance;
    public static UIWaitingCastResult Instance
    {
        get
        {
            if (m_Instance == null)
                m_Instance = Instantiate(Resources.Load<UIWaitingCastResult>("UIWaitingCastResult"));
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


    private System.Action<Result> m_OnClickContinue;

    private IMarker m_Target;
    private SpellData m_Spell;
    private int m_LoadingTweenId;
    private int m_ResultsTweenId;
    private int m_DelayTweenId;
    private Result m_CastResults;

    protected override void Awake()
    {
        base.Awake();

        m_Instance = this;
        
        m_LoadingGroup.gameObject.SetActive(false);
        m_ResultGroup.gameObject.SetActive(false);
        m_LoadingGroup.alpha = 0;
        m_ResultGroup.alpha = 0;
        
        m_ContinueButton.onClick.AddListener(OnClickContinue);
        m_CloseButton.onClick.AddListener(OnClickClose);
    }

    public void Show(IMarker target, SpellData spell, List<spellIngredientsData> ingredients, System.Action<Result> onContinue)
    {
        LeanTween.cancel(m_LoadingTweenId);
        LeanTween.cancel(m_DelayTweenId);

        CloseResults();

        m_Target = target;
        m_Spell = spell;
        m_CastResults = null;
        m_OnClickContinue = onContinue;

        //setup loading
        m_TitleText.text = "Casting " + spell.displayName;

        //disable all ingredients
        m_ToolsFill.enabled = m_HerbsFill.enabled = m_GemsFill.enabled = false;
        m_ToolsIcon.color = m_HerbsIcon.color = m_GemsIcon.color = m_IngredientColor;

        //enable the ones being used
        for (int i = 0; i < ingredients.Count; i++)
        {
            if (ingredients[i].count <= 0)
                continue;

            IngredientDict ingredientData = DownloadedAssets.GetIngredient(ingredients[i].id);
            if (ingredientData == null)
                continue;

            if(ingredientData.type == "tool")
            {
                m_ToolsFill.enabled = true;
                m_ToolsIcon.color = Color.white;
            }
            else if (ingredientData.type == "gem")
            {
                m_GemsFill.enabled = true;
                m_GemsIcon.color = Color.white;
            }
            else if (ingredientData.type == "herb")
            {
                m_HerbsFill.enabled = true;
                m_HerbsIcon.color = Color.white;
            }
        }

        //load the school crest
        if (spell.school < 0)
            m_SchoolCrest.sprite = m_ShadowCrest;
        else if (spell.school > 0)
            m_SchoolCrest.sprite = m_WhiteCrest;
        else
            m_SchoolCrest.sprite = m_GreyCrest;

        //load the glyph icon
        m_LodingSpellGlyph.color = new Color(0, 0, 0, 0);
        string baseSpell = string.IsNullOrEmpty(spell.baseSpell) ? spell.id : spell.baseSpell;
        DownloadedAssets.GetSprite(baseSpell, 
            (spr) =>
            {
                m_LodingSpellGlyph.sprite = spr;
                LeanTween.value(0, 1, 0.5f).setEaseOutCubic().setOnUpdate((float t) =>
                {
                    m_LodingSpellGlyph.color = new Color(1, 1, 1, t);
                });
            });
                
        //activateloading group after few moments
        m_DelayTweenId = LeanTween.value(0, 0, 0)
            .setDelay(0.3f)
            .setOnStart(() =>
            {
                m_LoadingGroup.gameObject.SetActive(true);
                m_LoadingTweenId = LeanTween.alphaCanvas(m_LoadingGroup, 1f, 0.5f).setEaseOutCubic().uniqueId;
            })
            .uniqueId;

        //animate main group
        Show();
    }

    public void ShowResults(SpellDict spell, Result result)
    {
        LeanTween.cancel(m_ResultsTweenId);
        CloseLoading();

        m_CastResults = result;

        m_TitleText.text = "Results";
        m_ResultSpellTitle.text = spell.spellName;

        //load glyph
        m_ResultSpellGlyph.color = new Color(0, 0, 0, 0);
        string baseSpell = string.IsNullOrEmpty(m_Spell.baseSpell) ? m_Spell.id : m_Spell.baseSpell;
        DownloadedAssets.GetSprite(baseSpell,
            (spr) =>
            {
                m_ResultSpellGlyph.sprite = spr;
                m_ResultSpellGlyph.color = new Color(1, 1, 1, 1);
            });

        //stats
        m_DamageDealt.text = 
            result.total <= 0 ?
            $"Damage: {Mathf.Abs(result.total)}" :
            $"Healed: {result.total}";
        m_XPGained.text = $"XP gained: {result.xpGain}";
        if (result.critical)
            m_ResultText.text = "Critical Hit!";
        else if (result.effect == "backfire")
            m_ResultText.text = "Spell backfired!";
        else if (result.effect == "fail")
            m_ResultText.text = "Spell failed!";
        else if (result.effect == "fizzle")
            m_ResultText.text = "Spell fizzled!";
        else
            m_ResultText.text = "";

        m_ResultGroup.interactable = false;
        m_ResultGroup.gameObject.SetActive(true);
        m_ResultsTweenId = LeanTween.alphaCanvas(m_ResultGroup, 1f, 1f)
            .setEaseOutCubic()
            .setOnComplete(() => { m_ResultGroup.interactable = true; })
            .uniqueId;
    }

    public void CloseLoading()
    {
        LeanTween.cancel(m_LoadingTweenId);
        LeanTween.cancel(m_DelayTweenId);

        m_LoadingTweenId = LeanTween.alphaCanvas(m_LoadingGroup, 0, 0.25f)
            .setEaseOutCubic()
            .setOnComplete(() => { m_LoadingGroup.gameObject.SetActive(false); })
            .uniqueId;
    }

    public void CloseResults()
    {
        LeanTween.cancel(m_ResultsTweenId);

        m_ResultsTweenId = LeanTween.alphaCanvas(m_ResultGroup, 0, 0.25f)
            .setEaseOutCubic()
            .setOnComplete(() => { m_ResultGroup.gameObject.SetActive(false); })
            .uniqueId;
    }

    public void OnClickClose()
    {
        m_OnClickContinue?.Invoke(null);
        m_OnClickContinue = null;

        CloseResults();
        CloseLoading();

        Close();
    }

    public void OnClickContinue()
    {
        m_OnClickContinue?.Invoke(m_CastResults);
        m_OnClickContinue = null;

        OnClickClose();
    }
}
