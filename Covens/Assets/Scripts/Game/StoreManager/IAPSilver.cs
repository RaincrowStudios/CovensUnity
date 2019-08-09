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
        public SilverBundleData data;
        public System.Action<string> callback;
    }

    private static Dictionary<string, string> m_ProductMap;
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
            Debug.LogError("IAP already initialized");
            return;
        }
        
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        m_ProductMap = new Dictionary<string, string>();

        string log = "Initializing IAP\nProducts:";
        foreach (var prod in StoreManagerAPI.SilverBundleDict)
        {
            builder.AddProduct(prod.Value.product, ProductType.Consumable);
            m_ProductMap.Add(prod.Value.product, prod.Key);

            log += "\n\t" + prod.Value.product;
        }
        Debug.Log(log);

        UnityPurchasing.Initialize(this, builder);
    }


    private bool IsInitialized()
    {
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }

    public void BuyProductID(StoreApiItem storeProduct, System.Action<string> callback)
    {
        if (m_OngoingPurchase != null)
        {
            callback?.Invoke("another purchase is in progress [" + m_OngoingPurchase.id + "]");
            return;
        }

        Debug.Log("Initializing purchase: " + storeProduct.productId);

        if (IsInitialized())
        {
            Raincrow.Analytics.Events.PurchaseAnalytics.StartIAP(storeProduct.productId);


            Product product = m_StoreController.products.WithID(storeProduct.productId);

            m_OngoingPurchase = new OngoingPurchase
            {
                id = storeProduct.id,
                data = StoreManagerAPI.GetSilverBundle(storeProduct.id),
                callback = callback
            };

            if (product != null && product.availableToPurchase)
            {
                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                m_StoreController.InitiatePurchase(product);
            }
            else
            {
                string error = "Purchase failed. Not purchasing product, either is not found or is not available for purchase";
                Debug.LogError(error);
                callback?.Invoke(error);
            }
        }
        else
        {
            string error = "Purchase failed. Store not ready.";
            Debug.LogError(error);
            callback?.Invoke(error);
        }
    }

    private void FinishPurchase(string id, string error)
    {
        if (m_OngoingPurchase == null)
        {
            Debug.LogError("no pruchase ongoing");
            return;
        }

        if (string.IsNullOrEmpty(error) == false)
            Debug.LogError(error);

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
            Debug.LogError("RestorePurchases FAIL. Not initialized.");
            return;
        }
        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            Debug.Log("RestorePurchases started ...");
            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
            apple.RestoreTransactions((result) =>
            {
                Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
            });
        }
        else
        {
            Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        m_StoreController = controller;
        m_StoreExtensionProvider = extensions;
        Debug.Log("IAP initialized");
    }


    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogError("OnInitializeFailed InitializationFailureReason:" + error);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        if (!LoginAPIManager.characterLoggedIn)
        {
            return PurchaseProcessingResult.Pending;
        }

        string id = m_ProductMap[args.purchasedProduct.definition.id];
        SilverBundleData data = StoreManagerAPI.GetSilverBundle(id);

        StoreManagerAPI.Purchase(
            id,
            "silver",
            null,
            args.purchasedProduct.receipt,
            (error) =>
            {
                if (string.IsNullOrEmpty(error))
                {
                    PlayerDataManager.playerData.silver += data.amount;
                    if (PlayerManagerUI.Instance != null)
                        PlayerManagerUI.Instance.UpdateDrachs();

                    //remove from pending
                    m_StoreController.ConfirmPendingPurchase(args.purchasedProduct);
                }
                else
                {
                    //remove from pending so its not processed again
                    if (error == "6006" || error == "6005" || error == "6004")
                        m_StoreController.ConfirmPendingPurchase(args.purchasedProduct);
                }

                FinishPurchase(id, error);
            }
        );
        return PurchaseProcessingResult.Pending;
    }


    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        string id = m_ProductMap[product.definition.id];
        FinishPurchase(id, failureReason.ToString());
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
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
}
