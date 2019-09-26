using System.Linq;
using System;
using Raincrow.Maps;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using TMPro;

public class LocationPlayerAction : MonoBehaviour
{
    private static LocationPlayerAction Instance { get; set; }

    [SerializeField] private GameObject m_MoveCloser;
    [SerializeField] private Button m_MoveCloserCloseBtn;
    [SerializeField] private Button m_CenterOnPlayerBtn;
    [SerializeField] private Button m_SpellBookBtn;
    [SerializeField] private Transform m_MainUITransform;
    [SerializeField] private GameObject m_Bos;

    [SerializeField] private LocationActionButton m_ActionBtn;
    [SerializeField] private Sprite m_flySprite;
    [SerializeField] private Sprite m_CloakSprite;
    [SerializeField] private Sprite m_SummonSprite;
    [SerializeField] private CanvasGroup[] m_CloakUIDisable;
    [SerializeField] private Transform m_CloakUIGreyScale;
    [SerializeField] private Button m_DisableCloakBtn;
    [SerializeField] private TextMeshProUGUI m_EnergyText;
    [SerializeField] private Slider m_EnergySlider;
    [SerializeField] private CanvasGroup m_CloakUI;
    [SerializeField] private Button m_helpButton;

    private const int MOVE_TIMER = 5;
    private const int SUMMON_TIMER = 30;
    private const int CLOAK_TIMER = 180;

    private static LocationActionButton[] m_BtnArr = new LocationActionButton[3];
    public static IMarker playerMarker { get; private set; }
    public static LocationPosition selectedPosition { get; private set; }
    public static int getCurrentIsland => playerWitchToken.island;
    public static int playerPopIndex => playerWitchToken.popIndex;

    public static WitchToken playerWitchToken => playerMarker.Token as WitchToken;
    public static bool isCloaked { get; private set; }

    private static SpiritToken summonedSpirit;

    public static void OnSummon(SpiritToken spirit)
    {
        if (spirit.owner == PlayerDataManager.playerData.instance)
        {
            summonedSpirit = spirit;
            m_BtnArr[2].SetLock(true);
        }

    }

    public static void RemoveSummonedSpirit(string instance)
    {
        if (summonedSpirit != null && instance == summonedSpirit.instance)
        {
            summonedSpirit = null;
            m_BtnArr[2].SetLock(false);
        }
    }

    public static void UpdateEnergy(int energy)
    {
        Instance.m_EnergySlider.maxValue = PlayerDataManager.playerData.baseEnergy;
        Instance.m_EnergySlider.value = PlayerDataManager.playerData.energy;
        Instance.m_EnergyText.text = $"{PlayerDataManager.playerData.energy}/{PlayerDataManager.playerData.baseEnergy}";
    }

    public static void SetSelectedPosition(LocationPosition locationPosition)
    {
        selectedPosition = locationPosition;
    }

    public static void HideMoveCloser()
    {
        SoundManagerOneShot.Instance.PlayButtonTap();
        Instance.m_MoveCloser.SetActive(false);
        LocationUnitSpawner.EnableMarkers();
    }

    public static void ShowMoveCloser(bool isEmptySlot = false)
    {
        if (Instance.m_MoveCloser.activeInHierarchy)
            Instance.m_MoveCloser.SetActive(false);
        Instance.m_MoveCloser.SetActive(true);

        if (isEmptySlot)
            LocationUnitSpawner.DisableMarkers();
    }

    void OnEnable()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Awake()
    {
        Debug.Log("LOCATION SETUP");
        Instance = this;
        m_SpellBookBtn.onClick.AddListener(() =>
        {
            Utilities.InstantiateUI(m_Bos, m_MainUITransform);
        });
        m_MoveCloserCloseBtn.onClick.AddListener(HideMoveCloser);
        m_CenterOnPlayerBtn.onClick.AddListener(CenterOnPlayer);
        m_DisableCloakBtn.onClick.AddListener(RequestDisableCloaking);
        LocationIslandController.OnEnterLocation += () => Debug.Log("||||||||||||||POPS IN"); ;

        LocationIslandController.OnEnterLocation += Setup;
        LocationIslandController.OnExitLocation -= Setup;
        LocationIslandController.OnExitLocation -= () =>
        {
            OnMapEnergyChange.OnPlayerEnergyChange -= UpdateEnergy;

        };
        LocationIslandController.OnExitLocation += () =>
        {
            if (isCloaked)
            {
                RenderSettings.fog = false;
                isCloaked = false;
            }
        };
        m_helpButton.onClick.AddListener(() =>
        {
            LocationTutorial.Instance.Open();
        });

    }

    private void Setup()
    {
        Debug.Log("Setting up Player Actions");

        AvatarSpriteUtil.Instance.GenerateWardrobePortrait((s) =>
           {
               Instance.m_CenterOnPlayerBtn.GetComponent<Image>().sprite = s;
           });
        LocationActionButton btn;
        UpdateEnergy(0);
        OnMapEnergyChange.OnPlayerEnergyChange += UpdateEnergy;
        if (m_BtnArr[0] == null)
        {
            btn = Instantiate(Instance.m_ActionBtn, Instance.transform) as LocationActionButton;
            btn.Setup(MOVE_TIMER, m_flySprite, () =>
            {
                LocationUnitSpawner.MoveWitch(selectedPosition.island, selectedPosition.position);
            });
            m_BtnArr[1] = btn;

            btn = Instantiate(Instance.m_ActionBtn, Instance.transform) as LocationActionButton;
            btn.Setup(SUMMON_TIMER, m_SummonSprite, () =>
            {
                UISummoning.Open(selectedPosition.position, selectedPosition.island, null, (s) =>
                {
                    UISummoning.Close();
                    if (s != null)
                    {
                        UIGlobalPopup.ShowError(null, "Summoning Failed");
                    }
                }, null);
            });
            m_BtnArr[2] = btn;

            btn = Instantiate(Instance.m_ActionBtn, Instance.transform) as LocationActionButton;
            btn.Setup(PlayerDataManager.playerData.level + 20, m_CloakSprite, () =>
           {

               var data = new
               {
                   spell = "spell_astral"
               };
               APIManager.Instance.Post(
                  "character/cast/" + playerWitchToken.instance,
                  JsonConvert.SerializeObject(data), (s, r) =>
                  {
                      if (r == 200)
                      {
                          RenderSettings.fog = true;
                          RenderSettings.fogMode = FogMode.Linear;
                          RenderSettings.fogColor = new Color(0.14f, 0.14f, 0.14f);
                          CenterOnPlayer();
                          UIQuickCast.EnableQuickcastButtons(false);
                          if (UIPlayerInfo.IsShowing)
                              UIPlayerInfo.SetVisibility(false);
                          if (UISpiritInfo.IsShowing)
                              UISpiritInfo.SetVisibility(false);
                          LeanTween.value(1, 0, .5f).setOnUpdate((float v) => CloakingFX(v));
                          isCloaked = true;
                          SoundManagerOneShot.Instance.FadeOutBGTrack();
                          SoundManagerOneShot.Instance.PlayCloakingSFX(true);
                          foreach (var item in LocationUnitSpawner.Markers)
                          {
                              item.Value.ScaleNamePlate(false, .5f);
                              //   item.Value.EnergyRingFade(1, .5f);
                          }
                      }
                      else
                      {
                          Debug.LogError("Cloaking failed");
                          UIGlobalPopup.ShowError(() => { }, "Cloaking failed. Error Code: " + s);
                      }
                  });
           }, true);
            m_BtnArr[0] = btn;
        }
    }

    private void CloakingFX(float v)
    {
        foreach (var item in m_CloakUIDisable)
        {
            if (item != null)
                item.alpha = v;
        }
        m_CloakUI.gameObject.SetActive(true);
        m_CloakUI.alpha = Mathf.Lerp(1, 0, v);
        m_CloakUIGreyScale.gameObject.SetActive(true);
        m_CloakUIGreyScale.SetParent(playerMarker.GameObject.transform.GetChild(0));
        m_CloakUIGreyScale.localPosition = Vector3.zero;
        m_CloakUIGreyScale.localScale = Vector3.one * Mathf.Lerp(1, 0, v);
        RenderSettings.fogStartDistance = Mathf.Lerp(-1446, 2100, v);
        RenderSettings.fogEndDistance = Mathf.Lerp(878, 2200, v);
        m_BtnArr[1].transform.localScale = Mathf.Lerp(0, 1, v) * Vector3.one;
        m_BtnArr[2].transform.localScale = Mathf.Lerp(0, 1, v) * Vector3.one;
        m_BtnArr[0].transform.localScale = Mathf.Lerp(1.2f, 1, v) * Vector3.one;

        //  LocationUnitSpawner.guardianMarker.SetAlpha(.5f);
    }

    public void RequestDisableCloaking()
    {
        SoundManagerOneShot.Instance.PlayButtonTap();
        m_DisableCloakBtn.interactable = false;
        APIManager.Instance.Delete(
               "character/astral", "{}", (s, r) =>
               {
                   //    if (r == 200)
                   //    {
                   m_BtnArr[0].Setup(180);
                   DisableCloaking();
                   //    }
                   m_DisableCloakBtn.interactable = true;
               });
    }

    public static void DisableCloaking()
    {
        foreach (var item in LocationUnitSpawner.Markers)
        {
            item.Value.ScaleNamePlate(true, .5f);
            // item.Value.EnergyRingFade(0, .5f);
        }
        if (UIPlayerInfo.IsShowing)
            UIPlayerInfo.SetVisibility(true);
        if (UISpiritInfo.IsShowing)
            UISpiritInfo.SetVisibility(true);

        if (UIPlayerInfo.IsShowing || UISpiritInfo.IsShowing)
            UIQuickCast.EnableQuickcastButtons(true);


        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogColor = new Color(0.14f, 0.14f, 0.14f);
        LeanTween.value(0, 1, .5f).setOnUpdate((float v) => Instance.CloakingFX(v)).setOnComplete(() =>
        {
            isCloaked = false;
            SoundManagerOneShot.Instance.FadeInBGTrack();
            Debug.Log("isCloaked = false");
            SoundManagerOneShot.Instance.PlayCloakingSFX(false);
            RenderSettings.fog = false;
            Instance.m_CloakUI.gameObject.SetActive(false);
            Instance.m_CloakUIGreyScale.gameObject.SetActive(false);

        });
    }

    private static void CenterOnPlayer()
    {
        SoundManagerOneShot.Instance.PlayButtonTap();
        if (!UIPlayerInfo.IsShowing && !UISpellcastBook.IsOpen)
        {
            LocationIslandController.moveCamera(playerMarker.AvatarTransform.position);
        }
    }

    public static void SetPlayerMarker(IMarker m)
    {
        playerMarker = m;
    }

    public static bool CanSelectIsland(Token token)
    {
        if (token.popIndex != -1)
        {
            if (getCurrentIsland == 0 && token.island == 5) return true;
            else if (getCurrentIsland == 5 && token.island == 0) return true;
            else return Math.Abs(token.island - getCurrentIsland) < 2;

        }
        else return true;
    }

    public static void ShowActions()
    {

        UIQuickCast.Open(() =>
        {
            UIQuickCast.EnableQuickcastButtons(false);
            Debug.Log(m_BtnArr.Length);
            for (int i = 0; i < m_BtnArr.Length; i++)
            {
                Debug.Log(m_BtnArr[i].gameObject.name);
                UIQuickCast.AddItem(m_BtnArr[i].gameObject, i, () =>
                {

                    if (i < m_BtnArr.Length && m_BtnArr[i].gameObject.activeInHierarchy)
                    {
                        m_BtnArr[i].gameObject.SetActive(false);
                        m_BtnArr[i].transform.SetParent(Instance.transform);
                    }

                });
                m_BtnArr[i].gameObject.SetActive(true);
            }
        });
    }

    public static void HideActions()
    {
        UIQuickCast.EnableQuickcastButtons(true);
        // Debug.Log(m_BtnArr.Length);
        for (int i = 0; i < m_BtnArr.Length; i++)
        {
            if (m_BtnArr[i] != null && m_BtnArr[i].gameObject.activeInHierarchy)
            {
                m_BtnArr[i].gameObject.SetActive(false);
                m_BtnArr[i].transform.SetParent(Instance.transform);
            }
        }
    }

    public static void ShowSpells()
    {

        LoadPOPManager.HandleQuickCastOpen(ShowSpellHelper);

    }

    private static void ShowSpellHelper()
    {
        UIQuickCast.EnableQuickcastButtons(true);
        UIQuickCast.AddItem(m_BtnArr[0].gameObject, 0, () =>
                {
                    if (m_BtnArr[0].gameObject.activeInHierarchy)
                    {
                        m_BtnArr[0].gameObject.SetActive(false);
                        if (Instance != null && Instance.transform != null)
                            m_BtnArr[0].transform.SetParent(Instance.transform);
                    }
                });
        m_BtnArr[0].gameObject.SetActive(true);
    }
}