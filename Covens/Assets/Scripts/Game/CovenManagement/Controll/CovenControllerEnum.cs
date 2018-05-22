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
        All = Remove | Promote | ChangeTitle,
    }

    public enum CovenRole
    {
        None = 0,
        Member,
        Administrator,
        Moderator,
    }


    public enum CovenStatus
    {
        Online,
        Offline,
        InBattle,
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
