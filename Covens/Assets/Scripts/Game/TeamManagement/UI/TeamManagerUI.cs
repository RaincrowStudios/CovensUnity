using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
using Newtonsoft.Json;
using Oktagon.Localization;
using TMPro;
using Raincrow;

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
        //item prefab
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

    [SerializeField] private Button m_CreateCovenButton;
    [SerializeField] private Button m_SendRequestButton;
    [SerializeField] private Button m_ViewRequestsButton;
    [SerializeField] private Button m_SendInviteButton;
    [SerializeField] private Button m_ViewInvitesButton;
    [SerializeField] private Button m_ViewMembersButton;
    [SerializeField] private Button m_LeaveCovenButton;
    [SerializeField] private Button m_DisbandCovenButton;

    private static TeamManagerUI m_Instance;

    private TeamData m_CovenData = null;
    private CanvasGroup[] m_Screens;
    private Screen m_CurrentScreen;
    private int m_TweenId;
    private int m_ScreenTweenId;

    private void Awake()
    {
        m_Instance = this;

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
        m_CreateCovenButton.gameObject.SetActive(false);
        m_SendRequestButton.gameObject.SetActive(false);
        m_ViewRequestsButton.gameObject.SetActive(false);
        m_SendInviteButton.gameObject.SetActive(false);
        m_ViewInvitesButton.gameObject.SetActive(false);
        m_ViewMembersButton.gameObject.SetActive(false);
        m_LeaveCovenButton.gameObject.SetActive(false);
        m_DisbandCovenButton.gameObject.SetActive(false);

        m_CreateCovenButton.onClick.AddListener(OnClickCreate);
        m_DisbandCovenButton.onClick.AddListener(OnClickDisband);
    }

    private void Start()
    {
        Open(PlayerDataManager.playerData.coven);
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
                    case Screen.HOME: SetupHome(); break;
                    case Screen.MEMBERS: SetupMembers(); break;
                    case Screen.INVITES: SetupInvites(); break;
                    case Screen.REQUESTS: SetupRequests(); break;
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

    private void SetupHome()
    {
    }

    private void SetupMembers()
    {

    }

    private void SetupInvites()
    {
        if (m_CovenData == null)
        {
            //player has no coven


            m_ViewMembersButton.gameObject.SetActive(false);
            m_ViewRequestsButton.gameObject.SetActive(false);
            m_LeaveCovenButton.gameObject.SetActive(false);
            m_DisbandCovenButton.gameObject.SetActive(false);
            m_ViewInvitesButton.gameObject.SetActive(false);

            m_CreateCovenButton.gameObject.SetActive(true);
            m_SendRequestButton.gameObject.SetActive(true);
        }
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
                        (result, response) =>
                        {
                            if (result == 200)
                            {
                                m_InputPopup.Close();
                                SetScreen(Screen.HOME);
                            }
                            else
                            {
                                m_InputPopup.Error(LocalizeLookUp.GetText("error_" + response));
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
                        UIGlobalErrorPopup.ShowError(null, LocalizeLookUp.GetText("error_" + response));
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
       
    private void Show(string covenName)
    {
        Screen screen = Screen.HOME;
        if (string.IsNullOrEmpty(covenName) && string.IsNullOrEmpty(PlayerDataManager.playerData.coven))
        {
            screen = Screen.INVITES;
        }

        m_Canvas.enabled = true;
        m_InputRaycaster.enabled = true;

        m_Canvas.enabled = true;
        m_InputRaycaster.enabled = true;
        m_TweenId = LeanTween.alphaCanvas(m_MainCanvasGroup, 1f, 1f)
            .setEaseOutCubic()
            .uniqueId;

        //if (PlayerDataManager.playerData.coven)
        m_Instance.SetScreen(screen);
    }

    private void Hide()
    {
        LeanTween.cancel(m_TweenId);

        m_InputRaycaster.enabled = false;
        m_TweenId = LeanTween.alphaCanvas(m_MainCanvasGroup, 0, 1.5f)
             .setEaseOutCubic()
             .setOnComplete(() =>
             {
                 m_Canvas.enabled = false;
             })
             .uniqueId;
    }

    public static void Open(string coven)
    {
        //load the login scene
        if (m_Instance != null)
        {
            m_Instance.Show(coven);
        }
        else
        {
            LoadingOverlay.Show();

            SceneManager.LoadSceneAsync(
                SceneManager.Scene.COVEN_MANAGEMENT,
                UnityEngine.SceneManagement.LoadSceneMode.Additive,
                (progress) =>
                {
                },
                () =>
                {
                    m_Instance.Show(coven);
                    LoadingOverlay.Hide();
                });
        }
    }
}