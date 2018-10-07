using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UI;

public class SpellCarouselManager : MonoBehaviour
{
	public static SpellCarouselManager Instance{ get; set;}
	public static SpellData currentSpellData; 
	public static string targetType = "none";
	public Text warningText;
	public GameObject spellCarousel;
	public GameObject[] glow;
	public Text spellName;
	public Text spellCost;
	public Text spellDesc;
	public RectTransform container;
	public GameObject spellButton;
	Dictionary<string,SpellButtonData> spellButtonDataDict = new Dictionary<string, SpellButtonData> ();
	public float fadeSpeed = 1;
	public int bttnOffset = 215;

	public ScrollSnapController SSC;
	bool hasCreated = false;
	bool hasCreatedSelf = false;
	string previousSpell = "";
	void Awake()
	{
		Instance = this;
	}

	public void WSStateChange( )
	{
		if (MapSelection.currentView == CurrentView.IsoView) {
			var l = spellButtonDataDict.Values.ToList (); 
			for (int i = 0; i < l.Count; i++) {
				SetupButtonState (l[i].spellData);
			}
			//			foreach (var item in spellButtonDataDict) {
			//				SetupButtonState (item.Value.spellData);
			//			}
		}
	}

	public void WSCooldownChange(CoolDown cooldown, bool isAdd)
	{
		if(MapSelection.currentView == CurrentView.IsoView){
			if (spellButtonDataDict.ContainsKey (cooldown.spell)) {
				if (isAdd) {
					spellButtonDataDict [cooldown.spell].isCooldown = true;
				}else {
					spellButtonDataDict [cooldown.spell].isCooldown = false;
				}
				spellButtonDataDict [cooldown.spell].StateChange ();
			}
		}
	}

	public void CreateButtons()
	{

		foreach (Transform item in container.transform) {
			Destroy (item.gameObject);
		}
		spellButtonDataDict.Clear ();

		var bttn = new List<RectTransform> ();
		var bttnCG = new List<CanvasGroup> ();
		List<SpellData> sd = new List<SpellData> ();
		for (int i = 0; i < PlayerDataManager.playerData.spells.Count; i++) {
			var curSpell = PlayerDataManager.playerData.spells [i];
			sd.Add (PlayerDataManager.playerData.spells [i]);
		}
		for (int i = 0; i < sd.Count; i++) {
			bttn.Add (InstantiateObject (i * bttnOffset,sd[i]));
		}

		SSC.bttn = bttn;
		SSC.Initialize ();

	}

	void SetupButtonState (SpellData data)
	{
		foreach (var item in data.states) {
			if (!MapSelection.IsSelf) {
				if (item == MarkerSpawner.SelectedMarker.state) {
					spellButtonDataDict [data.id].isActive = true;
					spellButtonDataDict [data.id].StateChange ();
					break;
				}
				else {
					spellButtonDataDict [data.id].isActive = false;
					spellButtonDataDict [data.id].StateChange ();
				}
			}
		}
	}

	RectTransform InstantiateObject(float offset,SpellData data)    
	{
		GameObject g = Instantiate(spellButton, container.transform);
		g.transform.SetParent(container.transform); 
		g.transform.localPosition = Vector3.zero;
		g.transform.localEulerAngles = Vector3.zero;
		g.transform.transform.localScale = Vector3.one;
		g.name = data.id; 

		var rt = g.GetComponent<RectTransform> ();
		rt.anchoredPosition = new Vector2 (offset, 0);

		var sbd = g.GetComponent<SpellButtonData> (); 
		spellButtonDataDict[data.id] = sbd;
		sbd.setupButton (data);


		foreach (var item in PlayerDataManager.playerData.cooldownList) {
			if (item.spell == data.id) {
				spellButtonDataDict [data.id].isCooldown = true;
				spellButtonDataDict [data.id].StateChange ();
			}
		}

		SetupButtonState (data);
		if (spellButtonDataDict.ContainsKey (data.id))
			spellButtonDataDict [data.id].validStates = data.states;
		return  rt; 
	}

	public void SetupSpellInfo( )
	{
		if (currentSpellData.id != previousSpell) {
			spellName.text = DownloadedAssets.spellDictData [currentSpellData.id].spellName; 
			spellDesc.text = DownloadedAssets.spellDictData [currentSpellData.id].spellDescription; 
			spellCost.text = "Cost : " + PlayerDataManager.playerData.spellsDict [currentSpellData.id].cost.ToString ();
			if (currentSpellData.school > 0) {  
				if (!glow [0].activeInHierarchy)
					glow [0].SetActive (true);
				glow [2].SetActive (false);
				glow [1].SetActive (false); 
			} else if (currentSpellData.school < 0) {  
				if (!glow [2].activeInHierarchy)
					glow [2].SetActive (true);
				glow [0].SetActive (false);
				glow [1].SetActive (false); 
			} else {
				if (!glow [1].activeInHierarchy)
					glow [1].SetActive (true);
				glow [2].SetActive (false);
				glow [0].SetActive (false); 
			}
			previousSpell = currentSpellData.id;
		}
	}

	public void ManageSpellButton(bool isActive, string id)
	{
		if (isActive) {
			if (spellButtonDataDict.ContainsKey (currentSpellData.id)) {
				spellButtonDataDict [id].transform.GetChild (2).gameObject.SetActive (true);
				if (!spellButtonDataDict[id].isActive) {
					SetupWarning ("Invalid Target");
				} else if (spellButtonDataDict[id].isCooldown) {
					SetupWarning (spellName.text + " can be used again in 10 minutes.");
				} else if (spellButtonDataDict[id].isLowEnergy) {
					SetupWarning ("Insufficient Energy");
				} else {
					SetupWarning ("");
				}
			}
		}else {
			spellButtonDataDict [id].transform.GetChild (2).gameObject.SetActive (false);
		}
	}

	public void Hide()
	{
		SSC.isActive = false;
		StartCoroutine (ManageScale (true));
	}

	IEnumerator ManageScale (bool isDown)
	{
		if (isDown && SSC.transform.localScale.x == 0) {
			yield break;
		}

		if (!isDown && SSC.transform.localScale.x == 1) {
			yield break;
		}

		float t = 0;
		while (t<=1) {
			t += Time.deltaTime * fadeSpeed;
			if (isDown) {
				SSC.transform.localScale = Vector3.one * Mathf.SmoothStep (1, 0, t);
			} else {
				SSC.transform.localScale = Vector3.one * Mathf.SmoothStep (0, 1, t);
			}
			yield return null;
		}
	}

	public void SetupWarning(string msg)
	{
		if (msg == "") {
			warningText.gameObject.SetActive (false);
		} else {
			warningText.gameObject.SetActive (true);
			warningText.text = msg;
		}
	}

	public void Show()
	{
		SSC.gameObject.SetActive (true);
		CreateButtons ();
		SSC.isActive = true;
		StartCoroutine (ManageScale (false));
	}

}
