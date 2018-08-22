using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GestureRecognizer;

public class SpellGestureManager : MonoBehaviour {
	public static SpellGestureManager Instance { get; set;}

	public SpellTraceManager STM;
	DrawDetector DD;
	void Awake()
	{
		Instance = this;
	}
	void Start()
	{
	}

	public void OnRecognize(RecognitionResult result){

		if (result != RecognitionResult.Empty && result.gesture.id == DownloadedAssets.spellDictData[SpellCarouselManager.currentSpellData.id].spellGlyph.ToString()) {
			print( result.gesture.id + "\n" + Mathf.RoundToInt (result.score.score * 100) + "%");
			SpellCastAPI.CastSpell ();
			SpellCastUIManager.SpellAccuracy = Mathf.RoundToInt (result.score.score * 100);
			STM.enabled = false;
		} else {
			print ("Not Recognized");
		}
	}

	void OnEnable()
	{
		SetGestureLibrary (DownloadedAssets.spellDictData[SpellCarouselManager.currentSpellData.id].spellGlyph);
	}

	public void SetGestureLibrary(int glyphID)
	{
		DD = GetComponent<DrawDetector> ();

		if (glyphID == 1) {
			DD.maxLines = 3;
		}else if (glyphID == 2) {
			DD.maxLines = 2;
		}else if (glyphID == 3) {
			DD.maxLines = 1;
		}else if (glyphID ==4) {
			DD.maxLines = 2;
		}else if (glyphID ==5) {
			DD.maxLines = 1;
		}else if (glyphID == 6) {
			DD.maxLines = 2;
		}else if (glyphID == 7) {
			DD.maxLines = 1;
		}else if (glyphID == 8) {
			DD.maxLines = 1;
		}else if (glyphID ==9) {
			DD.maxLines = 1;
		}else if (glyphID == 10) {
			DD.maxLines = 1;
		}else if (glyphID == 11) {
			DD.maxLines = 2;
		}else if (glyphID == 12) {
			DD.maxLines = 2;
		}else if (glyphID == 13) {
			DD.maxLines = 2;
		}else if (glyphID == 14) {
			DD.maxLines = 2;
		}else if (glyphID == 15) {
			DD.maxLines = 2;
		}else if (glyphID == 16) {
			DD.maxLines = 1;
		}
		DD.ClearLines ();
	}

}
