using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ConditionsManager : MonoBehaviour
{
	public static ConditionsManager Instance{ get; set; }

	public static Dictionary<string,Conditions> Conditions = new Dictionary<string, Conditions> ();

	[Header ("Main UI")]
	bool isClicked = false;
	public Animator anim;
	public Text Counter;
	public Text CounterBuff;
	public Text CounterDebuff;
	public GameObject counterObject;
	int buffCounter;
	int debuffCounter;

	public Transform ContainerBuffs;
	public Transform ContainerDebuffs;
	public GameObject ConditionPrefab;
	Dictionary<string, ConditionButtonData> holders = new Dictionary<string, ConditionButtonData> ();

	[Header ("Selected UI")]
	public Transform selectContainer;
	public Transform selectTargetContainer;
	Dictionary<string, ConditionButtonData> holdersSelected = new Dictionary<string, ConditionButtonData> ();
	Dictionary<string, ConditionButtonData> holdersTargetSelected = new Dictionary<string, ConditionButtonData> ();

	public static Dictionary<string,Conditions> ConditionsTarget = new Dictionary<string, Conditions> ();

	void Awake ()
	{
		Instance = this;
	}

	public void RemoveCondition (string instance, bool self = true)
	{
		print ("removing");
		if (self) {
			if (Conditions.ContainsKey (instance)) {
				Conditions.Remove (instance);
				print ("removedd");
			}
			Init ();
		} else {
			if (ConditionsTarget.ContainsKey (instance)) {
				Conditions.Remove (instance);
				print ("removed target");
			}
			Init (false);
		}
	}

	public void AddCondition (Conditions condition, bool self = true)
	{
		if (self) {
			Conditions.Add (condition.instance, condition);
			Init ();
		} else {
			ConditionsTarget.Add (condition.instance, condition);
			Init (false);
		}
	}

	public void Init (bool self = true)
	{
		if (self) {
			buffCounter = debuffCounter = 0;
			Counter.text = Conditions.Count.ToString ();
			if (Conditions.Count > 0) {
				counterObject.SetActive (true);
			} else {
				counterObject.SetActive (false);
			}
		}
		if (OnPlayerSelect.currentView == CurrentView.MapView) {
			if (isClicked && self) {
				holders.Clear ();
				foreach (Transform item in ContainerDebuffs) {
					Destroy (item.gameObject);
				}

				foreach (Transform item in ContainerBuffs) {
					Destroy (item.gameObject);
				}
			
				foreach (var item in Conditions) {
					if (item.Value.isBuff) {
						buffCounter++;
					} else {
						debuffCounter++;
					}
					if (!holders.ContainsKey (item.Value.displayName)) {
						GameObject g;
						if (item.Value.isBuff) {
							g = Utilities.InstantiateObject (ConditionPrefab, ContainerBuffs);
						} else {
							g = Utilities.InstantiateObject (ConditionPrefab, ContainerDebuffs);
						}
						var cbd = g.GetComponent<ConditionButtonData> ();
						cbd.Setup (SpellGlyphs.glyphs [item.Value.id], item.Value.Description);
						holders.Add (item.Value.displayName, cbd);
					} else {
						holders [item.Value.displayName].IncrementCounter ();
					}
				}
				CounterBuff.text = "buff(" + buffCounter.ToString () + ")";  
				CounterDebuff.text = "debuff(" + debuffCounter.ToString () + ")";  
			}
		} else {
			if (self) {
				holdersSelected.Clear ();
				foreach (Transform item in selectContainer) {
					Destroy (item.gameObject);
				}

				foreach (var item in Conditions) {
		
					if (!holdersSelected.ContainsKey (item.Value.displayName)) {
						GameObject g = Utilities.InstantiateObject (ConditionPrefab, ContainerBuffs);
						var cbd = g.GetComponent<ConditionButtonData> ();
						cbd.Setup (SpellGlyphs.glyphs [item.Value.id], item.Value.Description);
						holdersSelected.Add (item.Value.displayName, cbd);
					} else {
						holdersSelected [item.Value.displayName].IncrementCounter ();
					}
				}
			} else {
				holdersTargetSelected.Clear ();

				foreach (Transform item in selectTargetContainer) {
					Destroy (item.gameObject);
				}

				foreach (var item in ConditionsTarget) {
					if (!holdersSelected.ContainsKey (item.Value.displayName)) {
						GameObject g = Utilities.InstantiateObject (ConditionPrefab, ContainerBuffs);
						var cbd = g.GetComponent<ConditionButtonData> ();
						cbd.Setup (SpellGlyphs.glyphs [item.Value.id], item.Value.Description);
						holdersSelected.Add (item.Value.displayName, cbd);
					} else {
						holdersSelected [item.Value.displayName].IncrementCounter ();
					}
				}
			}
		}
		
	}

	public void Animate ()
	{
		if (!isClicked) {
			anim.SetBool ("animate", true);
			isClicked = true;
			Init ();
		} else {
			close ();
		}
	}

	void close ()
	{
		anim.SetBool ("animate", false);
		Invoke ("DisableClick", .4f);
	}

	void DisableClick ()
	{
		isClicked = false;
	}
}

