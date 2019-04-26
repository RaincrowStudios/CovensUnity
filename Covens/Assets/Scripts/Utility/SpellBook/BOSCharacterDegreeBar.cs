using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BOSCharacterDegreeBar : MonoBehaviour {
	public Slider bar;
	//public float xPos;
	//public GameObject Needle;
	//public Vector3 needleVec;
	// Use this for initialization
	void Start () {
		bar = transform.GetChild (0).GetComponent<Slider>();
		SetupBar ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public void SetupBar() {
		bar.value = ((float)PlayerDataManager.playerData.degree) + 15f;
		//xPos = MapUtils.scale (1f, 29f, 0, bar.rect.width, (float)PlayerDataManager.playerData.degree +15f);
		//xPos = MapUtils.scale (bar.rect.width, 0f, 1f, 29f, (float)PlayerDataManager.playerData.degree + 15f);
		//Debug.Log (xPos);
		//Debug.Log (bar.rect.width);
		//Debug.Log (PlayerDataManager.playerData.degree + 15f);
		//var p = Utilities.InstantiateObject (Needle, bar);
		//needleVec = new Vector3(xPos, bar.localPosition.y, bar.localPosition.z);
		//LeanTween.moveLocal (p, needleVec, 0.001f);
		//p.transform.localPosition = new Vector3(xPos, bar.localPosition.y, bar.localPosition.z);

	}
}
