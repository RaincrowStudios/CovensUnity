using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SummoningUI : MonoBehaviour {
	
	public static SummoningUI Instance {get; set;}
	public RectTransform runeBase;
	public RectTransform outerText1;
	public RectTransform outerText2;
	public RectTransform planets;

	public RectTransform herbT;
	public RectTransform toolT;
	public RectTransform gemT;

	public GameObject gemParticle;
	public GameObject gemParticleClouds;
	public Image GemIcon;

	public GameObject toolParticle;
	public GameObject toolParticleClouds;
	public Image toolIcon;

	public GameObject herbParticle;
	public GameObject herbParticleClouds;
	public Image herbIcon;

	public float moveSpeed = 3;
	bool isMoved = false;


	void Awake(){
		Instance = this;
	}

	void Start () {
		
	}

	public void AddIngredient (int i)
	{
		if (i == 0) {
			gemParticle.SetActive (false);
			toolParticle.SetActive (false);
			gemParticleClouds.SetActive (false);
			toolParticleClouds.SetActive (false);
			herbParticle.SetActive (true);
			herbParticleClouds.SetActive (true);
			GemIcon.color = toolIcon.color = new Color (.3f, .3f, .3f);
			herbIcon.color = Color.white;
			gemT.anchoredPosition3D = new Vector3 (gemT.anchoredPosition3D.x, gemT.anchoredPosition3D.y, 0);
			toolT.anchoredPosition3D = new Vector3 (toolT.anchoredPosition3D.x, toolT.anchoredPosition3D.y, 0);
			herbT.anchoredPosition3D  = new Vector3 (herbT.anchoredPosition3D.x, herbT.anchoredPosition3D.y, -50);
		} 
		else if (i == 1) {
			gemParticle.SetActive (true);
			toolParticle.SetActive (false);
			gemParticleClouds.SetActive (true);
			toolParticleClouds.SetActive (false);
			herbParticle.SetActive (false);
			herbParticleClouds.SetActive (false);
			herbIcon.color = toolIcon.color = new Color (.3f, .3f, .3f);
			GemIcon.color = Color.white;
			herbT.anchoredPosition3D = new Vector3 (herbT.anchoredPosition3D.x, herbT.anchoredPosition3D.y, 0);
			toolT.anchoredPosition3D = new Vector3 (toolT.anchoredPosition3D.x, toolT.anchoredPosition3D.y, 0);
			gemT.anchoredPosition3D  = new Vector3 (gemT.anchoredPosition3D.x, gemT.anchoredPosition3D.y, -50);
		} 
		else   {
			gemParticle.SetActive (false);
			toolParticle.SetActive (true);
			gemParticleClouds.SetActive (false);
			toolParticleClouds.SetActive (true);
			herbParticle.SetActive (false);
			herbParticleClouds.SetActive (false);
			herbIcon.color = GemIcon.color = new Color (.3f, .3f, .3f);
			toolIcon.color = Color.white;
			herbT.anchoredPosition3D = new Vector3 (herbT.anchoredPosition3D.x, herbT.anchoredPosition3D.y, 0);
			gemT.anchoredPosition3D = new Vector3 (gemT.anchoredPosition3D.x, gemT.anchoredPosition3D.y, 0);
			toolT.anchoredPosition3D  = new Vector3 (toolT.anchoredPosition3D.x, toolT.anchoredPosition3D.y, -50);
		}
		if (!isMoved) {
			StartCoroutine (IngrdientIn ());
			isMoved = true;
		}
	}

	public void IngredientBack ()
	{
		isMoved = false;
		StartCoroutine (IngredientOut ());
		gemParticle.SetActive (false);
		toolParticle.SetActive (false);
		gemParticleClouds.SetActive (false);
		toolParticleClouds.SetActive (false);
		herbParticle.SetActive (false);
		herbParticleClouds.SetActive (false);
		toolIcon.color = herbIcon.color = GemIcon.color = new Color (.3f, .3f, .3f);
		herbT.anchoredPosition3D = new Vector3 (herbT.anchoredPosition3D.x, herbT.anchoredPosition3D.y, 0);
		gemT.anchoredPosition3D = new Vector3 (gemT.anchoredPosition3D.x, gemT.anchoredPosition3D.y, 0);
		toolT.anchoredPosition3D  = new Vector3 (toolT.anchoredPosition3D.x, toolT.anchoredPosition3D.y, 0);
	}

	IEnumerator IngrdientIn ()
	{
		float t = 0;
		while (t <= 1f) {
			t += Time.deltaTime*moveSpeed;
			IngredientAnimateIn (t);
			yield return null;
		}
	}


	IEnumerator IngredientOut ()
	{
		float t = 1;
		while (t >= 0f) {
			t -= Time.deltaTime*moveSpeed;
			IngredientAnimateIn (t);
			yield return null;
		}
	}

	void IngredientAnimateIn (float t)
	{
		runeBase.anchoredPosition = new Vector2( Mathf.SmoothStep (0, 643, t),0);
		planets.anchoredPosition3D = new Vector3( 0,0,Mathf.SmoothStep (0, -23, t));
		outerText1.anchoredPosition3D = new Vector3( 0,0,Mathf.SmoothStep (0, 33, t));
		outerText2.anchoredPosition3D = new Vector3( 0,0,Mathf.SmoothStep (0, 40, t));
		runeBase.localEulerAngles = new Vector3 (0, Mathf.SmoothStep (0, 40, t), 0);
		var pos = Vector3.Lerp (new Vector3 (0, 0, 0), new Vector3 (0, 0, 0), Mathf.SmoothStep (0, 1f, t));
	}
}
