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
    [SerializeField] private TextMeshProUGUI descLong;
    [SerializeField] private GameObject crestTransform;



    [SerializeField] private GameObject spellTransform;
    [SerializeField] private CanvasGroup contentCG;
    [SerializeField] private CanvasGroup navCG;
    [SerializeField] private Image crestImage;
    [SerializeField] private Image spellImage;
    [SerializeField] private Button castButton;
    [SerializeField] private Button signatureButton;


    [SerializeField] private Sprite[] crestSprites;
    [SerializeField] private LeanTweenType easeType;
    [SerializeField] private float speed;
    [SerializeField] private float movement = 200;
    [SerializeField] private Color unselectedColor;

    [Header("Signature")]
    [SerializeField] private GameObject ingredient;
    [SerializeField] private GameObject ingredientUndiscovered;
    [SerializeField] private TextMeshProUGUI toolText;
    [SerializeField] private TextMeshProUGUI herbText;
    [SerializeField] private TextMeshProUGUI gemText;
    [SerializeField] private Button signatureBackButton;

    private GameObject contentTransform;
    private bool canSwipe = true;
    private SwipeDetector swipeDetector;
    private string currentSpell = "";
    private Dictionary<string, Transform> navButtons = new Dictionary<string, Transform>();
    private int index = 0;
    private List<SpellData> spellList = new List<SpellData>();
    private List<SpellData> signatureList = new List<SpellData>();
    private bool isSpell = true;
    private string baseSpell = "";
    private void Start()
    {
        contentTransform = contentCG.gameObject;
        contentCG.alpha = 0;
        swipeDetector = gameObject.AddComponent<SwipeDetector>() as SwipeDetector;
        swipeDetector.canSwipe = true;
        swipeDetector.SwipeLeft = OnSwipeLeft;
        swipeDetector.SwipeRight = OnSwipeRight;
        swipeDetector.detectSwipeOnlyAfterRelease = true;
        signatureButton.onClick.AddListener(() => { CreateSignatureNavigation(currentSpell); });
        signatureBackButton.onClick.AddListener(ExitSignature);
        LeanTween.alphaCanvas(navCG, 1, .7f);
        CreateSpellNavigation();
    }

    private void ClearNavigation()
    {
        spellList.Clear();
        signatureList.Clear();
        navButtons.Clear();
        foreach (Transform item in navTransform)
        {
            Destroy(item.gameObject);
        }
        currentSpell = "";
    }

    private void CreateSpellNavigation()
    {
        isSpell = true;
        ingredient.SetActive(false);
        descLong.gameObject.SetActive(true);
        signatureBackButton.gameObject.SetActive(false);

        ClearNavigation();
        foreach (var item in PlayerDataManager.spells)
        {
            // Debug.Log(item.Value.id);
            var navButton = Utilities.InstantiateObject(circleNavPrefab, navTransform, 0);
            navButton.GetComponentInChildren<Button>().onClick.AddListener(() => { ShowSpell(item.Key, item.Key); });
            DownloadedAssets.GetSprite(item.Key, navButton.transform.GetChild(1).GetComponent<Image>());
            navButtons[item.Key] = navButton.transform;
            spellList.Add(item.Value);
            LeanTween.scale(navButton, Vector3.one, .8f).setEase(LeanTweenType.easeInOutSine);
        }
        if (baseSpell == "")
            ShowSpell(spellList[0].id, spellList[0].id);
        else
            ShowSpell(baseSpell, baseSpell);
    }

    private void ShowSpell(string id, string baseSpell, bool isButton = true, int side = 0)
    {

        var curList = spellList;
        if (!isSpell)
            curList = signatureList;

        if (isButton)
        {
            for (int i = 0; i < curList.Count; i++)
            {
                if (curList[i].id == id)
                    index = i;
            }
        }

        if (isSpell)
        {
            if (curList[index].signatures.Count < 1)
                signatureButton.gameObject.SetActive(false);
            else
                signatureButton.gameObject.SetActive(true);
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

        SetSpellUI(id, baseSpell, side);
    }

    private void SetSpellUI(string id, string baseSpell, int side)
    {

        canSwipe = false;
        LeanTween.scale(crestTransform, Vector2.one * .8f, speed * .5f).setEase(easeType);
        LeanTween.scale(spellTransform, Vector2.one * 1.2f, speed * .5f).setEase(easeType);
        if (side < 0)
            LeanTween.moveLocalX(contentTransform, -movement, speed * .5f).setEase(easeType);
        else if (side > 0)
            LeanTween.moveLocalX(contentTransform, movement, speed * .5f).setEase(easeType);

        var lID = LeanTween.alphaCanvas(contentCG, 0, speed * .5f).id;
        LeanTween.descr(lID).setOnComplete(() => { SetSpellPost(id, baseSpell, side); }).setEase(easeType);
    }
    private void SetSpellPost(string id, string baseSpell, int side)
    {
        if (side < 0)
            contentTransform.transform.Translate(movement, 0, 0);
        else if (side > 0)
            contentTransform.transform.Translate(-movement, 0, 0);

        if (id == "spell_trueSight" || id == "spell_invisibility")
            castButton.gameObject.SetActive(true);
        else
            castButton.gameObject.SetActive(false);

        var curSpell = PlayerDataManager.spells[baseSpell];
        cost.text = $"Cost: <b>{curSpell.cost.ToString()}";
        title.text = DownloadedAssets.spellDictData[id].spellName;
        descShort.text = DownloadedAssets.spellDictData[id].spellDescription;
        if (isSpell)
            descLong.text = DownloadedAssets.spellDictData[id].spellLore;
        DownloadedAssets.GetSprite(baseSpell, spellImage);
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
        if (!isSpell)
            curList = signatureList;
        if (!canSwipe)
            return;
        if (index > 0)
            index--;
        else
            index = curList.Count - 1;
        if (isSpell)
            ShowSpell(curList[index].id, curList[index].id, false, 1);
        else
            ShowSpell(signatureList[index].id, signatureList[index].baseSpell, false, 1);


    }
    private void OnSwipeLeft()
    {

        if (!canSwipe)
            return;

        var curList = spellList;
        if (!isSpell)
            curList = signatureList;

        if (index < curList.Count - 1)
            index++;
        else
            index = 0;
        if (isSpell)
            ShowSpell(curList[index].id, curList[index].id, false, -1);
        else
            ShowSpell(signatureList[index].id, signatureList[index].baseSpell, false, -1);

    }
    private void CreateSignatureNavigation(string spell)
    {
        isSpell = false;
        descLong.gameObject.SetActive(false);
        ingredient.SetActive(true);
        signatureBackButton.gameObject.SetActive(true);
        ClearNavigation();
        foreach (var item in PlayerDataManager.spells[spell].signatures)
        {
            // Debug.Log(item.Value.id);
            var navButton = Utilities.InstantiateObject(circleNavPrefab, navTransform, 0);
            navButton.GetComponentInChildren<Button>().onClick.AddListener(() => { ShowSpell(item.id, item.baseSpell); });
            DownloadedAssets.GetSprite(item.baseSpell, navButton.transform.GetChild(1).GetComponent<Image>());
            navButtons[item.id] = navButton.transform;
            LeanTween.scale(navButton, Vector3.one, .8f).setEase(LeanTweenType.easeInOutSine);

            signatureList.Add(item);
        }
        contentCG.alpha = 0;
        ShowSpell(signatureList[0].id, signatureList[0].baseSpell);
        baseSpell = spell;
    }

    private void ExitSignature()
    {
        if (!isSpell)
        {
            CreateSpellNavigation();
        }
    }

}