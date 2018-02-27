using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupMarker : MonoBehaviour {

	public SpriteRenderer body;
	public SpriteRenderer glow;

	public Sprite maleWhite;
	public Sprite maleShadow;
	public Sprite maleGrey;
	public Sprite femaleWhite;
	public Sprite femaleShadow;
	public Sprite femaleGrey;

	public Sprite male;
	public Sprite female;
	public float iniScale;
	public float s;
	float curZoom,prevZoom =0;
	OnlineMaps map;
	public OnlineMapsMarker3D m;
	public enum playerType
	{
		shadow,
		white,
		grey
	};
		
	public void Setup(playerType t , int distance, bool isMale)
	{
//		print (t);
//		print (isMale);
		if (isMale) {
			body.sprite = male;
	
			if (t == playerType.grey) {
				body.sprite = maleGrey;
				glow.color = Utilities.Blue;
			} else if (t == playerType.shadow) {
				body.sprite = maleShadow;
				glow.color = Utilities.Purple;
			} else {
				body.sprite = maleWhite;
				glow.color = Utilities.Orange;
			}
		} else {
			if (t == playerType.grey) {
				body.sprite = femaleGrey;
				glow.color = Utilities.Blue;
			} else if (t == playerType.shadow) {
				body.sprite = femaleShadow;
				glow.color = Utilities.Purple;
			} else {
				body.sprite = femaleWhite;
				glow.color = Utilities.Orange;
			}
		}
		if (distance == 1) {
			glow.gameObject.SetActive (false);
		} else if (distance == 2) {
			body.color = new Color (body.color.r, body.color.g, body.color.b, .83f);
			glow.gameObject.SetActive (false);
		} else if (distance == 3) {
			body.color = new Color (body.color.r, body.color.g, body.color.b, .65f);
			glow.gameObject.SetActive (false);
		} else if (distance == 4) {
			body.color = new Color (body.color.r, body.color.g, body.color.b, .43f);
			glow.color = new Color (glow.color.r, glow.color.g, glow.color.b, .5f);
		}  else {
			body.color = new Color (body.color.r, body.color.g, body.color.b, .2f);
		}

	}
}
