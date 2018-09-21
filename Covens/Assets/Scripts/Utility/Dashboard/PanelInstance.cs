using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;


public class PanelInstance : MonoBehaviour
{
    [Header("Login")]
    public InputField m_Login;
    public InputField m_LoginPassword;
    public InputField m_LoginToken;
    public InputField m_LoginWSToken;



    [Header("Coven")]
    public Text m_CovenTitle;
    public InputField m_CovenCreate;
    public InputField m_CovenInvite;
    public InputField m_CovenAlly;
    public InputField m_CovenUnally;
    public InputField m_CovenJoinToken;
    public InputField m_CovenRequest;

    public InputField m_CovenKick;
    public InputField m_CovenAcceptMember;
    public InputField m_CovenRejectMember;
    public InputField m_Log;

    public InputField m_CovenPromote;
    public InputField m_covenPromoteTo;

    public class PlayerData
    {
        public string WSToken;
        public string LoginToken;
        public string Login;
        public string Password;
        public CovenController m_pController;
        public MarkerDataDetail m_pPlayerData;
        public bool m_bLoggedin = false;
        public Dictionary<string, string> m_Responses = new Dictionary<string, string>();
    }


    private PlayerData m_Player;
    public int Index;
    public DashboardToolbox m_Toolbox;


    public void OnClickLogin()
    {
        m_Log.text = "";
        m_Player = new PlayerData();
        m_Player.Login = m_Login.text;
        m_Player.Password = m_LoginPassword.text;

        Action<string, int> Success = (string result, int status) =>
        {
            Log("LoginProcess status: " + status);
            if (status == 200)
            {
                m_Player.m_Responses.Add("LoginResponse", result);
                PlayerLoginCallback Json = JsonConvert.DeserializeObject<PlayerLoginCallback>(result);
                m_Player.WSToken = Json.wsToken;
                m_Player.LoginToken = Json.token;
                m_Player.m_bLoggedin = true;
                m_Toolbox.SetText(Index, m_Player.Login);
                m_LoginToken.text = Json.token;
                m_LoginWSToken.text = Json.wsToken;
                m_Player.m_pController = new CovenController(Json.character.coven);
                m_Player.m_pPlayerData = Json.character;
                m_CovenTitle.text = "Coven[" + Json.character.coven + "]";
                if (!string.IsNullOrEmpty(Json.character.coven))
                {
                    OnClickDisplay();
                }
            }
        };
//        LoginAPIManager.Login(m_Player.Login, m_Player.Password, Success);
    }
    void UpdateTokens()
    {
        LoginAPIManager.loginToken = m_Player.LoginToken;
        LoginAPIManager.wssToken = m_Player.WSToken;
        CovenController.Player = m_Player.m_pController;
        PlayerDataManager.playerData = m_Player.m_pPlayerData;
    }
    public void OnClickCreate()
    {
        m_Log.text = "";
        m_Player = new PlayerData();
        m_Player.Login = m_Login.text;
        m_Player.Password = m_LoginPassword.text;

//        LoginAPIManager.CreateAccount(m_Player.Login, m_Player.Password, m_Player.Login + "@DashHugo.com",
//            (string result, int status) =>
//            {
//                if (status == 200)
//                {
//                    var Json = JsonConvert.DeserializeObject<PlayerLoginCallback>(result);
//                    m_Player.WSToken = Json.token;
//                    m_Player.LoginToken = Json.token;
//                    OnClickCreateChar();
//                }
//            }
//            );
    }
    private void OnClickCreateChar()
    {
        UpdateTokens();
//        LoginAPIManager.CreateCharacter(m_Player.Login, UnityEngine.Random.Range(0,10) > 5,
//            (string result, int status) =>
//            {
//                if (status == 200)
//                {
//                    var Json = JsonConvert.DeserializeObject<PlayerLoginCallback>(result);
//                    m_Player.LoginToken = Json.token;
//                }
//                OnClickLogin();
//            }
//            );
    }
    public void OnClickCovenCreate()
    {
        UpdateTokens();
        m_Player.m_pController.CreateCoven(m_CovenCreate.text, Success("Coven Created"), Fail("OnClickCovenCreate"));
    }
    public void OnClickCovenInvite()
    {
        UpdateTokens();
        m_Player.m_pController.InvitePlayer(m_CovenInvite.text, Success("Coven Player Invited"), Fail("OnClickCovenInvite"));
    }
    public void OnClickCovenAlly()
    {
        UpdateTokens();
        m_Player.m_pController.Ally(m_CovenAlly.text, Success("Coven Allied"), Fail("OnClickCovenAlly"));
    }
    public void OnClickCovenUnally()
    {
        UpdateTokens();
        m_Player.m_pController.Unally(m_CovenUnally.text, Success("Coven Unallied"), Fail("OnClickCovenUnally"));
    }
    public void OnClickCovenKick()
    {
        UpdateTokens();
        m_Player.m_pController.Kick(m_CovenKick.text, Success("Coven Member kicked"), Fail("OnClickCovenKick"));
    }
    public void OnClickCovenAcceptMember()
    {
        UpdateTokens();
        m_Player.m_pController.AcceptMember(m_CovenAcceptMember.text, Success("Coven Member accepted"), Fail("OnClickCovenAcceptMember"));
    }
    public void OnClickCovenRejectMember()
    {
        UpdateTokens();
        m_Player.m_pController.RejectMember(m_CovenUnally.text, Success("Coven Member rejected"), Fail("OnClickCovenRejectMember"));
    }
    public void OnClickCovenRequest()
    {
        UpdateTokens();
        m_Player.m_pController.RequestJoinCoven(m_CovenRequest.text, Success("Coven requested"), Fail("OnClickCovenRequest"));
    }
    public void OnClickCovenPromote()
    {
        UpdateTokens();
        CovenController.CovenRole eRole = (CovenController.CovenRole)int.Parse(m_covenPromoteTo.text);
        CovenController.CovenRole eNewRole = CovenController.GetNextRole(eRole);
        m_Player.m_pController.PromoteMember(m_CovenPromote.text, eNewRole, Success("user promoted"), Fail("OnClickCovenRequest"));
    }
    public void OnClickCharacterInvites()
    {
        UpdateTokens();
        Action<CovenOverview[], string> Complete = (CovenOverview[] vCovens, string failure) =>
        {
            string s = "- Coven Invites: ( " + (vCovens != null ? vCovens.Length.ToString() : "0") + " )";
            foreach (var pCoven in vCovens)
                s += "\n  - covenName[" + pCoven.covenName + "] inviteToken[" + pCoven.inviteToken + "] members[" + pCoven.members+ "]";
            Log(s);
        };
        m_Player.m_pController.CharacterInvites(Complete);
    }
    public void OnClickDisplay()
    {
        UpdateTokens();
        Action<CovenData> Success = (CovenData pData) =>
        {
            m_CovenTitle.text = "Coven[" + pData.covenName + "] own[" + pData.createdBy + "] ally[" + pData.allies.Length + "] allied[" + pData.alliedCovens.Length + "]";
            string sMembers = "- Members: ( " + (pData.members != null ? pData.members.Length.ToString() : "0") + " )";
            foreach (var p in pData.members)
                sMembers += "\n  - displayName[" + p.displayName + "] Role[" + CovenController.ParseRole(p.role) + "] title[" + p.title + "] status[" + p.state + "]";
            string sAllies = "- Allies ( " + (pData.allies != null ? pData.allies.Length.ToString() : "0") + " ):";
            foreach (var p in pData.allies)
                sAllies += "\n  - covenName[" + p.covenName+ "] members[" + p.members + "] rank[" + p.rank + "]";
            string sAllieds = "- Allieds ( " + (pData.alliedCovens != null ? pData.alliedCovens.Length.ToString() : "0") + " ):";
            foreach (var p in pData.alliedCovens)
                sAllieds += "\n  - covenName[" + p.covenName + "] members[" + p.members + "] rank[" + p.rank + "]";
            Log(sMembers);
            Log(sAllies);
            Log(sAllieds);
            OnClickRequestInvites();
        };
        m_Player.m_pController.RequestDisplayCoven(Success, null);
    }
    public void OnClickCovenJoin()
    {
        Action Success = () =>
        {
            Log("Join success");
        };

        UpdateTokens();
        m_Player.m_pController.JoinCoven(m_CovenJoinToken.text, Success, Fail("OnClickCovenJoin"));
    }
    public void OnClickRequestInvites()
    {
        UpdateTokens();
        Action<MemberInvite> Success = (MemberInvite pMembers) =>
        {
            string s = "- Invites: ( " + (pMembers.invites != null ? pMembers.invites.Length.ToString() : "0") + " )";
            foreach(var pMember in pMembers.invites)
                s += "\n  - displayName[" + pMember.displayName + "]: " + pMember.inviteToken;
            Log(s);
            s = "- Requests: ( "+(pMembers.requests != null ? pMembers.requests.Length.ToString() : "0") +" )";
            foreach (var pMember in pMembers.requests)
                s += "\n  - displayName[" + pMember.displayName + "] inviteToken[" + pMember.inviteToken + "]";

            Log(s);
        };
        m_Player.m_pController.CovenViewPending(Success, Fail("OnClickRequestInvites"));
    }
    public Action<string> Success(string sName)
    {
        return (string s) =>{ Log("Success: " + sName); };
    }
    public Action<string> Fail(string sName)
    {
        return (string s) => { Log("Fail: " + sName); };
    }


    void Log(string sLog)
    {
        Debug.Log("[PanelInstance] " + sLog);
        m_Log.text += "\n" + sLog;
    }
}