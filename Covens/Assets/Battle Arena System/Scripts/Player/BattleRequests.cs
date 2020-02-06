using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Raincrow.Maps;
using Oktagon.Analytics;
using Raincrow.Analytics;

namespace BattleArena
{
   public static class BattleRequests
    {
        public static void Challenge(IMarker target, System.Action success = null, System.Action error = null)
        {
            APIManager.Instance.Post(
                "battle/challenge/" + target.Token.Id, "{}",
                (response, result) =>
                {
                    if (result == 200)
                    {
                        success?.Invoke();
                    }
                    else
                    {
                        error?.Invoke();
                    }
                });
        }

        public static void Planning(int battleId, System.Action success = null, System.Action error = null)
        {
            APIManager.Instance.Post(
                "battle/planning/" + battleId, "{}",
                (response, result) =>
                {
                    if (result == 200)
                    {
                        success?.Invoke();
                    }
                    else
                    {
                        error?.Invoke();
                    }
                });
        }
    }
}


