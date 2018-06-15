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
	public GameObject gemCountObject; 
	public GameObject toolCountObject;

	public GameObject gemPS;
	public GameObject herbPS;
	public GameObject toolPS;

	public GameObject gemFractal;
	public GameObject herbFractal;
	public GameObject toolFractal;

	public GameObject clearObject;
	public GameObject selectedItem;
	public Text selectedItemText;
	public Text TitleName;
	public GameObject clearWarning;

	public GameObject listObject;

	public static int gemCount = 0;
	public static int herbCount =0;
	public static int toolCount= 0;

	Dictionary<string,GameObject> allItems = new Dictionary<string,GameObject>();
	public static Dictionary<string,string> ingIDs = new Dictionary<string, string>();
	InventoryItems deletedHerb;
	InventoryItems deletedTool;
	InventoryItems deletedGem;

	public string addedHerb ="";
	public string addedTool="";
	public string addedGem="";


	MarkerSpawner.MarkerType currentType;

	void Awake(){
		Instance = this;
	}

	public void init()
	{
		foreach (var item in allItems) {
			Destroy (item.Value);
		}

	
		listObject.SetActive (true);
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

		if (gemCount == 0 && herbCount == 0 && toolCount == 0) {
			print ("INIT!!");
			IngredientsUI.Instance.SpellInit ();
			IngredientsUI.Instance.HideSkipIng (true);
		} else {
			IngredientsUI.Instance.SpellAdded ();
		}
		
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
		PopulateItem (PlayerDataManager.playerData.ingredients.gemsDict );
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
		PopulateItem (PlayerDataManager.playerData.ingredients.toolsDict );
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
		PopulateItem (PlayerDataManager.playerData.ingredients.herbsDict );
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
		IngredientsUI.Instance.SpellAdded ();
		IngredientsUI.Instance.HideSkipIng (false);

		if (type == MarkerSpawner.MarkerType.herb) {
			print ("herb");
			if (itemName != addedHerb && addedHerb != "") {
				var m = PS.main;
				m.startColor = Utilities.Red;
				var col = text.GetComponent<Button> ().colors;
				col.pressedColor = Utilities.Red;
				text.GetComponent<Button> ().colors = col;
				clearWarning.SetActive (true);
				clearWarning.GetComponent<Text>().text = "Clear the current item before adding a new one.";
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

			if (herbCount >= 5) {
				clearWarning.SetActive (true);
				clearWarning.GetComponent<Text>().text = "You can only use a maximum of 5 items.";
				var m = PS.main;
				m.startColor = Utilities.Red;
				var col = text.GetComponent<Button> ().colors;
				col.pressedColor = Utilities.Red;
				text.GetComponent<Button> ().colors = col;
				return;
			}
		
			clearObject.SetActive (true);

			if (PlayerDataManager.playerData.ingredients.herbsDict.ContainsKey (itemName)) {
				PlayerDataManager.playerData.ingredients.herbsDict [itemName].count--;
				herbCount++;
				if ( PlayerDataManager.playerData.ingredients.herbsDict [itemName].count == 0) {
					deletedHerb = PlayerDataManager.playerData.ingredients.herbsDict [itemName];
					PlayerDataManager.playerData.ingredients.herbsDict.Remove (itemName);
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

			herbFractal.SetActive (true);

			if (PlayerDataManager.playerData.ingredients.herbsDict.ContainsKey (itemName)) {
				text.text = itemName + " (" + PlayerDataManager.playerData.ingredients.herbsDict [itemName].count.ToString () + ")";
			}
		
			herbCountObject.GetComponentInChildren<Text> ().text = herbCount.ToString ();
		}

		if (type == MarkerSpawner.MarkerType.tool) {
			print ("tool");

	

			if (itemName != addedTool && addedTool != "") {
				var m = PS.main;
				m.startColor = Utilities.Red;
				clearWarning.GetComponent<Text>().text = "Clear the current item before adding a new one.";
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


			if (toolCount >= 5) {
				clearWarning.SetActive (true);
				clearWarning.GetComponent<Text>().text = "You can only use a maximum of 5 items.";
				var m = PS.main;
				m.startColor = Utilities.Red;
				var col = text.GetComponent<Button> ().colors;
				col.pressedColor = Utilities.Red;
				text.GetComponent<Button> ().colors = col;
				return;
			}

			clearObject.SetActive (true);

			if (PlayerDataManager.playerData.ingredients.toolsDict.ContainsKey (itemName)) {
				PlayerDataManager.playerData.ingredients.toolsDict [itemName].count--;
				toolCount++;
				if ( PlayerDataManager.playerData.ingredients.toolsDict [itemName].count == 0) {
					deletedTool = PlayerDataManager.playerData.ingredients.toolsDict [itemName];
					PlayerDataManager.playerData.ingredients.toolsDict.Remove (itemName);
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
			if (PlayerDataManager.playerData.ingredients.toolsDict.ContainsKey (itemName)) {
				text.text = itemName + " (" + PlayerDataManager.playerData.ingredients.toolsDict [itemName].count.ToString () + ")";
			}
			toolFractal.SetActive (true);

			toolCountObject.GetComponentInChildren<Text> ().text = toolCount.ToString ();
		}

		if (type == MarkerSpawner.MarkerType.gem) {

		
			if (itemName != addedGem && addedGem != "") {
				var m = PS.main;
				clearWarning.GetComponent<Text>().text = "Clear the current item before adding a new one.";
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


			if (gemCount >= 5) {
				clearWarning.SetActive (true);
				clearWarning.GetComponent<Text>().text = "You can only use a maximum of 5 items.";
				var m = PS.main;
				m.startColor = Utilities.Red;
				var col = text.GetComponent<Button> ().colors;
				col.pressedColor = Utilities.Red;
				text.GetComponent<Button> ().colors = col;
				return;
			}

			clearObject.SetActive (true);

			if (PlayerDataManager.playerData.ingredients.gemsDict.ContainsKey (itemName)) {
				PlayerDataManager.playerData.ingredients.gemsDict [itemName].count--;
				gemCount++;
				if ( PlayerDataManager.playerData.ingredients.gemsDict [itemName].count == 0) {
					deletedGem = PlayerDataManager.playerData.ingredients.gemsDict [itemName];
					PlayerDataManager.playerData.ingredients.gemsDict.Remove (itemName);
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
			if (PlayerDataManager.playerData.ingredients.gemsDict.ContainsKey (itemName)) {
				text.text = itemName + " (" + PlayerDataManager.playerData.ingredients.gemsDict [itemName].count.ToString () + ")";
			}
			gemFractal.SetActive (true);
		
			gemCountObject.GetComponentInChildren<Text> ().text = gemCount.ToString ();
		}
	}

	public void clear(bool isUser)
	{
		selectedItem.SetActive (false);
		clearWarning.SetActive (false);
		print(herbCount + "" + toolCount + "" + gemCount);

		IngredientsUI.Instance.HideSkipIng (true);
	

		if (currentType == MarkerSpawner.MarkerType.herb) {
			if (PlayerDataManager.playerData.ingredients.herbsDict.ContainsKey (addedHerb)) {
				PlayerDataManager.playerData.ingredients.herbsDict [addedHerb].count += herbCount;
				allItems [addedHerb].GetComponent<Text> ().text = addedHerb + " (" + PlayerDataManager.playerData.ingredients.herbsDict [addedHerb].count.ToString () + ")";
			} else {
				PlayerDataManager.playerData.ingredients.herbsDict.Add (addedHerb, deletedHerb);
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
			if (PlayerDataManager.playerData.ingredients.toolsDict.ContainsKey (addedTool)) {
				PlayerDataManager.playerData.ingredients.toolsDict [addedTool].count += toolCount;
				allItems [addedTool].GetComponent<Text> ().text = addedTool + " (" + PlayerDataManager.playerData.ingredients.toolsDict [addedTool].count.ToString () + ")";
			} else {
				PlayerDataManager.playerData.ingredients.toolsDict.Add (addedTool, deletedTool);
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
			if (PlayerDataManager.playerData.ingredients.gemsDict.ContainsKey (addedGem)) {
				PlayerDataManager.playerData.ingredients.gemsDict [addedGem].count += gemCount;
				allItems [addedGem].GetComponent<Text> ().text = addedGem + " (" + PlayerDataManager.playerData.ingredients.gemsDict [addedGem].count.ToString () + ")";
			} else {
				PlayerDataManager.playerData.ingredients.gemsDict.Add (addedGem, deletedGem);
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

		if (gemCount == 0 && herbCount == 0 && toolCount == 0) {
			IngredientsUI.Instance.SpellInit ();
		} else
			IngredientsUI.Instance.SpellAdded ();
		
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
