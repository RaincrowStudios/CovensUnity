using UnityEngine;
using System.Collections;
using Newtonsoft.Json;
using System.Collections.Generic;
using BestHTTP.SocketIO;
using BestHTTP.SocketIO.JsonEncoders;
using Raincrow.GameEventResponses;

public class SocketClient : MonoBehaviour
{
    public static SocketClient Instance { get; set; }
    public static bool SocketPaused { get; set; }

    public static event System.Action<CommandResponse> OnResponseParsedEvent;

    private SocketManager _socketManager;
    private bool _isRefreshingConnection = false;

    public Queue<CommandResponse> responsesQueue = new Queue<CommandResponse>();    

    private static Dictionary<string, IGameEventHandler> m_EventActionDictionary = new Dictionary<string, IGameEventHandler>
    {
        { SpellCastHandler.EventName,           new SpellCastHandler()      },
        { MoveTokenHandler.EventName,           new MoveTokenHandler()      },
        { AddImmunityHandler.EventName,         new AddImmunityHandler()    },
        { RemoveImmunityHandler.EventName,      new RemoveImmunityHandler() },
        { AddWitchHandler.EventName,            new AddWitchHandler()       },
        { AddSpiritHandler.EventName,           new AddSpiritHandler()      },
        { AddCollectableHandler.EventName,      new AddCollectableHandler() },
        { RemoveTokenHandler.EventName,         new RemoveTokenHandler()    },
        { SummonSpiritHandler.EventName,        new SummonSpiritHandler()   },
        { LevelUpHandler.EventName,             new LevelUpHandler()        },
        { ChangeDegreeHandler.EventName,        new ChangeDegreeHandler()   },

        //{ "map_energy_change",          OnMapEnergyChange.HandleEvent },
        //{ "map_token_move",             OnMapTokenMove.HandleEvent },
        //{ "map_token_remove",           OnMapTokenRemove.HandleEvent },
        //{ "map_location_lost",          OnMapLocationLost.HandleEvent },
        //{ "map_location_gained",        OnMapLocationGained.HandleEvent },
        //{ "map_condition_add",          OnMapConditionAdd.HandleEvent },
        //{ "map_condition_remove",       OnMapConditionRemove.HandleEvent },
        //{ "map_condition_trigger",      OnMapConditionTrigger.HandleEvent },
        //{ "map_degree_change",          OnMapDegreeChange.HandleEvent },
        //{ "map_shout",                  OnMapShout.HandleEvent },
        //{ "map_level_up",               OnMapLevelUp.HandleEvent },
        //{ "map_channel_start",          SpellChanneling.OnMapChannelingStart },
        //{ "map_channel_end",            SpellChanneling.OnMapChannelingFinish },

        ////{ "character_new_signature",  OnSignatureDiscovered.HandleEvent },
        //{ "character_death",            OnCharacterDeath.HandleEvent },
        //{ "character_silver_add",       OnCharacterGainSilver.HandleEvent },
        //{ "character_xp_gain",          OnCharacterXpGain.HandleEvent },
        //{ "character_location_gained",  OnCharacterLocationGained.HandleEvent },
        //{ "character_location_lost",    OnCharacterLocationLost.HandleEvent },
        //{ "character_location_boot",    OnCharacterLocationBoot.HandleEvent },
        //{ "character_location_reward",  OnCharacterLocationReward.HandleEvent },
        //{ "character_new_spirit",       OnCharacterNewSpirit.HandleEvent },
        //{ "character_spell_move",       OnCharacterSpellMove.HandleEvent },
        //{ "character_cooldown_start",   CooldownManager.OnCooldownStart },
        //{ "character_cooldown_end",     CooldownManager.OnCooldownFinish },

        //{ "character_spirit_banished",  OnCharacterSpiritBanished.HandleEvent },

        //{ "character_daily_progress",   OnCharacterDailyProgress.HandleEvent },
        //{ "character_alignment_change", OnCharacterAlignmentChange.HandleEvent},
        ////{ "character_spirit_expire",  OnCharacterSpiritExpired.HandleEvent },
        //{ "character_spirit_sentinel",  OnCharacterSpiritSentinel.HandleEvent },
        //{ "character_spirit_summoned",  OnCharacterSpiritSummoned.HandleEvent },
        //{ "character_creatrix_add",     OnCreatrixGift.HandleEvent },
        //{ "character_creatrix_shop",    OnCreatrixGift.HandleEvent },

        //{ "coven_was_allied",           TeamManager.OnReceiveCovenAlly },
        //{ "coven_was_unallied",         TeamManager.OnReceiveCovenUnally },
        //{ "coven_allied",               TeamManager.OnReceiveCovenMemberAlly },
        //{ "coven_member_unally",        TeamManager.OnReceiveCovenMemberUnally },
        //{ "character_coven_kick",       TeamManager.OnReceiveCovenMemberKick },
        //{ "coven_invite_requested",     TeamManager.OnReceiveCovenMemberRequest },
        //{ "coven_member_promoted",      TeamManager.OnReceiveCovenMemberPromote },
        //{ "coven_member_titled",        TeamManager.OnReceiveCovenMemberTitleChange },
        //{ "coven_member_join",          TeamManager.OnReceiveCovenMemberJoin },
        //{ "coven_request_invite",       TeamManager.OnReceiveRequestInvite },
        //{ "coven_member_left",          TeamManager.OnReceiveCovenMemberLeave },
        //{ "coven_disbanded",            TeamManager.OnReceiveCovenDisbanded },
        //{ "character_coven_invite",     TeamManager.OnReceivedCovenInvite },
        //{ "coven_member_invited",       TeamManager.OnReceivedPlayerInvited },
        //{ "character_coven_reject",     TeamManager.OnReceiveRequestRejected },
        //{ "coven_created",              TeamManager.OnCovenCreated_Websocket },

        //{ "location_spirit_summon",     PlaceOfPower.OnLocationSpiritSummon },
    };

    void Awake()
    {
        Instance = this;
        Application.targetFrameRate = 30;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

#if DISABLE_LOG
        Debug.unityLogger.logEnabled = false;
#endif
    }

    public void InitiateSocketConnection(bool isRefresh = false)
    {
        _isRefreshingConnection = isRefresh;
        if (isRefresh)
        {
            DisconnectFromSocket();
        }
        ConnectToSocket();
    }

    public bool IsConnected()
    {
        if (_socketManager != null)
        {
            return _socketManager.Socket.IsOpen;
        }
        return false;
    }

    private void ConnectToSocket()
    {
        Debug.Log("Connecting to Socket");

        SocketOptions socketOptions = new SocketOptions()
        {
            AdditionalQueryParams = new PlatformSupport.Collections.ObjectModel.ObservableDictionary<string, string>()
        };
        socketOptions.AdditionalQueryParams.Add("token", LoginAPIManager.wssToken);

        _socketManager = new SocketManager(new System.Uri(CovenConstants.wssAddress), socketOptions)
        {
            Encoder = new JsonDotNetEncoder()
        };
        _socketManager.Socket.On(SocketIOEventTypes.Connect, OnConnect);
        _socketManager.Socket.On(SocketIOEventTypes.Disconnect, OnDisconnect);
        _socketManager.Socket.On(SocketIOEventTypes.Error, OnError);
        _socketManager.Socket.On("game.event", OnGameEvent);

#if LOCAL_API
        UnityMainThreadDispatcher.Instance().Enqueue(LoginAPIManager.WebSocketConnected);
#else
        _socketManager.Open();
#endif
    }    

    #region Socket 

    private void OnConnect(Socket socket, Packet packet, object[] args)
    {
        Debug.LogFormat("Connected to Socket: {0} - Token: {1}", CovenConstants.wssAddress, LoginAPIManager.wssToken);

        if (!_isRefreshingConnection)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(LoginAPIManager.WebSocketConnected);
        }

        StartCoroutine(ReadFromQueue());
    }

    private void OnGameEvent(Socket socket, Packet packet, object[] args)
    {
        if (!FTFManager.InFTF)
        {
            string command = args[0].ToString();
            string data = args[1].ToString();
            //Debug.LogFormat("Queueing Response from Socket: {0} - {1}", command, data);
            CommandResponse response = new CommandResponse()
            {
                Command = command,
                Data = data
            };
            responsesQueue.Enqueue(response);
        }   
    }

    private void OnError(Socket socket, Packet packet, object[] args)
    {
        if (args != null && args.Length > 0)
        {
            string errorMessage = args[0].ToString();
            Debug.LogErrorFormat("Socket Error: {0}", errorMessage);
        }

        if (!LoginAPIManager.accountLoggedIn)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(LoginAPIManager.initiateLogin);
        }
        else
        {
            UnityMainThreadDispatcher.Instance().Enqueue(GameResyncHandler.ResyncGame);
        }
    }

    private void OnDisconnect(Socket socket, Packet packet, object[] args)
    {
        if (args != null && args.Length > 0)
        {
            string errorMessage = args[0].ToString();
            Debug.Log(string.Concat("Disconnected from Socket: ", errorMessage));
        }
    }

#endregion

    private void DisconnectFromSocket()
    {
        Debug.Log("Disconnecting from socket");

        StopAllCoroutines();

        if (_socketManager != null)
        {
            _socketManager.Socket.Off(SocketIOEventTypes.Connect, OnConnect);
            _socketManager.Socket.Off(SocketIOEventTypes.Disconnect, OnDisconnect);
            _socketManager.Socket.Off(SocketIOEventTypes.Error, OnError);
            _socketManager.Socket.Off("game.event", OnGameEvent);

            _socketManager.Socket.Disconnect();
            _socketManager = null;
        }
    }

    protected virtual void OnDestroy()
    {
        DisconnectFromSocket();
    }

    public void AddMessage(CommandResponse response)
    {
        responsesQueue.Enqueue(response);
    }

    private IEnumerator ReadFromQueue()
    {
        int batchIndex = 0;
        int batchSize = 50;

        while (true)
        {
            while (_socketManager.Socket.IsOpen && !SocketPaused && responsesQueue.Count > 0)
            {
                CommandResponse response = responsesQueue.Dequeue();

                try
                {
                    ManageData(response);
                }
                catch (System.Exception e)
                {
                    string innerException = e.InnerException != null ? e.InnerException.Message : string.Empty;
                    string debugString = string.Concat("Error parsing ws event.",
                                                       System.Environment.NewLine, "Exception: ", e.Message,
                                                       System.Environment.NewLine, "InnerException: ", innerException,
                                                       System.Environment.NewLine, "Stacktrace: ", e.StackTrace,
                                                       System.Environment.NewLine, "nData: ", response);
                    Debug.LogError(debugString);
                }

                batchIndex++;
                if (batchIndex >= batchSize)
                {
                    batchSize = 0;
                    yield return null;
                }
            }

            yield return null;
        }
    }

    public void ManageData(CommandResponse response)
    {
        OnResponseParsedEvent?.Invoke(response);

        if (m_EventActionDictionary.ContainsKey(response.Command))
        {
            //Debug.LogFormat("Invoking Response from Socket: {0} - {1}", response.Command, response.Data);
            m_EventActionDictionary[response.Command].HandleResponse(response.Data);
        }
        else if (response.Command != "character_daily_reset")
        {
            Debug.LogError("Command not implemented: " + response.Command + "\n" + response.Data);
        }
    }
}

public class CommandResponse
{
    public string Command { get; set; }
    public string Data { get; set; }
}

public class WSData
{
    public string json { get; set; }

    public string command { get; set; }

    public string instance { get; set; }

    public Condition condition { get; set; }
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

    //public Result result { get; set; }

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

    public ulong xpGain { get; set; }

    public ulong newXp { get; set; }

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

    public double timestamp { get; set; }

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

    public uint casterEnergy { get; set; }

    public double cooldownTime { get; set; }

    public ChannelingData channeling { get; set; }
}

public class CreatrixData
{
    public int[] amount { get; set; }
    public string[] type { get; set; }
    public string id { get; set; }
}

//"channeling":{"instance":"local:23cc3a98-aa1e-4259-b4c6-851cc1fe2977","power":10,"resilience":10,"crit":1,"limit":20,"tick":1},
public class ChannelingData
{
    public string instance;
    public int power;
    public int resilience;
    public int crit;
    public float limit;
    public float tick;
}

public sealed class JsonDotNetEncoder : IJsonEncoder
{
    public List<object> Decode(string json)
    {
        return JsonConvert.DeserializeObject<List<object>>(json);
    }

    public string Encode(List<object> obj)
    {
        return JsonConvert.SerializeObject(obj);
    }
}
