using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMain_StateAnim : MonoBehaviour
{
    [System.Serializable]
    public struct AnimStateSettings
    {
        public Vector2 leftBar_pos;
        public Vector2 bottomBar_pos;
        public Vector2 energy_offMin;
        public Vector2 quickcast_pos;
        public Vector2 worldboss_pos;
        public float obj_scale;
        public float bar_alpha;
    }

    public enum State
    {
        MAP,
        MAP_SELECT,
        POP,
        POP_SELECT,
        BOSS,
        BOSS_SELECT
    }

    [SerializeField] private RectTransform leftBar;
    [SerializeField] private RectTransform bottomBar;
    [SerializeField] private GameObject[] scaleObjects;
    [SerializeField] private CanvasGroup[] seasonalUI;
    [SerializeField] private float time = 1;
    [SerializeField] private LeanTweenType tweenType;
    [SerializeField] private CanvasGroup[] bars;
    [SerializeField] private RectTransform energy;
    [SerializeField] private RectTransform quickCast;
    [SerializeField] private RectTransform worldboss;


    [Header("Buttons")]
    [SerializeField] private Button m_SummonButton;
    [SerializeField] private Button m_ShoutButton;

    [Header("Anim")]
    [SerializeField] private AnimStateSettings m_MapView;
    [SerializeField] private AnimStateSettings m_MapView_Select;
    [SerializeField] private AnimStateSettings m_PopView;
    [SerializeField] private AnimStateSettings m_PopView_Select;
    [SerializeField] private AnimStateSettings m_Worldboss;
    [SerializeField] private AnimStateSettings m_Worldboss_Select;

    private int m_TweenId;
    private bool m_InBossArea;
    private bool m_InPop;
    private bool m_TargetSelected;
    private bool m_BossSelected;
    private bool m_Flying;

    void Awake()
    {
        SetState(State.MAP);

        LocationIslandController.OnEnterLocation += OnEnterPop;
        LocationIslandController.OnExitLocation += OnLeavePop;

        MapView.OnEnterBossArea += OnEnterBossArea;
        MapView.OnLeaveBossArea += OnLeaveBossArea;

        UISpiritInfo.OnOpen += OnSelectCharacter;
        UIPlayerInfo.OnOpen += OnSelectCharacter;
        UISpiritInfo.OnClose += OnUnselectCharacter;
        UIPlayerInfo.OnClose += OnUnselectCharacter;
        UIWorldBoss.OnSelectBoss += OnSelectBoss;
        UIWorldBoss.OnUnselectBoss += OnUnselectBoss;
    }

    public void OnLocationClick()
    {
        var k = Resources.Load<GameObject>("LocationManagerUI");
        Instantiate(k);
    }

    private void OnEnterPop()
    {
        m_InPop = true;
        UpdateUIState();
    }

    private void OnLeavePop()
    {
        m_InPop = false;
        UpdateUIState();
    }

    private void OnEnterFlight()
    {
        m_Flying = true;

        m_SummonButton.interactable = false;
        m_ShoutButton.interactable = false;
    }

    private void OnLeaveFlight()
    {
        m_Flying = false;

        m_SummonButton.interactable = true;
        m_ShoutButton.interactable = true;
    }

    private void OnEnterBossArea(WorldBossMarker boss)
    {
        m_InBossArea = true;
        UpdateUIState();
    }

    private void OnLeaveBossArea()
    {
        m_InBossArea = false;
        UpdateUIState();
    }

    private void OnSelectCharacter()
    {
        m_TargetSelected = true;
        UpdateUIState();
    }

    private void OnUnselectCharacter()
    {
        m_TargetSelected = false;
        UpdateUIState();
    }

    private void OnSelectBoss()
    {
        m_BossSelected = true;
        UpdateUIState();
    }

    private void OnUnselectBoss()
    {
        m_BossSelected = false;
        UpdateUIState();
    }

    private void UpdateUIState()
    {
        if (m_BossSelected)
        {
            SetState(State.BOSS_SELECT);
        }
        else if (m_TargetSelected)
        {
            if (m_InPop)
                SetState(State.POP_SELECT);
            else
                SetState(State.MAP_SELECT);
        }
        else
        {
            if (m_InPop)
                SetState(State.POP);
            else if (m_InBossArea)
                SetState(State.BOSS);
            else
                SetState(State.MAP);
        }
    }

    public AnimStateSettings GetStateSettings(State state)
    {
        switch (state)
        {
            case State.MAP: return m_MapView;
            case State.MAP_SELECT: return m_MapView_Select;
            case State.POP: return m_PopView;
            case State.POP_SELECT: return m_PopView_Select;
            case State.BOSS: return m_Worldboss;
            case State.BOSS_SELECT: return m_Worldboss_Select;
        }
        return m_MapView;
    }

    public void SetState(State state)
    {
        AnimStateSettings target = GetStateSettings(state);
        LeanTween.cancel(m_TweenId);

        Vector2 leftbar_pos = leftBar.anchoredPosition;
        Vector2 bottombar_pos = bottomBar.anchoredPosition;
        Vector2 energy_offMin = energy.offsetMin; ;
        Vector2 quickcast_pos = quickCast.anchoredPosition;
        Vector2 worldboss_pos = worldboss.anchoredPosition;

        float scale = scaleObjects[0].transform.localScale.x;
        float alpha = bars[0].alpha;

        m_TweenId = LeanTween.value(0, 1, 1)
            .setEaseOutCubic()
            .setOnUpdate((float t) =>
            {
                leftBar.anchoredPosition = Vector2.Lerp(leftbar_pos, target.leftBar_pos, t);
                bottomBar.anchoredPosition = Vector2.Lerp(bottombar_pos, target.bottomBar_pos, t);
                energy.offsetMin = Vector2.Lerp(energy_offMin, target.energy_offMin, t);
                quickCast.anchoredPosition = Vector2.Lerp(quickcast_pos, target.quickcast_pos, t);
                worldboss.anchoredPosition = Vector2.Lerp(worldboss_pos, target.worldboss_pos, t);

                float _scale = Mathf.Lerp(scale, target.obj_scale, t);
                foreach (var item in scaleObjects)
                    item.transform.localScale = new Vector3(_scale, _scale, _scale);

                float _alpha = Mathf.Lerp(alpha, target.bar_alpha, t);
                foreach (var item in bars)
                    item.alpha = _alpha;
                foreach (var item in seasonalUI)
                    item.alpha = _alpha;
            })
            .uniqueId;
    }

#if UNITY_EDITOR
    private void SetState(AnimStateSettings target)
    {
        leftBar.anchoredPosition = target.leftBar_pos;
        bottomBar.anchoredPosition = target.bottomBar_pos;
        energy.offsetMin = target.energy_offMin;
        quickCast.anchoredPosition = target.quickcast_pos;
        worldboss.anchoredPosition = target.worldboss_pos;

        float _scale = target.obj_scale;
        foreach (var item in scaleObjects)
            item.transform.localScale = new Vector3(_scale, _scale, _scale);

        float _alpha = target.bar_alpha;
        foreach (var item in bars)
            item.alpha = _alpha;
        foreach (var item in seasonalUI)
            item.alpha = _alpha;
    }

    [ContextMenu("SetState - Map")]
    private void Debug_SetMap() => SetState(GetStateSettings(State.MAP));

    [ContextMenu("SetState - MapSelect")]
    private void Debug_SetMapSelect() => SetState(GetStateSettings(State.MAP_SELECT));

    [ContextMenu("SetState - WorldBoss")]
    private void Debug_SetWorldBoss() => SetState(GetStateSettings(State.BOSS));

    [ContextMenu("SetState - WorldBossSelect")]
    private void Debug_SetWorldBossSelect() => SetState(GetStateSettings(State.BOSS_SELECT));
#endif
}
