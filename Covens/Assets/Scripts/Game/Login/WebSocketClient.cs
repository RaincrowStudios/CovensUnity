using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading;

public class WebSocketClient : MonoBehaviour
{
    public static WebSocketClient Instance { get; set; }

    public static event Action<WSData> OnResponseParsedEvt;

    public static bool websocketReady = false;
    public WebSocket curSocket;
    bool canRun = true;
    bool refresh = false;
    Thread WebSocketProcessing;

    public Queue<string> wssQueue = new Queue<string>();

    private const bool localAPI =
#if LOCAL_API
            true;
#else
            false;
#endif

    private static Dictionary<string, Action<WSData>> m_EventActionDictionary = new Dictionary<string, Action<WSData>>
    {
        { "map_spell_cast",             OnMapSpellcast.HandleEvent },
        { "map_immunity_add",           OnMapImmunityChange.OnAddImmunity },
        { "map_immunity_remove",        OnMapImmunityChange.OnRemoveImmunity },
        { "map_energy_change",          OnMapEnergyChange.HandleEvent },
        { "map_portal_summon",          OnMapPortalSummon.HandleEvent },
        { "map_token_add",              OnMapTokenAdd.HandleEvent },
        { "map_token_move",             OnMapTokenMove.HandleEvent },
        { "map_token_remove",           OnMapTokenRemove.HandleEvent },
        { "map_location_lost",          OnMapLocationLost.HandleEvent },
        { "map_location_gained",        OnMapLocationGained.HandleEvent },
        { "map_condition_add",          OnMapConditionAdd.HandleEvent },
        { "map_condition_remove",       OnMapConditionRemove.HandleEvent },
        { "map_condition_trigger",      OnMapConditionTrigger.HandleEvent },
        { "map_degree_change",          OnMapDegreeChange.HandleEvent },
        { "map_shout",                  OnMapShout.HandleEvent },
        { "map_level_up",               OnMapLevelUp.HandleEvent },

        //{ "character_new_signature",    OnSignatureDiscovered.HandleEvent },
        { "character_death",            OnCharacterDeath.HandleEvent },
        { "character_xp_gain",          OnCharacterXpGain.HandleEvent },
        { "character_location_gained",  OnCharacterLocationGained.HandleEvent },
        { "character_location_lost",    OnCharacterLocationLost.HandleEvent },
        { "character_location_boot",    OnCharacterLocationBoot.HandleEvent },
        { "character_location_reward",  OnCharacterLocationReward.HandleEvent },
        { "character_new_spirit",       OnCharacterNewSpirit.HandleEvent },
        { "character_spell_move",       OnCharacterSpellMove.HandleEvent },
        { "characer_spirit_banished",   OnCharacterSpiritBanished.HandleEvent },
        { "character_daily_progress",   OnCharacterDailyProgress.HandleEvent },
        { "character_spirit_expired",   OnCharacterSpiritExpired.HandleEvent },
        { "character_spirit_sentinel",  OnCharacterSpiritSentinel.HandleEvent },
        { "character_spirit_summoned",  OnCharacterSpiritSummoned.HandleEvent },

        { "coven_was_allied",           TeamManager.OnReceiveCovenAlly },
        { "coven_was_unallied",         TeamManager.OnReceiveCovenUnally },
        { "coven_allied",               TeamManager.OnReceiveCovenMemberAlly },
        { "coven_member_unally",        TeamManager.OnReceiveCovenMemberUnally },
        { "character_coven_kick",       TeamManager.OnReceiveCovenMemberKick },
        { "coven_invite_requested",     TeamManager.OnReceiveCovenMemberRequest },
        { "coven_member_promoted",      TeamManager.OnReceiveCovenMemberPromote },
        { "coven_member_titled",        TeamManager.OnReceiveCovenMemberTitleChange },
        { "coven_member_join",          TeamManager.OnReceiveCovenMemberJoin },
        { "coven_request_invite",       TeamManager.OnReceiveRequestInvite },
        { "coven_member_left",          TeamManager.OnReceiveCovenMemberLeave },
        { "coven_disbanded",            TeamManager.OnReceiveCovenDisbanded },
        { "character_coven_invite",     TeamManager.OnReceivedCovenInvite },
        { "coven_member_invited",       TeamManager.OnReceivedPlayerInvited },
        { "character_coven_reject",     TeamManager.OnReceiveRequestRejected },
    };

    void Awake()
    {
        Instance = this;
        //Application.targetFrameRate = 45;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    public void InitiateWSSCOnnection(bool isRefresh = false)
    {
        refresh = isRefresh;
        if (isRefresh)
        {
            this.StopAllCoroutines();
            AbortThread();
        }
        StartCoroutine(EstablishWSSConnection());
    }


    IEnumerator EstablishWSSConnection()
    {
        print("Connecting to WSS @ " + CovenConstants.wssAddress + LoginAPIManager.wssToken);

        curSocket = new WebSocket(new Uri(CovenConstants.wssAddress + LoginAPIManager.wssToken));

        if (localAPI)
        {
            yield return 0;
            UnityMainThreadDispatcher.Instance().Enqueue(LoginAPIManager.WebSocketConnected);
        }
        else
        {
            yield return StartCoroutine(curSocket.Connect());
            canRun = true;
            StartCoroutine(ReadFromQueue());
            HandleThread();
        }
    }


    public void HandleThread()
    {
        WebSocketProcessing = new Thread(() => ReadCommands(curSocket));
        WebSocketProcessing.Start();
    }

    void ReadCommands(WebSocket w)
    {
        //		print ("Starting Thread");
        while (canRun)
        {
            string reply = w.RecvString();
            if (reply != null)
            {
                if (reply != "200")
                {
                    if (LoginAPIManager.loggedIn && websocketReady)
                    {
                        wssQueue.Enqueue(reply);
                    }
                }
                else
                {
                    //					print ("Refresh Success!");
                    if (!refresh)
                    {
                        UnityMainThreadDispatcher.Instance().Enqueue(LoginAPIManager.WebSocketConnected);
                    }
                }
            }
            if (curSocket.error != null)
            {
                if (!LoginAPIManager.loggedIn)
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(LoginUIManager.Instance.initiateLogin);
                }
                else
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(PlayerManager.Instance.initStart);
                }
                Debug.LogError("Error: " + curSocket.error);
                break;
            }
        }
    }

    void AbortThread()
    {
        //		print ("Closing Thread!");
        if (WebSocketProcessing != null)
        {
            canRun = false;
            curSocket.Close();
            WebSocketProcessing.Abort();
        }
    }

    void OnApplicationQuit()
    {
        AbortThread();
    }

    public void AddMessage(string json)
    {

        wssQueue.Enqueue(json);
    }

    IEnumerator ReadFromQueue()
    {
        while (canRun)
        {
            if (wssQueue.Count > 0)
            {
                string json = wssQueue.Dequeue();
                WSData data = JsonConvert.DeserializeObject<WSData>(json);
                data.json = json;
                ManageData(data);
            }
            yield return 1;
        }
    }

    public void ManageData(WSData data)
    {
        if (OnResponseParsedEvt != null)
            OnResponseParsedEvt(data);
        try
        {
            //if (LoginAPIManager.FTFComplete && !CheckMsgState(data.timeStamp))
            //    return;

            if (m_EventActionDictionary.ContainsKey(data.command))
                m_EventActionDictionary[data.command].Invoke(data);
            else
                Debug.LogError("command not implemented: " + data.command);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message + "\n" + e.StackTrace);
            Debug.LogError(data.json);
        }
    }

    static bool CheckMsgState(double javaTimeStamp)
    {
        System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        dtDateTime = dtDateTime.AddMilliseconds(javaTimeStamp).ToUniversalTime();
        var timeSpan = DateTime.UtcNow.Subtract(dtDateTime);
        return timeSpan.TotalSeconds < PlayerManager.reinitTime;
    }
}



public class WSData
{
    public string json { get; set; }

    public string command { get; set; }

    public string instance { get; set; }

    public Conditions condition { get; set; }
    // map commands
    public string caster { get; set; }

    public bool isCoven { get; set; }

    public int spiritCount { get; set; }

    public string id { get; set; }
    public string status { get; set; }

    public string casterType { get; set; }

    public string targetType { get; set; }

    public string casterInstance { get; set; }

    public string controlledBy { get; set; }

    public string target { get; set; }

    public string targetInstance { get; set; }

    public string spell { get; set; }

    public string baseSpell { get; set; }

    public string baseEffect { get; set; }

    public Result result { get; set; }

    public int newEnergy { get; set; }

    public string newState { get; set; }

    public string shout { get; set; }
    //	public string conditionInstance { get; set;}
    //	public string condition { get; set;}
    public int newLevel { get; set; }

    public int newBaseEnergy { get; set; }

    public int newDegree { get; set; }

    public int oldDegree { get; set; }

    public Token token { get; set; }

    public string immunity { get; set; }
    //char commands
    public string covenName { get; set; }

    public string locationName { get; set; }

    public string displayName { get; set; }

    public int reward { get; set; }

    public string portalInstance { get; set; }

    public int xpGain { get; set; }

    public int newXp { get; set; }

    public int xpToLevelUp { get; set; }

    public string location { get; set; }

    public double banishedOn { get; set; }

    public double createdOn { get; set; }

    public double latitude { get; set; }

    public double longitude { get; set; }

    public double expiresOn { get; set; }

    public string spirit { get; set; }

    public string killer { get; set; }

    public string type { get; set; }

    public string owner { get; set; }

    public string inviteToken { get; set; }

    public string daily { get; set; }

    public int count { get; set; }

    public int silver { get; set; }

    public SpellData signature { get; set; }

    public int energy { get; set; }

    public string state { get; set; }

    public string action { get; set; }

    public int xp { get; set; }

    public int degree { get; set; }

    public int targetEnergy { get; set; }

    public string targetStatus { get; set; }

    public InteractionType iType;

    public string member { get; set; }

    public string coven { get; set; }

    public string newTitle { get; set; }

    public int newRole { get; set; }

    public int level { get; set; }

    public double timeStamp { get; set; }

    public string[] tags { get; set; }
}
