using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Raincrow.Analytics.Events;

[RequireComponent(typeof(SwipeDetector))]
public class SpellManager : MonoBehaviour
{
    public static SpellManager Instance { get; set; }
    public static SpellData CurrentSpell;

    public GameObject[] spellBookButtons;
    public GameObject spellBook;
    public Text spellTitle;
    public Text spellDesc;
    public Text spellLore;
    public Text spellEnergy;
    public GameObject closeButton;
    public Image spellGlyph;
    public GameObject[] schoolItemsWhite;
    public GameObject[] schoolItemsShadow;
    public GameObject[] schoolItemsGrey;
    public Text[] schoolColorText;
    public GameObject ingredientObject;
    public SpellFilter filter = SpellFilter.shadow;

    public Color shadowColor;
    public Color whiteColor;
    public Color greyColor;

    public Transform circleNavContainer;
    public GameObject circleNavIcon;

    List<Image> NavItems = new List<Image>();

    public Sprite circleSprite;
    public Sprite circleFill;

    /*
	 ORDER IS WHITE GRAY SHADOW FOR ARRAY
	*/

    public static List<SpellData> whiteSpells = new List<SpellData>();
    public static List<SpellData> greySpells = new List<SpellData>();
    public static List<SpellData> shadowSpells = new List<SpellData>();
    public Text PlayerImmune;
    public GameObject loadingFX;

    public GameObject SignatureObject;
    public Text signatureTitle;

    public Button castButton;
    public Button increasePowerButton;
    public GameObject conditions;
    [HideInInspector]
    public SwipeDetector SD;

    public static int whiteSpellIndex = 0;
    public static int greySpellIndex = 0;
    public static int shadowSpellIndex = 0;
    bool Immune = false;
    bool spellBookOpened = false;
    bool isExit = false;

    public static bool isInSpellView = false;

    [SerializeField] public CanvasGroup mainCanvasGroup;

    void Awake()
    {
        Instance = this;
        SD = GetComponent<SwipeDetector>();
        SD.SwipeRight += OnSwipeRight;
        SD.SwipeLeft += OnSwipeLeft;

        if (mainCanvasGroup == null)
        {
            mainCanvasGroup = this.GetComponent<CanvasGroup>();
        }
        mainCanvasGroup.interactable = false;
    }

    public void ChangeFilterType(int i)
    {
        if (i == 0)
        {
            filter = SpellFilter.white;
        }
        else if (i == 1)
        {
            filter = SpellFilter.grey;
        }
        else
        {
            filter = SpellFilter.shadow;
        }
        SetPageFilter();
    }

    public void Initialize()
    {
        mainCanvasGroup.interactable = true;

        isInSpellView = true;
        isExit = false;
        Immune = false;
        loadingFX.SetActive(false);
        Show(conditions);
        PlayerImmune.gameObject.SetActive(false);

        spellBook.SetActive(false);
        foreach (var item in spellBookButtons)
        {
            Show(item);
        }
        whiteSpells = PlayerDataManager.playerData.spells.Where(spell => spell.school > 0).OrderBy(spell => spell.displayName).ToList();
        greySpells = PlayerDataManager.playerData.spells.Where(spell => spell.school == 0).OrderBy(spell => spell.displayName).ToList();
        shadowSpells = PlayerDataManager.playerData.spells.Where(spell => spell.school < 0).OrderBy(spell => spell.displayName).ToList();

        whiteSpells = filterSpellSigs(whiteSpells);
        greySpells = filterSpellSigs(greySpells);
        shadowSpells = filterSpellSigs(shadowSpells);
        SD.canSwipe = true;
        //		SetPageFilter (); 
        if (MarkerSpawner.SelectedMarker.state == "dead" || MarkerSpawner.SelectedMarker.energy == 0)
        {
            HitFXManager.Instance.TargetDead();
        }
        Show(closeButton);

        RemoveInvalidSpells();
    }

    public void ForceCloseSpellBook()
    {
        mainCanvasGroup.interactable = false;

        Hide(closeButton);
        foreach (var item in spellBookButtons)
        {
            Hide(item);
        }
        isExit = true;
        if (SignatureObject.activeInHierarchy)
        {
            SignatureObject.SetActive(false);
        }
        SD.canSwipe = false;
        Hide(conditions);
        isInSpellView = false;
    }

    List<SpellData> filterSpellSigs(List<SpellData> spells)
    {
        List<SpellData> tempSpells = new List<SpellData>();
        foreach (var item in whiteSpells)
        {
            if (item.baseSpell == item.id)
                tempSpells.Add(item);
            else
            {
                if (item.unlocked)
                    tempSpells.Add(item);
            }
        }
        return tempSpells;
    }

    public void StateChanged()
    {
        string currentWhiteSpell = whiteSpells[whiteSpellIndex].id;
        string currentGreySpell = greySpells[greySpellIndex].id;
        string currentShadowSpell = shadowSpells[shadowSpellIndex].id;

        whiteSpells = PlayerDataManager.playerData.spells.Where(spell => spell.school > 0).OrderBy(spell => spell.displayName).ToList();
        greySpells = PlayerDataManager.playerData.spells.Where(spell => spell.school == 0).OrderBy(spell => spell.displayName).ToList();
        shadowSpells = PlayerDataManager.playerData.spells.Where(spell => spell.school < 0).OrderBy(spell => spell.displayName).ToList();

        whiteSpells = filterSpellSigs(whiteSpells);
        greySpells = filterSpellSigs(greySpells);
        shadowSpells = filterSpellSigs(shadowSpells);
        RemoveInvalidSpells();

        for (int i = 0; i < whiteSpells.Count; i++)
        {
            if (whiteSpells[i].id == currentWhiteSpell)
                whiteSpellIndex = i;
        }

        for (int i = 0; i < greySpells.Count; i++)
        {
            if (greySpells[i].id == currentGreySpell)
                greySpellIndex = i;
        }

        for (int i = 0; i < shadowSpells.Count; i++)
        {
            if (shadowSpells[i].id == currentShadowSpell)
                shadowSpellIndex = i;
        }


        foreach (Transform item in circleNavContainer)
        {
            Destroy(item.gameObject);
        }
        NavItems.Clear();

        if (spellBookOpened)
        {
            //			print ("StateChanged!");
            if (filter == SpellFilter.shadow)
            {
                for (int i = 0; i < shadowSpells.Count; i++)
                {
                    SpawnCircle();
                }
                SetCircleState(shadowSpellIndex);
                SetSpell(shadowSpells[shadowSpellIndex]);
            }
            else if (filter == SpellFilter.white)
            {
                for (int i = 0; i < whiteSpells.Count; i++)
                {
                    SpawnCircle();
                }
                SetCircleState(whiteSpellIndex);
                SetSpell(whiteSpells[whiteSpellIndex]);

            }
            else
            {
                for (int i = 0; i < greySpells.Count; i++)
                {
                    SpawnCircle();
                }
                SetCircleState(greySpellIndex);
                SetSpell(greySpells[greySpellIndex]);
            }

        }

    }

    void RemoveInvalidSpells()
    {
        //		print ("Removing Invalid Spells");

        try
        {
            string curState = MarkerSpawner.SelectedMarker.state;
            //		print ("Current State of Player : " + curState + curState.Length);
            var tempWhiteSpells = new List<SpellData>();
            //			print(whiteSpells.Count + "white Spell Count");

            foreach (var item in whiteSpells)
            {
                //				print(item.id);
                foreach (var state in item.states)
                {
                    if (state == curState)
                    {
                        tempWhiteSpells.Add(item);
                        break;
                    }
                }
            }

            var tempShadowSpells = new List<SpellData>();

            foreach (var item in shadowSpells)
            {
                foreach (var state in item.states)
                {
                    if (state == curState)
                    {
                        tempShadowSpells.Add(item);
                        break;
                    }
                }
            }

            var tempGreySpells = new List<SpellData>();
            //			print(greySpells.Count + "Grey Spell Count");
            foreach (var item in greySpells)
            {
                //				print(item.id);
                foreach (var state in item.states)
                {
                    if (state == curState)
                    {
                        tempGreySpells.Add(item);
                        break;
                    }
                }
            }

            whiteSpells = tempWhiteSpells;
            greySpells = tempGreySpells;
            shadowSpells = tempShadowSpells;


        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }
    }

    public void OnSwipeRight()
    {
        //swipe is only used in the spell list
        if (spellBook.activeSelf == false)
        {
            return;
        }

        SoundManagerOneShot.Instance.PlayWhisper();
        if (filter == SpellFilter.white)
        {
            if (whiteSpellIndex < whiteSpells.Count - 1)
            {
                whiteSpellIndex++;
            }
            else
            {
                whiteSpellIndex = 0;
            }
            SetSpell(whiteSpells[whiteSpellIndex]);
            SetCircleState(whiteSpellIndex);
        }
        else if (filter == SpellFilter.shadow)
        {
            if (shadowSpellIndex < shadowSpells.Count - 1)
            {
                shadowSpellIndex++;
            }
            else
            {
                shadowSpellIndex = 0;
            }
            SetSpell(shadowSpells[shadowSpellIndex]);
            SetCircleState(shadowSpellIndex);
        }
        else
        {
            if (greySpellIndex < greySpells.Count - 1)
            {
                greySpellIndex++;
            }
            else
            {
                greySpellIndex = 0;
            }
            SetSpell(greySpells[greySpellIndex]);
            SetCircleState(greySpellIndex);
        }
    }

    public void OnSwipeLeft()
    {
        if (spellBook.activeSelf == false)
        {
            return;
        }

        SoundManagerOneShot.Instance.PlayWhisper();
        if (filter == SpellFilter.white)
        {
            if (whiteSpellIndex > 0)
            {
                whiteSpellIndex--;
            }
            else
            {
                whiteSpellIndex = whiteSpells.Count - 1;
            }
            SetSpell(whiteSpells[whiteSpellIndex]);
            SetCircleState(whiteSpellIndex);
        }
        else if (filter == SpellFilter.shadow)
        {
            if (shadowSpellIndex > 0)
            {
                shadowSpellIndex--;
            }
            else
            {
                shadowSpellIndex = shadowSpells.Count - 1;
            }
            SetSpell(shadowSpells[shadowSpellIndex]);
            SetCircleState(shadowSpellIndex);
        }
        else
        {
            if (greySpellIndex > 0)
            {
                greySpellIndex--;
            }
            else
            {
                greySpellIndex = greySpells.Count - 1;
            }
            SetSpell(greySpells[greySpellIndex]);
            SetCircleState(greySpellIndex);
        }
    }

    public void Exit()
    {
        mainCanvasGroup.interactable = false;

        if (isExit)
        {
            foreach (var item in spellBookButtons)
            {
                item.SetActive(false);
            }
            closeButton.SetActive(false);
            return;
        }

        isExit = true;
        if (SignatureObject.activeInHierarchy)
        {
            SignatureObject.SetActive(false);
        }
        //		print ("Exitng Spell Cast");
        SD.canSwipe = false;
        SoundManagerOneShot.Instance.MenuSound();
        if (spellBookOpened)
            Hide(spellBook);
        //		print ("going Back");
        MapSelection.Instance.GoBack();
        //		print ("hiding close");
        Hide(closeButton);
        if (PlayerImmune.gameObject.activeInHierarchy)
        {
            Hide(PlayerImmune.gameObject);
        }
        else
        {
            foreach (var item in spellBookButtons)
            {
                Hide(item);
            }
        }
        Hide(conditions);
        //		print (Immune);
        if (Immune)
        {
            HitFXManager.Instance.SetImmune(false, true);
        }
        if (MarkerSpawner.selectedType != MarkerSpawner.MarkerType.spirit)
        {
            if (MarkerSpawner.SelectedMarker.state == "dead" || MarkerSpawner.SelectedMarker.energy == 0)
            {
                HitFXManager.Instance.TargetRevive(true);
            }
        }
        isInSpellView = false;
    }

    public void CloseSpellBook(bool isCast = false)
    {
        spellBookOpened = false;

        SoundManagerOneShot.Instance.MenuSound();

        Hide(spellBook);
        if (MarkerSpawner.selectedType != MarkerSpawner.MarkerType.witch || !isCast)
        {
            foreach (var item in spellBookButtons)
            {
                Show(item);
            }
        }
        Show(closeButton);
    }

    public void OpenSpellBook()
    {
        spellGlyph.GetComponent<CanvasGroup>().alpha = 1;
        spellGlyph.transform.localScale = Vector3.one;
        spellBookOpened = true;
        SoundManagerOneShot.Instance.MenuSound();

        foreach (var item in spellBookButtons)
        {
            Hide(item);
        }
        Show(spellBook);
        Hide(closeButton);

    }

    void SetPageFilter()
    {

        foreach (var item in schoolItemsGrey)
        {
            item.SetActive(false);
        }
        foreach (var item in schoolItemsWhite)
        {
            item.SetActive(false);
        }
        foreach (var item in schoolItemsShadow)
        {
            item.SetActive(false);
        }
        schoolItemsGrey[2].transform.GetChild(0).gameObject.SetActive(false);
        schoolItemsWhite[2].transform.GetChild(0).gameObject.SetActive(false);
        schoolItemsShadow[2].transform.GetChild(0).gameObject.SetActive(false);
        foreach (Transform item in circleNavContainer)
        {
            Destroy(item.gameObject);
        }
        NavItems.Clear();

        if (filter == SpellFilter.shadow)
        {
            schoolColorText[0].color = shadowColor;
            schoolColorText[1].color = shadowColor;
            for (int i = 0; i < shadowSpells.Count; i++)
            {
                SpawnCircle();
            }

            foreach (var item in schoolItemsShadow)
            {
                item.SetActive(true);
            }
            SetCircleState(shadowSpellIndex);
            SetSpell(shadowSpells[shadowSpellIndex]);
        }
        else if (filter == SpellFilter.white)
        {
            schoolColorText[0].color = whiteColor;
            schoolColorText[1].color = whiteColor;
            for (int i = 0; i < whiteSpells.Count; i++)
            {
                SpawnCircle();
            }

            foreach (var item in schoolItemsWhite)
            {
                item.SetActive(true);
            }
            SetCircleState(whiteSpellIndex);
            SetSpell(whiteSpells[whiteSpellIndex]);

        }
        else
        {
            schoolColorText[0].color = greyColor;
            schoolColorText[1].color = greyColor;
            for (int i = 0; i < greySpells.Count; i++)
            {
                SpawnCircle();
            }

            foreach (var item in schoolItemsGrey)
            {
                item.SetActive(true);
            }
            SetCircleState(greySpellIndex);
            SetSpell(greySpells[greySpellIndex]);
        }

        OpenSpellBook();
    }

    void SpawnCircle()
    {
        var g = Utilities.InstantiateObject(circleNavIcon, circleNavContainer, 1.5f);
        var sp = g.GetComponentInChildren<Image>();
        NavItems.Add(sp);
    }

    void SetCircleState(int index)
    {
        if (NavItems.Count - 1 < index)
        {
            greySpellIndex = 0;
            shadowSpellIndex = 0;
            whiteSpellIndex = 0;
            index = 0;
        }
        foreach (var item in NavItems)
        {
            item.sprite = circleSprite;
        }
        NavItems[index].sprite = circleFill;
    }

    void SetSpell(SpellData currentSpell)
    {
        CurrentSpell = currentSpell;
        spellTitle.text = "Cast " + currentSpell.displayName;
        spellDesc.text = currentSpell.description;
        spellLore.text = currentSpell.lore;
        spellEnergy.text = "Cost : " + currentSpell.cost.ToString();

        DownloadedAssets.GetSprite(currentSpell.baseSpell, spellGlyph);
        if (IngredientManager.Instance.AddBaseIngredients())
        {
            increasePowerButton.interactable = true;
            castButton.interactable = true;
        }
        else
        {
            increasePowerButton.interactable = false;
            castButton.interactable = false;
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

        if (!g.activeInHierarchy)
        {
            g.SetActive(true);
        }
        var anim = g.GetComponent<Animator>();
        anim.Play("out");
        if (isDisable)
            StartCoroutine(DisableObject(g));
    }

    IEnumerator DisableObject(GameObject g)
    {
        yield return new WaitForSeconds(.55f);
        g.SetActive(false);
    }

    #endregion

    public void CastSpell()
    {
        castButton.interactable = false;

        var data = CalculateSpellData();

        SpellAnalytics.CastSpell(data.spell, MarkerSpawner.selectedType.ToString(), data.ingredients);

        CastSpellAPI(data);
        StartCoroutine(CastSpellFX());
    }

    public bool castingSpellAnim = false;
    IEnumerator CastSpellFX()
    {
        castingSpellAnim = true;
        if (ingredientObject.activeInHierarchy)
        {
            Hide(ingredientObject);
            yield return new WaitForSeconds(.4f);
        }

        StartCoroutine(HideGlyph());
        SoundManagerOneShot.Instance.PlayWhisperFX();

        if (filter == SpellFilter.grey)
        {
            var g = schoolItemsGrey[2];
            g.transform.GetChild(0).gameObject.SetActive(true);
        }
        else if (filter == SpellFilter.shadow)
        {
            var g = schoolItemsShadow[2];
            g.transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            var g = schoolItemsWhite[2];
            g.transform.GetChild(0).gameObject.SetActive(true);
        }
        yield return new WaitForSeconds(2);
        CloseSpellBook(true);
        castingSpellAnim = false;
    }

    IEnumerator HideGlyph()
    {
        var cg = spellGlyph.GetComponent<CanvasGroup>();
        float t = 0;
        while (t <= 1)
        {
            t += Time.deltaTime * 2;
            cg.alpha = Mathf.SmoothStep(1, 0, t);
            spellGlyph.transform.localScale = Vector3.one * Mathf.SmoothStep(1, 0, t);
            yield return 0;
        }
    }

    SpellTargetData CalculateSpellData()
    {
        var data = new SpellTargetData();
        data.ingredients = new List<spellIngredientsData>();
        data.spell = CurrentSpell.baseSpell;
        data.target = MarkerSpawner.instanceID;
        if (IngredientManager.addedHerb != "")
        {
            data.ingredients.Add(new spellIngredientsData
            {
                id = IngredientManager.addedHerb,
                count = IngredientManager.addedHerbCount
            });
        }
        if (IngredientManager.addedTool != "")
        {
            data.ingredients.Add(new spellIngredientsData
            {
                id = IngredientManager.addedTool,
                count = IngredientManager.addedToolCount
            });
        }
        if (IngredientManager.addedGem != "")
        {
            data.ingredients.Add(new spellIngredientsData
            {
                id = IngredientManager.addedGem,
                count = IngredientManager.addedGemCount
            });
        }
        IngredientManager.ClearIngredient();
        return data;
    }

    void CastSpellAPI(SpellTargetData target)
    {
        mainCanvasGroup.interactable = false;
        loadingFX.SetActive(true);
        APIManager.Instance.PostCoven("spell/targeted", JsonConvert.SerializeObject(target), GetCastSpellCallback);
    }

    void GetCastSpellCallback(string result, int response)
    {
        //mainCanvasGroup.interactable = true;

        print("Casting Response : " + result);

        if (response == 200)
        {
            try
            {

            }
            catch (System.Exception e)
            {
                Debug.LogError(e.ToString() + "\n" + e.StackTrace);
            }
        }
        else
        {
            print("turning on error");
            closeButton.SetActive(true);
            loadingFX.SetActive(false);
            mainCanvasGroup.interactable = true;
            if (result == "4301")
            {
                HitFXManager.Instance.TargetDead(true);
            }
            else if (result == "4700")
            {
                PlayerDataManager.playerData.state = "dead";
                PlayerDataManager.playerData.energy = 0;
                Exit();
            }
            else if (result == "4704")
            {
                HitFXManager.Instance.Escape();
            }
            else
            {
                UIGlobalErrorPopup.ShowError(() => Exit(), "Unknown error [" + result + "]");
            }
        }
    }

    public void IsImmune(bool immune)
    {
        Immune = immune;
        if (immune)
        {
            //foreach (var item in spellBookButtons)
            //{
            //    //				print ("Hiding Button");
            //    Hide(item);
            //}
            PlayerImmune.text = MarkerSpawner.SelectedMarker.displayName + " is now immune to you.";
            Show(PlayerImmune.gameObject);
        }
        else
        {
            //			print ("Show button");

            foreach (var item in spellBookButtons)
            {
                Show(item);
            }
            Hide(PlayerImmune.gameObject);
        }
    }

    public void CastSpellFTF()
    {
        StartCoroutine(CastSpellFX());
    }
}

public enum SpellFilter
{
    white,
    shadow,
    grey
}

