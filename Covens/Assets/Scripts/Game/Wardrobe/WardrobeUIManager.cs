using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Reflection;

public class WardrobeUIManager : MonoBehaviour
{
    public static WardrobeUIManager Instance { get; set; }

    public Animator anim;
    public GameObject wardobeContainer;
    public Transform container;
    public GameObject prefab;
    public int spawnCount = 20;
    public Text subtitle;
    List<WardrobeItemButton> items = new List<WardrobeItemButton>();
    public CanvasGroup[] buttons;
    public Transform highlight;

    public Sprite[] itemIcons;

    public Sprite hat_1;
    public Sprite dress_1;
    public Sprite highBoots_1;
    public Sprite boots_1;
    public Sprite shoulderTattoo_1;
    public Sprite chestTattoo_1;
    public Sprite choker_1;
    public Sprite rightHandBracelet_1;
    public Sprite leftHandBracelet_1;
    public Sprite gloves_1;
    public Sprite censor_1;
    public Sprite[] hair_1;

    List<string> wardrobeItems = new List<string>() {
        "pants",
        "torso",
        "hat",
        "hair",
        "neck",
        "tattoo",
        "hand",
        "wrist",
        "censor",
        "feet",
    };


    void Awake()
    {
        Instance = this;
    }

    public void ShowWardrobe()
    {
        //UIStateManager.Instance.CallWindowChanged(false);
        SoundManagerOneShot.Instance.MenuSound();
        wardobeContainer.SetActive(true);
        anim.SetBool("animate", true);
    }


    public void HideWardrobe()
    {
        anim.SetBool("animate", false);
        Invoke("DisableDelay", 1f);
        SoundManagerOneShot.Instance.MenuSound();
        //UIStateManager.Instance.CallWindowChanged(true);

    }

    void DisableDelay()
    {
        wardobeContainer.SetActive(false);

    }

    public void Reset()
    {
        filter(EnumWardrobeCategory.None, false);
        highlight.gameObject.SetActive(false);
        ResetAlpha();
    }
    void ResetAlpha()
    {
        foreach (var item in buttons)
        {
            item.alpha = .5f;
        }
    }

    void Start()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            var g = Utilities.InstantiateObject(prefab, container);
            var WID = g.GetComponent<WardrobeItemButton>();
            WardrobeData data = new WardrobeData();
            data.itemName = wardrobeItems[UnityEngine.Random.Range(0, wardrobeItems.Count)];
            data.type = (EnumWardrobeCategory)Enum.Parse(typeof(EnumWardrobeCategory), data.itemName);
            //WID.data = data;
            items.Add(WID);
            WID.icon.sprite = itemIcons[UnityEngine.Random.Range(0, itemIcons.Length - 1)];
        }
        Reset();
    }

    public void filter(EnumWardrobeCategory type, bool isSelected = true)
    {
        if (!isSelected)
        {
            foreach (var item in items)
            {
                item.gameObject.SetActive(true);
            }
            subtitle.text = "";
            return;
        }
        subtitle.text = type.ToString();
        highlight.gameObject.SetActive(true);
        /*int index = wardrobeItems.IndexOf (type.ToString ()); 
		if (index < 3) {
			highlight.transform.localEulerAngles = new Vector3 (0, 0, 30 * index);
		} else if (index >= 3 && index < 8) {
			index++;
			highlight.transform.localEulerAngles = new Vector3 (0, 0, 30 * index);
		} else {
			index= index + 2;
			highlight.transform.localEulerAngles = new Vector3 (0, 0, 30 * index);
		}
		Debug.Log (index);
		if (type == EnumWardrobeCategory.Hat || type == EnumWardrobeCategory.Hand) {
			foreach (WardrobeItemData item in items) { 
				if (item.data.type != EnumWardrobeCategory.Hand && item.data.type != EnumWardrobeCategory.Hand) {
					item.gameObject.SetActive (false);
				} else {
					item.gameObject.SetActive (true);
				}

			}
		} else {
			foreach (WardrobeItemData item in items) { 
				if (item.data.type != type) {
					item.gameObject.SetActive (false);
				} else {
					item.gameObject.SetActive (true);
				}
			}
		}*/
        ResetAlpha();
    }
}

public class WardrobeData
{
    public EnumWardrobeCategory type;
    public string itemName;
}
