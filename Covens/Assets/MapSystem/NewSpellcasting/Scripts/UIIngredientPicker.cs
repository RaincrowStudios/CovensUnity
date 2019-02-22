using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIIngredientPicker : MonoBehaviour
{
    private static UIIngredientPicker m_Instance;
    public static UIIngredientPicker Instance
    {
        get
        {
            if (m_Instance == null)
            {
                UIIngredientPicker prefab = Resources.Load<UIIngredientPicker>("UIIngredientPicker");
                m_Instance = Instantiate(prefab);
            }
            return m_Instance;
        }
    }
            
    
    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private GraphicRaycaster m_InputRaycaster;
    [SerializeField] private CanvasGroup m_CanvasGroup;

    [SerializeField] private TextMeshProUGUI m_ItemPrefab;
    [SerializeField] private LayoutGroup m_Container;

    [SerializeField] private Button m_ConfirmButton;
    [SerializeField] private Button m_CancelButton;

    [SerializeField] private Color m_TextColor = Color.white;
    [SerializeField] private Color m_SelectedColor = Color.white;

    private int m_TweenId;
    private IngredientType m_IngredientType;
    private List<TextMeshProUGUI> m_TextPrefabPool = new List<TextMeshProUGUI>();
    private List<Button> m_ButtonPrefabPool = new List<Button>();
    private List<InventoryItems> m_Items;

    //public string selectedIngredientName { get; private set; }
    //public string selectedIngredientId { get; private set; }
    private int m_SelectedIngredientIndex;

    public System.Action<InventoryItems, IngredientType> onSelectIngredient { get; set; }

    private void Awake()
    {
        m_Instance = this;
        m_ConfirmButton.onClick.AddListener(OnClickConfirm);
        m_CancelButton.onClick.AddListener(OnClickCancel);
        m_ItemPrefab.gameObject.SetActive(false);
        m_ItemPrefab.transform.SetParent(this.transform);

        m_InputRaycaster.enabled = false;
        m_Canvas.enabled = false;
        m_CanvasGroup.alpha = 0;
        m_IngredientType = IngredientType.none;
    }

    private void OnClickConfirm()
    {
        if (m_SelectedIngredientIndex < m_Items.Count)
            onSelectIngredient?.Invoke(m_Items[m_SelectedIngredientIndex], m_IngredientType);
        Close();
    }

    private void OnClickCancel()
    {
        Close();
    }

    public void Show(IngredientType type, InventoryItems selected)
    {
        if(m_IngredientType == type)
        {
            ReOpen();
            return;
        }

        m_IngredientType = type;

        if (m_SelectedIngredientIndex < m_TextPrefabPool.Count)
            m_TextPrefabPool[m_SelectedIngredientIndex].color = m_TextColor;

        for (int i = 0; i < m_TextPrefabPool.Count; i++)
        {
            m_TextPrefabPool[i].gameObject.SetActive(false);
        }

        switch (type)
        {
            case IngredientType.gem: m_Items = PlayerDataManager.playerData.ingredients.gems; break;
            case IngredientType.herb: m_Items = PlayerDataManager.playerData.ingredients.herbs; break;
            case IngredientType.tool: m_Items = PlayerDataManager.playerData.ingredients.tools; break;
            default: m_Items = new List<InventoryItems>(); break;
        }

        for (int i = 0; i < m_Items.Count; i++)
        {

        }

        m_ConfirmButton.interactable = false;

        ReOpen();
    }

    public void Close()
    {
        m_InputRaycaster.enabled = false;
        m_TweenId = LeanTween.value(m_CanvasGroup.alpha, 0, 0.5f)
          .setOnUpdate((float t) =>
          {
              m_CanvasGroup.alpha = t;
          })
          .setOnComplete(() =>
          {
              m_Canvas.enabled = false;
          })
          .setEaseOutCubic()
          .uniqueId;
    }

    private void ReOpen()
    {
        m_Canvas.enabled = true;
        m_InputRaycaster.enabled = true;
        m_TweenId = LeanTween.value(m_CanvasGroup.alpha, 1, 0.5f)
           .setOnUpdate((float t) =>
           {
               m_CanvasGroup.alpha = t;
           })
           .setOnComplete(() =>
           {

           })
           .setEaseOutCubic()
           .uniqueId;
    }
}
