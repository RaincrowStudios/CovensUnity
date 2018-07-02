using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryAPI
{
    /*Expected
    {
    "consumable"
    }
    */
    public static void Consume(string sItemID, int iAmount, Action<string> pSuccess, Action<string> pError)
    {
        var pData = new Inventory_Consume();
        pData.consumable = sItemID;
        CovenManagerAPI.PostCoven<string>("inventory/consume", pData, pSuccess, pError);
    }
    /*Expected
    {
      "equipped" [object of changed items]
    }
    */
    public static void Equip(Equipped pItems, Action<string> pSuccess, Action<string> pError)
    {
        var pData = new Inventory_Equip();
        pData.equipped = pItems;
        CovenManagerAPI.PostCoven<string>("inventory/equip", pData, pSuccess, pError);
    }


    public static void Display(Action<string> pSuccess, Action<string> pError)
    {
        CovenManagerAPI.PostCoven<string>("inventory/display", null, pSuccess, pError);
    }
}