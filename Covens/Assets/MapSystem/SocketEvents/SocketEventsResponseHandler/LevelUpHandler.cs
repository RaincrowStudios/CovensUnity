using Newtonsoft.Json;
using Oktagon.Analytics;
using Raincrow.Analytics;
using Raincrow.Maps;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.GameEventResponses
{
    public class LevelUpHandler : IGameEventHandler
    {
        public struct LevelUpEventData
        {
            [JsonProperty("_id")]
            public string instance;
            public int level;
            public int silver;
            public int power;
            public int resilience;
            public double timestamp;
        }

        public static event System.Action<int> OnPlayerLevelUp;

        public string EventName => "level.up";
        
        public void HandleResponse(string eventData)
        {
            LevelUpEventData data = JsonConvert.DeserializeObject<LevelUpEventData>(eventData);

            if (data.instance == PlayerDataManager.playerData.instance)
            {
                PlayerData player = PlayerDataManager.playerData;

                if (data.level == 3)
                {
                    AppsFlyerAPI.ReachedLevelThree();
                    ReviewPopupController.Init();
                }

                OnPlayerLevelUp?.Invoke(data.level);

                //update level
                player.level = data.level;
                player.silver += data.silver;
                player.basePower += data.power;
                player.baseResilience += data.resilience;

                string upgradedStats = "none";
                if (data.power > 0)
                {
                    upgradedStats = "power";
                }
                else if (data.resilience > 0)
                {
                    upgradedStats = "resilience";
                }

                Dictionary<string, object> eventParams = new Dictionary<string, object>()
                {
                    {"clientVersion", Application.version },
                    {"level", player.level },
                    {"upgradedStats", upgradedStats }
                };

                OktAnalyticsManager.PushEvent(CovensAnalyticsEvents.LevelUp, eventParams);

                if (player.energy < player.baseEnergy)
                {
                    OnMapEnergyChange.ForceEvent(PlayerManager.marker, player.baseEnergy, data.timestamp);
                }
                PlayerDataManager.playerData.UpdateSpells();
                
                //udpate UI
                PlayerManagerUI.Instance.playerlevelUp();
                PlayerManagerUI.Instance.UpdateEnergy();
                PlayerManagerUI.Instance.UpdateDrachs();

                //show ui feedback
                UILevelUp.Instance.Show(data);
            }
            else
            {
                IMarker marker = MarkerSpawner.GetMarker(data.instance);

                if (marker != null)
                {
                    WitchToken token = marker.Token as WitchToken;
                    if (token != null)
                    {
                        //update token data
                        token.level = data.level;

                        if (token.energy < token.baseEnergy)
                            OnMapEnergyChange.ForceEvent(marker, token.baseEnergy, data.timestamp);

                        //update level text
                        marker.SetStats();
                    }
                }
            }
        }
    }
}