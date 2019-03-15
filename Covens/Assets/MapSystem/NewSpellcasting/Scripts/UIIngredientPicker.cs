using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Raincrow.Maps;

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
    private List<TextMeshProUGUI> m_TextPool = new List<TextMeshProUGUI>();
    private List<Button> m_ButtonPool = new List<Button>();
    private List<InventoryItems> m_Items;
    private TextMeshProUGUI m_NoneTextButton;

    private int m_Selected;
    int m_PreviousSelected;

    public static System.Action<InventoryItems, IngredientType> onSelectIngredient { get; set; }

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


        //none button
        m_NoneTextButton = Instantiate(m_ItemPrefab, m_Container.transform);
        m_NoneTextButton.text = "None";
        m_NoneTextButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            SetSelected(-1);
        });
        m_NoneTextButton.gameObject.SetActive(true);

        Spellcasting.OnSpellCast += Spellcasting_OnSpellCast;
    }

    private void Spellcasting_OnSpellCast(IMarker arg1, SpellDict arg2, Result arg3)
    {
        //resets the UI after the casting is completed
        m_IngredientType = IngredientType.none;
    }

    private void OnClickConfirm()
    {
        if (m_Selected < 0)
            onSelectIngredient?.Invoke(null, m_IngredientType);
        else if (m_Selected < m_Items.Count)
            onSelectIngredient?.Invoke(m_Items[m_Selected], m_IngredientType);

        Close();
    }

    private void OnClickCancel()
    {
        SetSelected(m_PreviousSelected);
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
        
        for (int i = 0; i < m_TextPool.Count; i++)
        {
            m_TextPool[i].gameObject.SetActive(false);
        }

        switch (type)
        {
            case IngredientType.gem: m_Items = PlayerDataManager.playerData.ingredients.gems; break;
            case IngredientType.herb: m_Items = PlayerDataManager.playerData.ingredients.herbs; break;
            case IngredientType.tool: m_Items = PlayerDataManager.playerData.ingredients.tools; break;
            default: m_Items = new List<InventoryItems>(); break;
        }

        for (int i = m_Items.Count; i < m_TextPool.Count; i++)
        {
            m_TextPool[i].gameObject.SetActive(false);
        }

        int ii = 0;

        for (int i = 0; i < m_Items.Count; i++)
        {
            if (m_Items[i].count <= 0)
                continue;

            TextMeshProUGUI itemText;
            Button itemButton;

            if (i >= m_ButtonPool.Count)
            {
                m_TextPool.Add(Instantiate(m_ItemPrefab, m_Container.transform));
                m_ButtonPool.Add(m_TextPool[m_TextPool.Count - 1].GetComponent<Button>());
            }

            itemText = m_TextPool[ii];
            itemButton = m_ButtonPool[ii];
            ii++;

            itemText.text = $"{m_Items[i].name}({m_Items[i].count})";

            int aux = i;

            itemButton.onClick.RemoveAllListeners();
            itemButton.onClick.AddListener(() =>
            {
                SetSelected(aux);
            });

            if (selected == m_Items[i])
            {
                m_PreviousSelected = i;
                SetSelected(i);
            }

            itemButton.gameObject.SetActive(true);
        }

        if (selected == null)
        {
            m_PreviousSelected = -1;
            SetSelected(-1);
        }

        ReOpen();
    }

    public void Close()
    {
        m_InputRaycaster.enabled = false;
        LeanTween.cancel(m_TweenId);
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
        LeanTween.cancel(m_TweenId);
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

    private void SetSelected(int index)
    {
        if (m_Selected < 0)
            m_NoneTextButton.color = m_TextColor;
        else if (m_Selected < m_TextPool.Count)
            m_TextPool[m_Selected].color = m_TextColor;

        if (index < 0)
            m_NoneTextButton.color = m_SelectedColor;
        else if (index < m_TextPool.Count)
            m_TextPool[index].color = m_SelectedColor;

        m_Selected = index;
    }
}
