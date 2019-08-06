using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIInventoryItemPicker : MonoBehaviour
{
    //[SerializeField] private TextMeshProUGUI m_AmountText;
    
    public UIInventoryWheelItem attachedItem { get; set; }
    public int amount { get; private set; }
	public Image Picker;

	//public void Start () {
	//	Picker = this.GetComponent<Image> ();
	//}
    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }

    public void SetFillAmount(int amount)
    {
        this.amount = amount;
        //m_AmountText.text = amount.ToString();
		Picker.fillAmount = (float)amount / 5;
    }

    public void Setup(UIInventoryWheelItem reference, int amount)
    {
        SetFillAmount(amount);
        Setup(reference);
    }


    public void Setup(UIInventoryWheelItem reference)
    {
        attachedItem = reference;
        transform.SetParent(reference.iconReference);
        transform.localPosition = Vector3.zero;
    }
}
