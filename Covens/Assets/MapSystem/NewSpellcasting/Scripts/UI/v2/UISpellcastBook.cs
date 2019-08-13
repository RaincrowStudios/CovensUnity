using EnhancedUI.EnhancedScroller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Raincrow;
using Raincrow.Maps;
using Raincrow.GameEventResponses;

public class UISpellcastBook : MonoBehaviour, IEnhancedScrollerDelegate
{
    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private RectTransform Container;
    [SerializeField] private GraphicRaycaster m_InputRaycaster;
    [SerializeField] private EnhancedScroller m_Scroller;
    [SerializeField] private CanvasGroup m_ScrollerCanvasGroup;
    [SerializeField] private UISpellcard m_CardPrefab;
    [SerializeField] private Image m_InventoryGlow;

    [SerializeField] private Image m_TargetPortrait;
    [SerializeField] private TextMeshProUGUI m_TargetName;
    [SerializeField] private Image m_TargetEnergy;
    [SerializeField] private RectTransform m_NamePanel;

    [SerializeField] private Button m_CloseButton;
    [SerializeField] private Button m_PortraitButton;
    [SerializeField] private Button m_InventoryButton;

    [SerializeField] private Sprite[] m_TierSprite;

    [SerializeField] private TextMeshProUGUI m_BottomText;


    private static UISpellcastBook m_Instance;

    private List<SpellData> m_PlayerSpells;
    private List<SpellData> m_ScrollerSpells = new List<SpellData>();

    private IMarker m_TargetMarker;
    private CharacterMarkerData m_TargetData;

    private int? m_SelectedSchool = null;
    private SpellData m_SelectedSpell = null;
    private int m_SelectedSpellIndex = 0;
    private CollectableItem m_Herb;
    private CollectableItem m_Tool;
    private CollectableItem m_Gem;

    private System.Action<SpellData, List<spellIngredientsData>> m_OnConfirmSpell;
    private System.Action m_OnBack;
    private System.Action m_OnClose;

    private int m_MoveTweenId;

    public static bool IsOpen
    {
        get
        {
            if (m_Instance == null)
                return false;

            return m_Instance.m_InputRaycaster.enabled;
        }
    }

    public static void Open(
        CharacterMarkerData target,
        IMarker marker,
        List<SpellData> spells,
        System.Action<SpellData, List<spellIngredientsData>> onConfirm,
        System.Action onClickBack = null,
        System.Action onClickClose = null)
    {
        if (m_Instance == null)
        {
            SceneManager.LoadSceneAsync(
                SceneManager.Scene.SPELLCAST_BOOK,
                UnityEngine.SceneManagement.LoadSceneMode.Additive,
                (progress) => { },
                () =>
                {
                    m_Instance.Show(target, marker, spells, onConfirm, onClickBack, onClickClose);
                });
        }
        else
        {
            m_Instance.Show(target, marker, spells, onConfirm, onClickBack, onClickClose);
        }
    }

    public static void Close()
    {
        if (m_Instance == null)
            return;

        m_Instance.Hide();
    }

    private void Awake()
    {
        m_Instance = this;
        m_InventoryButton.interactable = false;
        m_InventoryButton.GetComponent<CanvasGroup>().alpha = 0f;
        m_InventoryGlow.gameObject.SetActive(false);
        m_Scroller.Delegate = this;

        m_Canvas.enabled = false;
        m_InputRaycaster.enabled = false;

        m_InventoryButton.onClick.AddListener(OnClickInventory);
        m_PortraitButton.onClick.AddListener(OnClickPortrait);
        m_CloseButton.onClick.AddListener(OnClickClose);

        DownloadedAssets.OnWillUnloadAssets += DownloadedAssets_OnWillUnloadAssets;
        Container.anchoredPosition = Vector3.right * Container.rect.width;
    }

    private void DownloadedAssets_OnWillUnloadAssets()
    {
        if (IsOpen)
            return;

        DownloadedAssets.OnWillUnloadAssets -= DownloadedAssets_OnWillUnloadAssets;

        m_Scroller.ClearAll();
        SceneManager.UnloadScene(SceneManager.Scene.SPELLCAST_BOOK, null, null);
    }

    private void Show(
        CharacterMarkerData target,
        IMarker marker,
        List<SpellData> spells,
        System.Action<SpellData, List<spellIngredientsData>> onConfirm,
        System.Action onBack = null,
        System.Action onClose = null)
    {
        m_TargetMarker = marker;
        m_TargetData = target;

        m_OnConfirmSpell = onConfirm;
        m_OnBack = onBack;
        m_OnClose = onClose;

        m_PlayerSpells = spells;
        SetSchool(m_SelectedSchool);
        SetupTarget(marker, target);
        SetupBottomText();



        AnimOpen();
        m_Canvas.enabled = true;
        m_InputRaycaster.enabled = true;

        UpdateCanCast();

        SpellCastHandler.OnPlayerTargeted += SpellCastHandler_OnPlayerTargeted;
        OnMapEnergyChange.OnEnergyChange += OnMapEnergyChange_OnEnergyChange;
    }

    private void OnMapEnergyChange_OnEnergyChange(string character, int energy)
    {
        if (character != m_TargetMarker.Token.Id)
            return;

        SetupTargetEnergy(
            energy,
            m_TargetData.baseEnergy,
            m_TargetMarker.Type == MarkerManager.MarkerType.SPIRIT ? new int?() : (m_TargetMarker as WitchMarker).witchToken.degree);

        UpdateCanCast();
    }

    private void SpellCastHandler_OnPlayerTargeted(string attacker, SpellData spell, SpellCastHandler.Result Result)
    {
        if (attacker != m_TargetMarker.Token.Id)
            return;

        UpdateCanCast();
    }
    private void SetupBottomText()
    {
        if (PlayerManager.inSpiritForm == false) //physical
        {
            m_BottomText.text = LocalizeLookUp.GetText("spell_form_physical");
        }
        else //spirit 
        {
            m_BottomText.text = LocalizeLookUp.GetText("spell_form_spirit");
        }
    }

    private void AnimOpen()
    {
        SoundManagerOneShot.Instance.PlayWhisperFX();
        SoundManagerOneShot.Instance.PlayWooshShort();

        LeanTween.cancel(m_MoveTweenId);
        m_MoveTweenId = LeanTween.value(Container.anchoredPosition.x, 0, 0.5f)
            .setEase(LeanTweenType.easeInCubic)
            .setOnUpdate((float x) => Container.anchoredPosition = new Vector3(x, 0, 0))
            .uniqueId;
        //LeanTween.moveLocalX(Container.gameObject, 0f, 0.5f).setEase(LeanTweenType.easeInCubic);
        LeanTween.value(0f, 1f, 0.3f).setOnComplete(() =>
        {
            LeanTween.alphaCanvas(m_ScrollerCanvasGroup, 1f, 1f);
        });
    }
    private void AnimClose()
    {
        SoundManagerOneShot.Instance.PlayWhisperFX();
        LeanTween.alphaCanvas(m_ScrollerCanvasGroup, 0f, 0.4f);
        //LeanTween.moveLocalX(Container.gameObject, 1308, 0.5f).setEase(LeanTweenType.easeInCubic).setOnComplete(() => { m_Canvas.enabled = false; });
        LeanTween.cancel(m_MoveTweenId);
        m_MoveTweenId = LeanTween.value(Container.anchoredPosition.x, Container.rect.width + 50, 0.5f)
            .setEase(LeanTweenType.easeInCubic)
            .setOnUpdate((float x) => Container.anchoredPosition = new Vector3(x, 0, 0))
            .setOnComplete(() => m_Canvas.enabled = false)
            .uniqueId;
    }
    private void Hide()
    {
        AnimClose();
        //m_Canvas.enabled = false;
        m_InputRaycaster.enabled = false;

        m_OnConfirmSpell = null;
        m_OnBack = null;
        m_OnClose = null;

        CloseInventory();

        OnSelectCard(null);
    }



    private void SetupTarget(IMarker marker, CharacterMarkerData data)
    {
        m_TargetName.text = "";
        m_TargetPortrait.overrideSprite = null;

        if (marker.Type == MarkerManager.MarkerType.WITCH)
        {
            WitchMarker witch = marker as WitchMarker;

            m_TargetName.text = witch.witchToken.displayName;

            SetupTargetEnergy(data.energy, data.baseEnergy, witch.witchToken.degree);

            witch.GetPortrait(spr =>
            {
                m_TargetPortrait.overrideSprite = spr;
            });
        }
        else if (marker.Type == MarkerManager.MarkerType.SPIRIT)
        {
            SpiritMarker spirit = marker as SpiritMarker;

            m_TargetName.text = spirit.spiritData.Name;
            SetupTargetEnergy(data.energy, data.baseEnergy, null);

            int idx = Mathf.Clamp(spirit.spiritData.tier - 1, 0, 4);
            m_TargetPortrait.overrideSprite = m_TierSprite[idx];
        }

        LeanTween.value(0, 1, 1f).setOnUpdate((float t) =>
        {
            m_NamePanel.sizeDelta = m_TargetName.rectTransform.sizeDelta;
        });
    }

    private void SetupTargetEnergy(int energy, int baseEnergy, int? school)
    {
        if (school == null)
        {
            m_TargetEnergy.color = Color.white;
        }
        else
        {
            if (school < 0)
                m_TargetEnergy.color = Utilities.Purple;
            else if (school > 0)
                m_TargetEnergy.color = Utilities.Orange;
            else
                m_TargetEnergy.color = Utilities.Blue;
        }

        m_TargetEnergy.fillAmount = (float)energy / baseEnergy;
    }

    private void SetSchool(int? school)
    {
        SoundManagerOneShot.Instance.PlayAHSAWhisper();
        //block the filter change if a card is selected
        if (m_SelectedSpell != null)
            return;

        if (school == m_SelectedSchool && m_ScrollerSpells.Count > 0)
            return;

        m_SelectedSchool = school;

        m_ScrollerSpells.Clear();
        foreach (SpellData spell in m_PlayerSpells)
        {
            if (school == null || school == spell.school)
                m_ScrollerSpells.Add(spell);
        }

        m_Scroller.ReloadData();
    }

    private void OnSelectCard(UISpellcard card)
    {
        SoundManagerOneShot.Instance.PlayButtonTap();
        if (UIInventory.isOpen)
        {
            CloseInventory();
            return;
        }

        if (card == null || m_SelectedSpell != null)
        {
            m_SelectedSpell = null;
        }
        else
        {
            m_SelectedSpell = card.Spell;
            m_SelectedSpellIndex = card.dataIndex;
        }

        //disable unselected cards
        UISpellcard[] cards = m_Scroller.GetComponentsInChildren<UISpellcard>();
        foreach (UISpellcard _card in cards)
        {
            if (m_SelectedSpell == null || _card.Spell.id == m_SelectedSpell.id)
            {
                _card.SetAlpha(1, 1);
            }
            else
            {
                _card.SetAlpha(0.15f, 1);
            }
        }

        SetIngredients(m_SelectedSpell == null ? null : m_SelectedSpell.ingredients);
        EnableInventoryButton(m_SelectedSpell != null);

        if (m_SelectedSpell != null)
        {
            m_Scroller.ScrollRect.enabled = true;
            FocusOn(m_SelectedSpellIndex, 0.5f, () => m_Scroller.ScrollRect.enabled = false);
        }
        else
        {
            m_Scroller.ScrollRect.enabled = true;
        }
    }

    private void UpdateCanCast(string spell = null)
    {
        UISpellcard[] cards = m_Scroller.GetComponentsInChildren<UISpellcard>();
        foreach (UISpellcard card in cards)
        {
            if (spell != null && spell != card.Spell.id)
                continue;

            card.UpdateCancast(m_TargetData, m_TargetMarker);
        }
    }

    private void OnClickCast(UISpellcard card)
    {
        SoundManagerOneShot.Instance.PlayEnYaSa();
        //in case the player clicked the glyph without first selecting a card
        if (m_SelectedSpell == null || m_SelectedSpell.id != card.Spell.id)
        {
            SetIngredients(card.Spell.ingredients);
        }

        List<spellIngredientsData> ingredients = new List<spellIngredientsData>();

        if (string.IsNullOrEmpty(m_Herb.id) == false)
            ingredients.Add(new spellIngredientsData(m_Herb.id, m_Herb.count));
        if (string.IsNullOrEmpty(m_Tool.id) == false)
            ingredients.Add(new spellIngredientsData(m_Tool.id, m_Tool.count));
        if (string.IsNullOrEmpty(m_Gem.id) == false)
            ingredients.Add(new spellIngredientsData(m_Gem.id, m_Gem.count));

        m_OnConfirmSpell?.Invoke(card.Spell, ingredients);
        Hide();
    }

    private void OnSelectSchool(int school)
    {
        SoundManagerOneShot.Instance.PlayWhisperFX();
        SoundManagerOneShot.Instance.PlayButtonTap();

        //disable filter while ivnentory picker is open
        if (UIInventory.isOpen)
            return;

        if (m_SelectedSchool == null || m_SelectedSchool != school)
            SetSchool(school);
        else
            SetSchool(null);
    }

    private void OnClickPortrait()
    {
        m_OnBack?.Invoke();
        Hide();
    }

    private void OnClickClose()
    {
        m_OnClose?.Invoke();
        Hide();
    }

    #region INVENTORY

    private void OnClickInventory()
    {

        if (UIInventory.isOpen)
            CloseInventory();
        else
            OpenInventory();
    }

    private void EnableInventoryButton(bool enabled)
    {
        m_InventoryButton.interactable = enabled;
        m_InventoryGlow.gameObject.SetActive(m_Herb.id != null || m_Tool.id != null || m_Gem.id != null);
        if (enabled == true)
        {
            LeanTween.alphaCanvas(m_InventoryButton.GetComponent<CanvasGroup>(), 1f, 0.5f);
        }
        else
        {
            m_InventoryButton.GetComponent<CanvasGroup>().alpha = 0f;
        }

    }

    private void OpenInventory()
    {
        SoundManagerOneShot.Instance.PlayWhisperFX();
        UIInventory.Instance.Show(OnClickInventoryItem, OnCloseInventory, false, false);

        //lock if necessary
        UIInventory.Instance.LockIngredients(m_SelectedSpell.ingredients, 0);

        //set the ivnentory with the current ingredients
        List<CollectableItem> selected = new List<CollectableItem>()
        {
            m_Herb, m_Tool, m_Gem
        };
        UIInventory.Instance.SetSelected(selected);

        OnOpenInventory();
    }

    private void CloseInventory()
    {
        SoundManagerOneShot.Instance.PlayButtonTap();
        if (UIInventory.isOpen == false)
            return;

        UIInventory.Instance.Close();
        OnCloseInventory();
    }

    private void OnOpenInventory()
    {
        SoundManagerOneShot.Instance.PlayWhisperFX();
        SoundManagerOneShot.Instance.PlayButtonTap();
        //lock scroller
        m_Scroller.ScrollRect.enabled = false;

        //move scroller to the right
        FocusOn(m_SelectedSpellIndex, 0.75f);
    }

    private void OnCloseInventory()
    {
        SoundManagerOneShot.Instance.PlayButtonTap();
        EnableInventoryButton(true);

        //unlock scroller
        m_Scroller.ScrollRect.enabled = true;

        //move scroller to center
        FocusOn(m_SelectedSpellIndex);
    }

    private void OnClickInventoryItem(UIInventoryWheelItem item)
    {
        SoundManagerOneShot.Instance.PlayButtonTap();
        //just resets if clicking on an empty inventory item
        if (item.inventoryItemId == null)
        {
            //resets the picker
            item.SetIngredientPicker(0);
            if (item.type == "herb")
                m_Herb = new CollectableItem();
            else if (item.type == "tool")
                m_Tool = new CollectableItem();
            else if (item.type == "gem")
                m_Gem = new CollectableItem();
            return;
        }

        //List<string> required = m_SelectedSpell.ingredients == null ? new List<string>() : new List<string>(m_SelectedSpell.ingredients);
        IngredientData itemData = item.itemData;
        int maxAmount = Mathf.Min(5, PlayerDataManager.playerData.GetIngredient(item.inventoryItemId));

        if (itemData.Type == IngredientType.herb)
        {
            if (string.IsNullOrEmpty(m_Herb.id))
            {
                //select the ingredient
                m_Herb = new CollectableItem
                {
                    id = item.inventoryItemId,
                    count = Mathf.Min(1, maxAmount)
                };
                item.SetIngredientPicker(m_Herb.count);
            }
            else if (m_Herb.id != item.inventoryItemId)
            {
                //unselect the previous ingredient
                m_Herb = new CollectableItem();
                UIInventory.Instance.herbsWheel.ResetPicker();
            }
            else
            {
                //increment the selected ingredient
                m_Herb.count = (int)Mathf.Repeat(m_Herb.count + 1, maxAmount + 1);
                m_Herb.count = Mathf.Clamp(m_Herb.count, 1, maxAmount);
                item.SetIngredientPicker(m_Herb.count);
            }
        }
        else if (itemData.Type == IngredientType.tool)
        {
            if (string.IsNullOrEmpty(m_Tool.id))
            {
                m_Tool = new CollectableItem
                {
                    id = item.inventoryItemId,
                    count = Mathf.Min(1, maxAmount)
                };
                item.SetIngredientPicker(m_Tool.count);
            }
            else if (m_Tool.id != item.inventoryItemId)
            {
                m_Tool = new CollectableItem();
                UIInventory.Instance.toolsWheel.ResetPicker();
            }
            else
            {
                m_Tool.count = (int)Mathf.Repeat(m_Tool.count + 1, maxAmount + 1);
                m_Tool.count = Mathf.Clamp(m_Tool.count, 1, maxAmount);
                item.SetIngredientPicker(m_Tool.count);
            }
        }
        else if (itemData.Type == IngredientType.gem)
        {
            if (string.IsNullOrEmpty(m_Gem.id))
            {
                m_Gem = new CollectableItem
                {
                    id = item.inventoryItemId,
                    count = Mathf.Min(1, maxAmount)
                };
                item.SetIngredientPicker(m_Gem.count);
            }
            else if (m_Gem.id != item.inventoryItemId)
            {
                m_Gem = new CollectableItem();
                UIInventory.Instance.gemsWheel.ResetPicker();
            }
            else
            {
                m_Gem.count = (int)Mathf.Repeat(m_Gem.count + 1, maxAmount + 1);
                m_Gem.count = Mathf.Clamp(m_Gem.count, 1, maxAmount);
                item.SetIngredientPicker(m_Gem.count);
            }
        }
    }

    private void SetIngredients(string[] ingredients)
    {
        m_Herb.id = m_Tool.id = m_Gem.id = null;
        m_Herb.count = m_Tool.count = m_Gem.count = 0;

        if (ingredients != null)
        {
            for (int i = 0; i < ingredients.Length; i++)
            {
                IngredientType ingrType = DownloadedAssets.GetCollectable(ingredients[i]).Type;

                if (ingrType == IngredientType.herb)
                {
                    m_Herb = new CollectableItem
                    {
                        id = ingredients[i],
                        count = 1
                    };
                }
                else if (ingrType == IngredientType.tool)
                {
                    m_Tool = new CollectableItem
                    {
                        id = ingredients[i],
                        count = 1
                    };
                }
                else if (ingrType == IngredientType.gem)
                {
                    m_Gem = new CollectableItem
                    {
                        id = ingredients[i],
                        count = 1
                    };
                }
            }
        }
    }

    #endregion

    #region SCROLLER

    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        UISpellcard cellView = scroller.GetCellView(m_CardPrefab) as UISpellcard;
        cellView.SetData(m_ScrollerSpells[dataIndex], OnSelectSchool, OnSelectCard, OnClickCast);

        cellView.UpdateCancast(m_TargetData, m_TargetMarker);
        if (m_SelectedSpell == null || m_ScrollerSpells[dataIndex].id == m_SelectedSpell.id)
        {
            cellView.SetAlpha(1);

        }
        else
        {
            cellView.SetAlpha(0.15f);
        }

        return cellView;
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return 300;
    }

    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        return m_ScrollerSpells.Count;
    }

    public void FocusOn(int dataIndex, float offset = 0.5f, System.Action onComplete = null)
    {
        m_Scroller.JumpToDataIndex(
            dataIndex: dataIndex,
            scrollerOffset: offset,
            cellOffset: 0.5f,
            tweenType: EnhancedScroller.TweenType.easeOutCubic,
            tweenTime: 0.5f,
            jumpComplete: onComplete);
    }

    #endregion
}
