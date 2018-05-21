using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


/// <summary>
/// Coven's logic goes here
/// </summary>
[System.Serializable]
public class CovenController //: Patterns.SingletonComponent<CovenController>
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



    #region enumerators

    [Flags]
    public enum CovenActions
    {
        None = 0,
        Ally = 1,
        Unally = 2,
        Accept = 4,
        Reject = 8,
        Invite = 16,
        See = 32,
    }

    [Flags]
    public enum CovenPlayerActions
    {
        None = 0,
        Remove = 1,
        Promote = 2,
        ChangeTitle = 4,
        All = Remove | Promote | ChangeTitle,
    }

    public enum CovenRole
    {
        None,
        Moderator,
        Administrator,
        Member,
    }


    public enum CovenStatus
    {
        Online,
        Offline,
        InBattle,
    }
    #endregion


    public CovenController( string sCovenName)
    {
        CovenName = sCovenName;
    }


    #region gets

    // all data are in offline wip mode for now
    public string sPlayerName
    {
        get { return PlayerDataManager.playerData.displayName; }
    }

    public bool IsInCoven
    {
        get;set;
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



    #region not a member requests



    private CovenInvite m_CovenInvite;
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

        CovenManagerAPI.RequestCovenInvites(sPlayerName, pSuccess, pError);
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
    public void CreateCoven(string sCovenName, Action<CovenData> pSuccess, Action<string> pError)
    {
        CovenManagerAPI.CreateCoven(sCovenName, pSuccess, pError);
    }
    /// <summary>
    /// displays a coven
    /// </summary>
    /// <param name="pSuccess"></param>
    /// <param name="pError"></param>
    public void RequestDisplayCoven(Action<CovenData> pSuccess, Action<string> pError)
    {
        CovenManagerAPI.RequestDisplayCoven(CovenName, pSuccess, pError);
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
            if (pSuccess != null)
                pSuccess();
            // update the player data
            PlayerDataManager.playerData.coven = sCovenName;
        };
        Action<string> Error = (string sError) =>
        {
            if (pError != null)
                pError(sError);
        };
        CovenManagerAPI.Join(sCovenName, Success, Error);
    }
    #endregion


    #region members

    public void RequestCovensData(Action<CovenData> pSuccess, Action<string> pError)
    {
        //if (!IsInCoven)
        //    return;
        CovenManagerAPI.RequestDisplayCoven(CovenName,
            (CovenData pData) => { m_LastCovenData = pData; if (pSuccess != null) pSuccess(pData); },
            (string sError) => { if (pError != null) pError(sError); }
            );
    }
    public void UpdateCovensTitles(CovenItem pItemToUpdate)
    {

    }

    #endregion


    #region Coven invite-alliance
    public void Ally(string sCovenName, Action<CovenData> pSuccess, Action<string> pError)
    {
        CovenManagerAPI.Ally(sCovenName, pSuccess, pError);
    }
    //covens/coven/unally --> req: {covenName: str} --> res: 200
    public void Unally(string sCovenName, Action<CovenData> pSuccess, Action<string> pError)
    {
        CovenManagerAPI.Unally(sCovenName, pSuccess, pError);
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


    public CovenActions GetCovenAvailableActions()
    {
        CovenActions eActions = CovenActions.None;
        if (Player.IsInCoven)
        {
            if (IsCovenAnAlly)
                eActions = CovenActions.Unally;
            else
                eActions = CovenActions.Ally;
        }
        else
        {
            eActions = CovenActions.Accept;// | CovenActions.Reject;
        }
        return eActions;
    }

    public CovenPlayerActions GetPossibleActions()
    {
        return GetActionsByTitle(CurrentRole);
    }


#region static methods


    public static CovenRole ParseRole(string sEnum)
    {
        try
        {
            CovenRole eRole = (CovenRole)Enum.Parse(typeof(CovenRole), sEnum, true);
            return eRole;
        }
        catch (System.Exception e) { }
        return CovenRole.None;
    }
    /// <summary>
    /// gets the possible actions by its each title
    /// </summary>
    /// <param name="eTitle"></param>
    /// <returns></returns>
    public static CovenPlayerActions GetActionsByTitle(CovenRole eTitle)
    {
        switch (eTitle)
        {
            case CovenRole.Administrator:
            case CovenRole.Moderator:
                return CovenPlayerActions.All;
        }
        return CovenPlayerActions.None;
    }


    /// <summary>
    /// gets the allowed titles per player
    /// </summary>
    /// <param name="eCurrentTitle"></param>
    /// <returns></returns>
    public static List<CovenRole> GetAllowedTitles(CovenRole eCurrentTitle)
    {
        List<CovenRole> vAllowedList = new List<CovenRole>();
        var list = Enum.GetValues(typeof(CovenRole));
        foreach (object ob in list)
        {
            if ((CovenRole)ob == CovenRole.None)
                continue;
            vAllowedList.Add((CovenRole)ob);
        }
        return vAllowedList;
    }
    public static List<CovenRole> GetTitleList()
    {
        List<CovenRole> vAllowedList = new List<CovenRole>();
        var list = Enum.GetValues(typeof(CovenRole));
        foreach (object ob in list)
        {
            vAllowedList.Add((CovenRole)ob);
        }
        return vAllowedList;
    }



#endregion




}
