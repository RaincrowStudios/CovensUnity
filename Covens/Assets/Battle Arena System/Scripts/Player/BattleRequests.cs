using UnityEngine;
using Raincrow.Maps;
using Raincrow.BattleArena.Model;
using Newtonsoft.Json;

namespace BattleArena
{
   public static class BattleRequests
    {
        public static void Ready(string battleId, System.Action success = null, System.Action error = null)
        {
            APIManager.Instance.Post(
                "battle/ready/" + battleId, "{}",
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

        ///////////////////////
        /////// ACTIONS ///////
        ///////////////////////

        public static void Move(int _line, int _column, System.Action success = null, System.Action error = null)
        {

            var data = new
            {
                column = _column,
                line = _line
            };

            string dataJson = JsonConvert.SerializeObject(data);

            APIManager.Instance.Post("character/move", dataJson,
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
        public static void Flee(System.Action success = null, System.Action error = null)
        {
            APIManager.Instance.Post("character/flee", "{}",
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


