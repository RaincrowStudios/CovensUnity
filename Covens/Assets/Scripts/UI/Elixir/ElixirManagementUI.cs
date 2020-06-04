using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Raincrow.Store;
using Newtonsoft.Json;

public class ElixirManagementUI : MonoBehaviour
{
    [Header("Basic Config")]
    [SerializeField] private Transform _rootElixirsStore;
    [SerializeField] private ElixirStoreUI _elixirStorePrefab;
    [SerializeField] private Button _closeButton;
    [SerializeField] private GameObject _loadingPopup;
    [SerializeField] private ElixirHolderUI _elixirHolderUI;

    [Header("Others")]
    [SerializeField] private Sprite _goldSprite;
    [SerializeField] private Sprite _silverSprite;

    private Dictionary<string, ElixirStoreUI> _dicElixirStoreUI = new Dictionary<string, ElixirStoreUI>();
    IEnumerator Start()
    {
        _closeButton.onClick.AddListener(Hide);

        foreach (StoreItem item in StoreManagerAPI.StoreData.Consumables)
        {
            bool isGold = item.gold > 0;
            int amount = GetAmountElixir(item.id);
            Sprite elixirIcon = null;



            bool isDownloading = true;

            DownloadedAssets.GetSprite(
                item.id,
                spr =>
                {
                    elixirIcon = spr;
                    isDownloading = false;
                },
                true
             );

            while (isDownloading)
            {
                yield return null;
            }


            StatusEffect statusEffect = PlayerDataManager.playerData.GetEffectById(item.spellId);

            ElixirDataStore elixirData = new ElixirDataStore
            {
                id = item.id,
                spellId = item.spellId,
                ElixirName = LocalizeLookUp.GetStoreTitle(item.id),
                OwnedAmount = amount,
                Price = isGold ? item.gold : item.silver,
                IconElixir = elixirIcon,
                IconCurrency = isGold ? _goldSprite : _silverSprite,
                PriceType = isGold ? ElixirDataStore.CurrencyType.gold : ElixirDataStore.CurrencyType.silver,
                ExpiresOn = statusEffect != null ? statusEffect.expiresOn : 0
            };

            ElixirStoreUI elixirPrefab = Instantiate(_elixirStorePrefab, _rootElixirsStore);
            _dicElixirStoreUI.Add(item.id, elixirPrefab);
            elixirPrefab.Setup(elixirData, OnClickConsume, OnClickPurcharse);
        }
    }

    private void OnClickPurcharse(StoreItem storeItem, int amount, Sprite elixirIcon)
    {
        LoadingOverlay.Show();
        UIStorePurchase.Show(
            storeItem,
            StoreManagerAPI.TYPE_ELIXIRS,
            LocalizeLookUp.GetStoreTitle(storeItem.id),
            LocalizeLookUp.GetStoreDesc(storeItem.id) + "\n" + $" ({LocalizeLookUp.GetText("store_gear_owned_upper")}: {(amount == 0 ? LocalizeLookUp.GetText("lt_none") : amount.ToString())})",
            elixirIcon,
            null,
            (error) =>
            {
                LoadingOverlay.Hide();
                UIStorePurchase.Close();

                if (string.IsNullOrEmpty(error))
                {
                    UIStorePurchaseSuccess.Show(
                        LocalizeLookUp.GetStoreTitle(storeItem.id),
                        LocalizeLookUp.GetStoreSubtitle(storeItem.id),
                        elixirIcon,
                        null);

                    _dicElixirStoreUI[storeItem.id].IncreaseAmountElixirs(1);
                }
                else
                {
                    UIGlobalPopup.ShowError(null, APIManager.ParseError(error));
                }
            });
    }

    private void OnClickConsume(string id, string spellId, Button consumeButton)
    {
        consumeButton.interactable = false;

        if (PlayerDataManager.playerData.HaveEffect(spellId))
        {
            UIGlobalPopup.ShowError(() => { }, LocalizeLookUp.GetText("consumed_error_elixir"));
            return;
        }

        UIGlobalPopup.ShowPopUp(
            confirmAction: (System.Action)(() =>
            {
                APIManager.Instance.Post("character/consume/" + id, "{}", OnConsumeResponse);
                _loadingPopup.SetActive(true);
            }),
            cancelAction: () =>
            {
                consumeButton.interactable = true;
            },
            txt: LocalizeLookUp.GetText("ui_drink_potion")// "Drink the potion?"
        );
    }

    private void OnConsumeResponse(string response, int result)
    {
        _loadingPopup.SetActive(false);

        if (result == 200)
        {
            StatusEffect effect = JsonConvert.DeserializeObject<StatusEffect>(response);
            string consumableId = StoreManagerAPI.StoreData.GetConsumableIdBySpellId(effect.spell);

            UIGlobalPopup.ShowPopUp(
                () =>
                {

                },
                LocalizeLookUp.GetStoreDesc(consumableId)
            );

            CharacterToken characterToken = PlayerManager.marker.Token as CharacterToken;
            characterToken.effects.Add(effect);

            PlayerConditionManager.OnPlayerApplyStatusEffect?.Invoke(effect);

            PlayerManager.marker.OnApplyStatusEffect(effect);

            _dicElixirStoreUI[consumableId].IncreaseAmountElixirs(-1);
            _dicElixirStoreUI[consumableId].UpdateTimer(PlayerDataManager.playerData.GetEffectById(effect.spell).expiresOn);
            _elixirHolderUI.UpdateElixirsStatus();
        }
        else
        {
            UIGlobalPopup.ShowError(null, APIManager.ParseError(response));
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);

        foreach (Item item in PlayerDataManager.playerData.inventory.consumables)
        {
            if (_dicElixirStoreUI.ContainsKey(item.id))
            {
                string spellId = StoreManagerAPI.StoreData.GetSpellIdByConsumableId(item.id);
                StatusEffect statusEffect = PlayerDataManager.playerData.GetEffectById(spellId);
                if (statusEffect != null)
                {
                    _dicElixirStoreUI[item.id].UpdateTimer(statusEffect.expiresOn);
                }

                _dicElixirStoreUI[item.id].UpdateElixirs(GetAmountElixir(item.id));

            }
        }
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private int GetAmountElixir(string id)
    {
        int amount = 0;
        foreach (var elixir in PlayerDataManager.playerData.inventory.consumables)
        {
            if (elixir.id == id)
            {
                amount = elixir.count;
                break;
            }
        }
        return amount;
    }
}