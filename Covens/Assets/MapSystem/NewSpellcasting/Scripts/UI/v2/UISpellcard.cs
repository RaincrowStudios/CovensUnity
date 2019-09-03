using EnhancedUI.EnhancedScroller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Raincrow.Maps;
using static CooldownManager;

public class UISpellcard : MonoBehaviour// : EnhancedScrollerCellView
{
    [SerializeField] private CanvasGroup m_CanvasGroup;
    [SerializeField] private TextMeshProUGUI m_SpellName;
    [SerializeField] private TextMeshProUGUI m_SpellCost;
    [SerializeField] private TextMeshProUGUI m_SpellDescription;

    [SerializeField] private Image m_SchoolIcon;
    [SerializeField] private Image m_SpellIcon;
    [SerializeField] private Image m_SchoolFrame;
    [SerializeField] private Image m_SchoolFrame2;
    [SerializeField] private Image m_SpellFrame;

    [SerializeField] private Button m_CardButton;
    [SerializeField] private Button m_SchoolButton;
    [SerializeField] private Button m_SpellButton;

    [Header("Ingredients")]
    [SerializeField] private HorizontalLayoutGroup m_IngredientsContainer;
    [SerializeField] private Image m_Gem;
    [SerializeField] private Image m_Tool;
    [SerializeField] private Image m_Herb;
    [SerializeField] private Button m_GemButton;
    [SerializeField] private Button m_ToolButton;
    [SerializeField] private Button m_HerbButton;

    [Header("cooldowns")]
    [SerializeField] private Image m_CooldownBackground;
    [SerializeField] private Image m_CooldownIcon;
    [SerializeField] private TextMeshProUGUI m_CooldownTex;

    [Header("crests")]
    [SerializeField] private Sprite m_ShadowCrest;
    [SerializeField] private Sprite m_GreyCrest;
    [SerializeField] private Sprite m_WhiteCrest;


    public SpellData Spell { get; set; }
    public RectTransform RectTransform { get; private set; }

    private System.Action<int> m_OnClickSchool;
    private System.Action<UISpellcard> m_OnClickCard;
    private System.Action<UISpellcard> m_OnClickGlyph;

    private int m_TweenId;
    private int m_CooldownTweenId;

    private void Awake()
    {
        m_SchoolButton.onClick.AddListener(OnClickSchool);
        m_CardButton.onClick.AddListener(OnClickCard);
        m_SpellButton.onClick.AddListener(OnClickGlyph);
        m_CooldownTex.text = "";

        RectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        transform.localScale = Vector3.one;
    }

    public void SetData(
        SpellData spell,
        System.Action<int> onClickSchool,
        System.Action<UISpellcard> onClickCard,
        System.Action<UISpellcard> onClickGlyph)
    {
        LeanTween.cancel(m_CooldownTweenId);

        name = "[" + spell.id + "] UISpellcard prefab";
        Spell = spell;
        m_OnClickSchool = onClickSchool;
        m_OnClickCard = onClickCard;
        m_OnClickGlyph = onClickGlyph;

        m_SpellName.text = spell.Name;
        m_SpellCost.text = LocalizeLookUp.GetText("spell_data_cost").Replace("{{Energy Cost}}", spell.cost.ToString());
        m_SpellDescription.text = PlayerManager.inSpiritForm ? spell.SpiritDescription : spell.PhysicalDescription;

        if (spell.school < 0)
        {
            m_SchoolFrame.color = m_SpellFrame.color = Utilities.Purple;
            m_SchoolIcon.overrideSprite = m_ShadowCrest;
        }
        else if (spell.school > 0)
        {
            m_SchoolFrame.color = m_SpellFrame.color = Utilities.Orange;
            m_SchoolIcon.overrideSprite = m_WhiteCrest;
        }
        else
        {
            m_SchoolFrame.color = m_SpellFrame.color = Utilities.Blue;
            m_SchoolIcon.overrideSprite = m_GreyCrest;
        }
        m_SchoolFrame2.color = m_SchoolFrame.color;
        m_SpellIcon.overrideSprite = m_CooldownIcon.overrideSprite = null;
        DownloadedAssets.GetSprite(spell.id, spr =>
        {
            m_SpellIcon.overrideSprite = m_CooldownIcon.overrideSprite = spr;
        });
    }

    public void UpdateCancast(CharacterMarkerData targetData, IMarker targetMarker)
    {
        LeanTween.cancel(m_CooldownTweenId);

        Spellcasting.SpellState canCast = Spellcasting.CanCast(Spell, targetMarker, targetData);
        if (canCast == Spellcasting.SpellState.CanCast)
        {
            m_SpellFrame.gameObject.SetActive(true);
            m_SchoolFrame.gameObject.SetActive(true);
            m_SpellButton.interactable = true;
            m_SpellButton.gameObject.GetComponent<CanvasGroup>().alpha = 1f;
            m_SchoolButton.gameObject.GetComponent<CanvasGroup>().alpha = 1f;
        }
        else
        {
            m_SchoolFrame.gameObject.SetActive(false);
            m_SpellFrame.gameObject.SetActive(false);
            m_SpellButton.interactable = false;
            m_SpellButton.gameObject.GetComponent<CanvasGroup>().alpha = 0.5f;
            m_SchoolButton.gameObject.GetComponent<CanvasGroup>().alpha = 0.5f;
        }

        if (canCast == Spellcasting.SpellState.InCooldown)
        {
            Cooldown? cd = CooldownManager.GetCooldown(Spell.id);
            if (cd != null)
            {
                float remaining = cd.Value.Remaining;
                float total = cd.Value.total;
                m_CooldownTweenId = LeanTween.value(remaining / total, 0, remaining)
                    .setOnUpdate((float t) => m_CooldownBackground.fillAmount = m_CooldownIcon.fillAmount = t)
                    .setOnComplete(() => UpdateCancast(targetData, targetMarker))
                    .uniqueId;
            }

        }
        else
        {
            m_CooldownBackground.fillAmount = m_CooldownIcon.fillAmount = 0;
        }

        SetIngredients(Spell != null ? Spell.ingredients : null);
    }

    private void OnClickSchool()
    {
        m_OnClickSchool?.Invoke(Spell.school);
    }

    private void OnClickCard()
    {
        m_OnClickCard?.Invoke(this);
    }

    private void OnClickGlyph()
    {
        m_OnClickGlyph?.Invoke(this);
    }

    public void SetAlpha(float a, float time = 0)
    {
        LeanTween.cancel(m_TweenId);

        if (time == 0)
        {
            m_CanvasGroup.alpha = a;
            return;
        }

        m_TweenId = LeanTween.alphaCanvas(m_CanvasGroup, a, time).setEaseOutCubic().uniqueId;
    }

    public void SetInteractable(bool interactable)
    {
        m_CanvasGroup.interactable = interactable;
    }

    public void SetIngredients(List<string> requiredIngredients)
    {
        m_GemButton.onClick.RemoveAllListeners();
        m_ToolButton.onClick.RemoveAllListeners();
        m_HerbButton.onClick.RemoveAllListeners();

        if (requiredIngredients == null || requiredIngredients.Count == 0)
        {
            m_IngredientsContainer.gameObject.SetActive(false);
            return;
        }
        else
        {
            m_IngredientsContainer.gameObject.SetActive(true);
        }

        string requiredGem = null;
        string requiredTool = null;
        string requiredHerb = null;

        foreach (string ingr in requiredIngredients)
        {
            var data = DownloadedAssets.GetCollectable(ingr);
            if (data.Type == IngredientType.gem)
            {
                requiredGem = ingr;
                m_GemButton.onClick.AddListener(() => UIConditionInfo.Instance.Show(LocalizeLookUp.GetCollectableName(ingr), LocalizeLookUp.GetCollectableDesc(ingr), m_Gem.rectTransform, new Vector2(0, 0), false));
            }
            else if (data.Type == IngredientType.tool)
            {
                requiredTool = ingr;
                m_ToolButton.onClick.AddListener(() => UIConditionInfo.Instance.Show(LocalizeLookUp.GetCollectableName(ingr), LocalizeLookUp.GetCollectableDesc(ingr), m_Tool.rectTransform, new Vector2(0, 0), false));
            }
            else if (data.Type == IngredientType.herb)
            {
                requiredHerb = ingr;
                m_HerbButton.onClick.AddListener(() => UIConditionInfo.Instance.Show(LocalizeLookUp.GetCollectableName(ingr), LocalizeLookUp.GetCollectableDesc(ingr), m_Herb.rectTransform, new Vector2(0, 0), false));
            }
        }

        Color enableColor = Color.white;
        Color disableColor = new Color(1, 1, 1, 0.2f);

        //gem
        if (requiredGem == null)
        {
            m_Gem.gameObject.SetActive(false);
        }
        else
        {
            m_Gem.gameObject.SetActive(true);
            if (PlayerDataManager.playerData.GetIngredient(requiredGem) > 0)
                m_Gem.color = enableColor;
            else
                m_Gem.color = disableColor;
        }

        //tool
        if (requiredTool == null)
        {
            m_Tool.gameObject.SetActive(false);
        }
        else
        {
            m_Tool.gameObject.SetActive(true);
            if (PlayerDataManager.playerData.GetIngredient(requiredTool) > 0)
                m_Tool.color = enableColor;
            else
                m_Tool.color = disableColor;
        }

        //herb
        if (requiredHerb == null)
        {
            m_Herb.gameObject.SetActive(false);
        }
        else
        {
            m_Herb.gameObject.SetActive(true);
            if (PlayerDataManager.playerData.GetIngredient(requiredHerb) > 0)
                m_Herb.color = enableColor;
            else
                m_Herb.color = disableColor;
        }
    }
}
