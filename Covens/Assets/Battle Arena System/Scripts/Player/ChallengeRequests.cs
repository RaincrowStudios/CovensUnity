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
                        UIGlobalPopup.ShowPopUp(() => { }, LocalizeLookUp.GetText("battle_error_on_challenge"));
                        error?.Invoke();
                    }
                });
        }

        public static void Join(string id, System.Action success = null, System.Action error = null)
        {
            UIMain.Instance.ShowBattleWaitScreen(0.3f, LocalizeLookUp.GetText("battle_wait_join_for_battle"));
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
                        UIGlobalPopup.ShowPopUp(() => { }, LocalizeLookUp.GetText("battle_error_on_join"));
                        UIMain.Instance.HideBattleWaitScreen(0.3f);
                        error?.Invoke();
                    }
                });
        }

        public static void Leave(string id, System.Action success = null, System.Action error = null)
        {
            APIManager.Instance.Post(
                "battle/leave" + id, "{}",
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


