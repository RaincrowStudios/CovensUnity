using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InventoryInfo : MonoBehaviour
{
	public static InventoryInfo Instance { get; set;}
	public GameObject info;
	public Image icon;
	public Text displayName;
	public Text rarity;
	public Text desc;

    //	public Text hint;
    bool isOn= false;

	void Awake()
	{
		Instance = this;
	}

    public void Show(string id, Sprite sp)
    {
        if (id == "null")
            return;

        IngredientData ingredient = DownloadedAssets.GetCollectable(id);

        displayName.text = LocalizeLookUp.GetCollectableName(id);
        desc.text = LocalizeLookUp.GetCollectableDesc(id);

        string r;
        if (ingredient.rarity == 1)
        {
            r = "Common";
        }
        else if (ingredient.rarity == 2)
        {
            r = "Less Common";
        }
        else if (ingredient.rarity == 3)
        {
            r = "Rare";
        }
        else if (ingredient.rarity == 4)
        {
            r = "Exotic";
        }
        else
        {
            r = "?";
        }

        rarity.text = r;
        icon.sprite = sp;
        info.SetActive(true);
    }

	void Update()
	{
		if(isOn && Input.GetMouseButtonUp(0)){
			info.SetActive (false);
			isOn = false;
		}
    }
}

