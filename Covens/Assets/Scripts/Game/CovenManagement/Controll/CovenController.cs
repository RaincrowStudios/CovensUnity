using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


/// <summary>
/// Coven's logic goes here
/// </summary>
public class CovenController : Patterns.SingletonComponent<CovenController>
{


    private CovenData m_LastCovenData;


    #region enumerators

    [Flags]
    public enum CovenActions
    {
        Chat,
        Edit,
        Invite,
        Leave
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

    #endregion


    #region gets

    // all data are in offline wip mode for now

    public bool IsInCoven
    {
        //get { return !string.IsNullOrEmpty(PlayerDataManager.playerData.coven); }
        get { return true; }
    }
    public string CovenName
    {
        //get { return PlayerDataManager.playerData.coven; }
        get { return "CovenName"; }
    }
    public string CovenStatus
    {
        //get { return PlayerDataManager.playerData.covenStatus; }
        get { return "CovenStatus"; }
    }
    public string CovenOwner
    {
        //get { return PlayerDataManager.playerData.ownerCoven; }
        get { return "myself"; }
    }
    public bool CanJoinCoven
    {
        //get{ return !IsInCoven && true; }
        get { return true; }
    }
    public CovenRole CurrentRole
    {
        //get{ return !IsInCoven && true; }
        get { return CovenRole.Administrator; }
    }

    public bool NeedsReload
    {
        get { return !IsDataLoaded; }
    }

    public bool IsDataLoaded
    {
        get { return m_LastCovenData != null; }
    }

    #endregion

    public void RequestCovensData(Action<CovenData> pSuccess, Action<string> pError)
    {
        if (!IsInCoven)
            return;
        CovenManagerAPI.GetCovenData(CovenName,
            (CovenData pData) => { m_LastCovenData = pData; if (pSuccess != null) pSuccess(pData); },
            (string sError) => { if (pError != null) pError(sError); }
            );
    }
    public void UpdateCovensTitles(CovenItem pItemToUpdate)
    {

    }


    private List<CovenItem> m_JoinRequestList = new List<CovenItem>();


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
        
    #endregion



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
