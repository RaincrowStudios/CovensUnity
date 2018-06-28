using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumableItemModel
{

    public ConsumableItem m_Data;
    private EnumConsumable m_EnumConsumable;


    public string ID
    {
        get{ return m_Data.id; }
    }
    public int Count
    {
        get { return m_Data.count; }
    }
    public EnumConsumable ConsumableType
    {
        get { return m_EnumConsumable; }
    }

    public ConsumableItemModel(ConsumableItem pData)
    {
        m_Data = pData;
        m_EnumConsumable = ParseType(pData);
    }

    public void Consume(int iAmount)
    {
        m_Data.count = Count - iAmount;
    }

    EnumConsumable ParseType(ConsumableItem pData)
    {
        if (pData.id == "consumable_energyPotion100")
            return EnumConsumable.Energy;
        if (pData.id == "consumable_wisdomBooster1")
            return EnumConsumable.Wisdom;
        if (pData.id == "consumable_aptitudeBooster1")
            return EnumConsumable.Aptitude;
        return EnumConsumable.None;
    }

}
