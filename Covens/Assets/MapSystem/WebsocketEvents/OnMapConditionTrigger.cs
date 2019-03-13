using UnityEngine;
using System.Collections;

public static class OnMapConditionTrigger
{
    public static event System.Action<Conditions> OnConditionTriggered;

    public static void HandleEvent(WSData data)
    {
        MarkerDataDetail player = PlayerDataManager.playerData;

        if (data.condition.bearer == player.instance && player.conditionsDict.ContainsKey(data.condition.instance))
        {
            ConditionsManager.Instance.ConditionTrigger(data.condition.instance);
            if (MapSelection.currentView == CurrentView.IsoView)
            {
                ConditionsManagerIso.Instance.ConditionTrigger(data.condition.instance, true);
            }
        }

        //if (data.condition.bearer == MarkerSpawner.instanceID)
        //{
        //    if (MapSelection.currentView == CurrentView.IsoView)
        //    {
        //        ConditionsManagerIso.Instance.WSRemoveCondition(data.condition.instance, false);
        //    }
        //    else
        //    {
        //        if (MarkerSpawner.SelectedMarker.conditionsDict.ContainsKey(data.condition.instance))
        //        {
        //            MarkerSpawner.SelectedMarker.conditionsDict.Remove(data.condition.instance);
        //        }
        //    }
        //}

        OnConditionTriggered?.Invoke(data.condition);
    }
}
