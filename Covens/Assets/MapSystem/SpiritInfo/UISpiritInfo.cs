using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Raincrow.Maps;
using Raincrow.GameEventResponses;

public class UISpiritInfo : UIInfoPanel
{
    [SerializeField] private UIConditionList m_ConditionList;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI m_SpiritName;
    [SerializeField] private TextMeshProUGUI m_Tier;
    [SerializeField] private TextMeshProUGUI m_Energy;
    [SerializeField] private TextMeshProUGUI m_Desc;
    [SerializeField] private TextMeshProUGUI m_CastText;

    [Header("Buttons")]
    [SerializeField] private Button m_InfoButton;
    [SerializeField] private Button m_DescButton;
    [SerializeField] private Button m_QuickBless;
    [SerializeField] private Button m_QuickSeal;
    [SerializeField] private Button m_QuickHex;
    [SerializeField] private Button m_CastButton;
    [SerializeField] private Button m_BackButton;
    [SerializeField] private Button m_CloseButton;

    private static UISpiritInfo m_Instance;
    public static UISpiritInfo Instance
    {
        get
        {
            if (m_Instance == null)
                m_Instance = Instantiate(Resources.Load<UISpiritInfo>("UISpiritInfo"));
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

    private SpiritMarker m_Spirit;
    private SpiritToken m_Token;
    private SpiritData m_SpiritData;
    private SelectSpiritData_Map m_Details;

    private float m_PreviousMapZoom;

    public SpiritToken Spirit { get { return m_Token; } }

    protected override void Awake()
    {
        base.Awake();

        m_CastButton.onClick.AddListener(OnClickCast);
        m_BackButton.onClick.AddListener(OnClickBack);
        m_CloseButton.onClick.AddListener(OnClickClose);
        m_InfoButton.onClick.AddListener(OnClickInfo);

        m_QuickBless.onClick.AddListener(() => QuickCast("spell_bless"));
        m_QuickHex.onClick.AddListener(() => QuickCast("spell_hex"));
        m_QuickSeal.onClick.AddListener(() => QuickCast("spell_seal"));
    }

    public void Show(IMarker spirit, Token token)
    {
        if (isOpen)
            return;

        m_Spirit = spirit as SpiritMarker;
        m_Token = token as SpiritToken;
        m_SpiritData = DownloadedAssets.spiritDict[m_Token.spiritId];
        m_Details = null;
        m_DescButton.onClick.RemoveAllListeners();

        m_SpiritName.text = m_SpiritData.Name;
        m_Energy.text = LocalizeLookUp.GetText("cast_energy").ToUpper() + " <color=black>" + m_Token.energy.ToString() + " / " + m_Token.baseEnergy.ToString() + "</color>";
        m_Desc.text = LocalizeLookUp.GetText("location_owned").Replace("{{Controller}}", "[" + LocalizeLookUp.GetText("loading") + "]");//"Belongs to [Loading...]";

        string tier;
        switch (m_SpiritData.tier)
        {
            case 1: tier = LocalizeLookUp.GetText("cast_spirit_lesser"); break;
            case 2: tier = LocalizeLookUp.GetText("cast_spirit_greater"); break;
            case 3: tier = LocalizeLookUp.GetText("cast_spirit_superior"); break;
            case 4: tier = LocalizeLookUp.GetText("cast_spirit_legendary"); break;
            default: tier = "?"; break;
        };
        m_Tier.text = tier;
                        
        previousMapPosition = MapsAPI.Instance.GetWorldPosition();
        m_PreviousMapZoom = MapsAPI.Instance.normalizedZoom;

        MainUITransition.Instance.HideMainUI();

        MarkerSpawner.HighlightMarker(new List<IMarker> { PlayerManager.marker, m_Spirit }, true);

        OnMapEnergyChange.OnPlayerDead += _OnCharacterDead;
        OnMapEnergyChange.OnEnergyChange += _OnMapEnergyChange;
        MoveTokenHandler.OnTokenMove += _OnMapTokenMove;
        SpellCastHandler.OnApplyStatusEffect += _OnStatusEffectApplied;
        MarkerSpawner.OnImmunityChange += _OnImmunityChange;
        RemoveTokenHandler.OnTokenRemove += _OnMapTokenRemove;
        BanishManager.OnBanished += Abort;

        Show();
        m_ConditionList.show = false;
        SoundManagerOneShot.Instance.PlaySpiritSelectedSpellbook();
    }

    public override void ReOpen()
    {
        base.ReOpen();

        UpdateCanCast();

        MapsAPI.Instance.allowControl = false;

        IMarker spirit = MarkerManager.GetMarker(m_Token.instance);

        //if the spirit was destroyed, close the ui
        if (spirit != null)
            MapCameraUtils.FocusOnMarker(spirit.gameObject.transform.position);
        else
            Close();
    }

    public override void Close()
    {
        base.Close();

        OnMapEnergyChange.OnPlayerDead -= _OnCharacterDead;
        OnMapEnergyChange.OnEnergyChange -= _OnMapEnergyChange;
        MoveTokenHandler.OnTokenMove -= _OnMapTokenMove;
        MarkerSpawner.OnImmunityChange -= _OnImmunityChange;
        SpellCastHandler.OnApplyStatusEffect -= _OnStatusEffectApplied;
        RemoveTokenHandler.OnTokenRemove -= _OnMapTokenRemove;
        BanishManager.OnBanished -= Abort;

        MapsAPI.Instance.allowControl = true;
        MapCameraUtils.FocusOnPosition(previousMapPosition, m_PreviousMapZoom, true);
        MainUITransition.Instance.ShowMainUI();

        MarkerSpawner.HighlightMarker(new List<IMarker> { PlayerManager.marker, m_Spirit }, false);
    }

    public void SetupDetails(SelectSpiritData_Map details)
    {
        if (details == null)
        {
            Abort();
            return;
        }

        m_Details = details;
        
        if (string.IsNullOrEmpty(m_Details.owner))
        {
            if (m_SpiritData.tier == 1)
                m_Tier.text = LocalizeLookUp.GetText("ftf_wild_spirit") + " (" + LocalizeLookUp.GetText("cast_spirit_lesser") + ")";//"Wild Spirit (Lesser)";
            else if (m_SpiritData.tier == 2)
                m_Tier.text = LocalizeLookUp.GetText("ftf_wild_spirit") + " (" + LocalizeLookUp.GetText("cast_spirit_greater") + ")";//"Wild Spirit (Greater)";
            else if (m_SpiritData.tier == 3)
                m_Tier.text = LocalizeLookUp.GetText("ftf_wild_spirit") + " (" + LocalizeLookUp.GetText("cast_spirit_superior") + ")";//"Wild Spirit (Superior)";
            else
                m_Tier.text = LocalizeLookUp.GetText("ftf_wild_spirit") + " (" + LocalizeLookUp.GetText("cast_spirit_legendary") + ")";//"Wild Spirit (Legendary)";

            m_Desc.text = LocalizeLookUp.GetText("cast_spirit_knowledge");// "Defeating this spirit will give you the power to summon it.";
        }
        else
        {
            if (string.IsNullOrEmpty(details.coven))
            {
                m_Desc.text = LocalizeLookUp.GetText("location_owned").Replace("{{Controller}}", "<color=black>" + details.owner + "</color>");
                m_DescButton.onClick.AddListener(OnClickOwner);
            }
            else
            {
                m_Desc.text = LocalizeLookUp.GetText("location_owned").Replace("{{Controller}}", LocalizeLookUp.GetText("leaderboard_coven") + " <color=black>" + details.coven + "</color>");
                m_DescButton.onClick.AddListener(OnClickCoven);
            }
        }

        UpdateCanCast();
        m_ConditionList.Setup(m_Details.effects);
    }

    private void UpdateCanCast()
    {
        if (m_Details == null)
        {
            m_QuickBless.interactable = m_QuickHex.interactable = m_QuickSeal.interactable = m_CastButton.interactable = false;
            m_CastText.text = LocalizeLookUp.GetText("spellbook_more_spells") + " (" + LocalizeLookUp.GetText("loading") + ")";
            return;
        }

        Spellcasting.SpellState canCast = Spellcasting.CanCast((SpellData)null, m_Spirit, m_Details);


        if (canCast == Spellcasting.SpellState.TargetImmune)
            m_CastText.text = LocalizeLookUp.GetText("spell_immune_to_you");//"Player is immune to you";
        else if (canCast == Spellcasting.SpellState.PlayerSilenced)
            m_CastText.text = LocalizeLookUp.GetText("ftf_silenced");//You are silenced";
        else
            m_CastText.text = LocalizeLookUp.GetText("spellbook_more_spells");//More Spells";

        m_QuickBless.interactable = m_QuickHex.interactable = m_QuickSeal.interactable = m_CastButton.interactable = canCast == Spellcasting.SpellState.CanCast;

        if (UISpellcasting.isOpen)
            UISpellcasting.Instance.UpdateCanCast();
    }

    private void OnClickCast()
    {
        this.Hide();

        UISpellcasting.Instance.Show(m_Details, m_Spirit, PlayerDataManager.playerData.Spells,
            UISpellcasting_OnCastResult,
            ReOpen,
            UISpellcasting_OnClickClose);
    }

    private void QuickCast(string spellId)
    {
        SpellData spell = DownloadedAssets.GetSpell(spellId);

        if (spell == null)
            return;

        //StreetMapUtils.FocusOnTarget(PlayerManager.marker);

        Hide();

        //send the cast
        Spellcasting.CastSpell(spell, m_Spirit, new List<spellIngredientsData>(), (result) =>
        {
            //return to the spirit UI no matter the result
            ReOpen();
        },
        () =>
        {
            OnClickClose();
        });
    }

    private void OnClickBack()
    {
        Close();
    }

    private void OnClickClose()
    {
        Close();
    }

    private void OnClickInfo()
    {
        UIDetailedSpiritInfo.Instance.Show(m_SpiritData, m_Token);
    }

    private void OnClickOwner()
    {
        Debug.Log("TODO: go to player");
    }

    private void OnClickCoven()
    {
        Debug.Log("TODO: Open coven");
    }
    
    private void Abort()
    {
        //wait for the result screen (UIspellcasting  will call OnFinishFlow)
        if (UIWaitingCastResult.isOpen)
            return;

        if (UISpellcasting.isOpen)
            UISpellcasting.Instance.Close();

        Close();
    }

    private void UISpellcasting_OnCastResult()
    {
        //if token is gone
        if (MarkerSpawner.GetMarker(m_Token.instance) == null)
        {
            Close();
            UISpellcasting.Instance.Close();
        }
    }

    private void UISpellcasting_OnClickClose()
    {
        //close this too
        Close();
    }

    private void _OnCharacterDead()
    {
        Abort();
    }

    private void _OnMapTokenMove(string instance, Vector3 position)
    {
        if (m_Token.instance == instance)
        {
            MapCameraUtils.FocusOnMarker(position);
        }
    }

    private void _OnMapEnergyChange(string instance, int newEnergy)
    {
        if (instance == m_Token.instance)
        {
            m_Energy.text = LocalizeLookUp.GetText("card_witch_energy").ToUpper() + " <color=black>" + newEnergy.ToString() + "</color>";

            if (newEnergy == 0)
            {
                //let the player see the result of his spellcasting
                if (UIWaitingCastResult.isOpen)
                    return;

                //if he is not waiting for the result, just close the ui
                Abort();
            }
        }
    }

    private void _OnImmunityChange(string caster, string target, bool immune)
    {
        if (caster == PlayerDataManager.playerData.instance && target == this.m_Token.instance)
        {
            UpdateCanCast();
        }
        else if (target == PlayerDataManager.playerData.instance && caster == this.m_Token.instance)
        {
            UpdateCanCast();
        }
    }

    private void _OnStatusEffectApplied(string character, StatusEffect statusEffect)
    {
        if (character != this.m_Token.instance)
            return;

        foreach(StatusEffect item in m_Details.effects)
        {
            if (item.spell == statusEffect.spell)
            {
                m_Details.effects.Remove(item);
                break;
            }
        }
        m_Details.effects.Add(statusEffect);

        m_ConditionList.AddCondition(statusEffect);
    }

    private void _OnMapTokenRemove(string instance)
    {
        if (instance == Spirit.instance)
        {
            Abort();
            //UIGlobalErrorPopup.ShowPopUp(null, LocalizeLookUp.GetText("spellbook_witch_is_gone").Replace("{{witch name}}", m_SpiritData.spiritName));// + " is gone.");
        }
    }
}
