using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UISpellcastingIngredients : MonoBehaviour
{
    [Header("Main")]
    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private GraphicRaycaster m_InputRaycaster;
    [SerializeField] private CanvasGroup m_CanvasGroup;
    [SerializeField] private RectTransform m_Panel;
    [SerializeField] private Button m_ConfirmButton;
    [SerializeField] private Button m_CloseButton;

    [Header("")]
    [SerializeField] private UIIngredientButton m_ToolButton;
    [SerializeField] private UIIngredientButton m_HerbButton;
    [SerializeField] private UIIngredientButton m_GemButton;

    private int m_TweenId;

    public static UISpellcastingIngredients Instance { get; private set; }
    public List<spellIngredientsData> ingredients { get { return new List<spellIngredientsData>(); } }

    private InventoryItems m_SelectedTool;
    private InventoryItems m_SelectedHerb;
    private InventoryItems m_SelectedGem;
    private int m_ToolAmount;
    private int m_HerbAmount;
    private int m_GemAmount;

    private void Awake()
    {
        Instance = this;
        m_Canvas.enabled = false;
        m_InputRaycaster.enabled = false;
        m_CanvasGroup.alpha = 0;

        m_CloseButton.onClick.AddListener(OnClickCancel);
        m_ConfirmButton.onClick.AddListener(OnClickConfirm);

        m_ToolButton.onClick = () => OnClickIngredient(IngredientType.tool);
        m_HerbButton.onClick = () => OnClickIngredient(IngredientType.herb);
        m_GemButton.onClick = () => OnClickIngredient(IngredientType.gem);

        m_ToolButton.onSubtract = OnRemoveIngredient;
        m_GemButton.onSubtract = OnRemoveIngredient;
        m_HerbButton.onSubtract = OnRemoveIngredient;

        m_ToolButton.onAdd = OnAddIngredient;
        m_GemButton.onAdd = OnAddIngredient;
        m_HerbButton.onAdd = OnAddIngredient;
    }

    private void Start()
    {
        UIIngredientPicker.Instance.onSelectIngredient = OnChangeIngredient;
    }

    public void Show(SpellData spell)
    {
        UISpellcasting.Instance.Close();

        m_Canvas.enabled = true;
        m_InputRaycaster.enabled = true;

        m_TweenId = LeanTween.value(0, 1, 0.5f)
           .setOnUpdate((float t) =>
           {
               m_Panel.anchoredPosition = new Vector2((1 - t) * m_Panel.sizeDelta.x, 0);
               m_CanvasGroup.alpha = t;
           })
           //.setOnComplete(() =>
           //{
           //    UISpellcasting.Instance.EnableCanvas(false);
           //})
           .setEaseOutCubic()
           .uniqueId;

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
        UIIngredientPicker.Instance.Show(type, m_SelectedTool);
    }

    private void OnAddIngredient(InventoryItems item, IngredientType type)
    {
        Debug.Log("increment " + type);
    }

    private void OnRemoveIngredient(InventoryItems item, IngredientType type)
    {
        Debug.Log("decrement " + type);
    }

    private void OnChangeIngredient(InventoryItems item, IngredientType type)
    {
        Debug.Log("selected " + type + ": " + item.displayName);
    }

}
