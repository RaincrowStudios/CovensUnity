using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerTween : MonoBehaviour {

	public GameObject closeUp;
	public GameObject icon;
	bool isClose;
	public SpriteRenderer sp;
	public enum MyEnum
	{
		spirit,item,witch
	}

	public MyEnum type;

	void Start(){
//		if (type == MyEnum.spirit) {
//			sp.sprite = SpriteHolder.instance.spirits [Random.Range (0, SpriteHolder.instance.spirits.Length)];
//		} else if (type == MyEnum.witch) {
//			sp.sprite = SpriteHolder.instance.player [Random.Range (0, SpriteHolder.instance.player.Length)];
//		
//		} else {
//			sp.sprite = SpriteHolder.instance.collectible [Random.Range (0, SpriteHolder.instance.collectible.Length)];
//		
//		}

	}
	// Use this for initialization
	void OnTriggerEnter(){
		
		if (!isClose) {
			LeanTween.scale (icon, Vector3.zero, .3f);
			LeanTween.scale (closeUp, Vector3.one, .3f);
			isClose = true;
		}
	}
	void OnTriggerExit(){
		if (isClose) {
			LeanTween.scale (icon, Vector3.one, .3f);
			LeanTween.scale (closeUp, Vector3.zero, .3f);
			isClose = false;
		}
	}
}
