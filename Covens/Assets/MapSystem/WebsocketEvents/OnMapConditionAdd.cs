using UnityEngine;
using System.Collections;

public static class OnMapConditionAdd
{
    public static event System.Action<Conditions> OnConditionAdded;

    public static void HandleEvent(WSData data)
    {
        if (data.condition.bearer == PlayerDataManager.playerData.instance)
        {
            ConditionsManager.Instance.WSAddCondition(data.condition);
            if (data.condition.status == "silenced")
            {
                Debug.Log("SILENCED!!");
                BanishManager.silenceTimeStamp = data.expiresOn;
                BanishManager.Instance.Silenced(data);
            }
            if (data.condition.status == "bound")
            {
                Debug.Log("BOUND!!");
                BanishManager.bindTimeStamp = data.expiresOn;
                BanishManager.Instance.Bind(data);
                if (LocationUIManager.isLocation)
                {
                    LocationUIManager.Instance.Bind(true);
                }
            }
            if (MapSelection.currentView == CurrentView.IsoView)
            {
                ConditionsManagerIso.Instance.WSAddCondition(data.condition, true);
            }
        }
        //else if (data.condition.bearer == MarkerSpawner.instanceID)
        //{
        //    MarkerSpawner.SelectedMarker.conditionsDict.Add(data.condition.instance, data.condition);
        //    if (MapSelection.currentView == CurrentView.IsoView)
        //    {
        //        ConditionsManagerIso.Instance.WSAddCondition(data.condition, false);
        //    }
        //}

        OnConditionAdded?.Invoke(data.condition);
    }
}
