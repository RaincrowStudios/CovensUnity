using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Raincrow.Maps;

public class UISpellcastingIngredients : MonoBehaviour
{
    [Header("Main")]
    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private GraphicRaycaster m_InputRaycaster;
    [SerializeField] private CanvasGroup m_CanvasGroup;
    [SerializeField] private RectTransform m_Panel;
    [SerializeField] private Button m_ConfirmButton;
    [SerializeField] private TextMeshProUGUI m_ConfirmText;
    [SerializeField] private Button m_CloseButton;

    [Header("")]
    [SerializeField] private UIIngredientButton m_ToolButton;
    [SerializeField] private UIIngredientButton m_HerbButton;
    [SerializeField] private UIIngredientButton m_GemButton;
    
    private static UISpellcastingIngredients m_Instance;
    public static UISpellcastingIngredients Instance
    {
        get
        {
            if (m_Instance == null)
            {
                UISpellcastingIngredients prefab = Resources.Load<UISpellcastingIngredients>("UISpellcastingIngredients");
                m_Instance = Instantiate(prefab);
            }
            return m_Instance;
        }
    }
    
    private int m_TweenId;
    private InventoryItems m_SelectedTool;
    private InventoryItems m_SelectedHerb;
    private InventoryItems m_SelectedGem;
    private int m_ToolAmount;
    private int m_HerbAmount;
    private int m_GemAmount;

    public static System.Action<List<spellIngredientsData>> onConfirmIngredients;

    public static bool isOpen
    {
        get
        {
            if (m_Instance == null)
                return false;
            else
                return m_Instance.m_InputRaycaster.enabled;
        }
    }

    private void Awake()
    {
        m_Instance = this;
        m_Canvas.enabled = false;
        m_InputRaycaster.enabled = false;
        m_CanvasGroup.alpha = 0;

        m_CloseButton.onClick.AddListener(OnClickCancel);
        m_ConfirmButton.onClick.AddListener(OnClickConfirm);

        m_ToolButton.onClick = () => OnClickIngredient(IngredientType.tool);
        m_HerbButton.onClick = () => OnClickIngredient(IngredientType.herb);
        m_GemButton.onClick = () => OnClickIngredient(IngredientType.gem);

        m_ToolButton.onAmountChange = OnChangeAmount;
        m_HerbButton.onAmountChange = OnChangeAmount;
        m_GemButton.onAmountChange = OnChangeAmount;

        Spellcasting.OnSpellCast += Spellcasting_OnSpellCast;
    }

    private void Spellcasting_OnSpellCast(IMarker arg1, SpellDict arg2, Result arg3)
    {
        //resets the UI after the casting is completed
        m_SelectedTool = m_SelectedGem = m_SelectedHerb = null;
        m_ToolAmount = m_GemAmount = m_HerbAmount = 0;
    }

    private void Start()
    {
        UIIngredientPicker.onSelectIngredient = OnChangeIngredient;
    }

    public void Show(SpellData spell)
    {
        m_Canvas.enabled = true;
        m_InputRaycaster.enabled = true;

        m_TweenId = LeanTween.value(0, 1, 0.5f)
           .setOnUpdate((float t) =>
           {
               m_Panel.anchoredPosition = new Vector2((1 - t) * m_Panel.sizeDelta.x, 0);
               m_CanvasGroup.alpha = t;
           })
           .setEaseOutCubic()
           .uniqueId;
        
        SetConfirmText();

        if (spell.ingredients != null && spell.ingredients.Length > 0)
        {
            for (int i = 0; i < spell.ingredients.Length; i++)
            {

            }
        }

        m_ToolButton.Setup(m_SelectedTool, IngredientType.tool, m_ToolAmount);
        m_HerbButton.Setup(m_SelectedHerb, IngredientType.herb, m_HerbAmount);
        m_GemButton.Setup(m_SelectedGem, IngredientType.gem, m_GemAmount);
    }

    public void Close()
    {
        m_InputRaycaster.enabled = false;
        m_TweenId = LeanTween.value(1, 0, 0.5f)
           .setOnUpdate((float t) =>
           {
               m_Panel.anchoredPosition = new Vector2(t * m_Panel.sizeDelta.x, 0);
               m_CanvasGroup.alpha = t;
           })
           .setOnComplete(() =>
           {
               m_Canvas.enabled = false;
           })
           .setEaseOutCubic()
           .uniqueId;
    }

    private void OnClickConfirm()
    {
        List<spellIngredientsData> ingredients = new List<spellIngredientsData>();
        
        if(m_SelectedHerb != null && m_HerbAmount > 0)
        {
            ingredients.Add(new spellIngredientsData
            {
                id = m_SelectedHerb.id,
                count = m_HerbAmount
            });
        }
        if(m_SelectedTool != null && m_ToolAmount > 0)
        {
            ingredients.Add(new spellIngredientsData
            {
                id = m_SelectedTool.id,
                count = m_ToolAmount
            });
        }
        if (m_SelectedGem != null && m_GemAmount > 0)
        {
            ingredients.Add(new spellIngredientsData
            {
                id = m_SelectedGem.id,
                count = m_GemAmount
            });
        }

        onConfirmIngredients?.Invoke(ingredients);
        this.Close();
    }

    private void OnClickCancel()
    {
        UISpellcasting.Instance.ReOpen();
        Close();
    }

    private void OnClickIngredient(IngredientType type)
    {
        InventoryItems selected = null;
        switch (type)
        {
            case IngredientType.gem: selected = m_SelectedGem; break;
            case IngredientType.herb: selected = m_SelectedHerb; break;
            case IngredientType.tool: selected = m_SelectedTool; break;
        }
        UIIngredientPicker.Instance.Show(type, selected);
    }

    private void OnChangeAmount(InventoryItems item, IngredientType type, int amount)
    {
        switch (type)
        {
            case IngredientType.tool:
                m_ToolAmount = amount;
                break;
            case IngredientType.herb:
                m_HerbAmount = amount;
                break;
            case IngredientType.gem:
                m_GemAmount = amount;
                break;
        }

        SetConfirmText();
    }

    private void OnChangeIngredient(InventoryItems item, IngredientType type)
    {
        switch (type)
        {
            case IngredientType.tool:
                m_SelectedTool = item;
                m_ToolAmount = item == null ? 0 : 1;
                m_ToolButton.Setup(item, type, 1);
                break;
            case IngredientType.herb:
                m_SelectedHerb = item;
                m_HerbAmount = item == null ? 0 : 1;
                m_HerbButton.Setup(item, type, 1);
                break;
            case IngredientType.gem:
                m_SelectedGem = item;
                m_GemAmount = item == null ? 0 : 1;
                m_GemButton.Setup(item, type, 1);
                break;
        }

        SetConfirmText();
    }


    private void SetConfirmText()
    {
        bool ingredientSelected = m_ToolAmount > 0 || m_HerbAmount > 0 || m_GemAmount > 0;

        if (ingredientSelected)
            m_ConfirmText.text = "Cast";
        else
            m_ConfirmText.text = "Cast without ingredients";
    }
}
