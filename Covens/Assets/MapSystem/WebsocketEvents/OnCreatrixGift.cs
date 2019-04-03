using UnityEngine;
using System.Collections;

public class OnCreatrixGift : MonoBehaviour
{
    public static void HandleEvent(WSData data)
    {
        if (data.creatrix.id != null)
        {
            ManageCreatrixGift.Instance.CreatrixShopAdd(data);
        }
        else
            ManageCreatrixGift.Instance.CreatrixItemAdd(data);

    }
}