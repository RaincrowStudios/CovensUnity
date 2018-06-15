using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GestureRecognizer;

public class SpellGestureManager : MonoBehaviour {
	public static SpellGestureManager Instance { get; set;}

	DrawDetector DD;
	public float minMatch = 0.05f;
	public SpellTraceManager STM;

	void Awake()
	{
		Instance = this;
	}
	void Start()
	{
	}

	public void OnRecognize(RecognitionResult result){
		if (result != RecognitionResult.Empty && result.gesture.id == SpellCarousel.currentSpell) {
			print( result.gesture.id + "\n" + Mathf.RoundToInt (result.score.score * 100) + "%");
			SpellCastAPI.CastSpell ();
			STM.enabled = false;
		} else {
			print ("Not Recognized");
		}
	}
		

	public void SetGestureLibrary(Spells spell)
	{
		DD = GetComponent<DrawDetector> ();

		if (spell == Spells.spell_bind) {
			DD.maxLines = 2;
		}else if (spell == Spells.spell_hex) {
			DD.maxLines = 1;
		}else if (spell == Spells.spell_sunEater) {
			DD.maxLines = 2;
		}else if (spell == Spells.spell_dispel) {
			DD.maxLines = 1;
		}else if (spell == Spells.spell_grace) {
			DD.maxLines = 1;
		}else if (spell == Spells.spell_invisibility) {
			DD.maxLines = 1;
		}else if (spell == Spells.spell_resurrection) {
			DD.maxLines = 2;
		}else if (spell == Spells.spell_silence) {
			DD.maxLines = 2;
		}else if (spell == Spells.spell_leech) {
			DD.maxLines = 3;
		}else if (spell == Spells.spell_whiteFlame) {
			DD.maxLines = 2;
		}else if (spell == Spells.spell_bless) {
			DD.maxLines = 1;
		}
		DD.ClearLines ();
	}

}
