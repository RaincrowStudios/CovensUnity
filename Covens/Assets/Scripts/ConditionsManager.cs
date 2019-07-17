using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public static class ConditionsManager
{
    private static Dictionary<string, Condition> m_ConditionsDictionary = new Dictionary<string, Condition>();

    public static List<Condition> conditions { get { return new List<Condition>(m_ConditionsDictionary.Values); } }

    public static void SetupConditions()
    {
        foreach (var item in PlayerDataManager.playerData.conditions)
        {
            ManageCondition(item, false);
        }
    }

    private static void ManageCondition(Condition item, bool isRemove)
    {
        if (string.IsNullOrEmpty(item.baseSpell))
        {
            string debug = "CONDITION WITH EMPTY baseSpell. SKIPPING.";
            if (Application.isEditor || Debug.isDebugBuild)
                debug += "\n" + Newtonsoft.Json.JsonConvert.SerializeObject(item, Newtonsoft.Json.Formatting.Indented);
            Debug.LogError(debug);
            return;
        }

        if (isRemove)
        {
            foreach(Condition _condition in m_ConditionsDictionary.Values)
            {
                if (_condition.instance == item.instance || _condition.baseSpell == item.baseSpell)
                {
                    m_ConditionsDictionary.Remove(_condition.baseSpell);

                    if (item.status == "bound" || _condition.status == "bound")
                        BanishManager.Instance.Unbind();
                    else if (item.status == "silenced" || _condition.status == "silenced")
                        BanishManager.Instance.UnSilenced();

                    break;
                }
            }
        }
        else
        {
            m_ConditionsDictionary[item.baseSpell] = item;

            if (item.status == "silenced")
            {
            }
            if (item.status == "bound")
            {
                BanishManager.Instance.Bind(item);
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

    public static void WSAddCondition(Condition condition)
    {
        ManageCondition(condition, false);

        if (condition.baseSpell == "spell_invisibility")
            return;

        //force a removecondition event after timer ends localy
        if (condition.constant == false)
        {
            System.TimeSpan timespan = Utilities.TimespanFromJavaTime(condition.expiresOn);

            LeanTween.value(0, 0, (float)timespan.TotalSeconds) 
                .setOnComplete(() =>
                {
                    
                    //check if the conditions wasnt already removed
                    if (m_ConditionsDictionary.ContainsKey(condition.baseSpell) == false)
                        return;

                    //gets the condition again in case it was stacked in the meantime
                    Condition current = m_ConditionsDictionary[condition.baseSpell];
                    timespan = Utilities.TimespanFromJavaTime(current.expiresOn);
                    

                    if (timespan.TotalSeconds >= 0)
                        return;

                    var data = new WSData
                    {
                        condition = current
                    };
                    Debug.Log(Newtonsoft.Json.JsonConvert.SerializeObject(data));
                    OnMapConditionRemove.HandleEvent(data);
                });
        }
    }

    public static void WSRemoveCondition(string spell, string instance)
    {
        foreach(Condition _condition in m_ConditionsDictionary.Values)
        {
            if (_condition.baseSpell == spell || _condition.instance == instance)
            {
                ManageCondition(_condition, true);
                return;
            }
        }
    }

    public static bool IsConditionActive(string spell)
    {
        foreach (Condition _condition in m_ConditionsDictionary.Values)
        {
            if (_condition.baseSpell == spell)
                return true;
        }

        return false;
    }
}

