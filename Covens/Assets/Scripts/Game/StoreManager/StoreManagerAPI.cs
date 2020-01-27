using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.ComponentModel;

namespace Raincrow.Store
{
    public class StoreData
    {
        [JsonProperty("Packs1")]
        public Dictionary<string, PackData> Packs;
        public List<StoreItem> Bundles;
        public List<StoreItem> Consumables;
        public List<StoreItem> Currencies;
        public List<StoreItem> Cosmetics;
        public List<StoreItem> Styles;

        public int GetPrice(string type, string id, bool IsSilver)
        {
            List<StoreItem> items = null;
            switch (type)
            {
                case StoreManagerAPI.TYPE_CURRENCY: items = new List<StoreItem>(Currencies); break;
                case StoreManagerAPI.TYPE_COSMETIC: items = new List<StoreItem>(Cosmetics); items.AddRange(Styles); break;
                case StoreManagerAPI.TYPE_INGREDIENT_BUNDLE: items = new List<StoreItem>(Bundles); break;
                case StoreManagerAPI.TYPE_ELIXIRS: items = new List<StoreItem>(Consumables); break;
            }

            if (items == null)
                return 0;

            foreach (var item in items)
            {
                if (item.id == id)
                    return IsSilver ? item.silver : item.gold;
            }

            Debug.LogError("store item not found for \"" + id + "\"");
            return 0;
        }
    }

    public struct StoreItem
    {
        [DefaultValue("")]
        public string id;
        public double unlockOn;
        [DefaultValue("")]
        public string tooltip;
        public int silver;
        public int gold;
    }
    
    public struct ConsumableData
    {
        public float duration;
    }

    public struct CurrencyBundleData
    {
        [DefaultValue("")]
        public string product;
        public float cost;
        [JsonProperty("silverAmount")]
        public int silver;
        [JsonProperty("goldAmount")]
        public int gold;
        [JsonProperty("silverBonus")]
        public int silverBonus;
        [JsonProperty("goldBonus")]
        public int goldBonus;
    }

    public struct PackData
    {
        [JsonProperty("productId")]
        public string product;
        public float fullPrice;
        public bool isFree;
        public double expiresOn;
        public List<PackItemData> content;
    }

    public struct PackItemData
    {
        public const string INGREDIENT_BUNDLE = "bundles";

        [JsonProperty("item")]
        public string id;
        [JsonProperty("itemType")]
        public string type;
        [JsonProperty("amount")]
        public int amount;
    }
       
    public static class StoreManagerAPI
    {
        public const string TYPE_CURRENCY = "currency";
        public const string TYPE_COSMETIC = "cosmetics";
        public const string TYPE_INGREDIENT_BUNDLE = "bundles";
        public const string TYPE_ELIXIRS = "consumables";
        public const string TYPE_PACK = "packs";

        public static StoreData StoreData { get; set; }
        
        public static Dictionary<string, List<ItemData>> IngredientBundleDict { get; set; }
        public static Dictionary<string, ConsumableData> ConsumableDict { get; set; }
        public static Dictionary<string, CurrencyBundleData> CurrencyBundleDict { get; set; }

        public static event System.Action<string, string, string, int> OnPurchaseComplete;



        public static ItemData[] GetIngredientBundle(string id)
        {
            if (IngredientBundleDict.ContainsKey(id))
            {
                return IngredientBundleDict[id].ToArray();
            }
            else
            {
                LogError($"ingredient bundle not found (\"{id}\")");
                return new ItemData[0];
            }
        }

        public static ConsumableData GetConsumable(string id)
        {
            if (ConsumableDict.ContainsKey(id))
            {
                return ConsumableDict[id];
            }
            else
            {
                LogError($"consumable not found (\"{id}\")");
                return new ConsumableData();
            }
        }

        public static CurrencyBundleData GetCurrencyBundle(string id)
        {
            if (CurrencyBundleDict.ContainsKey(id))
            {
                return CurrencyBundleDict[id];
            }
            else
            {
                LogError($"currency bundle not found (\"{id}\")");
                return new CurrencyBundleData();
            }
        }

        public static PackData GetPackData(string id)
        {
            if (StoreData.Packs != null && StoreData.Packs.ContainsKey(id))
            {
                return StoreData.Packs[id];
            }
            else
            {
                LogError($"special pack not found (\"{id}\")");
                return new PackData();
            }
        }


        public static void Purchase(string id, string type, string currency, System.Action<string> callback)
        {
            Purchase(id, type, currency, null, callback);
        }

        public static void Purchase(string id, string type, string currency, string receipt, System.Action<string> callback)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("itemId", id);
            data.Add("itemType", type);
            if (currency != null)
                data.Add("currency", currency);
            if (receipt != null)
                data.Add("receipt", receipt);

            Log("purchasing \"" + id + "\"");

            APIManager.Instance.Post("shop/purchase", JsonConvert.SerializeObject(data), (response, result) =>
            {
                if (result == 200)
                {
                    Log($"purchase complete: [{type}] {id}");

                    AddItem(id, type);

                    int silver = 0;
                    int gold = 0;
                    int price = 0;
                    if (currency == "silver")
                    {
                        silver = StoreData.GetPrice(type, id, true);
                        price = silver;
                    }
                    if (currency == "gold")
                    {
                        gold = StoreData.GetPrice(type, id, false);
                        price = gold;
                    }

                    if (silver != 0)
                    {
                        Log("paid " + silver + " drachs");
                        PlayerDataManager.playerData.silver -= silver;
                    }
                    if (gold != 0)
                    {
                        Log("paid " + gold + " gold");
                        PlayerDataManager.playerData.gold -= gold;
                    }

                    if (PlayerManagerUI.Instance != null)
                        PlayerManagerUI.Instance.UpdateDrachs();

                    callback?.Invoke(null);
                    OnPurchaseComplete?.Invoke(id, type, currency, price);
                }
                else
                {
                    callback?.Invoke(response);
                }
            });
        }

        public static void AddItem(string id, string type)
        {
            Log("Adding items for purchase of " + type + ": " + id);

            switch (type)
            {
                case TYPE_CURRENCY:
                {
                    CurrencyBundleData data = GetCurrencyBundle(id);
                    PlayerDataManager.playerData.silver += (data.silver + data.silverBonus);
                    PlayerDataManager.playerData.gold += (data.gold + data.goldBonus);
                    PlayerManagerUI.Instance?.UpdateDrachs();
                    break;
                }
                case TYPE_COSMETIC:
                {
                    //add the item to inventory
                    CosmeticData cosmetic = DownloadedAssets.GetCosmetic(id);
                    if (!PlayerDataManager.playerData.inventory.cosmetics.Exists(c => c.id == id))
                    {
                        PlayerDataManager.playerData.inventory.cosmetics.Add(cosmetic);
                    }
                    break;
                }
                case TYPE_INGREDIENT_BUNDLE:
                {
                    //add the ingredients to the inventory
                    ItemData[] bundle = GetIngredientBundle(id);
                    for (int i = 0; i < bundle.Length; i++)
                    {
                        PlayerDataManager.playerData.AddIngredient(bundle[i].id, bundle[i].count);
                    }
                    break;
                }
                case TYPE_ELIXIRS:
                {
                    Item elixir = PlayerDataManager.playerData.inventory.consumables.Find(it => it.id == id);
                    ConsumableData consumable = GetConsumable(id); ;
                    if (elixir == null || string.IsNullOrEmpty(elixir.id))
                    {
                        //add one if none is found on the players inventory
                        PlayerDataManager.playerData.inventory.consumables.Add(new Item
                        {
                            id = id,
                            count = 1
                        });
                    }
                    else
                    {   
                        //increment if it already exists on the player's inventory
                        elixir.count += 1;
                    }
                    break;
                }
                case TYPE_PACK:
                {
                    PackData data = StoreManagerAPI.GetPackData(id);

                    PlayerDataManager.playerData.OwnedPacks.Add(id);

                    foreach (var item in data.content)
                    {
                        if (item.type == "effect")
                        {

                        }
                        else if (item.type == StoreManagerAPI.TYPE_CURRENCY)
                        {
                            if (item.id == "gold")
                            {
                                PlayerDataManager.playerData.gold += item.amount;
                                Log("Adding gold x" + item.amount);
                            }
                            else if (item.id == "silver")
                            {
                                PlayerDataManager.playerData.silver += item.amount;
                                Log("Adding gold silver" + item.amount);
                            }

                            PlayerManagerUI.Instance?.UpdateDrachs();
                        }
                        else
                        {
                            AddItem(item.id, item.type);
                        }
                    }
                    break;
                }
                default:
                {
                    Debug.LogException(new System.Exception("store item type " + type + " (" + id + ") not implemetend"));
                    break;
                }
            }
        }


        private static void Log(string msg)
        {
#if UNITY_EDITOR
            Debug.Log("[<color=cyan>StoreAPI</color>] " + msg);
            return;
#else
            Debug.Log("[StoreAPI] " + msg);
#endif            
        }

        private static void LogError(string msg)
        {
#if UNITY_EDITOR
            Debug.LogError("[<color=cyan>StoreAPI</color>] " + msg);
            return;
#else
            Debug.LogException(new System.Exception("[StoreAPI] " + msg));
#endif
        }
    }
}
