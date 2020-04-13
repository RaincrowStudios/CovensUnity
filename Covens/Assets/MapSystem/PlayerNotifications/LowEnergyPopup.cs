using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Oktagon.Analytics;
using Raincrow.Analytics;

public class LowEnergyPopup : MonoBehaviour
{
    private static LowEnergyPopup Instance { get; set; }

    [SerializeField] private TextMeshProUGUI m_offerText;
    [SerializeField] private TextMeshProUGUI m_buttonText;
    [SerializeField] private Button m_close;
    [SerializeField] private Button m_continue;
    [SerializeField] private CanvasGroup thisCG;

    private System.Action m_OnEnergyRestored;
    private static bool HasShownDeathMessage = false;

    public static void Show(System.Action onRestored = null)
    {
        if (Instance == null && !HasShownDeathMessage)
        {
            Utilities.InstantiateObject(Resources.Load<GameObject>("UILowEnergyPopUp"), null);
            Instance.m_OnEnergyRestored = onRestored;
            HasShownDeathMessage = true;
        }
    }

    private void Awake()
    {
        Instance = this;
        thisCG = this.GetComponent<CanvasGroup>();
        LeanTween.alphaCanvas(thisCG, 1f, 0.5f).setEaseInCubic();
        BackButtonListener.AddCloseAction(Close);
    }

    // Start is called before the first frame update
    void Start()
    {
        m_offerText.text = LocalizeLookUp.GetText("energy_restore_message");
        m_close.onClick.AddListener(() => { Close(); });

        //this will need localization
        if (PlayerDataManager.playerData.gold < 1)
        {
            m_buttonText.text = string.Concat("<color=red>", LocalizeLookUp.GetText("energy_restore_missing"), "</color>");
            m_continue.onClick.AddListener(() =>
            {
                UIStore.OpenSilverStore();
            });
        }
        else
        {
            m_buttonText.text = LocalizeLookUp.GetText("energy_restore_offer");
            m_continue.onClick.AddListener(() =>
            {
                SetupConfirmation();
            });
        }

    }

    void SetupConfirmation()
    {
        m_offerText.text = LocalizeLookUp.GetText("energy_restore_confirm");
        m_buttonText.text = LocalizeLookUp.GetText("store_accept");
        m_continue.onClick.AddListener(() =>
        {
            PurchaseEnergy((error) => 
            {
                if (string.IsNullOrEmpty(error))
                    m_OnEnergyRestored?.Invoke();
            });
        });
    }

    // Update is called once per frame
    public static void PurchaseEnergy(System.Action<string> callback = null)
    {
        LoadingOverlay.Show();

        APIManager.Instance.Post("shop/energy", "{}", (response, result) =>
        {
            LoadingOverlay.Hide();
            if (result == 200)
            {
                Instance.Close();

                Dictionary<string, object> eventParams = new Dictionary<string, object>
                {
                    { "clientVersion", Application.version },
                    { "productID", "fullenergyrestore" },
                    { "productCategory", "energyrestore" },
                    { "silverDrach", 0 },
                    { "goldDrach", 1 },
                };

                OktAnalyticsManager.PushEvent(CovensAnalyticsEvents.PurchaseCurrency, eventParams);

                PlayerDataManager.playerData.gold -= 1;
                OnMapEnergyChange.ForceEvent(PlayerManager.marker, PlayerDataManager.playerData.baseEnergy);

                PlayerManagerUI.Instance.UpdateDrachs();
                PlayerManagerUI.Instance.UpdateEnergy();

                UIGlobalPopup.ShowPopUp(null, LocalizeLookUp.GetText("blessing_full"));
                callback?.Invoke(null);
            }
            else
            {
                UIGlobalPopup.ShowError(null, APIManager.ParseError(response));
                callback?.Invoke(response);
            }
        });
    }

    void Close()
    {
        BackButtonListener.RemoveCloseAction();
        LeanTween.alphaCanvas(thisCG, 0f, 0.4f).setOnComplete(() =>
        {
            Destroy(gameObject);
        }).setEaseOutCubic();
    }
}