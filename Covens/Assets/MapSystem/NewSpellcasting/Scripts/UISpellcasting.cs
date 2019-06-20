using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Raincrow.Maps;
using TMPro;

public class UISpellcasting : UIInfoPanel
{
    [SerializeField] private Button m_BackButton;
    [SerializeField] private Button m_CloseButton;

    [Header("School selection")]
    [SerializeField] private Button m_ShadowButton;
    [SerializeField] private Button m_GreyButton;
    [SerializeField] private Button m_LightButton;
    [SerializeField] private TextMeshProUGUI m_ShadowText;
    [SerializeField] private TextMeshProUGUI m_GreyText;
    [SerializeField] private TextMeshProUGUI m_WhiteText;

    [Header("shared")]
    [SerializeField] private Button m_CastButton;
    [Header("Inventory Button and Button Accessories")]
    [SerializeField] private Button m_InventoryButton;
    [SerializeField] private CanvasGroup o_InventoryButtonCG;
    [SerializeField] private GameObject o_InventoryButtonImage;
    [SerializeField] private GameObject o_InventoryButtonTop;

    [Header("Spell selection")]
    [SerializeField] private CanvasGroup m_SelectionGroup;
    [SerializeField] private TextMeshProUGUI m_SelectedTitle;
    [SerializeField] private TextMeshProUGUI m_SelectedCost;
    [SerializeField] private Button m_SpellInfoButton;
    [SerializeField] private UISpellcastingItem m_SpellEntryPrefab;
    [SerializeField] private Transform m_SpellContainer;
    [SerializeField] private RectTransform m_SelectedSpellOverlay;
    [SerializeField] private Image m_SelectedSpell_Glow;

    [Header("Spell info")]
    [SerializeField] private CanvasGroup m_InfoGroup;
    [SerializeField] private TextMeshProUGUI m_InfoTitle;
    [SerializeField] private TextMeshProUGUI m_InfoCost;
    [SerializeField] private TextMeshProUGUI m_InfoDesc;
    [SerializeField] private Button m_InfoBackButton;

    private static UISpellcasting m_Instance;
    public static UISpellcasting Instance
    {
        get
        {
            if (m_Instance == null)
                m_Instance = Instantiate(Resources.Load<UISpellcasting>("UISpellcasting"));
            return m_Instance;
        }
    }

    public static bool isOpen
    {
        get
        {
            if (m_Instance == null)
                return false;
            else
                return m_Instance.IsShowing;
        }
    }

    private List<UISpellcastingItem> m_SpellButtons = new List<UISpellcastingItem>();
    private List<SpellData> m_Spells;
    private CharacterMarkerDetail m_Target;
    private IMarker m_Marker;
    private System.Action m_OnFinishSpellcasting;
    private System.Action m_OnBack;
    private System.Action m_OnClose;
    private int m_SelectedSchool = -999;
    private int m_PreviousSchool = -999;
    private int m_PreviousSpell = 0;
    private int m_InfoTweenId;
    private SpellData m_SelectedSpell;

    private bool m_HerbRequired;
    private bool m_ToolRequired;
    private bool m_GemRequired;

    private InventoryItems m_SelectedHerb = null;
    private InventoryItems m_SelectedTool = null;
    private InventoryItems m_SelectedGem = null;
    private int m_SelectedHerbAmount = 0;
    private int m_SelectedToolAmount = 0;
    private int m_SelectedGemAmount = 0;
	public GameObject ShadowGlyphBG;
	public GameObject GreyGlyphBG;
	public GameObject WhiteGlyphBG;
	public Image CastGlyphBG;

    protected override void Awake()
    {
        base.Awake();

        m_Instance = this;
        o_InventoryButtonImage.GetComponent<Image>().color = Color.white;
        //setup initial state
        m_SpellEntryPrefab.gameObject.SetActive(false);
        m_SpellEntryPrefab.transform.SetParent(this.transform);
        m_SelectedSpellOverlay.gameObject.SetActive(false);
        m_SelectedSpellOverlay.SetParent(transform);
        LeanTween.alphaCanvas(o_InventoryButtonCG, 0f, 0.01f);
        o_InventoryButtonTop.SetActive(false);
        //setup buttons
        m_BackButton.onClick.AddListener(OnClickBack);
        m_CloseButton.onClick.AddListener(OnClickClose);
        m_CastButton.onClick.AddListener(OnConfirmSpellcast);
        m_InventoryButton.onClick.AddListener(OnClickInventory);

        m_ShadowButton.onClick.AddListener(() =>
        {
            SetupSpellSelection(-1);
        });
        m_GreyButton.onClick.AddListener(() =>
        {
            SetupSpellSelection(0);
        });
        m_LightButton.onClick.AddListener(() =>
        {
            SetupSpellSelection(1);
        });

        m_SpellInfoButton.onClick.AddListener(OnClickSpellInfo);
        m_InfoBackButton.onClick.AddListener(OnClickCloseInfo);
    }

    public void Show(CharacterMarkerDetail target, IMarker marker, List<SpellData> spells, System.Action onFinishSpellcasting, System.Action onBack = null, System.Action onClose = null)
    {
        m_Target = target;
        m_Marker = marker;
        m_Spells = spells;
        m_OnFinishSpellcasting = onFinishSpellcasting;
        m_OnBack = onBack;
        m_OnClose = onClose;

        int school = m_PreviousSchool;
        if (school == -999)
            school = PlayerDataManager.playerData.degree == 0 ? 0 : (int)Mathf.Sign(PlayerDataManager.playerData.degree);

        SetupSpellSelection(school);

        m_InfoGroup.gameObject.SetActive(false);
        m_SelectionGroup.alpha = 1;

        base.Show();
    }

    public override void Close()
    {
        base.Close();

        if (UIInventory.isOpen)
            UIInventory.Instance.Close(true);
        o_InventoryButtonTop.SetActive(false);
        m_SelectedHerb = m_SelectedTool = m_SelectedGem = null;
        m_SelectedHerbAmount = m_SelectedToolAmount = m_SelectedGemAmount = 0;

        m_SelectedSchool = -999;
        m_OnFinishSpellcasting = null;
        m_OnBack = null;
        m_OnClose = null;
    }

    protected override void ReOpenAnimation()
    {
        base.ReOpenAnimation();
        LeanTween.alphaCanvas(o_InventoryButtonCG, 0, 0.5f).setOnComplete(() =>
           {
               LeanTween.alphaCanvas(o_InventoryButtonCG, 1f, 0.3f);
           });
        o_InventoryButtonTop.SetActive(true);
    }

    public override void Hide()
    {

        base.Hide();
        //.setEaseOutCubic;
        //var p = o_ButtonGlow.GetComponentInParent<CanvasGroup>();
        //m_InventoryButton.gameObject.SetActive(false);
        LeanTween.alphaCanvas(o_InventoryButtonCG, 0f, 0.2f);
        o_InventoryButtonImage.GetComponent<Image>().color = Color.white;
        o_InventoryButtonTop.SetActive(false);
        //LeanTween.alphaCanvas (p, 0f, 0.5f);
        //o_ButtonGlow.SetActive (false);
    }

    public void SetupSpellSelection(int school)
    {
        if (m_SelectedSchool != school)
        {
            m_PreviousSpell = 0;

            m_InfoGroup.alpha = 0;

            m_SelectedSpellOverlay.gameObject.SetActive(false);

			m_ShadowText.text = LocalizeLookUp.GetText ("generic_shadow");// "Shadow";
			m_GreyText.text = LocalizeLookUp.GetText ("generic_grey");//  "Grey";
			m_WhiteText.text = LocalizeLookUp.GetText ("generic_white");//  "White";
            Color color;
            if (school < 0)
            {
                //m_ShadowText.text = "<u>Shadow</u>";
				ShadowGlyphBG.gameObject.SetActive (true);
				LeanTween.alphaCanvas (ShadowGlyphBG.GetComponent<CanvasGroup>(), 1f, 0.7f).setEase (LeanTweenType.easeInOutCubic);
				LeanTween.alphaCanvas (GreyGlyphBG.GetComponent<CanvasGroup>(), 0f, 0.7f).setEase (LeanTweenType.easeInOutCubic).setOnComplete(() => { 
					GreyGlyphBG.gameObject.SetActive (false);
				});
				LeanTween.alphaCanvas (WhiteGlyphBG.GetComponent<CanvasGroup>(), 0f, 0.7f).setEase (LeanTweenType.easeInOutCubic).setOnComplete(() => {
					WhiteGlyphBG.gameObject.SetActive (false);
				});
				CastGlyphBG.color = new Color (0.88f, 0.7294f, 1f, 0.545f);
                color = Utilities.Purple;
            }
            else if (school > 0)
            {
                //m_WhiteText.text = "<u>White</u>";
				//ShadowGlyphBG.gameObject.SetActive (false);
				//GreyGlyphBG.gameObject.SetActive (false);
				CastGlyphBG.color = new Color (1f, 0.891f, 0.731f, 0.545f);
				WhiteGlyphBG.gameObject.SetActive (true);
				LeanTween.alphaCanvas (WhiteGlyphBG.GetComponent<CanvasGroup>(), 1f, 0.7f).setEase (LeanTweenType.easeInOutCubic);

				LeanTween.alphaCanvas (GreyGlyphBG.GetComponent<CanvasGroup>(), 0f, 0.7f).setEase (LeanTweenType.easeInOutCubic).setOnComplete(() => { 
					GreyGlyphBG.gameObject.SetActive (false);
				});
				LeanTween.alphaCanvas (ShadowGlyphBG.GetComponent<CanvasGroup>(), 0f, 0.7f).setEase (LeanTweenType.easeInOutCubic).setOnComplete(() => {
					ShadowGlyphBG.gameObject.SetActive (false);
				});
                color = Utilities.Orange;
            }
            else
            {
                //m_GreyText.text = "<u>Grey</u>";
				//ShadowGlyphBG.gameObject.SetActive (false);
				GreyGlyphBG.gameObject.SetActive (true);
				LeanTween.alphaCanvas (GreyGlyphBG.GetComponent<CanvasGroup>(), 1f, 0.7f).setEase (LeanTweenType.easeInOutCubic);
				CastGlyphBG.color = new Color (0.7294f, 0.8526f, 1f, 0.545f);
				LeanTween.alphaCanvas (ShadowGlyphBG.GetComponent<CanvasGroup>(), 0f, 0.7f).setEase (LeanTweenType.easeInOutCubic).setOnComplete(() => { 
					ShadowGlyphBG.gameObject.SetActive (false);
				});
				LeanTween.alphaCanvas (WhiteGlyphBG.GetComponent<CanvasGroup>(), 0f, 0.7f).setEase (LeanTweenType.easeInOutCubic).setOnComplete(() => {
					WhiteGlyphBG.gameObject.SetActive (false);
				});
				//WhiteGlyphBG.gameObject.SetActive (false);

                color = Utilities.Blue;
            }
            color.a = 0.3f;
            m_SelectedSpell_Glow.color = color;

            //setup spells
            StopAllCoroutines();

            //disable buttons
            for (int i = 0; i < m_SpellButtons.Count; i++)
                m_SpellButtons[i].Hide();

            m_SelectedSpellOverlay.gameObject.SetActive(false);

            List<SpellData> spells = new List<SpellData>();
            for (int i = 0; i < m_Spells.Count; i++)
            {
                if (m_Spells[i].school == school)
                    spells.Add(m_Spells[i]);
            }
            StartCoroutine(SetupSpellList(spells));
        }

        m_PreviousSchool = m_SelectedSchool = school;
    }

    private IEnumerator SetupSpellList(List<SpellData> spells)
    {
        for (int i = 0; i < spells.Count; i++)
        {
            UISpellcastingItem item;
            if (i >= m_SpellButtons.Count)
                m_SpellButtons.Add(Instantiate(m_SpellEntryPrefab, m_SpellContainer));

            item = m_SpellButtons[i];
            int aux = i;
            item.Setup(m_Target, m_Marker, spells[i], (_item, spell) => { m_PreviousSpell = aux; OnSelectSpell(_item, spell); });
        }
        yield return 0;

        LayoutRebuilder.ForceRebuildLayoutImmediate(m_SpellContainer.parent.GetComponent<RectTransform>());

        m_SpellButtons[m_PreviousSpell].OnClick();

        for (int i = 0; i < spells.Count; i++)
        {
            m_SpellButtons[i].Show();
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void FinishSpellcastingFlow()
    {
        ReOpen();
        m_OnFinishSpellcasting?.Invoke();
    }

    public override void ReOpen()
    {
        base.ReOpen();
        if (m_SelectedSpell != null)
            LockIngredients(m_SelectedSpell.ingredients);
        UpdateCanCast();
    }

    private void OnClickBack()
    {
        System.Action action = m_OnBack;
        Close();
        action?.Invoke();
    }

    private void OnClickClose()
    {
        System.Action action = m_OnClose;
        Close();
        action?.Invoke();
    }

    private void OnClickSpellInfo()
    {
        ShowSpellInfo(DownloadedAssets.GetSpell(m_SelectedSpell.id), m_SelectedSpell);
    }

    public void UpdateCanCast()
    {
        Spellcasting.SpellState canCast = Spellcasting.CanCast(m_SelectedSpell, m_Marker, m_Target);

        m_CastButton.interactable = canCast == Spellcasting.SpellState.CanCast;
        TextMeshProUGUI castText = m_CastButton.GetComponent<TextMeshProUGUI>();

        if (canCast == Spellcasting.SpellState.TargetImmune)
        {
			castText.text =  LocalizeLookUp.GetText ("spell_immune_to_you");// "Witch is immune";

        }
        else if (canCast == Spellcasting.SpellState.PlayerSilenced)
        {
			castText.text =  LocalizeLookUp.GetText ("ftf_silenced");// "You are silenced";
        }
        else if (canCast == Spellcasting.SpellState.MissingIngredients)
        {
			castText.text =  LocalizeLookUp.GetText ("inventory_missing") + " " + LocalizeLookUp.GetText ("store_ingredients");// ;// "Missing ingredients";
        }
        else if (canCast == Spellcasting.SpellState.CanCast)
        {
            if (BuildIngredientList().Count > 0)
				castText.text = LocalizeLookUp.GetText ("card_witch_cast_ingredients");//  "Cast with ingredients";
            else
				castText.text = LocalizeLookUp.GetText ("card_witch_cast");//  "Cast";
        }
        else
        {
            string displayname = m_Target is WitchMarkerDetail ? (m_Target as WitchMarkerDetail).displayName : DownloadedAssets.spiritDictData[(m_Target as SpiritMarkerDetail).id].spiritName;
			castText.text = LocalizeLookUp.GetText ("card_witch_cant_cast").Replace("{{target}}", displayname);//  "Can't cast on " + m_Target.displayName;
        }
    }

    private void OnSelectSpell(UISpellcastingItem item, SpellData spell)
    {
        m_SelectedSpell = spell;

        m_HerbRequired = m_ToolRequired = m_GemRequired = false;
        if (spell.ingredients != null)
        {
            for (int i = 0; i < spell.ingredients.Length; i++)
            {
                IngredientType ingType;
                InventoryItems invItem;
                PlayerDataManager.playerData.ingredients.GetIngredient(spell.ingredients[i], out invItem, out ingType);

                if (ingType == IngredientType.herb)
                    m_HerbRequired = true;
                else if (ingType == IngredientType.tool)
                    m_ToolRequired = true;
                else if (ingType == IngredientType.gem)
                    m_GemRequired = true;
            }
        }

        m_SelectedSpellOverlay.SetParent(item.transform);
        m_SelectedSpellOverlay.localPosition = Vector2.zero;
        m_SelectedSpellOverlay.gameObject.SetActive(true);

        m_SelectedTitle.text = spell.displayName;
		m_SelectedCost.text = LocalizeLookUp.GetText ("moon_energy").Replace ("{{Amount}}", spell.cost.ToString());// $"({spell.cost} Energy)";

        LockIngredients(spell.ingredients);
        UpdateCanCast();
    }

    private void OnConfirmSpellcast()
    {
        Hide();

        if (UIInventory.isOpen)
        {
            UIInventory.Instance.Close(true);
            m_CloseButton.gameObject.SetActive(true);
        }

        List<spellIngredientsData> ingredients = BuildIngredientList();
        m_SelectedHerb = m_SelectedTool = m_SelectedGem = null;
        m_SelectedHerbAmount = m_SelectedToolAmount = m_SelectedGemAmount = 0;

        //send the cast
        Spellcasting.CastSpell(m_SelectedSpell, m_Marker, ingredients,
            (result) => //ON CLICK CONTINUE
            {
                //if success, return to player info
				if (result != null && (result.effect == /*LocalizeLookUp.GetText ("spell_cast_success")*/ "success" || result.effect == /* LocalizeLookUp.GetText ("spell_fizzle")))*/ "fizzle"))
                {
                    FinishSpellcastingFlow();
                }
                else //reopen the UI for a possible retry
                {
                    if (m_Marker.customData != null)
                    {
                        IMarker marker = MarkerManager.GetMarker((m_Marker.customData as Token).instance);
                        if (marker != null)
                            MapCameraUtils.FocusOnMarker(marker.gameObject.transform.position);
                    }
                    ReOpen();
                }

                UpdateCanCast();
            },
            () => //ON CLICK CLOSE
            {
                OnClickClose();
            });
    }

    ////////////////// SPELL INFO
    ///
    public void ShowSpellInfo(SpellDict spellData, SpellData serverData)
    {
        if (spellData == null)
            return;

        m_InfoTitle.text = spellData.spellName;
		m_InfoCost.text = LocalizeLookUp.GetText ("moon_energy").Replace ("{{Amount}}", serverData.cost.ToString());//$"({serverData.cost} Energy)";


        if (PlayerManager.inSpiritForm)
            m_InfoDesc.text = spellData.spellDescription;
        else
            m_InfoDesc.text = spellData.spellDescriptionPhysical;

        m_InfoGroup.alpha = 0;
        m_InfoGroup.blocksRaycasts = true;
        m_InfoGroup.gameObject.SetActive(true);

        LeanTween.cancel(m_InfoTweenId);
        m_InfoTweenId = LeanTween.value(0, 1, 0.7f)//LeanTween.alphaCanvas(m_InfoGroup, 1f, 1.25f).setEaseOutCubic().uniqueId;
            .setEaseOutCubic()
            .setOnUpdate((float t) =>
            {
                m_InfoGroup.alpha = t;
                m_SelectionGroup.alpha = 1 - t;
            })
            .uniqueId;
    }

    private void OnClickCloseInfo()
    {
        m_InfoGroup.blocksRaycasts = false;
        LeanTween.cancel(m_InfoTweenId);
        m_InfoTweenId = LeanTween.value(1, 0, 0.7f)//LeanTween.alphaCanvas(m_InfoGroup, 1f, 1.25f).setEaseOutCubic().uniqueId;
            .setEaseOutCubic()
            .setOnUpdate((float t) =>
            {
                m_InfoGroup.alpha = t;
                m_SelectionGroup.alpha = 1 - t;
            })
            .setOnComplete(() => { m_InfoGroup.gameObject.SetActive(false); })
            .uniqueId;
    }

    ////////////////////// INGREDIENT PICKER
    ///

    private void OnClickInventory()
    {
        if (UIInventory.isOpen)
        {
            UIInventory.Instance.Close();
            m_CloseButton.gameObject.SetActive(true);
            o_InventoryButtonImage.GetComponent<Image>().color = Color.white;

        }
        else
        {
            UIInventory.Instance.Show(OnSelectInventoryItem, null, false, false, false);
            UIInventory.Instance.LockIngredients(m_SelectedSpell.ingredients, 0f);
            m_CloseButton.gameObject.SetActive(false);
            o_InventoryButtonImage.GetComponent<Image>().color = Color.grey;
        }
    }

    private void OnSelectInventoryItem(UIInventoryWheelItem item)
    {
        if (item.itemData == null || item.inventoryItem == null)
        {
            //resets the picker
            item.SetIngredientPicker(0);

            if (item.type == "herb")
            {
                m_SelectedHerb = null;
                m_SelectedHerbAmount = 0;
            }
            if (item.type == "tool")
            {
                m_SelectedTool = null;
                m_SelectedToolAmount = 0;
            }
            if (item.type == "gem")
            {
                m_SelectedGem = null;
                m_SelectedGemAmount = 0;
            }

            UpdateCanCast();
            return;
        }

        List<string> requiredIngredients = m_SelectedSpell.ingredients == null ? new List<string>() : new List<string>(m_SelectedSpell.ingredients);

        int maxAmount = Mathf.Min(5, item.inventoryItem.count);

        if (item.itemData.type == "herb")
        {
            if (m_HerbRequired && requiredIngredients.Contains(item.inventoryItem.id) == false)
                return;

            //set the new selected ingredient
            if (item.inventoryItem != m_SelectedHerb)
            {
                if (m_SelectedHerb == null)
                {
                    m_SelectedHerb = item.inventoryItem;
                    m_SelectedHerbAmount = Mathf.Min(1, maxAmount);
                    item.SetIngredientPicker(m_SelectedHerbAmount);
                }
                else //reset the previous ingredient
                {
                    m_SelectedHerbAmount = 0;
                    UIInventory.Instance.herbsWheel.ResetPicker();
                }
            }
            else //increase the currently selected ingredient
            {
                m_SelectedHerbAmount = (int)Mathf.Repeat(m_SelectedHerbAmount + 1, maxAmount + 1);

                if (m_HerbRequired && m_SelectedHerbAmount == 0)
                    m_SelectedHerbAmount = Mathf.Min(1, maxAmount);

                item.SetIngredientPicker(m_SelectedHerbAmount);
            }

            if (m_SelectedHerbAmount == 0)
                m_SelectedHerb = null;
        }
        else if (item.itemData.type == "tool")
        {
            if (m_ToolRequired && requiredIngredients.Contains(item.inventoryItem.id) == false)
                return;

            if (item.inventoryItem != m_SelectedTool)
            {
                if (m_SelectedTool == null)
                {
                    //m_LastToolItem = item;
                    m_SelectedTool = item.inventoryItem;
                    m_SelectedToolAmount = Mathf.Min(1, maxAmount);
                    item.SetIngredientPicker(m_SelectedToolAmount);
                }
                else
                {
                    m_SelectedToolAmount = 0;
                    UIInventory.Instance.toolsWheel.ResetPicker();
                }
            }
            else
            {
                m_SelectedToolAmount = (int)Mathf.Repeat(m_SelectedToolAmount + 1, maxAmount + 1);

                if (m_ToolRequired && m_SelectedToolAmount == 0)
                    m_SelectedToolAmount = Mathf.Min(1, maxAmount);

                item.SetIngredientPicker(m_SelectedToolAmount);
            }

            if (m_SelectedToolAmount == 0)
                m_SelectedTool = null;
        }
        else //if (item.itemData.type == "gem")
        {
            if (m_GemRequired && requiredIngredients.Contains(item.inventoryItem.id) == false)
                return;

            if (item.inventoryItem != m_SelectedGem)
            {
                if (m_SelectedGem == null)
                {
                    //m_LastGemItem = item;
                    m_SelectedGem = item.inventoryItem;
                    m_SelectedGemAmount = Mathf.Min(1, maxAmount);
                    item.SetIngredientPicker(m_SelectedGemAmount);
                }
                else
                {
                    m_SelectedGemAmount = 0;
                    UIInventory.Instance.gemsWheel.ResetPicker();
                }
            }
            else
            {
                m_SelectedGemAmount = (int)Mathf.Repeat(m_SelectedGemAmount + 1, maxAmount + 1);

                if (m_GemRequired && m_SelectedGemAmount == 0)
                    m_SelectedGemAmount = Mathf.Min(1, maxAmount);

                item.SetIngredientPicker(m_SelectedGemAmount);
            }

            if (m_SelectedGemAmount == 0)
                m_SelectedGem = null;
        }

        UpdateCanCast();
    }

    private void LockIngredients(string[] ingredients)
    {
        m_SelectedHerb = m_SelectedTool = m_SelectedGem = null;
        m_SelectedHerbAmount = m_SelectedToolAmount = m_SelectedGemAmount = 0;

        //reset current ingredients
        for (int i = 0; i < ingredients.Length; i++)
        {
            IngredientType ingrType;
            InventoryItems ingr;
            PlayerDataManager.playerData.ingredients.GetIngredient(ingredients[i], out ingr, out ingrType);

            if (ingrType == IngredientType.herb)
            {
                m_SelectedHerb = ingr;
                m_SelectedHerbAmount = 1;
            }
            else if (ingrType == IngredientType.tool)
            {
                m_SelectedTool = ingr;
                m_SelectedToolAmount = 1;
            }
            else if (ingrType == IngredientType.gem)
            {
                m_SelectedGem = ingr;
                m_SelectedGemAmount = 1;
            }
        }

        //lock the inventory and set the currently selected ingredients
        if (UIInventory.isOpen)
            UIInventory.Instance.LockIngredients(ingredients, .5f);
    }

    private List<spellIngredientsData> BuildIngredientList()
    {
        List<spellIngredientsData> ingredients = new List<spellIngredientsData>();

        if (m_SelectedHerb != null && m_SelectedHerbAmount > 0)
        {
            ingredients.Add(new spellIngredientsData
            {
                id = m_SelectedHerb.id,
                count = m_SelectedHerbAmount
            });
        }

        if (m_SelectedTool != null && m_SelectedToolAmount > 0)
        {
            ingredients.Add(new spellIngredientsData
            {
                id = m_SelectedTool.id,
                count = m_SelectedToolAmount
            });
        }

        if (m_SelectedGem != null && m_SelectedGemAmount > 0)
        {
            ingredients.Add(new spellIngredientsData
            {
                id = m_SelectedGem.id,
                count = m_SelectedGemAmount
            });
        }

        return ingredients;
    }
}