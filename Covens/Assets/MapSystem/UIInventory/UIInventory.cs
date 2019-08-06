using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInventory : MonoBehaviour
{
    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private GraphicRaycaster m_InputRaycaster;

    [Header("wheel")]
    [SerializeField] private UIInventoryWheel m_HerbsWheel;
    [SerializeField] private UIInventoryWheel m_ToolsWheel;
    [SerializeField] private UIInventoryWheel m_GemsWheel;

    [Header("Buttons")]
    [SerializeField] private Button m_CloseButton;
    [SerializeField] private Button m_ApothecaryButton;
    [SerializeField] private GameObject m_EmptyApothecary;

    public CanvasGroup inventoryCG;
    private static UIInventory m_Instance;
    public static UIInventory Instance
    {
        get
        {
            if (m_Instance == null)
                m_Instance = Instantiate(Resources.Load<UIInventory>("UIInventory"));
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
                return m_Instance.m_InputRaycaster.enabled;
        }
    }

    public static bool isInstantiated
    {
        get { return m_Instance != null; }
    }

    public UIInventoryWheel herbsWheel { get { return m_HerbsWheel; } }
    public UIInventoryWheel toolsWheel { get { return m_ToolsWheel; } }
    public UIInventoryWheel gemsWheel { get { return m_GemsWheel; } }

    private System.Action m_OnClickClose;

    private void Awake()
    {
        inventoryCG.alpha = 0;
        m_Canvas.enabled = false;
        m_InputRaycaster.enabled = false;
        m_CloseButton.onClick.AddListener(OnClickClose);
        m_ApothecaryButton.onClick.AddListener(OnClickApothecary);

        DownloadedAssets.OnWillUnloadAssets += OnWillUnloadAssets;
    }

    private void OnWillUnloadAssets()
    {
        if (isOpen)
            return;

        DownloadedAssets.OnWillUnloadAssets -= OnWillUnloadAssets;
        Destroy(this.gameObject);
    }

    public void Show(System.Action<UIInventoryWheelItem> onSelectItem, System.Action onClickClose, bool showApothecary, bool enableCloseButton)
    {
        m_OnClickClose = onClickClose;

        m_HerbsWheel.Setup("herb", PlayerDataManager.playerData.GetAllIngredients(IngredientType.herb), onSelectItem);
        m_ToolsWheel.Setup("tool", PlayerDataManager.playerData.GetAllIngredients(IngredientType.tool), onSelectItem);
        m_GemsWheel.Setup("gem", PlayerDataManager.playerData.GetAllIngredients(IngredientType.gem), onSelectItem);

        m_HerbsWheel.LockIngredient(null, 0);
        m_ToolsWheel.LockIngredient(null, 0);
        m_GemsWheel.LockIngredient(null, 0);
        
        m_ApothecaryButton.gameObject.SetActive(showApothecary && PlayerDataManager.playerData.energy != 0);

        if (m_ApothecaryButton.gameObject.activeSelf)
        {
            bool hasPotions = false;

            for(int i = 0; i < PlayerDataManager.playerData.inventory.consumables.Count; i++)
            {
                if (PlayerDataManager.playerData.inventory.consumables[i].count > 0)
                {
                    hasPotions = true;
                    break;
                }
            }

            m_ApothecaryButton.interactable = hasPotions;
            m_EmptyApothecary.gameObject.SetActive(hasPotions == false);
        }

        m_CloseButton.gameObject.SetActive(enableCloseButton);

        //if (resetIngredientPicker)
        //    ResetIngredientPicker();

        AnimateIn();
    }

    public void Close()
    {
        AnimateOut();

        //if (resetIngrPicker)
        ResetIngredientPicker();
    }

    public void ResetIngredientPicker()
    {
        m_HerbsWheel.ResetPicker();
        m_ToolsWheel.ResetPicker();
        m_GemsWheel.ResetPicker();
    }

    private void AnimateIn()
    {
        LeanTween.alphaCanvas(inventoryCG, 1f, 0.5f);
        m_GemsWheel.AnimIn1();
        m_ToolsWheel.AnimIn2();
        m_HerbsWheel.AnimIn3();

        m_HerbsWheel.enabled = true;
        m_ToolsWheel.enabled = true;
        m_GemsWheel.enabled = true;

        m_Canvas.enabled = true;
        m_InputRaycaster.enabled = true;
    }

    private void AnimateOut()
    {
        m_GemsWheel.ResetAnim1();
        m_ToolsWheel.ResetAnim2();
        m_HerbsWheel.ResetAnim3();
        LeanTween.alphaCanvas(inventoryCG, 0f, 0.3f).setOnComplete(() =>
       {
           m_HerbsWheel.enabled = false;
           m_ToolsWheel.enabled = false;
           m_GemsWheel.enabled = false;

           m_Canvas.enabled = false;
           m_InputRaycaster.enabled = false;
       });

    }

    private void OnClickClose()
    {
        Close();

        if (m_OnClickClose != null)
            m_OnClickClose?.Invoke();
    }

    private void OnClickApothecary()
    {
        this.Close();
		UICollectableInfo.Instance.Close ();
        UIApothecary.Instance.Show(
            null,
            () => //on returning from apothecary
            {
                this.AnimateIn();
            },
            null);
    }

    public void LockIngredients(string[] ingredients, float animDuration)
    {
        m_HerbsWheel.LockIngredient(null, 0);
        m_ToolsWheel.LockIngredient(null, 0);
        m_GemsWheel.LockIngredient(null, 0);

        IngredientType type;
        for (int i = 0; i < ingredients.Length; i++)
        {
            type = DownloadedAssets.GetCollectable(ingredients[i]).Type;

            if (type == IngredientType.herb)
                m_HerbsWheel.LockIngredient(ingredients[i], animDuration);
            else if (type == IngredientType.tool)
                m_ToolsWheel.LockIngredient(ingredients[i], animDuration);
            else if (type == IngredientType.gem)
                m_GemsWheel.LockIngredient(ingredients[i], animDuration);
        }
    }

    public void SetSelected(List<CollectableItem> items)
    {
        ResetIngredientPicker();

        if (items == null)
            return;

        IngredientData itemData;
        foreach (var _item in items)
        {
            if (string.IsNullOrEmpty(_item.id))
                continue;

            itemData = DownloadedAssets.GetCollectable(_item.id);

            if (string.IsNullOrEmpty(itemData.type))
                continue;

            switch (itemData.Type)
            {
                case IngredientType.herb: m_HerbsWheel.SetPicker(_item.id, _item.count); break;
                case IngredientType.tool: m_ToolsWheel.SetPicker(_item.id, _item.count); break;
                case IngredientType.gem:  m_GemsWheel.SetPicker(_item.id, _item.count); break;
            }
        }
    }
}
