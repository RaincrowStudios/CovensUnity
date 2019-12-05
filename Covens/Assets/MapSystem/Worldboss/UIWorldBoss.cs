using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Raincrow.Maps;

public class UIWorldBoss : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_Title;
    [SerializeField] private Image m_BossPortrait;
    [SerializeField] private Image m_BossEnergyBar;
    [SerializeField] private TextMeshProUGUI m_BossEnergyPercent;
    [Space]
    [SerializeField] private TextMeshProUGUI[] m_DamageRank;
    [Space]
    [SerializeField] private TextMeshProUGUI m_PlayerRank;
    [SerializeField] private TextMeshProUGUI m_PlayerName;
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
        m_CloseButton.onClick.AddListener(OnClickClose);

        MapView.OnEnterBossArea += OnEnterBossArea;
        MapView.OnLeaveBossArea += MapView_OnLeaveBossArea;


        MarkerSpawner.Instance.OnSelectMarker += (m) =>
        {
            if (m.Type == MarkerSpawner.MarkerType.BOSS)
                Open(m as WorldBossMarker);
        };
    }

    private void MapView_OnLeaveBossArea()
    {
        BossRankHandler.OnUpdateBossRank -= OnBossRankUpdate;
    }

    private void OnEnterBossArea(WorldBossMarker boss)
    {
        m_BossMarker = boss;

        m_Title.text = LocalizeLookUp.GetText("worldboss_title").Replace("{{name}}", LocalizeLookUp.GetSpiritName(boss.bossToken.spiritId));
        boss.GetPortrait(spr => m_BossPortrait.sprite = spr);
        m_BossEnergyBar.fillAmount = boss.bossToken.energy / (float)boss.bossToken.baseEnergy;

        BossRankHandler.OnUpdateBossRank += OnBossRankUpdate;
    }

    private void OnLeaveBossArea()
    {

    }

    private void OnBossRankUpdate(BossRankHandler.EventData data)
    {
        //update energy
        Debug.Log(data.energy + " / " + m_BossMarker.bossToken.baseEnergy);
        Debug.LogError(data.energy + " / " + (float)m_BossMarker.bossToken.baseEnergy);
        float amount = data.energy / (float)m_BossMarker.bossToken.baseEnergy;
        m_BossEnergyBar.fillAmount = amount;
        m_BossEnergyPercent.text = ((int)(amount * 100)) + "%";

        //update rank
        int aux = 0;
        bool isTop3 = false;
        for (int i = data.rank.Length - 1; i >= 0 && i >= data.rank.Length - 3; i--)
        {
            foreach (string name in data.rank[i].Keys)
            {
                m_DamageRank[aux].text = $"{name} ({-data.rank[i][name].total})";
                if (name == PlayerDataManager.playerData.name)
                    isTop3 = true;
            }
            aux++;
        }

        if (isTop3)
        {
            m_PlayerName.text = m_PlayerName.text = "";
        }
        else
        {
            m_PlayerName.text = PlayerDataManager.playerData.name;
            m_PlayerRank.text = "?th";
        }
    }

    private void Open(WorldBossMarker boss)
    {
        m_InputBlocker.SetActive(true);

        Vector3 pos = boss.transform.position + MapsAPI.Instance.mapCenter.forward * 70;
        MapCameraUtils.FocusOnPosition(pos, 1f, false, 1f);
        MapCameraUtils.SetExtraFOV(-3);

        MarkerSpawner.HighlightMarkers(new List<MuskMarker> { PlayerManager.witchMarker, boss });

        foreach (var rank in m_DamageRank)
            rank.text = "";
        m_PlayerName.text = "";
        m_PlayerRank.text = "";

        OnSelectBoss?.Invoke();
    }

    private void Close()
    {
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
