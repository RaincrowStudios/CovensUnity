using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ConditionsManager : MonoBehaviour
{
	public static ConditionsManager Instance{ get; set; }
	public Dictionary<string,ConditionButtonData> conditionButtonDict = new Dictionary<string, ConditionButtonData>();
	[Header ("Main UI")]
	bool isClicked = false;
	public Animator anim;
	public Text Counter;
	public GameObject counterObject;
	public Transform Container;
	public GameObject ConditionPrefab;
	public GameObject FX;
	public GameObject FXTrigger;
	void Awake ()
	{
		Instance = this;
        Container.parent.parent.gameObject.SetActive(false);
    }

	public void Animate ()
	{
		if (!isClicked)
        {
            Container.parent.parent.gameObject.SetActive(true);
            anim.Play ("in");
			isClicked = true;
			SetupConditions ();
		} else {
			close ();
		}
	}

	void close ()
	{
		anim.Play ("out");
		Invoke ("DisableClick", .4f);
		Invoke ("ClearItems", 1.5f);
	}

	void ClearItems()
	{
		foreach (Transform item in Container) {
			Destroy (item.gameObject);
        }
        counterObject.SetActive(false);
    }

	void DisableClick ()
	{
		isClicked = false;

		if (PlayerDataManager.playerData.conditionsDict.Count == 0) 
			counterObject.SetActive (false);
	}

	public void SetupConditions()
	{
		foreach (var item in PlayerDataManager.playerData.conditionsDict) {
			ManageCondition (item.Value, false);  
		}
	}

	void ManageCondition (Conditions item, bool isRemove )
	{
		if (!isRemove) {
			if (!conditionButtonDict.ContainsKey (item.id)) {
				var g = Utilities.InstantiateObject (ConditionPrefab, Container);
				var data = g.GetComponent<ConditionButtonData> (); 
				data.Setup (item);  
				conditionButtonDict.Add (item.id, data);
			} else {
				conditionButtonDict [item.id].Add (item);
			}
		} else {
			if (conditionButtonDict.ContainsKey (item.id)) {
				conditionButtonDict [item.id].Remove (item);
			}
		}
	}

	public void ConditionTrigger(string instance)
	{
		if (PlayerDataManager.playerData.conditionsDict.ContainsKey (instance)) {
			var conditionData =PlayerDataManager.playerData.conditionsDict[instance];
			if (isClicked) {
				if (conditionButtonDict.ContainsKey (instance)) { 
					conditionButtonDict [conditionData.id].ConditionTrigger ();
				}
			} else {
				FXTrigger.SetActive (true);
			}
		}
	
	}

	public void WSAddCondition(Conditions condition)
	{
		PlayerDataManager.playerData.conditionsDict.Add (condition.instance, condition); 
		SetupButton (true);
		if (!isClicked) {
			FX.SetActive (true);
		} 
		ManageCondition (condition, false);
	}

	public void WSRemoveCondition(string instance)
	{
		var condDict = PlayerDataManager.playerData.conditionsDict;  
//		print ("Removing condition normal");

		Conditions removedCondition = new Conditions ();
		if (condDict.ContainsKey (instance)) { 
//			print ("Contains Condition");
			removedCondition.id = condDict [instance].id;
			condDict.Remove (instance); 
		}
		if (condDict.Count > 0) {
			Counter.text = condDict.Count.ToString ();
			FX.SetActive (true);
		} else {
			counterObject.SetActive (false);
		} 
		ManageCondition (removedCondition, true);
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

