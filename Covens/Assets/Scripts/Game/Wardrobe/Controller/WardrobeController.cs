using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class WardrobeController : Patterns.SingletonClass<WardrobeController>
{


    public List<WardrobeItemModel> GetAvailableItens(EnumGender eGender)
    {
        return ItemDB.Instance.GetItens(eGender);
    }

    public List<WardrobeItemModel> GetAvailableItens(EnumEquipmentSlot eSlot, EnumGender eGender)
    {
        
        List<WardrobeItemModel> vItemList = new List<WardrobeItemModel>();
        WardrobeItemModel[] vAvailableList = ItemDB.Instance.Itens;// GetAvailableItens(eGender);
        for (int i = 0; i < vAvailableList.Length; i++)
        {
            if (vAvailableList[i].GenderEnum == eGender && vAvailableList[i].EquipmentSlotEnum == eSlot )
            {
                vItemList.Add(vAvailableList[i]);
            }
        }
        return vItemList;
    }
    public List<WardrobeItemModel> GetAvailableItens(EnumWardrobeCategory eCategories, EnumGender eGender)
    {
        List<WardrobeItemModel> vItemList = new List<WardrobeItemModel>();
        WardrobeItemModel[] vAvailableList = ItemDB.Instance.Itens;
        for (int i = 0; i < vAvailableList.Length; i++)
        {
            if (vAvailableList[i].GenderEnum == eGender && (vAvailableList[i].WardrobeCategory & eCategories) != 0)
            {
                vItemList.Add(vAvailableList[i]);
            }
        }
        return vItemList;
    }

}