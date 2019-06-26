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
    public static bool Pause { get; set; }

    public static event Action<WSData> OnResponseParsedEvt;

    public WebSocket curSocket;
    bool canRun = true;
    bool refresh = false;
    Thread WebSocketProcessing;
    private bool websocketReady = false;

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
        { "map_channel_start",          SpellChanneling.OnMapChannelingStart },
        { "map_channel_end",            SpellChanneling.OnMapChannelingFinish },

        //{ "character_new_signature",  OnSignatureDiscovered.HandleEvent },
        { "character_death",            OnCharacterDeath.HandleEvent },
        { "character_silver_add",       OnCharacterGainSilver.HandleEvent },
        { "character_xp_gain",          OnCharacterXpGain.HandleEvent },
        { "character_location_gained",  OnCharacterLocationGained.HandleEvent },
        { "character_location_lost",    OnCharacterLocationLost.HandleEvent },
        { "character_location_boot",    OnCharacterLocationBoot.HandleEvent },
        { "character_location_reward",  OnCharacterLocationReward.HandleEvent },
        { "character_new_spirit",       OnCharacterNewSpirit.HandleEvent },
        { "character_spell_move",       OnCharacterSpellMove.HandleEvent },
        { "character_cooldown_start",   OnCharacterCooldown.OnStart },
        { "character_cooldown_end",     OnCharacterCooldown.OnFinish },

        { "character_spirit_banished",  OnCharacterSpiritBanished.HandleEvent },

        { "character_daily_progress",   OnCharacterDailyProgress.HandleEvent },
        { "character_alignment_change", OnCharacterAlignmentChange.HandleEvent},
        //{ "character_spirit_expire",  OnCharacterSpiritExpired.HandleEvent },
        { "character_spirit_sentinel",  OnCharacterSpiritSentinel.HandleEvent },
        { "character_spirit_summoned",  OnCharacterSpiritSummoned.HandleEvent },
        { "character_creatrix_add",     OnCreatrixGift.HandleEvent },
        { "character_creatrix_shop",    OnCreatrixGift.HandleEvent },

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
        { "coven_created",              TeamManager.OnCovenCreated_Websocket },

        { "location_spirit_summon",     PlaceOfPower.OnLocationSpiritSummon },
    };

    void Awake()
    {
        Instance = this;
        Application.targetFrameRate = 30;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

#if DISABLE_LOG
        Debug.unityLogger.logEnabled = false;
#endif

        LoginAPIManager.OnCharacterInitialized += OnCharacterInitialized;
    }

    private void OnCharacterInitialized()
    {
        LoginAPIManager.OnCharacterInitialized -= OnCharacterInitialized;
        websocketReady = true;
    }

    public void InitiateWSSCOnnection(bool isRefresh = false)
    {
        refresh = isRefresh;
        if (isRefresh)
        {
            this.StopAllCoroutines();
            AbortThread();
            websocketReady = true;
        }
        StartCoroutine(EstablishWSSConnection());
    }


    IEnumerator EstablishWSSConnection()
    {
        Debug.Log("Connecting to WSS");

        curSocket = new WebSocket(new Uri(CovenConstants.wssAddress + LoginAPIManager.wssToken));

        if (localAPI)
        {
            yield return 0;
            UnityMainThreadDispatcher.Instance().Enqueue(LoginAPIManager.WebSocketConnected);
        }
        else
        {
            yield return StartCoroutine(curSocket.Connect());

            if (string.IsNullOrEmpty(curSocket.error))
            {
                Debug.Log(CovenConstants.wssAddress + LoginAPIManager.wssToken);
                Debug.Log("Connected to WSS");
                canRun = true;
                StartCoroutine(ReadFromQueue());
                HandleThread();
            }
            else
            {
                Debug.Log("Failed to connect to WSS:\n" + curSocket.error);
                StartCoroutine(EstablishWSSConnection());
            }
        }
    }


    public void HandleThread()
    {
        WebSocketProcessing = new Thread(() => ReadCommands(curSocket));
        WebSocketProcessing.Start();
    }

    void ReadCommands(WebSocket w)
    {
        while (canRun)
        {
            string reply = w.RecvString();
            if (reply != null)
            {
                if (reply != "200")
                {
                    if (websocketReady && !LoginAPIManager.isInFTF)
                    {
                        wssQueue.Enqueue(reply);
                    }
                }
                else if (!refresh)
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(LoginAPIManager.WebSocketConnected);
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

    public void AbortThread()
    {
        //		Debug.Log ("Closing Thread!");
        if (WebSocketProcessing != null)
        {
            canRun = false;
            curSocket.Close();
            websocketReady = false;
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

    private int m_BatchIndex = 0;
    private int m_BatchSize = 50;

    IEnumerator ReadFromQueue()
    {
        while (canRun)
        {
            while (Pause)
            {
                yield return 0;
            }

            while (wssQueue.Count > 0)
            {
                string json = wssQueue.Dequeue();

                try
                {
                    WSData data = JsonConvert.DeserializeObject<WSData>(json);
                    data.json = json;
                    ManageData(data);
                }
                catch (System.Exception e)
                {
                    string innerException = e.InnerException != null ? e.InnerException.Message : "";
                    string debugString = "Error parsing ws event.\nException: " + e.Message + "\nInnerException: " + innerException + "\n\nStacktrace: " + e.StackTrace + "\n\nData: " + json;
                    Debug.LogError(debugString);
                }

                m_BatchIndex++;

                if (m_BatchIndex >= m_BatchSize)
                {
                    m_BatchSize = 0;
                    yield return 0;
                }
            }

            yield return 0;
        }
    }

    public void ManageData(WSData data)
    {
        if (OnResponseParsedEvt != null)
            OnResponseParsedEvt(data);

        if (m_EventActionDictionary.ContainsKey(data.command))
            m_EventActionDictionary[data.command].Invoke(data);
        else
        {
            if (data.command != "character_daily_reset")
                Debug.LogError("command not implemented: " + data.command + "\n" + data.json);
        }
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

    public string targetState { get; set; }

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

    //

    public int amount { get; set; }

    public int hexCount { get; set; }

    public int bindCountDown { get; set; }

    public int resChange { get; set; }

    public int sealChange { get; set; }

    public int pwrChange { get; set; }

    public int leechEnergy { get; set; }

    public int shadowFeetEnergy { get; set; }

    public int clarityChange { get; set; }

    public CreatrixData creatrix { get; set; }

    public int currentAlignment { get; set; }
    public int minAlignment { get; set; }
    public int maxAlignment { get; set; }

    public int casterEnergy { get; set; }
}

public class CreatrixData
{
    public int[] amount { get; set; }
    public string[] type { get; set; }
    public string id { get; set; }
}
