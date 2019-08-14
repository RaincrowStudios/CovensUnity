using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIQuickCast : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_CastText;

    [Header("Buttons")]
    [SerializeField] private Button m_MoreSpells;
    [SerializeField] private Button m_QuickBless;
    [SerializeField] private Button m_QuickSeal;
    [SerializeField] private Button m_QuickHex;

    [Header("Cooldown")]
    [SerializeField] private Image m_BlessCooldown;
    [SerializeField] private Image m_SealCooldown;
    [SerializeField] private Image m_HexCooldown;

    public System.Action OnClickCast;
    public System.Action<string> OnQuickCast;

    private int m_BlessTweenId;
    private int m_SealTweenId;
    private int m_HexTweenId;

    private string m_PreviousMarker = "";

    private void Awake()
    {
        m_MoreSpells.onClick.AddListener(() => OnClickCast?.Invoke());

        m_QuickBless.onClick.AddListener(() => OnQuickCast?.Invoke("spell_bless"));
        m_QuickHex.onClick.AddListener(() => OnQuickCast?.Invoke("spell_hex"));
        m_QuickSeal.onClick.AddListener(() => OnQuickCast?.Invoke("spell_seal"));
    }

    public void UpdateCanCast(CharacterMarkerData data, IMarker marker)
    {
        if (data == null || marker == null)
        {
            m_QuickBless.interactable = m_QuickHex.interactable = m_QuickSeal.interactable = m_MoreSpells.interactable = false;
            m_CastText.text = LocalizeLookUp.GetText("spellbook_more_spells") + " (" + LocalizeLookUp.GetText("loading") + ")";
        }

        //quick cast
        Spellcasting.SpellState hexStatus = Spellcasting.CanCast("spell_hex", marker, data);
        Spellcasting.SpellState sealStatus = Spellcasting.CanCast("spell_seal", marker, data);
        Spellcasting.SpellState blesStatus = Spellcasting.CanCast("spell_bless", marker, data);

        m_QuickHex.interactable = hexStatus == Spellcasting.SpellState.CanCast;
        m_QuickSeal.interactable = sealStatus == Spellcasting.SpellState.CanCast;
        m_QuickBless.interactable = blesStatus == Spellcasting.SpellState.CanCast;

        //quickcast cooldown
        LeanTween.cancel(m_BlessTweenId);
        LeanTween.cancel(m_HexTweenId);
        LeanTween.cancel(m_SealTweenId);

        CooldownManager.Cooldown? cd;

        //hex
        cd = CooldownManager.GetCooldown("spell_hex");
        if (cd.HasValue)
        {
            m_HexTweenId = LeanTween.value(cd.Value.Remaining / cd.Value.total, 0, cd.Value.Remaining)
                .setOnUpdate((float t) => m_HexCooldown.fillAmount = t)
                .setOnComplete(() => UpdateCanCast(data, marker))
                .uniqueId;
        }
        else
        {
            m_HexCooldown.fillAmount = 0;
        }

        //seal
        cd = CooldownManager.GetCooldown("spell_seal");
        if (cd.HasValue)
        {
            m_SealTweenId = LeanTween.value(cd.Value.Remaining / cd.Value.total, 0, cd.Value.Remaining)
                .setOnUpdate((float t) => m_SealCooldown.fillAmount = t)
                .setOnComplete(() => UpdateCanCast(data, marker))
                .uniqueId;
        }
        else
        {
            m_SealCooldown.fillAmount = 0;
        }

        //bless
        cd = CooldownManager.GetCooldown("spell_bless");
        if (cd.HasValue)
        {
            m_BlessTweenId = LeanTween.value(cd.Value.Remaining / cd.Value.total, 0, cd.Value.Remaining)
                .setOnUpdate((float t) => m_BlessCooldown.fillAmount = t)
                .setOnComplete(() => UpdateCanCast(data, marker))
                .uniqueId;
        }
        else
        {
            m_BlessCooldown.fillAmount = 0;
        }

        //
        Spellcasting.SpellState canCast = Spellcasting.CanCast((SpellData)null, marker, data);
        m_MoreSpells.interactable = canCast == Spellcasting.SpellState.CanCast;

        if (canCast == Spellcasting.SpellState.TargetImmune)
        {
            m_CastText.text = LocalizeLookUp.GetText("spell_immune_to_you");
            if (m_PreviousMarker != marker.Token.instance)
            {
                SoundManagerOneShot.Instance.WitchImmune();
                m_PreviousMarker = marker.Token.instance;
            }
        }
        else if (canCast == Spellcasting.SpellState.PlayerSilenced)
        {
            m_CastText.text = LocalizeLookUp.GetText("ftf_silenced");
        }
        else
        {
            m_CastText.text = LocalizeLookUp.GetText("spellbook_more_spells");
        }
    }
}
