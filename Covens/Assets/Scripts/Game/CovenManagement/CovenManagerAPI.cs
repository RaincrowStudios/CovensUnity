//#define LOCAL
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

public class CovenManagerAPI
{
    static CovenRequest_ByName RequestByCovenName(string sName)
    {
        var data = new CovenRequest_ByName();
        data.covenName = sName;
        return data;
    }
    static CovenRequest_ByInstance RequestByCovenInstance(string sInstance)
    {
        var data = new CovenRequest_ByInstance();
        data.coven = sInstance;
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
    public static void CovenDisplay(string sCovenInstance, string sCovenName, Action<CovenData> pSuccess, Action<string> pError)
    {
        object pData = null;
        if (!string.IsNullOrEmpty(sCovenInstance))
        {
            pData = RequestByCovenInstance(sCovenInstance);
        }
        else
        {
            if (string.IsNullOrEmpty(sCovenInstance))
            {
                pData = RequestByCovenName(sCovenName);
            }
        }
        if(pData == null)
        {
            Debug.LogError("NOOOOOOO sCovenInstance[" + sCovenInstance + "] sCovenName[" + sCovenName + "]");
            pError("shit...");
            return;
        }
#if SERVER_FAKE
        PostCoven<CovenData>("coven/display" /*+ sCovenName*/, pData, pSuccess, pError);
#else
        PostCoven<CovenData>("coven/display", pData, pSuccess, pError);
#endif
    }
    // covens/coven/create --> req: {covenName: str} --> res: 200
    public static void CreateCoven(string sCovenName, Action<string> pSuccess, Action<string> pError)
    {
        var pData = RequestByCovenName(sCovenName);
        PutCoven<string>("coven/create", pData, pSuccess, pError);
    }

    // covens/coven/request --> req: {covenName: str} --> res: 200 | WSS --> command: coven_member_request
    public static void CovenRequest(string sCovenName, Action<string> pSuccess, Action<string> pError)
    {
        var pData = RequestByCovenName(sCovenName);
        PostCoven<string>("coven/request", pData, pSuccess, pError);
    }

    // covens/coven/covenInvites (new)
    public static void CharacterInvites(string sPlayerName, Action<CovenOverview[]> pSuccess, Action<string> pError)
    {
        PlayerRequestData pData = new PlayerRequestData();
        pData.playerName = sPlayerName;
        GetCoven<CovenOverview[]>("character/invites", pData, pSuccess, pError);
    }

    // covens/coven/join --> req: {inviteToken: str} --> res: 200 | WSS --> command: coven_member_join
    public static void CovenJoin(string sCovenToken, Action<string> pSuccess, Action<string> pError)
    {
        var pData = new CovenRequest_Join();
        pData.inviteToken = sCovenToken;
        PostCoven<string>("coven/join", pData, pSuccess, pError);
    }
    #endregion


    #region Member Requests

    // covens/coven/disband --> req: {} --> res: 200
    public static void CovenDisband(string sCovenName, Action<string> pSuccess, Action<string> pError)
    {
        var pData = RequestByCovenName(sCovenName);
        DeleteCoven<string>("coven/disband", pData, pSuccess, pError);
    }

    // covens/coven/leave -->req:  {} --> res: 200 | WSS --> command: coven_member_leave
    public static void CovenLeave(string sCovenName, string sPlayerName, Action<string> pSuccess, Action<string> pError)
    {
        var pData = RequestByCovenName(sCovenName);
        GetCoven<string>("coven/leave", pData, pSuccess, pError);
    }
    // covens/coven/invite --> req: {invitedId: str || invitedName: str} --> res: 200 | WSS --> inviteToken
    public static void CovenInvite(string sCovenName, string sPlayerName, Action<string> pSuccess, Action<string> pError)
    {
        var pData = new CovenRequest_Invite();
        pData.invitedName = sPlayerName;
        PostCoven<string>("coven/invite", pData, pSuccess, pError);
    }

    // covens/coven/kick --> req: {memberId: str || memberName: str} --> res: 200 | WSS --> command: coven_member_kick
    public static void CovenKick(string sCovenName, string sUserName, Action<string> pSuccess, Action<string> pError)
    {
        var pData = new CovenRequest_Kick();
        pData.kickedName = sUserName;
        PostCoven<string>("coven/kick", pData, pSuccess, pError);
    }

    // covens/coven/promote --> req: {role: int, memberId: str,  || memberName: str} --> res: 200 | WSS --> command: coven_member_promote, role: int
    // rank => role
    public static void CovenPromote(string sCovenName, string sUserName, CovenController.CovenRole eToRole, Action<string> pSuccess, Action<string> pError)
    {
        var pData = new CovenRequest_Promote();
        pData.promotedName = sUserName;
        pData.promotion = (int)eToRole;
        PostCoven<string>("coven/promote", pData, pSuccess, pError);
    }

    // covens/coven/title --> req: {title: str, memberId: str,  || memberName: str} --> res: 200 | WSS --> command: coven_member_title, title: str
    public static void CovenTitle(string sCovenName, string sUserName, string sTitle, Action<string> pSuccess, Action<string> pError)
    {
        var pData = new CovenRequest_Title();
        pData.title = sTitle;
        pData.titledName = sUserName;
        PostCoven<string>("coven/title", pData, pSuccess, pError);
    }
    // covens/coven/accept --> req: {title: str, memberId: str,  || memberName: str} --> res: 200 | WSS --> command: coven_member_title, title: str
    /*public static void CovenAccept(string sCovenName, string sUserName, Action<string> pSuccess, Action<string> pError)
    {
        var pData = new CovenPlayerRequestData();
        pData.covenName = sCovenName;
        pData.playerName = sUserName;
        PostCoven<string>("coven/accept", pData, pSuccess, pError);
    }*/
    // covens/coven/reject --> req: {title: str, memberId: str,  || memberName: str} --> res: 200
    public static void CovenReject(string sCovenName, string sUserName, Action<string> pSuccess, Action<string> pError)
    {
        var pData = new CovenPlayerRequestData();
        //pData.covenName = sCovenName;
        pData.request = sUserName;
        PostCoven<string>("coven/reject", pData, pSuccess, pError);
    }

    public static void CovenViewPending(string sCovenName, Action<MemberInvite> pSuccess, Action<string> pError)
    {
        var pData = RequestByCovenName(sCovenName);
        GetCoven<MemberInvite>("coven/view-pending", pData, pSuccess, pError);
    }
    #endregion


    #region coven to coven

    //covens/coven/ally --> req: {covenName: str} --> res: 200
    public static void CovenAlly(string sCovenName, Action<string> pSuccess, Action<string> pError)
    {
        var pData = RequestByCovenName(sCovenName);
        PostCoven<string>("coven/ally", pData, pSuccess, pError);
    }

    //covens/coven/unally --> req: {covenName: str} --> res: 200
    public static void CovenUnally(string sCovenName, Action<string> pSuccess, Action<string> pError)
    {
        var pData = RequestByCovenName(sCovenName);
        PostCoven<string>("coven/unally", pData, pSuccess, pError);
    }
    //// covens/coven/allyList --> req: {covenName: str} --> res: CovenInvite
    //public static void AllyList(string sCovenName, Action<CovenInvite> pSuccess, Action<string> pError)
    //{
    //    var pData = Default(sCovenName);
    //    PostCoven<CovenInvite>("coven/ally-list", pData, pSuccess, pError);
    //}
    //// covens/coven/requestList --> req: {covenName: str} --> res: CovenInvite
    //public static void RequestList(string sCovenName, Action<MemberInvite> pSuccess, Action<string> pError)
    //{
    //    var pData = Default(sCovenName);
    //    PostCoven<MemberInvite>("coven/request-list", pData, pSuccess, pError);
    //}
    #endregion


    #region general calls


    public static void FindPlayer(string sUserName, bool bHasCoven, Action<FindResponse> pSuccess, Action<string> pError)
    {
        var pData = new FindRequest();
        pData.query = sUserName;

#if SERVER_FAKE
        Action<FindResponse> Success = (FindResponse pSuc) =>
        {
            for (int i = 0; i < pSuc.matches.Length; i++)
            {
                pSuc.matches[i] = pSuc.matches[i].Replace("#str#", sUserName);
            }
            if (pSuccess != null)
                pSuccess(pSuc);
        };
        PostCoven<FindResponse>("coven/find-player", pData, Success, pError);
#else
        PostCoven<FindResponse>("coven/find-player", pData, pSuccess, pError);
#endif
    }
    public static void FindCoven(string sCovenName, Action<FindResponse> pSuccess, Action<string> pError)
    {
        var pData = new FindRequest();
        pData.query = sCovenName;
#if SERVER_FAKE
        Action<FindResponse> Success = (FindResponse pSuc) =>
        {
            for (int i = 0; i < pSuc.matches.Length; i++)
            {
                pSuc.matches[i] = pSuc.matches[i].Replace("#str#", sCovenName);
            }
            if (pSuccess != null)
                pSuccess(pSuc);
        };
        PostCoven<FindResponse>("coven/find-coven", pData, Success, pError);
#else
        PostCoven<FindResponse>("coven/find-coven", pData, pSuccess, pError);
#endif
    }

    #endregion


#endif
    #endregion





    #region inner post methods

    static JsonSerializerSettings notNull = new JsonSerializerSettings
    {
        NullValueHandling = NullValueHandling.Ignore
    };

    private static string Serialize(object pData)
    {
        return JsonConvert.SerializeObject(pData, notNull);
    }



    public static void PostCoven<T>(string sEndpoint, object pData, Action<T> Success, Action<string> Failure)
    {
        Action<string, int> pResponse = (string result, int response) =>
        {
            OnResponse<T>(sEndpoint, result, response, Success, Failure);
        };
        APIManager.Instance.Post(sEndpoint, Serialize(pData), pResponse);
    }

    public static void PutCoven<T>(string sEndpoint, object pData, Action<T> Success, Action<string> Failure)
    {
        Action<string, int> pResponse = (string result, int response) =>
        {
            OnResponse<T>(sEndpoint, result, response, Success, Failure);
        };
        APIManager.Instance.Put(sEndpoint, Serialize(pData), pResponse);
    }
    public static void GetCoven<T>(string sEndpoint, object pData, Action<T> Success, Action<string> Failure)
    {
        Action<string, int> pResponse = (string result, int response) =>
        {
            OnResponse<T>(sEndpoint, result, response, Success, Failure);
        };
        APIManager.Instance.Get(sEndpoint, Serialize(pData), pResponse);
    }
    public static void DeleteCoven<T>(string sEndpoint, object pData, Action<T> Success, Action<string> Failure)
    {
        Action<string, int> pResponse = (string result, int response) =>
        {
            OnResponse<T>(sEndpoint, result, response, Success, Failure);
        };
        APIManager.Instance.Delete(sEndpoint, Serialize(pData), pResponse);
    }
    private static void OnResponse<T>(string sEndpoint, string result, int response, Action<T> Success, Action<string> Failure)
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
                T pResponseData = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(result);
                //T pResponseData = JsonConvert.DeserializeObject<T>(result);
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
            string sErrorMessage = Oktagon.Localization.Lokaki.GetText(result);
            // detailed error
            if(CovenConstants.Debug)
                sError = "Response \nError: '" + sErrorMessage + "'[" + response + "]\n result: " + result;
            else
                sError = sErrorMessage;
        }

        if (Failure != null)
        {
            if (CovenConstants.Debug)
                Failure("==> [" + sEndpoint + "]\n" + sError);
            else
                Failure(sError);
        }
        LogError("[" + sEndpoint + "] " + sError);
    }

    #endregion

    static void Log(string sLog)
    {
//        Debug.Log("[CovenManagerAPI] " + sLog);
    }
    static void LogError(string sLog)
    {
        Debug.LogError("[CovenManagerAPI] " + sLog);
    }
}

