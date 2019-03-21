using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public static class ConditionsManager
{
    private static List<Conditions> m_Conditions = new List<Conditions>();
    private static Dictionary<string, Conditions> m_ConditionsDictionary = new Dictionary<string, Conditions>();

    public static Conditions[] conditions { get { return m_Conditions.ToArray(); } }

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
            m_Conditions.Remove(item);
            m_ConditionsDictionary.Remove(item.instance);

            if (item.status == "silenced")
                BanishManager.Instance.unSilenced();

            if (item.status == "bound")
            {
                BanishManager.Instance.Unbind();
                if (LocationUIManager.isLocation)
                    LocationUIManager.Instance.Bind(false);
            }
        }
        else
        {
            if (m_ConditionsDictionary.ContainsKey(item.instance))
            {
                m_ConditionsDictionary[item.instance] = item;
                for(int i = 0; i < m_Conditions.Count; i++)
                {
                    if (m_Conditions[i].instance == item.instance)
                        m_Conditions[i] = item;
                }
            }
            else
            {
                m_Conditions.Add(item);
                m_ConditionsDictionary[item.instance] = item;
            }

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

                BanishManager.Instance.Bind();
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
		ManageCondition (condition, false);
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

	//public void SetupButton(bool state)
	//{
	//	try{
	//	Counter.text = PlayerDataManager.playerData.conditionsDict.Count.ToString ();
	//	}catch{
	//		// conditionsNUll;
	//	}
	//	counterObject.SetActive (state);
	//}
}

