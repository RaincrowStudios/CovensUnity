using UnityEngine;
using System.Collections;

public static class OnMapConditionRemove
{
    public static void HandleEvent(WSData data)
    {
        if (data.condition.bearer == PlayerDataManager.playerData.instance)
        {

            if (MapSelection.currentView == CurrentView.IsoView)
            {
                ConditionsManagerIso.Instance.WSRemoveCondition(data.condition.instance, true);
            }
            ConditionsManager.Instance.WSRemoveCondition(data.condition.instance);
            bool isSilenced = false;
            bool isBound = false;

            foreach (var item in PlayerDataManager.playerData.conditionsDict)
            {
                if (item.Value.status == "silenced")
                {
                    BanishManager.silenceTimeStamp = item.Value.expiresOn;
                    isSilenced = true;
                    break;
                }
                else
                    isSilenced = false;
            }

            foreach (var item in PlayerDataManager.playerData.conditionsDict)
            {
                if (item.Value.status == "bound")
                {
                    BanishManager.bindTimeStamp = item.Value.expiresOn;
                    isBound = true;
                    break;
                }
                else
                    isBound = false;
            }

            if (data.condition.status == "silenced")
            {
                if (!isSilenced)
                {
                    BanishManager.Instance.unSilenced();
                }
            }

            if (!isBound && data.condition.status == "bound")
            {
                BanishManager.Instance.Unbind();
                if (LocationUIManager.isLocation)
                {
                    LocationUIManager.Instance.Bind(false);
                }
            }

        }
        else if (data.condition.bearer == MarkerSpawner.instanceID)
        {
            //					print ("<color=red>" + data.json + "</color>");
            if (MapSelection.currentView == CurrentView.IsoView)
            {
                ConditionsManagerIso.Instance.WSRemoveCondition(data.condition.instance, false);
            }
            if (MarkerSpawner.SelectedMarker.conditionsDict.ContainsKey(data.condition.instance))
            {
                //					print ("Contains Condition");
                MarkerSpawner.SelectedMarker.conditionsDict.Remove(data.condition.instance);
                //					print ("Removed Condition");
            }
            else
            {
                //					print ("<b>Did not contain the condition! >>>>></b> ==" + data.condition.instance);
            }
        }
    }
}
