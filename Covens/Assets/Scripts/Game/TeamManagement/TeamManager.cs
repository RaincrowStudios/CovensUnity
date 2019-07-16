using Newtonsoft.Json;
using System.Collections.Generic;

public static class TeamManager
{
    public enum CovenRole
    {
        MEMBER = 0,
        MODERATOR = 1,
        ADMIN = 2,
        None = 100
    }


    public static event System.Action<string, string> OnJoinCoven;
    public static event System.Action OnLeaveCovenRequested;
    public static event System.Action OnKicked;
    public static event System.Action<string, string> OnCovenCreated;
    public static event System.Action OnCovenDisbanded;

    public static CovenRole MyRole{
        get
        {
            //if (PlayerDataManager.playerData.coven)
            return CovenRole.MEMBER;
        }
    }



    public static void CreateCoven(string name, System.Action<int, string> callback)
    {
        string data = $"{{\"name\":\"{name}\"}}";
        APIManager.Instance.Post(
            "coven",
            data, 
            (response, result) =>
            {
                if (result == 200)
                {

                }
                callback?.Invoke(result, response);
            });
    }

    public static void DisbandCoven(System.Action<int, string> callback)
    {
        APIManager.Instance.Delete(
            "coven/disband",
            "",
            (response, result) =>
            {
                if (result == 200)
                {
                    //PlayerDataManager.playerData.coven = null;
                }
                callback(result, response);
            });
    }

    public static void SendRequest(string covenId, System.Action<int, string> callback)
    {

    }
}