using UnityEngine;
using UnityEngine.Purchasing;
using Newtonsoft.Json;
using Raincrow.Store;
using System.Collections.Generic;

public class IAPSilver : MonoBehaviour, IStoreListener
{

    public static IAPSilver instance { get; set; }
    private static IStoreController m_StoreController;
    private static IExtensionProvider m_StoreExtensionProvider;
        
    private class OngoingPurchase
    {
        public string id;
        public System.Action<string> callback;
    }

    private static Dictionary<string, string> m_CurrencyMap;
    private static Dictionary<string, string> m_PackMap;
    private static OngoingPurchase m_OngoingPurchase;

    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        InitializePurchasing();
    }

    public void InitializePurchasing()
    {
        if (IsInitialized())
        {
            LogError("IAP already initialized");
            return;
        }
        
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        m_CurrencyMap = new Dictionary<string, string>();
        m_PackMap = new Dictionary<string, string>();

        string log = "Initializing IAP";

        log += "\nCurrency bundles:";
        foreach (var prod in StoreManagerAPI.CurrencyBundleDict)
        {
            builder.AddProduct(prod.Value.product, ProductType.Consumable);
            m_CurrencyMap.Add(prod.Value.product, prod.Key);

            log += "\n\t" + prod.Value.product;
        }

        log += "\nPacks:\n";
        if (StoreManagerAPI.PackDict != null)
        {
            foreach (var prod in StoreManagerAPI.PackDict)
            {
                builder.AddProduct(prod.Value.product, ProductType.Consumable);
                m_PackMap.Add(prod.Value.product, prod.Key);
                log += "\n\t" + prod.Value.product;
            }
        }

        Log(log);

        UnityPurchasing.Initialize(this, builder);
    }


    private bool IsInitialized()
    {
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }

    public void BuyProductID(string id, System.Action<string> callback)
    {
        if (m_OngoingPurchase != null)
        {
            LogError("Another purchase is in progress [" + m_OngoingPurchase.id + "]");
            callback?.Invoke("Another purchase is in progress [" + m_OngoingPurchase.id + "]");
            return;
        }

        string productId;
        if (StoreManagerAPI.PackDict.ContainsKey(id))
            productId = StoreManagerAPI.PackDict[id].product;
        else
            productId = StoreManagerAPI.GetCurrencyBundle(id).product;

        Log("Initializing purchase: " + productId);

        if (IsInitialized())
        {
            Raincrow.Analytics.Events.PurchaseAnalytics.StartIAP(productId);
            
            Product product = m_StoreController.products.WithID(productId);
            
            if (product != null && product.availableToPurchase)
            {
                Log($"Purchasing product asychronously: '{product.definition.id}'");

                m_OngoingPurchase = new OngoingPurchase
                {
                    id = id,
                    callback = callback
                };

                m_StoreController.InitiatePurchase(product);
            }
            else
            {
                string error = "Purchase failed. Not purchasing product, either is not found or is not available for purchase";
                LogError(error);
                callback?.Invoke(error);
            }
        }
        else
        {
            string error = "Purchase failed. Store not ready.";
            LogError(error);
            callback?.Invoke(error);
        }
    }

    private void FinishPurchase(string id, string error)
    {
        Log("Finishing pruchase for \"" + id + "\"");

        if (m_OngoingPurchase == null)
        {
            LogError("No purchase ongoing");
            return;
        }

        if (string.IsNullOrEmpty(error) == false)
            LogError(error);

        if (m_OngoingPurchase != null && m_OngoingPurchase.id == id)
        {
            Raincrow.Analytics.Events.PurchaseAnalytics.CompleteIAP(m_OngoingPurchase.id);
            AppsFlyerAPI.TrackStorePurchaseEvent(m_OngoingPurchase.id);
            m_OngoingPurchase?.callback(error);
            m_OngoingPurchase = null;
        }        
    }

    public void RestorePurchases()
    {
        if (!IsInitialized())
        {
            LogError("RestorePurchases FAIL. Not initialized.");
            return;
        }

        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            Log("RestorePurchases started ...");
            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
            apple.RestoreTransactions((result) =>
            {
                Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
            });
        }
        else
        {
            Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        m_StoreController = controller;
        m_StoreExtensionProvider = extensions;
        Log("IAP initialized");
    }


    public void OnInitializeFailed(InitializationFailureReason error)
    {
        LogError("OnInitializeFailed InitializationFailureReason:" + error);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        Log("Processing purchase for \"" + args.purchasedProduct.definition.id + "\"");

        if (!LoginAPIManager.characterLoggedIn)
        {
            Log("Character not logged in. Purchase pending");
            return PurchaseProcessingResult.Pending;
        }

        string product = args.purchasedProduct.definition.id;
        string id;
        string type;

        if (m_PackMap.ContainsKey(product))
        {
            id = m_PackMap[product];
            type = StoreManagerAPI.TYPE_PACK;
        }
        else
        {
            id = m_CurrencyMap[product];
            type = StoreManagerAPI.TYPE_CURRENCY;
        }

        Log("id: " + id + "\nreceipt: " + args.purchasedProduct.receipt);

        StoreManagerAPI.Purchase(
            id,
            type,
            null,
            args.purchasedProduct.receipt,
            (error) =>
            {                
                if (string.IsNullOrEmpty(error))
                {
                    Log("Processing successful: " + id);

                    //add the item to the player localy
                    StoreManagerAPI.AddItem(id, type);

                    //remove from pending
                    m_StoreController.ConfirmPendingPurchase(args.purchasedProduct);
                }
                else
                {
                    Debug.LogException(new System.Exception($"IAP processing error \"{id}\":\n{error}"));

                    //remove from pending so its not processed again
                    if (error == "6005" || error == "6004")
                    {
                        Debug.LogException(new System.Exception("IAP Validation error:\n" + args.purchasedProduct.receipt));
                        m_StoreController.ConfirmPendingPurchase(args.purchasedProduct);
                    }
                }

                FinishPurchase(id, error);
            }
        );
        return PurchaseProcessingResult.Pending;
    }


    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Log($"OnPurchaseFailed: FAIL. Product: '{product.definition.storeSpecificId}', PurchaseFailureReason: {failureReason}");

        string id = m_CurrencyMap.ContainsKey(product.definition.id) ? m_CurrencyMap[product.definition.id] : m_PackMap[product.definition.id];
        string error = "PurchaseFailureReason" + ((int)failureReason).ToString();
        FinishPurchase(id, error);
    }

    public Product GetProduct(string productId)
    {
        foreach (var product in m_StoreController.products.all)
        {
            if (product.definition.id.Equals(productId))
            {
                return product;
            }
        }

        return null;
    }

    public static string GetLocalizedPrice(string productId)
    {
        foreach (var product in m_StoreController.products.all)
        {
            if (product.definition.id.Equals(productId))
            {
                return product.metadata.localizedPriceString;
            }
        }

        return "0.00";
    }

    private static void Log(string msg)
    {
#if UNITY_EDITOR
        Debug.Log("[<color=cyan>IAPSilver</color>] " + msg);
        return;
#endif
        Debug.Log("[IAPSilver] " + msg);
    }

    private static void LogError(string msg)
    {
#if UNITY_EDITOR
        Debug.LogError("[<color=cyan>IAPSilver</color>] " + msg);
        return;
#endif
        Debug.LogError("[IAPSilver] " + msg);
    }
}
