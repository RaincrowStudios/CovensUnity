using System;
using Raincrow.Maps;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
public class LocationPlayerAction : MonoBehaviour
{
    private static LocationPlayerAction Instance { get; set; }

    [SerializeField] private GameObject m_MoveCloser;
    [SerializeField] private Button m_MoveCloserCloseBtn;
    [SerializeField] private Button m_CenterOnPlayerBtn;

    [SerializeField] private LocationActionButton m_ActionBtn;
    [SerializeField] private Sprite m_flySprite;
    [SerializeField] private Sprite m_CloakSprite;
    [SerializeField] private Sprite m_SummonSprite;

    private const int MOVE_TIMER = 5;
    private const int SUMMON_TIMER = 30;
    private const int CLOAK_TIMER = 180;

    private static LocationActionButton[] m_BtnArr = new LocationActionButton[3];
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
        Instance = this;
        m_MoveCloserCloseBtn.onClick.AddListener(HideMoveCloser);
        m_CenterOnPlayerBtn.onClick.AddListener(CenterOnPlayer);
        LocationActionButton btn;
        btn = Instantiate(m_ActionBtn, transform) as LocationActionButton;
        btn.Setup(MOVE_TIMER, m_flySprite, () =>
        {
            LocationUnitSpawner.MoveWitch(selectedPosition.island, selectedPosition.position);
        });
        m_BtnArr[1] = btn;

        btn = Instantiate(m_ActionBtn, transform) as LocationActionButton;
        btn.Setup(SUMMON_TIMER, m_SummonSprite, () => { });
        m_BtnArr[2] = btn;

        btn = Instantiate(m_ActionBtn, transform) as LocationActionButton;
        btn.Setup(CLOAK_TIMER, m_CloakSprite, () =>
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
                       Debug.Log(s);
                       Debug.Log("cloak success");
                       LocationUnitSpawner.EnableCloaking(playerWitchToken.instance);
                   }
               });
        });
        m_BtnArr[0] = btn;
    }



    private static void CenterOnPlayer()
    {
        if (!UIPlayerInfo.isShowing && !UISpellcastBook.IsOpen)
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
            for (int i = 0; i < m_BtnArr.Length; i++)
            {
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
        for (int i = 0; i < m_BtnArr.Length; i++)
        {
            if (m_BtnArr[i].gameObject.activeInHierarchy)
            {
                m_BtnArr[i].gameObject.SetActive(false);
                m_BtnArr[i].transform.SetParent(Instance.transform);
            }
        }
    }

    public static void ShowSpells()
    {
        UIQuickCast.Open(() =>
        {
            UIQuickCast.EnableQuickcastButtons(true);
            UIQuickCast.AddItem(m_BtnArr[0].gameObject, 0, () =>
                    {
                        if (m_BtnArr[0].gameObject.activeInHierarchy)
                        {
                            m_BtnArr[0].gameObject.SetActive(false);
                            m_BtnArr[0].transform.SetParent(Instance.transform);
                        }
                    });
            m_BtnArr[0].gameObject.SetActive(true);
        });

    }
}