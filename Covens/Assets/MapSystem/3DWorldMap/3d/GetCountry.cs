using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetCountry : MonoBehaviour {

	[SerializeField] private Camera cam;
    [SerializeField] private SpriteMapsController SMC;
    [SerializeField] private LayerMask raycastLayers;

	public Text type;
	public Text id;
	public GameObject container;
	public Color initial;
	public Color selected;
	Renderer previousRend;
	public float speed;
	string previousText = "";

	void Start()
	{
        SMC.onChangePosition += UpdateUI;
        SMC.onChangeZoom += UpdateUI;
	}

    void UpdateUI()
	{		
		if (cam.orthographicSize > 1.5f) {
			type.text = "Country";
		} else {
			type.text = "Dominion";
		}

		Ray ray = cam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
		RaycastHit hit;

		if (Physics.Raycast (ray, out hit, Mathf.Infinity, raycastLayers))
        {
			if (container.activeSelf == false || previousText != hit.transform.name) {
				container.SetActive (true);
				id.text = hit.transform.name;
				previousText = hit.transform.name;
				if (cam.orthographicSize <= 1.5f) {
					if (previousRend != null)
						previousRend.material.color = initial;

					var ren = hit.transform.GetComponent<Renderer> ();
					initial = ren.material.color;
					ren.material.color = selected;

					previousRend = ren;
				}
			}
//			print ("I'm looking at " + hit.transform.name);
		} else {
			container.SetActive (false);
		}
	}

//	IEnumerator changeColor ()
//	{
//		float t = 0;
//		while (t <= 1) {
//			t += Time.deltaTime;
//			yield return 0;
//		}
//	}
}
