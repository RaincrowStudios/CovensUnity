using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConditionButtonData : MonoBehaviour {

	public GameObject info;
	public Text counterText;
	public Text desc;
	public GameObject counterObject;
	public Image spell;
	bool isClicked = false;

	public Animator anim;

	void Update()
	{
		if (isClicked) {
			if (Input.GetMouseButtonDown (0)) {
				close ();
			}
		}
	}

	void Start()
	{
		info.SetActive (false);	
		info.SetActive (true);	
	}

	public void Setup(Sprite spellGlyph, string description, int counter)
	{
		if (counter > 1) {
			counterObject.SetActive (true);
			counterText.text = counter.ToString ();
		} else {
			counterObject.SetActive (false);
		}
	}

	public void Animate()
	{
		if (!isClicked) {
			anim.SetBool ("animate", true);
			isClicked = true;
		} 
	}

	void close()
	{
		anim.SetBool ("animate", false);
		Invoke ("DisableClick", .4f);
	}
	void DisableClick()
	{
		isClicked = false;
	}

}
