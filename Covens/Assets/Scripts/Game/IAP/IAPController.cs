using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Purchasing;
using UnityEngine;


/// <summary>
/// iap controller
///  - purchases
///  - store the current purchase
///  - (not anymore) validates through server
/// </summary>
public class IAPController : Patterns.SingletonComponent<IAPController>
{
    public enum PurchaseStep
    {
        None,
        InProgress,
        Completed,
        Error,
        NotLoaded,
    }


    public IAPUnity m_StoreImplementation;
    private Action<PurchaseEventArgs> m_pSuccess;
    private Action<string> m_pFailure;


    static public void Load()
    {
        var vIAPList = StoreDB.Instance.GetItens(new EnumStoreType[] { EnumStoreType.IAP });
        if (Instance.m_StoreImplementation == null)
            Instance.m_StoreImplementation = Instance.gameObject.AddComponent<IAPUnity>();
        List<string> vList = new List<string>();
        for (int i = 0; i < vIAPList.Count; i++)
            vList.Add(vIAPList[i].ID);
        Instance.Initialize(vList);
    }


    public void Initialize(List<string> sIAPList)
    {
        Debug.Log("IAPController.Initialize> Initialize");
        m_StoreImplementation.Initialize(sIAPList, OnInitialize);
    }
    void OnInitialize(Product[] vProducts)
    {
        for (int i = 0; i < vProducts.Length; i++)
        {
            StoreItemModel pItem = StoreDB.Instance.GetItem(vProducts[i].definition.id);
            if (pItem == null)
                continue;
            pItem.Iap = vProducts[i].metadata.localizedPriceString;
        }
    }

    public void Purchase(string sID, Action<PurchaseEventArgs> pSuccess, Action<string> pFailure)
    {
        Debug.Log("IAPController.Purchase> sID: " + sID);
        m_pSuccess = pSuccess;
        m_pFailure = pFailure;
        m_StoreImplementation.Purchase(sID, PurchaseSuccess, PurchaseFailure);
    }

    /* validation will be done in tstore controller because the route is only ready for purchase-silver
    public void Validate(PurchaseEventArgs pArgs)
    {
        string sReceipt = pArgs.purchasedProduct.receipt;
        string sTransactionId = pArgs.purchasedProduct.transactionID;
        //StoreController.Instance.PurchaseSilver()
    }
    */


    void PurchaseSuccess (PurchaseEventArgs pArgs)
    {
        //Validate(pArgs);
        Debug.Log("IAPController.PurchaseSuccess>");
        m_pSuccess(pArgs);
    }
    void PurchaseFailure (PurchaseFailureReason eFail)
    {
        Debug.Log("IAPController.PurchaseFailure>" + eFail);
        m_pFailure(eFail.ToString());
    }


    /*
    IEnumerator PurchaseProcess(string sID, Action<string> pSuccess, Action<string> pFailure)
    {
        yield return null;

        // 
        bool bCompleted = false;
        Action<PurchaseEventArgs> Success = (PurchaseEventArgs pArgs) =>
        {
            bCompleted = true;
        };
        Action<PurchaseFailureReason> Failure = (PurchaseFailureReason eFail) =>
        {
            bCompleted = true;
        };
        m_StoreImplementation.Purchase(sID, Success, Failure);

        while()
    }*/
}