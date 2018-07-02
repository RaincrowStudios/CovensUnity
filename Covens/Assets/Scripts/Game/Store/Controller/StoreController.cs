using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreController : Patterns.SingletonComponent<StoreController>
{



    #region server synch

    public static void Purchase(string sItemID, Action<string> pSuccess, Action<string> pError)
    {
        StoreAPI.Purchase(sItemID, pSuccess, pError);
    }
    public static void PurchaseSilver(string sItemID, Action<string> pSuccess, Action<string> pError)
    {
        StoreAPI.PurchaseSilver(sItemID, pSuccess, pError);
    }
    public static void Display(Action<string> pSuccess, Action<string> pError)
    {
        StoreAPI.Display(pSuccess, pError);
    }

    #endregion


}