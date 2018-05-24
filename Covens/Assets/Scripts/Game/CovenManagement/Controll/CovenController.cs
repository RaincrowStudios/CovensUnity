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
    private List<CovenMember> m_JoinRequestList = new List<CovenMember>();
    private CovenInvite m_CovenInvite;



    public CovenController(string sCovenId)
    {
        CovenId = sCovenId;
        //CovenName = sCovenName;
    }



    #region gets

    public CovenData Data
    {
        get { return m_LastCovenData; }
        private set { m_LastCovenData = value; }
    }

    public CovenOverview[] AlliedCovens
    {
        get { return Player.Data.alliedCovens; }
    }
    public CovenOverview[] Allies
    {
        get { return Player.Data.allies; }
    }

    
    public string PlayerName
    {
        get { return PlayerDataManager.playerData.displayName; }
    }

    public bool IsInCoven
    {
        get { return !string.IsNullOrEmpty(CovenId); }
    }
    public string CovenId
    {
        get; set;
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
        set { if (value) Data = null; }
    }

    public bool IsDataLoaded
    {
        get { return Data != null; }
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

    public int MembersRequest
    {
        get { return 2; }
    }
    public int AlliancesRequest
    {
        get { return GetAllianceRequests(); }
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
    public bool CanPromoteUser(CovenMember pUser)
    {
        CovenRole eUserRole = ParseRole(pUser.role);
        if (CurrentRole == CovenRole.Member || eUserRole == CovenRole.Administrator)
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

    /// <summary>
    /// returns a list of allied covens that are not our alli
    /// </summary>
    /// <param name="pCovenOverview"></param>
    /// <returns></returns>
    public int GetAllianceRequests()
    {
        if (Player.Data == null)
            return 0;

        int iCount = 0;
        for (int i = 0; i < AlliedCovens.Length; i++)
        {
            bool bIsAllied = false;
            for (int j = 0; j < Allies.Length; j++)
            {
                if (AlliedCovens[i].coven == Allies[j].coven)
                {
                    bIsAllied = true;
                }
            }
            if (bIsAllied)
                iCount++;
        }
        return iCount;
    }
    public List<CovenOverview> GetAllianceRequestsList()
    {
        if (Player.Data == null)
            return null;

        List<CovenOverview> pList = new List<CovenOverview>(AlliedCovens);
        for (int i = 0; i < Allies.Length; i++)
        {
            bool bFound = false;
            for (int j = 0; j < pList.Count; j++)
            {
                if (AlliedCovens[i].coven == pList[j].coven)
                {
                    bFound = true;
                }
            }
            if(!bFound)
            {
                pList.Add(Allies[i]);
            }
        }
        return pList;
    }

    public bool CheckIfCovenIsAnAlly(CovenOverview pCovenOverview)
    {
        if (Player.Data == null)
            return false;

        for (int i = 0; i < Allies.Length; i++)
        {
            if (pCovenOverview.coven == Allies[i].coven)
                return true;
        }

        return false;
    }

    public void Setup(CovenOverview pCovenOverview)
    {
        IsCovenAnAlly = CheckIfCovenIsAnAlly(pCovenOverview);// pCovenOverview.isAlly;
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

    private void UpdateCovenDataResponse(CovenData pData, Action<CovenData> pSuccess)
    {
        Data = pData;
        CovenName = pData.covenName;
        if (pSuccess != null)
            pSuccess(pData);
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

        CovenManagerAPI.CharacterInvites(PlayerName, pSuccess, pError);
    }

    /// <summary>
    /// requests to a coven to enter
    /// </summary>
    /// <param name="sCovenName"></param>
    /// <param name="pSuccess"></param>
    /// <param name="pError"></param>
    public void RequestJoinCoven(string sCovenName, Action<string> pSuccess, Action<string> pError)
    {
        CovenManagerAPI.CovenRequest(sCovenName, pSuccess, pError);
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
        Action<CovenData> Success = (CovenData pData) => { UpdateCovenDataResponse(pData, pSuccess); };
        CovenManagerAPI.CovenDisplay(CovenId, Success, pError);
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
        CovenManagerAPI.CovenJoin(sCovenName, Success, pError);
    }
    #endregion



    #region members

    public void Disband(Action<string> pSuccess, Action<string> pError)
    {
        CovenManagerAPI.CovenDisband(CovenName, pSuccess, pError);
    }
    public void LeaveCoven(Action<string> pSuccess, Action<string> pError)
    {
        CovenManagerAPI.CovenLeave(CovenName, PlayerName, pSuccess, pError);
    }
    public void InvitePlayer(string sPlayerName, Action<string> pSuccess, Action<string> pError)
    {
        CovenManagerAPI.CovenInvite(CovenName, sPlayerName, pSuccess, pError);
    }
    public void Kick(string sUserName, Action<string> pSuccess, Action<string> pError)
    {
        //Action<CovenData> Success = (CovenData pData) => { UpdateCovenDataResponse(pData, pSuccess); };
        CovenManagerAPI.CovenKick(CovenName, sUserName, pSuccess, pError);
    }
    public void PromoteMember(string sUserName, Action<string> pSuccess, Action<string> pError)
    {
        //Action<CovenData> Success = (CovenData pData) => { UpdateCovenDataResponse(pData, pSuccess); };
        CovenManagerAPI.CovenPromote(CovenName, sUserName, pSuccess, pError);
    }
    public void UpdateCovensTitles(string sUserName, string sTitle, Action<string> pSuccess, Action<string> pError)
    {
        CovenManagerAPI.CovenTitle(CovenName, sUserName, sTitle, pSuccess, pError);
    }
    public void AcceptMember(string sUserName, Action<string> pSuccess, Action<string> pError)
    {
        //Action<CovenData> Success = (CovenData pData) => { UpdateCovenDataResponse(pData, pSuccess); };
        CovenManagerAPI.CovenAccept(CovenName, sUserName, pSuccess, pError);
    }
    public void RejectMember(string sUserName, Action<string> pSuccess, Action<string> pError)
    {
        CovenManagerAPI.CovenReject(CovenName, sUserName, pSuccess, pError);
    }
    public void CovenViewPending(Action<MemberInvite> pSuccess, Action<string> pError)
    {
        CovenManagerAPI.CovenViewPending(CovenName, pSuccess, pError);
    }

    #endregion


    #region Coven invite-alliance

    public void Ally(string sCovenName, Action<string> pSuccess, Action<string> pError)
    {
        CovenManagerAPI.CovenAlly(sCovenName, pSuccess, pError);
    }
    public void Unally(string sCovenName, Action<string> pSuccess, Action<string> pError)
    {
        CovenManagerAPI.CovenUnally(sCovenName, pSuccess, pError);
    }
    #endregion


    #region invitation

    public int GetRequestCount()
    {
        return m_JoinRequestList.Count;
    }
    public List<CovenMember> GetRequestList()
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
