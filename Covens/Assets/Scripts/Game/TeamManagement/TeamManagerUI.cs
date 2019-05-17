using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
using Newtonsoft.Json;
using Oktagon.Localization;
using TMPro;

[RequireComponent(typeof(TeamManager))]
[RequireComponent(typeof(TeamUIHelper))]
public class TeamManagerUI : MonoBehaviour
{
    public static TeamManagerUI Instance { get; set; }

    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private GraphicRaycaster m_InputRaycaster;
    [SerializeField] private CanvasGroup m_CanvasGroup;
    [SerializeField] private RectTransform m_RectTransform;

    [SerializeField] private TeamConfirmPopUp confirmPopup;
    [SerializeField] private TeamInputPopup inputPopup;

    public static TeamConfirmPopUp ConfirmPopup { get { return Instance.confirmPopup; } }
    public static TeamInputPopup InputPopup { get { return Instance.inputPopup; } }

    [Header("Footer Buttons")]
    public GameObject[] allButtons;
    public Button btnInvite;
    public Button btnPending;
    public Button btnEdit;
    public Button btnRequests;
    public Button btnAllies;
    public Button btnAllied;
    public Button btnLeave;
    public Button btnBack;
    public Button btnCreate;
    public Button btnRequest;
    public Button btnAlly;
    public Button btnDisband;
    public Button closeButton;
    public Button btnCovenInfo;
    public Button btnMotto;
    public Button btnMembers;
    public Button btnLeaderboards;
    public Button btnRequestInvite;

    public TextMeshProUGUI covenTitle;
    public TextMeshProUGUI subTitle;

    public GameObject titleInCoven;
    public GameObject titleLocation;
    public GameObject titleCovenReq;
    public GameObject titlePlayer;

    public RectTransform loadingUIRect;

    public enum ScreenType
    {
        Members,
        CharacterInvite,
        CovenDisplay,
        CovenAllies,
        CovenAllied,
        EditCoven,
        RequestsCoven,
        Leaderboard,
        Locations,
        /*CovenInfoSelf, CovenInfoOther,*/
        InvitesCoven,
    }

    public static TeamData teamData = null;
    public static bool isOpen { get; private set; }

    public ScreenType currentScreen { get; private set; }
    public ScreenType previousScreen { get; private set; }

    bool isCoven
    {
        get { return PlayerDataManager.playerData.covenName == "" ? false : true; }
    }

    private string selectedPlayerID;
    public string selectedCovenID;

    void Awake()
    {
        Instance = this;

        if (confirmPopup == null)
            confirmPopup = GetComponentInChildren<TeamConfirmPopUp>();
        if (inputPopup == null)
            inputPopup = GetComponentInChildren<TeamInputPopup>();

        currentScreen = ScreenType.CovenDisplay;
        previousScreen = ScreenType.CovenDisplay;

        covenTitle.text = "";
        subTitle.text = "";

        m_Canvas.enabled = false;
        m_InputRaycaster.enabled = false;
        m_CanvasGroup.alpha = 0;
        m_RectTransform.localScale = Vector3.zero;
    }

    void Start()
    {
        closeButton.onClick.AddListener(Close);
        btnCreate.onClick.AddListener(CreateCovenRequest);
        btnBack.onClick.AddListener(GoBack);
        btnRequest.onClick.AddListener(RequestInvite);
        btnCovenInfo.onClick.AddListener(() => ShowCovenInfo(PlayerDataManager.playerData.covenName));
        btnLeave.onClick.AddListener(SendCovenLeave);
        btnRequests.onClick.AddListener(() => { SetScreenType(ScreenType.RequestsCoven); });
        btnPending.onClick.AddListener(() => { SetScreenType(ScreenType.InvitesCoven); });
       // btnAllied.onClick.AddListener(() => { SetScreenType(ScreenType.CovenAllies); });
       // btnAllies.onClick.AddListener(() => { SetScreenType(ScreenType.CovenAllied); });
        btnInvite.onClick.AddListener(SendInvite);
        btnAlly.onClick.AddListener(SendCovenAlly);
        btnEdit.onClick.AddListener(() => SetScreenType(ScreenType.EditCoven));
        btnDisband.onClick.AddListener(CovenDisbandRequest);
        btnMotto.onClick.AddListener(ChangeMotto);
        btnMembers.onClick.AddListener(() => SetScreenType(ScreenType.Members));
        btnLeaderboards.onClick.AddListener(OnClickLeaderboard);
        btnRequestInvite.onClick.AddListener(OnClickRequestInvite);
    }

    void Setloading(bool isLoading)
    {
        if (isLoading)
        {
            loadingUIRect.gameObject.SetActive(true);
            loadingUIRect.GetComponent<CanvasGroup>().alpha = 0;
            loadingUIRect.localScale = Vector3.zero;
            LTDescr descrAlpha = LeanTween.alphaCanvas(loadingUIRect.GetComponent<CanvasGroup>(), 1, .28f).setEase(LeanTweenType.easeInOutSine);
            LTDescr descrScale = LeanTween.scale(loadingUIRect, Vector3.one, .4f).setEase(LeanTweenType.easeInOutSine);
        }
        else
        {
            LTDescr descrAlpha = LeanTween.alphaCanvas(loadingUIRect.GetComponent<CanvasGroup>(), 0, .28f).setEase(LeanTweenType.easeInOutSine);
            LTDescr descrScale = LeanTween.scale(loadingUIRect, Vector3.zero, .4f).setEase(LeanTweenType.easeInOutSine);
            descrAlpha.setOnComplete(() => { loadingUIRect.gameObject.SetActive(false); });
        }
    }

    void Rebuild()
    {
        Setloading(true);
        SetBodyHeader();
        DisableButtons();
        setHeaderBtn(false);
        if (currentScreen == ScreenType.CharacterInvite)
        {
            TeamManager.GetCharacterInvites(CharaterInviteUI);
        }
        else if (currentScreen == ScreenType.CovenDisplay)
        {
            TeamManager.GetCovenDisplay(DisplayCovenUI, selectedCovenID);
        }
        else if (currentScreen == ScreenType.CovenAllies)
        {
            TeamManager.GetCovenAllies(CovenAlliesUI);
        }
        else if (currentScreen == ScreenType.CovenAllied)
        {
            TeamManager.GetCovenAllied(CovenAlliedUI);
        }
        else if (currentScreen == ScreenType.EditCoven)
        {
            EditCovenUI();
        }
        else if (currentScreen == ScreenType.RequestsCoven)
        {
            TeamManager.GetCovenRequests(RequestCovenUI);
        }
        else if (currentScreen == ScreenType.InvitesCoven)
        {
            TeamManager.GetCovenInvites(InviteCovenUI);
			//TeamManager.GetCovenRequests(RequestCovenUI);

        }
        //else if (currentScreen == ScreenType.Leaderboard)
        //{
        //    TeamManager.GetTopCovens(LeaderboardUI, (errorCode) => { });
        //}
        else if (currentScreen == ScreenType.Locations)
        {
            TeamManager.GetPlacesOfPower(LocationsUI, selectedCovenID);
        }
        //else if (currentScreen == ScreenType.CovenInfoOther)
        //{
        //    TeamManager.GetCovenDisplay(CovenInfoUIOther, selectedCovenID);
        //}
        //else if (currentScreen == ScreenType.CovenInfoSelf)
        //{
        //    CovenInfoUI();
        //}
        else if (currentScreen == ScreenType.Members)
        {
            TeamManager.GetCovenDisplay(CovenMembersUI, selectedCovenID);
        }

        previousScreen = currentScreen;
    }

    private void OnClickRequestInvite()
    {
        confirmPopup.ShowPopUp(
            confirmAction: () => SendRequestInvite(selectedCovenID),
            cancelAction: () => { },
            txt: "Send request to join \"" + selectedCovenID + "\"?"
        );
    }

    public void OnClickLeaderboard()
    {
        UIMain.Instance.CreateLeaderboardsCoven();
        Leaderboards.Instance.ShowCovens();
    }

    public void ShowPlacesOfPower(string covenId)
    {
        selectedCovenID = covenId;
        SetScreenType(ScreenType.Locations);
    }

    #region CovenCreate

    public void CreateCovenRequest()
    {
        inputPopup.ShowPopUp(SendCovenCreateRequest, () => { }, "Choose a name for your coven.");
    }

    void SendCovenCreateRequest(string id)
    {
        Setloading(true);
        PlayerDataManager.playerData.covenName = id;
        selectedCovenID = id;
        TeamManager.CreateCoven(CovenCreateResponse, id);
    }

    void CovenCreateResponse(int responseCode)
    {
        Setloading(false);
        if (responseCode == 200)
        {
            inputPopup.Close();
            SetScreenType(ScreenType.CovenDisplay);
            ChatConnectionManager.Instance.SendCovenChange();
        }
        else
        {
            PlayerDataManager.playerData.covenName = "";
            if (responseCode == 4301)
            {
                inputPopup.Error("Coven name in use.");
            }
            else if (responseCode == 4104)
            {
                inputPopup.Error("Coven name is invalid.");
            }
            else if (responseCode == 4100)
            {
                inputPopup.Error("Coven name is empty");
            }
            else
            {
                inputPopup.Error("Error Code : " + responseCode.ToString());
            }
        }

    }

    #endregion

    #region RequestInvite

    public void RequestInvite()
    {
        inputPopup.ShowPopUp(SendRequestInvite, () => { }, "Enter the name of coven you to join.");
    }

    void SendRequestInvite(string id)
    {
        Setloading(true);
        TeamManager.RequestInvite(RequestInviteResponse, id);
    }

    void RequestInviteResponse(int responseCode)
    {
        Setloading(false);
        Debug.Log(responseCode);
        if (responseCode == 200)
        {
            inputPopup.Close();
            confirmPopup.ShowPopUp(() => { }, "Request sent successfully.");
        }
        else if (responseCode == 4805)
        {
            if (inputPopup.isOpen) inputPopup.Error("Request already Sent");
            if (confirmPopup.isOpen) confirmPopup.ShowPopUp(() => { }, "Request already Sent");
        }
        else if (responseCode == 4809)
        {
            if (inputPopup.isOpen) inputPopup.Error("Coven is full.");
            if (confirmPopup.isOpen) confirmPopup.ShowPopUp(() => { }, "Coven is full.");
        }
        else if (responseCode == 4301 || responseCode == 4300)
        {
            if (inputPopup.isOpen) inputPopup.Error("Coven not found");
        }
        else
        {
            if (inputPopup.isOpen) inputPopup.Error("Error Code : " + responseCode.ToString());
            if (confirmPopup.isOpen) confirmPopup.ShowPopUp(() => { }, "Error Code : " + responseCode.ToString());
        }

    }

    #endregion

    #region SendInvite

    public void SendInvite()
    {
        inputPopup.ShowPopUp(SendInviteRequest, () => { }, "Enter the name of player to invite.");
    }

    void SendInviteRequest(string id)
    {
        Setloading(true);
        TeamManager.InviteCoven(InviteResponse, id);
    }

    void InviteResponse(int responseCode)
    {
        Setloading(false);

        if (responseCode == 200)
        {
            inputPopup.Close();
            confirmPopup.ShowPopUp(() => { SetScreenType(ScreenType.InvitesCoven); }, "Invite sent successfully.");
        }
        else if (responseCode == 4803)
        {
            inputPopup.Error("Request already Sent");
        }
        else if (responseCode == 4809)
        {
            inputPopup.Error("Coven is full.");
        }
        else if (responseCode == 4802 || responseCode == 4800)
        {
            inputPopup.Error("Not Authorized");
        }
        else if (responseCode == 4301)
        {
            inputPopup.Error("Player not found");
        }
        else if (responseCode == 4300)
        {
            inputPopup.Error("Player name is empty");
        }
        else
        {
            inputPopup.Error("Error Code : " + responseCode.ToString());
        }

    }

    #endregion

    #region Join [player]

    public void SendJoin()
    {
        confirmPopup.ShowPopUp(JoinRequest, () => { SetScreenType(currentScreen); }, "Do you want to join this coven?");
    }

    void JoinRequest()
    {
        Setloading(true);
        TeamManager.CovenReject(JoinResponse, selectedPlayerID);
    }

    void JoinResponse(int responseCode)
    {
        Setloading(false);

        if (responseCode == 200)
        {
            confirmPopup.Close();
            PlayerDataManager.playerData.covenName = selectedPlayerID;
            confirmPopup.ShowPopUp(() => { SetScreenType(ScreenType.CovenDisplay); }, "Successfully joined coven!");
        }
        else if (responseCode == 4807)
        {
            confirmPopup.Error("Invite was cancelled by the coven.");
        }
        else if (responseCode == 4804)
        {
            confirmPopup.Error("Coven was disbanded.");
        }
        else if (responseCode == 4809)
        {
            confirmPopup.Error("Coven is full.");
        }
        else
        {
            inputPopup.Error("Error Code : " + responseCode.ToString());
        }
    }

    #endregion

    #region DeclineInvite [player]

    public void SendDeclineInvite()
    {
        confirmPopup.ShowPopUp(DeclineInviteRequest, () => { SetScreenType(currentScreen); }, "Do you want to decline this invite?");
    }

    void DeclineInviteRequest()
    {
        Setloading(true);
        TeamManager.CovenReject(DeclineInviteResponse, selectedPlayerID);
    }

    void DeclineInviteResponse(int responseCode)
    {
        Setloading(false);

        if (responseCode == 200)
        {
            confirmPopup.Close();
            confirmPopup.ShowPopUp(() => { SetScreenType(currentScreen); }, "Successfully declined the invite.");
        }
        else if (responseCode == 4807)
        {
            confirmPopup.Error("Invite was cancelled by the coven.");
        }
        else if (responseCode == 4804)
        {
            confirmPopup.Error("Coven was disbanded.");
        }
        else
        {
            inputPopup.Error("Error Code : " + responseCode.ToString());
        }
    }

    #endregion

    #region CancelInvite [coven]

    public void SendCancel(TeamInvites invite)
    {
        selectedPlayerID = invite.displayName;
        confirmPopup.ShowPopUp(() => CancelInviteRequest(invite.inviteToken), () => { }, "Do you want to cancel this invite?");
    }

    void CancelInviteRequest(string inviteToken)
    {
        Setloading(true);
        TeamManager.CovenCancel(CancelInviteResponse, inviteToken);
    }

    void CancelInviteResponse(int responseCode)
    {
        Setloading(false);

        if (responseCode == 200 || responseCode == 4807)
        {
            confirmPopup.ShowPopUp(() => { SetScreenType(ScreenType.InvitesCoven); }, "Successfully cancelled the invite.");
        }
        else if (responseCode == 4800)
        {
            confirmPopup.Error("Not Authorized");
        }
        else
        {
            inputPopup.Error("Error Code : " + responseCode.ToString());
        }
    }

    #endregion

    #region RejectInviteRequest [coven]

    public void SendRejectInvite()
    {
        confirmPopup.ShowPopUp(RejectInviteRequest, () => { SetScreenType(currentScreen); }, "Do you want to decline this invite?");
    }

    void RejectInviteRequest()
    {
        Setloading(true);
        TeamManager.CovenReject(RejectInviteResponse, selectedPlayerID);
    }

    void RejectInviteResponse(int responseCode)
    {
        Setloading(false);

        if (responseCode == 200 || responseCode == 4807)
        {
            confirmPopup.Close();
            confirmPopup.ShowPopUp(() => { SetScreenType(currentScreen); }, "Successfully declined the invite.");
        }
        else if (responseCode == 4800)
        {
            confirmPopup.Error("Not Authorized");
        }
        else
        {
            inputPopup.Error("Error Code : " + responseCode.ToString());
        }
    }

    #endregion

    #region CovenLeave

    public void SendCovenLeave()
    {
        confirmPopup.ShowPopUp(CovenLeaveRequest, () => { }, "Do you want to leave your coven?");
    }

    void CovenLeaveRequest()
    {
        Setloading(true);
        TeamManager.CovenLeave(CovenLeaveResponse);
    }

    void CovenLeaveResponse(int responseCode)
    {
        Setloading(false);

        if (responseCode == 200 || responseCode == 4802)
        {
            PlayerDataManager.playerData.covenName = "";
            ChatConnectionManager.Instance.SendCovenChange();
            TeamManager.CovenData = null;
            confirmPopup.ShowPopUp(() => { SetScreenType(ScreenType.CharacterInvite); }, "Successfully left the coven.");
        }
        else
        {
            inputPopup.Error("Error Code : " + responseCode.ToString());
        }
    }

    #endregion

    #region CovenDisband

    public void SendCovenDisband()
    {
        confirmPopup.ShowPopUp(CovenDisbandRequest, () => { SetScreenType(currentScreen); }, "Do you want to disband your coven?");
    }

    void CovenDisbandRequest()
    {
        Setloading(true);
        TeamManager.CovenDisband(CovenDisbandResponse);
    }

    void CovenDisbandResponse(int responseCode)
    {
        Setloading(false);

        if (responseCode == 200 || responseCode == 4804)
        {
            PlayerDataManager.playerData.covenName = "";
            confirmPopup.ShowPopUp(() => { SetScreenType(ScreenType.CharacterInvite); }, "Coven successfully disbanded.");                 //check allied coven and coven allied
        }
        else if (responseCode == 4802 || responseCode == 4800)
        {
            confirmPopup.Error("Not Authorized");
        }
        else
        {
            inputPopup.Error("Error Code : " + responseCode.ToString());
        }

    }


    #endregion

    #region CovenAllyInputField

    public void CovenAllyInputField()
    {
        inputPopup.ShowPopUp(SendCovenAllyRequestInputField, () => { SetScreenType(currentScreen); }, "Do you want to ally with this coven?");
    }

    void SendCovenAllyRequestInputField(string id)
    {
        Setloading(true);
        TeamManager.RequestInvite(CovenAllyResponseInputField, id);
    }

    void CovenAllyResponseInputField(int responseCode)
    {
        Setloading(false);

        if (responseCode == 200 || responseCode == 4808)
        {
            inputPopup.Close();
            confirmPopup.ShowPopUp(() => { SetScreenType(currentScreen); }, "coven successfully unallied.");               //check allied coven and coven allied
        }
        else if (responseCode == 4804)
        {
            inputPopup.Error("Coven was disbanded.");
        }
        else if (responseCode == 4802 || responseCode == 4800)
        {
            inputPopup.Error("Not Authorized");
        }
        else
        {
            inputPopup.Error("Error Code : " + responseCode.ToString());
        }

    }

    #endregion

    #region CovenAlly

    public void SendCovenAlly()
    {
        inputPopup.ShowPopUp(SendCovenAllyRequest, () => { SetScreenType(currentScreen); }, "Do you want to ally with this coven?");
    }

    public void SendCovenAllyRequest(string covenName)
    {
        Setloading(true);
        selectedCovenID = covenName;
        TeamManager.AllyCoven(CovenAllyResponse, covenName);
    }

    void CovenAllyResponse(int responseCode)
    {
        Setloading(false);

        string errorMessage = null;
        if (responseCode == 200 || responseCode == 4808)
        {
            inputPopup.Close();
            confirmPopup.ShowPopUp(() => { SetScreenType(currentScreen); }, "Coven successfully allied.");                 //check allied coven and coven allied
        }
        else if (responseCode == 4804)
        {
            errorMessage = "Coven was disbanded.";
        }
        else if (responseCode == 4802 || responseCode == 4800)
        {
            errorMessage = "Not Authorized";
        }
        else
        {
            errorMessage = "Error Code : " + responseCode.ToString();
        }

        if (errorMessage != null)
        {
            confirmPopup.Error(errorMessage);
            inputPopup.Error(errorMessage);
        }
    }

    #endregion

    #region CovenUnally

    public void SendCovenUnally(string id)
    {
        Setloading(true);
        TeamManager.UnallyCoven(CovenUnallyResponse, id);
    }

    void CovenUnallyResponse(int responseCode)
    {
        Setloading(false);

        if (responseCode == 200 || responseCode == 4808)
        {
            confirmPopup.ShowPopUp(() => { SetScreenType(currentScreen); }, "Coven successfully unallied.");              //check allied coven and coven allied
        }
        else if (responseCode == 4804)
        {
            confirmPopup.Error("Coven was disbanded.");
        }
        else if (responseCode == 4802 || responseCode == 4800)
        {
            confirmPopup.Error("Not Authorized");
        }
        else
        {
            inputPopup.Error("Error Code : " + responseCode.ToString());
        }

    }

    #endregion

    #region ViewPlayer

    public void SendViewCharacter(string id)
    {
        Debug.Log("sending Character");
        Setloading(true);
        TeamManager.ViewCharacter(id, GetViewCharacter);
    }

    public void GetViewCharacter(MarkerDataDetail player, int resultCode)
    {
        Setloading(false);
        if (resultCode == 200)
        {
            TeamPlayerView.Instance.Setup(player);
        }
        else
        {
            confirmPopup.ShowPopUp(() => { }, "Error: " + resultCode);
        }
    }

    #endregion

    #region EditMember

    //promote

    public void SendPromote(string playerName, TeamManager.CovenRole role)
    {
        string roleName = role.ToString();
        string promoteText = "Do you wanna promote <name> to <role>?"
            .Replace("<name>", playerName)
            .Replace("<role>", roleName);

        confirmPopup.ShowPopUp(
            () =>
            {
                Setloading(true);
                TeamManager.CovenPromote(
                    (result) =>
                    {
                        Setloading(false);
                        if (result == 200)
                        {
                            confirmPopup.ShowPopUp(() => { }, "<player> was promoted to <role>.".Replace("<player>", playerName).Replace("<role>", roleName));
                        }
                        else
                        {
                            string errorMessage = "Error: " + result;
                            confirmPopup.Error(errorMessage);
                        }
                    },
                    playerName,
                    role
                );
            },
            () => { },
            promoteText
        );
    }

    public void SendDemote(string playerName, TeamManager.CovenRole role)
    {
        string roleName = role.ToString();
        string demoteText = "Do you wanna demote <name> to <role>?"
            .Replace("<name>", playerName)
            .Replace("<role>", roleName);

            confirmPopup.ShowPopUp(
                () =>
                {
                    Setloading(true);
                    //TeamManager.CovenPromote(
                    //    (result) =>
                    //    {
                    //        Setloading(false);
                    //        if (result == 200)
                    //        {
                    //            confirmPopup.ShowPopUp(() => { }, "<player> was demoted to <role>.".Replace("<player>", playerName).Replace("<role>", roleName));
                    //        }
                    //        else
                    //        {
                    //            string errorMessage = "Error: " + result;
                    //            confirmPopup.Error(errorMessage);
                    //        }
                    //    },
                    //    playerName,
                    //    role
                    //);
                },
                () => { },
                demoteText
            );
    }

    //kick

    public void KickCovenMember(string playerName, Action onKick)
    {
        string kickText = "Click Yes to remove <name> form the Coven.".Replace("<name>", playerName); //Click Yes to remove <name> form the Coven.
        confirmPopup.ShowPopUp(() => SendKick(playerName, onKick), () => { }, kickText);
    }

    private void SendKick(string playerName, Action onKick)
    {
        Setloading(true);
        TeamManager.CovenKick(result => OnSendKickResponse(result, playerName, onKick), playerName);
    }

    private void OnSendKickResponse(int result, string playerName, Action onKick)
    {
        Setloading(false);
        if (result == 200)
        {
            //remove from the cached member list
            for (int i = 0; i < TeamManager.CovenData.members.Count; i++)
            {
                if (TeamManager.CovenData.members[i].displayName == playerName)
                {
                    TeamManager.CovenData.members.RemoveAt(i);
                    break;
                }
            }
            confirmPopup.ShowPopUp(() => { }, "<name> was kicked out form the coven.".Replace("<name>", playerName)); //<name> was kicked out form the coven.
            onKick?.Invoke();
        }
        else //show error message
        {
            string errorMessage = "Error: " + result;
            confirmPopup.Error(errorMessage);
        }
    }

    #endregion

    public void ChangeMotto()
    {
        inputPopup.ShowPopUp(
            confirmAction: (value) =>
            {
                if (value != TeamManager.CovenData.motto)
                {
                    SetMottoRequest(value);
                }
            },
            cancelAction: () => { },
            txt: "What is your coven's motto.",
            initialInput: TeamManager.CovenData.motto
        );
    }

    private void SetMottoRequest(string motto)
    {
        Setloading(true);
        TeamManager.SetMotto(result => SetMottoResponse(result, motto), motto);
    }

    private void SetMottoResponse(int result, string motto)
    {
        Setloading(false);
        if (result == 200)
        {
            inputPopup.Close();
            confirmPopup.ShowPopUp(() => { }, "Motto succesfully set.");
            TeamManager.CovenData.motto = motto;
            TeamCovenView.Instance.SetMotto(TeamManager.CovenData);
        }
        else
        {
            inputPopup.Error("Error: " + result);
        }
    }

    public void ShowCovenInfo(string covenName)
    {
        selectedCovenID = covenName;

        if (covenName == PlayerDataManager.playerData.covenName)
        {
            TeamCovenView.Instance.Show(TeamManager.CovenData);
        }
        else
        {
            Setloading(true);
            TeamManager.GetCovenDisplay(
                (teamData) =>
                {
                    if (teamData != null)
                    {
                        TeamCovenView.Instance.Show(teamData);
                    }
                    Setloading(false);
                },
                covenName
            );
        }
    }

    public void SetScreenType(ScreenType screenType)
    {
        if (screenType != ScreenType.CovenDisplay)
            TeamCovenView.Instance.Close();
        currentScreen = screenType;
        Rebuild();
    }

    #region UI Setup Methods

    void LocationsUI(TeamLocation[] data)
    {
        Setloading(false);
        btnBack.gameObject.SetActive(true);
        string popRewards = "Total Silver: " + teamData.totalSilver.ToString() + "  |  Total Gold: " + teamData.totalGold.ToString() + "  |  Total Energy: " + teamData.totalEnergy.ToString();
        SetHeader("Places of Power", popRewards);
        TeamUIHelper.Instance.CreateLocations(data);
    }

	void InviteCovenUI(TeamInvites[] data)
    {
        Setloading(false);
        btnBack.gameObject.SetActive(true);
        btnInvite.gameObject.SetActive(true);
        SetHeader("Invites to Players", PlayerDataManager.playerData.covenName);
        TeamUIHelper.Instance.CreateInvites(data);
		btnMembers.gameObject.SetActive (false);

    }

    void RequestCovenUI(TeamInviteRequest[] data)
    {
        Setloading(false);
        btnBack.gameObject.SetActive(true);
        SetHeader("Join Requests", PlayerDataManager.playerData.covenName);
        TeamUIHelper.Instance.CreateRequests(data);
		btnMembers.gameObject.SetActive (false);

    }

    void CovenAlliesUI(TeamAlly[] data)
    {
        bool showInviteAlly = TeamManager.CurrentRole >= TeamManager.CovenRole.Moderator;

        Setloading(false);
        SetHeader("Ally Covens", PlayerDataManager.playerData.covenName);
        btnBack.gameObject.SetActive(true);
        btnAlly.gameObject.SetActive(showInviteAlly);
        TeamUIHelper.Instance.CreateAllies(data);
    }

    void CovenAlliedUI(TeamAlly[] data)
    {
        SetHeader("Covens allied with you", PlayerDataManager.playerData.covenName);
        Setloading(false);
        btnBack.gameObject.SetActive(true);
        TeamUIHelper.Instance.CreateAllied(data);
    }

    void CharaterInviteUI(TeamInvites[] data)
    {
        Setloading(false);
        TeamUIHelper.Instance.CreateInvites(data);
        SetHeader("Invites", "No Coven");
        btnCreate.gameObject.SetActive(true);
        btnRequest.gameObject.SetActive(true);
		btnMembers.gameObject.SetActive (false);

    }

    void DisplayCovenUI(TeamData data)
    {
        Setloading(false);

        if (data == null)
        {
            confirmPopup.ShowPopUp(() => Close(), "Coven not found");
            return;
        }

        TeamUIHelper.Instance.clearContainer();
        teamData = data;
        SetDisplayCovenButtons(data);
        SetHeader();
        setHeaderBtn(false);
        TeamCovenView.Instance.Show(data);
		Debug.Log ("DisplayCovenUI()");
    }

    void CovenMembersUI(TeamData data)
    {
        Setloading(false);
        teamData = data;
        btnBack.gameObject.SetActive(true);
        SetHeader();
        setHeaderBtn(false);
        TeamUIHelper.Instance.CreateMembers(data.members);
		btnMembers.gameObject.SetActive (false);
		Debug.Log ("CovenMembersUI()");
    }

    void EditCovenUI()
    {
        Setloading(false);
        SetHeader();
        setHeaderBtn(true);

        btnBack.gameObject.SetActive(true);
        btnDisband.gameObject.SetActive(TeamManager.CurrentRole >= TeamManager.CovenRole.Administrator);
        btnMotto.gameObject.SetActive(TeamManager.CurrentRole >= TeamManager.CovenRole.Administrator);

        TeamUIHelper.Instance.CreateMembers(TeamManager.CovenData.members);

        foreach (TeamItemData item in TeamUIHelper.Instance.uiItems.Values)
        {
            item.EnableEdit(true);
        }
		Debug.Log ("EditCovenUI()");
    }

    //void DisplayCovenUIOther(TeamData data)
    //{
    //    Setloading(false);
    //    teamData = data;
    //    SetHeader(data.covenName, data.dominion);
    //    setHeaderBtn(true);
    //    btnBack.gameObject.SetActive(true);
    //}


    void SetBodyHeader()
    {
        titleCovenReq.SetActive(false);
        titleInCoven.SetActive(false);
        titlePlayer.SetActive(false);
        titleLocation.SetActive(false);

        switch (currentScreen)
        {
            case ScreenType.CovenAllies:
            case ScreenType.CovenAllied:
            case ScreenType.CharacterInvite:
                titleCovenReq.SetActive(true);
                break;
            case ScreenType.Members:
            case ScreenType.EditCoven:
                titleInCoven.SetActive(true);
                break;
            case ScreenType.Locations:
                titleLocation.SetActive(true);
                break;
            case ScreenType.InvitesCoven:
            case ScreenType.RequestsCoven:
                titlePlayer.SetActive(true);
                break;
        }
    }

    private void SetDisplayCovenButtons(TeamData data)
    {
		Debug.Log ("SetDisplayCovenButtons()");
        //if viweing own coven
        if (data.covenName == PlayerDataManager.playerData.covenName)
        {
            TeamManager.CovenRole CurrentRole = TeamManager.CurrentRole;
            bool showPlayerInvites = CurrentRole >= TeamManager.CovenRole.Member;
            bool showAllies = CurrentRole >= TeamManager.CovenRole.Member;
            bool showEdit = CurrentRole >= TeamManager.CovenRole.Moderator;

            btnRequests.gameObject.SetActive(showPlayerInvites);
            btnPending.gameObject.SetActive(showPlayerInvites);
           // btnAllied.gameObject.SetActive(showAllies);
           // btnAllies.gameObject.SetActive(showAllies);
            btnLeave.gameObject.SetActive(true);
            btnEdit.gameObject.SetActive(showEdit);
            //btnMembers.gameObject.SetActive(true);
			if (btnEdit.gameObject.activeSelf == true) {
				btnMembers.gameObject.SetActive (false);
				Debug.Log ("if");
			}
			else {
				btnEdit.gameObject.SetActive (false);
				btnMembers.gameObject.SetActive (true);
				Debug.Log ("else");
			}
        }
        else //if viewing other coven
        {
            bool showRequestInvite = string.IsNullOrEmpty(PlayerDataManager.playerData.covenName);

			btnEdit.gameObject.SetActive (true);
            btnMembers.gameObject.SetActive(false);
            btnRequestInvite.gameObject.SetActive(showRequestInvite);
        }
    }

    void DisableButtons()
    {
        foreach (var item in allButtons)
        {
            item.SetActive(false);
        }
		//btnEdit.gameObject.SetActive (false);
        //btnMembers.gameObject.SetActive(true);
        btnMotto.gameObject.SetActive(false);
        btnRequestInvite.gameObject.SetActive(false);
		Debug.Log ("DisableButtons()");
    }

    void GoBack()
    {
        //return to main screen
        if (isCoven || string.IsNullOrEmpty(selectedCovenID) == false)
            SetScreenType(ScreenType.CovenDisplay);
        else
            SetScreenType(ScreenType.CharacterInvite);
    }

    void GoBack(ScreenType screenType)
    {
        SetScreenType(screenType);
    }

    #endregion

    public void Show(string covenName = null)
    {
        if (string.IsNullOrEmpty(covenName))
            covenName = PlayerDataManager.playerData.covenName;

        selectedCovenID = covenName;

        m_Canvas.enabled = true;
        m_InputRaycaster.enabled = true;

        LTDescr descrAlpha = LeanTween.alphaCanvas(m_CanvasGroup, 1, .28f).setEase(LeanTweenType.easeInOutSine);
        LTDescr descrScale = LeanTween.scale(m_RectTransform, Vector3.one, .4f).setEase(LeanTweenType.easeInOutSine)
            .setOnComplete(() =>
            {
                UIStateManager.Instance.CallWindowChanged(false);
                MapsAPI.Instance.HideMap(true);
            });
        GoBack();
        isOpen = true;
    }

    public void Close()
    {
        m_InputRaycaster.enabled = false;
        MapsAPI.Instance.HideMap(false);
        UIStateManager.Instance.CallWindowChanged(true);

        LTDescr descrAlpha = LeanTween.alphaCanvas(m_CanvasGroup, 0, .28f).setEase(LeanTweenType.easeInOutSine);
        LTDescr descrScale = LeanTween.scale(m_RectTransform.GetComponent<RectTransform>(), Vector3.zero, .4f).setEase(LeanTweenType.easeInOutSine);
        descrScale.setOnComplete(() =>
        {
            m_Canvas.enabled = false;
        });
        isOpen = false;
    }

    void SetHeader(string title, string subtitle)
    {
        covenTitle.text = title;
        subTitle.text = subtitle;
    }

    void SetHeader()
    {
        covenTitle.text = teamData.covenName;
        subTitle.text = "Total Silver: " + teamData.totalSilver.ToString() + "  |  Total Gold: " + teamData.totalGold.ToString() + "  |  Total Energy: " + teamData.totalEnergy.ToString();
    }

    void setHeaderBtn(bool active)
    {
        //btnCovenInfo.GetComponent<Text>().raycastTarget = active;
    }

    public static string GetTimeStamp(double javaTimeStamp)
    {
        if (javaTimeStamp < 159348924)
        {
            string s = "unknown";
            return s;
        }
        System.DateTime dtDateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Local);
        dtDateTime = dtDateTime.AddMilliseconds(javaTimeStamp).ToLocalTime();

        return dtDateTime.ToString("d");
    }
}

