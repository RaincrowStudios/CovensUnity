using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


/// <summary>
/// Coven's logic goes here
/// </summary>
public class CovenController : MonoBehaviour
{


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
        All = Remove | Promote,
    }

    public enum CovenTitle
    {
        None,
        Owner,
        Elder,
        Reverant,
        Initiat,
        Novice,
    }

    #endregion


    #region static methods

    /// <summary>
    /// gets the possible actions by its each title
    /// </summary>
    /// <param name="eTitle"></param>
    /// <returns></returns>
    public static CovenPlayerActions GetActionsByTitle(CovenTitle eTitle)
    {
        switch (eTitle)
        {
            case CovenTitle.Owner:
            case CovenTitle.Reverant:
                return CovenPlayerActions.All;
            case CovenTitle.Elder:
                return CovenPlayerActions.Promote;
        }
        return CovenPlayerActions.None;
    }

    /// <summary>
    /// gets the allowed titles per player
    /// </summary>
    /// <param name="eCurrentTitle"></param>
    /// <returns></returns>
    public static List<CovenTitle> GetAllowedTitles(CovenTitle eCurrentTitle)
    {
        List<CovenTitle> vAllowedList = new List<CovenTitle>();
        var list = Enum.GetValues(typeof(CovenTitle));
        foreach (object ob in list)
        {
            if ((CovenTitle)ob == CovenTitle.None)
                continue;
            vAllowedList.Add((CovenTitle)ob);
        }
        return vAllowedList;
    }

    #endregion

}
