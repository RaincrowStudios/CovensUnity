using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// CovenController enumerator operations goes here
/// </summary>
public partial class CovenController
{

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
        ManageAlliance = 8,
        ManageInvite = 16,
        All = Remove | Promote | ChangeTitle,
    }
    
    public enum CovenRole
    {
        [Oktagon.Localization.LokakiID("Enum_Member")]
        Member = 0,
        [Oktagon.Localization.LokakiID("Enum_Moderator")]
        Moderator,
        [Oktagon.Localization.LokakiID("Enum_Administrator")]
        Administrator,
        [Oktagon.Localization.LokakiID("Enum_Member")]
        None =100
    }


    public enum CovenStatus
    {
        Online,
        Offline,
        InBattle,
    }
    #endregion


    #region role actions matrix

    /// <summary>
    /// overrall role action matrix
    /// </summary>
    public static Dictionary<CovenRole, CovenPlayerActions> RoleActionMatrix = new Dictionary<CovenRole, CovenPlayerActions>()
    {
        { CovenRole.Administrator, CovenPlayerActions.ChangeTitle |  CovenPlayerActions.Promote | CovenPlayerActions.Remove | CovenPlayerActions.ManageAlliance | CovenPlayerActions.ManageInvite},
        { CovenRole.Moderator, CovenPlayerActions.ChangeTitle |  CovenPlayerActions.Promote | CovenPlayerActions.Remove | CovenPlayerActions.ManageAlliance | CovenPlayerActions.ManageInvite },
        { CovenRole.Member, CovenPlayerActions.None }
    };

    /// <summary>
    /// whitch role can KICK the role?
    /// </summary>
    public static Dictionary<CovenRole, CovenRole[]> RoleKickMatrix = new Dictionary<CovenRole, CovenRole[]>()
    {
        { CovenRole.Administrator, new CovenRole[]{ CovenRole.Administrator, CovenRole.Moderator, CovenRole.Member } },
        { CovenRole.Moderator, new CovenRole[]{ /*CovenRole.Moderator,*/ CovenRole.Member } },
        { CovenRole.Member, new CovenRole[]{} },
    };
    /// <summary>
    /// whitch role can PROMOTE the role?
    /// </summary>
    public static Dictionary<CovenRole, CovenRole[]> RolePromoteMatrix = new Dictionary<CovenRole, CovenRole[]>()
    {
        { CovenRole.Administrator, new CovenRole[]{ CovenRole.Moderator, CovenRole.Member } },
        { CovenRole.Moderator, new CovenRole[]{ CovenRole.Member } },
        { CovenRole.Member, new CovenRole[]{} },
    };
    /// <summary>
    /// whitch role can CHANGE TITLE the role?
    /// </summary>
    public static Dictionary<CovenRole, CovenRole[]> RoleTitleMatrix = new Dictionary<CovenRole, CovenRole[]>()
    {
        { CovenRole.Administrator, new CovenRole[]{ CovenRole.Administrator, CovenRole.Moderator, CovenRole.Member } },
        { CovenRole.Moderator, new CovenRole[]{ CovenRole.Moderator, CovenRole.Member } },
        { CovenRole.Member, new CovenRole[]{} },
    };


    public static bool RoleCanPromote(CovenRole eRoleOwner, CovenRole eRoleOther)
    {
        if (RolePromoteMatrix.ContainsKey(eRoleOwner))
            return RolePromoteMatrix[eRoleOwner].Contains<CovenRole>(eRoleOther);
        return false;
    }
    public static bool RoleCanKick(CovenRole eRoleOwner, CovenRole eRoleOther)
    {
        if (RoleKickMatrix.ContainsKey(eRoleOwner))
            return RoleKickMatrix[eRoleOwner].Contains<CovenRole>(eRoleOther);
        return false;
    }
    public static bool RoleCanChangeTitle(CovenRole eRoleOwner, CovenRole eRoleOther)
    {
        if (RoleTitleMatrix.ContainsKey(eRoleOwner))
            return RoleTitleMatrix[eRoleOwner].Contains<CovenRole>(eRoleOther);
        return false;
    }
    public static bool RoleCanManageAlliance(CovenRole eRoleOwner)
    {
        if (RoleActionMatrix.ContainsKey(eRoleOwner))
        {
            CovenPlayerActions eActions = RoleActionMatrix[eRoleOwner];
            return (eActions & CovenController.CovenPlayerActions.ManageAlliance) != 0;
        }
        return false;
    }
    #endregion

    public CovenActions GetCovenAvailableActions()
    {
        CovenActions eActions = CovenActions.None;
        if (Player.IsInCoven)
        {
            if (RoleCanManageAlliance(Player.CurrentRole))
            {
                if (IsCovenAnAlly)
                    eActions = CovenActions.Unally;
                else
                    eActions = CovenActions.Ally;
            }
        }
        else
        {
            eActions = CovenActions.Accept;
        }
        return eActions;
    }

    public CovenPlayerActions GetPossibleActions(CovenMember pUser, bool bIsPlayer)
    {
        return GetActionsByTitle(CurrentRole, pUser, bIsPlayer);
    }


    /// <summary>
    /// gets the possible actions by its each title
    /// </summary>
    /// <param name="ePlayerRole"></param>
    /// <returns></returns>

    public static CovenPlayerActions GetActionsByTitle(CovenRole ePlayerRole, CovenMember pUser, bool bIsPlayer = false)
    {
        CovenRole eUserRole = ParseRole(pUser.role);
        CovenPlayerActions eActions = 0;

        // can we promote
        if (RoleCanPromote(ePlayerRole, eUserRole) && CanBePromoted(eUserRole))
        {
            eActions |= CovenPlayerActions.Promote;
        }

        // can we change title
        if(RoleCanChangeTitle(ePlayerRole, eUserRole))
        {
            eActions |= CovenPlayerActions.ChangeTitle;
        }

        // can we kick user
        if (!bIsPlayer && RoleCanKick(ePlayerRole, eUserRole))
        {
            eActions |= CovenPlayerActions.Remove;
        }

        return eActions;
    }



    #region static role methods


    public static CovenRole ParseRole(string sEnum)
    {
        try
        {
            CovenRole eRole = (CovenRole)Enum.Parse(typeof(CovenRole), sEnum, true);
            return eRole;
        }
        catch (System.Exception e) { }
        return CovenRole.Member;
    }
    public static CovenRole ParseRole(int iValue)
    {
        try
        {
            return (CovenRole)iValue;
        }
        catch (System.Exception e) { }
        return CovenRole.Member;
    }

    //public static CovenPlayerActions GetActionsByTitle(CovenRole ePlayerRole, CovenRole eMemberRole, bool bIsPlayer = false)
    //{
    //    switch (ePlayerRole)
    //    {
    //        case CovenRole.Administrator:
    //            if (bIsPlayer)
    //                return CovenPlayerActions.ChangeTitle;
    //            return CovenPlayerActions.All;
    //        case CovenRole.Moderator:
    //            // moderators can only edit actions on members
    //            if (eMemberRole == CovenRole.Administrator)
    //                return CovenPlayerActions.None;
    //            if (bIsPlayer)
    //                return CovenPlayerActions.ChangeTitle;
    //            return CovenPlayerActions.All;
    //    }
    //    return CovenPlayerActions.None;
    //}

    public static CovenRole GetNextRole(CovenRole eCurrent)
    {
        switch (eCurrent)
        {
            case CovenRole.Moderator:
                return CovenRole.Administrator;
            case CovenRole.Member:
                return CovenRole.Moderator;
        }
        return CovenRole.None;
    }
    public static bool CanBePromoted(CovenRole eCurrent)
    {
        return GetNextRole(eCurrent) != CovenRole.None;
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
            //if ((CovenRole)ob == CovenRole.Memberno)
            //    continue;
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
