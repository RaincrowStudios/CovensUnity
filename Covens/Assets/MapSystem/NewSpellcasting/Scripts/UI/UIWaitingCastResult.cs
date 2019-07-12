using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Raincrow.GameEvent;

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


    private System.Action<MapSpellCastResult> m_OnClickContinue;
    private System.Action m_OnClose;

    private IMarker m_Target;
    private SpellData m_Spell;
    private int m_LoadingTweenId;
    private int m_ResultsTweenId;
    private int m_DelayTweenId;
    private int m_ButtonTweenId;
    private MapSpellCastResult m_CastResults;
    private bool m_WaitingResults = false;

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

    public void Show(IMarker target, SpellData spell, List<spellIngredientsData> ingredients, System.Action<MapSpellCastResult> onContinue, System.Action onClose = null)
    {
        m_WaitingResults = true;

        LeanTween.cancel(m_LoadingTweenId);
        LeanTween.cancel(m_DelayTweenId);
        LeanTween.cancel(m_ButtonTweenId);

        CloseResults();

        m_Target = target;
        m_Spell = spell;
        m_CastResults = null;
        m_OnClickContinue = onContinue;
        m_OnClose = onClose;

        //setup loading
        m_TitleText.text = LocalizeLookUp.GetText("card_witch_casting").Replace("{{Spell Name}}", LocalizeLookUp.GetSpellName(spell.id));//"Casting " + spell.displayName;

        //disable all ingredients
        m_ToolsFill.enabled = m_HerbsFill.enabled = m_GemsFill.enabled = false;
        m_ToolsIcon.color = m_HerbsIcon.color = m_GemsIcon.color = m_IngredientColor;

        //enable the ones being used
        for (int i = 0; i < ingredients.Count; i++)
        {
            if (ingredients[i].count <= 0)
                continue;

            IngredientData ingredientData = DownloadedAssets.GetCollectable(ingredients[i].id);

            if (ingredientData.type == "tool")
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
        DownloadedAssets.GetSprite(spell.id,
            (spr) =>
            {
                m_LodingSpellGlyph.overrideSprite = spr;
                LeanTween.value(0, 1, 0.5f).setEaseOutCubic().setOnUpdate((float t) =>
                {
                    m_LodingSpellGlyph.color = new Color(1, 1, 1, t);
                });
            });

        //activate loading group after few moments
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

    public void ShowResults(SpellData spell, MapSpellCastResult result)
    {
        LeanTween.cancel(m_ResultsTweenId);
        LeanTween.cancel(m_ButtonTweenId);
        CloseLoading();

        m_CastResults = result;

        m_TitleText.text = LocalizeLookUp.GetText("generic_results");//"Results";
        m_ResultSpellTitle.text = LocalizeLookUp.GetSpellName(spell.id);

        //load glyph
        m_ResultSpellGlyph.color = new Color(0, 0, 0, 0);

        DownloadedAssets.GetSprite(spell.id,
            (spr) =>
            {
                m_ResultSpellGlyph.overrideSprite = spr;
                m_ResultSpellGlyph.color = new Color(1, 1, 1, 1);
            });

        //stats
        m_DamageDealt.text =
            result.Damage <= 0 ?
            LocalizeLookUp.GetText("generic_damage") + " : " + Mathf.Abs(result.Damage)/*$"Damage: {Mathf.Abs(result.total)}"*/ :
            LocalizeLookUp.GetText("generic_healed") + " : " + result.Damage;//$"Healed: {result.total}";

        // TODO: XP GAIN will come from somewhere else
        //m_XPGained.text = LocalizeLookUp.GetText("spirit_deck_xp_gained").Replace("{{Number}}", result.xpGain.ToString());// $"XP gained: {result.xpGain}";
        //if (result.xpGain == 0)
        //    m_XPGained.gameObject.SetActive(false);
        //else
        //    m_XPGained.gameObject.SetActive(true);

        if (result.IsCritical)
        {
            m_ResultText.text = LocalizeLookUp.GetText("cast_crit") + " " + LocalizeLookUp.GetText("card_witch_cast");// "Critical Hit!";
        }
        //else if (result.effect == "backfire")
        //    m_ResultText.text = LocalizeLookUp.GetText("spell_cast_backfire");//"Spell backfired!";
        else if (result.IsSuccess)
        {
            m_ResultText.text = LocalizeLookUp.GetText("spell_fail");//"Spell failed!";
        }        
        else
        {
            m_ResultText.text = "";
        }
        //else if (result.effect == "fizzle")
        //    m_ResultText.text = LocalizeLookUp.GetText("spell_fizzle");//"Spell fizzled!";

        //only enable continue after few moments
        m_ResultGroup.interactable = false;
        m_ButtonTweenId = LeanTween.value(0, 0, 0).setDelay(0.2f).setOnStart(() =>
            {
                m_ResultGroup.interactable = true;
            }).uniqueId;

        m_ResultGroup.gameObject.SetActive(true);
        m_ResultsTweenId = LeanTween.alphaCanvas(m_ResultGroup, 1f, 1f)
            .setEaseOutCubic()
            .uniqueId;
    }

    public override void Close()
    {
        base.Close();

        m_OnClickContinue = null;
        m_OnClose = null;

        CloseResults();
        CloseLoading();
    }

    public void CloseLoading()
    {
        m_WaitingResults = false;

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

    private void OnClickClose()
    {
        m_OnClose?.Invoke();
        Close();
    }

    public void OnClickContinue()
    {
        m_OnClickContinue?.Invoke(m_CastResults);
        Close();
    }
}
