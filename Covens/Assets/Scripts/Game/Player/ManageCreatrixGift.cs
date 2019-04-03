using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ManageCreatrixGift : MonoBehaviour
{
    public static ManageCreatrixGift Instance { get; set; }
    public Transform container;
    public GameObject creatrixItem;
    public GameObject creatrixShop;

    void Awake()
    {
        Instance = this;
    }

    public void CreatrixItemAdd(WSData data)
    {
        var pData = PlayerDataManager.playerData.ingredients;
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

            s += $"{data.creatrix.amount[i].ToString()} {data.creatrix.type[i]} |";
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
        p.GetChild(6).GetComponent<TextMeshProUGUI>().text = DownloadedAssets.storeDict[data.creatrix.id].title;
        p.GetChild(7).GetComponent<Button>().onClick.AddListener(() =>
        {
            PlayerManager.Instance.initStart();
            Destroy(k);
        });
    }
}
