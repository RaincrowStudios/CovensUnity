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

    private System.Action<UIInventoryWheelItem> m_OnSelectItem;
    public System.Action m_OnClickClose;

    private void Awake()
    {
        m_Canvas.enabled = false;
        m_InputRaycaster.enabled = false;
        m_CloseButton.onClick.AddListener(OnClickClose);
    }

    public void Show(System.Action<UIInventoryWheelItem> onSelectItem, System.Action onClickClose, bool showApothecary)
    {
        m_OnSelectItem = onSelectItem;
        m_OnClickClose = onClickClose;

        m_HerbsWheel.Setup(PlayerDataManager.playerData.ingredients.herbs, onSelectItem);
        m_ToolsWheel.Setup(PlayerDataManager.playerData.ingredients.tools, onSelectItem);
        m_GemsWheel.Setup(PlayerDataManager.playerData.ingredients.gems, onSelectItem);

        m_HerbsWheel.enabled = true;
        m_ToolsWheel.enabled = true;
        m_GemsWheel.enabled = true;

        AnimateIn();
    }

    public void Close()
    {
        m_HerbsWheel.enabled = false;
        m_ToolsWheel.enabled = false;
        m_GemsWheel.enabled = false;

        AnimateOut();
    }

    private void AnimateIn()
    {
        m_Canvas.enabled = true;
        m_InputRaycaster.enabled = true;
    }

    private void AnimateOut()
    {
        m_Canvas.enabled = false;
        m_InputRaycaster.enabled = false;
    }

    private void OnClickClose()
    {
        if (m_OnClickClose != null)
            m_OnClickClose?.Invoke();
        else
            Close();
    }
}
