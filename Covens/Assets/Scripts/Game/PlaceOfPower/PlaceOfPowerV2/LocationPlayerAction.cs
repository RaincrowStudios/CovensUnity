using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System;
using Raincrow.Maps;
using UnityEngine;
using UnityEngine.UI;

public class LocationPlayerAction : MonoBehaviour
{

    private static LocationPlayerAction Instance { get; set; }

    [SerializeField] private CanvasGroup m_MoveActionCG;
    [SerializeField] private Image[] m_SummonCountDown;
    [SerializeField] private Image[] m_FlightCountDown;
    [SerializeField] private Button m_SummonBtn;
    [SerializeField] private Button m_FlightBtn;
    [SerializeField] private GameObject m_MoveCloser;
    [SerializeField] private Button m_MoveCloserCloseBtn;

    public const float MOVE_TIMER = 5;
    public const float SUMMON_TIMER = 30;

    public static IMarker playerMarker { get; private set; }
    public static LocationPosition selectedPosition { get; private set; }
    public static int getCurrentIsland => playerWitchToken.island;
    public static int playerPopIndex => playerWitchToken.popIndex;
    public static WitchToken playerWitchToken => playerMarker.Token as WitchToken;


    public static void SetSelectedPosition(LocationPosition locationPosition)
    {
        selectedPosition = locationPosition;
    }

    public static void HideMoveCloser()
    {
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

    private void Awake()
    {
        m_MoveCloserCloseBtn.onClick.AddListener(HideMoveCloser);
        Instance = this;
        m_FlightBtn.onClick.AddListener(Move);
        SetMoveActionState(false);
        m_MoveActionCG.alpha = .25f;
        InitializeMoveActions(m_SummonCountDown, m_SummonBtn);
        InitializeMoveActions(m_FlightCountDown, m_FlightBtn);
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

    public static void SetMoveActionState(bool interactable)
    {
        Instance.m_MoveActionCG.interactable = interactable;
    }

    public static void MakeVisible()
    {
        Instance.m_MoveActionCG.blocksRaycasts = true;
        SetMoveActionState(true);
        Instance.m_MoveActionCG.alpha = 1;
    }

    public static void MakeTransparent()
    {
        Instance.m_MoveActionCG.alpha = .25f;
        Instance.m_MoveActionCG.blocksRaycasts = false;
    }

    private void Move()
    {
        Debug.Log("Move button click");
        LocationUnitSpawner.MoveWitch(selectedPosition.island, selectedPosition.position, () =>
        {
            SetButtonState(MOVE_TIMER, m_FlightCountDown, m_FlightBtn);
        });
    }


    private void Summon()
    {
        SetButtonState(MOVE_TIMER, Instance.m_FlightCountDown, Instance.m_FlightBtn);
    }

    private static void SetButtonState(float timer, Image[] imgArr, Button btn)
    {
        btn.interactable = false;
        LeanTween.value(1, 0, timer).setOnUpdate((float v) =>
        {
            imgArr[0].fillAmount = v;
            imgArr[1].fillAmount = v;
        }).setOnComplete(() =>
        {
            btn.interactable = true;
        });
    }

    private void InitializeMoveActions(Image[] imgArr, Button btn)
    {
        imgArr[0].fillAmount = 0;
        imgArr[1].fillAmount = 0;
        btn.interactable = true;
    }
}