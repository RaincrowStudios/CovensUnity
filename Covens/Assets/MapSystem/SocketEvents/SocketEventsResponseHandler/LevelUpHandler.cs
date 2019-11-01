using Newtonsoft.Json;
using Raincrow.Maps;
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
                if (player.energy < player.baseEnergy)
                    OnMapEnergyChange.ForceEvent(PlayerManager.marker, player.baseEnergy, data.timestamp);
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
                IMarker marker = MarkerManager.GetMarker(data.instance);

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