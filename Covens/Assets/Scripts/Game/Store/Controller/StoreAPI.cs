using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreAPI
{
    public static void Purchase(string sItemID, string sCurrency, int iAmount, Action<string> pSuccess, Action<string> pError)
    {
        var pData = new Shop_Purchase();
        pData.purchaseItem = sItemID;
        pData.currency = sCurrency;
        pData.amount = iAmount;
        CovenManagerAPI.PostCoven<string>("shop/purchase", pData, pSuccess, pError);
    }
    public static void PurchaseSilver(string sItemID, Action<string> pSuccess, Action<string> pError)
    {
        var pData = new Shop_PurchaseSilver();
        pData.id = sItemID;
        CovenManagerAPI.PostCoven<string>("shop/purchase-silver", pData, pSuccess, pError);
    }
    public static void Display(Action<Shop_DisplayResponse> pSuccess, Action<string> pError)
    {
        CovenManagerAPI.GetCoven<Shop_DisplayResponse>("shop/display", null, pSuccess, pError);
    }
}