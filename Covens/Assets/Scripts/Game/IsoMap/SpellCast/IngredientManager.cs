using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;

public class IngredientManager : MonoBehaviour
{

    public static IngredientManager Instance { get; set; }

    public static string addedHerb = "";
    public static string addedGem = "";
    public static string addedTool = "";
    public static int addedHerbCount = 0;
    public static int addedGemCount = 0;
    public static int addedToolCount = 0;

    public GameObject ingredientObject;
    public GameObject ingredientPickerObject;
    public GameObject gemObject;
    public GameObject otherIngScroll;
    public Text itemPickerTitle;
    public Text itemPickerInfo;
    public GameObject chooseIng;
    public GameObject actionObject;

    public Text castSpellText;

    public Text selectedToolText;
    public Text selectedHerbText;
    public Text selectedGemText;

    public Text selectedToolCountText;
    public Text selectedHerbCountText;
    public Text selectedGemCountText;

    public List<Text> gemsTitle = new List<Text>();

    //add buttons tool gem herb
    public GameObject[] addButtons;

    public GameObject herbFX;
    public GameObject toolFX;
    public GameObject gemFX;


    public GameObject herbLocked;
    public GameObject toolLocked;
    public GameObject gemLocked;

    public Transform container;
    public GameObject text;

    public GameObject continuePicker;

    IngredientType selectedType;
    Text currentSelectedText = null;
    bool CanSelect = false;
    bool isDrag = false;
    int lastIndex = 0;
    bool isOpen;
    public Button gemButton;
    public Button herbButton;
    public Button toolButton;

    public GameObject chooseForMeText;

    static SpellData curIngData;

    void Awake()
    {
        Instance = this;
    }

    public void Open()
    {
        SoundManagerOneShot.Instance.MenuSound();
        ingredientPickerObject.SetActive(false);
        Show(ingredientObject);
        SetupButtonsInit();
        SetChooseText();
    }

    public bool AddBaseIngredients()
    {
        ResetIngredientsOnPageChange();

        curIngData = SpellManager.CurrentSpell;

        if (curIngData.baseSpell == curIngData.id)
        {
            chooseForMeText.SetActive(true);
            return true;
        }
        else
        {
            chooseForMeText.SetActive(false);
        }

        var pData = PlayerDataManager.playerData.ingredients;
        string missing = "Missing : ";
        if (curIngData.gem != "")
        {
            if (pData.gemsDict.ContainsKey(curIngData.gem) && pData.gemsDict[curIngData.gem].count > 0)
            {
                addedGem = curIngData.gem;
                addedGemCount = 1;
                pData.gemsDict[curIngData.gem].count--;
                if (pData.gemsDict[curIngData.gem].count < 1)
                {
                    pData.gemsDict.Remove(curIngData.gem);
                }
            }
            else
            {
                missing += DownloadedAssets.ingredientDictData[curIngData.gem].name + ".";
                SpellManager.Instance.spellDesc.text = missing;
                return false;
            }
        }
        else
        {
            addedGem = "";
            addedGemCount = 0;
        }

        if (curIngData.herb != "")
        {
            if (pData.herbsDict.ContainsKey(curIngData.herb) && pData.herbsDict[curIngData.herb].count > 0)
            {
                addedHerb = curIngData.herb;
                addedHerbCount = 1;
                pData.herbsDict[curIngData.herb].count--;
                if (pData.herbsDict[curIngData.herb].count < 1)
                {
                    pData.herbsDict.Remove(curIngData.herb);
                }
            }
            else
            {
                missing += DownloadedAssets.ingredientDictData[curIngData.herb].name + ".";
                SpellManager.Instance.spellDesc.text = missing;
                return false;
            }
        }
        else
        {
            addedHerb = "";
            addedHerbCount = 0;
        }

        if (curIngData.tool != "")
        {
            if (pData.toolsDict.ContainsKey(curIngData.tool) && pData.toolsDict[curIngData.tool].count > 0)
            {
                addedTool = curIngData.tool;
                addedToolCount = 1;
                pData.toolsDict[curIngData.tool].count--;
                if (pData.toolsDict[curIngData.tool].count < 1)
                {
                    pData.toolsDict.Remove(curIngData.tool);
                }
            }
            else
            {
                missing += DownloadedAssets.ingredientDictData[curIngData.tool].name + ".";
                SpellManager.Instance.spellDesc.text = missing;
                return false;
            }
        }
        else
        {
            addedTool = "";
            addedToolCount = 0;
        }


        return true;
    }

    void SetupButtonsInit()
    {
        herbButton.onClick.RemoveAllListeners();
        gemButton.onClick.RemoveAllListeners();
        toolButton.onClick.RemoveAllListeners();
        var pData = PlayerDataManager.playerData.ingredients;
        //_____________________________________________________________________________________________________________
        if (curIngData.tool == "")
        {
            selectedToolText.transform.parent.gameObject.SetActive(false);
            selectedToolCountText.text = "";

            toolLocked.SetActive(false);
            addButtons[0].SetActive(true);
            toolButton.onClick.AddListener(delegate
            {
                AddIngredients(IngredientType.tool);
            });
        }
        else
        {

            selectedToolText.transform.parent.gameObject.SetActive(true);
            selectedToolText.text = DownloadedAssets.ingredientDictData[curIngData.tool].name;
            selectedToolCountText.text = "1";

            toolLocked.SetActive(true);
            addButtons[0].SetActive(false);
            toolButton.onClick.AddListener(delegate
            {
                IncreaseCount(IngredientType.tool);
            });
        }
        //_____________________________________________________________________________________________________________
        if (curIngData.gem == "")
        {
            selectedGemText.transform.parent.gameObject.SetActive(false);
            selectedGemCountText.text = "";
            gemLocked.SetActive(false);
            addButtons[1].SetActive(true);
            gemButton.onClick.AddListener(delegate
            {
                AddIngredients(IngredientType.gem);
            });
        }
        else
        {

            selectedGemText.transform.parent.gameObject.SetActive(true);
            selectedGemText.text = DownloadedAssets.ingredientDictData[curIngData.gem].name;
            selectedGemCountText.text = "1";
            gemLocked.SetActive(true);
            addButtons[1].SetActive(false);
            gemButton.onClick.AddListener(delegate
            {
                IncreaseCount(IngredientType.gem);
            });
        }
        //_____________________________________________________________________________________________________________
        if (curIngData.herb == "")
        {
            selectedHerbText.transform.parent.gameObject.SetActive(false);
            selectedHerbCountText.text = "";
            herbLocked.SetActive(false);
            addButtons[2].SetActive(true);
            herbButton.onClick.AddListener(delegate
            {
                AddIngredients(IngredientType.herb);
            });
        }
        else
        {
            selectedHerbText.transform.parent.gameObject.SetActive(true);
            selectedHerbText.text = DownloadedAssets.ingredientDictData[curIngData.herb].name;
            selectedHerbCountText.text = "1";
            herbLocked.SetActive(true);
            addButtons[2].SetActive(false);
            herbButton.onClick.AddListener(delegate
            {
                IncreaseCount(IngredientType.herb);
            });
        }
        //_____________________________________________________________________________________________________________

        if (curIngData.tool != "" && curIngData.gem != "" && curIngData.herb != "")
        {
            chooseIng.SetActive(false);
            actionObject.SetActive(true);
        }
        else
        {
            chooseIng.SetActive(true);
            actionObject.SetActive(false);
        }

        herbFX.SetActive(false);
        toolFX.SetActive(false);
        gemFX.SetActive(false);
    }

    public void IncreaseCount(IngredientType type)
    {
        var pData = PlayerDataManager.playerData.ingredients;
        if (type == IngredientType.gem)
        {
            if (pData.gemsDict.ContainsKey(addedGem))
            {
                if (addedGemCount < 5 && pData.gemsDict[addedGem].count > 0)
                {
                    addedGemCount++;
                    pData.gemsDict[addedGem].count--;
                    selectedGemCountText.text = addedGemCount.ToString();
                    SoundManagerOneShot.Instance.PlayItemAdded();
                }
                else
                {
                    SoundManagerOneShot.Instance.PlayError();
                }
            }
            else
            {
                SoundManagerOneShot.Instance.PlayError();
            }
        }
        else if (type == IngredientType.herb)
        {
            if (pData.herbsDict.ContainsKey(addedHerb))
            {
                if (addedHerbCount < 5 && pData.herbsDict[addedHerb].count > 0)
                {
                    addedHerbCount++;
                    pData.herbsDict[addedHerb].count--;
                    selectedHerbCountText.text = addedHerbCount.ToString();
                    SoundManagerOneShot.Instance.PlayItemAdded();
                }
                else
                {
                    SoundManagerOneShot.Instance.PlayError();
                }
            }
            else
            {
                SoundManagerOneShot.Instance.PlayError();
            }
        }
        else
        {
            if (pData.toolsDict.ContainsKey(addedTool))
            {
                if (addedToolCount < 5 && pData.toolsDict[addedTool].count > 0)
                {
                    addedToolCount++;
                    pData.toolsDict[addedTool].count--;
                    selectedToolCountText.text = addedToolCount.ToString();
                    SoundManagerOneShot.Instance.PlayItemAdded();
                }
                else
                {
                    SoundManagerOneShot.Instance.PlayError();
                }
            }
            else
            {
                SoundManagerOneShot.Instance.PlayError();
            }
        }
        SetChooseText();
    }

    public void AddIngredients(IngredientType type)
    {
        ShowPicker(type);
    }

    void ShowPicker(IngredientType type)
    {
        SoundManagerOneShot.Instance.MenuSound();
        currentSelectedText = null;
        selectedType = type;
        if (type == IngredientType.gem)
        {
            itemPickerTitle.text = "Gems";

        }
        else if (type == IngredientType.herb)
        {
            itemPickerTitle.text = "Botanicals";
        }
        else
        {
            itemPickerTitle.text = "Tools";
        }
        SetPage();
        Show(ingredientPickerObject);
        isOpen = true;
    }

    public void closePicker()
    {
        SoundManagerOneShot.Instance.MenuSound();
        var pData = PlayerDataManager.playerData.ingredients;

        if (selectedType == IngredientType.herb)
        {
            if (addedHerb != "")
            {
                pData.herbsDict[addedHerb].count += addedHerbCount;
            }
            addedHerb = "";
            addedHerbCount = 0;
        }
        else if (selectedType == IngredientType.tool)
        {
            if (addedTool != "")
            {
                pData.toolsDict[addedTool].count += addedToolCount;
            }
            addedTool = "";
            addedToolCount = 0;
        }
        else
        {
            if (addedGem != "")
            {
                pData.gemsDict[addedGem].count += addedGemCount;
            }
            addedGem = "";
            addedGemCount = 0;
        }
        SetIngredientsButtons();
        Hide(ingredientPickerObject);
        isOpen = false;

    }

    public void SetIngredientsButtons()
    {
        if (selectedType == IngredientType.herb)
        {
            if (addedHerb != "")
            {
                selectedHerbText.text = DownloadedAssets.ingredientDictData[addedHerb].name;
                selectedHerbCountText.text = addedHerbCount.ToString();
                selectedHerbText.transform.parent.gameObject.SetActive(true);
                if (!herbFX.activeInHierarchy)
                {
                    herbFX.SetActive(true);
                }
            }
            else
            {
                selectedHerbCountText.text = "";
                selectedHerbText.transform.parent.gameObject.SetActive(false);
                herbFX.SetActive(false);
            }
        }
        if (selectedType == IngredientType.tool)
        {
            if (addedTool != "")
            {
                selectedToolText.text = DownloadedAssets.ingredientDictData[addedTool].name;
                selectedToolCountText.text = addedToolCount.ToString();
                selectedToolText.transform.parent.gameObject.SetActive(true);
                if (!toolFX.activeInHierarchy)
                {
                    toolFX.SetActive(true);
                }
            }
            else
            {
                selectedToolCountText.text = "";
                selectedToolText.transform.parent.gameObject.SetActive(false);
                toolFX.SetActive(false);
            }
        }
        if (selectedType == IngredientType.gem)
        {
            if (addedGem != "")
            {
                selectedGemText.text = DownloadedAssets.ingredientDictData[addedGem].name;
                selectedGemCountText.text = addedGemCount.ToString();
                selectedGemText.transform.parent.gameObject.SetActive(true);
                if (!gemFX.activeInHierarchy)
                {
                    gemFX.SetActive(true);
                }
            }
            else
            {
                selectedGemCountText.text = "";
                selectedGemText.transform.parent.gameObject.SetActive(false);
                gemFX.SetActive(false);
            }
        }
        SetChooseText();
    }

    void SetChooseText()
    {
        if (addedHerb == addedGem && addedGem == addedTool)
        {
            chooseIng.SetActive(true);
            actionObject.SetActive(false);
        }
        else
        {
            Hide(chooseIng);
            Show(actionObject);
        }
        castSpellText.text = "Cast " + SpellManager.CurrentSpell.displayName;
    }

    public void ContinuePicker()
    {
        SoundManagerOneShot.Instance.MenuSound();
        SetIngredientsButtons();
        Hide(ingredientPickerObject);
        isOpen = false;
    }

    void SetPage()
    {
        var pData = PlayerDataManager.playerData.ingredients;
        foreach (Transform item in container)
        {
            Destroy(item.gameObject);
        }

        if (selectedType != IngredientType.gem)
        {
            gemObject.SetActive(false);
            otherIngScroll.SetActive(true);
        }
        else
        {
            gemObject.SetActive(true);
            otherIngScroll.SetActive(false);
        }

        if (selectedType == IngredientType.herb)
        {
            if (addedHerb != "")
            {
                continuePicker.SetActive(true);
            }
            else
                continuePicker.SetActive(false);
            var temp = pData.herbsDict.Values.ToList();
            temp = temp.OrderBy(i => i.name).ToList();
            foreach (var item in temp)
            {
                var g = Utilities.InstantiateObject(text, container);
                g.name = item.id;
                if (item.id == addedHerb)
                {
                    g.GetComponent<Text>().color = Utilities.Blue;
                    currentSelectedText = g.GetComponent<Text>();
                }
                g.GetComponent<Text>().text = item.name + " (" + item.count.ToString() + ")";
            }
            if (pData.herbsDict.Count > 20)
            {
                for (int i = 0; i < 4; i++)
                {
                    var g = Utilities.InstantiateObject(text, container);
                    g.name = "null";
                    g.GetComponent<Text>().text = "";
                }
            }

            if (addedHerb == "")
            {
                if (pData.herbsDict.Count > 0)
                    itemPickerInfo.text = "Tap an ingredient to add";
                else
                    itemPickerInfo.text = "You do not have any herbs.";

            }
            else
            {
                itemPickerInfo.text = addedHerbCount.ToString() + " " + DownloadedAssets.ingredientDictData[addedHerb].name;
            }

        }
        else if (selectedType == IngredientType.tool)
        {
            if (addedTool != "")
            {
                continuePicker.SetActive(true);
            }
            else
                continuePicker.SetActive(false);
            var temp = pData.toolsDict.Values.ToList();
            temp = temp.OrderBy(i => i.name).ToList();
            foreach (var item in temp)
            {
                var g = Utilities.InstantiateObject(text, container);
                g.name = item.id;
                if (item.id == addedTool)
                {
                    g.GetComponent<Text>().color = Utilities.Blue;
                    currentSelectedText = g.GetComponent<Text>();
                }
                g.GetComponent<Text>().text = item.name + " (" + item.count.ToString() + ")";

            }
            if (pData.toolsDict.Count > 20)
            {
                for (int i = 0; i < 4; i++)
                {
                    var g = Utilities.InstantiateObject(text, container);
                    g.name = "null";
                    g.GetComponent<Text>().text = "";
                }
            }
            if (addedTool == "")
            {
                if (pData.toolsDict.Count > 0)
                    itemPickerInfo.text = "Tap an ingredient to add";
                else
                    itemPickerInfo.text = "You do not have any tools.";

            }
            else
            {
                itemPickerInfo.text = addedToolCount.ToString() + " " + DownloadedAssets.ingredientDictData[addedTool].name;
            }
        }
        else
        {
            if (addedGem != "")
            {
                continuePicker.SetActive(true);
                itemPickerInfo.text = addedGemCount.ToString() + " " + DownloadedAssets.ingredientDictData[addedGem].name;
            }
            else
            {
                continuePicker.SetActive(false);
                if (pData.gemsDict.Count > 0)
                    itemPickerInfo.text = "Tap an ingredient to add";
                else
                    itemPickerInfo.text = "You do not have any gems.";

            }

            foreach (var item in gemsTitle)
            {
                item.text = DownloadedAssets.ingredientDictData[item.gameObject.name].name;
                item.color = Color.gray;
            }
            foreach (var item in pData.gemsDict)
            {
                if (item.Key == "coll_bloodAgate")
                {
                    gemsTitle[0].text += "(" + item.Value.count + ")";
                    gemsTitle[0].color = (addedGem == item.Key ? Utilities.Blue : Color.white);
                }
                else if (item.Key == "coll_malachite")
                {
                    gemsTitle[1].text += "(" + item.Value.count + ")";
                    gemsTitle[1].color = (addedGem == item.Key ? Utilities.Blue : Color.white);

                }
                else if (item.Key == "coll_brimstone")
                {
                    gemsTitle[2].text += "(" + item.Value.count + ")";
                    gemsTitle[2].color = (addedGem == item.Key ? Utilities.Blue : Color.white);

                }
                else if (item.Key == "coll_motherTear")
                {
                    gemsTitle[3].text += "(" + item.Value.count + ")";
                    gemsTitle[3].color = (addedGem == item.Key ? Utilities.Blue : Color.white);

                }
                else
                {
                    gemsTitle[4].text += "(" + item.Value.count + ")";
                    gemsTitle[4].color = (addedGem == item.Key ? Utilities.Blue : Color.white);
                }
            }
        }

    }

    void Update()
    {
        if (!isOpen)
            return;
        if (Input.GetMouseButtonUp(0) && !isDrag && !CanSelect)
        {
            CanSelect = true;
            return;
        }
        if (CanSelect && Input.GetMouseButtonUp(0))
        {
            PointerEventData ped = new PointerEventData(null);
            ped.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(ped, results);
            foreach (var item in results)
            {

                if (item.gameObject.name != "null" && item.gameObject.tag == "ingredient")
                {
                    OnTapItem(item.gameObject.name, item.gameObject.GetComponent<Text>());
                }
            }
        }
    }

    void OnTapItem(string item, Text text)
    {
        if (selectedType == IngredientType.herb)
        {
            if (currentSelectedText != null)
            {
                if (item != addedHerb)
                {
                    PlayerDataManager.playerData.ingredients.herbsDict[addedHerb].count += addedHerbCount;
                    currentSelectedText.text = PlayerDataManager.playerData.ingredients.herbsDict[addedHerb].name + " (" + PlayerDataManager.playerData.ingredients.herbsDict[item].count.ToString() + ")";
                    currentSelectedText.color = Color.white;
                    Debug.Log(addedHerb);
                    addedHerb = item;
                    addedHerbCount = 1;
                    PlayerDataManager.playerData.ingredients.herbsDict[addedHerb].count--;
                    text.text = PlayerDataManager.playerData.ingredients.herbsDict[addedHerb].name + " (" + PlayerDataManager.playerData.ingredients.herbsDict[addedHerb].count.ToString() + ")";
                    text.color = Utilities.Blue;
                    currentSelectedText = text;
                    SoundManagerOneShot.Instance.PlayItemAdded();
                }
                else
                {
                    if (addedHerbCount >= 5)
                    {
                        SoundManagerOneShot.Instance.PlayError();
                        return;

                    }
                    SoundManagerOneShot.Instance.PlayItemAdded();

                    if (PlayerDataManager.playerData.ingredients.herbsDict[addedHerb].count > 0)
                    {
                        addedHerb = item;
                        addedHerbCount++;
                        PlayerDataManager.playerData.ingredients.herbsDict[addedHerb].count--;
                        text.text = PlayerDataManager.playerData.ingredients.herbsDict[addedHerb].name + " (" + PlayerDataManager.playerData.ingredients.herbsDict[addedHerb].count.ToString() + ")";
                    }
                }
            }
            else
            {
                SoundManagerOneShot.Instance.PlayItemAdded();
                addedHerb = item;
                addedHerbCount = 1;
                PlayerDataManager.playerData.ingredients.herbsDict[addedHerb].count--;
                text.text = PlayerDataManager.playerData.ingredients.herbsDict[addedHerb].name + " (" + PlayerDataManager.playerData.ingredients.herbsDict[addedHerb].count.ToString() + ")";
                text.color = Utilities.Blue;
                currentSelectedText = text;
            }

            itemPickerInfo.text = addedHerbCount.ToString() + " " + DownloadedAssets.ingredientDictData[addedHerb].name;
            continuePicker.SetActive(true);
        }
        else if (selectedType == IngredientType.tool)
        {
            if (currentSelectedText != null)
            {
                if (item != addedTool)
                {
                    PlayerDataManager.playerData.ingredients.toolsDict[addedTool].count += addedToolCount;
                    currentSelectedText.text = PlayerDataManager.playerData.ingredients.toolsDict[addedTool].name + " (" + PlayerDataManager.playerData.ingredients.toolsDict[item].count.ToString() + ")";
                    currentSelectedText.color = Color.white;
                    addedTool = item;
                    addedToolCount = 1;
                    PlayerDataManager.playerData.ingredients.toolsDict[addedTool].count--;

                    text.text = PlayerDataManager.playerData.ingredients.toolsDict[addedTool].name + " (" + PlayerDataManager.playerData.ingredients.toolsDict[addedTool].count.ToString() + ")";
                    text.color = Utilities.Blue;
                    currentSelectedText = text;
                    SoundManagerOneShot.Instance.PlayItemAdded();
                }
                else
                {
                    if (addedToolCount >= 5)
                    {
                        SoundManagerOneShot.Instance.PlayError();
                        return;
                    }
                    SoundManagerOneShot.Instance.PlayItemAdded();
                    if (PlayerDataManager.playerData.ingredients.toolsDict[addedTool].count > 0)
                    {
                        addedTool = item;
                        addedToolCount++;
                        PlayerDataManager.playerData.ingredients.toolsDict[addedTool].count--;
                        text.text = PlayerDataManager.playerData.ingredients.toolsDict[addedTool].name + " (" + PlayerDataManager.playerData.ingredients.toolsDict[addedTool].count.ToString() + ")";
                    }
                }
            }
            else
            {
                SoundManagerOneShot.Instance.PlayItemAdded();
                addedTool = item;
                addedToolCount = 1;
                PlayerDataManager.playerData.ingredients.toolsDict[addedTool].count--;
                text.text = PlayerDataManager.playerData.ingredients.toolsDict[addedTool].name + " (" + PlayerDataManager.playerData.ingredients.toolsDict[addedTool].count.ToString() + ")";
                text.color = Utilities.Blue;
                currentSelectedText = text;
            }
            itemPickerInfo.text = addedToolCount.ToString() + " " + DownloadedAssets.ingredientDictData[addedTool].name;
            continuePicker.SetActive(true);
        }
        else
        {
            if (PlayerDataManager.playerData.ingredients.gemsDict.ContainsKey(item))
            {
                if (PlayerDataManager.playerData.ingredients.gemsDict[item].count < 1)
                {
                    PlayerDataManager.playerData.ingredients.gemsDict.Remove(item);
                    SoundManagerOneShot.Instance.PlayError();
                    return;
                }
            }
            else
            {
                SoundManagerOneShot.Instance.PlayError();
                return;
            }
            if (currentSelectedText != null)
            {
                if (item != addedGem)
                {
                    PlayerDataManager.playerData.ingredients.gemsDict[addedGem].count += addedGemCount;
                    currentSelectedText.text = PlayerDataManager.playerData.ingredients.gemsDict[addedGem].name + " (" + PlayerDataManager.playerData.ingredients.gemsDict[addedGem].count.ToString() + ")";
                    currentSelectedText.color = Color.white;
                    addedGem = item;
                    addedGemCount = 1;
                    PlayerDataManager.playerData.ingredients.gemsDict[addedGem].count--;

                    text.text = PlayerDataManager.playerData.ingredients.gemsDict[addedGem].name + " (" + PlayerDataManager.playerData.ingredients.gemsDict[addedGem].count.ToString() + ")";
                    text.color = Utilities.Blue;
                    currentSelectedText = text;
                    SoundManagerOneShot.Instance.PlayItemAdded();
                }
                else
                {
                    if (addedGemCount >= 5)
                    {
                        SoundManagerOneShot.Instance.PlayError();
                        return;
                    }
                    SoundManagerOneShot.Instance.PlayItemAdded();
                    if (PlayerDataManager.playerData.ingredients.gemsDict[addedGem].count > 0)
                    {
                        addedGem = item;
                        addedGemCount++;
                        PlayerDataManager.playerData.ingredients.gemsDict[addedGem].count--;
                        text.text = PlayerDataManager.playerData.ingredients.gemsDict[addedGem].name + " (" + PlayerDataManager.playerData.ingredients.gemsDict[addedGem].count.ToString() + ")";
                    }
                }
            }
            else
            {
                SoundManagerOneShot.Instance.PlayItemAdded();
                addedGem = item;
                addedGemCount = 1;
                PlayerDataManager.playerData.ingredients.gemsDict[addedGem].count--;
                text.text = PlayerDataManager.playerData.ingredients.gemsDict[addedGem].name + " (" + PlayerDataManager.playerData.ingredients.gemsDict[addedGem].count.ToString() + ")";
                text.color = Utilities.Blue;
                currentSelectedText = text;
            }
            itemPickerInfo.text = addedGemCount.ToString() + " " + DownloadedAssets.ingredientDictData[addedGem].name;
            continuePicker.SetActive(true);
        }
    }

    public void SetDragggin(bool drag)
    {
        isDrag = drag;
        if (isDrag)
            CanSelect = false;
    }

    void returnAllIngredients()
    {
        var pData = PlayerDataManager.playerData.ingredients;

        if (addedGem != "" && addedGem != curIngData.gem)
        {
            pData.gemsDict[addedGem].count += addedGemCount;
            addedGem = "";
            addedGemCount = 0;
        }
        if (addedHerb != "" && addedHerb != curIngData.herb)
        {
            pData.herbsDict[addedHerb].count += addedHerbCount;
            addedHerb = "";
            addedHerbCount = 0;
        }
        if (addedTool != "" && addedTool != curIngData.tool)
        {
            pData.toolsDict[addedTool].count += addedToolCount;
            addedTool = "";
            addedToolCount = 0;
        }

    }

    static void ResetIngredientsOnPageChange()
    {
        var pData = PlayerDataManager.playerData.ingredients;

        if (addedGem != "")
        {
            pData.gemsDict[addedGem].count += addedGemCount;
            addedGem = "";
            addedGemCount = 0;
        }
        if (addedHerb != "")
        {
            pData.herbsDict[addedHerb].count += addedHerbCount;
            addedHerb = "";
            addedHerbCount = 0;
        }
        if (addedTool != "")
        {
            pData.toolsDict[addedTool].count += addedToolCount;
            addedTool = "";
            addedToolCount = 0;
        }

    }

    public static void ClearIngredient()
    {
        var pData = PlayerDataManager.playerData.ingredients;

        if (addedGem != "" && pData.gemsDict.ContainsKey(addedGem))
        {
            Debug.Log(addedGem);
            Debug.Log(addedGemCount);
            //			pData.gemsDict [addedGem].count -= addedGemCount;
            if (pData.gemsDict[addedGem].count < 1)
            {
                pData.gemsDict.Remove(addedGem);
            }

            Debug.Log(pData.gemsDict[addedGem].count);
        }
        if (addedHerb != "" && pData.herbsDict.ContainsKey(addedHerb))
        {
            Debug.Log(addedHerb);
            //			pData.herbsDict [addedHerb].count -= addedHerbCount;
            if (pData.herbsDict[addedHerb].count < 1)
            {
                pData.herbsDict.Remove(addedHerb);
            }
        }
        if (addedTool != "" && pData.toolsDict.ContainsKey(addedTool))
        {
            Debug.Log(addedTool);
            //			pData.toolsDict [addedTool].count -= addedToolCount;
            if (pData.toolsDict[addedTool].count < 1)
            {
                pData.toolsDict.Remove(addedTool);
            }
        }

        addedTool = "";
        addedHerb = "";
        addedGem = "";
        addedGemCount = 0;
        addedHerbCount = 0;
        addedToolCount = 0;
    }

    public void CancelIngredients()
    {
        returnAllIngredients();
        Hide(ingredientObject);
        SoundManagerOneShot.Instance.MenuSound();
    }

    public void ChooseRandomHelper()
    {
        SoundManagerOneShot.Instance.PlayWhisperFX();
        if (curIngData.baseSpell != curIngData.id)
        {
            SoundManagerOneShot.Instance.PlayError();
            chooseForMeText.SetActive(false);
            return;
        }
        SetChooseText();
        StartCoroutine(ChooseRandom());
    }

    IEnumerator ChooseRandom()
    {
        Show(actionObject);
        Hide(chooseIng);
        foreach (var item in addButtons)
        {
            item.SetActive(false);
        }
        #region pickRandom
        var pData = PlayerDataManager.playerData.ingredients;
        if (pData.herbsDict.Count > 0)
        {
            var herbList = pData.herbsDict.Values.ToList();
            addedHerb = herbList[Random.Range(0, herbList.Count)].id;
            if (pData.herbsDict[addedHerb].count >= 3)
            {
                var r = Random.Range(1, 4);
                addedHerbCount = r;
                pData.herbsDict[addedHerb].count -= r;
            }
            else
            {
                addedHerbCount = Random.Range(1, pData.herbsDict[addedHerb].count);
                pData.herbsDict[addedHerb].count -= addedHerbCount;
                if (pData.herbsDict[addedHerb].count <= 0)
                {
                    pData.herbsDict.Remove(addedHerb);
                }
            }
        }
        if (pData.toolsDict.Count > 0)
        {
            var toolList = pData.toolsDict.Values.ToList();
            addedTool = toolList[Random.Range(0, toolList.Count)].id;
            if (pData.toolsDict[addedTool].count >= 3)
            {
                var r = Random.Range(1, 4);
                addedToolCount = r;
                pData.toolsDict[addedTool].count -= r;
            }
            else
            {
                addedToolCount = Random.Range(1, pData.toolsDict[addedTool].count);
                pData.toolsDict[addedTool].count -= addedToolCount;
                if (pData.toolsDict[addedTool].count <= 0)
                {
                    pData.toolsDict.Remove(addedTool);
                }
            }
        }
        if (pData.gemsDict.Count > 0)
        {
            var gemList = pData.gemsDict.Values.ToList();
            addedGem = gemList[Random.Range(0, gemList.Count)].id;
            if (pData.gemsDict[addedGem].count >= 3)
            {
                var r = Random.Range(1, 4);
                addedGemCount = r;
                pData.gemsDict[addedGem].count -= r;
            }
            else
            {
                addedGemCount = Random.Range(1, pData.gemsDict[addedGem].count);
                pData.gemsDict[addedGem].count -= addedGemCount;
                if (pData.gemsDict[addedGem].count <= 0)
                {
                    pData.gemsDict.Remove(addedGem);
                }
            }
        }
        #endregion
        yield return new WaitForSeconds(.55f);
        if (addedGem != "")
        {
            SoundManagerOneShot.Instance.IngredientAdded();
            selectedGemText.text = DownloadedAssets.ingredientDictData[addedGem].name;
            selectedGemCountText.text = addedToolCount.ToString();
            selectedGemText.transform.parent.gameObject.SetActive(true);
            if (!gemFX.activeInHierarchy)
            {
                gemFX.SetActive(true);
            }
        }
        yield return new WaitForSeconds(.55f);
        if (addedHerb != "")
        {
            SoundManagerOneShot.Instance.IngredientAdded();
            selectedHerbText.text = DownloadedAssets.ingredientDictData[addedHerb].name;
            selectedHerbCountText.text = addedHerbCount.ToString();
            selectedHerbText.transform.parent.gameObject.SetActive(true);
            if (!herbFX.activeInHierarchy)
            {
                herbFX.SetActive(true);
            }
        }
        yield return new WaitForSeconds(.55f);
        if (addedTool != "")
        {
            SoundManagerOneShot.Instance.IngredientAdded();
            selectedToolText.text = DownloadedAssets.ingredientDictData[addedTool].name;
            selectedToolCountText.text = addedToolCount.ToString();
            selectedToolText.transform.parent.gameObject.SetActive(true);
            if (!toolFX.activeInHierarchy)
            {
                toolFX.SetActive(true);
            }
        }

    }

    #region Animation

    void Show(GameObject g)
    {
        if (g.activeInHierarchy)
            g.SetActive(false);
        var anim = g.GetComponent<Animator>();
        g.SetActive(true);
        anim.Play("in");
    }

    void Hide(GameObject g, bool isDisable = true)
    {
        if (isDisable)
            StopCoroutine("DisableObject");

        if (g.activeInHierarchy)
        {
            var anim = g.GetComponent<Animator>();
            anim.Play("out");
            if (isDisable)
                StartCoroutine(DisableObject(g));
        }
    }

    IEnumerator DisableObject(GameObject g)
    {
        yield return new WaitForSeconds(.55f);
        g.SetActive(false);
    }
    #endregion

}
