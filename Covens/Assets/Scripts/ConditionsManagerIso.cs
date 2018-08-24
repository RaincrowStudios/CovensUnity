using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ConditionsManagerIso : MonoBehaviour
{
	public static ConditionsManagerIso Instance{ get; set; }
	public Dictionary<string,ConditionButtonData> conditionsDictSelf = new Dictionary<string, ConditionButtonData>();
	public Dictionary<string,ConditionButtonData> conditionsDictTarget = new Dictionary<string, ConditionButtonData>();
	public Transform ContainerSelf;
	public Transform ContainerTarget;
	public GameObject ConditionPrefabTarget;
	public GameObject ConditionPrefabSelf;
	void Awake ()
	{
		Instance = this;
	}

	void close ()
	{
		Invoke ("ClearItems", .5f);
	}

	void ClearItems()
	{
		foreach (Transform item in ContainerSelf) {
			Destroy (item.gameObject);
		}
		foreach (Transform item in ContainerTarget) {
			Destroy (item.gameObject);
		}
		conditionsDictSelf.Clear ();
		conditionsDictTarget.Clear ();

	}
		
	public void SetupConditions()
	{
		foreach (var item in PlayerDataManager.playerData.conditionsDict) {
			SpawnCondition (item.Value, true);
		}
		if (!MapSelection.IsSelf) {
			foreach (var item in MarkerSpawner.SelectedMarker.conditionsDict) {
				SpawnCondition (item.Value, false);
			}
		}
	}

	void SpawnCondition (Conditions item, bool isSelf)
	{
		if(isSelf){
			if (conditionsDictSelf.ContainsKey (item.spellID)) {
				conditionsDictSelf [item.spellID].conditions [item.conditionInstance] = item;
			} 
		else {
			var g = Utilities.InstantiateObject (ConditionPrefabSelf, ContainerSelf);
			var data = g.GetComponent<ConditionButtonData> ();
			data.conditions.Add (item.conditionInstance, item);
			data.Setup ();
			conditionsDictSelf.Add (item.spellID, data);
			}
		}else {
			if (conditionsDictTarget.ContainsKey (item.spellID)) {
				conditionsDictTarget [item.spellID].conditions [item.conditionInstance] = item;
			} 
			else {
				var g = Utilities.InstantiateObject (ConditionPrefabTarget, ContainerTarget);
				var data = g.GetComponent<ConditionButtonData> ();
				data.conditions.Add (item.conditionInstance, item);
				data.Setup ();
				conditionsDictTarget.Add (item.spellID, data);
			}
		}
	}

	public void ConditionTrigger(Conditions condition, bool isSelf)
	{
		if (isSelf) {
			if (conditionsDictSelf.ContainsKey (condition.spellID)) {
				conditionsDictSelf [condition.spellID].ConditionTrigger ();
			}
		} else {
			if (conditionsDictTarget.ContainsKey (condition.spellID)) {
				conditionsDictTarget [condition.spellID].ConditionTrigger ();
			}
		}
	}

	public void WSAddCondition(Conditions condition,bool isSelf)
	{
		if (isSelf) {
			if (conditionsDictSelf.ContainsKey (condition.spellID)) {
				conditionsDictSelf [condition.spellID].conditions.Add (condition.conditionInstance, condition);
				conditionsDictSelf [condition.spellID].Setup (true);
			} else {
				SpawnCondition (condition,true);
			}
			conditionsDictSelf [condition.spellID].ConditionChange ();
		} else {
			MarkerSpawner.SelectedMarker.conditionsDict.Add (condition.conditionInstance, condition); 
			if (conditionsDictTarget.ContainsKey (condition.spellID)) {
				conditionsDictTarget [condition.spellID].conditions.Add (condition.conditionInstance, condition);
				conditionsDictTarget [condition.spellID].Setup (true);
			} else {
				SpawnCondition (condition,false);
			}
			conditionsDictTarget [condition.spellID].ConditionChange ();
		}
	}

	public void WSRemoveCondition(string conditionInstance, bool isSelf)
	{
		if (isSelf) {
			if (!PlayerDataManager.playerData.conditionsDict.ContainsKey (conditionInstance)) {
				return;
			}
			var sID = PlayerDataManager.playerData.conditionsDict [conditionInstance].spellID;
			if (conditionsDictSelf.ContainsKey (sID)) {
				conditionsDictSelf [sID].conditions.Remove (conditionInstance);
				if (conditionsDictSelf [sID].conditions.Count > 0) {
					conditionsDictSelf [sID].Setup (true);
					conditionsDictSelf [sID].ConditionChange ();
				} else {
					Destroy (conditionsDictSelf [sID].gameObject);
					conditionsDictSelf.Remove (sID);
				}
			}
		} else {
			if (!MarkerSpawner.SelectedMarker.conditionsDict.ContainsKey (conditionInstance)) {
				return;
			}
			var sID = MarkerSpawner.SelectedMarker.conditionsDict [conditionInstance].spellID;
			if (conditionsDictTarget.ContainsKey (sID)) {
				conditionsDictTarget [sID].conditions.Remove (conditionInstance);
				if (conditionsDictTarget [sID].conditions.Count > 0) {
					conditionsDictTarget [sID].Setup (true);
					conditionsDictTarget [sID].ConditionChange ();
				} else {
					Destroy (conditionsDictTarget [sID].gameObject);
					conditionsDictTarget.Remove (sID);
				}
			}
			MarkerSpawner.SelectedMarker.conditionsDict.Remove (conditionInstance);
		}
	}
		
}

