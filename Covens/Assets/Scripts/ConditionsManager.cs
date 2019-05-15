using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public static class ConditionsManager
{
    private static Dictionary<string, Conditions> m_ConditionsDictionary = new Dictionary<string, Conditions>();

    public static List<Conditions> conditions { get { return new List<Conditions>(m_ConditionsDictionary.Values); } }

    private static List<string> conditionTempfix = new List<string>();

    public static void SetupConditions()
    {
        foreach (var item in PlayerDataManager.playerData.conditions)
        {
            ManageCondition(item, false);
        }
    }

    private static void ManageCondition(Conditions item, bool isRemove)
    {
        if (isRemove)
        {
            m_ConditionsDictionary.Remove(item.instance);

            if (item.status == "silenced")
                BanishManager.Instance.unSilenced();

            if (item.status == "bound")
            {
                BanishManager.Instance.Unbind();
                // if (LocationUIManager.isLocation)
                //     LocationUIManager.Instance.Bind(false);
            }
        }
        else
        {
            if (m_ConditionsDictionary.ContainsKey(item.instance))
                m_ConditionsDictionary[item.instance] = item;
            else
                m_ConditionsDictionary[item.instance] = item;

            if (item.status == "silenced")
            {
                Debug.Log("SILENCED!!");

                BanishManager.silenceTimeStamp = item.expiresOn;
                BanishManager.isSilenced = true;
            }
            if (item.status == "bound")
            {
                Debug.Log("BOUND!!");

                BanishManager.isBind = true;
                BanishManager.bindTimeStamp = item.expiresOn;

                if (LocationUIManager.isLocation)
                    LocationUIManager.Instance.Bind(true);

                // BanishManager.Instance.Bind();
                PlayerManager.Instance.CancelFlight();
            }
        }
    }

    public static void ConditionTrigger(string instance)
    {
        //if (PlayerDataManager.playerData.conditionsDict.ContainsKey (instance)) {
        //	var conditionData =PlayerDataManager.playerData.conditionsDict[instance];
        //	if (isClicked) {
        //		if (conditionButtonDict.ContainsKey (instance)) { 
        //			conditionButtonDict [conditionData.id].ConditionTrigger ();
        //		}
        //	} else {
        //		FXTrigger.SetActive (true);
        //	}
        //}	
    }

    public static void WSAddCondition(Conditions condition)
    {
        if (conditionTempfix.Contains(condition.instance))
        {
            conditionTempfix.Remove(condition.instance);
            return;
        }

        ManageCondition(condition, false);
    }

    public static void WSRemoveCondition(string instance)
    {
        if (m_ConditionsDictionary.ContainsKey(instance))
        {
            Conditions condit = m_ConditionsDictionary[instance];
            //m_Conditions.Remove(condit);
            //m_ConditionsDictionary.Remove(instance);
            ManageCondition(condit, true);
        }
        else
        {
            conditionTempfix.Add(instance);
        }
    }

    public static bool IsConditionActive(string instance)
    {
        return m_ConditionsDictionary.ContainsKey(instance);
    }
}

