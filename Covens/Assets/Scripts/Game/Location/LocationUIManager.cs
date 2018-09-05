using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LocationUIManager : MonoBehaviour
{
	public static LocationUIManager Instance{ get; set;}

	public GameObject locationPrefab;
	GameObject locRune;

	public List<RectTransform> cards = new List<RectTransform>();

	public GameObject spiritCard;
	public GameObject emptyCard;
	public RectTransform container;
	public GameObject SpiritSummonUI;
	public CanvasGroup[] CG;
	public CanvasGroup[] CGPartial;

	public Animator locAnim;
	public Text title;
	public Text ownedBy;

	int bttnDistance;
	float[] distances;
	float[] distanceReposition;
	public float snapSpeed = 1;
	public RectTransform center;
	int minButtonNum;
	bool dragging = false;

	Vector2 ini;
	Vector2 final;

	void Awake(){
		Instance = this;
	}

	public void Escape(){
		locAnim.SetBool ("animate", false);  
		locRune.GetComponent<Animator> ().SetTrigger ("back");
//		Destroy (locRune, 2.5f);
		StartCoroutine (MoveBack ());
		PlayerManager.marker.instance.SetActive(true);
		if(PlayerManager.physicalMarker != null)
			PlayerManager.physicalMarker.instance.SetActive(true);
	}

	public void OnEnterLocation(){
		OnlineMaps.instance.zoom = 16;
		PlayerManager.marker.instance.SetActive(false);
		title.text = MarkerSpawner.SelectedMarker.displayName;
		if(PlayerManager.physicalMarker != null)
			PlayerManager.physicalMarker.instance.SetActive(false);

		locRune = Utilities.InstantiateObject (locationPrefab, MarkerSpawner.SelectedMarker3DT); 
		locRune.transform.localRotation = Quaternion.Euler (90, 0, 0); 
		locAnim.SetBool ("animate", true); 
		StartCoroutine (MoveMap ()); 
	}

	IEnumerator MoveMap ()
	{
		var OM = OnlineMaps.instance;
		float t = 0;
		ini = OnlineMaps.instance.position;
		final = MarkerSpawner.SelectedMarkerPos;
		final.x += 0.00043027191f;
		final.y += 0.00055482578f;
		while (t <= 1) {
			t += Time.deltaTime * 2;
			OM.position = Vector2.Lerp (ini, final, t);


			foreach (var item in CG) {
				item.alpha = Mathf.SmoothStep (1, 0, t);
			}
			foreach (var item in CGPartial) {
				item.alpha = Mathf.SmoothStep (1, .3f, t);
			}
			yield return 0;
		}
	}

	IEnumerator MoveBack ()
	{
		var OM = OnlineMaps.instance;
		float t = 1;

		while (t >= 0) {
			t -= Time.deltaTime ;
			OM.position = Vector2.Lerp (ini, final, t*1.5f);

			foreach (var item in CG) {
				item.alpha = Mathf.SmoothStep (1, 0, t);
			}
			foreach (var item in CGPartial) {
				item.alpha = Mathf.SmoothStep (1, .3f, t);
			}
			yield return 0;
		}
	}

	void Update()
	{
		if (Input.GetMouseButtonDown (0) ) {
			var ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit)) {
				if (hit.collider.gameObject.name == "button") {
					OnSummon ();
				}
			} 
		}
		if (SpiritSummonUI.activeInHierarchy) {
			for (int i = 0; i < cards.Count; i++) {
				distanceReposition[i] = center.position.x - cards [i].position.x; 
				distances [i] = Mathf.Abs (distanceReposition [i]);
			}

			float minDistance = Mathf.Min (distances);
			for (int a = 0; a < cards.Count; a++) {
				if (minDistance == distances [a]) {
					minButtonNum = a;
				}
			}

			if (!dragging) {
				LerpToBttn(-cards[minButtonNum].anchoredPosition.x);
			}
		}
	}

	public void OnSummon()
	{
		SpiritSummonUI.SetActive (true);

		var empty = Utilities.InstantiateObject (emptyCard, container.transform); 
		empty.name = "empty"; 
		var empty1 = Utilities.InstantiateObject (emptyCard, container.transform); 
		empty1.name = "empty"; 

		for (int i = 0; i < 5; i++) {
			var g = Utilities.InstantiateObject (spiritCard, container.transform);
			g.name = i.ToString();
			cards.Add (g.GetComponent<RectTransform>());
		}

		var empty2 = Utilities.InstantiateObject (emptyCard, container.transform); 
		empty2.name = "empty"; 
		var empty3 = Utilities.InstantiateObject (emptyCard, container.transform); 
		empty3.name = "empty"; 

		distances = new float[cards.Count];
		distanceReposition = new float[cards.Count]; 
		bttnDistance = (int)Mathf.Abs (cards [1].anchoredPosition.x - cards [0].anchoredPosition.x);
	}

	void LerpToBttn(float position)
	{
		float newX = Mathf.Lerp (container.anchoredPosition.x, position, Time.deltaTime * snapSpeed);
		Vector2 newPosition = new Vector2 (newX, container.anchoredPosition.y);
		container.anchoredPosition = newPosition;
	}

	public void StartDragging()
	{
		dragging = true;
	}

	public void StopDragging()
	{
		dragging = false;
	}
}

