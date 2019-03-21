using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public static class ConditionsManager
{
    private static List<Conditions> m_Conditions = new List<Conditions>();
    private static Dictionary<string, Conditions> m_ConditionsDictionary = new Dictionary<string, Conditions>();

	public static void SetupConditions()
	{
		foreach (var item in PlayerDataManager.playerData.conditionsDict)
        {
			ManageCondition (item.Value, false);  
		}
	}

    private static void ManageCondition(Conditions item, bool isRemove)
    {
        if (isRemove)
        {
            m_Conditions.Remove(item);
            m_ConditionsDictionary.Remove(item.instance);
        }
        else
        {
            m_Conditions.Add(item);
            m_ConditionsDictionary[item.instance] = item;
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
		ManageCondition (condition, false);
	}

    public static void WSRemoveCondition(string instance)
    {
        if (m_ConditionsDictionary.ContainsKey(instance))
        {
            removedCondition.id = condDict[instance].id;
            condDict.Remove(instance);
        }

        if (m_Conditions.Count > 0)
        {
            Counter.text = condDict.Count.ToString();
        }
        else
        {
            counterObject.SetActive(false);
        }
        ManageCondition(removedCondition, true);
    }

	public void SetupButton(bool state)
	{
		try{
		Counter.text = PlayerDataManager.playerData.conditionsDict.Count.ToString ();
		}catch{
			// conditionsNUll;
		}
		counterObject.SetActive (state);
	}
}

