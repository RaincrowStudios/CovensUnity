using UnityEngine;
using System.Collections;

public class OnCreatrixGift : MonoBehaviour
{
    public static void HandleEvent(WSData data)
    {
        if (data.command == "character_creatrix_add")
        {
            ManageCreatrixGift.Instance.CreatrixItemAdd(data);
        }
        else
        {
            ManageCreatrixGift.Instance.CreatrixShopAdd(data);
        }
    }
}