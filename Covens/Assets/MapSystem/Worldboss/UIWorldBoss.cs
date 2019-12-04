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
    }

    public void Setup(WorldBossMarker boss)
    {
        m_InputBlocker.SetActive(true);
    }

    public void Open(WorldBossMarker boss)
    {
        m_InputBlocker.SetActive(true);

        Vector3 pos = boss.transform.position + MapsAPI.Instance.mapCenter.forward * 70;
        MapCameraUtils.FocusOnPosition(pos, 1f, false, 1f);
        MapCameraUtils.SetExtraFOV(-3);

        MarkerSpawner.HighlightMarkers(new List<MuskMarker> { PlayerManager.witchMarker, boss });

        m_Title.text = LocalizeLookUp.GetText("worldboss_title").Replace("{{name}}", LocalizeLookUp.GetSpiritName(boss.bossToken.spiritId));
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
