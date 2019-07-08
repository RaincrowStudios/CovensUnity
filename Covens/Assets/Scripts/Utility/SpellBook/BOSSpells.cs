using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro;
public class BOSSpells : BOSBase
{
    [SerializeField] private GameObject circleNavPrefab;
    [SerializeField] private Transform navTransform;
    [SerializeField] private Sprite navSelected;
    [SerializeField] private Sprite navUnSelected;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI cost;
    [SerializeField] private TextMeshProUGUI descShort;
    [SerializeField] private GameObject crestTransform;



    [SerializeField] private GameObject spellTransform;
    [SerializeField] private CanvasGroup contentCG;
    [SerializeField] private CanvasGroup navCG;
    [SerializeField] private Image crestImage;
    [SerializeField] private Image spellImage;
    [SerializeField] private Button castButton;
    [SerializeField] private Sprite[] crestSprites;
    [SerializeField] private LeanTweenType easeType;
    [SerializeField] private float speed;
    [SerializeField] private float movement = 200;
    [SerializeField] private Color unselectedColor;

    [SerializeField] private TextMeshProUGUI toolText;
    [SerializeField] private TextMeshProUGUI herbText;
    [SerializeField] private TextMeshProUGUI gemText;

    private GameObject contentTransform;
    private bool canSwipe = true;
    private SwipeDetector swipeDetector;
    private string currentSpell = "";
    private Dictionary<string, Transform> navButtons = new Dictionary<string, Transform>();
    private int index = 0;
    private List<SpellData> spellList = new List<SpellData>();

    private void Start()
    {
        contentTransform = contentCG.gameObject;
        contentCG.alpha = 0;
        swipeDetector = gameObject.AddComponent<SwipeDetector>() as SwipeDetector;
        swipeDetector.canSwipe = true;
        swipeDetector.SwipeLeft = OnSwipeLeft;
        swipeDetector.SwipeRight = OnSwipeRight;
        swipeDetector.detectSwipeOnlyAfterRelease = true;

        LeanTween.alphaCanvas(navCG, 1, .7f);
        CreateSpellNavigation();
    }

    private void ClearNavigation()
    {
        spellList.Clear();
        // signatureList.Clear();
        navButtons.Clear();
        foreach (Transform item in navTransform)
        {
            Destroy(item.gameObject);
        }
        currentSpell = "";
    }

    private void CreateSpellNavigation()
    {


        ClearNavigation();
        spellList = PlayerDataManager.playerData.Spells;
        foreach (var item in spellList)
        {
            var navButton = Utilities.InstantiateObject(circleNavPrefab, navTransform, 0);
            navButton.GetComponentInChildren<Button>().onClick.AddListener(() => { ShowSpell(item.id); });
            DownloadedAssets.GetSprite(item.id, navButton.transform.GetChild(1).GetComponent<Image>());
            navButtons[item.id] = navButton.transform;
            LeanTween.scale(navButton, Vector3.one, .8f).setEase(LeanTweenType.easeInOutSine);
        }
        ShowSpell(spellList[0].id);

    }

    private void ShowSpell(string id, bool isButton = true, int side = 0)
    {

        var curList = spellList;


        if (isButton)
        {
            for (int i = 0; i < curList.Count; i++)
            {
                if (curList[i].id == id)
                    index = i;
            }
        }


        if (currentSpell != "")
        {
            navButtons[currentSpell].GetChild(0).GetComponent<Image>().sprite = navUnSelected;
            navButtons[currentSpell].GetChild(0).GetComponent<Image>().color = unselectedColor;
            navButtons[currentSpell].GetChild(1).GetComponent<Image>().color = Color.black;
        }
        currentSpell = id;
        navButtons[currentSpell].GetChild(0).GetComponent<Image>().sprite = navSelected;
        navButtons[currentSpell].GetChild(0).GetComponent<Image>().color = Color.white;
        navButtons[currentSpell].GetChild(1).GetComponent<Image>().color = Color.white;

        SetSpellUI(id, side);
    }

    private void SetSpellUI(string id, int side)
    {

        canSwipe = false;
        LeanTween.scale(crestTransform, Vector2.one * .8f, speed * .5f).setEase(easeType);
        LeanTween.scale(spellTransform, Vector2.one * 1.2f, speed * .5f).setEase(easeType);
        if (side < 0)
            LeanTween.moveLocalX(contentTransform, -movement, speed * .5f).setEase(easeType);
        else if (side > 0)
            LeanTween.moveLocalX(contentTransform, movement, speed * .5f).setEase(easeType);

        var lID = LeanTween.alphaCanvas(contentCG, 0, speed * .5f).id;
        LeanTween.descr(lID).setOnComplete(() => { SetSpellPost(id, side); }).setEase(easeType);
    }

    private void SetSpellPost(string id, int side)
    {
        if (side < 0)
            contentTransform.transform.Translate(movement, 0, 0);
        else if (side > 0)
            contentTransform.transform.Translate(-movement, 0, 0);

        if (id == "spell_trueSight" || id == "spell_invisibility")
            castButton.gameObject.SetActive(true);
        else
            castButton.gameObject.SetActive(false);

        var curSpell = DownloadedAssets.GetSpell(id);
        cost.text = LocalizeLookUp.GetText("spell_cost") + ": <b>" + curSpell.cost.ToString();
        title.text = curSpell.Name;

        descShort.text = PlayerManager.inSpiritForm ? curSpell.SpiritDescription : curSpell.PhysicalDescription;

        toolText.text = gemText.text = herbText.text = LocalizeLookUp.GetText("lt_none");
        foreach (string ingredientId in curSpell.ingredients)
        {
            IngredientData ingredient = DownloadedAssets.GetCollectable(ingredientId);
            switch (ingredient.Type)
            {
                case IngredientType.gem:
                    gemText.text = LocalizeLookUp.GetCollectableName(ingredientId);
                    continue;
                case IngredientType.herb:
                    herbText.text = LocalizeLookUp.GetCollectableName(ingredientId);
                    continue;
                case IngredientType.tool:
                    toolText.text = LocalizeLookUp.GetCollectableName(ingredientId);
                    continue;
            }
        }

        DownloadedAssets.GetSprite(id, spellImage);
        if (curSpell.school == 0)
            crestImage.sprite = crestSprites[0];
        else if (curSpell.school == 1)
            crestImage.sprite = crestSprites[1];
        else
            crestImage.sprite = crestSprites[2];

        LeanTween.moveLocalX(contentTransform, 0, speed * .5f).setEase(easeType);

        LeanTween.scale(crestTransform, Vector2.one, speed).setEase(easeType);
        LeanTween.scale(spellTransform, Vector2.one, speed * .5f).setEase(easeType);
        LeanTween.alphaCanvas(contentCG, 1, speed).setEase(easeType);
        canSwipe = true;
    }
    private void OnSwipeRight()
    {
        var curList = spellList;
        if (!canSwipe)
            return;
        if (index > 0)
            index--;
        else
            index = curList.Count - 1;
        ShowSpell(curList[index].id, false, 1);



    }
    private void OnSwipeLeft()
    {

        if (!canSwipe)
            return;

        var curList = spellList;
        // if (!isSpell)
        //     curList = signatureList;

        if (index < curList.Count - 1)
            index++;
        else
            index = 0;

        ShowSpell(curList[index].id, false, -1);

    }



}