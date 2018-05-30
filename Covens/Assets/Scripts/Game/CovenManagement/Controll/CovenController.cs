using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


/// <summary>
/// Coven's logic goes here
/// </summary>
[System.Serializable]
public partial class CovenController
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

    private bool m_bIsCreatingCoven = false;


    public CovenController(string sCovenId)
    {
        CovenId = sCovenId;
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
        get { return PlayerDataManager.playerData.ownerCoven; }
    }
    public bool CanJoinCoven
    {
        get { return !IsInCoven; }
    }
    public CovenRole CurrentRole
    {
        get;
        set;
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
    public bool CanKickUser(CovenMember pUser)
    {
        CovenRole eUserRole = ParseRole(pUser.role);
        if (CurrentRole == CovenRole.Member || eUserRole == CovenRole.Administrator)
            return false;
        if (CurrentRole == CovenRole.Administrator)
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
        Setup(pCovenOverview.covenName, pCovenOverview.coven);
    }
    public void Setup(string sCovenName, string sCovenId = null)
    {
        CovenName = sCovenName;
        if (IsPlayerCoven)
        {
            PlayerDataManager.Instance.OnPlayerJoinCoven(sCovenId);
        }
    }

    private void UpdateCovenDataResponse(CovenData pData, Action<CovenData> pSuccess)
    {
        Data = pData;
        CovenName = pData.covenName;
        CovenId = pData.coven;
        if (IsPlayerCoven)
        {
            foreach( var pMember in pData.members)
            {
                if(pMember.displayName == PlayerName)
                {
                    CurrentRole = ParseRole(pMember.role);
                    break;
                }
            }
            if (m_bIsCreatingCoven)
            {
                PlayerDataManager.Instance.OnPlayerJoinCoven(pData.coven);
                m_bIsCreatingCoven = false;
            }
        }
        if (pSuccess != null)
            pSuccess(pData);
    }
    public void OnLeaveCoven(Action<string> pSuccess)
    {
        Data = null;
        CovenName = null;
        CovenId = null;
        if (IsPlayerCoven)
        {
            PlayerDataManager.Instance.OnPlayerLeaveCoven();
        }
        if (pSuccess != null)
            pSuccess(null);
    }


    #region Request - not a member


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

        Action<CovenInvite> Success = (CovenInvite pInvites) => {
            m_CovenInvite = pInvites;
            pOnComplete(pInvites, null);
        };
        Action<string> Error = (string sError) => {
            pOnComplete(null, sError);
        };

        CovenManagerAPI.CharacterInvites(PlayerName, Success, Error);
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
        m_bIsCreatingCoven = true;
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


    #region Request - members

    public void Disband(Action<string> pSuccess, Action<string> pError)
    {
        CovenManagerAPI.CovenDisband(CovenName, pSuccess, pError);
    }
    public void LeaveCoven(Action<string> pSuccess, Action<string> pError)
    {
        Action<string> Success = (string sOk) => { OnLeaveCoven(pSuccess); };
        CovenManagerAPI.CovenLeave(CovenName, PlayerName, Success, pError);
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
    public void PromoteMember(string sUserName, CovenController.CovenRole eToRole, Action<string> pSuccess, Action<string> pError)
    {
        //Action<CovenData> Success = (CovenData pData) => { UpdateCovenDataResponse(pData, pSuccess); };
        CovenManagerAPI.CovenPromote(CovenName, sUserName, eToRole, pSuccess, pError);
    }
    public void UpdateCovensTitles(string sUserName, string sTitle, Action<string> pSuccess, Action<string> pError)
    {
        CovenManagerAPI.CovenTitle(CovenName, sUserName, sTitle, pSuccess, pError);
    }
    public void AcceptMember(string sUserName, Action<string> pSuccess, Action<string> pError)
    {
        //Action<CovenData> Success = (CovenData pData) => { UpdateCovenDataResponse(pData, pSuccess); };
        //CovenManagerAPI.CovenAccept(CovenName, sUserName, pSuccess, pError);
        CovenManagerAPI.CovenInvite(CovenName, sUserName, pSuccess, pError);
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


    #region Request - Coven invite-alliance

    public void Ally(string sCovenName, Action<string> pSuccess, Action<string> pError)
    {
        CovenManagerAPI.CovenAlly(sCovenName, pSuccess, pError);
    }
    public void Unally(string sCovenName, Action<string> pSuccess, Action<string> pError)
    {
        CovenManagerAPI.CovenUnally(sCovenName, pSuccess, pError);
    }
    #endregion


    /*#region invitation

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

    #endregion*/


    #region Request - general calls
    public void FindPlayer(string sUserName, Action<FindResponse> pSuccess, Action<string> pError)
    {
        CovenManagerAPI.FindPlayer(sUserName, false, pSuccess, pError);
    }
    public void FindCoven(string sCovenName, Action<FindResponse> pSuccess, Action<string> pError)
    {
        CovenManagerAPI.FindCoven(sCovenName, pSuccess, pError);
    }
    #endregion


    public CovenMember GetMemberByName(string sMemberID)
    {
        foreach (CovenMember pMember in Data.members)
        {
            if (pMember.displayName == sMemberID)
                return pMember;
        }
        return null;
    }
    public event Action<string> OnCovenDataChanged;
    void DidChangeCovenData(string sReason)
    {
        if (OnCovenDataChanged != null)
            OnCovenDataChanged(sReason);
    }



    #region websockets

    public void OnReceiveCovenMemberAlly(WebSocketResponse pResp)
    {
        //"command":"coven_member_ally",
        //"member":"okthugo021",
        //"coven":"okt-19"
    }
    public void OnReceiveCovenMemberUnally(WebSocketResponse pResp)
    {
        //"command":"coven_member_unally",
        //"member":"okthugo021",
        //"coven":"okt-19"
    }
    public void OnReceiveCovenMemberKick(WebSocketResponse pResp)
    {
        //"command":"coven_member_kick",
        //"coven":"okt-19"
        bool bHasChanged = false;
        List<CovenMember> vMembers = new List<CovenMember>();
        foreach (CovenMember pMember in Data.members)
        {
            // 
            vMembers.Add(pMember);
        }
        if (bHasChanged)
        {
            Data.members = vMembers.ToArray();
            // does not need to notify
            //DidChangeCovenData(pResp.command);
        }
    }
    public void OnReceiveCovenMemberRequest(WebSocketResponse pResp)
    {

    }
    public void OnReceiveCovenMemberPromote(WebSocketResponse pResp)
    {
        // coven_member_promote, role: int
        CovenMember pMember = GetMemberByName(pResp.member);
        if (pMember != null)
        {
            pMember.role = pResp.role;
            // does not need to notify
            //DidChangeCovenData(pResp.command);
        }
        else
        {
            Debug.LogError("Did not find member with name: " + pResp.member);
        }
    }
    public void OnReceiveCovenMemberJoin(WebSocketResponse pResp)
    {
        //"command":"coven_member_join",
        //"member":"okthugo032",
        //"level":1,
        //"degree":0
        if (GetMemberByName(pResp.member) == null)
        {
            // it reloads the data to make sure we have the right data
            Action<CovenData> Success = (CovenData pCoven) =>
            {
                DidChangeCovenData(pResp.command);
            };
            RequestDisplayCoven(Success, null);
        }
        else
            Debug.LogError("already has this member in controller: " + pResp.member);
    }
    public void OnReceiveCovenMemberTitleChange(WebSocketResponse pResp)
    {
        //"command":"coven_title_change",
        //"coven":"covis1",
        //"title":"master"
        CovenMember pMember = GetMemberByName(PlayerName);
        if(pMember != null)
        {
            pMember.title = pResp.title;
        }
        // does not need to notify
        //DidChangeCovenData(pResp.command);
    }
    #endregion

}
