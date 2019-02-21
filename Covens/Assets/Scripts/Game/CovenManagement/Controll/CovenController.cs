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
            if (m_PlayerInstance == null && PlayerDataManager.playerData != null)
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
    private CovenOverview[] m_CovenInvite;

    private bool m_bIsCreatingCoven = false;
    private long Identifier;
    private static long IdentifierCounter = 0;
    public event Action<string> OnCovenDataChanged;

    public CovenController(string sCovenId)
    {
        CovenId = sCovenId;
        Identifier = ++IdentifierCounter;
    }


    #region Coven Player specific gets
    public string PlayerName
    {
        get { return PlayerDataManager.playerData.displayName; }
    }
    public bool IsInCoven
    {
        get { return !string.IsNullOrEmpty(CovenId); }
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
    public bool CanManageAlliance
    {
        get
        {
            return RoleCanManageAlliance(CurrentRole);
        }
    }
    public CovenOverview[] AlliedCovens
    {
        get { return Player.Data.alliedCovens; }
        set { Player.Data.alliedCovens = value; }
    }
    public CovenOverview[] Allies
    {
        get { return Player.Data.allies; }
        set { Player.Data.allies = value; }
    }
    #endregion


    #region Coven specific gets

    public CovenData Data
    {
        get { return m_LastCovenData; }
        private set { m_LastCovenData = value; }
    }
    public string CovenId
    {
        get; set;
    }
    public string CovenName
    {
        get;
        private set;
    }
    public string CovenDominion
    {
        get { return Data != null ? Data.dominion : ""; }
    }
    public string CovenOwner
    {
        get { return PlayerDataManager.playerData.ownerCoven; }
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
    public bool IsCovenAnAlly
    {
        get;set;
    }
    public int AlliancesRequest
    {
        get { return GetAllianceRequests(); }
    }

    #endregion



    #region initialization flow

    public static void Load()
    {
        if(Player.IsInCoven && !Player.IsDataLoaded)
        {
            Player.RequestDisplayCoven(null, null);
        }
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
            CovenOverview pCoven = GetCoven(Allies, AlliedCovens[i].coven);
            // not finding means we are allied
            if (pCoven == null)
                iCount++;
        }
        return iCount;
    }
    private CovenOverview GetCoven(CovenOverview[] vCovens, string sCoven)
    {
        for (int i = 0; i < vCovens.Length; i++)
        {
            if (vCovens[i] != null && vCovens[i].coven == sCoven)
                return vCovens[i];
        }
        return null;
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
                if (Allies[i].coven == pList[j].coven)
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

    public bool CheckIfCovenIsAnAlly(string sCovenName)
    {
        if (Player.Data == null)
            return false;

        for (int i = 0; i < Allies.Length; i++)
        {
            if (sCovenName == Allies[i].coven)
                return true;
        }

        return false;
    }

    public void Setup(CovenOverview pCovenOverview)
    {
        if (pCovenOverview != null)
        {
            IsCovenAnAlly = CheckIfCovenIsAnAlly(pCovenOverview.coven);// pCovenOverview.isAlly;
            Setup(pCovenOverview.covenName, pCovenOverview.coven);
        }
    }
    public void Setup(string sCovenName, string sCovenId = null)
    {
        CovenName = sCovenName;
        Debug.Log(">> Setting CovenName[" + CovenName + "]: " + Identifier);
        if (IsPlayerCoven)
        {
            PlayerDataManager.Instance.OnPlayerJoinCoven(sCovenId, sCovenName); 
            if(ChatConnectionManager.Instance != null)
                ChatConnectionManager.Instance.SendCovenChannelRequest();
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
				PlayerDataManager.Instance.OnPlayerJoinCoven(CovenId, CovenName);
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
        Debug.Log(">> Setting CovenName[null]: " + Identifier);
        CovenId = null;
        if (IsPlayerCoven)
        {
            PlayerDataManager.Instance.OnPlayerLeaveCoven();
        }
        if (pSuccess != null)
            pSuccess(null);
    }

    public CovenMember GetMemberByName(string sMemberID)
    {
        foreach (CovenMember pMember in Data.members)
        {
            if (pMember.displayName == sMemberID)
                return pMember;
        }
        return null;
    }
    
    void DidChangeCovenData(string sReason)
    {
        if (OnCovenDataChanged != null)
            OnCovenDataChanged(sReason);
    }

    private void OnPromoteUserRole(string sUserName, CovenController.CovenRole eToRole)
    {
        CovenMember pMember = GetMemberByName(sUserName);
        pMember.role = (int)eToRole;
    }

    private void OnUpdateUserTitle(string sUserName, string sTitle)
    {
        CovenMember pMember = GetMemberByName(sUserName);
        pMember.title = sTitle;
    }
    private void OnKickUser(string sUserName)
    {
        RemoveUserFromData(sUserName);
    }

    bool RemoveUserFromData(string sUserName)
    {
        List<CovenMember> vMembers = new List<CovenMember>(Data.members);
        foreach (CovenMember pMember in Data.members)
        {
            if (pMember.displayName == sUserName)
            {
                vMembers.Remove(pMember);
                Data.members = vMembers.ToArray();
                return true;
            }
        }
        return false;
    }
    void AddUserMember(string sName, int iLevel, string sDegree)
    {
        CovenMember pNewMember = new CovenMember();
        pNewMember.displayName = sName;
        pNewMember.level = iLevel;
        pNewMember.degree = sDegree;
        var members = new List<CovenMember>(Data.members);
        members.Add(pNewMember);
        Data.members = members.ToArray();
    }



    #region Request - not a member


    public CovenOverview[] GetCurrentInvites()
    {
        return m_CovenInvite;
    }

    /// <summary>
    /// requests the covens who wants to join you
    /// </summary>
    /// <param name="pOnComplete"></param>
    public void CharacterInvites(Action<CovenOverview[], string> pOnComplete)
    {
#if !UNITY_EDITOR
        if (!IsPlayerCoven)
            pOnComplete(null, "Not allowed Action");
#endif

        Action<CovenOverview[]> Success = (CovenOverview[] pInvites) => {
            m_CovenInvite = pInvites;
            if (pOnComplete != null) pOnComplete(pInvites, null);
        };
        Action<string> Error = (string sError) => {
            if (pOnComplete != null) pOnComplete(null, sError);
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
        Debug.Log(">> CreateCoven: " + this.Identifier);
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
        Debug.Log(">> RequestDisplayCoven: " + this.Identifier+ "   CovenName: " + CovenName);
        CovenManagerAPI.CovenDisplay(CovenId, CovenName, Success, pError);
    }



    /// <summary>
    /// 
    /// </summary>
    /// <param name="sCovenInviteToken"></param>
    /// <param name="pSuccess"></param>
    /// <param name="pError"></param>
    public void JoinCoven(string sCovenInviteToken, Action pSuccess, Action<string> pError)
    {
        Action<string> Success = (string sOk) =>
        {
            // update the player data
            Setup(null);
            if (pSuccess != null)
                pSuccess();
        };
        CovenManagerAPI.CovenJoin(sCovenInviteToken, Success, pError);
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
        Action<string> Success = (string sOk) => {
            OnKickUser(sUserName);
            if (pSuccess != null)
                pSuccess(sOk);
        };
        //Action<CovenData> Success = (CovenData pData) => { UpdateCovenDataResponse(pData, pSuccess); };
        CovenManagerAPI.CovenKick(CovenName, sUserName, Success, pError);
    }
    public void PromoteMember(string sUserName, CovenController.CovenRole eToRole, Action<string> pSuccess, Action<string> pError)
    {
        Action<string> Success = (string sOk) => {
            OnPromoteUserRole(sUserName, eToRole);
            if (pSuccess != null)
                pSuccess(sOk);
        };
        CovenManagerAPI.CovenPromote(CovenName, sUserName, eToRole, Success, pError);
    }
    public void UpdateCovensTitles(string sUserName, string sTitle, Action<string> pSuccess, Action<string> pError)
    {
        Action<string> Success = (string sOk) => {
            OnUpdateUserTitle(sUserName, sTitle);
            if (pSuccess != null)
                pSuccess(sOk);
        };
        CovenManagerAPI.CovenTitle(CovenName, sUserName, sTitle, Success, pError);
    }
    public void AcceptMember(string sUserName, Action<string> pSuccess, Action<string> pError)
    {
        // it does not accept a member. it invites again, so he joint into the coven
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






#region websockets

    public void OnReceiveCovenAlly(WSData pResp)
    {
        // covenName: str
        // "command":"coven_was_allied",
        Action<CovenData> Success = (CovenData pCoven) =>
        {
            DidChangeCovenData(pResp.command);
            if (!CheckIfCovenIsAnAlly(pResp.covenName))
            {
                // ally to a coven
                Action AllyCallback = () =>
                {
                    Ally(pResp.covenName, (string s)=> { DidChangeCovenData(CovenConstants.Commands.coven_was_allied); }, null);
                };

                // Coven Has Allied
                // The coven <name> has allied to your coven. Do you wish to ally with <name>
                UIGenericPopup.ShowYesNoPopup(
                    Oktagon.Localization.Lokaki.GetText("Coven_AllyTitle").Replace("<name>", pResp.covenName),
                    Oktagon.Localization.Lokaki.GetText("Coven_AllyDesc").Replace("<name>", pResp.covenName),
                    AllyCallback, null
                    );
            }
        };
        RequestDisplayCoven(Success, null);
    }
    public void OnReceiveCovenUnally(WSData pResp)
    {
        // "command":"coven_was_unallied",
        // "covenName":"okt-19"
        Action<CovenData> Success = (CovenData pCoven) =>
        {
            DidChangeCovenData(pResp.command);
        };
        RequestDisplayCoven(Success, null);
    }
    public void OnReceiveRequestInvite(WSData pResp)
    {
        // "command":"coven_request_invite",
        // "character":"c79b0fb6-672d-44a4-9ac8-cc6e50436aee",
        // "displayName":"okthugo012"
        Action<CovenData> Success = (CovenData pCoven) =>
        {
            DidChangeCovenData(pResp.command);
        };
        RequestDisplayCoven(Success, null);
    }
    public void OnReceiveCovenMemberAlly(WSData pResp)
    {
        // displayName: str,
        // coven: str,
        // covenName: str"
        // "command":"coven_member_ally",
        // it reloads the data to make sure we have the right data
        //Action<CovenData> Success = (CovenData pCoven) =>
        //{
        //    DidChangeCovenData(pResp.command);
        //};
        //RequestDisplayCoven(Success, null);

        List<CovenOverview> vNewAllies = new List<CovenOverview>(Allies);
        CovenOverview pCoven = new CovenOverview();
        pCoven.coven = pResp.coven;
        pCoven.covenName = pResp.covenName;
        vNewAllies.Add(pCoven);
        Allies = vNewAllies.ToArray();
        DidChangeCovenData(pResp.command);
    }
    public void OnReceiveCovenMemberUnally(WSData pResp)
    {
        // displayName: str,
        // covenName: str"
        //"command":"coven_member_unally",
        // it reloads the data to make sure we have the right data
        List<CovenOverview> vNewAllied = new List<CovenOverview>(Allies);
        foreach (var coven in vNewAllied)
        {
            if (coven.covenName == pResp.covenName)
            {
                vNewAllied.Remove(coven);
                break;
            }
        }

        // update data
        Allies = vNewAllied.ToArray();
        DidChangeCovenData(pResp.command);
    }
    public void OnReceiveCovenMemberKick(WSData pResp)
    {
        //"command":"character_coven_kick",
        //"covenName":"h2"

        //"command":"coven_member_kick",
        //"coven":"okt-19"
        UIGenericPopup.ShowConfirmPopupLocalized("General_Info", "Coven_KickNotification", "General_Ok", null);
        //pResp.covenName
        OnLeaveCoven(null);
        CovenView.Instance.ShowMain();
    }
    public void OnReceiveCovenMemberRequest(WSData pResp)
    {
        // displayName: str,
        // level: int,
        // degree: int
        // command: coven_member_request
        
    }
    public void OnReceiveCovenMemberPromote(WSData pResp)
    {
        // member: str,
        // displayName: str,
        // newRole: int"
        CovenMember pMember = GetMemberByName(pResp.displayName);
        if (pMember != null)
        {
            pMember.role = pResp.newRole;
            DidChangeCovenData(pResp.command);
        }
        else
        {
            Debug.LogError("Did not find member with name: " + pResp.member);
        }
    }
    public void OnReceiveCovenMemberJoin(WSData pResp)
    {
        //displayName: str,
        //level: int,
        //degree: int"
        if (GetMemberByName(pResp.displayName) == null)
        {
            AddUserMember(pResp.displayName, pResp.level, pResp.degree.ToString());
            DidChangeCovenData(pResp.command);
            // it reloads the data to make sure we have the right data
            //Action<CovenData> Success = (CovenData pCoven) =>
            //{
            //    DidChangeCovenData(pResp.command);
            //};
            //RequestDisplayCoven(Success, null);
        }
        else
            Debug.LogError("already has this member in controller: " + pResp.member);
    }
    public void OnReceiveCovenMemberLeave(WSData pResp)
    {
        // am I still in a coven?
        if (IsInCoven)
        {
            // displayName: str
            RemoveUserFromData(pResp.displayName);
            DidChangeCovenData(pResp.command);
        }
        //Action<CovenData> Success = (CovenData pCoven) =>
        //{
        //    DidChangeCovenData(pResp.command);
        //};
        //RequestDisplayCoven(Success, null);
    }
    public void OnReceiveCovenMemberTitleChange(WSData pResp)
    {
        // member: str,
        // displayName: str,
        // newTitle: str"
        CovenMember pMember = GetMemberByName(pResp.displayName);
        if(pMember != null)
        {
            pMember.title = pResp.newTitle;
        }
        // does not need to notify
        DidChangeCovenData(pResp.command);
    }
    public void OnReceivedCovenInvite(WSData pResp)
    {
        // "command":"character_coven_invite",
        // "displayName":"auto-104",
        // "covenName":"h2",
        // "inviteToken":"us-east1:c9d42be0-ad59-11e8-a2f9-41d028c9a1d2"
        // threr is no id
        Action Confirm = () =>
        {
            UIGenericLoadingPopup.ShowLoading();
            Action Success = () =>
            {
                UIGenericLoadingPopup.CloseLoading();
                CovenView.Instance.ShowTabMembers(CovenController.Player);
            };
            Action<string> Error = (string sError) =>
            {
                UIGenericLoadingPopup.CloseLoading();
                UIGenericPopup.ShowErrorPopupLocalized(sError, null);
            };

            JoinCoven(pResp.inviteToken, Success, Error );
        };
        UIGenericPopup.ShowYesNoPopup(
            Oktagon.Localization.Lokaki.GetText("Coven_ReceiveInvite").Replace("<name>", pResp.covenName),
            Oktagon.Localization.Lokaki.GetText("Coven_ReceiveInviteDesc").Replace("<name>", pResp.covenName),
            Confirm, null
            );
    }
    #endregion

}
