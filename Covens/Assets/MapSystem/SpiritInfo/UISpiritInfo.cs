using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Raincrow.Maps;
using Raincrow.GameEventResponses;

public class UISpiritInfo : UIInfoPanel
{
    [SerializeField] private UIQuickCast m_CastMenu;
    [SerializeField] private UIConditionList m_ConditionList;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI m_SpiritName;
    [SerializeField] private TextMeshProUGUI m_Tier;
    [SerializeField] private TextMeshProUGUI m_Energy;
    [SerializeField] private TextMeshProUGUI m_Desc;

    [Header("Buttons")]
    [SerializeField] private Button m_InfoButton;
    [SerializeField] private Button m_DescButton;
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

    private SpiritMarker m_SpiritMarker;
    private SpiritToken m_SpiritToken;
    private SpiritData m_SpiritData;
    private SelectSpiritData_Map m_SpiritDetails;

    private float m_PreviousMapZoom;

    public SpiritToken SpiritToken { get { return m_SpiritToken; } }

    protected override void Awake()
    {
        base.Awake();

        m_BackButton.onClick.AddListener(OnClickBack);
        m_CloseButton.onClick.AddListener(OnClickClose);
        m_InfoButton.onClick.AddListener(OnClickInfo);

        m_CastMenu.OnClickCast = OnClickCast;
        m_CastMenu.OnQuickCast = (spell) => QuickCast(spell);
    }

    public void Show(IMarker spirit, Token token)
    {
        if (isOpen)
            return;

        m_SpiritMarker = spirit as SpiritMarker;
        m_SpiritToken = token as SpiritToken;
        m_SpiritData = DownloadedAssets.spiritDict[m_SpiritToken.spiritId];
        m_SpiritDetails = null;
        m_DescButton.onClick.RemoveAllListeners();

        m_SpiritName.text = m_SpiritData.Name;
        m_Energy.text = LocalizeLookUp.GetText("cast_energy").ToUpper() + " <color=black>" + m_SpiritToken.energy.ToString() + " / " + m_SpiritToken.baseEnergy.ToString() + "</color>";
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
        m_PreviousMapZoom = Mathf.Min(0.98f, MapsAPI.Instance.normalizedZoom);


        OnMapEnergyChange.OnPlayerDead += _OnCharacterDead;
        OnMapEnergyChange.OnEnergyChange += _OnMapEnergyChange;

        Show();
        m_ConditionList.show = false;
        SoundManagerOneShot.Instance.PlaySpiritSelectedSpellbook();

        if (!LocationIslandController.isInBattle)
        {
            MainUITransition.Instance.HideMainUI();
            MarkerSpawner.HighlightMarker(new List<IMarker> { PlayerManager.marker, m_SpiritMarker });
            MoveTokenHandler.OnTokenMove += _OnMapTokenMove;
            SpellCastHandler.OnApplyStatusEffect += _OnStatusEffectApplied;
            MarkerSpawner.OnImmunityChange += _OnImmunityChange;
            RemoveTokenHandler.OnTokenRemove += _OnMapTokenRemove;
            BanishManager.OnBanished += Abort;
        }

    }

    public override void ReOpen()
    {
        base.ReOpen();

        UpdateCanCast();
        IMarker spirit = MarkerManager.GetMarker(m_SpiritToken.instance);

        if (!LocationIslandController.isInBattle)
        {
            MapsAPI.Instance.allowControl = false;
            //if the spirit was destroyed, close the ui
            if (spirit != null)
                MapCameraUtils.FocusOnMarker(spirit.GameObject.transform.position);
            else
                Close();
        }
        else
        {
            if (spirit == null) Close();
        }
    }

    public override void Close()
    {
        base.Close();

        OnMapEnergyChange.OnPlayerDead -= _OnCharacterDead;
        OnMapEnergyChange.OnEnergyChange -= _OnMapEnergyChange;

        if (!LocationIslandController.isInBattle)
        {
            MoveTokenHandler.OnTokenMove -= _OnMapTokenMove;
            MarkerSpawner.OnImmunityChange -= _OnImmunityChange;
            SpellCastHandler.OnApplyStatusEffect -= _OnStatusEffectApplied;
            RemoveTokenHandler.OnTokenRemove -= _OnMapTokenRemove;
            BanishManager.OnBanished -= Abort;

            MapsAPI.Instance.allowControl = true;
            MapCameraUtils.FocusOnPosition(MapsAPI.Instance.mapCenter.position, m_PreviousMapZoom, true);
            MainUITransition.Instance.ShowMainUI();
            MarkerSpawner.HighlightMarker(new List<IMarker> { });
        }
        else
        {
            LocationUnitSpawner.EnableMarkers();
        }

    }

    public void SetupDetails(SelectSpiritData_Map details)
    {
        if (details == null)
        {
            Abort();
            return;
        }

        m_SpiritDetails = details;

        if (string.IsNullOrEmpty(m_SpiritDetails.owner))
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
        m_ConditionList.Setup(m_SpiritDetails.effects);
    }

    private void UpdateCanCast()
    {
        if (m_SpiritDetails == null)
        {
            m_CastMenu.UpdateCanCast(null, null);
            return;
        }

        m_CastMenu.UpdateCanCast(m_SpiritDetails, m_SpiritMarker);
    }

    private void OnClickCast()
    {
        this.Hide();

        List<SpellData> spells = new List<SpellData>(PlayerDataManager.playerData.Spells);
        spells.RemoveAll(spell => spell.target == SpellData.Target.SELF);

        UISpellcastBook.Open(
            m_SpiritDetails,
            m_SpiritMarker,
            spells,
            (spell, ingredients) =>
            {
                Spellcasting.CastSpell(spell, m_SpiritMarker, ingredients,
                    (result) => ReOpen(),
                    () => OnClickClose()
                );
            },
            () => this.ReOpen(),
            () => Close()
        );
    }

    private void QuickCast(string spellId)
    {
        SpellData spell = DownloadedAssets.GetSpell(spellId);

        if (spell == null)
            return;

        //StreetMapUtils.FocusOnTarget(PlayerManager.marker);

        Hide();

        //send the cast
        Spellcasting.CastSpell(spell, m_SpiritMarker, new List<spellIngredientsData>(), (result) =>
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
        UIDetailedSpiritInfo.Instance.Show(m_SpiritData, m_SpiritToken);
    }

    private void OnClickOwner()
    {
        Debug.LogError("TODO: go to player");
    }

    private void OnClickCoven()
    {
        TeamManagerUI.OpenName(m_SpiritDetails.coven);
    }

    private void Abort()
    {
        //wait for the result screen (UIspellcasting  will call OnFinishFlow)
        if (UIWaitingCastResult.isOpen)
            return;

        Close();
    }

    private void UISpellcasting_OnCastResult()
    {
        //if token is gone
        if (MarkerSpawner.GetMarker(m_SpiritToken.instance) == null)
        {
            Close();
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
        if (m_SpiritToken.instance == instance)
        {
            MapCameraUtils.FocusOnMarker(position);
        }
    }

    private void _OnMapEnergyChange(string instance, int newEnergy)
    {
        if (instance == m_SpiritToken.instance)
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
        if (caster == PlayerDataManager.playerData.instance && target == this.m_SpiritToken.instance)
        {
            UpdateCanCast();
        }
        else if (target == PlayerDataManager.playerData.instance && caster == this.m_SpiritToken.instance)
        {
            UpdateCanCast();
        }
    }

    private void _OnStatusEffectApplied(string character, StatusEffect statusEffect)
    {
        if (character != this.m_SpiritToken.instance)
            return;

        foreach (StatusEffect item in m_SpiritDetails.effects)
        {
            if (item.spell == statusEffect.spell)
            {
                m_SpiritDetails.effects.Remove(item);
                break;
            }
        }
        m_SpiritDetails.effects.Add(statusEffect);

        m_ConditionList.AddCondition(statusEffect);
    }

    private void _OnMapTokenRemove(string instance)
    {
        if (instance == SpiritToken.instance)
        {
            Abort();
            //UIGlobalErrorPopup.ShowPopUp(null, LocalizeLookUp.GetText("spellbook_witch_is_gone").Replace("{{witch name}}", m_SpiritData.spiritName));// + " is gone.");
        }
    }
}
