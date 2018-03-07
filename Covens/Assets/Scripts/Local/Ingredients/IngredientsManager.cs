using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngredientsManager : MonoBehaviour {
	
	public static IngredientsManager Instance {get; set;}

	public GameObject ingredientsContainer;

	public Transform container;
	public GameObject ingredientPrefab;

	public GameObject herbCountObject;
	public GameObject  gemCountObject;
	public GameObject toolCountObject;

	public GameObject gemPS;
	public GameObject herbPS;
	public GameObject toolPS;

	public GameObject clearObject;
	public GameObject selectedItem;
	public Text selectedItemText;
	public Text TitleName;
	public GameObject clearWarning;

	int gemCount;
	public int herbCount;
	int toolCount;

	Dictionary<string,GameObject> allItems = new Dictionary<string,GameObject>();

	InventoryItems deletedHerb;
	InventoryItems deletedTool;
	InventoryItems deletedGem;

	public string addedHerb;
	public string addedTool;
	public string addedGem;

	MarkerSpawner.MarkerType currentType;

	void Awake(){
		Instance = this;
	}

	public void init()
	{
		foreach (var item in allItems) {
			Destroy (item.Value);
		}
		allItems.Clear ();
		clearWarning.SetActive (false);
		ingredientsContainer.SetActive (true);
		clearObject.SetActive (false);
		if (herbCount == 0) {
			herbCountObject.SetActive (false);
		} else
			clearObject.SetActive (true);
		if (toolCount == 0) {
			toolCountObject.SetActive (false);
		}else
			clearObject.SetActive (true);
		if (gemCount == 0) {
			gemCountObject.SetActive (false);
		}else
			clearObject.SetActive (true);

	}

	public void InitializeGems () {

		if (gemCount != 0) {
			selectedItem.SetActive (true);
			selectedItemText.text = addedGem;
		}else
			selectedItem.SetActive (false);
		gemPS.SetActive (true);
		toolPS.SetActive (false);
		herbPS.SetActive (false);
		currentType = MarkerSpawner.MarkerType.gem;
		init ();
		TitleName.text = "Gems";
		PopulateItem (PlayerDataManager.playerData.inventory.gemsDict );
	}

	public void InitializeTools () {
		if (toolCount != 0) {
			selectedItem.SetActive (true);
			selectedItemText.text = addedTool;
		}else
			selectedItem.SetActive (false);
		gemPS.SetActive (false);
		toolPS.SetActive (true);
		herbPS.SetActive (false);
		currentType = MarkerSpawner.MarkerType.tool;
		init ();
		TitleName.text = "Tools";
		PopulateItem (PlayerDataManager.playerData.inventory.toolsDict );
	}

	public void InitializeHerbs () {
		if (herbCount != 0) {
			selectedItem.SetActive (true);
			selectedItemText.text = addedHerb;
		}else
			selectedItem.SetActive (false);
		gemPS.SetActive (false);
		toolPS.SetActive (false);
		herbPS.SetActive (true);
		currentType = MarkerSpawner.MarkerType.herb;
		init ();
		TitleName.text = "Botanicals";
		PopulateItem (PlayerDataManager.playerData.inventory.herbsDict );
	}

	void PopulateItem( Dictionary<string,InventoryItems> dict )
	{
		foreach (var item in dict) {
			CreateItem (item.Key, item.Value.count,currentType);
		}
	}

	void CreateItem(string id, int count, MarkerSpawner.MarkerType type)
	{
		var g = Utilities.InstantiateObject (ingredientPrefab, container);
		g.GetComponent<IngredientButtonData> ().type = type;
		g.name = id;
		g.GetComponent<Text> ().text = id + " (" + count.ToString () + ")";
		allItems.Add (id, g);
	}

	public void AddItem(string itemName,MarkerSpawner.MarkerType type, Text text , ParticleSystem PS)
	{
		if (type == MarkerSpawner.MarkerType.herb) {

			if (herbCount >= 5)
				return;

			if (itemName != addedHerb && addedHerb != "") {
				var m = PS.main;
				m.startColor = Utilities.Red;
				var col = text.GetComponent<Button> ().colors;
				col.pressedColor = Utilities.Red;
				text.GetComponent<Button> ().colors = col;
				clearWarning.SetActive (true);
				return;
			} 
			else {
				clearWarning.SetActive (false);
				var m = PS.main;
				m.startColor = Utilities.Blue;
				var col = text.GetComponent<Button> ().colors;
				col.pressedColor = Utilities.Blue;
				text.GetComponent<Button> ().colors = col;
			}

			clearObject.SetActive (true);


			if (PlayerDataManager.playerData.inventory.herbsDict.ContainsKey (itemName)) {
				PlayerDataManager.playerData.inventory.herbsDict [itemName].count--;
				herbCount++;
				if ( PlayerDataManager.playerData.inventory.herbsDict [itemName].count == 0) {
					deletedHerb = PlayerDataManager.playerData.inventory.herbsDict [itemName];
					PlayerDataManager.playerData.inventory.herbsDict.Remove (itemName);
					allItems.Remove (itemName);
					Destroy (text.gameObject);
				}
			} 

			if (herbCount > 0) {
				herbCountObject.SetActive (true);
				clearObject.SetActive (true);
			}
			selectedItem.SetActive (true);
			selectedItemText.text = itemName;
			addedHerb = itemName;
			if (PlayerDataManager.playerData.inventory.herbsDict.ContainsKey (itemName)) {
				text.text = itemName + " (" + PlayerDataManager.playerData.inventory.herbsDict [itemName].count.ToString () + ")";
			}
		
			herbCountObject.GetComponentInChildren<Text> ().text = herbCount.ToString ();
		}

		if (type == MarkerSpawner.MarkerType.tool) {

			if (toolCount >= 5)
				return;

			if (itemName != addedTool && addedTool != "") {
				var m = PS.main;
				m.startColor = Utilities.Red;
				var col = text.GetComponent<Button> ().colors;
				col.pressedColor = Utilities.Red;
				text.GetComponent<Button> ().colors = col;
				clearWarning.SetActive (true);
				return;
			} else {
				clearWarning.SetActive (false);
				var m = PS.main;
				m.startColor = Utilities.Blue;
				var col = text.GetComponent<Button> ().colors;
				col.pressedColor = Utilities.Blue;
				text.GetComponent<Button> ().colors = col;
			}

			clearObject.SetActive (true);

			if (PlayerDataManager.playerData.inventory.toolsDict.ContainsKey (itemName)) {
				PlayerDataManager.playerData.inventory.toolsDict [itemName].count--;
				toolCount++;
				if ( PlayerDataManager.playerData.inventory.toolsDict [itemName].count == 0) {
					deletedTool = PlayerDataManager.playerData.inventory.toolsDict [itemName];
					PlayerDataManager.playerData.inventory.toolsDict.Remove (itemName);
					allItems.Remove (itemName);
					Destroy (text.gameObject);
				}
			} 

			if (toolCount > 0) {
				toolCountObject.SetActive (true);
				clearObject.SetActive (true);
			}
			selectedItem.SetActive (true);
			selectedItemText.text = itemName;
			addedTool = itemName;
			if (PlayerDataManager.playerData.inventory.toolsDict.ContainsKey (itemName)) {
				text.text = itemName + " (" + PlayerDataManager.playerData.inventory.toolsDict [itemName].count.ToString () + ")";
			}

			toolCountObject.GetComponentInChildren<Text> ().text = toolCount.ToString ();
		}

		if (type == MarkerSpawner.MarkerType.gem) {

			if (gemCount >= 5)
				return;

			if (itemName != addedGem && addedGem != "") {
				var m = PS.main;
				m.startColor = Utilities.Red;
				var col = text.GetComponent<Button> ().colors;
				col.pressedColor = Utilities.Red;
				text.GetComponent<Button> ().colors = col;
				clearWarning.SetActive (true);
				return;
			} else {
				clearWarning.SetActive (false);
				var m = PS.main;
				m.startColor = Utilities.Blue;
				var col = text.GetComponent<Button> ().colors;
				col.pressedColor = Utilities.Blue;
				text.GetComponent<Button> ().colors = col;
			}

			clearObject.SetActive (true);

			if (PlayerDataManager.playerData.inventory.gemsDict.ContainsKey (itemName)) {
				PlayerDataManager.playerData.inventory.gemsDict [itemName].count--;
				gemCount++;
				if ( PlayerDataManager.playerData.inventory.gemsDict [itemName].count == 0) {
					deletedGem = PlayerDataManager.playerData.inventory.gemsDict [itemName];
					PlayerDataManager.playerData.inventory.gemsDict.Remove (itemName);
					allItems.Remove (itemName);
					Destroy (text.gameObject);
				}
			} 

			if (gemCount > 0) {
				gemCountObject.SetActive (true);
				clearObject.SetActive (true);
			}
			selectedItem.SetActive (true);
			selectedItemText.text = itemName;
			addedGem = itemName;
			if (PlayerDataManager.playerData.inventory.gemsDict.ContainsKey (itemName)) {
				text.text = itemName + " (" + PlayerDataManager.playerData.inventory.gemsDict [itemName].count.ToString () + ")";
			}
		
			gemCountObject.GetComponentInChildren<Text> ().text = gemCount.ToString ();
		}
	}

	public void clear(bool isUser)
	{
		selectedItem.SetActive (false);
		clearWarning.SetActive (false);

		if (currentType == MarkerSpawner.MarkerType.herb) {
			if (PlayerDataManager.playerData.inventory.herbsDict.ContainsKey (addedHerb)) {
				PlayerDataManager.playerData.inventory.herbsDict [addedHerb].count += herbCount;
				allItems [addedHerb].GetComponent<Text> ().text = addedHerb + " (" + PlayerDataManager.playerData.inventory.herbsDict [addedHerb].count.ToString () + ")";
			} else {
				PlayerDataManager.playerData.inventory.herbsDict.Add (addedHerb, deletedHerb);
				deletedHerb.count = herbCount;
				CreateItem (addedHerb, deletedHerb.count,MarkerSpawner.MarkerType.herb);
			}
			if (isUser) {
				clearObject.SetActive (false);
			}
			herbCountObject.SetActive (false);
			herbCount = 0;
			addedHerb = "";
		}

		if (currentType == MarkerSpawner.MarkerType.tool) {
			if (PlayerDataManager.playerData.inventory.toolsDict.ContainsKey (addedTool)) {
				PlayerDataManager.playerData.inventory.toolsDict [addedTool].count += toolCount;
				allItems [addedTool].GetComponent<Text> ().text = addedTool + " (" + PlayerDataManager.playerData.inventory.toolsDict [addedTool].count.ToString () + ")";
			} else {
				PlayerDataManager.playerData.inventory.toolsDict.Add (addedTool, deletedTool);
				deletedTool.count = toolCount;
				CreateItem (addedTool, deletedTool.count,MarkerSpawner.MarkerType.tool);
			}
			if (isUser) {
				clearObject.SetActive (false);
			}
			toolCountObject.SetActive (false);
			toolCount = 0;
			addedTool = "";
		}

		if (currentType == MarkerSpawner.MarkerType.gem) {
			if (PlayerDataManager.playerData.inventory.gemsDict.ContainsKey (addedGem)) {
				PlayerDataManager.playerData.inventory.gemsDict [addedGem].count += gemCount;
				allItems [addedGem].GetComponent<Text> ().text = addedGem + " (" + PlayerDataManager.playerData.inventory.gemsDict [addedGem].count.ToString () + ")";
			} else {
				PlayerDataManager.playerData.inventory.gemsDict.Add (addedGem, deletedGem);
				deletedGem.count = gemCount;
				CreateItem (addedGem, deletedGem.count,MarkerSpawner.MarkerType.gem);
			}
			if (isUser) {
				clearObject.SetActive (false);
			}
			gemCountObject.SetActive (false);
			gemCount = 0;
			addedGem = "";
		}
	}

	public void close()
	{
		selectedItem.SetActive (false);
		clearWarning.SetActive (false);

		if (herbCount == 0)
			herbPS.SetActive (true);
		else
			herbPS.SetActive (false);

		if (gemCount == 0)
			gemPS.SetActive (true);
		else
			gemPS.SetActive (false);

		if (toolCount == 0)
			toolPS.SetActive (true);
		else
			toolPS.SetActive (false);

		foreach (var item in allItems) {
			Destroy (item.Value);
		}
		allItems.Clear ();
		ingredientsContainer.SetActive (false);
	}
}
