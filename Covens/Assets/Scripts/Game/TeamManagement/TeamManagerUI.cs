using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
using Newtonsoft.Json;
using Oktagon.Localization;

[RequireComponent(typeof(TeamManager))]
[RequireComponent(typeof(TeamUIHelper))]
public class TeamManagerUI : MonoBehaviour
{
    public static TeamManagerUI Instance { get; set; }

    [SerializeField] private GameObject content;

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

    public Text covenTitle;
    public Text subTitle;

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
    private string selectedCovenID;

    void Awake()
    {
        Instance = this;
        content.SetActive(false);
        currentScreen = ScreenType.CovenDisplay;
        previousScreen = ScreenType.CovenDisplay;
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
        btnAllied.onClick.AddListener(() => { SetScreenType(ScreenType.CovenAllies); });
        btnAllies.onClick.AddListener(() => { SetScreenType(ScreenType.CovenAllied); });
        btnInvite.onClick.AddListener(SendInvite);
        btnAlly.onClick.AddListener(SendCovenAlly);
        btnEdit.onClick.AddListener(() => SetScreenType(ScreenType.EditCoven));
        btnDisband.onClick.AddListener(CovenDisbandRequest);
        btnMotto.onClick.AddListener(OnClickMotto);
        btnMembers.onClick.AddListener(() => SetScreenType(ScreenType.Members));
        btnLeaderboards.onClick.AddListener(OnClickLeaderboard);
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

    public void OnClickLeaderboard()
    {
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
        TeamInputPopup.Instance.ShowPopUp(SendCovenCreateRequest, () => { }, "Choose a name for your coven.");
    }

    void SendCovenCreateRequest(string id)
    {
        Setloading(true);
        PlayerDataManager.playerData.covenName = id;
        TeamManager.CreateCoven(CovenCreateResponse, id);
    }

    void CovenCreateResponse(int responseCode)
    {
        Setloading(false);
        if (responseCode == 200)
        {
            TeamInputPopup.Instance.Close();
            SetScreenType(ScreenType.CovenDisplay);
        }
        else
        {
            PlayerDataManager.playerData.covenName = "";
            if (responseCode == 4103)
            {
                TeamInputPopup.Instance.Error("Coven name in use.");
            }
            else if (responseCode == 4104)
            {
                TeamInputPopup.Instance.Error("Coven name is invalid.");
            }
            else if (responseCode == 4100)
            {
                TeamInputPopup.Instance.Error("Coven name is empty");
            }
            else
            {
                TeamInputPopup.Instance.Error("Error Code : " + responseCode.ToString());
            }
        }

    }

    #endregion

    #region RequestInvite

    public void RequestInvite()
    {
        TeamInputPopup.Instance.ShowPopUp(SendRequestInvite, () => { }, "Enter the name of coven you to join.");
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
            TeamInputPopup.Instance.Close();
            TeamConfirmPopUp.Instance.ShowPopUp(() => { }, "Request sent successfully.");
        }
        else if (responseCode == 4805)
        {
            TeamInputPopup.Instance.Error("Request already Sent");
        }
        else if (responseCode == 4809)
        {
            TeamInputPopup.Instance.Error("Coven is full.");
        }
        else if (responseCode == 4301 || responseCode == 4300)
        {
            TeamInputPopup.Instance.Error("Coven not found");
        }
        else
        {
            TeamInputPopup.Instance.Error("Error Code : " + responseCode.ToString());
        }

    }

    #endregion

    #region SendInvite
    
    public void SendInvite()
    {
        TeamInputPopup.Instance.ShowPopUp(SendInviteRequest, () => { }, "Enter the name of player to invite.");
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
            TeamInputPopup.Instance.Close();
            TeamConfirmPopUp.Instance.ShowPopUp(() => { SetScreenType(ScreenType.InvitesCoven); }, "Invite sent successfully.");
        }
        else if (responseCode == 4803)
        {
            TeamInputPopup.Instance.Error("Request already Sent");
        }
        else if (responseCode == 4809)
        {
            TeamInputPopup.Instance.Error("Coven is full.");
        }
        else if (responseCode == 4802 || responseCode == 4800)
        {
            TeamInputPopup.Instance.Error("Not Authorized");
        }
        else if (responseCode == 4301)
        {
            TeamInputPopup.Instance.Error("Player not found");
        }
        else if (responseCode == 4300)
        {
            TeamInputPopup.Instance.Error("Player name is empty");
        }
        else
        {
            TeamInputPopup.Instance.Error("Error Code : " + responseCode.ToString());
        }

    }

    #endregion

    #region Join [player]

    public void SendJoin()
    {
        TeamConfirmPopUp.Instance.ShowPopUp(JoinRequest, () => { SetScreenType(currentScreen); }, "Do you want to join this coven?");
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
            TeamConfirmPopUp.Instance.Close();
            PlayerDataManager.playerData.covenName = selectedPlayerID;
            TeamConfirmPopUp.Instance.ShowPopUp(() => { SetScreenType(ScreenType.CovenDisplay); }, "Successfully joined coven!");
        }
        else if (responseCode == 4807)
        {
            TeamConfirmPopUp.Instance.Error("Invite was cancelled by the coven.");
        }
        else if (responseCode == 4804)
        {
            TeamConfirmPopUp.Instance.Error("Coven was disbanded.");
        }
        else if (responseCode == 4809)
        {
            TeamConfirmPopUp.Instance.Error("Coven is full.");
        }
        else
        {
            TeamInputPopup.Instance.Error("Error Code : " + responseCode.ToString());
        }
    }

    #endregion

    #region DeclineInvite [player]

    public void SendDeclineInvite()
    {
        TeamConfirmPopUp.Instance.ShowPopUp(DeclineInviteRequest, () => { SetScreenType(currentScreen); }, "Do you want to decline this invite?");
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
            TeamConfirmPopUp.Instance.Close();
            TeamConfirmPopUp.Instance.ShowPopUp(() => { SetScreenType(currentScreen); }, "Successfully declined the invite.");
        }
        else if (responseCode == 4807)
        {
            TeamConfirmPopUp.Instance.Error("Invite was cancelled by the coven.");
        }
        else if (responseCode == 4804)
        {
            TeamConfirmPopUp.Instance.Error("Coven was disbanded.");
        }
        else
        {
            TeamInputPopup.Instance.Error("Error Code : " + responseCode.ToString());
        }
    }

    #endregion

    #region CancelInvite [coven]

    public void SendCancel(TeamInvites invite)
    {
        selectedPlayerID = invite.displayName;
        TeamConfirmPopUp.Instance.ShowPopUp(() => CancelInviteRequest(invite.inviteToken), () => { }, "Do you want to cancel this invite?");
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
            TeamConfirmPopUp.Instance.ShowPopUp(() => { SetScreenType(ScreenType.InvitesCoven); }, "Successfully cancelled the invite.");
        }
        else if (responseCode == 4800)
        {
            TeamConfirmPopUp.Instance.Error("Not Authorized");
        }
        else
        {
            TeamInputPopup.Instance.Error("Error Code : " + responseCode.ToString());
        }
    }

    #endregion

    #region RejectInviteRequest [coven]

    public void SendRejectInvite()
    {
        TeamConfirmPopUp.Instance.ShowPopUp(RejectInviteRequest, () => { SetScreenType(currentScreen); }, "Do you want to decline this invite?");
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
            TeamConfirmPopUp.Instance.Close();
            TeamConfirmPopUp.Instance.ShowPopUp(() => { SetScreenType(currentScreen); }, "Successfully declined the invite.");
        }
        else if (responseCode == 4800)
        {
            TeamConfirmPopUp.Instance.Error("Not Authorized");
        }
        else
        {
            TeamInputPopup.Instance.Error("Error Code : " + responseCode.ToString());
        }
    }

    #endregion

    #region CovenLeave

    public void SendCovenLeave()
    {
        TeamConfirmPopUp.Instance.ShowPopUp(CovenLeaveRequest, () => {}, "Do you want to leave your coven?");
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
            TeamManager.CovenData = null;
            TeamConfirmPopUp.Instance.ShowPopUp(() => { SetScreenType(ScreenType.CharacterInvite); }, "Successfully left the coven.");
        }
        else
        {
            TeamInputPopup.Instance.Error("Error Code : " + responseCode.ToString());
        }
    }

    #endregion

    #region CovenDisband

    public void SendCovenDisband()
    {
        TeamConfirmPopUp.Instance.ShowPopUp(CovenDisbandRequest, () => { SetScreenType(currentScreen); }, "Do you want to disband your coven?");
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
            TeamConfirmPopUp.Instance.ShowPopUp(() => { SetScreenType(ScreenType.CharacterInvite); }, "Coven successfully disbanded.");                 //check allied coven and coven allied
        }
        else if (responseCode == 4802 || responseCode == 4800)
        {
            TeamConfirmPopUp.Instance.Error("Not Authorized");
        }
        else
        {
            TeamInputPopup.Instance.Error("Error Code : " + responseCode.ToString());
        }

    }


    #endregion

    #region CovenAllyInputField

    public void CovenAllyInputField()
    {
        TeamInputPopup.Instance.ShowPopUp(SendCovenAllyRequestInputField, () => { SetScreenType(currentScreen); }, "Do you want to ally with this coven?");
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
            TeamInputPopup.Instance.Close();
            TeamConfirmPopUp.Instance.ShowPopUp(() => { SetScreenType(currentScreen); }, "coven successfully unallied.");               //check allied coven and coven allied
        }
        else if (responseCode == 4804)
        {
            TeamInputPopup.Instance.Error("Coven was disbanded.");
        }
        else if (responseCode == 4802 || responseCode == 4800)
        {
            TeamInputPopup.Instance.Error("Not Authorized");
        }
        else
        {
            TeamInputPopup.Instance.Error("Error Code : " + responseCode.ToString());
        }

    }

    #endregion

    #region CovenAlly

    public void SendCovenAlly()
    {
        TeamInputPopup.Instance.ShowPopUp(SendCovenAllyRequest, () => { SetScreenType(currentScreen); }, "Do you want to ally with this coven?");
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
            TeamInputPopup.Instance.Close();
            TeamConfirmPopUp.Instance.ShowPopUp(() => { SetScreenType(currentScreen); }, "Coven successfully allied.");                 //check allied coven and coven allied
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
            TeamConfirmPopUp.Instance.Error(errorMessage);
            TeamInputPopup.Instance.Error(errorMessage);
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
            TeamConfirmPopUp.Instance.ShowPopUp(() => { SetScreenType(currentScreen); }, "Coven successfully unallied.");              //check allied coven and coven allied
        }
        else if (responseCode == 4804)
        {
            TeamConfirmPopUp.Instance.Error("Coven was disbanded.");
        }
        else if (responseCode == 4802 || responseCode == 4800)
        {
            TeamConfirmPopUp.Instance.Error("Not Authorized");
        }
        else
        {
            TeamInputPopup.Instance.Error("Error Code : " + responseCode.ToString());
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
            TeamConfirmPopUp.Instance.ShowPopUp(() => { }, "Error: " + resultCode);
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

        TeamConfirmPopUp.Instance.ShowPopUp(
            () => {
                Setloading(true);
                TeamManager.CovenPromote(
                    (result) => {
                        Setloading(false);
                        if(result == 200)
                        {
                            TeamConfirmPopUp.Instance.ShowPopUp(() => { }, "<player> was promoted to <role>.".Replace("<player>", playerName).Replace("<role>", roleName));
                        }
                        else
                        {
                            string errorMessage = "Error: " + result;
                            TeamConfirmPopUp.Instance.Error(errorMessage);
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

    //kick

    public void KickCovenMember(string playerName, Action onKick)
    {
        string kickText = "Click Yes to remove <name> form the Coven.".Replace("<name>", playerName); //Click Yes to remove <name> form the Coven.
        TeamConfirmPopUp.Instance.ShowPopUp(() => SendKick(playerName, onKick), () => { }, kickText);
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
            TeamConfirmPopUp.Instance.ShowPopUp(() => { }, "<name> was kicked out form the coven.".Replace("<name>", playerName)); //<name> was kicked out form the coven.
            onKick?.Invoke();
        }
        else //show error message
        {
            string errorMessage = "Error: " + result;
            TeamConfirmPopUp.Instance.Error(errorMessage);
        }
    }

    #endregion

    private void OnClickMotto()
    {
        TeamInputPopup.Instance.ShowPopUp(
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
            TeamInputPopup.Instance.Close();
            TeamConfirmPopUp.Instance.ShowPopUp(() => { }, "Motto succesfully set.");
            TeamManager.CovenData.motto = motto;
        }
        else
        {
            TeamInputPopup.Instance.Error("Error: " + result);
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
    }

    void RequestCovenUI(TeamInviteRequest[] data)
    {
        Setloading(false);
        btnBack.gameObject.SetActive(true);
        SetHeader("Join Requests", PlayerDataManager.playerData.covenName);
        TeamUIHelper.Instance.CreateRequests(data);
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
    }

    void DisplayCovenUI(TeamData data)
    {
        Setloading(false);

        if(data == null)
        {
            TeamConfirmPopUp.Instance.ShowPopUp(() => Close(), "Coven not found");
            return;
        }

        TeamUIHelper.Instance.clearContainer();
        teamData = data;
        SetDisplayCovenButtons(data);
        SetHeader();
        setHeaderBtn(false);
        TeamCovenView.Instance.Show(data);
    }

    void CovenMembersUI(TeamData data)
    {
        Setloading(false);
        teamData = data;
        btnBack.gameObject.SetActive(true);
        SetHeader();
        setHeaderBtn(false);
        TeamUIHelper.Instance.CreateMembers(data.members);
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
        //if viweing own coven
        if (data.covenName == PlayerDataManager.playerData.covenName)
        {
            TeamManager.CovenRole CurrentRole = TeamManager.CurrentRole;
            bool showPlayerInvites = CurrentRole >= TeamManager.CovenRole.Moderator;
            bool showAllies = CurrentRole >= TeamManager.CovenRole.Member;
            bool showEdit = CurrentRole >= TeamManager.CovenRole.Moderator;

            btnRequests.gameObject.SetActive(showPlayerInvites);
            btnPending.gameObject.SetActive(showPlayerInvites);
            btnAllied.gameObject.SetActive(showAllies);
            btnAllies.gameObject.SetActive(showAllies);
            btnLeave.gameObject.SetActive(true);
            btnEdit.gameObject.SetActive(showEdit);
            btnMembers.gameObject.SetActive(true);
        }
        else //if viewing other coven
        {
            btnAllied.gameObject.SetActive(true);
            btnAllies.gameObject.SetActive(true);
            btnMembers.gameObject.SetActive(true);
        }
    }

    void DisableButtons()
    {
        foreach (var item in allButtons)
        {
            item.SetActive(false);
        }
        btnMotto.gameObject.SetActive(false);
    }

    void GoBack()
    {        
        if (currentScreen == ScreenType.EditCoven)
        {
            currentScreen = ScreenType.CovenDisplay;

            //disable the edit options
            foreach (TeamItemData item in TeamUIHelper.Instance.uiItems.Values)
            {
                item.EnableEdit(false);
            }

            //reset the bottom buttons
            DisableButtons();
            if (isCoven)
            {
                SetDisplayCovenButtons(teamData);
            }
            else
            {
                btnCreate.gameObject.SetActive(true);
                btnRequest.gameObject.SetActive(true);
            }

            return;
        }
        
        //return to main screen
        if (isCoven)
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
        if (string.IsNullOrEmpty(covenName ))
            covenName = PlayerDataManager.playerData.covenName;

        selectedCovenID = covenName;

        content.SetActive(true);
        content.GetComponent<CanvasGroup>().alpha = 0;
        content.GetComponent<RectTransform>().localScale = Vector2.zero;
        LTDescr descrAlpha = LeanTween.alphaCanvas(content.GetComponent<CanvasGroup>(), 1, .28f).setEase(LeanTweenType.easeInOutSine);
        LTDescr descrScale = LeanTween.scale(content.GetComponent<RectTransform>(), Vector2.one, .4f).setEase(LeanTweenType.easeInOutSine);
        GoBack();
        isOpen = true;
    }

    public void Close()
    {
        LTDescr descrAlpha = LeanTween.alphaCanvas(content.GetComponent<CanvasGroup>(), 0, .28f).setEase(LeanTweenType.easeInOutSine);
        LTDescr descrScale = LeanTween.scale(content.GetComponent<RectTransform>(), Vector3.zero, .4f).setEase(LeanTweenType.easeInOutSine);
        descrScale.setOnComplete(() =>
        {
            content.SetActive(false);
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
        subTitle.text = teamData.dominion;
    }

    void setHeaderBtn(bool active)
    {
        btnCovenInfo.GetComponent<Text>().raycastTarget = active;
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

