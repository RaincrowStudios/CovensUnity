using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class WardrobeController : Patterns.SingletonClass<WardrobeController>
{

    public List<WardrobeItemModel> AvailableItens
    {
        get
        {
            // TODO: get the right itens from the server
            return ItemDB.Instance.GetItens(EnumGender.Female);
        }
    }


    public List<WardrobeItemModel> GetAvailableItens(EnumEquipmentSlot eSlot)
    {
        List<WardrobeItemModel> vItemList = new List<WardrobeItemModel>();
        List<WardrobeItemModel> vAvailableList = AvailableItens;
        for (int i = 0; i < vAvailableList.Count; i++)
        {
            if (vAvailableList[i].EquipmentSlotEnum == eSlot)
            {
                vItemList.Add(vAvailableList[i]);
            }
        }
        return vItemList;
    }
    public List<WardrobeItemModel> GetAvailableItens(EnumWardrobeCategory eCategories)
    {
        List<WardrobeItemModel> vItemList = new List<WardrobeItemModel>();
        List<WardrobeItemModel> vAvailableList = AvailableItens;
        for (int i = 0; i < vAvailableList.Count; i++)
        {
            if ((vAvailableList[i].WardrobeCategory & eCategories) != 0)
            {
                vItemList.Add(vAvailableList[i]);
            }
        }
        return vItemList;
    }

}