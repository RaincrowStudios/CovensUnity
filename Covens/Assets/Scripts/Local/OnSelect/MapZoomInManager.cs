using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapZoomInManager : MonoBehaviour {

	public static MapZoomInManager Instance {get; set;}

	OnlineMapsTileSetControl mapControl;
	OnlineMaps map;
	public float Offset = 0;
	public float moveSpeed = 1;
	Vector2 curPos;
	public CanvasGroup MainUICanvasGroup;
	public GameObject SpellUI;
 	CanvasGroup SpellUICanvasGroup;

	public GameObject ArenaParticleFx;

	public Light spotLight;

	void Awake()
	{
		Instance = this;		
	}

	void Start()
	{
		mapControl= OnlineMapsTileSetControl.instance;  
		map = OnlineMaps.instance;
		SpellUICanvasGroup = SpellUI.GetComponent<CanvasGroup> ();
	}

	public void OnSelect (Vector2 pos) {
		SpellUI.SetActive (true);
		ArenaParticleFx.SetActive (false);
		SpellUICanvasGroup.alpha = 0;

		Transform ring = PlayerManager.AttackRing.transform.GetChild(0);
		foreach (Transform item in ring) {
			item.gameObject.SetActive (false);
		}
		mapControl.allowUserControl = false; 
		curPos = map.position;
		StartCoroutine (PersepectiveZoomIn (pos));
	} 

	IEnumerator PersepectiveZoomIn(Vector2 focusPos)
	{
		
		float t = 0;
		while (t <= 1f) {
			t += Time.deltaTime * moveSpeed;
			map.position = Vector2.Lerp (curPos, focusPos, Mathf.SmoothStep (0, 1f, t));
			mapControl.cameraRotation = Vector2.Lerp (Vector2.zero, new Vector2 (50.8f, 0), Mathf.SmoothStep (0, 1f, Mathf.SmoothStep (0, 1f, t)));
			mapControl.cameraDistance = Mathf.SmoothStep (550f, 483.2f, Mathf.SmoothStep (0, 1f, t));
			MainUICanvasGroup.alpha = Mathf.SmoothStep (1f, 0f, t);
			SpellUICanvasGroup.alpha = t;
			RenderSettings.ambientLight = Color.Lerp (new Color (.875f, .875f, .875f), new Color (.05f, .05f, .05f), Mathf.SmoothStep (0, 1f, t));
			spotLight.intensity = Mathf.SmoothStep (0, 12.2f, t);
			yield return null;
		}

	}


	public void Back()
	{
		StartCoroutine (PersepectiveZoomOut ());
	}

	IEnumerator PersepectiveZoomOut( )
	{
		float t = 1;
		while (t >= 0f) {
			t -= Time.deltaTime * moveSpeed;
			mapControl.cameraRotation = Vector2.Lerp (Vector2.zero, new Vector2 (50.8f, 0), Mathf.SmoothStep (0, 1f, t));
			mapControl.cameraDistance = Mathf.SmoothStep (550f, 483.2f, t);
			MainUICanvasGroup.alpha = Mathf.SmoothStep (1f, 0f, t);
			SpellUICanvasGroup.alpha = t;
			RenderSettings.ambientLight = Color.Lerp (new Color (.875f, .875f, .875f), new Color (.05f, .05f, .05f), Mathf.SmoothStep (0, 1f, t));
			spotLight.intensity = Mathf.SmoothStep (0, 12.2f, t);
			yield return null;
		}
		ArenaParticleFx.SetActive (true);
		Transform ring = PlayerManager.AttackRing.transform.GetChild(0);
		foreach (Transform item in ring) {
			item.gameObject.SetActive (true);
		}
		mapControl.allowUserControl = true; 
		SpellUI.SetActive (false);

	}
		
}
