using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Raincrow.Maps;

public static class SummoningManager
{
    public static event System.Action<SpiritMarker> OnSpiritSummoned;

    public static bool CanSummon(string spirit)
    {
        SpiritData data = DownloadedAssets.GetSpirit(spirit);

        if (string.IsNullOrEmpty(data.herb) == false && PlayerDataManager.playerData.GetIngredient(data.herb) == 0)
            return false;

        if (string.IsNullOrEmpty(data.tool) == false && PlayerDataManager.playerData.GetIngredient(data.tool) == 0)
            return false;

        if (string.IsNullOrEmpty(data.gem) == false && PlayerDataManager.playerData.GetIngredient(data.gem) == 0)
            return false;

        return true;
    }

    public static void Summon(string spirit, System.Action<SpiritMarker, string> callback)
    {
        APIManager.Instance.Post("character/summon/" + spirit, "{}", (string s, int r) =>
        {
            if (r == 200)
            {
                //remove the ingredients
                RemoveIngredients(spirit);
                
                //spawn the marker
                SpiritToken token = JsonConvert.DeserializeObject<SpiritToken>(s);
                SpiritMarker marker = MarkerSpawner.Instance.AddMarker(token) as SpiritMarker;
                
                SpiritData spiritData = DownloadedAssets.GetSpirit(spirit);
                int tier = spiritData.tier;
                int summonCost = PlayerDataManager.summoningCosts[tier - 1];
                int xpGained = tier * 25;

                if (summonCost > PlayerDataManager.playerData.energy)
                    OnCharacterDeath.OnSummonDeath?.Invoke(spirit);

                OnMapEnergyChange.ForceEvent(PlayerManager.marker, PlayerDataManager.playerData.energy - summonCost);

                PlayerDataManager.playerData.AddExp(xpGained);
                callback(marker, null);
                OnSpiritSummoned?.Invoke(marker);
            }
            else
            {
                callback(null, s);
            }
        });
    }

    public static void SummonPoP(string spirit, int position, int island, System.Action<string> callback)
    {
        Dictionary<string, object> data = new Dictionary<string, object>
        {
            { "position", position },
            { "island", island }
        };

        APIManager.Instance.Post("character/summon/" + spirit, JsonConvert.SerializeObject(data), (string s, int r) =>
        {
            if (r == 200)
            {
                //remove the ingredients
                RemoveIngredients(spirit);
                
                SpiritData spiritData = DownloadedAssets.GetSpirit(spirit);
                int tier = spiritData.tier;
                int summonCost = PlayerDataManager.summoningCosts[tier - 1] * 3;

                OnMapEnergyChange.ForceEvent(PlayerManager.marker, PlayerDataManager.playerData.energy - summonCost);

                callback(null);
            }
            else
            {
                callback(s);
            }
        });
    }

    private static void RemoveIngredients(string spiritId)
    {
        SpiritData spirit = DownloadedAssets.GetSpirit(spiritId);
        List<spellIngredientsData> toRemove = new List<spellIngredientsData>();

        if (string.IsNullOrEmpty(spirit.gem) == false)
            PlayerDataManager.playerData.SubIngredient(spirit.gem, 1);

        if (string.IsNullOrEmpty(spirit.tool) == false)
            PlayerDataManager.playerData.SubIngredient(spirit.tool, 1);

        if (string.IsNullOrEmpty(spirit.herb) == false)
            PlayerDataManager.playerData.SubIngredient(spirit.herb, 1);
    }
}
