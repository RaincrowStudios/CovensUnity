using Newtonsoft.Json;
using Raincrow.Team;
using System.Collections.Generic;
using UnityEngine;

public static class TeamManager
{    
    public static event System.Action<string, string> OnJoinCoven;
    public static event System.Action OnLeaveCovenRequested;
    public static event System.Action OnKicked;
    public static event System.Action<string, string> OnCovenCreated;
    public static event System.Action OnCovenDisbanded;

    public static string MyCovenId { get { return PlayerDataManager.playerData.covenInfo.coven; } }

    public static TeamData MyCoven { get; private set; }

    public static CovenRole MyRole
    {
        get
        {
            if (string.IsNullOrEmpty(MyCovenId))
                return CovenRole.NONE;
            else
                return (CovenRole)PlayerDataManager.playerData.covenInfo.role;
        }
    }


    public static void GetCoven()
    {
        if (string.IsNullOrEmpty(MyCovenId))
            return;

        GetCoven(MyCovenId, (covenData, error) =>
        {
            MyCoven = covenData;
        });
    }

    public static void GetCoven(string covenId, System.Action<TeamData, string> callback)
    {
        string endpoint = string.Concat("coven/", covenId);
        APIManager.Instance.Get(endpoint, (response, result) =>
        {
            if (result == 200)
            {
                //return the TeamData
                TeamData teamData = JsonUtility.FromJson<TeamData>(response);
                callback?.Invoke(teamData, null);
            }
            else
            {
                //return the error
                callback?.Invoke(null, APIManager.ParseError(response));
            }
        });
    }

    public static void CreateCoven(string covenName, System.Action<TeamData, string> callback)
    {
        string data = $"{{\"name\":\"{covenName}\"}}";
        
        APIManager.Instance.Post("coven/", data, (response, responseCode) =>
        {
            if (responseCode == 200)
            {
                TeamData teamData = JsonUtility.FromJson<TeamData>(response);
                callback(teamData, null);
            }
            else
            {
                callback(null, APIManager.ParseError(response));
            }
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
                    MyCoven = null;
                    PlayerDataManager.playerData.covenInfo = new CovenInfo
                    {
                        coven = null,
                        joinedOn = 0,
                        role = 0,
                    };
                }
                callback(result, response);
            });
    }

    public static void SendRequest(string covenId, System.Action<int, string> callback)
    {

    }
}