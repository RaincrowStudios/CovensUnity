using UnityEngine;
using UnityEngine.Purchasing;
using Newtonsoft.Json;
public class IAPSilver : MonoBehaviour, IStoreListener
{

    public static IAPSilver instance { get; set; }
    private static IStoreController m_StoreController;
    private static IExtensionProvider m_StoreExtensionProvider;


    public static string silver1 = "com.raincrow.covens.silver1";
    public static string silver2 = "com.raincrow.covens.silver2";
    public static string silver3 = "com.raincrow.covens.silver3";
    public static string silver4 = "com.raincrow.covens.silver4";
    public static string silver5 = "com.raincrow.covens.silver5";
    public static string silver6 = "com.raincrow.covens.silver6";

    public static StoreApiItem selectedSilverPackage;
    //	private static string kProductNameAppleSubscription =  "com.unity3d.subscription.new";

    //	private static string kProductNameGooglePlaySubscription =  "com.unity3d.subscription.original"; 
    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        if (m_StoreController == null)
        {
            InitializePurchasing();
        }
    }

    public void InitializePurchasing()
    {
        if (IsInitialized())
        {
            return;
        }

        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        builder.AddProduct(silver1, ProductType.Consumable);
        builder.AddProduct(silver2, ProductType.Consumable);
        builder.AddProduct(silver3, ProductType.Consumable);
        builder.AddProduct(silver4, ProductType.Consumable);
        builder.AddProduct(silver5, ProductType.Consumable);
        builder.AddProduct(silver6, ProductType.Consumable);


        UnityPurchasing.Initialize(this, builder);
    }


    private bool IsInitialized()
    {
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }


    public void BuyProductID(StoreApiItem storeProduct)
    {
        Raincrow.Analytics.Events.PurchaseAnalytics.StartIAP(storeProduct.id);
        selectedSilverPackage = storeProduct;
        var productId = "com.raincrow.covens." + storeProduct.id;
        Debug.Log("Trying to buy + " + productId);
        if (IsInitialized())
        {
            Product product = m_StoreController.products.WithID(productId);

            if (product != null && product.availableToPurchase)
            {
                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                m_StoreController.InitiatePurchase(product);
            }
            else
            {
                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        else
        {
            Debug.Log("BuyProductID FAIL. Not initialized.");
        }
    }

    public void RestorePurchases()
    {
        if (!IsInitialized())
        {
            Debug.Log("RestorePurchases FAIL. Not initialized.");
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
        Debug.Log("OnInitialized: PASS");
        m_StoreController = controller;
        m_StoreExtensionProvider = extensions;
    }


    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }

    void SendTransaction(string token)
    {
        if (LoginAPIManager.characterLoggedIn)
        {
            Debug.Log("buying silver");
            var data = new { purchaseItem = selectedSilverPackage.id, receipt = token };
            APIManager.Instance.Post("shop/purchase-silver", JsonConvert.SerializeObject(data), ResponseO);
        }
    }

    public void ResponseO(string s, int r)
    {
        Debug.Log(s);
        if (r == 200)
        {
            Raincrow.Analytics.Events.PurchaseAnalytics.CompleteIAP(selectedSilverPackage.id);
            ShopManager.Instance.OnBuy();
            //Will need to specify a currency soon.

            AppsFlyerAPI.TrackStorePurchaseEvent(selectedSilverPackage.id);

        }
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        if (!LoginAPIManager.characterLoggedIn)
        {
            return PurchaseProcessingResult.Pending;
        }

        if (string.Equals(args.purchasedProduct.definition.id, silver1, System.StringComparison.Ordinal))
        {
            //			SilverDrachs.text = "+100 SILVER DRACHS";
            Debug.Log(args.purchasedProduct.receipt);
            SendTransaction(args.purchasedProduct.receipt);
        }
        else if (string.Equals(args.purchasedProduct.definition.id, silver2, System.StringComparison.Ordinal))
        {
            //			SilverDrachs.text = "+550 SILVER DRACHS";
            Debug.Log(args.purchasedProduct.receipt);
            SendTransaction(args.purchasedProduct.receipt);
        }
        else if (string.Equals(args.purchasedProduct.definition.id, silver3, System.StringComparison.Ordinal))
        {
            //			SilverDrachs.text = "+1200 SILVER DRACHS";
            Debug.Log(args.purchasedProduct.receipt);
            SendTransaction(args.purchasedProduct.receipt);
        }
        else if (string.Equals(args.purchasedProduct.definition.id, silver4, System.StringComparison.Ordinal))
        {
            //			SilverDrachs.text = "+2500 SILVER DRACHS";
            Debug.Log(args.purchasedProduct.receipt);
            SendTransaction(args.purchasedProduct.receipt);
        }
        else if (string.Equals(args.purchasedProduct.definition.id, silver5, System.StringComparison.Ordinal))
        {
            //			SilverDrachs.text = "+5200 SILVER DRACHS";
            Debug.Log(args.purchasedProduct.receipt);
            SendTransaction(args.purchasedProduct.receipt);
        }
        else if (string.Equals(args.purchasedProduct.definition.id, silver6, System.StringComparison.Ordinal))
        {
            //			SilverDrachs.text = "+14500 SILVER DRACHS";
            Debug.Log(args.purchasedProduct.receipt);
            SendTransaction(args.purchasedProduct.receipt);
        }
        else
        {
            Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
        }

        return PurchaseProcessingResult.Complete;
    }


    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
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
