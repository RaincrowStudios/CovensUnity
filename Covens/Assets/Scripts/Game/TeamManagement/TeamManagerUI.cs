using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
using Newtonsoft.Json;

[RequireComponent(typeof(TeamManager))]
[RequireComponent(typeof(TeamUIHelper))]
public class TeamManagerUI : MonoBehaviour
{
    public static TeamManagerUI Instance { get; set; }

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

    public Text covenTitle;
    public Text subTitle;

    public GameObject titleInCoven;
    public GameObject titleLocation;
    public GameObject titleCovenReq;
    public GameObject titlePlayer;

    public RectTransform loadingUIRect;

    public static bool viewPlayer;

    public enum ScreenType
    {
        CharacterInvite, CovenDisplay, CovenDisplayOther, AlliedCoven, CovenAllied, EditCoven, RequestsCoven, Leaderboard, Locations, /*CovenInfoSelf, CovenInfoOther,*/ InvitesCoven
    }

    public static TeamData teamData = null;

    ScreenType currentScreen = ScreenType.CovenDisplay;
    ScreenType previousScreen = ScreenType.CovenDisplay;

    bool isCoven
    {
        get { return PlayerDataManager.playerData.covenName == "" ? false : true; }
    }

    private string selectedPlayerID;
    private string selectedCovenID;

    void Awake()
    {
        Instance = this;
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
        btnAllied.onClick.AddListener(() => { SetScreenType(ScreenType.AlliedCoven); });
        btnAllies.onClick.AddListener(() => { SetScreenType(ScreenType.CovenAllied); });
        btnInvite.onClick.AddListener(SendInvite);
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
            TeamManager.GetCovenDisplay(DisplayCovenUI);
        }
        else if (currentScreen == ScreenType.CovenDisplayOther)
        {
            TeamManager.GetCovenDisplay(DisplayCovenUI, selectedCovenID);
        }
        else if (currentScreen == ScreenType.AlliedCoven)
        {
            TeamManager.GetAlliedCoven(AlliedCovenUI);
        }
        else if (currentScreen == ScreenType.CovenAllied)
        {
            TeamManager.GetCovenAllied(CovenAlliedUI);
        }
        else if (currentScreen == ScreenType.EditCoven)
        {
            // add logic
        }
        else if (currentScreen == ScreenType.RequestsCoven)
        {
            TeamManager.GetCovenRequests(RequestCovenUI);
        }
        else if (currentScreen == ScreenType.InvitesCoven)
        {
            TeamManager.GetCovenInvites(InviteCovenUI);
        }
        else if (currentScreen == ScreenType.Leaderboard)
        {
            TeamManager.GetTopCovens(LeaderboardUI);
        }
        else if (currentScreen == ScreenType.Locations)
        {
            LocationsUI(teamData.controlledLocations);
        }
        //else if (currentScreen == ScreenType.CovenInfoOther)
        //{
        //    TeamManager.GetCovenDisplay(CovenInfoUIOther, selectedCovenID);
        //}
        //else if (currentScreen == ScreenType.CovenInfoSelf)
        //{
        //    CovenInfoUI();
        //}

        previousScreen = currentScreen;
    }

    #region CovenCreate

    public void CreateCovenRequest()
    {
        TeamInputPopup.Instance.ShowPopUp(SendCovenCreateRequest, () => { SetScreenType(ScreenType.CharacterInvite); }, "Choose a name for your coven.");
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
        TeamInputPopup.Instance.ShowPopUp(SendRequestInvite, () => { SetScreenType(ScreenType.CharacterInvite); }, "Enter the name of coven you to join.");
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
            TeamConfirmPopUp.Instance.ShowPopUp(() => { SetScreenType(ScreenType.CharacterInvite); }, "Request sent successfully.");
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
            TeamConfirmPopUp.Instance.ShowPopUp(() => { SetScreenType(ScreenType.CovenDisplay); }, "Invite sent successfully.");
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
            //remove the TeamItemData from the cached list and redraw the UI
            for (int i = 0; i < TeamUIHelper.Instance.lastInvites.Count; i++)
            {
                TeamInvites inviteItem = TeamUIHelper.Instance.lastInvites[i];
                if (inviteItem.displayName == selectedPlayerID)
                {
                    TeamUIHelper.Instance.lastInvites.RemoveAt(i);
                    InviteCovenUI(TeamUIHelper.Instance.lastInvites.ToArray());
                    break;
                }
            }

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
        TeamConfirmPopUp.Instance.ShowPopUp(CovenLeaveRequest, () => { SetScreenType(currentScreen); }, "Do you want to leave your coven?");
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
            Debug.Log("leaving Coven");
            PlayerDataManager.playerData.covenName = "";
            TeamConfirmPopUp.Instance.ShowPopUp(() => { SetScreenType(ScreenType.CharacterInvite); }, "Successfully left the coven.");              //check allied coven and coven allied
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
            TeamConfirmPopUp.Instance.Close();
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
        TeamConfirmPopUp.Instance.ShowPopUp(SendCovenAllyRequest, () => { SetScreenType(currentScreen); }, "Do you want to ally with this coven?");
    }

    void SendCovenAllyRequest()
    {
        Setloading(true);
        TeamManager.RequestInvite(CovenAllyResponse, selectedCovenID);
    }

    void CovenAllyResponse(int responseCode)
    {
        Setloading(false);

        if (responseCode == 200 || responseCode == 4808)
        {
            TeamConfirmPopUp.Instance.Close();
            TeamConfirmPopUp.Instance.ShowPopUp(() => { SetScreenType(currentScreen); }, "Coven successfully allied.");                 //check allied coven and coven allied
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

    #region CovenUnally

    public void SendCovenUnally()
    {
        TeamConfirmPopUp.Instance.ShowPopUp(SendCovenUnallyRequest, () => { SetScreenType(currentScreen); }, "Do you want to unally with this coven?");
    }

    void SendCovenUnallyRequest()
    {
        Setloading(true);
        TeamManager.RequestInvite(CovenUnallyResponse, selectedCovenID);
    }

    void CovenUnallyResponse(int responseCode)
    {
        Setloading(false);

        if (responseCode == 200 || responseCode == 4808)
        {
            TeamConfirmPopUp.Instance.Close();
            TeamConfirmPopUp.Instance.ShowPopUp(() => { SetScreenType(ScreenType.AlliedCoven); }, "Coven successfully unallied.");              //check allied coven and coven allied
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
        TeamManager.ViewCharacter(id);
    }

    public void GetViewCharacter(string s, int r)
    {
        Setloading(false);
        Debug.Log(s);
        if (r == 200)
        {
            TeamPlayerView.Instance.Setup(JsonConvert.DeserializeObject<MarkerDataDetail>(s));
            DisableButtons();
            btnBack.gameObject.SetActive(true);
            viewPlayer = true;
        }
        else
        {
            viewPlayer = false;
        }
    }

    #endregion

    public void ShowCovenInfo(string covenName)
    {
        Setloading(true);
        selectedCovenID = covenName;

        if (covenName == PlayerDataManager.playerData.covenName)
        {
            DisableButtons();
            TeamCovenView.Instance.Show(teamData);
            btnBack.gameObject.SetActive(true);
        }
        else
        {
            Setloading(true);
            TeamManager.GetCovenDisplay(
                (teamData) =>
                {
                    DisableButtons();
                    TeamCovenView.Instance.Show(teamData);
                    SetHeader(teamData.covenName, teamData.dominion);
                    Setloading(false);
                    btnBack.gameObject.SetActive(true);
                },
                covenName
            );
        }
    }

    public void SetScreenType(ScreenType screenType)
    {
        Debug.Log(screenType);
        Debug.Log(PlayerDataManager.playerData.covenName);
        currentScreen = screenType;
        Rebuild();
    }

    #region UI Setup Methods
    
    void CovenInfoUI()
    {
        Setloading(false);
        btnBack.gameObject.SetActive(true);
        SetHeader();
        TeamCovenView.Instance.Show(teamData);
    }

    void DisplayPlayerInfo(MarkerDataDetail data)
    {
        Setloading(false);
        SetHeader(data.displayName, (data.covenName == "" ? "No Coven" : data.covenName));
        btnBack.gameObject.SetActive(true);
    }

    void LocationsUI(TeamLocation[] data)
    {
        Setloading(false);
        btnBack.gameObject.SetActive(true);
        string popRewards = "Total Silver: " + teamData.totalSilver.ToString() + "  |  Total Gold: " + teamData.totalGold.ToString() + "  |  Total Energy: " + teamData.totalEnergy.ToString();
        SetHeader("Places of Power", popRewards);
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

    void AlliedCovenUI(TeamInvites[] data)
    {
        bool showInviteAlly = TeamManager.CurrentRole >= TeamManager.CovenRole.Moderator;

        Setloading(false);
        SetHeader("Ally Covens", PlayerDataManager.playerData.covenName);
        btnBack.gameObject.SetActive(true);
        btnAlly.gameObject.SetActive(showInviteAlly);
        TeamUIHelper.Instance.CreateAllied(data);
    }

    void CovenAlliedUI(TeamInvites[] data)
    {
        bool showInviteAlly = TeamManager.CurrentRole >= TeamManager.CovenRole.Moderator;

        SetHeader("Covens allied with you", PlayerDataManager.playerData.covenName);
        Setloading(false);
        btnBack.gameObject.SetActive(true);
        btnAlly.gameObject.SetActive(showInviteAlly);
        TeamUIHelper.Instance.CreateCovenAllied(data);
    }

    void LeaderboardUI(LeaderboardData[] data)
    {
        SetHeader("Leaderboards", "Top Covens");
        Setloading(false);
        btnBack.gameObject.SetActive(true);
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
        teamData = data;
        SetDisplayCovenButtons(data);
        SetHeader();
        setHeaderBtn(true);
        TeamUIHelper.Instance.CreateMembers(teamData.members);
    }

    void DisplayCovenUIOther(TeamData data)
    {
        Setloading(false);
        teamData = data;
        SetHeader(data.covenName, data.dominion);
        setHeaderBtn(true);
        btnBack.gameObject.SetActive(true);
    }


    void SetBodyHeader()
    {
        titleCovenReq.SetActive(false);
        titleInCoven.SetActive(false);
        titlePlayer.SetActive(false);
        titleLocation.SetActive(false);
        if (currentScreen == ScreenType.AlliedCoven || currentScreen == ScreenType.CovenAllied || currentScreen == ScreenType.CharacterInvite)
        {
            titleCovenReq.SetActive(true);
        }
        else if (currentScreen == ScreenType.CovenDisplay || currentScreen == ScreenType.CovenDisplayOther)
        {
            titleInCoven.SetActive(true);
        }
        else if (currentScreen == ScreenType.Locations)
        {
            titleLocation.SetActive(true);
        }
        else if (currentScreen == ScreenType.InvitesCoven || currentScreen == ScreenType.RequestsCoven)
        {
            titlePlayer.SetActive(true);
        }
    }

    private void SetDisplayCovenButtons(TeamData data)
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
    }

    void DisableButtons()
    {
        foreach (var item in allButtons)
        {
            item.SetActive(false);
        }
    }

    void GoBack()
    {
        if (viewPlayer)
        {
            viewPlayer = false;
            TeamPlayerView.Instance.Close();
            SetScreenType(currentScreen);
        }
        else
        {
            if (isCoven)
                SetScreenType(ScreenType.CovenDisplay);
            else
                SetScreenType(ScreenType.CharacterInvite);
        }

        if (TeamCovenView.Instance.IsVisible)
            TeamCovenView.Instance.Close();
    }

    void GoBack(ScreenType screenType)
    {
        SetScreenType(screenType);
    }

    #endregion

    public void Close()
    {
        LTDescr descrAlpha = LeanTween.alphaCanvas(GetComponent<CanvasGroup>(), 0, .28f).setEase(LeanTweenType.easeInOutSine);
        LTDescr descrScale = LeanTween.scale(GetComponent<RectTransform>(), Vector3.zero, .4f).setEase(LeanTweenType.easeInOutSine);
        descrScale.setOnComplete(() =>
        {
            if (TeamCovenView.Instance.IsVisible)
                TeamCovenView.Instance.Close();
            gameObject.SetActive(false);
        });
    }

    void OnEnable()
    {
        GetComponent<CanvasGroup>().alpha = 0;
        GetComponent<RectTransform>().localScale = Vector2.zero;
        LTDescr descrAlpha = LeanTween.alphaCanvas(GetComponent<CanvasGroup>(), 1, .28f).setEase(LeanTweenType.easeInOutSine);
        LTDescr descrScale = LeanTween.scale(GetComponent<RectTransform>(), Vector2.one, .4f).setEase(LeanTweenType.easeInOutSine);
        GoBack();
    }

    void SetHeader(string title, string subtitle)
    {
        covenTitle.text = title;
        subTitle.text = subtitle;
    }

    void SetHeader()
    {
        covenTitle.text = PlayerDataManager.playerData.covenName;
        subTitle.text = PlayerDataManager.playerData.dominion;
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

