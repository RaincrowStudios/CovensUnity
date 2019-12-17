using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Raincrow.Maps;
using Raincrow.GameEventResponses;

public class UIWorldBoss : MonoBehaviour
{
    [SerializeField] private UIMarkerPointer m_MarkerPointer;
    [Space]
    [SerializeField] private TextMeshProUGUI m_Title;
    [SerializeField] private Image m_Portrait;
    [SerializeField] private Image m_BossEnergyBar;
    [SerializeField] private UIConditionList m_ConditionList;
    [SerializeField] private TextMeshProUGUI m_BossEnergyPercent;
    [SerializeField] private TextMeshProUGUI[] m_DamageRank;
    [Space]
    [SerializeField] private TextMeshProUGUI m_PlayerRank;
    [SerializeField] private TextMeshProUGUI m_PlayerName;
    [SerializeField] private TextMeshProUGUI m_PlayerSpace;
    [Space]
    [SerializeField] private GameObject m_InputBlocker;
    [SerializeField] private Button m_CloseButton;

    private WorldBossMarker m_BossMarker;

    private int m_TweenId;
    public static event System.Action OnSelectBoss;
    public static event System.Action OnUnselectBoss;

    private void Awake()
    {
        m_InputBlocker.SetActive(false);
        m_MarkerPointer.gameObject.SetActive(false);
        m_CloseButton.onClick.AddListener(OnClickClose);

        MapView.OnEnterBossArea += MapView_OnEnterBossArea;
        MapView.OnLeaveBossArea += MapView_OnLeaveBossArea;
    }

    private void Start()
    {
        MarkerSpawner.Instance.OnSelectMarker += (m) =>
        {
            if (m.Type == MarkerSpawner.MarkerType.BOSS)
                Open(m as WorldBossMarker);
        };
    }

    private void MapView_OnLeaveBossArea()
    {
        BossRankHandler.OnUpdateBossRank -= OnBossRankUpdate;
        m_MarkerPointer.gameObject.SetActive(false);

        if (m_InputBlocker.activeSelf)
            Close();

        SpellCastHandler.OnSpellCast -= SpellCastHandler_OnSpellCast;
        TickSpellHandler.OnSpellTick -= TickSpellHandler_OnSpellTick;
    }

    private void MapView_OnEnterBossArea(WorldBossMarker boss)
    {
        string spiritId = boss.bossToken.spiritId;
        m_BossMarker = boss;

        DownloadedAssets.GetSprite(spiritId + "_portrait", m_Portrait);

        m_Title.text = LocalizeLookUp.GetSpiritName(spiritId);
        m_BossEnergyBar.fillAmount = boss.bossToken.energy / (float)boss.bossToken.baseEnergy;
        m_BossEnergyPercent.text = ((int)(m_BossEnergyBar.fillAmount * 100)) + "%";

        BossRankHandler.OnUpdateBossRank += OnBossRankUpdate;
        SpellCastHandler.OnSpellCast += SpellCastHandler_OnSpellCast;
        TickSpellHandler.OnSpellTick += TickSpellHandler_OnSpellTick;

        m_MarkerPointer.SetTarget(boss);
        m_MarkerPointer.gameObject.SetActive(true);

        //clear UI
        foreach (var rank in m_DamageRank)
            rank.text = "";
        m_PlayerName.text = "";
        m_PlayerRank.text = "";
        m_PlayerSpace.text = "";
    }

    private void TickSpellHandler_OnSpellTick(SpellCastHandler.SpellCastEventData data)
    {
        if (data.caster.id != m_BossMarker.Token.Id)
            return;

        //if (data.result.isSuccess)
        //{
        //    if (data.spell == "spell_channeling")
        //    {
        //        PlayerNotificationManager.Instance.ShowNotification(
        //            LocalizeLookUp.GetText("character_channeling")
        //                .Replace("{{character}}", LocalizeLookUp.GetSpiritName(m_BossMarker.bossToken.spiritId)),
        //            m_BossPortrait.overrideSprite);
        //    }
        //}
    }

    private void SpellCastHandler_OnSpellCast(SpellCastHandler.SpellCastEventData data)
    {
        if (data.caster.id != m_BossMarker.Token.Id)
            return;

        if (data.result.isSuccess)
        {
            if (data.spell == "spell_channeling")
            {
                PlayerNotificationManager.Instance.ShowNotification(
                    LocalizeLookUp.GetText("character_channeling")
                        .Replace("{{character}}", LocalizeLookUp.GetSpiritName(m_BossMarker.bossToken.spiritId)));
            }
        }

        if (data.result.isSuccess)
        {
            if (data.spell == "spell_addledMind")
            {
                PlayerNotificationManager.Instance.ShowNotification(
                    LocalizeLookUp.GetText("character_interrupted")
                        .Replace("{{character}}", LocalizeLookUp.GetSpiritName(m_BossMarker.bossToken.spiritId)));
            }
        }
    }

    private void OnBossRankUpdate(BossRankHandler.EventData data)
    {
        //update energy
        float amount = data.energy / (float)m_BossMarker.bossToken.baseEnergy;
        m_BossEnergyBar.fillAmount = amount;
        m_BossEnergyPercent.text = ((int)(amount * 100)) + "%";

        //update rank
        bool isTop3 = false;
        for (int i = 0; i < m_DamageRank.Length; i++)
        {
            foreach (string name in data.rank[i].Keys)
            {
                m_DamageRank[i].text = name;//$"{name} ({-data.rank[i][name].total})";
                if (name == PlayerDataManager.playerData.name || name == TeamManager.MyCovenInfo.name)
                {
                    isTop3 = true;
                    m_DamageRank[i].text = string.Concat(m_DamageRank[i].text, " (", LocalizeLookUp.GetText("cast_you"), ")");
                }
            }
        }

        if (isTop3)
        {
            m_PlayerRank.text = m_PlayerName.text = m_PlayerSpace.text = "";
        }
        else
        {
            for (int i = 3; i < m_DamageRank.Length; i++)
            {
                foreach (string name in data.rank[i].Keys)
                {
                    if (string.IsNullOrEmpty(TeamManager.MyCovenId))
                    {
                        if (name == PlayerDataManager.playerData.name)
                        {
                            m_PlayerName.text = string.Concat(name, " (", LocalizeLookUp.GetText("cast_you"), ")");
                            m_PlayerRank.text = (i+1) + "th";
                            m_PlayerSpace.text = "...";
                            return;
                        }
                    }
                    else
                    {
                        if (name == TeamManager.MyCovenInfo.name)
                        {
                            m_PlayerName.text = string.Concat(name, " (", LocalizeLookUp.GetText("cast_you"), ")");
                            m_PlayerRank.text = (i+1) + "th";
                            m_PlayerSpace.text = "...";
                            return;
                        }
                    }
                }
            }
        }
    }

    //private void MarkerSpawner_OnReceiveMarkerData(string arg1, CharacterMarkerData arg2)
    //{

    //}

    private void _OnStatusEffectApplied(string character, string caster, StatusEffect statusEffect)
    {
        if (character != m_BossMarker.Token.instance)
            return;

        m_ConditionList.AddCondition(statusEffect);
    }

    private void _OnExpireEffect(string character, StatusEffect effect)
    {
        if (character != m_BossMarker.Token.instance)
            return;

        m_ConditionList.RemoveCondition(effect);
    }

    private void Open(WorldBossMarker boss)
    {
        m_InputBlocker.SetActive(true);
        //MarkerSpawner.Instance.OnReceiveMarkerData += MarkerSpawner_OnReceiveMarkerData;
        SpellCastHandler.OnApplyEffect += _OnStatusEffectApplied;
        SpellCastHandler.OnExpireEffect += _OnExpireEffect;

        m_ConditionList.Setup(boss.bossToken.effects);

        Vector3 pos = boss.transform.position + MapsAPI.Instance.mapCenter.forward * 70;
        MapCameraUtils.FocusOnPosition(pos, 1f, false, 1f);
        MapCameraUtils.SetExtraFOV(-3);

        MarkerSpawner.HighlightMarkers(new List<MuskMarker> { PlayerManager.witchMarker, boss });

        OnSelectBoss?.Invoke();
    }

    private void Close()
    {
        //MarkerSpawner.Instance.OnReceiveMarkerData -= MarkerSpawner_OnReceiveMarkerData;
        SpellCastHandler.OnApplyEffect -= _OnStatusEffectApplied;
        SpellCastHandler.OnExpireEffect -= _OnExpireEffect;
        m_InputBlocker.SetActive(false);

        MapCameraUtils.FocusOnPosition(MapsAPI.Instance.mapCenter.position, 0.99f, true, 1f);
        MapCameraUtils.SetExtraFOV(0);

        MarkerSpawner.HighlightMarkers(new List<MuskMarker> { });

        OnUnselectBoss?.Invoke();
    }

    private void OnClickClose()
    {
        Close();
    }
}
