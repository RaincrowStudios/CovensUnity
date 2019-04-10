using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class UIInventoryWheel : MonoBehaviour
{
    [SerializeField] private Transform m_StartReference;
    [SerializeField] private Transform m_EndReference;
    [SerializeField] private UIInventoryWheelItem m_ItemPrefab;
    [SerializeField] private UIInventoryItemPicker m_PickerPrefab;

    [Header("settings")]
    [SerializeField] private bool m_ClampRotation = true;
    [SerializeField] private float m_Spacing;
    [SerializeField] private float m_Sensivity = 0.01f;
    [SerializeField] private int m_MaxItems = 12;
    [SerializeField] private UIInventoryWheelItem[] m_PrearrangedItems;

    private SimplePool<UIInventoryItemPicker> m_PickerPool;
    private List<UIInventoryItemPicker> m_Pickers = new List<UIInventoryItemPicker>();

    private List<UIInventoryWheelItem> m_Items; //all the instantiate wheelItems
    private List<InventoryItems> m_Inventory; //all the inventory items available in the wheel
    private System.Action<UIInventoryWheelItem> m_OnSelectItem;
    private UIInventoryWheelItem m_SelectedItem;
    private float m_Angle; //current wheel rotation
    private bool m_IsDragging = false;
    private float m_LastY;
    private float m_AspectRatio;
    private int m_IntertiaTween;
    private int m_FocusTweenId;
    private float m_UpperBorder;
    private float m_LowerBorder;
    private bool m_IngredientLocked;
    private InventoryItems m_PickerItemRef;
    private int m_PickerAmountRef;

    public Transform o_StartReference;
    public Transform o_EndReference;

    private void Awake()
    {
        if (m_ItemPrefab)
            m_ItemPrefab.gameObject.SetActive(false);

        m_PickerPool = new SimplePool<UIInventoryItemPicker>(m_PickerPrefab, 1);

        //init the wheel with the predefined items and prefabs
        if (m_PrearrangedItems.Length > 0)
        {
            m_Items = new List<UIInventoryWheelItem>();
            UIInventoryWheelItem wheelItem;
            InventoryItems playerItem;

            for (int i = 0; i < m_PrearrangedItems.Length; i++)
            {
                wheelItem = m_PrearrangedItems[i];
                playerItem = PlayerDataManager.playerData.ingredients.GetIngredient(wheelItem.m_ItemId);
                wheelItem.Setup(playerItem, this, i);
                m_Items.Add(wheelItem);
            }
        }
        else //pre instantiate the wheelItems
        {
            m_Items = new List<UIInventoryWheelItem>();
            for (int i = 0; i < m_MaxItems; i++)
            {
                m_Items.Add(Instantiate(m_ItemPrefab, this.transform));
                m_Items[i].name = name + "[" + i + "]";
            }
        }
    }

    private void OnEnable()
    {
        m_AspectRatio = 720f / Screen.height;
        foreach (var item in m_Items)
        {
            item.Refresh();
        }
    }

    private void LateUpdate()
    {
        if (Input.GetMouseButtonDown(0) && m_IngredientLocked == false)
        {
            if (LeanTween.isTweening(m_FocusTweenId) == false)
            {
                if (IsMouseInsideCircle(transform, m_StartReference, m_EndReference))
                {
                    m_IsDragging = true;
                    m_LastY = Input.mousePosition.y;
                    LeanTween.cancel(m_IntertiaTween);
                }
            }
        }

        //handle inertia
        if (Input.GetMouseButtonUp(0) && m_IsDragging)
        {
            m_IsDragging = false;

            if (LeanTween.isTweening(m_FocusTweenId) == false)
            {
                float delta = ClampRotation((Input.mousePosition.y - m_LastY) * m_AspectRatio * m_Sensivity * 13);
                m_IntertiaTween = LeanTween.value(m_Angle, ClampRotation(m_Angle + delta), 1f)
                    .setEaseOutCubic()
                    .setOnUpdate((float t) =>
                    {
                        m_Angle = t;
                        transform.eulerAngles = new Vector3(0, 0, m_Angle);
                        ManageItems();
                    })
                    .uniqueId;
            }
        }

        //handle draging
        if (m_IsDragging)
        {
            float delta = (Input.mousePosition.y - m_LastY) * m_AspectRatio * m_Sensivity;
            m_LastY = Input.mousePosition.y;
            m_Angle = ClampRotation(m_Angle + delta);
            transform.eulerAngles = new Vector3(0, 0, m_Angle);
            ManageItems();
        }
    }

    private void ManageItems()
    {
        foreach (UIInventoryItemPicker _picker in m_Pickers)
            _picker.transform.rotation = Quaternion.identity;

        if (m_PrearrangedItems.Length > 0)
            return;

        if (m_Angle < m_LowerBorder)
        {
            int nextIndex = m_Items[m_Items.Count - 1].index + 1;

            //move first element to the end of the list
            UIInventoryWheelItem aux = m_Items[0];

            foreach (UIInventoryItemPicker _picker in m_Pickers)
            {
                if (_picker.attachedItem == aux)
                {
                    m_PickerPool.Despawn(_picker);
                    m_Pickers.Remove(_picker);
                    break;
                }
            }

            m_Items.RemoveAt(0);
            m_Items.Add(aux);

            //set it up with the next item data
            int ingrIndex = (int)Mathf.Repeat(nextIndex, m_Inventory.Count);
            aux.transform.localEulerAngles = new Vector3(0, 0, nextIndex * m_Spacing);
            aux.Setup(m_Inventory[ingrIndex], this, nextIndex);

            if (aux.inventoryItem == m_PickerItemRef)
            {
                aux.SetAmount(aux.inventoryItem.count - m_PickerAmountRef);

                UIInventoryItemPicker picker = m_PickerPool.Spawn();
                picker.Setup(aux, m_PickerAmountRef);
                m_Pickers.Add(picker);
            }

            m_LowerBorder -= m_Spacing;
            m_UpperBorder -= m_Spacing;

            ManageItems();
        }
        else if (m_Angle > m_UpperBorder)
        {
            int previousIndex = m_Items[0].index - 1;

            //move last element to the beginning
            UIInventoryWheelItem aux = m_Items[m_Items.Count - 1];

            foreach (UIInventoryItemPicker _picker in m_Pickers)
            {
                if (_picker.attachedItem == aux)
                {
                    m_PickerPool.Despawn(_picker);
                    m_Pickers.Remove(_picker);
                    break;
                }
            }

            m_Items.RemoveAt(m_Items.Count - 1);
            m_Items.Insert(0, aux);

            //set it up with the next item data
            int ingrIndex = (int)Mathf.Repeat(previousIndex, m_Inventory.Count);
            aux.transform.localEulerAngles = new Vector3(0, 0, previousIndex * m_Spacing);
            aux.Setup(m_Inventory[ingrIndex], this, previousIndex);

            if (aux.inventoryItem == m_PickerItemRef)
            {
                aux.SetAmount(aux.inventoryItem.count - m_PickerAmountRef);

                UIInventoryItemPicker picker = m_PickerPool.Spawn();
                picker.Setup(aux, m_PickerAmountRef);
                m_Pickers.Add(picker);
            }

            m_LowerBorder += m_Spacing;
            m_UpperBorder += m_Spacing;

            ManageItems();
        }
    }

    public void Setup(List<InventoryItems> items, System.Action<UIInventoryWheelItem> onSelectItem)
    {
        m_Inventory = items;
        m_OnSelectItem = onSelectItem;

        if (m_PrearrangedItems.Length > 0) //the items have already been set up on Awake
            return;

        //setup the initial items and reset the values
        //todo: when reopening, go back to the previous state
        for (int i = 0; i < m_Items.Count; i++)
        {
            if (i < items.Count)
                m_Items[i].Setup(items[i], this, i);
            else
                m_Items[i].Setup(items[(int)Mathf.Repeat(i - items.Count, items.Count)], this, i);

            m_Items[i].transform.localEulerAngles = new Vector3(0, 0, i * m_Spacing);
            m_Items[i].gameObject.SetActive(true);
        }

        m_Angle = -m_Items.Count * 0.5f * m_Spacing;
        transform.eulerAngles = new Vector3(0, 0, m_Angle);
        m_LowerBorder = m_Angle - m_Spacing;
        m_UpperBorder = m_Angle + m_Spacing;
        m_SelectedItem = null;
    }

    public void SelectItem(UIInventoryWheelItem wheelItem)
    {
        m_IsDragging = false;
        LeanTween.cancel(m_IntertiaTween);
        m_SelectedItem = wheelItem;
        m_OnSelectItem?.Invoke(wheelItem);
    }

    private float ClampRotation(float angle)
    {
        if (m_ClampRotation)
        {
            return Mathf.Clamp(
                angle,
                -m_Inventory.Count * m_Spacing,
                0
            );
        }
        else
        {
            return angle;
        }
    }

    public static bool IsMouseInsideCircle(Transform centerRef, Transform minRadiusRef, Transform maxRadiusRef)
    {
        Camera cam = Camera.main;
        Vector2 mousePos = Input.mousePosition;
        Vector2 ccenterCenter = centerRef.position;
        Vector2 minRadius = minRadiusRef.position;
        Vector2 maxRadius = maxRadiusRef.position;

        float pos = Mathf.Sqrt(Mathf.Pow(mousePos.x - ccenterCenter.x, 2) + Mathf.Pow(mousePos.y - ccenterCenter.y, 2));
        float min = Mathf.Sqrt(Mathf.Pow(minRadius.x - ccenterCenter.x, 2) + Mathf.Pow(minRadius.y - ccenterCenter.y, 2));
        float max = Mathf.Sqrt(Mathf.Pow(maxRadius.x - ccenterCenter.x, 2) + Mathf.Pow(maxRadius.y - ccenterCenter.y, 2));

        return pos >= min && pos < max;
    }

    public void SetPicker(UIInventoryWheelItem reference, int amount)
    {
        if (amount > 0)
        {
            m_PickerItemRef = reference.inventoryItem;
            m_PickerAmountRef = amount;

            if (m_Pickers.Count == 0)
                m_Pickers.Add(m_PickerPool.Spawn());

            foreach (UIInventoryItemPicker _picker in m_Pickers)
                _picker.Setup(reference, amount);
        }
        else
        {
            ResetPicker();
        }
    }

    public void ResetPicker()
    {
        m_PickerItemRef = null;
        m_PickerAmountRef = 0;
        m_PickerPool.DespawnAll();
        m_Pickers.Clear();
    }

    public void LockIngredient(InventoryItems item, float animDuration)
    {
        m_IngredientLocked = item != null;

        if (!m_IngredientLocked)
        {
            m_SelectedItem?.SetIngredientPicker(0);
            m_SelectedItem = null;
            ResetPicker();
            return;
        }

        for (int j = 0; j < m_Inventory.Count; j++)
        {
            if (m_Inventory[j] == item)
            {
                m_PickerItemRef = item;

                //if the wheel was preset, there is no guarantee the wheel is ordered the same way as the inventory
                if (m_PrearrangedItems.Length > 0)
                {
                    for (int i = 0; i < m_Items.Count; i++)
                    {
                        if (m_Items[i].inventoryItem == item)
                        {
                            Focus(i, animDuration, null);
                            m_SelectedItem = m_Items[i];
                            m_SelectedItem.SetIngredientPicker(1);
                            return;
                        }
                    }
                }
                else
                {
                    Focus(j, animDuration, () =>
                    {
                        for (int i = 0; i < m_Items.Count; i++)
                        {
                            if (m_Items[i].inventoryItem == item)
                            {
                                m_SelectedItem = m_Items[i];
                                m_SelectedItem.SetIngredientPicker(1);
                                return;
                            }
                        }
                    });

                }
            }
        }
    }

    public void Focus(int index, float animDuration, System.Action onItemInscreen)
    {
        LeanTween.cancel(m_IntertiaTween);
        LeanTween.cancel(m_FocusTweenId);

        float targetAngle = ClampRotation(-index * m_Spacing);
        float diff = targetAngle - m_Angle;

        float validAngle = m_Spacing * (m_MaxItems / 2f);

        m_FocusTweenId = LeanTween.value(m_Angle, targetAngle, animDuration)
            .setEaseOutElastic()
            .setOnUpdate((float t) =>
            {
                m_Angle = t;
                transform.eulerAngles = new Vector3(0, 0, m_Angle);
                ManageItems();

                if (onItemInscreen != null && Mathf.Abs(targetAngle - m_Angle) < validAngle)
                {
                    onItemInscreen.Invoke();
                    onItemInscreen = null;
                }
            })
            .setOnComplete(onItemInscreen)
            .uniqueId;
    }
    public void AnimIn1()
    {
        LeanTween.moveX(gameObject, o_StartReference.position.x, 0.001f);
        LeanTween.moveX(gameObject, o_EndReference.position.x, 0.5f).setEase(LeanTweenType.easeOutCubic);
    }
    public void AnimIn2()
    {
        LeanTween.moveX(gameObject, o_StartReference.position.x, 0.001f);
        LeanTween.moveX(gameObject, o_EndReference.position.x, 0.6f).setEase(LeanTweenType.easeOutCubic);
    }
    public void AnimIn3()
    {
        LeanTween.moveX(gameObject, o_StartReference.position.x, 0.001f);
        LeanTween.moveX(gameObject, o_EndReference.position.x, 0.7f).setEase(LeanTweenType.easeOutCubic);
    }
    public void ResetAnim1()
    {
        LeanTween.moveX(gameObject, o_StartReference.position.x, 0.4f);
    }
    public void ResetAnim2()
    {
        LeanTween.moveX(gameObject, o_StartReference.position.x, 0.3f);
    }
    public void ResetAnim3()
    {
        LeanTween.moveX(gameObject, o_StartReference.position.x, 0.2f);
    }
}
