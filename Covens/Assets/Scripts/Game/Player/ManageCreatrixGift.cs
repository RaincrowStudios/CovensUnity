using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ManageCreatrixGift : MonoBehaviour
{
    public static ManageCreatrixGift Instance { get; set; }
    public Transform container;
    public GameObject creatrixItem;
    public GameObject creatrixShop;
    public Sprite bundle1;
    public Sprite bundle2;
    public Sprite bundle3;
    void Awake()
    {
        Instance = this;
    }

    public void CreatrixItemAdd(WSData data)
    {
        foreach (Transform item in container)
        {
            Destroy(item.gameObject);
        }
        var k = Utilities.InstantiateUI(creatrixItem, container);
        string s = "";
        for (int i = 0; i < data.creatrix.type.Length; i++)
        {
            if (data.creatrix.type[i] == "silver")
            {
                PlayerDataManager.playerData.silver += data.creatrix.amount[i];
                PlayerManagerUI.Instance.UpdateDrachs();
            }
            else if (data.creatrix.type[i] == "gold")
            {
                PlayerDataManager.playerData.gold += data.creatrix.amount[i];
                PlayerManagerUI.Instance.UpdateDrachs();
            }
            else if (data.creatrix.type[i] == "energy")
            {
                PlayerDataManager.playerData.energy += data.creatrix.amount[i];
                PlayerManagerUI.Instance.UpdateEnergy();
            }

            s += $"{data.creatrix.amount[i].ToString()} {data.creatrix.type[i]} | ";
        }

        s = s.Substring(0, s.Length - 2);
        k.transform.GetChild(6).GetComponent<TextMeshProUGUI>().text = s;
    }

    public void CreatrixShopAdd(WSData data)
    {
        foreach (Transform item in container)
        {
            Destroy(item.gameObject);
        }
        var k = Utilities.InstantiateUI(creatrixShop, container);
        var p = k.transform.GetChild(3);
        var img = p.GetChild(5).GetComponent<Image>();

        if (data.creatrix.id == "bundle_abondiasBest")
        {
            img.sprite = bundle1;
        }
        else if (data.creatrix.id == "bundle_sapphosChoice")
        {
            img.sprite = bundle2;
        }
        else if (data.creatrix.id == "bundle_hermeticCollection")
        {
            img.sprite = bundle3;
        }
        else
        {
            DownloadedAssets.GetSprite(data.creatrix.id, img);
        }

        p.GetChild(6).GetComponent<TextMeshProUGUI>().text = LocalizeLookUp.GetStoreTitle(data.creatrix.id);
        p.GetChild(7).GetComponent<Button>().onClick.AddListener(() =>
        {
            Debug.Log("reseting stuff");
            GameResyncHandler.ResyncGame();
            Destroy(k);
        });
    }
}
