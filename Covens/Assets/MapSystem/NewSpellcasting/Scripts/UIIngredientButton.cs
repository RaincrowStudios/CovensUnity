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
    public System.Action<InventoryItems, IngredientType> onSubtract;
    public System.Action<InventoryItems, IngredientType> onAdd;

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

    public void Setup(InventoryItems ingredient, IngredientType type, int amount)
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
            m_Max = ingredient.count;
            m_ItemTitle.text = ingredient.displayName;
            m_ItemAmount.text = ingredient.count + "";
        }
    }

    private void OnClickAdd()
    {
        if (m_Ingredient == null)
            return;

        m_Amount = Mathf.Clamp(m_Amount + 1, m_Min, m_Max);
        onAdd?.Invoke(m_Ingredient, m_Type);
    }

    private void OnClickSub()
    {
        if (m_Ingredient == null)
            return;

        m_Amount = Mathf.Clamp(m_Amount - 1, m_Min, m_Max);
        onSubtract?.Invoke(m_Ingredient, m_Type);
    }
}
