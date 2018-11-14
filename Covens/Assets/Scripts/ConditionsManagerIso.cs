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
		ClearItems ();
		foreach (var item in PlayerDataManager.playerData.conditionsDict) {
			ManageCondition (item.Value, false,true);
		}
		foreach (var item in MarkerSpawner.SelectedMarker.conditionsDict) {
			ManageCondition (item.Value, false,false);
		}
	}

	void ManageCondition (Conditions item, bool isRemove, bool isSelf )
	{
		if (isSelf) {
			if (!isRemove) {
				if (!conditionsDictSelf.ContainsKey (item.id)) {
					var g = Utilities.InstantiateObject (ConditionPrefabSelf, ContainerSelf);
					var data = g.GetComponent<ConditionButtonData> (); 
					data.Setup (item);  
					conditionsDictSelf.Add (item.id, data);
				} else {
					conditionsDictSelf [item.id].Add (item);
				}
			} else {
				if (conditionsDictSelf.ContainsKey (item.id)) {
					conditionsDictSelf [item.id].Remove (item);
				}
			}
		} else {
			if (!isRemove) {
				if (!conditionsDictTarget.ContainsKey (item.id)) {
					var g = Utilities.InstantiateObject (ConditionPrefabTarget, ContainerTarget);
					var data = g.GetComponent<ConditionButtonData> (); 
					data.Setup (item);  
					conditionsDictTarget.Add (item.id, data);
				} else {
					conditionsDictTarget [item.id].Add (item);
				}
			} else {
				if (conditionsDictTarget.ContainsKey (item.id)) {
					conditionsDictTarget [item.id].Remove (item);
				}
			}
		}
	}


	public void ConditionTrigger(string instance, bool isSelf)
	{
		if (isSelf) {
			if (PlayerDataManager.playerData.conditionsDict.ContainsKey (instance)) {
				var data = PlayerDataManager.playerData.conditionsDict [instance];
				if (conditionsDictSelf.ContainsKey (data.id)) {
					conditionsDictSelf [data.id].ConditionTrigger (); 
				}
			}
		} else {
			if (MarkerSpawner.SelectedMarker.conditionsDict.ContainsKey (instance)) {
				var data = MarkerSpawner.SelectedMarker.conditionsDict [instance]; 
				if (conditionsDictTarget.ContainsKey (data.id)) {
					conditionsDictTarget [data.id].ConditionTrigger (); 
				}
			}
		}
	}

	public void WSAddCondition(Conditions condition,bool isSelf)
	{
		ManageCondition (condition, false, isSelf);
	}

	public void WSRemoveCondition(string conditionInstance, bool isSelf)
	{
		Conditions removedCondition = new Conditions();
		if (isSelf) {
			var cData = PlayerDataManager.playerData.conditionsDict;
			if (cData.ContainsKey(conditionInstance)) {
				removedCondition.id = cData [conditionInstance].id;
				ManageCondition(removedCondition,true,true);
			}
		} else {
			var cData = MarkerSpawner.SelectedMarker.conditionsDict; 
			if (cData.ContainsKey(conditionInstance)) {
				removedCondition.id = cData [conditionInstance].id;
				ManageCondition(removedCondition,true,false);
			}
		}
	}
		
}

