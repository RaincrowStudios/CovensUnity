using UnityEngine;
using Raincrow.Maps;
using Raincrow.BattleArena.Model;
using Newtonsoft.Json;

namespace BattleArena
{
    public static class ChallengeRequests
    {
        public static void Challenge(string id, System.Action success = null, System.Action error = null)
        {
            APIManager.Instance.Post(
                "battle/challenge/" + id, "{}",
                (response, result) =>
                {
                    if (result == 200)
                    {
                        success?.Invoke();
                    }
                    else
                    {
                        UIGlobalPopup.ShowPopUp(() => { }, "It was not possible to challenge");
                        error?.Invoke();
                    }
                });
        }

        public static void Join(string id, System.Action success = null, System.Action error = null)
        {
            APIManager.Instance.Post(
                "battle/join/" + id, "{}",
                (response, result) =>
                {
                    if (result == 200)
                    {
                        success?.Invoke();
                    }
                    else
                    {
                        UIGlobalPopup.ShowPopUp(() => { }, "Could not connect to the server!");
                        error?.Invoke();
                    }
                });
        }
    }
}


