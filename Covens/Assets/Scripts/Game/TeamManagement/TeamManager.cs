using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

public class TeamManager : MonoBehaviour
{
    public enum CovenRole
    {
        [Oktagon.Localization.LokakiID("Enum_Member")]
        Member = 0,
        [Oktagon.Localization.LokakiID("Enum_Moderator")]
        Moderator,
        [Oktagon.Localization.LokakiID("Enum_Administrator")]
        Administrator,
        [Oktagon.Localization.LokakiID("Enum_Member")]
        None = 100
    }

    public static CovenRole CurrentRole
    {
        get
        {
            if (string.IsNullOrEmpty(PlayerDataManager.playerData.covenName))
                return CovenRole.None;

            if (CovenData == null)
                return CovenRole.None;

            foreach (TeamMember member in CovenData.members)
            {
                if (member.displayName == PlayerDataManager.playerData.displayName)
                    return (CovenRole)member.role;
            }

            return CovenRole.None;
        }
    }
    public static TeamData CovenData = null;

    #region GetRequests

    public static void GetCharacterInvites(Action<TeamInvites[]> OnReceiveData)
    {
        SendRequest<TeamInvites[]>(OnReceiveData, "character/invites");
    }

    public static void GetCovenAllies(Action<TeamAlly[]> OnReceiveData)
    {
        SendRequest<TeamAlly[]>(OnReceiveData, "coven/display-allies");
    }

    public static void GetCovenAllied(Action<TeamAlly[]> OnReceiveData)
    {
        SendRequest<TeamAlly[]>(OnReceiveData, "coven/display-allied-covens");
    }

    public static void GetTopCovens(Action<LeaderboardData[]> OnReceiveData, Action<int> onFailure)
    {
        Leaderboards.Instance.GetLeaderboards(
            onSuccess: (witches, covens) => OnReceiveData?.Invoke(covens),
            onFailure: onFailure
        );
        //SendRequest<LeaderboardRoot>(OnReceiveData, "leaderboards/get");
    }

    public static void GetCovenRequests(Action<TeamInviteRequest[]> OnReceiveData)
    {
        SendRequest<TeamInviteRequest[]>(OnReceiveData, "coven/pending-requests");
    }

    public static void GetCovenInvites(Action<TeamInvites[]> OnReceiveData)
    {
        SendRequest<TeamInvites[]>(OnReceiveData, "coven/pending-invites");
    }

    #endregion

    #region PostRequests

    public static void GetCovenDisplay(Action<TeamData> OnReceiveData, string coven)
    {
        var data = new { covenName = coven };
        APIManager.Instance.PostData("coven/display", JsonConvert.SerializeObject(data), (string s, int r) =>
       {
           if (r == 200)
           {
               TeamData teamData = JsonConvert.DeserializeObject<TeamData>(s);
               if (teamData.covenName == PlayerDataManager.playerData.covenName)
                   CovenData = teamData;
               OnReceiveData(teamData);
           }
           else
           {
               Debug.Log(s);
               OnReceiveData(null);
           }
       });
    }

    public static void GetPlacesOfPower(Action<TeamLocation[]> callback, string covenName)
    {
        GetCovenDisplay(
            OnReceiveData: (coven) =>
            {
                if (coven != null)
                {
                    if (covenName == PlayerDataManager.playerData.covenName)
                        CovenData = coven;
                    callback(coven.controlledLocations);
                }
                else
                {
                    callback(new TeamLocation[0]);
                }
            },
            coven: covenName
        );
    }

    public static void AllyCoven(Action<int> OnReceiveData, string id)
    {
        var data = new { covenName = id };
        SendRequest(OnReceiveData, "coven/ally", JsonConvert.SerializeObject(data));
    }

    public static void UnallyCoven(Action<int> OnReceiveData, string id)
    {
        var data = new { covenName = id };
        SendRequest(OnReceiveData, "coven/unally", JsonConvert.SerializeObject(data));
    }

    public static void CreateCoven(Action<int> OnReceiveData, string id)
    {
        var data = new { covenName = id };
        SendRequestPut(OnReceiveData, "coven/create", JsonConvert.SerializeObject(data));
    }

    public static void RequestInvite(Action<int> OnReceiveData, string id)
    {
        var data = new { covenName = id };
        Debug.Log(JsonConvert.SerializeObject(data));
        SendRequest(OnReceiveData, "coven/request", JsonConvert.SerializeObject(data));
    }

    public static void InviteCoven(Action<int> OnReceiveData, string id)
    {
        var data = new { invitedName = id };
        SendRequest(OnReceiveData, "coven/invite", JsonConvert.SerializeObject(data));
    }

    public static void JoinCoven(Action<int> OnReceiveData, string id)
    {
        var data = new { inviteToken = id };
        SendRequest(OnReceiveData, "coven/join", JsonConvert.SerializeObject(data));
    }

    public static void SetMotto(Action<int> OnReceiveData, string id)
    {
        var data = new { motto = id };
        SendRequest(OnReceiveData, "coven/set-motto", JsonConvert.SerializeObject(data));
    }

    public static void CovenKick(Action<int> OnReceiveData, string id)
    {
        var data = new { kickedName = id };
        SendRequest(OnReceiveData, "coven/kick", JsonConvert.SerializeObject(data));
    }

    public static void CovenDecline(Action<int> OnReceiveData, string id)
    {
        var data = new { inviteToken = id };
        SendRequest(OnReceiveData, "coven/decline", JsonConvert.SerializeObject(data));
    }

    public static void CovenReject(Action<int> OnReceiveData, string id)
    {
        var data = new { request = id };
        SendRequest(OnReceiveData, "coven/reject", JsonConvert.SerializeObject(data));
    }

    public static void CovenCancel(Action<int> OnReceiveData, string id)
    {
        var data = new { inviteToken = id };
        SendRequest(OnReceiveData, "coven/cancel", JsonConvert.SerializeObject(data));
    }

    public static void CovenLeave(Action<int> OnReceiveData)
    {
        var data = new { coven = PlayerDataManager.playerData.covenName };
        APIManager.Instance.GetData("coven/leave", (string s, int r) =>
        {
            if (r == 200 || s == "4802")
            {
                PlayerDataManager.playerData.coven = "";
                PlayerDataManager.playerData.covenName = "";
            }
            OnReceiveData(r);
        });
    }

    public static void CovenDisband(Action<int> OnReceiveData)
    {
        var data = new { coven = PlayerDataManager.playerData.covenName };
        APIManager.Instance.DeleteData("coven/disband", (string s, int r) =>
        {
            if (r == 200 || s == "4804")
            {
                PlayerDataManager.playerData.coven = "";
                PlayerDataManager.playerData.covenName = "";
            }

            OnReceiveData(r);
        });
    }

    public static void CovenPromote(Action<int> OnReceiveData, string playerName, CovenRole playerRole)
    {
        var data = new { promotedName = playerName, promotion = (int)playerRole };
        APIManager.Instance.PostData(
            "coven/promote",
            JsonConvert.SerializeObject(data),
            (response, result) => OnReceiveData(result)
        );
    }

    public static void CovenSetTitle(Action<int> OnReceiveData, string playerName, string newTitle)
    {
        var data = new { titledName = playerName, title = newTitle };
        APIManager.Instance.PostData(
            "coven/title",
            JsonConvert.SerializeObject(data),
            (response, result) => OnReceiveData(result)
        );
    }

    public static void ViewCharacter(string id, Action<WitchMarkerDetail, int> callback)
    {
        var data = new { target = id };
        APIManager.Instance.PostData(
            endpoint: "chat/select",
            data: JsonConvert.SerializeObject(data),
            CallBack: (response, result) =>
            {
                if (result == 200)
                {
                    callback?.Invoke(JsonConvert.DeserializeObject<WitchMarkerDetail>(response), result);
                }
                else
                {
                    callback?.Invoke(null, result);
                }
            });
    }

    #endregion

    static void SendRequest<T>(Action<T> OnReceiveData, string URL)
    {
        APIManager.Instance.GetData(URL, (string s, int r) =>
        {
            Debug.Log(s);
            if (r == 200)
                OnReceiveData(JsonConvert.DeserializeObject<T>(s));
            // else
            //     Debug.Log(s);
        });
    }

    static void SendRequest(Action<int> OnReceiveData, string URL, string jsonData)
    {
        APIManager.Instance.PostData(URL, jsonData, (string s, int r) =>
        {
            int resCode;
            if (r == 200)
                resCode = 200;
            else
            {
                if (int.TryParse(s, out resCode))
                {

                }
                else
                {
                    resCode = 4300;
                }
            }
            Debug.Log(resCode);
            OnReceiveData(resCode);
        });
    }

    static void SendRequestPut(Action<int> OnReceiveData, string URL, string jsonData)
    {
        APIManager.Instance.PutData(URL, jsonData, (string s, int r) =>
        {
            int responseCode = r;

            if (r == 400)
                int.TryParse(s, out responseCode);
            
            OnReceiveData(responseCode);
        });
    }


    #region WebSocket events

    public static void OnReceiveCovenMemberAlly(WSData response)
    {
        /* my coven allied to another coven
        {
            "command":"coven_allied",
            "displayName":"name of the player that declared the alliance",
            "coven":"ally coven.. id?",
            "covenName":"ally covenName"
        }*/
        string covenName = response.covenName;
        string playerName = response.displayName;

        if (TeamManagerUI.isOpen && TeamManagerUI.Instance.currentScreen == TeamManagerUI.ScreenType.CovenAllies)
        {
            TeamManagerUI.Instance.SetScreenType(TeamManagerUI.ScreenType.CovenAllies);
        }
        if (response.covenName == PlayerDataManager.playerData.covenName)
			LogNotification(LocalizeLookUp.GetText ("log_chat_ally").Replace("{{Player}}", playerName).Replace("{{Coven Name}}", covenName));//$"{playerName} declared an alliance to {covenName}.");
    }

    public static void OnReceiveCovenMemberUnally(WSData response)
    {
        /* my coven canceled an alliance
        {
            "command":"coven_unallied",
            "displayName":"name of the player that canceled the alliance",
            "covenName":"ally covenName"
        }*/
        string covenName = response.covenName;
        string playerName = response.displayName;

        if (TeamManagerUI.isOpen && TeamManagerUI.Instance.currentScreen == TeamManagerUI.ScreenType.CovenAllies)
        {
            TeamManagerUI.Instance.SetScreenType(TeamManagerUI.ScreenType.CovenAllies);
        }
        if (response.covenName == PlayerDataManager.playerData.covenName)
			LogNotification(LocalizeLookUp.GetText ("log_chat_ally_revoke").Replace("{{Player}}", playerName).Replace("{{Coven Name}}", covenName));//$"{playerName} revoked the alliance with {covenName}.");
    }

    public static void OnReceiveCovenAlly(WSData response)
    {
        /* a coven declared alliance to my coven
        {
            "command":"coven_was_allied",
            "covenName":"coven name"
        }*/
        string covenName = response.covenName;

        if (TeamManagerUI.isOpen && TeamManagerUI.Instance.currentScreen == TeamManagerUI.ScreenType.CovenAllied)
        {
            TeamManagerUI.Instance.SetScreenType(TeamManagerUI.ScreenType.CovenAllied);
        }
        if (response.covenName == PlayerDataManager.playerData.covenName)
			LogNotification(LocalizeLookUp.GetText ("log_chat_ally_declare").Replace("{{Coven Name}}", covenName));//$"{covenName} declared an alliance to your coven.");
    }

    public static void OnReceiveCovenUnally(WSData response)
    {
        /* a coven revoked the alliance with my coven
        {
            "command":"coven_was_unallied",
            "covenName":"coven name"
        }*/
        string covenname = response.covenName;

        if (TeamManagerUI.isOpen && TeamManagerUI.Instance.currentScreen == TeamManagerUI.ScreenType.CovenAllied)
        {
            TeamManagerUI.Instance.SetScreenType(TeamManagerUI.ScreenType.CovenAllied);
        }
        if (response.covenName == PlayerDataManager.playerData.covenName)
			LogNotification(LocalizeLookUp.GetText ("log_chat_coven_unally").Replace("{{Coven Name}}", covenname));//$"{covenname} called off the alliance with your coven.");
    }

    public static void OnReceiveCovenMemberKick(WSData response)
    {
        /* triggered then the local player is kicked
        {
            "command":"character_coven_kick",
            "covenName":"covenName"
        }*/
        string covenName = response.covenName;

        PlayerDataManager.playerData.coven = "";
        PlayerDataManager.playerData.covenName = "";

        if (TeamManagerUI.isOpen)
        {
			TeamManagerUI.ConfirmPopup.ShowPopUp(() => TeamManagerUI.Instance.SetScreenType(TeamManagerUI.ScreenType.CharacterInvite), LocalizeLookUp.GetText ("coven_expell").Replace("{{CovenData Name}}", covenName));//$"You have been expelled from {covenName}.");
        }

        //LogChatMessage($"You have been expelled from {covenName}");
    }

    public static void OnReceiveCovenMemberRequest(WSData response)
    {
        /* triggered when a player requests to join the coven
        {
            "command":"coven_invite_requested",
            "displayName":"requester name",
            "level":1,
            "degree":0
        }*/
        string playerName = response.displayName;
        int playerLevel = response.level;
        int playerDegree = response.degree;

        if (TeamManagerUI.isOpen && TeamManagerUI.Instance.currentScreen == TeamManagerUI.ScreenType.RequestsCoven)
        {
            TeamManagerUI.Instance.SetScreenType(TeamManagerUI.ScreenType.RequestsCoven);
        }
        if (response.covenName == PlayerDataManager.playerData.covenName)
			LogNotification(LocalizeLookUp.GetText ("log_chat_member_request").Replace("{{Player Name}}", playerName));//$"{playerName} requested to join your coven.");
    }

    public static void OnReceiveCovenMemberPromote(WSData response)
    {
        /* triggered when a coven member is promoted
        {
            "command" : "coven_member_promoted",
            "member" : "promoter's displayName",
            "displayName" : "promoted's displayName",
            "newRole" : 1
        }*/

        string promoter = response.member;
        string promotedPlayer = response.displayName;
        int newRole = response.newRole;

        //update the role in the cached memberlist
        if (CovenData != null && CovenData.members != null)
        {
            for (int i = 0; i < CovenData.members.Count; i++)
            {
                if (CovenData.members[i].displayName == promotedPlayer)
                {
                    CovenData.members[i].role = newRole;
                    break;
                }
            }
        }

        //updated the view for the promoted player
        if (TeamManagerUI.isOpen)
        {
            if (promotedPlayer == PlayerDataManager.playerData.displayName)
            {
                string roleName = ((CovenRole)newRole).ToString();
				string message = LocalizeLookUp.GetText ("coven_player_promoted").Replace("{{role}}", roleName);//"You have been promoted to " + roleName;

                if (TeamManagerUI.Instance.currentScreen == TeamManagerUI.ScreenType.EditCoven || TeamManagerUI.Instance.currentScreen == TeamManagerUI.ScreenType.Members)
                {
                    foreach (var item in TeamUIHelper.Instance.uiItems.Values)
                        item.EnableEdit(true);
                }
            }

            if (TeamManagerUI.Instance.currentScreen == TeamManagerUI.ScreenType.CovenDisplay || TeamManagerUI.Instance.currentScreen == TeamManagerUI.ScreenType.EditCoven || TeamManagerUI.Instance.currentScreen == TeamManagerUI.ScreenType.Members)
            {
                if (TeamUIHelper.Instance.uiItems.ContainsKey(promotedPlayer))
                {
                    TeamItemData item = TeamUIHelper.Instance.uiItems[promotedPlayer];
                    
                    //setup the icons
                    item.adminIcon.SetActive(newRole == 2);
                    item.modIcon.SetActive(newRole == 1);

                    item.EnableEdit(true);
                }
            }
        }

        if (response.covenName == PlayerDataManager.playerData.covenName)
			LogNotification(LocalizeLookUp.GetText ("log_chat_member_promote").Replace("{{Promoter}}", promoter).Replace("{{Promoted Player}}", promotedPlayer));//$"{promoter} promoted {promotedPlayer}");
    }

    public static void OnReceiveCovenMemberTitleChange(WSData response)
    {
        /* triggered when a coven member changes another member's title
        {
            "command":"coven_member_titled",
            "member":"displayName of who made the change",
            "displayName":"diplayName of the titled played",
            "newTitle":"new title"
        }*/

        string entitler = response.member;
        string titledPlayer = response.displayName;
        string title = response.newTitle;

        //update the title in the cached memberlist
        if (CovenData != null && CovenData.members != null)
        {
            for (int i = 0; i < CovenData.members.Count; i++)
            {
                if (CovenData.members[i].displayName == titledPlayer)
                {
                    CovenData.members[i].title = title;
                    break;
                }
            }
        }

        //updated the view for the promoted player
        if (TeamManagerUI.isOpen)
        {
            if (TeamUIHelper.Instance.uiItems.ContainsKey(titledPlayer))
            {
                TeamItemData item = TeamUIHelper.Instance.uiItems[titledPlayer];
                if (item.title != null)
                    item.title.text = title;
                if (item.title)
                    item.titleField.text = title;
            }
        }
        if (response.covenName == PlayerDataManager.playerData.covenName)
        {
            if (titledPlayer == PlayerDataManager.playerData.displayName)
				LogNotification(LocalizeLookUp.GetText ("log_chat_titled").Replace("{{Titled Player}}", titledPlayer).Replace("{{Title}}", title));//$"{titledPlayer} is now \"{title}\"");
            else
				LogNotification(LocalizeLookUp.GetText ("log_chat_entitled").Replace("{{Titled Player}}", titledPlayer).Replace("{{Title}}", title).Replace("{{Entitler}}", entitler));//$"{titledPlayer} was entitled \"{title}\" by {entitler}");
        }
    }

    public static void OnReceiveCovenMemberJoin(WSData response)
    {
        /* triggered when a player joins the guild
        {
            command: "coven_member_join",
            displayName: "new player name",
            level: 1,
            degree: 0
        }*/

        string playerName = response.displayName;
        int playerLevel = response.level;
        int degree = response.degree;

        TeamMember newMember = new TeamMember();
        newMember.displayName = playerName;
        newMember.level = playerLevel;
        newMember.degree = degree;
		newMember.title = LocalizeLookUp.GetText ("coven_member_new_title");//.Replace("{{Coven Name}}", covenname));"Devotee";
        newMember.role = 0;
        newMember.joinedOn = newMember.lastActiveOn = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;

        //updated the cached memberlist
        if (CovenData != null && CovenData.members != null)
            CovenData.members.Add(newMember);

        //add the new member to the memberlist view
        if (TeamManagerUI.isOpen)
        {
            if (TeamManagerUI.Instance.currentScreen == TeamManagerUI.ScreenType.Members)
            {
                var tData = Utilities.InstantiateObject(TeamUIHelper.Instance.memberPrefab, TeamUIHelper.Instance.container).GetComponent<TeamItemData>();
                tData.Setup(newMember);
                tData.EnableEdit(true);
                tData.transform.GetChild(0).gameObject.SetActive(TeamUIHelper.Instance.uiItems.Count % 2 == 0);
                TeamUIHelper.Instance.uiItems.Add(newMember.displayName, tData);
            }
            else if (TeamManagerUI.Instance.currentScreen == TeamManagerUI.ScreenType.InvitesCoven)
            {
                if (TeamUIHelper.Instance.uiItems.ContainsKey(playerName))
                {
                    Destroy(TeamUIHelper.Instance.uiItems[playerName].gameObject);
                    TeamUIHelper.Instance.uiItems.Remove(playerName);
                }
            }
        }
        if (response.covenName == PlayerDataManager.playerData.covenName)
			LogNotification(LocalizeLookUp.GetText ("log_chat_coven_join").Replace("{{Player Name}}", playerName));//$"{playerName} joined the coven.");
    }

    /// <summary>
    /// NOT IMPLEMENTED - SERVER
    /// </summary>
    public static void OnReceiveRequestInvite(WSData response)
    {

    }

    public static void OnReceiveCovenMemberLeave(WSData response)
    {
        /* triggered when a player leaves the coven
         {
            "command":"coven_member_left",
            "displayName":"player name"
         }*/

        string playerName = response.displayName;

        //update the cached member list
        if (CovenData != null && CovenData.members != null)
        {
            for (int i = 0; i < CovenData.members.Count; i++)
            {
                if (CovenData.members[i].displayName == playerName)
                {
                    CovenData.members.RemoveAt(i);
                    break;
                }
            }
        }

        //update the view
        if (TeamManagerUI.isOpen)
        {
            //if (TeamUIHelper.Instance.uiItems.ContainsKey(playerName))
            //{
            //    TeamItemData item = TeamUIHelper.Instance.uiItems[playerName];
            //    Destroy(item.gameObject);
            //    TeamUIHelper.Instance.uiItems.Remove(playerName);
            //}
            TeamManagerUI.Instance.SetScreenType(TeamManagerUI.Instance.currentScreen);
        }

        if (response.covenName == PlayerDataManager.playerData.covenName)
			LogNotification(LocalizeLookUp.GetText ("log_chat_coven_abandon").Replace("{{Player Name}}", playerName));//$"{playerName} abandoned the coven.");
    }

    public static void OnReceivedCovenInvite(WSData response)
    {
        /* triggered when the local player receives a coven invite
         {
            "command":"character_coven_invite",
            "displayName":"inviter name",
            "covenName":"coven name",
            "inviteToken":"invitetoken"
         }*/

        string inviterName = response.displayName;
        string covenName = response.covenName;
        string inviteToken = response.inviteToken;

        if (TeamManagerUI.isOpen)
        {
            //destroy the "Nothing/No invites" item 
            if (TeamUIHelper.Instance.uiItems.Count == 0)
                Destroy(TeamUIHelper.Instance.container.GetChild(0).gameObject);

            //instantiate and setup the new invite
            TeamInvites invite = new TeamInvites()
            {
                covenName = covenName,
                inviteToken = inviteToken,
                invitedOn = (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds
            };

            var tData = Utilities.InstantiateObject(TeamUIHelper.Instance.requestPrefab, TeamUIHelper.Instance.container).GetComponent<TeamItemData>();
            tData.Setup(invite);
            tData.transform.GetChild(0).gameObject.SetActive(TeamUIHelper.Instance.uiItems.Count % 2 == 0);
        }

		LogNotification(LocalizeLookUp.GetText ("log_chat_coven_invited_by").Replace("{{Inviter Name}}", inviterName).Replace("{{Coven Name}}", covenName));//"You have been invited by " + inviterName + " to join the coven " + covenName);
    }

    public static void OnReceivedPlayerInvited(WSData response)
    {
        /* triggered a player is invited to join the coven
         {
            "command":"coven_member_invited",
            "displayName":"inviter name",
            "member":"invited name"
         }*/

        string inviterName = response.displayName;
        string invitedName = response.member;

        //cant add, there is no invitetoken
        //add to the view
        //if (TeamManagerUI.isOpen && TeamManagerUI.Instance.currentScreen == TeamManagerUI.ScreenType.InvitesCoven)
        //{
        //}
        if (response.covenName == PlayerDataManager.playerData.covenName)
			LogNotification(LocalizeLookUp.GetText ("log_chat_coven_invite").Replace("{{Inviter Name}}", inviterName).Replace("{{Invited Name}}", invitedName));//$"{inviterName} invited {invitedName} to join the coven.");
    }

    public static void OnReceiveRequestRejected(WSData response)
    {
        /* triggered when the local player receives a coven invite declined
         {
            "command":"character_coven_reject",
            "covenName":"coven name"
         }*/

        string covenName = response.covenName;
    }

    public static void OnReceiveCovenDisbanded(WSData response)
    {
        //remove coven from playerdata
        PlayerDataManager.playerData.coven = "";
        PlayerDataManager.playerData.covenName = "";

        /* triggered when the local player receives a coven invite declined
         {
            "command":"coven_disbanded",
            "displayName":"player name"
         }*/
        string playerName = response.displayName;

        //show disbanded popup and go to the invites screen
        if (TeamManagerUI.isOpen)
        {
            if (playerName != PlayerDataManager.playerData.displayName)
				TeamManagerUI.ConfirmPopup.ShowPopUp(() => TeamManagerUI.Instance.SetScreenType(TeamManagerUI.ScreenType.CharacterInvite), LocalizeLookUp.GetText ("log_chat_coven_disband").Replace("{{Player Name}}", playerName));//$"{playerName} disbanded the coven.");
        }
    }

    private static void LogNotification(string message)
    {
        PlayerNotificationManager.Instance.ShowNotification(message, PlayerNotificationManager.Instance.covenIcon);
    }

    #endregion
}

public enum TeamPrefabType
{
    Member, InviteRequest, Ally, UnAlly
}

public class TeamData
{
    public double createdOn { get; set; }
    public string motto { get; set; }
    public string coven { get; set; }
    public double disbandedOn { get; set; }
    public string covenName { get; set; }
    public string dominion { get; set; }
    public int rank { get; set; }
    public int score { get; set; }
    public int dominionRank { get; set; }
    public string createdBy { get; set; }
    public int totalSilver { get; set; }
    public int totalGold { get; set; }
    public int totalEnergy { get; set; }
    public TeamLocation[] controlledLocations { get; set; }
    public List<TeamMember> members { get; set; }

    [JsonIgnore]
    public int Degree
    {
        get
        {
            int result = 0;
            for (int i = 0; i < members.Count; i++)
            {
                result += members[i].degree;
            }
            return result;
        }
    }

    [JsonIgnore]
    public int CreatorDegree
    {
        get
        {
            for (int i = 0; i < members.Count; i++)
            {
                if (members[i].displayName == createdBy)
                {
                    return members[i].degree;
                }
            }
            return 0;
        }
    }
}

public class TeamMember
{
    public string state { get; set; }
    public string displayName { get; set; }
    public string title { get; set; }
    public int level { get; set; }
    public int degree { get; set; }
    public int role { get; set; }
    public double joinedOn { get; set; }
    public double lastActiveOn { get; set; }
}

public class TeamLocation
{
    public string instance { get; set; }
    public string displayName { get; set; }
    public double rewardOn { get; set; }
    public double controlledOn { get; set; }
    public double latitude { get; set; }
    public double longitude { get; set; }
}

public class TeamInviteRequest
{
    public string displayName { get; set; }
    public int level { get; set; }
    public int degree { get; set; }
    public string request { get; set; }
    public long requestedOn { get; set; }
}

public class TeamInvites
{
    public string displayName { get; set; }
    public string covenName { get; set; }
    public long invitedOn { get; set; }
    public string inviteToken { get; set; }
}

public class TeamAlly
{
    public string coven { get; set; }
    public long timestamp { get; set; }
    public string covenName { get; set; }
    public int rank { get; set; }
}