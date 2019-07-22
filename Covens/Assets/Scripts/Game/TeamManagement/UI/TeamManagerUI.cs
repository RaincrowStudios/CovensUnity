using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Raincrow;
using Raincrow.Team;

public class TeamManagerUI : MonoBehaviour
{
    public enum Screen
    {
        NONE = 0,
        HOME,
        MEMBERS,
        INVITES,
        REQUESTS,
    }

    [System.Serializable]
    public struct HomeProperties
    {
        public CanvasGroup m_CanvasGroup;
        public TextMeshProUGUI m_Motto;
        public TextMeshProUGUI m_FounderName;
        public TextMeshProUGUI m_Worldrank;
        public TextMeshProUGUI m_DominionRank;
        public TextMeshProUGUI m_CreatedOn;
        public TextMeshProUGUI m_PopCount;
        public TextMeshProUGUI m_CovenDegree;
        public TextMeshProUGUI m_CreatorDegree;
        public Image m_CovenSigil;
        public Image m_CreatorSigil;

        public Sprite whiteSchool;
        public Sprite shadowSchool;
        public Sprite greySchool;
    }

    [System.Serializable]
    public struct MembersProperties
    {
        public CanvasGroup m_CanvasGroup;
        public LayoutGroup m_Container;
        public TeamMemberItemUI m_ItemPrefab;

        public SimplePool<TeamMemberItemUI> m_Pool;
    }

    [System.Serializable]
    public struct InvitesProperties
    {
        public CanvasGroup m_CanvasGroup;
        public LayoutGroup m_Container;
        //item prefab
    }

    [System.Serializable]
    public struct RequestsProperties
    {
        public CanvasGroup m_CanvasGroup;
        public LayoutGroup m_Container;
        //item prefab
    }

    [Header("General")]
    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private GraphicRaycaster m_InputRaycaster;
    [SerializeField] private CanvasGroup m_MainCanvasGroup;
    [SerializeField] private TeamInputPopup m_InputPopup;
    [SerializeField] private GameObject m_LoadingObj;

    [Header("Header")]
    [SerializeField] private TextMeshProUGUI m_CovenName;
    [SerializeField] private TextMeshProUGUI m_SubTitle;

    [Header("Screens")]
    [SerializeField] private HomeProperties m_Home;
    [SerializeField] private MembersProperties m_Members;
    [SerializeField] private InvitesProperties m_Invites;
    [SerializeField] private RequestsProperties m_Requests;

    [Header("Buttons")]
    [SerializeField] private Button m_CloseButton;
    [SerializeField] private Button m_LeaderboardButton;

    [SerializeField] private Button m_BackButton;
    [SerializeField] private Button m_CreateCovenButton;
    [SerializeField] private Button m_SendRequestButton;
    [SerializeField] private Button m_ViewRequestsButton;
    [SerializeField] private Button m_SendInviteButton;
    [SerializeField] private Button m_ViewInvitesButton;
    [SerializeField] private Button m_ViewMembersButton;
    [SerializeField] private Button m_LeaveCovenButton;
    [SerializeField] private Button m_DisbandCovenButton;

    private static TeamManagerUI m_Instance;

    public static bool IsOpen
    {
        get
        {
            if (m_Instance == null)
                return false;
            return m_Instance.m_InputRaycaster.enabled;
        }
    }

    private TeamData m_CovenData = null;
    private CanvasGroup[] m_Screens;
    private Screen m_CurrentScreen;
    private int m_TweenId;
    private int m_ScreenTweenId;

    private void Awake()
    {
        m_Instance = this;

        m_Canvas.enabled = false;
        m_InputRaycaster.enabled = false;

        m_LoadingObj.SetActive(false);

        CanvasGroup emptyCg = new GameObject().AddComponent<CanvasGroup>();
        emptyCg.transform.SetParent(this.transform);
        emptyCg.alpha = 1;

        m_Screens = new CanvasGroup[]
        {
            emptyCg,
            m_Home.m_CanvasGroup,
            m_Members.m_CanvasGroup,
            m_Invites.m_CanvasGroup,
            m_Requests.m_CanvasGroup
        };

        m_MainCanvasGroup.alpha = 0;
        m_Home.m_CanvasGroup.alpha = 0;
        m_Members.m_CanvasGroup.alpha = 0;
        m_Invites.m_CanvasGroup.alpha = 0;
        m_Requests.m_CanvasGroup.alpha = 0;

        //setup ui
        m_CurrentScreen = Screen.NONE;

        //setup buttons
        //init state
        m_CreateCovenButton.gameObject.SetActive(false);
        m_SendRequestButton.gameObject.SetActive(false);
        m_ViewRequestsButton.gameObject.SetActive(false);
        m_SendInviteButton.gameObject.SetActive(false);
        m_ViewInvitesButton.gameObject.SetActive(false);
        m_ViewMembersButton.gameObject.SetActive(false);
        m_LeaveCovenButton.gameObject.SetActive(false);
        m_DisbandCovenButton.gameObject.SetActive(false);

        //listeners
        m_CloseButton.onClick.AddListener(Hide);
        m_LeaderboardButton.gameObject.SetActive(false);

        m_CreateCovenButton.onClick.AddListener(OnClickCreate);
        m_DisbandCovenButton.onClick.AddListener(OnClickDisband);
        m_ViewMembersButton.onClick.AddListener(OnClickMembers);

        //setup pools
        m_Members.m_Pool = new SimplePool<TeamMemberItemUI>(m_Members.m_ItemPrefab, 5);
    }

    private void SetScreen(Screen screen)
    {
        LeanTween.cancel(m_ScreenTweenId);
        int idx = (int)screen;
        float start = m_Screens[idx].alpha;
        float end = 1f;

        m_ScreenTweenId = LeanTween.value(0, 1, 1f)
            .setOnStart(() =>
            {
                for (int i = 0; i < m_Screens.Length; i++)
                {
                    if (i == idx)
                    {
                        //show the gameobject and enable interaction
                        m_Screens[i].gameObject.SetActive(true);
                        m_Screens[i].interactable = true;
                    }
                    else
                    {
                        //disable interaction with the other uis
                        m_Screens[i].interactable = false;
                    }
                }

                switch (screen)
                {
                    case Screen.HOME:       SetupHome(); break;
                    case Screen.MEMBERS:    SetupMembers(); break;
                    case Screen.INVITES:    SetupInvites(); break;
                    case Screen.REQUESTS:   SetupRequests(); break;
                }
            })
            .setOnUpdate((float t) =>
            {
                //fade the uis
                float a = LeanTween.easeOutCubic(start, end, t);

                for (int i = 0; i < m_Screens.Length; i++)
                {
                    if (i == idx)
                        m_Screens[i].alpha = a;
                    else
                        m_Screens[i].alpha = 1 - a;
                }
            })
            .setOnComplete(() =>
            {
                //hide other gameobjects
                for (int i = 0; i < m_Screens.Length; i++)
                {
                    if (i != idx)
                    {
                        m_Screens[i].gameObject.SetActive(false);
                    }
                }
            })
            .uniqueId;
    }

    private void DisableButtons()
    {
        m_BackButton.gameObject.SetActive(false);
        m_CreateCovenButton.gameObject.SetActive(false);
        m_SendRequestButton.gameObject.SetActive(false);
        m_ViewRequestsButton.gameObject.SetActive(false);
        m_SendInviteButton.gameObject.SetActive(false);
        m_ViewInvitesButton.gameObject.SetActive(false);
        m_ViewMembersButton.gameObject.SetActive(false);
        m_LeaveCovenButton.gameObject.SetActive(false);
        m_DisbandCovenButton.gameObject.SetActive(false);
    }

    private void SetupHome()
    {
        //setup content
        m_CovenName.text = m_CovenData.Name;
        m_SubTitle.text = "";

        m_Home.m_Motto.text = string.IsNullOrEmpty(m_CovenData.Motto) ? "\"" + LocalizeLookUp.GetText("coven_motto_here") + "\"" : "\"" + m_CovenData.Motto + "\"";
        m_Home.m_FounderName.text = LocalizeLookUp.GetText("coven_founder") + " " + m_CovenData.Founder.Name;
        m_Home.m_Worldrank.text = LocalizeLookUp.GetText("lt_world_rank") + " " + m_CovenData.WorldRank.ToString();
        m_Home.m_DominionRank.text = LocalizeLookUp.GetText("lt_dominion_rank") + " " + m_CovenData.DominionRank.ToString();
        m_Home.m_CreatedOn.text = LocalizeLookUp.GetText("coven_creation") + " " + Utilities.ShowDateTimeWithCultureInfo(m_CovenData.CreatedOn);
        m_Home.m_PopCount.text = "";
        m_Home.m_CovenDegree.text = Utilities.GetSchoolCoven(m_CovenData.School);
        m_Home.m_CreatorDegree.text = Utilities.GetSchool(m_CovenData.Founder.School);

        if (m_CovenData.School < 0)
            m_Home.m_CovenSigil.sprite = m_Home.shadowSchool;
        else if (m_CovenData.School > 0)
            m_Home.m_CovenSigil.sprite = m_Home.whiteSchool;
        else
            m_Home.m_CovenSigil.sprite = m_Home.greySchool;

        if (m_CovenData.Founder.School < 0)
            m_Home.m_CreatorSigil.sprite = m_Home.shadowSchool;
        else if (m_CovenData.Founder.School > 0)
            m_Home.m_CreatorSigil.sprite = m_Home.whiteSchool;
        else
            m_Home.m_CreatorSigil.sprite = m_Home.greySchool;

        //setup buttons
        DisableButtons();
        m_DisbandCovenButton.gameObject.SetActive(m_CovenData.IsMember && TeamManager.MyRole >= CovenRole.ADMIN);
        m_ViewRequestsButton.gameObject.SetActive(m_CovenData.IsMember && TeamManager.MyRole >= CovenRole.MODERATOR);
        m_SendInviteButton.gameObject.SetActive(m_CovenData.IsMember && TeamManager.MyRole >= CovenRole.MODERATOR);
        m_LeaveCovenButton.gameObject.SetActive(m_CovenData.IsMember);
        m_ViewMembersButton.gameObject.SetActive(true);
    }

    private void SetupMembers()
    {
        //setup members
        foreach (Transform item in m_Members.m_Container.transform)
        {
            m_Members.m_Pool.Despawn(item.GetComponent<TeamMemberItemUI>());
        }
        foreach (TeamMemberData member in m_CovenData.Members)
        {
            TeamMemberItemUI item = m_Members.m_Pool.Spawn(m_Members.m_Container.transform);
            item.Setup(
                member,
                m_CovenData.IsMember,
                (memberData) => Debug.LogError("TODO: SHOWPLAYER"));
        }

        //setup buttons
        DisableButtons();

        m_BackButton.onClick.RemoveAllListeners();
        m_BackButton.onClick.AddListener(() => SetScreen(Screen.HOME));

        m_BackButton.gameObject.SetActive(true);
    }

    private void SetupInvites()
    {
        //setup invites screen
        m_CovenName.text = LocalizeLookUp.GetText("coven_screen_invite"); // no invite
        m_SubTitle.text = LocalizeLookUp.GetText("cast_screen_no_coven"); // no clans

        //setup bottom
        DisableButtons();
        m_CreateCovenButton.gameObject.SetActive(true);
        m_SendRequestButton.gameObject.SetActive(true);
    }

    private void SetupRequests()
    {

    }

    private void OnClickCreate()
    {
        //show popup with the name inptu field
        m_InputPopup.ShowPopUp(
            confirmAction: (covenName) =>
            {
                m_InputPopup.Error("");
                string nameError = LoginUtilities.ValidateCovenName(covenName);

                if (string.IsNullOrEmpty(nameError))
                {
                    m_LoadingObj.SetActive(true);
                    //send the coven request
                    TeamManager.CreateCoven(
                        covenName,
                        (coven, error) =>
                        {
                            if (string.IsNullOrEmpty(error))
                            {
                                m_InputPopup.Close();
                                SetScreen(Screen.HOME);
                            }
                            else
                            {
                                m_InputPopup.Error(error);
                            }
                            m_LoadingObj.SetActive(false);
                        });
                }
                else
                {
                    m_InputPopup.Error(LocalizeLookUp.GetText("character_special_char"));
                }
            },
            cancelAction: () =>
            {
            },
            txt: LocalizeLookUp.GetText("coven_invite_choose"));
    }

    private void OnClickDisband()
    {
        UIGlobalErrorPopup.ShowPopUp(
            confirmAction: () =>
            {
                m_LoadingObj.SetActive(true);
                TeamManager.DisbandCoven((result, response) =>
                {
                    if (result == 200)
                    {
                        UIGlobalErrorPopup.ShowPopUp(null, LocalizeLookUp.GetText("coven_disband_success"));
                    }
                    else
                    {
                        UIGlobalErrorPopup.ShowError(null, APIManager.ParseError(response));
                    }
                    m_LoadingObj.SetActive(false);
                });
            },
            cancelAction: () =>
            {
            },
            LocalizeLookUp.GetText("coven_disband"));
    }

    private void OnClickSendInvite()
    {
        m_InputPopup.ShowPopUp(
           confirmAction: (characterName) =>
           {
               m_InputPopup.Error("");
               string nameError = LoginUtilities.ValidateCharacterName(characterName);

               if (string.IsNullOrEmpty(nameError))
               {
                   //m_LoadingObj.SetActive(true);
                   //send the coven invitation
                   //TeamManager.Reques
               }
               else
               {
                   m_InputPopup.Error(nameError);
               }
           },
           cancelAction: () =>
           {
           },
           txt: LocalizeLookUp.GetText("coven_invite_choose"));
    }

    private void OnClickMembers()
    {
        SetScreen(Screen.MEMBERS);
    }


    private void Show(string covenId)
    {
        m_Canvas.enabled = true;
        m_InputRaycaster.enabled = true;
        LeanTween.cancel(m_TweenId);
        m_TweenId = LeanTween.alphaCanvas(m_MainCanvasGroup, 1f, 1f)
            .setEaseOutCubic()
            .setOnComplete(() => MapsAPI.Instance.HideMap(true))
            .uniqueId;


        if (string.IsNullOrEmpty(covenId) || covenId == TeamManager.MyCovenId)
        {
            m_CovenData = TeamManager.MyCoven;
            if (m_CovenData == null)
            {
                //show no coven
                SetScreen(Screen.INVITES);
            }
            else
            {
                //show my coven
                SetScreen(Screen.HOME);
            }
        }
        else
        {
            m_LoadingObj.SetActive(true);
            TeamManager.GetCoven(covenId, (covenData, error) =>
            {
                if (string.IsNullOrEmpty(error))
                {
                    //show home
                    m_CovenData = covenData;
                    SetScreen(Screen.HOME);
                }
                else
                {
                    //close the ui and show error
                    UIGlobalErrorPopup.ShowError(null, error);
                    Hide();
                }
                m_LoadingObj.SetActive(false);
            });
        }
    }

    private void Hide()
    {
        SetScreen(Screen.NONE);

        LeanTween.cancel(m_TweenId);
        m_InputRaycaster.enabled = false;
        m_TweenId = LeanTween.alphaCanvas(m_MainCanvasGroup, 0, 1f)
             .setEaseOutCubic()
             .setOnComplete(() =>
             {
                 m_Canvas.enabled = false;
             })
             .uniqueId;

        MapsAPI.Instance.HideMap(false);
    }

    public static void Open(string covenId)
    {
        if (m_Instance != null)
        {
            m_Instance.Show(covenId);
        }
        else
        {
            //load the coven scene
            LoadingOverlay.Show();
            SceneManager.LoadSceneAsync(
                SceneManager.Scene.COVEN_MANAGEMENT,
                UnityEngine.SceneManagement.LoadSceneMode.Additive,
                (progress) =>
                {
                },
                () =>
                {
                    m_Instance.Show(covenId);
                    LoadingOverlay.Hide();
                });
        }
    }

#if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField] private string m_DebugString;
    
    [ContextMenu("Get Coven")]
    private void DebugGet()
    {
        if (IsOpen)
        {
            Hide();
            LeanTween.value(0, 0, 0.5f).setOnComplete(() => Show(m_DebugString));
        }
        else
        {
            Show(m_DebugString);
        }
    }
#endif
}