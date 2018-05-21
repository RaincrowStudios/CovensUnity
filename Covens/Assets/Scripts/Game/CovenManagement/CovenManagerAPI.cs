//#define LOCAL
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

public class CovenManagerAPI
{
    static CovenRequestData Default(string sName)
    {
        var data = new CovenRequestData();
        data.covenName = sName;
        //data.instanceId = PlayerDataManager.playerData.instance;
        return data;
    }


    #region fake calls

#if LOCAL
    private static string CreateFakeBigPlayer()
    {
        string[] vNames = new string[] { "Hugo ", "Lucas", "Diogo", "Sean", "Mridul", "Arthur", "Dragomir", "Dan", "Ed", "Travis" };
        string[] vSurNames = new string[] { "Matsumoto ", "Penhas", "Conchal", "Fox", "Hedrik", "Landis", "Holt", "de Luca" };
        return
            vNames[UnityEngine.Random.Range(0, vNames.Length)]
            + " " + vSurNames[UnityEngine.Random.Range(0, vSurNames.Length)]
            + " " + vSurNames[UnityEngine.Random.Range(0, vSurNames.Length)]
            ;
    }




    public static void CreateCoven(string sCovenName = null)
    {
    }

    public static CovenItem NewCovenItem()
    {
        string[] vStatus = new string[] { "On Line", "Battling in Arena", "Off Line" };
        CovenItem ci = new CovenItem();
        ci.playerName = CreateFakeBigPlayer();
        ci.playerLevel = UnityEngine.Random.Range(1, 99);
        ci.isCreator = false;
        ci.status = vStatus[UnityEngine.Random.Range(0, vStatus.Length)];
        ci.title = CovenController.CovenRole.Member.ToString();// UnityEngine.Random.Range(0, CovenController.GetTitleList().Count);
        ci.rank = CovenController.CovenRole.Member.ToString();
        return ci;
    }
    public static CovenData GetFakeCovenData(string sCovenName)
    {
        CovenData cd = new CovenData();
        cd.players = new List<CovenItem>();
        int iPlayers = 15;
        for (int i = 0; i < iPlayers; i++)
        {
            cd.players.Add(NewCovenItem());
        }
        cd.players[0].rank = CovenController.CovenRole.Administrator.ToString();
        cd.players[0].title = CovenController.CovenRole.Administrator.ToString();
        cd.players[0].isCreator = true;
        cd.players[0].status = "Online";
        cd.covenName = sCovenName;
        return cd;
    }

    public static void GetCovenData(string sCovenName, Action<CovenData> pSuccess, Action<string> pError)
    {
        if (pSuccess != null)
            pSuccess(GetFakeCovenData(sCovenName));
    }


    static void Ally(string sCovenName)
    {
        var pData = Default(sCovenName);
        pData.covenName = sCovenName;
        PostCoven<CovenData>("coven/ally", pData, null, null);
    }
#endif

    #endregion

    #region calls

#if !LOCAL


    #region not a member requests

    // covens/coven/display --> req: {covenName: str} --> res: {coven info}
    public static void RequestDisplayCoven(string sCovenName, Action<CovenData> pSuccess, Action<string> pError)
    {
        var pData = Default(sCovenName);
#if LOCAL_REQUEST
        PostCoven<CovenData>("coven/display-" + sCovenName, pData, pSuccess, pError);
#else
        PostCoven<CovenData>("coven/display", pData, pSuccess, pError);
#endif
    }
    // covens/coven/create --> req: {covenName: str} --> res: 200
    public static void CreateCoven(string sCovenName, Action<CovenData> pSuccess, Action<string> pError)
    {
        var pData = Default(sCovenName);
        PutCoven<CovenData>("coven/create", pData, pSuccess, pError);
    }

    // covens/coven/request --> req: {covenName: str} --> res: 200 | WSS --> command: coven_member_request
    public static void RequestJoinCoven(string sPlayerName, Action<string> pSuccess, Action<string> pError)
    {
        PlayerRequestData pData = new PlayerRequestData();
        pData.playerName = sPlayerName;
        PostCoven<string>("coven/request", pData, pSuccess, pError);
    }

    // covens/coven/request --> req: {playerName: str} --> res: 200 | WSS --> command: coven_member_request
    public static void RequestCovenInvites(string sPlayerName, Action<CovenInvite> pSuccess, Action<string> pError)
    {
        PlayerRequestData pData = new PlayerRequestData();
        pData.playerName = sPlayerName;
        PostCoven<CovenInvite>("coven/covenInvites", pData, pSuccess, pError);
    }

    // covens/coven/join --> req: {inviteToken: str} --> res: 200 | WSS --> command: coven_member_join
    public static void Join(string sCovenName, Action<string> pSuccess, Action<string> pError)
    {
        var pData = Default(sCovenName);
        PostCoven<string>("coven/join", pData, pSuccess, pError);
    }
#endregion



     #region Member Requests

    // covens/coven/disband --> req: {} --> res: 200
    public static void Disband(string sCovenName, Action<string> pSuccess, Action<string> pError)
    {
        var pData = Default(sCovenName);
        DeleteCoven<string>("coven/disband", pData, pSuccess, pError);
    }

    // covens/coven/leave -->req:  {} --> res: 200 | WSS --> command: coven_member_leave
    public static void Leave(string sCovenName, Action<string> pSuccess, Action<string> pError)
    {
        var pData = Default(sCovenName);
        PostCoven<string>("coven/leave", pData, pSuccess, pError);
    }
    public static void Invite(string sCovenName, Action<string> pSuccess, Action<string> pError)
    {
        var pData = Default(sCovenName);
        PostCoven<string>("coven/invite", pData, pSuccess, pError);
    }

    // covens/coven/kick --> req: {memberId: str || memberName: str} --> res: 200 | WSS --> command: coven_member_kick
    public static void Kick(string sCovenName, Action<CovenData> pSuccess, Action<string> pError)
    {
        var pData = Default(sCovenName);
        PostCoven<CovenData>("coven/kick", pData, pSuccess, pError);
    }

    // covens/coven/promote --> req: {rank: int, memberId: str,  || memberName: str} --> res: 200 | WSS --> command: coven_member_promote, rank: int
    public static void Promote(string sCovenName, Action<CovenData> pSuccess, Action<string> pError)
    {
        var pData = Default(sCovenName);
        PostCoven<CovenData>("coven/promote", pData, pSuccess, pError);
    }

    // covens/coven/title --> req: {title: str, memberId: str,  || memberName: str} --> res: 200 | WSS --> command: coven_member_title, title: str
    public static void Title(string sCovenName, Action<CovenData> pSuccess, Action<string> pError)
    {
        var pData = Default(sCovenName);
        PostCoven<CovenData>("coven/title", pData, pSuccess, pError);
    }
      #endregion


    #region coven to coven

    //covens/coven/ally --> req: {covenName: str} --> res: 200
    public static void Ally(string sCovenName, Action<CovenData> pSuccess, Action<string> pError)
    {
        var pData = Default(sCovenName);
        PostCoven<CovenData>("coven/ally", pData, pSuccess, pError);
    }

    //covens/coven/unally --> req: {covenName: str} --> res: 200
    public static void Unally(string sCovenName, Action<CovenData> pSuccess, Action<string> pError)
    {
        var pData = Default(sCovenName);
        PostCoven<CovenData>("coven/unally", pData, pSuccess, pError);
    }

     #endregion




    //
    /*public static void GetCovenData(string sCovenName, Action<CovenData> pSuccess, Action<string> pError)
    {
        var pData = Default(sCovenName);
        PostCoven<CovenData>("coven/display", pData, pSuccess, pError);
    }*/


    static void Ally(string sCovenName)
    {
        var pData = Default(sCovenName);
        pData.covenName = sCovenName;
        PostCoven<CovenData>("coven/ally", pData, null, null);
    }

#endif
        #endregion


        /*covens/coven/request --> req: {covenName: str} --> res: 200 | WSS --> command: coven_member_request
        covens/coven/display --> req: {covenName: str} --> res: {coven info}
        covens/coven/ally --> req: {covenName: str} --> res: 200
        covens/coven/unally --> req: {covenName: str} --> res: 200
        covens/coven/create --> req: {covenName: str} --> res: 200*/




        #region inner post methods
        private static void PostCoven<T>(string sEndpoint, object pData, Action<T> Success, Action<string> Failure)
    {
        Action<string, int> pResponse = (string result, int response) =>
        {
            OnResponse<T>(result, response, Success, Failure);
        };
        APIManager.Instance.PostCoven(sEndpoint, JsonConvert.SerializeObject(pData), pResponse);
    }

    private static void PutCoven<T>(string sEndpoint, object pData, Action<T> Success, Action<string> Failure)
    {
        Action<string, int> pResponse = (string result, int response) =>
        {
            OnResponse<T>(result, response, Success, Failure);
        };
        APIManager.Instance.PutCoven(sEndpoint, JsonConvert.SerializeObject(pData), pResponse);
    }
    private static void GetCoven<T>(string sEndpoint, object pData, Action<T> Success, Action<string> Failure)
    {
        Action<string, int> pResponse = (string result, int response) =>
        {
            OnResponse<T>(result, response, Success, Failure);
        };
        APIManager.Instance.GetCoven(sEndpoint, JsonConvert.SerializeObject(pData), pResponse);
    }
    private static void DeleteCoven<T>(string sEndpoint, object pData, Action<T> Success, Action<string> Failure)
    {
        Action<string, int> pResponse = (string result, int response) =>
        {
            OnResponse<T>(result, response, Success, Failure);
        };
        APIManager.Instance.DeleteCoven(sEndpoint, JsonConvert.SerializeObject(pData), pResponse);
    }
    private static void OnResponse<T>(string result, int response, Action<T> Success, Action<string> Failure)
    {
        string sError = "";
        Log("Response: " + result + " response: " + response);
        //200 - success
        if (response == 200)
        {
            try
            {
                //parse the json data
                if(typeof(T) == typeof(String))
                {
                    if (Success != null)
                        Success((T) Convert.ChangeType(result, typeof(T)));
                    return;
                }
                T pResponseData = JsonConvert.DeserializeObject<T>(result);
                if (Success != null)
                    Success(pResponseData);
                Log("Success");
                return;
            }
            catch (Exception e)
            {
                sError = "Response Parsing Error: " + e.ToString();
            }
        }
        else
        {
            sError = "Response Error: " + response + " result: " + result;
        }

        if (Failure != null)
            Failure(sError);
        LogError(sError);
    }

    #endregion

    static void Log(string sLog)
    {
        Debug.Log("[CovenManagerAPI] " + sLog);
    }
    static void LogError(string sLog)
    {
        Debug.LogError("[CovenManagerAPI] " + sLog);
    }
}

/*
covens/coven/display --> req: {covenName: str} --> res: {coven info}

covens/coven/ally --> req: {covenName: str} --> res: 200

covens/coven/unally --> req: {covenName: str} --> res: 200

covens/coven/create --> req: {covenName: str} --> res: 200

covens/coven/disband --> req: {} --> res: 200

covens/coven/request --> req: {covenName: str} --> res: 200 | WSS --> command: coven_member_request

covens/coven/invite --> req: {invitedId: str || invitedName: str} --> res: 200 | WSS --> inviteToken

covens/coven/join --> req: {inviteToken: str} --> res: 200 | WSS --> command: coven_member_join

covens/coven/leave -->req:  {} --> res: 200 | WSS --> command: coven_member_leave

covens/coven/kick --> req: {memberId: str || memberName: str} --> res: 200 | WSS --> command: coven_member_kick

covens/coven/title --> req: {title: str, memberId: str,  || memberName: str} --> res: 200 | WSS --> command: coven_member_title, title: str

covens/coven/promote --> req: {rank: int, memberId: str,  || memberName: str} --> res: 200 | WSS --> command: coven_member_promote, rank: int

covens/coven/location --> req: {memberId || memberName} --> res: {latitude: float, longitude: float} --> covens/map/move --> req: {physical: bool, latitude: float, longitude: float}
*/
