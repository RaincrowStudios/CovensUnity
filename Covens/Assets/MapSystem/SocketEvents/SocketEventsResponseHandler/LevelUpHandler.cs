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
            public double timestamp;
        }

        public string EventName => "level.up";
        
        public void HandleResponse(string eventData)
        {
            LevelUpEventData data = JsonConvert.DeserializeObject<LevelUpEventData>(eventData);
            PlayerData player = PlayerDataManager.playerData;

            if (data.instance == player.instance)
            {
                if (data.level == 3)
                    AppsFlyerAPI.ReachedLevelThree();

                //update level
                player.level = data.level;

                ////update required exp and baseenergy
                //player.xpToLevelUp = data.xpToLevelUp;
                //player.baseEnergy = data.newBaseEnergy;

                //udpate UI
                PlayerManagerUI.Instance.playerlevelUp();
                PlayerManagerUI.Instance.UpdateEnergy();

                //show ui feedback
                UILevelUp.Instance.Show();
            }
            else
            {
                IMarker marker = MarkerManager.GetMarker(data.instance);

                if (marker != null)
                {
                    WitchToken token = marker.customData as WitchToken;
                    if (token != null)
                    {
                        //update token data
                        token.level = data.level;

                        //update level text
                        marker.SetStats();
                    }
                }
            }
        }
    }
}