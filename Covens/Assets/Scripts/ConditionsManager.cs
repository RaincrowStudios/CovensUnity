using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ConditionsManager : MonoBehaviour
{
	public static ConditionsManager Instance{ get; set; }
	public Dictionary<string,ConditionButtonData> conditionsDict = new Dictionary<string, ConditionButtonData>();
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
	}

	public void Animate ()
	{
		if (!isClicked) {
			anim.SetBool ("animate", true);
			isClicked = true;
			SetupConditions ();
		} else {
			close ();
		}
	}

	void close ()
	{


		anim.SetBool ("animate", false);
		Invoke ("DisableClick", .4f);
		Invoke ("ClearItems", 1.5f);
	}

	void ClearItems()
	{
		foreach (Transform item in Container) {
			Destroy (item.gameObject);
		}
		conditionsDict.Clear ();
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
			SpawnCondition (item.Value);
		}
	}

	void SpawnCondition (Conditions item)
	{
		if (conditionsDict.ContainsKey (item.spellID)) {
			print ("Adding Condition");
			conditionsDict [item.spellID].conditions [item.instance] = item;
		}
		else {
			print ("Spawning Condition");
			var g = Utilities.InstantiateObject (ConditionPrefab, Container);
			var data = g.GetComponent<ConditionButtonData> ();
			data.conditions.Add (item.instance, item);
			data.Setup ();
			conditionsDict.Add (item.spellID, data);
		}
	}

	public void ConditionTrigger(Conditions condition)
	{
		if (isClicked) {
			if (conditionsDict.ContainsKey (condition.spellID)) {
				conditionsDict [condition.spellID].ConditionTrigger ();
			}
		} else {
			FXTrigger.SetActive (true);
		}
	}

	public void WSAddCondition(Conditions condition)
	{
		PlayerDataManager.playerData.conditionsDict.Add (condition.instance, condition); 
		SetupButton (true);
		if (isClicked) {
			if (conditionsDict.ContainsKey (condition.spellID)) {
				conditionsDict [condition.spellID].conditions.Add (condition.instance, condition);
				conditionsDict [condition.spellID].Setup (true);
			} else {
				SpawnCondition (condition);
			}
			conditionsDict [condition.spellID].ConditionChange ();
		} else {
			FX.SetActive (true);
		}
	}

	public void WSRemoveCondition(string conditionInstance)
	{
		Counter.text = PlayerDataManager.playerData.conditionsDict.Count.ToString ();
		if (!PlayerDataManager.playerData.conditionsDict.ContainsKey (conditionInstance)) {
			return;
		}
		if (isClicked) {
			var sID = PlayerDataManager.playerData.conditionsDict [conditionInstance].spellID;
			if (conditionsDict.ContainsKey (sID)) {
				conditionsDict [sID].conditions.Remove (conditionInstance);
				if (conditionsDict [sID].conditions.Count > 0) {
					conditionsDict [sID].Setup (true);
					conditionsDict [sID].ConditionChange ();
				}
				else {
					Destroy (conditionsDict [sID].gameObject);
					conditionsDict.Remove (sID);
				}
			}
		}
		print (PlayerDataManager.playerData.conditionsDict [conditionInstance].status + PlayerDataManager.playerData.conditionsDict [conditionInstance].spellID );
		print("Removing " + conditionInstance);
		PlayerDataManager.playerData.conditionsDict.Remove (conditionInstance); 

	

		if (conditionsDict.Count == 0) {
			counterObject.SetActive (false);
		} else {
			FX.SetActive (true);
		}
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

