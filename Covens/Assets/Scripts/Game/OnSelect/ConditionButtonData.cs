using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ConditionButtonData : MonoBehaviour {
	public Dictionary<string,Conditions> conditions = new Dictionary<string, Conditions>();
	public Text counterText;
	public Text desc;
	public GameObject counterObject;
	public Image spell;
	bool isClicked = false;
	int increment = 1;
	public GameObject fx;
	public GameObject fxTrigger;
	public GameObject onSelect;
	public Transform descObject;
	void Update()
	{
		if (isClicked) {
			if (Input.GetMouseButtonDown (0)) {
				close ();
			}
		}
	}

	public void Setup( bool isAdd = false )
	{
		counterText.text = conditions.Count.ToString();
		if (isAdd)
			return;
		spell.sprite = DownloadedAssets.getGlyph(conditions.ElementAt(0).Value.spellID); 
		desc.text = DownloadedAssets.conditionsDictData[conditions.ElementAt(0).Value.condition].conditionDescription;
	}

	public void Animate()
	{
		if (!isClicked) {
		onSelect.SetActive (true);
		StartCoroutine (scaleUp ());
			isClicked = true;
		} 
	}

	public void ConditionTrigger()
	{
		fxTrigger.SetActive (true);
	}

	public void ConditionChange()
	{
		fx.SetActive (true);
	}



	void close()
	{
		onSelect.SetActive (false);
		StartCoroutine (scaleDown ());
	}

	IEnumerator scaleUp()
	{
		float t = 0;
		while (t<=1) {
			t += Time.deltaTime * 3.5f;
			descObject.localScale = Vector3.one * Mathf.SmoothStep (0, 1, t);
			yield return 0;
		}
	}

	IEnumerator scaleDown()
	{
		float t = 0;
		while (t<=1) {
			t += Time.deltaTime * 5.5f;
			descObject.localScale = Vector3.one * Mathf.SmoothStep (1, 0, t);
			yield return 0;
		}
		isClicked = false;
	}
}
