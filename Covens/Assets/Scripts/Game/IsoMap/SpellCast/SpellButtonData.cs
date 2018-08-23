using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SpellButtonData : MonoBehaviour
{
	public Image spellSprite;
	public Image outerSprite;
	public Color disabledColor;
	public SpellData spellData;
	[HideInInspector] 
	public bool isActive = true;
	public bool isCooldown = false;
	public bool isLowEnergy = false;
	 ParticleSystem PS1;
	 ParticleSystem PS2;
	 Image particleSprite;
	public List<string> validStates = new List<string>();
	public void setupButton(SpellData data)
	{
		spellData = data;
		if (data.school > 0) {
			DestroyImmediate (transform.GetChild (3).gameObject);
			DestroyImmediate (transform.GetChild (3).gameObject);
		} else if (data.school == 0) {
			DestroyImmediate (transform.GetChild (3).gameObject);
			DestroyImmediate (transform.GetChild (2).gameObject);
		} else {
			DestroyImmediate (transform.GetChild (4).gameObject);
			DestroyImmediate (transform.GetChild (2).gameObject);
		}
		particleSprite = transform.GetChild (transform.childCount-1).GetChild (0).GetComponent<Image> ();
		PS1 = transform.GetChild (transform.childCount-1).GetChild (1).GetComponent<ParticleSystem> ();
		PS2 = transform.GetChild (transform.childCount-1).GetChild (2).GetComponent<ParticleSystem> ();
		spellSprite.sprite = DownloadedAssets.getGlyph (data.id);
	}

	public void OnClick()
	{
		if(isActive) 
			SpellCastUIManager.Instance.SelectSpell ( );
	}

	public void StateChange( )
	{
		var SCM =  SpellCarouselManager.Instance;
		if (isActive) { 
			if (spellData.cost > PlayerDataManager.playerData.energy) {
				isLowEnergy = true;
				ShowButtonFX (false);
			} else {
				if (isCooldown) {
					ShowButtonFX (false);
				} else {
					isLowEnergy = false;
					ShowButtonFX (true);
				}
			}
		} else {
			if (SpellCastUIManager.isSpellSelected) {
				SpellCastUIManager.Instance.SpellClose ();
			}
			ShowButtonFX (false);
		}
	}

	void ShowButtonFX(bool show){ 
		if (!show) {
			spellSprite.color = disabledColor;
			outerSprite.color = disabledColor;
		} else {
			spellSprite.color = Color.white;
			outerSprite.color = Color.white;
		}
		particleSprite.enabled = show; 
		var em1 = PS1.emission;
		em1.enabled = show;
		var em2 = PS2.emission;
		em2.enabled = show;
	}

}

