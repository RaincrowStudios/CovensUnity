using UnityEngine;
using System.Collections;
using Raincrow.Maps;

public static class OnMapConditionAdd
{
    public static event System.Action<Condition> OnConditionAdded;
    public static event System.Action<Condition> OnPlayerConditionAdded;

    public static string debugstring = "";

    public static void HandleEvent(WSData data)
    {
        Debug.Log($"<color=green>{data.json}</color>");
        //if (Application.isEditor || Debug.isDebugBuild)
        //{
        //    string instance = data.condition.instance;
        //    string bearer = "";
        //    if (data.condition.bearer == PlayerDataManager.playerData.instance)
        //        bearer = PlayerDataManager.playerData.displayName;
        //    else
        //    {
        //        IMarker marker = MarkerSpawner.GetMarker(data.condition.bearer);
        //        if (marker != null)
        //            bearer = string.IsNullOrEmpty(MarkerSpawner.GetMarker(data.condition.bearer).token.displayName) ? MarkerSpawner.GetMarker(data.condition.bearer).token.spiritId : MarkerSpawner.GetMarker(data.condition.bearer).token.displayName;
        //        else
        //            bearer = "null";
        //    }
        //    bearer += " [" + data.condition.bearer + "] ";

        //    string spell = data.condition.baseSpell;
        //    string stacked = data.condition.stacked.ToString();
        //    debugstring += "[" + instance + "]" + Time.time +"\n";
        //    debugstring += "\tspell: " + data.condition.baseSpell + "\n";
        //    debugstring += "\tstatus: " + data.condition.status + "\n";
        //    debugstring += "\tbearer: " + bearer + "\n";
        //    debugstring += "\tstacks: " + stacked;
        //    debugstring += "\n\n";

        //    Debug.Log("MAP CONDITION ADD\n" + debugstring);
        //}

        if (data.condition.bearer == PlayerDataManager.playerData.instance)
        {
            ConditionsManager.WSAddCondition(data.condition);
            OnPlayerConditionAdded?.Invoke(data.condition);
        }
        else
        {
            OnConditionAdded?.Invoke(data.condition);
        }
    }
}
