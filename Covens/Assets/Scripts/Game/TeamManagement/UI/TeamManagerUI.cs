using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Raincrow;
using Raincrow.Team;
using Raincrow.GameEventResponses;

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
        public Button m_MottoButton;

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
        public TextMeshProUGUI m_HeaderLevel;
        public TextMeshProUGUI m_HeaderName;
        public LayoutGroup m_Container;
        public TeamInviteItemUI m_ItemPrefab;
        public SimplePool<TeamInviteItemUI> m_Pool;
    }

    [System.Serializable]
    public struct RequestsProperties
    {
        public CanvasGroup m_CanvasGroup;
        public TextMeshProUGUI m_HeaderLevel;
        public TextMeshProUGUI m_HeaderName;
        public LayoutGroup m_Container;
        public TeamInviteItemUI m_ItemPrefab;
        public SimplePool<TeamInviteItemUI> m_Pool;
    }

    [Header("General")]
    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private GraphicRaycaster m_InputRaycaster;
    [SerializeField] private CanvasGroup m_MainCanvasGroup;
    [SerializeField] private TeamInputPopup m_InputPopup;

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
    [SerializeField] private Button m_JoinCovenButton;
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

    public static Screen CurrentScreen
    {
        get
        {
            if (IsOpen == false)
                return Screen.NONE;
            else
                return m_Instance.m_CurrentScreen;
        }
    }

    private TeamData m_CovenData = null;
    private CanvasGroup[] m_Screens;
    private Screen m_CurrentScreen;
    private System.Action m_OnClose;
    private int m_TweenId;
    private int m_ScreenTweenId;

    public static void OpenName(string covenName, System.Action onClose = null)
    {
        if (string.IsNullOrEmpty(covenName))
        {
            Debug.LogError("NULL NAME");
            return;
        }
        
        LoadScene(() =>
        {
            m_Instance.SetScreen(Screen.NONE);
            m_Instance.Show();
            m_Instance.m_OnClose = onClose;

            if (TeamManager.MyCovenData != null && covenName == TeamManager.MyCovenData.Name) //show the player's coven
            {
                Open(TeamManager.MyCovenData.Id, onClose);
            }
            else //get the coven from server
            {
                LoadingOverlay.Show();
                TeamManager.GetCoven(covenName, true, (covenData, error) =>
                {
                    LoadingOverlay.Hide();
                    if (string.IsNullOrEmpty(error))
                        m_Instance.Show(covenData);
                    else
                        UIGlobalErrorPopup.ShowError(m_Instance.OnClickClose, error);
                });
            }
        });
    }
    
    public static void Open(string covenId, System.Action onClose = null)
    {
        LoadScene(() =>
        {
            //open empty screen
            m_Instance.SetScreen(Screen.NONE);
            m_Instance.Show();
            m_Instance.m_OnClose = onClose;

            if (string.IsNullOrEmpty(covenId) || covenId == TeamManager.MyCovenId) //show the player's coven
            {
                if (string.IsNullOrEmpty(TeamManager.MyCovenId)) //player has no coven
                {
                    m_Instance.Show(null);
                }
                else
                {
                    if (TeamManager.MyCovenData == null)//try to request coven data from server
                    {
                        LoadingOverlay.Show();
                        TeamManager.GetCoven(TeamManager.MyCovenId, false, (covenData, error) =>
                        {
                            LoadingOverlay.Hide();
                            if (string.IsNullOrEmpty(error))
                                m_Instance.Show(covenData);
                            else
                                UIGlobalErrorPopup.ShowError(m_Instance.OnClickClose, error);
                        });
                    }
                    else //show the cached coven
                    {
                        m_Instance.Show(TeamManager.MyCovenData);
                    }
                }
            }
            else //get the coven from server
            {
                LoadingOverlay.Show();
                TeamManager.GetCoven(covenId, false, (covenData, error) =>
                {
                    LoadingOverlay.Hide();
                    if (string.IsNullOrEmpty(error))
                        m_Instance.Show(covenData);
                    else
                        UIGlobalErrorPopup.ShowError(m_Instance.OnClickClose, error);
                });
            }
        });
    }
    
    private static void LoadScene(System.Action onComplete)
    {
        if (m_Instance != null)
        {
            onComplete?.Invoke();
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
                    LoadingOverlay.Hide();
                    onComplete?.Invoke();
                });
        }
    }

    private void Awake()
    {
        m_Instance = this;

        m_Canvas.enabled = false;
        m_InputRaycaster.enabled = false;

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
        HideButtons();

        //listeners
        m_CloseButton.onClick.AddListener(OnClickClose);
        m_LeaderboardButton.gameObject.SetActive(false);

        m_CreateCovenButton.onClick.AddListener(OnClickCreate);
        m_JoinCovenButton.onClick.AddListener(OnClickJoin);
        m_SendRequestButton.onClick.AddListener(OnClickSendRequest);
        m_SendInviteButton.onClick.AddListener(OnClickSendInvite);
        m_DisbandCovenButton.onClick.AddListener(OnClickDisband);
        m_LeaveCovenButton.onClick.AddListener(OnClickLeave);

        m_BackButton.onClick.AddListener(() => SetScreen(Screen.HOME));
        m_ViewMembersButton.onClick.AddListener(() =>
        {
            if (m_CurrentScreen != Screen.MEMBERS)
                SetScreen(Screen.MEMBERS);
        });
        m_ViewInvitesButton.onClick.AddListener(() =>
        {
            if (m_CurrentScreen != Screen.INVITES)
                SetScreen(Screen.INVITES);
        });
        m_ViewRequestsButton.onClick.AddListener(() =>
        {
            if (m_CurrentScreen != Screen.REQUESTS)
                SetScreen(Screen.REQUESTS);
        });

        m_Home.m_MottoButton.onClick.AddListener(OnClickMotto);

        //setup pools
        m_Members.m_Pool = new SimplePool<TeamMemberItemUI>(m_Members.m_ItemPrefab, 5);
        m_Requests.m_Pool = new SimplePool<TeamInviteItemUI>(m_Requests.m_ItemPrefab, 5);
        m_Invites.m_Pool = new SimplePool<TeamInviteItemUI>(m_Invites.m_ItemPrefab, 5);

        GameResyncHandler.OnResyncStart += Hide;
    }

    private void Show()
    {
        m_Canvas.enabled = true;
        m_InputRaycaster.enabled = true;
        LeanTween.cancel(m_TweenId);
        m_TweenId = LeanTween.alphaCanvas(m_MainCanvasGroup, 1f, 1f)
            .setEaseOutCubic()
            .setOnComplete(() => MapsAPI.Instance.HideMap(true))
            .uniqueId;
    }

    private void Show(TeamData coven)
    {
        m_CovenData = coven;
        if (coven == null || coven == TeamManager.MyCovenData)
        {
            EnableEventListeners(false);
            EnableEventListeners(true);
        }

        Show();

        if (coven == null)
            SetScreen(Screen.INVITES);
        else
            SetScreen(Screen.HOME);
    }

    private void Hide()
    {
        EnableEventListeners(false);
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

    private void SetScreen(Screen screen)
    {
        Screen previousScreen = m_CurrentScreen;
        m_CurrentScreen = screen;
        
        LeanTween.cancel(m_ScreenTweenId, true);
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

    private void HideButtons()
    {
        m_CreateCovenButton.gameObject.SetActive(false);
        m_JoinCovenButton.gameObject.SetActive(false);
        m_ViewMembersButton.gameObject.SetActive(false);
        m_BackButton.gameObject.SetActive(false);
        m_LeaveCovenButton.gameObject.SetActive(false);
        m_DisbandCovenButton.gameObject.SetActive(false);
        m_ViewRequestsButton.gameObject.SetActive(false);
        m_ViewInvitesButton.gameObject.SetActive(false);
        m_SendInviteButton.gameObject.SetActive(false);
        m_SendRequestButton.gameObject.SetActive(false);

        //disable the highlights
        m_BackButton.transform.GetChild(0).gameObject.SetActive(false);
        m_CreateCovenButton.transform.GetChild(0).gameObject.SetActive(false);
        m_JoinCovenButton.transform.GetChild(0).gameObject.SetActive(false);
        m_SendRequestButton.transform.GetChild(0).gameObject.SetActive(false);
        m_ViewRequestsButton.transform.GetChild(0).gameObject.SetActive(false);
        m_SendInviteButton.transform.GetChild(0).gameObject.SetActive(false);
        m_ViewInvitesButton.transform.GetChild(0).gameObject.SetActive(false);
        m_ViewMembersButton.transform.GetChild(0).gameObject.SetActive(false);
        m_LeaveCovenButton.transform.GetChild(0).gameObject.SetActive(false);
        m_DisbandCovenButton.transform.GetChild(0).gameObject.SetActive(false);
    }

    private void EnableEventListeners(bool enable)
    {
        if (enable)
        {
            CovenDisbandHandler.OnCovenDisband += OnDisband;
            CovenInviteCancelHandler.OnCovenInviteCancel += OnInviteRemove;
            CovenInviteCancelMembersHandler.OnPlayerInviteCancel += OnInviteRemove;
            CovenInviteDeclineHandler.OnPlayerInviteDecline += OnInviteRemove;
            CovenInviteHandler.OnCovenInviteReceive += OnCovenInviteReceive;
            CovenInviteMembersHandler.OnInviteSent += OnCovenInviteSent;
            CovenJoinHandler.OnCovenJoin += OnJoinCoven;
            CovenJoinMembersHandler.OnNewMember += OnMemberAdd;
            CovenKickHandler.OnMemberKick += OnMemberRemove;
            CovenLeaveHandler.OnMemberLeave += OnMemberRemove;
            CovenMottoHandler.OnMottoChange += OnMottoChange;
            CovenRequestMembers.OnReceiveRequest += OnCovenRequestReceive;
            CovenRequestRejectHandler.OnRequestReject += OnRequestRemove;
            CovenRequestRejectMembersHandler.OnRequestReject += OnRequestRemove;
            CovenTitleHandler.OnTitleChange += OnMemberRefresh;
            CovenRoleHandler.OnRoleChange += OnMemberRefresh;
        }
        else
        {
            CovenDisbandHandler.OnCovenDisband -= OnDisband;
            CovenInviteCancelHandler.OnCovenInviteCancel -= OnInviteRemove;
            CovenInviteCancelMembersHandler.OnPlayerInviteCancel -= OnInviteRemove;
            CovenInviteDeclineHandler.OnPlayerInviteDecline -= OnInviteRemove;
            CovenInviteHandler.OnCovenInviteReceive -= OnCovenInviteReceive;
            CovenInviteMembersHandler.OnInviteSent -= OnCovenInviteSent;
            CovenJoinHandler.OnCovenJoin -= OnJoinCoven;
            CovenJoinMembersHandler.OnNewMember -= OnMemberAdd;
            CovenKickHandler.OnMemberKick -= OnMemberRemove;
            CovenLeaveHandler.OnMemberLeave -= OnMemberRemove;
            CovenMottoHandler.OnMottoChange -= OnMottoChange;
            CovenRequestMembers.OnReceiveRequest -= OnCovenRequestReceive;
            CovenRequestRejectHandler.OnRequestReject -= OnRequestRemove;
            CovenRequestRejectMembersHandler.OnRequestReject -= OnRequestRemove;
            CovenTitleHandler.OnTitleChange -= OnMemberRefresh;
            CovenRoleHandler.OnRoleChange -= OnMemberRefresh;
        }
    }
       
    private void OnJoinCoven(string covenName)
    {
        //reopen the UI
        if (m_CovenData == null)
            UIGlobalErrorPopup.ShowPopUp(() => Open(null), LocalizeLookUp.GetText("coven_invite_join_success"));
    }
    private void OnDisband()
    {
        //reopen the UI
        if (m_CovenData != null)
            UIGlobalErrorPopup.ShowError(() => Open(null), LocalizeLookUp.GetText("coven_disbanded"));
    }
    private void OnInviteRemove(string id)
    {
        if (m_CurrentScreen != Screen.INVITES)
            return;

        //disable the invite interaction
        var invites = m_Invites.m_Pool.GetInstances();
        foreach(TeamInviteItemUI item in invites)
        {
            if (item.ItemId == id)
            {
                item.Disable(true);
                break;
            }
        }
    }
    private void OnCovenInviteReceive(CovenInvite covenInvite)
    {
        if (m_CurrentScreen != Screen.INVITES)
            return;

        SpawnInvite(covenInvite);
    }
    private void OnCovenInviteSent(PendingInvite pendingInvite)
    {
        if (m_CurrentScreen != Screen.INVITES)
            return;

        SpawnInvite(pendingInvite);
    }
    private void OnMemberAdd(TeamMemberData member)
    {
        if (m_CurrentScreen != Screen.MEMBERS)
            return;

        SpawnMember(member);
    }
    private void OnMemberRemove(string name)
    {
        if (name == PlayerDataManager.playerData.name)
        {
            UIGlobalErrorPopup.ShowError(() => Open(null), LocalizeLookUp.GetText("coven_member_remove_success").Replace("{{name}}", name));
            return;
        }

        if (m_CurrentScreen != Screen.MEMBERS)
            return;

        var members = m_Members.m_Pool.GetInstances();
        foreach(TeamMemberItemUI member in members)
        {
            if (member.MemberData.Name == name)
            {
                member.Disable(true);
                break;
            }
        }
    }
    private void OnMottoChange(string motto)
    {
        if (m_CurrentScreen != Screen.HOME)
            return;

        if (m_CovenData == null)
            return;

        SetupHome();
    }
    private void OnCovenRequestReceive(PendingRequest pendingRequest)
    {
        if (m_CurrentScreen != Screen.REQUESTS)
            return;

        SpawnRequest(pendingRequest);
    }
    private void OnRequestRemove(string id)
    {
        if (m_CurrentScreen != Screen.REQUESTS)
            return;

        //disable the invite interaction
        var requests = m_Requests.m_Pool.GetInstances();
        foreach (TeamInviteItemUI item in requests)
        {
            if (item.ItemId == id)
            {
                item.Disable(true);
                break;
            }
        }
    }
    private void OnMemberRefresh(string id)
    {
        //refresh everything in case something important changed (like my role)
        if (id == PlayerDataManager.playerData.instance)
            RefreshMembers(null);
        //update the view for that player only
        else
            RefreshMembers(id);
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
                    LoadingOverlay.Show();
                    //send the coven request
                    TeamManager.CreateCoven(
                        covenName,
                        (coven, error) =>
                        {
                            LoadingOverlay.Hide();
                            if (string.IsNullOrEmpty(error))
                            {
                                m_InputPopup.Close();
                                Open(null);
                            }
                            else
                            {
                                m_InputPopup.Error(error);
                            }
                        });
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

    private void OnClickClose()
    {
        m_OnClose?.Invoke();
        m_OnClose = null;
        Hide();
    }

    #region COVEN HOME

    private void SetupHome()
    {
        if (m_CovenData == null)
        {
            Hide();
            return;
        }

        HideButtons();
        m_ViewMembersButton.gameObject.SetActive(true);
        m_JoinCovenButton.gameObject.SetActive(m_CovenData.IsMember == false);
        m_LeaveCovenButton.gameObject.SetActive(m_CovenData.IsMember);
        m_DisbandCovenButton.gameObject.SetActive(m_CovenData.IsMember && TeamManager.MyRole >= CovenRole.ADMIN);
        m_ViewRequestsButton.gameObject.SetActive(m_CovenData.IsMember);
        m_ViewInvitesButton.gameObject.SetActive(m_CovenData.IsMember);
        
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
    }
    
    private void OnClickLeave()
    {
        UIGlobalErrorPopup.ShowPopUp(
           confirmAction: () =>
           {
               LoadingOverlay.Show();
               TeamManager.LeaveCoven((error) =>
               {
                   LoadingOverlay.Hide();
                   if (string.IsNullOrEmpty(error))
                   {
                       UIGlobalErrorPopup.ShowPopUp(() => Show(null), LocalizeLookUp.GetText("coven_leave_success"));
                   }
                   else
                   {
                       UIGlobalErrorPopup.ShowError(null, error);
                   }
               });
           },
           cancelAction: () =>
           {
           },
           LocalizeLookUp.GetText("coven_leave"));
    }

    private void OnClickDisband()
    {
        UIGlobalErrorPopup.ShowPopUp(
            confirmAction: () =>
            {
                LoadingOverlay.Show();
                TeamManager.DisbandCoven((result, response) =>
                {
                    if (result == 200)
                    {
                        UIGlobalErrorPopup.ShowPopUp(() => Show(null), LocalizeLookUp.GetText("coven_disband_success"));
                    }
                    else
                    {
                        UIGlobalErrorPopup.ShowError(null, APIManager.ParseError(response));
                    }
                    LoadingOverlay.Hide();
                });
            },
            cancelAction: () =>
            {
            },
            LocalizeLookUp.GetText("coven_disband"));
    }

    private void OnClickJoin()
    {
        if (PlayerDataManager.playerData.covenRequests.Exists(req => req.coven == m_CovenData.Id))
        {
            UIGlobalErrorPopup.ShowError(null, LocalizeLookUp.GetText("coven_request_sent"));
            return;
        }

        UIGlobalErrorPopup.ShowPopUp(
            confirmAction: () =>
            {
                LoadingOverlay.Show();
                TeamManager.SendRequest(m_CovenData.Id, false, (request, error) =>
                {
                    LoadingOverlay.Hide();
                    if (string.IsNullOrEmpty(error))
                    {
                        UIGlobalErrorPopup.ShowPopUp(null, LocalizeLookUp.GetText("coven_request_success"));
                    }
                    else
                    {
                        UIGlobalErrorPopup.ShowError(null, error);
                    }
                });
            },
            cancelAction: () =>
            {
            },
            LocalizeLookUp.GetText("invite_request").Replace("{{Coven}}", m_CovenData.Name));
    }

    private void OnClickMotto()
    {
        m_InputPopup.ShowPopUp(
            txt: LocalizeLookUp.GetText("coven_motto"),
            confirmAction: (motto) =>
            {
                m_InputPopup.error.text = "";

                LoadingOverlay.Show();
                TeamManager.ChangeMotto(motto, (error) =>
                {
                    LoadingOverlay.Hide();
                    if (string.IsNullOrEmpty(error))
                    {
                        m_InputPopup.Close();
                        UIGlobalErrorPopup.ShowPopUp(
                            () =>
                            {
                                if (m_CurrentScreen == Screen.HOME)
                                    SetupHome();
                            }, 
                            LocalizeLookUp.GetText("coven_motto_set"));
                    }
                    else
                    {
                        m_InputPopup.Error(error);
                    }
                });
            },
            cancelAction: () =>
            {
            });
    }

    #endregion


    #region MEMBERS SCREEN

    private void SetupMembers()
    {
        HideButtons();
        m_LeaveCovenButton.gameObject.SetActive(m_CovenData.IsMember);
        m_JoinCovenButton.gameObject.SetActive(m_CovenData.IsMember == false);
        m_BackButton.gameObject.SetActive(true);

        m_SubTitle.text = m_CovenData.Name;
        m_CovenName.text = LocalizeLookUp.GetText("invite_member").Replace("{{member}}", "");

        //setup members
        m_Members.m_Pool.DespawnAll();
        foreach (TeamMemberData data in m_CovenData.Members)
            SpawnMember(data);
    }

    private TeamMemberItemUI SpawnMember(TeamMemberData member)
    {
        TeamMemberItemUI item = m_Members.m_Pool.Spawn(m_Members.m_Container.transform);
        item.name = "[covenMember] " + member.Name;
        item.Setup(
            m_CovenData,
            member,
            OnSelectMember,
            OnKickMember,
            OnPromoteMember,
            OnDemoteMember,
            OnClickMemberTitle);
        return item;
    }

    private void OnSelectMember(TeamMemberItemUI member)
    {
        LoadingOverlay.Show();
        TeamPlayerView.ViewCharacter(member.MemberData.Id, (m, s) => LoadingOverlay.Hide());
    }

    private void OnClickMemberTitle(TeamMemberItemUI member)
    {
        m_InputPopup.ShowPopUp(
            (title) =>
            {
                LoadingOverlay.Show();
                TeamManager.ChangeMemberTitle(member.MemberData, title, (error) =>
                {
                    LoadingOverlay.Hide();
                    if (string.IsNullOrEmpty(error))
                    {
                        m_InputPopup.Close();
                        member.Refresh();
                    }
                    else
                    {
                        m_InputPopup.Error(error);
                    }
                });
            },
            () => { },
            LocalizeLookUp.GetText("coven_set_title"));
    }

    private void RefreshMembers(string id)
    {
        //update the ui
        if (m_CurrentScreen != Screen.MEMBERS)
            return;

        foreach (Transform child in m_Members.m_Container.transform)
        {
            TeamMemberItemUI item = child.GetComponent<TeamMemberItemUI>();

            if (item.MemberData == null)
                continue;

            if (string.IsNullOrEmpty(id) == false && item.MemberData.Id != id)
                continue;

            item.Refresh();
        }
    }

    private void OnKickMember(string id)
    {
        //update the ui
        if (m_CurrentScreen != Screen.MEMBERS)
            return;

        for (int i = 0; i < m_Members.m_Container.transform.childCount; i++)
        {
            TeamMemberItemUI item = m_Members.m_Container.transform.GetChild(i).GetComponent<TeamMemberItemUI>();

            if (item.MemberData == null)
                continue;

            if (item.MemberData.Id == id)
                m_Members.m_Pool.Despawn(item);
            else
                item.Refresh();
        }
    }

    private void OnPromoteMember(string id, CovenRole role)
    {
        RefreshMembers(id);
    }

    private void OnDemoteMember(string id, CovenRole role)
    {
        RefreshMembers(id);
    }

    private void OnChangeMemberTitle(string id, string title)
    {
        RefreshMembers(id);
    }

    #endregion


    #region INVITES SCREEN

    private void SetupInvites()
    {
        HideButtons();
        m_Invites.m_Pool.DespawnAll();

        if (m_CovenData == null) //show the player's received invitations
        {
            m_CreateCovenButton.gameObject.SetActive(true);
            m_ViewRequestsButton.gameObject.SetActive(true);
            m_ViewInvitesButton.gameObject.SetActive(true);
            m_SendRequestButton.gameObject.SetActive(true);

            m_ViewInvitesButton.transform.GetChild(0).gameObject.SetActive(true);

            m_CovenName.text = LocalizeLookUp.GetText("coven_screen_invite");
            m_SubTitle.text = LocalizeLookUp.GetText("cast_screen_no_coven");
            m_Invites.m_HeaderLevel.text = LocalizeLookUp.GetText("generic_rank");
            m_Invites.m_HeaderName.text = LocalizeLookUp.GetText("chat_coven");

            foreach (var covenInvite in PlayerDataManager.playerData.covenInvites)
                SpawnInvite(covenInvite);
        }
        else //shoe the coven's sent invitation
        {
            m_BackButton.gameObject.SetActive(true);
            m_SendInviteButton.gameObject.SetActive(true);    

            m_CovenName.text = LocalizeLookUp.GetText("header_invites_players");
            m_SubTitle.text = m_CovenData.Name;
            m_Invites.m_HeaderLevel.text = LocalizeLookUp.GetText("card_witch_level");
            m_Invites.m_HeaderName.text = LocalizeLookUp.GetText("name");

            foreach (var pendingInvite in m_CovenData.PendingInvites)
                SpawnInvite(pendingInvite);
        }
    }

    private TeamInviteItemUI SpawnInvite(CovenInvite covenInvite)
    {
        TeamInviteItemUI item = m_Invites.m_Pool.Spawn(m_Invites.m_Container.transform);
        item.Setup(
            covenInvite,
            onAccept: () => Open(null),
            onReject: () => item.Disable(true)
        );
        return item;
    }

    private TeamInviteItemUI SpawnInvite(PendingInvite pendingInvite)
    {
        TeamInviteItemUI item = m_Invites.m_Pool.Spawn(m_Invites.m_Container.transform);
        item.Setup(
            pendingInvite,
            onCancel: () => item.Disable(true));
        return item;
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
                   //send the invite
                   LoadingOverlay.Show();
                   TeamManager.SendInvite(characterName, true, (invite, error) =>
                   {
                       LoadingOverlay.Hide();
                       if (string.IsNullOrEmpty(error))
                       {
                           m_InputPopup.Close();
                           //reopen the UI
                           UIGlobalErrorPopup.ShowPopUp(() => SetScreen(Screen.INVITES), LocalizeLookUp.GetText("coven_invite_success"));
                       }
                       else
                       {
                           m_InputPopup.Error(error);
                       }
                   });
               }
               else
               {
                   //show the invaldi name error
                   m_InputPopup.Error(nameError);
               }
           },
           cancelAction: () =>
           {
           },
           txt: LocalizeLookUp.GetText("coven_invite"));
    }

    #endregion


    #region REQUESTS SCREEN

    private void SetupRequests()
    {
        HideButtons();
        m_Requests.m_Pool.DespawnAll();

        if (m_CovenData == null)
        {
            m_CreateCovenButton.gameObject.SetActive(true);
            m_ViewRequestsButton.gameObject.SetActive(true);
            m_ViewInvitesButton.gameObject.SetActive(true);
            m_SendRequestButton.gameObject.SetActive(true);

            m_ViewRequestsButton.transform.GetChild(0).gameObject.SetActive(true);

            m_CovenName.text = LocalizeLookUp.GetText("header_requests");
            m_SubTitle.text = LocalizeLookUp.GetText("cast_screen_no_coven");
            m_Requests.m_HeaderLevel.text = LocalizeLookUp.GetText("generic_rank");
            m_Requests.m_HeaderName.text = LocalizeLookUp.GetText("chat_coven");

            //show the player's sent requests
            foreach (var request in PlayerDataManager.playerData.covenRequests)
                SpawnRequest(request);
        }
        else
        {
            m_BackButton.gameObject.SetActive(true);

            m_CovenName.text = LocalizeLookUp.GetText("header_requests");
            m_SubTitle.text = m_CovenData.Name;
            m_Requests.m_HeaderLevel.text = LocalizeLookUp.GetText("card_witch_level");
            m_Requests.m_HeaderName.text = LocalizeLookUp.GetText("name");

            //show the coven's received requests
            foreach (var request in m_CovenData.PendingRequests)
                SpawnRequest(request);
        }
    }

    private TeamInviteItemUI SpawnRequest(CovenRequest covenRequest)
    {
        TeamInviteItemUI item = m_Requests.m_Pool.Spawn(m_Requests.m_Container.transform);
        item.Setup(covenRequest);
        return item;
    }

    private TeamInviteItemUI SpawnRequest(PendingRequest pendingRequest)
    {
        TeamInviteItemUI item = m_Requests.m_Pool.Spawn(m_Requests.m_Container.transform);
        item.Setup(
            pendingRequest,
            onAccept: () => item.Disable(true),
            onReject: () => item.Disable(true)
        );
        return item;
    }

    private void OnClickSendRequest()
    {
        UICovenSearcher.Instance.Show(() => Open(TeamManager.MyCovenId));
    }

    #endregion
}