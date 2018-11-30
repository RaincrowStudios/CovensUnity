using UnityEngine;
using System.Collections;
using UnityEngine.UI;
[RequireComponent(typeof(TeamManager))]
[RequireComponent(typeof(TeamUIHelper))]
public class TeamManagerUI : MonoBehaviour
{
    public static TeamManagerUI Instance { get; set; }

    [Header("Footer Buttons")]
    public UnityEngine.GameObject[] allButtons;
    public UnityEngine.GameObject btnInvite;
    public UnityEngine.GameObject btnInvites;
    public UnityEngine.GameObject btnEdit;
    public UnityEngine.GameObject btnRequests;
    public UnityEngine.GameObject btnAllies;
    public UnityEngine.GameObject btnAllied;
    public UnityEngine.GameObject btnLeave;
    public UnityEngine.GameObject btnBack;
    public UnityEngine.GameObject btnCreate;
    public UnityEngine.GameObject btnRequest;
    public UnityEngine.GameObject btnAlly;
    public UnityEngine.GameObject btnDisband;

    [Header("Coven Info")]
    public Text covenTitle;
    public Text dominionTitle;
    public Text covenMotto;
    public Text founder;
    public Text worldRank;
    public Text dominionRank;
    public Text createdOn;
    public Text POPControlled;
    public Text covenType;
    public Text creatorType;
    public Sprite[] schoolSigils;
    public UnityEngine.GameObject btnCloseInfo;
    public Button btnClose;


    public RectTransform loadingUIRect;

    public enum ScreenType
    {
        CharacterInvite, CovenDisplay, AlliedCoven, CovenAllied, EditCoven, RequestCoven, Leaderboard, Locations, CovenInfoSelf, CovenInfoOther, InviteCoven
    }

    public static TeamData teamData = null;

    ScreenType currentScreen;


    bool isCoven
    {
        get { return PlayerDataManager.playerData.covenName == "" ? false : true; }
    }

    private string otherSelectedID;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        btnClose.onClick.AddListener(Close);
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
        DisableButtons();
        if (currentScreen == ScreenType.CharacterInvite)
        {
            TeamManager.GetCharacterInvites(CharaterInviteUI);
        }
        else if (currentScreen == ScreenType.CovenDisplay)
        {
            TeamManager.GetCovenDisplay(DisplayCovenUI);
        }
        else if (currentScreen == ScreenType.AlliedCoven)
        {
            TeamManager.GetAlliedCoven(AlliedCovenUI);
        }
        else if (currentScreen == ScreenType.CovenAllied)
        {
            TeamManager.GetAlliedCoven(CovenAlliedUI);
        }
        else if (currentScreen == ScreenType.EditCoven)
        {
            // add logic
        }
        else if (currentScreen == ScreenType.RequestCoven)
        {
            TeamManager.GetCovenRequests(RequestCovenUI);
        }
        else if (currentScreen == ScreenType.InviteCoven)
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
        else if (currentScreen == ScreenType.CovenInfoOther)
        {
            TeamManager.GetCovenDisplay(CovenInfoUIOther, otherSelectedID);
        }
        else if (currentScreen == ScreenType.CovenInfoSelf)
        {
            CovenInfoUI();
        }
    }

    #region CovenCreate

    public void CreateCovenRequest()
    {
        TeamInputPopup.Instance.ShowPopUp(SendCovenCreateRequest, () => { SetScreenType(ScreenType.CharacterInvite); }, "Choose a name for your coven.");
    }

    void SendCovenCreateRequest(string id)
    {
        Setloading(true);
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
        else if (responseCode == 4103)
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

    #endregion

    #region RequestInvite

    public void RequestInvite()
    {
        TeamInputPopup.Instance.ShowPopUp(SendRequestInvite, () => { SetScreenType(ScreenType.CharacterInvite); }, "Enter the name of player to invite.");
    }

    void SendRequestInvite(string id)
    {
        Setloading(true);
        TeamManager.RequestInvite(RequestInviteResponse, id);
    }

    void RequestInviteResponse(int responseCode)
    {
        Setloading(false);

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
        else if (responseCode == 4300)
        {
            TeamInputPopup.Instance.Error("Coven name is empty");
        }
        else if (responseCode == 4301)
        {
            TeamInputPopup.Instance.Error("Coven name not found");
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
        TeamInputPopup.Instance.ShowPopUp(SendInviteRequest, () => { SetScreenType(ScreenType.CovenDisplay); }, "Enter the name of player to invite.");
    }

    void SendInviteRequest(string id)
    {
        Setloading(true);
        TeamManager.RequestInvite(InviteResponse, id);
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
        TeamManager.CovenReject(JoinResponse, otherSelectedID);
    }

    void JoinResponse(int responseCode)
    {
        Setloading(false);

        if (responseCode == 200)
        {
            TeamConfirmPopUp.Instance.Close();
            PlayerDataManager.playerData.covenName = otherSelectedID;
            TeamConfirmPopUp.Instance.ShowPopUp(() => { SetScreenType(ScreenType.CovenDisplay); }, "You are now a member of " + otherSelectedID);
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
        TeamManager.CovenReject(DeclineInviteResponse, otherSelectedID);
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

    public void SendCancel()
    {
        TeamConfirmPopUp.Instance.ShowPopUp(CancelInviteRequest, () => { SetScreenType(currentScreen); }, "Do you want to cancel this invite?");
    }

    void CancelInviteRequest()
    {
        Setloading(true);
        TeamManager.CovenCancel(CancelInviteResponse, otherSelectedID);
    }

    void CancelInviteResponse(int responseCode)
    {
        Setloading(false);

        if (responseCode == 200 || responseCode == 4807)
        {
            TeamConfirmPopUp.Instance.Close();
            TeamConfirmPopUp.Instance.ShowPopUp(() => { SetScreenType(currentScreen); }, "Successfully cancelled the invite.");
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
        TeamManager.CovenReject(RejectInviteResponse, otherSelectedID);
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
            TeamConfirmPopUp.Instance.Close();
            PlayerDataManager.playerData.covenName = "";
            TeamConfirmPopUp.Instance.ShowPopUp(() => { SetScreenType(ScreenType.CharacterInvite); }, "Successfully left the coven."); 				//check allied coven and coven allied
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
            TeamConfirmPopUp.Instance.ShowPopUp(() => { SetScreenType(ScreenType.CharacterInvite); }, "Coven successfully disbanded."); 				//check allied coven and coven allied
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
            TeamConfirmPopUp.Instance.ShowPopUp(() => { SetScreenType(currentScreen); }, "coven successfully unallied."); 				//check allied coven and coven allied
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
        TeamManager.RequestInvite(CovenAllyResponse, otherSelectedID);
    }

    void CovenAllyResponse(int responseCode)
    {
        Setloading(false);

        if (responseCode == 200 || responseCode == 4808)
        {
            TeamConfirmPopUp.Instance.Close();
            TeamConfirmPopUp.Instance.ShowPopUp(() => { SetScreenType(currentScreen); }, "Coven successfully allied."); 				//check allied coven and coven allied
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
        TeamManager.RequestInvite(CovenUnallyResponse, otherSelectedID);
    }

    void CovenUnallyResponse(int responseCode)
    {
        Setloading(false);

        if (responseCode == 200 || responseCode == 4808)
        {
            TeamConfirmPopUp.Instance.Close();
            TeamConfirmPopUp.Instance.ShowPopUp(() => { SetScreenType(ScreenType.AlliedCoven); }, "Coven successfully unallied."); 				//check allied coven and coven allied
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

    public void SetScreenType(ScreenType screenType)
    {
        currentScreen = screenType;
        Rebuild();
    }

    #region UI Setup Methods

    void CovenInfoUI()
    {
        Setloading(false);
        SetDisplayCovenButtons(teamData);
    }

    void CovenInfoUIOther(TeamData data)
    {
        Setloading(false);
        btnBack.SetActive(true);
    }

    void LocationsUI(TeamLocation[] data)
    {
        Setloading(false);
        btnBack.SetActive(true);
        btnInvite.SetActive(true);
    }

    void InviteCovenUI(TeamInvites[] data)
    {
        Setloading(false);
        btnBack.SetActive(true);
        btnInvite.SetActive(true);
    }

    void RequestCovenUI(TeamInvites[] data)
    {
        Setloading(false);
        btnBack.SetActive(true);
        btnInvite.SetActive(true);
    }

    void AlliedCovenUI(TeamInvites[] data)
    {
        Setloading(false);
        btnBack.SetActive(true);
        btnAlly.SetActive(true);
    }

    void CovenAlliedUI(TeamInvites[] data)
    {
        Setloading(false);
        btnBack.SetActive(true);
        btnAlly.SetActive(true);
    }

    void LeaderboardUI(LeaderboardData[] data)
    {
        Setloading(false);
        btnBack.SetActive(true);
    }

    void CharaterInviteUI(TeamInvites[] data)
    {
        Setloading(false);
        btnCreate.SetActive(true);
        btnRequest.SetActive(true);
    }

    void DisplayCovenUI(TeamData data)
    {
        Setloading(false);
        teamData = data;
        SetDisplayCovenButtons(data);
    }

    private void SetDisplayCovenButtons(TeamData data)
    {
        btnInvite.SetActive(true);
        btnRequests.SetActive(true);
        btnInvites.SetActive(true);
        btnAllied.SetActive(true);
        btnAllies.SetActive(true);
        btnLeave.SetActive(true);
        btnEdit.SetActive(data.createdBy == PlayerDataManager.playerData.displayName);
    }

    void DisableButtons()
    {
        foreach (var item in allButtons)
        {
            item.SetActive(false);
        }
    }

    #endregion

    public void Close()
    {
        LTDescr descrAlpha = LeanTween.alphaCanvas(GetComponent<CanvasGroup>(), 0, .28f).setEase(LeanTweenType.easeInOutSine);
        LTDescr descrScale = LeanTween.scale(GetComponent<RectTransform>(), Vector3.zero, .4f).setEase(LeanTweenType.easeInOutSine);
        descrScale.setOnComplete(() => { gameObject.SetActive(false); });
    }

    void OnEnable()
    {
        GetComponent<CanvasGroup>().alpha = 0;
        GetComponent<RectTransform>().localScale = Vector2.zero;
        LTDescr descrAlpha = LeanTween.alphaCanvas(GetComponent<CanvasGroup>(), 1, .28f).setEase(LeanTweenType.easeInOutSine);
        LTDescr descrScale = LeanTween.scale(GetComponent<RectTransform>(), Vector2.one, .4f).setEase(LeanTweenType.easeInOutSine);
        if (isCoven)
            SetScreenType(ScreenType.CovenDisplay);
        else
            SetScreenType(ScreenType.CharacterInvite);
    }

}

