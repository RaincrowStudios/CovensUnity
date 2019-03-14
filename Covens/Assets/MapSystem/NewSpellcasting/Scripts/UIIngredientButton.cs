using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIIngredientButton : MonoBehaviour
{
    [SerializeField] private Button m_Button;
    [SerializeField] private TextMeshProUGUI m_ItemTitle;
    [SerializeField] private TextMeshProUGUI m_ItemAmount;
    [SerializeField] private Button m_SubtractButton;
    [SerializeField] private Button m_AddButton;

    public System.Action onClick;
    public System.Action<InventoryItems, IngredientType, int> onAmountChange;

    private InventoryItems m_Ingredient;
    private IngredientType m_Type;
    private int m_Amount;
    private int m_Min;
    private int m_Max;

    private void Awake()
    {
        m_Button.onClick.AddListener(() => onClick?.Invoke());
        m_SubtractButton.onClick.AddListener(OnClickSub);
        m_AddButton.onClick.AddListener(OnClickAdd);
    }

    public void Setup(InventoryItems ingredient, IngredientType type, int amount, bool lockIngredient = false)
    {
        m_Ingredient = ingredient;
        m_Type = type;
        m_Amount = amount;
        m_Min = 0;

        m_AddButton.interactable = ingredient != null;
        m_SubtractButton.interactable = ingredient != null;

        if (ingredient == null)
        {
            m_ItemTitle.text = "none";
            m_ItemAmount.text = "";
            return;
        }
        else
        {
            m_Max = Mathf.Min(ingredient.count, 5);
            m_ItemTitle.text = ingredient.name;
            m_ItemAmount.text = amount + "";
        }
    }

    private void OnClickAdd()
    {
        ChangeAmount(+1);
    }

    private void OnClickSub()
    {
        ChangeAmount(-1);
    }

    private void ChangeAmount(int increment)
    {
        if (m_Ingredient == null)
            return;

        m_Amount = Mathf.Clamp(m_Amount + increment, m_Min, m_Max);
        m_ItemAmount.text = $"{m_Amount}";
        m_SubtractButton.interactable = m_Amount > m_Min;
        m_AddButton.interactable = m_Amount < m_Max;
        onAmountChange?.Invoke(m_Ingredient, m_Type, m_Amount);
    }
}
