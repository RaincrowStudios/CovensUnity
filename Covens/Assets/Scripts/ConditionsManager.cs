using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public static class ConditionsManager
{
    private static Dictionary<string, Conditions> m_ConditionsDictionary = new Dictionary<string, Conditions>();

    public static List<Conditions> conditions { get { return new List<Conditions>(m_ConditionsDictionary.Values); } }

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
        ManageCondition(condition, false);

        //force a removecondition event after timer ends
        if (condition.constant == false)
        {
            System.TimeSpan timespan = Utilities.TimespanFromJavaTime(condition.expiresOn);

            LeanTween.value(0, 0, (float)timespan.TotalSeconds) 
                .setOnComplete(() =>
                {
                    if (m_ConditionsDictionary.ContainsKey(condition.instance))
                    {
                        var data = new WSData
                        {
                            condition = condition
                        };

                        OnMapConditionRemove.HandleEvent(data);
                    }
                });
        }
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
    }

    public static bool IsConditionActive(string instance)
    {
        return m_ConditionsDictionary.ContainsKey(instance);
    }
}

