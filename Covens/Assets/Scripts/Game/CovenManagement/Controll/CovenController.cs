using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


/// <summary>
/// Coven's logic goes here
/// </summary>
[System.Serializable]
public partial class CovenController //: Patterns.SingletonComponent<CovenController>
{
    private static CovenController m_PlayerInstance;
    public static CovenController Player
    {
        get
        {
            if (m_PlayerInstance == null)
            {
                m_PlayerInstance = new CovenController(PlayerDataManager.playerData.coven);
            }
            return m_PlayerInstance;
        }
        set { m_PlayerInstance = value; }
    }


    private CovenData m_LastCovenData;
    private List<CovenData> m_CovenRequests = new List<CovenData>();
    private List<CovenItem> m_JoinRequestList = new List<CovenItem>();
    private CovenInvite m_CovenInvite;



    public CovenController( string sCovenName)
    {
        CovenName = sCovenName;
    }


    #region gets

    public string PlayerName
    {
        get { return PlayerDataManager.playerData.displayName; }
    }

    public bool IsInCoven
    {
        get { return !string.IsNullOrEmpty(CovenName); }
    }
    public string CovenName
    {
        get; set;
    }
    public string CovenOwner
    {
        //get { return PlayerDataManager.playerData.ownerCoven; }
        get { return "myself"; }
    }
    public bool CanJoinCoven
    {
        get { return !IsInCoven; }
    }
    public CovenRole CurrentRole
    {
        //get { return CovenRole.Administrator; }
        get {
            return CovenRole.Administrator;
        }
        set { }
    }

    public bool NeedsReload
    {
        get { return !IsDataLoaded; }
        set { if (value) m_LastCovenData = null; }
    }

    public bool IsDataLoaded
    {
        get { return m_LastCovenData != null; }
    }
    public bool IsPlayerCoven
    {
        get { return this == Player; }
    }
    // for non player covens, they could be trying to request an alliance
    //public bool IsCovenRequestingAlliance
    //{
    //    get;set;
    //}
    public bool IsCovenAnAlly
    {
        get;set;
    }

#endregion


    /// <summary>
    /// check if user can be promoted by me
    /// - Admin can promote:
    ///     - Member to Moderator
    ///     - Moderator to Admin
    /// - Moderator can promote:
    ///     - Member to Moderator
    /// - Member can do nothing
    /// </summary>
    /// <param name="pUser"></param>
    /// <returns></returns>
    public bool CanPromoteUser(CovenItem pUser)
    {
        CovenRole eUserRole = ParseRole(pUser.role);
        if (CurrentRole == CovenRole.Member || CurrentRole == CovenRole.None || eUserRole == CovenRole.Administrator)
            return false;
        if(CurrentRole == CovenRole.Administrator)
        {
            return eUserRole == CovenRole.Moderator || eUserRole == CovenRole.Member;
        }
        if (CurrentRole == CovenRole.Moderator)
        {
            return eUserRole == CovenRole.Member;
        }
        return false;
    }


    public void Setup(CovenOverview pCovenOverview)
    {
        IsCovenAnAlly = pCovenOverview.isAlly;
        Setup(pCovenOverview.covenName);
    }
    public void Setup(string sCovenName)
    {
        CovenName = sCovenName;
        if (IsPlayerCoven)
        {
            PlayerDataManager.playerData.coven = CovenName;
        }
    }

    #region not a member requests




    public CovenInvite GetCurrentInvites()
    {
        return m_CovenInvite;
    }



    /// <summary>
    /// requests the covens who wants to join you
    /// </summary>
    /// <param name="pOnComplete"></param>
    public void RequestCovenInvites(Action<CovenInvite, string> pOnComplete)
    {
        if (!IsPlayerCoven)
            pOnComplete(null, "Not allowed Action");

        Action<CovenInvite> pSuccess = (CovenInvite pInvites) => {
            m_CovenInvite = pInvites;
            pOnComplete(pInvites, null);
        };
        Action<string> pError = (string sError) => {
            pOnComplete(null, sError);
        };

        CovenManagerAPI.RequestCovenInvites(PlayerName, pSuccess, pError);
    }

    /// <summary>
    /// requests to a coven to enter
    /// </summary>
    /// <param name="sCovenName"></param>
    /// <param name="pSuccess"></param>
    /// <param name="pError"></param>
    public void RequestJoinCoven(string sCovenName, Action<string> pSuccess, Action<string> pError)
    {
        CovenManagerAPI.RequestJoinCoven(sCovenName, pSuccess, pError);
    }
    /// <summary>
    /// creates a coven
    /// </summary>
    /// <param name="sCovenName"></param>
    /// <param name="pSuccess"></param>
    /// <param name="pError"></param>
    public void CreateCoven(string sCovenName, Action<string> pSuccess, Action<string> pError)
    {
        Action<string> Success = (string sOk) => 
        {
            Setup(sCovenName);
            if (pSuccess != null)
                pSuccess("ok");
        };
        CovenManagerAPI.CreateCoven(sCovenName, Success, pError);
    }
    /// <summary>
    /// displays a coven
    /// </summary>
    /// <param name="pSuccess"></param>
    /// <param name="pError"></param>
    public void RequestDisplayCoven(Action<CovenData> pSuccess, Action<string> pError)
    {
        Action<CovenData> Success = (CovenData pData) => { UpdatePlayerDataResponse(pData, pSuccess); };
        CovenManagerAPI.RequestDisplayCoven(CovenName, Success, pError);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sCovenName"></param>
    /// <param name="pSuccess"></param>
    /// <param name="pError"></param>
    public void JoinCoven(string sCovenName, Action pSuccess, Action<string> pError)
    {
        Action<string> Success = (string sOk) =>
        {
            // update the player data
            Setup(sCovenName);
            if (pSuccess != null)
                pSuccess();
        };
        CovenManagerAPI.Join(sCovenName, Success, pError);
    }
    #endregion



    private void UpdatePlayerDataResponse(CovenData pData, Action<CovenData> pSuccess)
    {
        Player.m_LastCovenData = pData;
        if (pSuccess != null)
            pSuccess(pData);
    }

    #region members

    public void Disband(Action<string> pSuccess, Action<string> pError)
    {
        CovenManagerAPI.Disband(CovenName, pSuccess, pError);
    }
    public void LeaveCoven(Action<string> pSuccess, Action<string> pError)
    {
        CovenManagerAPI.Leave(CovenName, PlayerName, pSuccess, pError);
    }
    public void InvitePlayer(string sPlayerName, Action<string> pSuccess, Action<string> pError)
    {
        CovenManagerAPI.Invite(CovenName, sPlayerName, pSuccess, pError);
    }
    public void Kick(string sUserName, Action<CovenData> pSuccess, Action<string> pError)
    {
        Action<CovenData> Success = (CovenData pData) => { UpdatePlayerDataResponse(pData, pSuccess); };
        CovenManagerAPI.Kick(CovenName, sUserName, Success, pError);
    }
    public void PromoteMember(string sUserName, CovenRole eRole, Action<CovenData> pSuccess, Action<string> pError)
    {
        Action<CovenData> Success = (CovenData pData) => { UpdatePlayerDataResponse(pData, pSuccess); };
        CovenManagerAPI.Promote(CovenName, sUserName, (int)eRole, Success, pError);
    }
    public void UpdateCovensTitles(string sUserName, string sTitle, Action<string> pSuccess, Action<string> pError)
    {
        CovenManagerAPI.Title(CovenName, sUserName, sTitle, pSuccess, pError);
    }
    
    public void AcceptMember(string sUserName, Action<CovenData> pSuccess, Action<string> pError)
    {
        Action<CovenData> Success = (CovenData pData) => { UpdatePlayerDataResponse(pData, pSuccess); };
        CovenManagerAPI.Accept(CovenName, sUserName, Success, pError);
    }
    public void RejectMember(string sUserName, Action<string> pSuccess, Action<string> pError)
    {
        CovenManagerAPI.Reject(CovenName, sUserName, pSuccess, pError);
    }
    #endregion


    #region Coven invite-alliance

    public void Ally(string sCovenName, Action<string> pSuccess, Action<string> pError)
    {
        CovenManagerAPI.Ally(sCovenName, pSuccess, pError);
    }
    public void Unally(string sCovenName, Action<string> pSuccess, Action<string> pError)
    {
        CovenManagerAPI.Unally(sCovenName, pSuccess, pError);
    }
    public void AllyList(Action<CovenInvite> pSuccess, Action<string> pError)
    {
        CovenManagerAPI.AllyList(CovenName, pSuccess, pError);
    }
    public void RequestList(Action<MemberInvite> pSuccess, Action<string> pError)
    {
        CovenManagerAPI.RequestList(CovenName, pSuccess, pError);
    }
    #endregion


    #region invitation

    public CovenData GetPlayerCovenData()
    {
        return m_LastCovenData;
    }
    public int GetRequestCount()
    {
        return m_JoinRequestList.Count;
    }
    public List<CovenItem> GetRequestList()
    {
        return m_JoinRequestList;
    }

    public List<CovenData> GetCovenRequests()
    {
        return m_CovenRequests;
    }

    #endregion


    #region general calls
    public void FindPlayer(string sUserName, Action<StringItens> pSuccess, Action<string> pError)
    {
        CovenManagerAPI.FindPlayer(sUserName, false, pSuccess, pError);
    }
    public void FindCoven(string sCovenName, Action<StringItens> pSuccess, Action<string> pError)
    {
        CovenManagerAPI.FindCoven(sCovenName, pSuccess, pError);
    }
    #endregion

}
