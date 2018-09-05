using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;


/// <summary>
/// manages the unity IAP purchase process
/// </summary>
public class IAPUnity : MonoBehaviour, IStoreListener
{

    private IStoreController m_StoreController;
    private IExtensionProvider m_ExtensionProvider;
    private StandardPurchasingModule m_pModule;
    private bool m_PurchaseInProgress;

    public bool IsGooglePlay { get { return Application.platform == RuntimePlatform.Android && m_pModule.appStore == AppStore.GooglePlay; } }
    public bool IsAppleStore { get { return Application.platform == RuntimePlatform.IPhonePlayer && m_pModule.appStore == AppStore.AppleAppStore; } }
    public bool IsAvailable { get; set; }
    private Action<PurchaseEventArgs> m_pSuccessCallback;
    private Action<PurchaseFailureReason> m_pFailCallback;
    private Action<Product[]> m_pInitializeCallback;


    public void Initialize(List<string> sIAPList, Action<Product[]> pInitializeCallback)
    {
        m_pInitializeCallback = pInitializeCallback;
        IsAvailable = false;
        m_pModule = StandardPurchasingModule.Instance();
        m_pModule.useFakeStoreUIMode = FakeStoreUIMode.StandardUser;
        ConfigurationBuilder pBuilder = ConfigurationBuilder.Instance(m_pModule);
        for (int i = 0; i < sIAPList.Count; i++)
        {
            pBuilder.AddProduct(sIAPList[i], ProductType.Consumable);
        }

        // Now we're ready to initialize Unity IAP.
        UnityPurchasing.Initialize(this, pBuilder);
        Log("Initialize");
    }

    
    public void Purchase(string sProductID, Action<PurchaseEventArgs> pSuccess, Action<PurchaseFailureReason> pFailure)
    {
        m_pSuccessCallback = pSuccess;
        m_pFailCallback = pFailure;
        if (m_PurchaseInProgress == true)
        {
            Log("Please wait, purchase in progress");
            m_pFailCallback(PurchaseFailureReason.ExistingPurchasePending);
            return;
        }

        if (m_StoreController == null)
        {
            Debug.LogError("Purchasing is not initialized");
            m_pFailCallback(PurchaseFailureReason.PurchasingUnavailable);
            return;
        }

        if (m_StoreController.products.WithID(sProductID) == null)
        {
            Debug.LogError("No product has id " + sProductID);
            m_pFailCallback(PurchaseFailureReason.ProductUnavailable);
            return;
        }

        m_PurchaseInProgress = true;
        m_StoreController.InitiatePurchase(m_StoreController.products.WithID(sProductID));
    }


    #region IStoreListener

    public void OnInitialized(IStoreController pController, IExtensionProvider pExtensions)
    {
        m_StoreController = pController;
        m_ExtensionProvider = pExtensions;
        IsAvailable = true;
        Log("OnInitialized");

        if (m_pInitializeCallback != null)
            m_pInitializeCallback(pController.products.all);
        m_pInitializeCallback = null;
    }

    public void OnInitializeFailed(InitializationFailureReason eError)
    {
        Log("OnInitializeFailed: " + eError);
        IsAvailable = false;
        m_pInitializeCallback = null;
    }

    public void OnPurchaseFailed(Product pProduct, PurchaseFailureReason eFailure)
    {
        Log("OnPurchaseFailed: transactionID[" + pProduct.transactionID + "]");
        m_pFailCallback(eFailure);
        m_pFailCallback = null;
        m_pSuccessCallback = null;
    }


    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs pPurchaseArgs)
    {
        Log("OnInitializeFailed");
        Log("Purchase OK: " + pPurchaseArgs.purchasedProduct.definition.id);
        Log("Receipt: " + pPurchaseArgs.purchasedProduct.receipt);
        m_PurchaseInProgress = false;

        m_pSuccessCallback(pPurchaseArgs);
        m_pFailCallback = null;
        m_pSuccessCallback = null;
        return PurchaseProcessingResult.Complete;
    }

    #endregion



    void Log(string sLog)
    {
//        Debug.Log("IAPUnity> " + sLog);
    }

}