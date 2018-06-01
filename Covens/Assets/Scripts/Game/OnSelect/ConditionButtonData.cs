using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConditionButtonData : MonoBehaviour {

	public Text counterText;
	public Text desc;
	public GameObject counterObject;
	public Image spell;
	bool isClicked = false;
	public Animator anim;
	int increment = 1;
	void Update()
	{
		if (isClicked) {
			if (Input.GetMouseButtonDown (0)) {
				close ();
			}
		}
	}

	public void IncrementCounter()
	{
		increment++;
		counterObject.SetActive (true);
		counterText.text = increment.ToString ();
	}

	public void Setup(Sprite spellGlyph, string description )
	{
		spell.sprite = spellGlyph;
		desc.text = description;
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
