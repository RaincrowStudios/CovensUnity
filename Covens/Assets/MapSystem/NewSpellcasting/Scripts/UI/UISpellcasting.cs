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

    private CollectableItem m_SelectedHerb = null;
    private CollectableItem m_SelectedTool = null;
    private CollectableItem m_SelectedGem = null;
    private int m_SelectedHerbAmount = 0;
    private int m_SelectedToolAmount = 0;
    private int m_SelectedGemAmount = 0;
    public CanvasGroup m_ShadowGlyphBG;
    public CanvasGroup m_GreyGlyphBG;
    public CanvasGroup m_WhiteGlyphBG;
    public Image m_CastGlyphBG;

    private Coroutine m_SetupListCoroutine;
    //private Coroutine m_CooldownCoroutine;
    private int m_HeaderTweenId;

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

        m_ShadowText.text = LocalizeLookUp.GetText("generic_shadow");// "Shadow";
        m_GreyText.text = LocalizeLookUp.GetText("generic_grey");//  "Grey";
        m_WhiteText.text = LocalizeLookUp.GetText("generic_white");//  "White";

        //m_ShadowGlyphBG.alpha = m_GreyGlyphBG.alpha = m_WhiteGlyphBG.alpha = 0;
        
        CooldownManager.OnCooldownEnd += OnCooldownEnd;
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

        LeanTween.alphaCanvas(o_InventoryButtonCG, 0f, 0.2f);
        o_InventoryButtonImage.GetComponent<Image>().color = Color.white;
        o_InventoryButtonTop.SetActive(false);
        m_CloseButton.interactable = false;
        LeanTween.value(0f, 1f, 1f).setOnComplete(() =>
        {
            m_CloseButton.interactable = true;
        });
    }

    public void SetupSpellSelection(int school)
    {
        if (m_SelectedSchool != school)
        {
            m_PreviousSpell = 0;
            m_InfoGroup.alpha = 0;
            m_SelectedSpellOverlay.gameObject.SetActive(false);
            
            Color color;
            List<CanvasGroup> toHide;
            List<CanvasGroup> toShow;

            if (school < 0)
            {
                color = Utilities.Purple;
                m_CastGlyphBG.color = new Color(0.88f, 0.7294f, 1f, 0.545f);
                toShow = new List<CanvasGroup> { m_ShadowGlyphBG };
                toHide = new List<CanvasGroup> { m_GreyGlyphBG, m_WhiteGlyphBG };
            }
            else if (school > 0)
            {
                color = Utilities.Orange;
                m_CastGlyphBG.color = new Color(1f, 0.891f, 0.731f, 0.545f);
                toShow = new List<CanvasGroup> { m_WhiteGlyphBG };
                toHide = new List<CanvasGroup> { m_ShadowGlyphBG, m_GreyGlyphBG };
            }
            else
            {
                color = Utilities.Blue;
                m_CastGlyphBG.color = new Color(0.7294f, 0.8526f, 1f, 0.545f);
                toShow = new List<CanvasGroup> { m_GreyGlyphBG };
                toHide = new List<CanvasGroup> { m_ShadowGlyphBG, m_WhiteGlyphBG };
            }

            color.a = 0.3f;
            m_SelectedSpell_Glow.color = color;

            foreach (CanvasGroup cg in toShow)
                cg.gameObject.SetActive(true);
            foreach (CanvasGroup cg in toHide)
                cg.gameObject.SetActive(false);
            
            //setup spells
            if (m_SetupListCoroutine != null)
            {
                StopCoroutine(m_SetupListCoroutine);
                m_SetupListCoroutine = null;
            }

            //disable buttons
            for (int i = 0; i < m_SpellButtons.Count; i++)
                m_SpellButtons[i].Hide();

            m_SelectedSpellOverlay.gameObject.SetActive(false);

            List<SpellData> spells = new List<SpellData>();
            for (int i = 0; i < m_Spells.Count; i++)
            {
                //if (PlaceOfPower.IsInsideLocation)
                //{
                //    if (m_Spells[i].id == "spell_bind" || m_Spells[i].baseSpell == "spell_bind")
                //        continue;
                //}
                //else
                //{
                //    if (m_Spells[i].popOnly)
                //        continue;
                //}

                if (m_Spells[i].school == school)
                    spells.Add(m_Spells[i]);
            }
            m_SetupListCoroutine = StartCoroutine(SetupSpellList(spells));
        }

        m_PreviousSchool = m_SelectedSchool = school;
    }

    private IEnumerator SetupSpellList(List<SpellData> spells)
    {
        yield return 0;
        for (int i = 0; i < spells.Count; i++)
        {
            if (i >= m_SpellButtons.Count)
                m_SpellButtons.Add(Instantiate(m_SpellEntryPrefab, m_SpellContainer));

            m_SpellButtons[i].Prepare();
        }

        UISpellcastingItem item;
        for (int i = 0; i < spells.Count; i++)
        {
            if (i >= m_SpellButtons.Count)
                m_SpellButtons.Add(Instantiate(m_SpellEntryPrefab, m_SpellContainer));

            item = m_SpellButtons[i];
            int aux = i;
            item.Setup(
                m_Target, 
                m_Marker, 
                spells[i], 
                (_item, spell) => 
                {
                    m_PreviousSpell = aux;
                    OnSelectSpell(_item, spell);
                }
            );

            if (i == m_PreviousSpell)
                item.OnClick();

            item.Show();
            yield return new WaitForSeconds(0.06f);
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
        ShowSpellInfo(m_SelectedSpell);
    }

    public void UpdateCanCast()
    {
        Spellcasting.SpellState canCast = Spellcasting.CanCast(m_SelectedSpell, m_Marker, m_Target);

        m_CastButton.interactable = canCast == Spellcasting.SpellState.CanCast;
        TextMeshProUGUI castText = m_CastButton.GetComponent<TextMeshProUGUI>();

        //if (m_CooldownCoroutine != null)
        //{
        //    StopCoroutine(m_CooldownCoroutine);
        //    m_CooldownCoroutine = null;
        //}

        switch (canCast)
        {
            case Spellcasting.SpellState.CanCast:
                if (BuildIngredientList().Count > 0)
                {
                    castText.text = LocalizeLookUp.GetText("card_witch_cast_ingredients");
                }
                else
                {
                    castText.text = LocalizeLookUp.GetText("card_witch_cast");
                }
                break;

            case Spellcasting.SpellState.TargetImmune:
                castText.text = LocalizeLookUp.GetText("spell_immune_to_you");
                break;

            case Spellcasting.SpellState.PlayerSilenced:
                castText.text = LocalizeLookUp.GetText("ftf_silenced");
                break;

            case Spellcasting.SpellState.MissingIngredients:
                castText.text = LocalizeLookUp.GetText("inventory_missing") + " " + LocalizeLookUp.GetText("store_ingredients");
                break;

            case Spellcasting.SpellState.NotInPop:
                castText.text = LocalizeLookUp.GetText("spell_notinpop");
                break;

            case Spellcasting.SpellState.InCooldown:
                //m_CooldownCoroutine = StartCoroutine(CooldownCoroutine(castText));
                castText.text = LocalizeLookUp.GetText("card_witch_cast");
                break;

            case Spellcasting.SpellState.InvalidState:
                if (m_SelectedSpell.states.Length == 1)
                {
                    if (m_SelectedSpell.states[0] == "dead")
                        castText.text = LocalizeLookUp.GetText("spell_targetnotdead");
                    else// if (states[0] == "vulnerable")
                        castText.text = LocalizeLookUp.GetText("spell_targetnotvulnerable");
                }
                else
                {
                    castText.text = LocalizeLookUp.GetText("spell_targetdead");
                }
                break;

            default:
                string displayname = "yourself";
                if (m_Marker.IsPlayer == false)
                    displayname = m_Target is WitchMarkerDetail ? (m_Target as WitchMarkerDetail).name : LocalizeLookUp.GetSpiritName((m_Target as SpiritMarkerDetail).id);
                castText.text = LocalizeLookUp.GetText("card_witch_cant_cast").Replace("{{target}}", displayname);//  "Can't cast on " + m_Target.displayName;
                break;
        }
        
        foreach (UISpellcastingItem item in m_SpellButtons)
        {
            if (item.Visible == false)
                break;

            item.UpdateCanCast(m_Target, m_Marker);
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
                CollectableItem invItem;
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

        m_SelectedTitle.text = spell.Name;
        m_SelectedCost.text = LocalizeLookUp.GetText("moon_energy").Replace("{{Amount}}", spell.cost.ToString());// $"({spell.cost} Energy)";

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
                if (result != null && result.IsSuccess)
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
    public void ShowSpellInfo(SpellData spell)
    {
        m_InfoTitle.text = spell.Name;
        m_InfoCost.text = LocalizeLookUp.GetText("moon_energy").Replace("{{Amount}}", spell.cost.ToString());//$"({serverData.cost} Energy)";


        if (PlayerManager.inSpiritForm)
            m_InfoDesc.text = spell.SpiritDescription;
        else
            m_InfoDesc.text = spell.PhysicalDescription;

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
        if (item.itemData.type != "?" || item.inventoryItem == null)
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
            if (m_HerbRequired && requiredIngredients.Contains(item.inventoryItem.collectible) == false)
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
            if (m_ToolRequired && requiredIngredients.Contains(item.inventoryItem.collectible) == false)
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
        else if (item.itemData.type == "gem")
        {
            if (m_GemRequired && requiredIngredients.Contains(item.inventoryItem.collectible) == false)
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
        //reset current ingredients
        m_SelectedHerb = m_SelectedTool = m_SelectedGem = null;
        m_SelectedHerbAmount = m_SelectedToolAmount = m_SelectedGemAmount = 0;

        if (ingredients != null)
        {
            for (int i = 0; i < ingredients.Length; i++)
            {
                IngredientType ingrType;
                CollectableItem ingr;
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
                id = m_SelectedHerb.collectible,
                count = m_SelectedHerbAmount
            });
        }

        if (m_SelectedTool != null && m_SelectedToolAmount > 0)
        {
            ingredients.Add(new spellIngredientsData
            {
                id = m_SelectedTool.collectible,
                count = m_SelectedToolAmount
            });
        }

        if (m_SelectedGem != null && m_SelectedGemAmount > 0)
        {
            ingredients.Add(new spellIngredientsData
            {
                id = m_SelectedGem.collectible,
                count = m_SelectedGemAmount
            });
        }

        return ingredients;
    }

    public void OnCooldownEnd(string id)
    {
        if (isOpen == false)
            return;
        if (m_SelectedSpell == null)
            return;
        if (m_SelectedSpell.id != id)
            return;

        UpdateCanCast();
    }

    //private IEnumerator CooldownCoroutine(TextMeshProUGUI castText)
    //{
    //    double timestamp = PlayerManager.m_CooldownDictionary[m_SelectedSpell.id];
    //    while (true)
    //    {
    //        //In cooldown for {{time}}
    //        castText.text = LocalizeLookUp.GetText("spell_incooldown")
    //           .Replace(
    //               "{{time}}",
    //               Utilities.GetSummonTime(timestamp)
    //           );
    //        //castText.text = "cooldown: " + Utilities.GetSummonTime(timestamp);
    //        yield return new WaitForSeconds(1);
    //    }
    //}
}