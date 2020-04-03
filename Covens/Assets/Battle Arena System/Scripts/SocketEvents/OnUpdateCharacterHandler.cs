using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.GameEventResponses
{
    public class OnUpdateCharacterHandler : IGameEventHandler
    {
        public string EventName => "update.character";

        public void HandleResponse(string json)
        {
            UpdateCharacterArgs updateCharacterArgs = JsonConvert.DeserializeObject<UpdateCharacterArgs>(json);

            // Experience
            PlayerDataManager.playerData.xp = updateCharacterArgs.Experience;

            // Energy
            PlayerDataManager.playerData.energy = updateCharacterArgs.Energy;
            PlayerDataManager.playerData.baseEnergy = updateCharacterArgs.BaseEnergy;

            // Cooldown
            PlayerDataManager.playerData.cooldowns = new List<PlayerCooldown>();
            PlayerDataManager.playerData.cooldowns.AddRange(updateCharacterArgs.Cooldowns);

            // Effects
            PlayerDataManager.playerData.effects = new List<StatusEffect>();
            PlayerDataManager.playerData.effects.AddRange(updateCharacterArgs.Effects);

            PlayerDataManager.playerData.state = updateCharacterArgs.State; // State
            PlayerDataManager.playerData.insideBattle = updateCharacterArgs.InsideBattle; // Inside Battle
            PlayerDataManager.playerData.gold = updateCharacterArgs.Gold; // Gold
            PlayerDataManager.playerData.silver = updateCharacterArgs.Silver; // Silver
            PlayerDataManager.playerData.degree = updateCharacterArgs.Degree; // Degree
            PlayerDataManager.playerData.level = updateCharacterArgs.Level; // Level
            PlayerDataManager.playerData.alignment = updateCharacterArgs.Alignment; // Alignment

            OnMapEnergyChange.ForceEvent(PlayerManager.marker, PlayerDataManager.playerData.energy);

            PlayerManagerUI.Instance.UpdateEnergy();
        }
    }

    public struct UpdateCharacterArgs
    {
        [JsonProperty("_id")] public string Id { get; set; }
        [JsonProperty("xp")] public ulong Experience { get; set; }
        [JsonProperty("energy")] public int Energy { get; set; }
        [JsonProperty("baseEnergy")] public int BaseEnergy { get; set; }
        [JsonProperty("cooldowns")] public List<PlayerCooldown> Cooldowns { get; set; }
        [JsonProperty("effects")] public List<StatusEffect> Effects { get; set; }
        [JsonProperty("insideBattle")] public bool InsideBattle { get; set; }
        [JsonProperty("battleId")] public string BattleId { get; set; }
        [JsonProperty("silver")] public int Silver { get; set; }
        [JsonProperty("gold")] public int Gold { get; set; }
        [JsonProperty("state")] public string State { get; set; }
        [JsonProperty("level")] public int Level { get; set; }
        [JsonProperty("degree")] public int Degree { get; set; }
        [JsonProperty("alignment")] public long Alignment { get; set; }
    }
}