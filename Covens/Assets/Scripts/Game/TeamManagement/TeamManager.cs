using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

public class TeamManager : MonoBehaviour
{

    #region GetRequests

    public static void GetCharacterInvites(Action<TeamInvites[]> OnReceiveData)
    {
        SendRequest<TeamInvites[]>(OnReceiveData, "character/invites");
    }

    public static void GetAlliedCoven(Action<TeamInvites[]> OnReceiveData)
    {
        SendRequest<TeamInvites[]>(OnReceiveData, "coven/display-allies");
    }

    public static void GetCovenAllied(Action<TeamInvites[]> OnReceiveData)
    {
        SendRequest<TeamInvites[]>(OnReceiveData, "coven/display-allied-covens");
    }

    public static void GetTopCovens(Action<LeaderboardData[]> OnReceiveData)
    {
        SendRequest<LeaderboardData[]>(OnReceiveData, "leaderboards/get-coven");
    }

    public static void GetCovenRequests(Action<TeamInvites[]> OnReceiveData)
    {
        SendRequest<TeamInvites[]>(OnReceiveData, "coven/pending-requests");
    }

    public static void GetCovenInvites(Action<TeamInvites[]> OnReceiveData)
    {
        SendRequest<TeamInvites[]>(OnReceiveData, "coven/pending-invites");
    }

    #endregion

    #region PostRequests

    public static void GetCovenDisplay(Action<TeamData> OnReceiveData)
    {
        var data = new { covenName = PlayerDataManager.playerData.covenName };
        Debug.Log(JsonConvert.SerializeObject(data));
        APIManager.Instance.PostData("coven/display", JsonConvert.SerializeObject(data), (string s, int r) =>
       {
           if (r == 200)
           {
               Debug.Log(s);
               OnReceiveData(JsonConvert.DeserializeObject<TeamData>(s));
           }
           else
           {
               Debug.Log(s);
           }
       });
    }

    public static void GetCovenDisplay(Action<TeamData> OnReceiveData, string coven)
    {
        var data = new { covenName = coven };
        APIManager.Instance.PostData("coven/display", JsonConvert.SerializeObject(data), (string s, int r) =>
       {
           if (r == 200)
               OnReceiveData(JsonConvert.DeserializeObject<TeamData>(s));
           else
           {
               Debug.Log(s);
           }
       });
    }

    public static void AllyCoven(Action<int> OnReceiveData, string id)
    {
        var data = new { coven = id };
        SendRequest(OnReceiveData, "coven/ally", JsonConvert.SerializeObject(data));
    }

    public static void UnallyCoven(Action<int> OnReceiveData, string id)
    {
        var data = new { coven = id };
        SendRequest(OnReceiveData, "coven/unally", JsonConvert.SerializeObject(data));
    }

    public static void CreateCoven(Action<int> OnReceiveData, string id)
    {
        var data = new { covenName = id };
        SendRequestPut(OnReceiveData, "coven/create", JsonConvert.SerializeObject(data));
    }

    public static void RequestInvite(Action<int> OnReceiveData, string id)
    {
        var data = new { covenName = id };
        Debug.Log(JsonConvert.SerializeObject(data));
        SendRequest(OnReceiveData, "coven/request", JsonConvert.SerializeObject(data));
    }

    public static void InviteCoven(Action<int> OnReceiveData, string id)
    {
        var data = new { invited = id };
        SendRequest(OnReceiveData, "coven/invite", JsonConvert.SerializeObject(data));
    }

    public static void JoinCoven(Action<int> OnReceiveData, string id)
    {
        var data = new { inviteToken = id };
        SendRequest(OnReceiveData, "coven/join", JsonConvert.SerializeObject(data));
    }

    public static void SetMotto(Action<int> OnReceiveData, string id)
    {
        var data = new { motto = id };
        SendRequest(OnReceiveData, "coven/set-motto", JsonConvert.SerializeObject(data));
    }

    public static void CovenKick(Action<int> OnReceiveData, string id)
    {
        var data = new { kicked = id };
        SendRequest(OnReceiveData, "coven/kick", JsonConvert.SerializeObject(data));
    }

    public static void CovenDecline(Action<int> OnReceiveData, string id)
    {
        var data = new { inviteToken = id };
        SendRequest(OnReceiveData, "coven/decline", JsonConvert.SerializeObject(data));
    }

    public static void CovenReject(Action<int> OnReceiveData, string id)
    {
        var data = new { inviteToken = id };
        SendRequest(OnReceiveData, "coven/reject", JsonConvert.SerializeObject(data));
    }

    public static void CovenCancel(Action<int> OnReceiveData, string id)
    {
        var data = new { inviteToken = id };
        SendRequest(OnReceiveData, "coven/cancel", JsonConvert.SerializeObject(data));
    }

    public static void CovenLeave(Action<int> OnReceiveData)
    {
        var data = new { coven = PlayerDataManager.playerData.covenName };
        APIManager.Instance.GetData("coven/leave", (string s, int r) =>
        {
            OnReceiveData(r);
        });
    }

    public static void CovenDisband(Action<int> OnReceiveData)
    {
        var data = new { coven = PlayerDataManager.playerData.covenName };
        APIManager.Instance.DeleteData("coven/disband", (string s, int r) =>
        {
            OnReceiveData(r);
        });
    }

    public static void ViewCharacter(string id)
    {
        var data = new { target = id };
        APIManager.Instance.PostData("chat/select", JsonConvert.SerializeObject(data), TeamManagerUI.Instance.GetViewCharacter);
    }

    #endregion

    static void SendRequest<T>(Action<T> OnReceiveData, string URL)
    {
        APIManager.Instance.GetData(URL, (string s, int r) =>
        {
            Debug.Log(s);
            if (r == 200)
                OnReceiveData(JsonConvert.DeserializeObject<T>(s));
            // else
            //     Debug.Log(s);
        });
    }

    static void SendRequest(Action<int> OnReceiveData, string URL, string jsonData)
    {
        APIManager.Instance.PostData(URL, jsonData, (string s, int r) =>
        {
            int resCode;
            if (r == 200)
                resCode = 200;
            else
            {
                if (int.TryParse(s, out resCode))
                {

                }
                else
                {
                    resCode = 4300;
                }
            }
            Debug.Log(resCode);
            OnReceiveData(resCode);
        });
    }

    static void SendRequestPut(Action<int> OnReceiveData, string URL, string jsonData)
    {
        APIManager.Instance.PutData(URL, jsonData, (string s, int r) =>
        {
            print(s);
            OnReceiveData(r);
        });
    }
}

public enum TeamPrefabType
{
    Member, InviteRequest, Ally, UnAlly
}

public class TeamData
{
    public double createdOn { get; set; }
    public string covenMotto { get; set; }
    public string coven { get; set; }
    public double disbandedOn { get; set; }
    public string covenName { get; set; }
    public string dominion { get; set; }
    public int covenDegree { get; set; }
    public int creatorDegree { get; set; }
    public int rank { get; set; }
    public int score { get; set; }
    public int dominionRank { get; set; }
    public string createdBy { get; set; }
    public int totalSilver { get; set; }
    public int totalGold { get; set; }
    public int totalEnergy { get; set; }
    public TeamLocation[] controlledLocations { get; set; }
    public List<TeamMember> members { get; set; }
}

public class TeamMember
{
    public string state { get; set; }
    public string displayName { get; set; }
    public string title { get; set; }
    public int level { get; set; }
    public int degree { get; set; }
    public int role { get; set; }
    public double joinedOn { get; set; }
    public double lastActiveOn { get; set; }
}

public class TeamLocation
{
    public string instance { get; set; }
    public string displayName { get; set; }
    public string timestamp { get; set; }
    public double latitude { get; set; }
    public double longitude { get; set; }
}

public class TeamInvites
{
    public string inviteToken { get; set; }
    public string covenName { get; set; }
    public double timestamp { get; set; }
    public string coven { get; set; }
    public int rank { get; set; }
}