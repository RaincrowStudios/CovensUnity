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
    [SerializeField] private Transform m_PickerObj;
    [SerializeField] private TextMeshProUGUI m_PickerAmount;

    [Header("settings")]
    [SerializeField] private bool m_ClampRotation = true;
    [SerializeField] private float m_Spacing;
    [SerializeField] private float m_Sensivity = 0.01f;
    [SerializeField] private int m_MaxItems = 12;
    [SerializeField] private UIInventoryWheelItem[] m_PrearrangedItems;

    private List<UIInventoryWheelItem> m_Items; //all the instantiate wheelItems
    private List<InventoryItems> m_Inventory; //all the inventory items available in the wheel
    private System.Action<UIInventoryWheelItem> m_OnSelectItem;
    private float m_Angle; //current wheel rotation
    private bool m_IsDragging = false;
    private float m_LastY;
    private float m_AspectRatio;
    private int m_IntertiaTween;
    private int m_FocusTweenId;
    private float m_UpperBorder;
    private float m_LowerBorder;
    private bool m_IngredientLocked;

    private void Awake()
    {
        if (m_ItemPrefab)
            m_ItemPrefab.gameObject.SetActive(false);

        m_PickerObj.gameObject.SetActive(false);

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

        m_PickerObj.SetAsLastSibling();
    }

    private void OnEnable()
    {
        m_AspectRatio = 720f / Screen.height;
        GetComponent<Canvas>().enabled = true;
        GetComponent<GraphicRaycaster>().enabled = true;
    }

    private void OnDisable()
    {
        GetComponent<Canvas>().enabled = false;
        GetComponent<GraphicRaycaster>().enabled = false;
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

        m_PickerObj.rotation = Quaternion.identity;
    }

    private void ManageItems()
    {
        if (m_PrearrangedItems.Length > 0)
            return;

        if (m_Angle < m_LowerBorder)
        {
            int nextIndex = m_Items[m_Items.Count - 1].index + 1;

            //move first element to the end of the list
            UIInventoryWheelItem aux = m_Items[0];
            m_Items.RemoveAt(0);
            m_Items.Add(aux);

            //set it up with the next item data
            int ingrIndex = (int)Mathf.Repeat(nextIndex, m_Inventory.Count - 1);
            aux.transform.localEulerAngles = new Vector3(0, 0, nextIndex * m_Spacing);
            aux.Setup(m_Inventory[ingrIndex], this, nextIndex);

            m_LowerBorder -= m_Spacing;
            m_UpperBorder -= m_Spacing;
        }
        else if (m_Angle > m_UpperBorder)
        {
            int previousIndex = m_Items[0].index - 1;

            //move last element to the beginning
            UIInventoryWheelItem aux = m_Items[m_Items.Count - 1];
            m_Items.RemoveAt(m_Items.Count - 1);
            m_Items.Insert(0, aux);

            //set it up with the next item data
            int ingrIndex = (int)Mathf.Repeat(previousIndex, m_Inventory.Count - 1);
            aux.transform.localEulerAngles = new Vector3(0, 0, previousIndex * m_Spacing);
            aux.Setup(m_Inventory[ingrIndex], this, previousIndex);

            m_LowerBorder += m_Spacing;
            m_UpperBorder += m_Spacing;
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
                m_Items[i].Setup(items[(int)Mathf.Repeat(i - items.Count, items.Count - 1)], this, i);

            m_Items[i].transform.localEulerAngles = new Vector3(0, 0, i * m_Spacing);
            m_Items[i].gameObject.SetActive(true);
        }

        m_Angle = -m_Items.Count * 0.5f * m_Spacing;
        transform.eulerAngles = new Vector3(0, 0, m_Angle);
        m_LowerBorder = m_Angle - m_Spacing;
        m_UpperBorder = m_Angle + m_Spacing;
    }

    public void SelectItem(UIInventoryWheelItem wheelItem)
    {
        m_IsDragging = false;
        LeanTween.cancel(m_IntertiaTween);
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

    public void SetPicker(Transform reference, int amount)
    {
        if (amount > 0)
        {
            m_PickerAmount.text = amount.ToString();
            m_PickerObj.position = reference.position;
            m_PickerObj.gameObject.SetActive(true);
        }
        else
        {
            m_PickerObj.gameObject.SetActive(false);
        }
    }

    public void ResetPicker()
    {
        m_PickerObj.gameObject.SetActive(false);
    }

    public void LockIngredient(InventoryItems item)
    {
        m_IngredientLocked = item != null;
        int indexOf = m_Inventory.IndexOf(item);
        if (indexOf >= 0 && indexOf < m_Inventory.Count)
        {
            Focus(indexOf, () =>
            {
                for (int i = 0; i < m_Items.Count; i++)
                {
                    if (m_Items[i].item == item)
                    {
                        SelectItem(m_Items[i]);
                        break;
                    }
                }
            });
        }
    }

    public void Focus(int index, System.Action onComplete)
    {
        LeanTween.cancel(m_IntertiaTween);
        LeanTween.cancel(m_FocusTweenId);
        m_FocusTweenId = LeanTween.value(m_Angle, ClampRotation(-index * m_Spacing), 1f)
            .setEaseOutCubic()
            .setOnUpdate((float t) =>
            {
                m_Angle = t;
                transform.eulerAngles = new Vector3(0, 0, m_Angle);
                ManageItems();
            })
            .setOnComplete(onComplete)
            .uniqueId;
    }
}
