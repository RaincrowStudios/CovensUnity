using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;

public class InventorySrollManager : MonoBehaviour {
	
	public static InventorySrollManager Instance {get; set;}
	public IngredientType Type = IngredientType.tool;
	public GameObject inventoryPrefab;
	public Sprite icon;
	float step =0;
	public Transform container;
	public List<Transform> allItems = new List<Transform>();
	List<IngredientDict> inventory = new List<IngredientDict>();
	public string colliderName;
	public float speed= 1;
	public float MaxSpeed= 18;

	public float inertia = 3;
	public bool CanRotate;
	bool isClicked = false;
	int direction = 0;

	float rotateSpeed;

	void Awake(){
		Instance = this;
	}

	void Update()
	{
		if (Input.GetMouseButtonDown (0)) {
			PointerEventData ped = new PointerEventData (null);
			ped.position = Input.mousePosition;
			List<RaycastResult> results = new List<RaycastResult> ();
			EventSystem.current.RaycastAll(ped, results);
			foreach (var item in results) {
				if (item.gameObject.name == colliderName)
					isClicked = true;

				print (item.gameObject.name);
			}
			if(isClicked)
				StartRotation ();
		}
		if (Input.GetMouseButtonUp (0)) {
			StopRotation ();
			isClicked = false;
		}
		if (CanRotate) {
			rotateSpeed = Input.GetAxis ("Mouse Y") * speed;
			if (rotateSpeed > 0)
				direction = 1;
			else
				direction = -1;

			rotateSpeed = Mathf.Clamp(  Mathf.Abs (rotateSpeed), 0, MaxSpeed);
			transform.Rotate (0, 0, rotateSpeed*direction );
		}
	}


	public void StartRotation()
	{
		StopAllCoroutines ();
		CanRotate = true;
		StartCoroutine (RotateGemWheel ());
	}

	public void StopRotation()
	{
		CanRotate = false;
		StartCoroutine (RotateGemWheel());
	}


	IEnumerator RotateGemWheel()
	{
		while (rotateSpeed>0) {
			rotateSpeed -= Time.deltaTime*inertia;
			transform.Rotate (0, 0, rotateSpeed*direction );
			yield return null;
		} 
	}
	void OnEnable () {
		var curDict = new Dictionary<string,InventoryItems> ();
		if (Type == IngredientType.tool) {
			 curDict = PlayerDataManager.playerData.ingredients.toolsDict;  
		} else {
			 curDict = PlayerDataManager.playerData.ingredients.herbsDict; 
		}
		var str = Type.ToString (); 
		inventory.Clear ();
		foreach (var item in DownloadedAssets.ingredientDictData) {
			if (item.Value.type == str)
				inventory.Add (item.Value);
		}
		float ItemCount = inventory.Count; 
		print (ItemCount + " " + Type );
		step = 360.0f / ItemCount;
		foreach (Transform item in container) {
			allItems.Clear ();
			Destroy (item.gameObject);
		}
		int i = 0;
		int activeItems = 0;
		foreach (var item in curDict)  {
			var g = Utilities.InstantiateObject (inventoryPrefab, container);
			g.GetComponent<InventoryItemManager> ().Setup (DownloadedAssets.ingredientDictData[ item.Key].name,item.Value.count, item.Key,true);   
			g.transform.localEulerAngles = new Vector3 (0, 0, i * step);
			allItems.Add (g.transform.GetChild (0));
			i++;
			activeItems++;
		}
		float rot = -(activeItems * step) / 2;
		transform.Rotate (0, 0, rot);
		foreach (var item in inventory) {
			if (!curDict.ContainsKey (item.id)) {
				var g = Utilities.InstantiateObject (inventoryPrefab, container);
				g.GetComponent<InventoryItemManager> ().Setup (item.name,0, item.id,false); 
				g.transform.localEulerAngles = new Vector3 (0, 0, i * step);
				allItems.Add (g.transform.GetChild (0));
				i++;
			}
		}
	}


	public void OnClick(string id)
	{
		InventoryInfo.Instance.Show (id, icon);
	}
}
