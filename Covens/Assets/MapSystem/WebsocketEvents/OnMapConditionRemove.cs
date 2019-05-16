using UnityEngine;
using System.Collections;
using Raincrow.Maps;

public static class OnMapConditionRemove
{
    public static event System.Action<Conditions> OnConditionRemoved;
    public static event System.Action<Conditions> OnPlayerConditionRemoved;

    public static string debugstring = "";

    public static void HandleEvent(WSData data)
    {
        Debug.Log("<color=red>" + data.json + "</color>");

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
        //    debugstring += "[" + instance + "]" + Time.time + "\n";
        //    debugstring += "\tstatus: " + data.condition.status + "\n";
        //    debugstring += "\tbearer: " + bearer + "\n";
        //    debugstring += "\n\n";

        //    Debug.Log("MAP CONDITION REMOVE\n" + debugstring);
        //}

        if (data.condition.bearer == PlayerDataManager.playerData.instance)
        {
            ConditionsManager.WSRemoveCondition(data.condition.baseSpell, data.condition.instance);
            OnPlayerConditionRemoved?.Invoke(data.condition);
        }
        else
        {
            OnConditionRemoved?.Invoke(data.condition);
        }
    }
}
