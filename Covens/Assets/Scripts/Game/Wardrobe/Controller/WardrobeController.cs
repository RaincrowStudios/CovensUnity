using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class WardrobeController : Patterns.SingletonClass<WardrobeController>
{

    public List<WardrobeItemModel> Itens
    {
        get
        {
            return ItemDB.Instance.GetItens(EnumGender.Female);
        }
    }
}