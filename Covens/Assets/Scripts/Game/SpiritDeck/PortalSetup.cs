using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PortalSetup : MonoBehaviour {

	public SpriteRenderer glowCG;
	public ParticleSystem ps1;
	public ParticleSystem ps2;
	float speed = 1f;
	public Image spiritIcon;
	public string id;
	ParticleSystemRenderer pr1;
	ParticleSystemRenderer pr2;


	void OnEnable()
	{
		pr1 = ps1.GetComponent<ParticleSystemRenderer> ();
		pr2 = ps2.GetComponent<ParticleSystemRenderer> ();
		glowCG.color = new Color(1,1,1,0);
		var p1 = ps1.emission;
		p1.enabled = false;
		var p2 = ps2.emission;
		p2.enabled = false;
		glowCG.sortingOrder = 202;
		pr1.sortingOrder = 203;
	}

	public void Focus()
	{
		glowCG.sortingOrder = 207;
		pr1.sortingOrder = 208;
		var p1 = ps1.emission;
		p1.enabled = true;
		var p2 = ps2.emission;
		p2.enabled = true;
		StartCoroutine (FocusHelper ());
	}

	IEnumerator FocusHelper()
	{
		float t = 0;
		while (t<=1) {
			t += Time.deltaTime * speed;
			glowCG.color = new Color(1,1,1, Mathf.SmoothStep (0, 1, t));
			
			yield return null;
		}
			
	}

	public void DeFocus()
	{
		if (ps1.emission.enabled == false)
			return;
		glowCG.sortingOrder = 202;
		pr1.sortingOrder = 203;

		var p1 = ps1.emission;
		p1.enabled = false;
		var p2 = ps2.emission;
		p2.enabled = false;
		StartCoroutine (DeFocusHelper ());
	}

	IEnumerator DeFocusHelper()
	{
		float t = 1;
		while (t>=0) {
			t -= Time.deltaTime * speed;
			glowCG.color = new Color(1,1,1, Mathf.SmoothStep (0, 1, t));
			yield return null;
		}

	}
}
