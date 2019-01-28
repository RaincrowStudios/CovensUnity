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

    public GameObject shoutBox;
    public int totalMessages = 0;
    public static bool websocketReady = false;
    public MovementManager MM;
    public WebSocket curSocket;
    bool canRun = true;
    bool refresh = false;
    Thread WebSocketProcessing;

    public bool ShowMessages = false;

    public Queue<WSData> wssQueue = new Queue<WSData>();

    private const bool localAPI = 
#if LOCAL_API 
            true;
#else
            false;
#endif

    void Awake()
    {
        Instance = this;
        Application.targetFrameRate = 45;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        LoginAPIManager.OnGetCharacter += OnLoginSuccess;
    }

    private void OnLoginSuccess()
    {
        LoginAPIManager.OnGetCharacter -= OnLoginSuccess;

        MM = MovementManager.Instance;
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

        print("Connecting to WSS @ " + Constants.wssAddress + LoginAPIManager.wssToken);
        if (refresh)
        {
            //			print("Reconnect WSS");
        }
        curSocket = new WebSocket(new Uri(Constants.wssAddress + LoginAPIManager.wssToken));

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

        //		yield return new WaitForSeconds (1);
        //		try{
        //		if (curSocket.RecvString ().ToString() == "200") {
        //			print ("OK from WSS");
        //			LoginAPIManager.WebSocketConnected ();
        //		}
        //		}catch(System.Exception e){
        //			Debug.LogError (e);
        //			LoginUIManager.Instance.initiateLogin ();	
        //		}
        //		string reply1 = curSocket.RecvString ();
        //		Debug.Log (reply1 + " WebsocketReply"); 
        //		while (true) {
        //			string reply = curSocket.RecvString ();
        //			if (reply != null) {
        //				if (reply != "200") {
        //					if (LoginAPIManager.loggedIn && websocketReady) {
        //						if (ShowMessages)
        //							print (reply);
        //						curMessage = reply;
        //						ParseJson (reply);
        //					}
        //				} else {
        //					print ("OK from WSS");
        //					if(!refresh)
        //					LoginAPIManager.WebSocketConnected ();
        //				}
        //			}
        //			if (curSocket.error != null) {
        //				if (!LoginAPIManager.loggedIn)
        //					LoginUIManager.Instance.initiateLogin ();
        //				else {
        //					PlayerManager.Instance.initStart();
        //				}
        //				Debug.LogError ("Error: " + curSocket.error);
        //				break;
        //			}
        //			yield return 0;
        //		}
        //		curSocket.Close ();
    }


    public void HandleThread()
    {
        //		AbortThread ();
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
                //				print (reply + "  reply");
                if (reply != "200")
                {
                    if (LoginAPIManager.loggedIn && websocketReady)
                    {

                        ManageThreadParsing(reply);
                    }
                }
                else
                {
                    //					print ("Refresh Success!");
                    if (!refresh)
                    {
                        UnityMainThreadDispatcher.Instance().Enqueue(LoginAPIManager.WebSocketConnected);
                    }
                    else
                    {
                        //						print ("Refresh Success!");
                    }
                }
            }
            if (curSocket.error != null)
            {
                if (!LoginAPIManager.loggedIn)
                    UnityMainThreadDispatcher.Instance().Enqueue(LoginUIManager.Instance.initiateLogin);
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

    public void FakeAddMessage(string json)
    {

        var data = JsonConvert.DeserializeObject<WSData>(json);
        data.json = json;
        wssQueue.Enqueue(data);
    }

    public void ManageThreadParsing(string json)
    {
        if (!LoginAPIManager.FTFComplete)
        {
            return;
        }
        try
        {
            var data = JsonConvert.DeserializeObject<WSData>(json);
            data.json = json;
            var pData = PlayerDataManager.playerData;
            if (data.command.Contains("character") || data.command.Contains("coven"))
            {
                wssQueue.Enqueue(data);
                return;
            }
            if (MapSelection.currentView == CurrentView.MapView)
            {
                if (data.command.Contains("token") || data.command.Contains("spell_cast"))
                {
                    wssQueue.Enqueue(data);
                }
                else if (data.command.Contains("condition"))
                {
                    if (data.condition.bearer == pData.instance)
                    {
                        wssQueue.Enqueue(data);
                    }
                }
                else if (data.command == map_energy_change || data.command == map_level_up || data.command == map_degree_change)
                {
                    if (data.instance == pData.instance)
                    {
                        //						print (json);
                        wssQueue.Enqueue(data);
                    }
                }
                else if (data.command == map_location_gained || data.command == map_location_lost || data.command == map_shout)
                {
                    wssQueue.Enqueue(data);
                }
                else if (data.command == map_immunity_add || data.command == map_immunity_remove)
                {
                    if (data.immunity == pData.instance || data.instance == pData.instance)
                    {
                        wssQueue.Enqueue(data);
                    }
                }
                else if (data.command == map_portal_summon)
                {
                    wssQueue.Enqueue(data);
                }

            }
            else
            {
                if (data.command.Contains("token"))
                {
                    if (!data.command.Contains("remove"))
                    {
                        if (data.token.instance == pData.instance || data.token.instance == MarkerSpawner.instanceID)
                        {
                            wssQueue.Enqueue(data);
                        }
                    }
                    else
                    {
                        if (LocationUIManager.isLocation)
                        {
                            wssQueue.Enqueue(data);
                        }
                    }
                }
                else if (data.command.Contains("condition"))
                {
                    if (data.condition.bearer == pData.instance || data.condition.bearer == MarkerSpawner.instanceID)
                    {
                        wssQueue.Enqueue(data);
                    }
                }
                else if (data.command == map_energy_change)
                {
                    if (data.instance == pData.instance || data.instance == MarkerSpawner.instanceID)
                    {
                        wssQueue.Enqueue(data);
                    }
                }
                else if (data.command == map_spell_cast)
                {
                    if (data.casterInstance == pData.instance || data.targetInstance == pData.instance)
                    {
                        wssQueue.Enqueue(data);
                    }
                }
                else if (data.command == map_level_up)
                {
                    if (data.instance == pData.instance || data.instance == MarkerSpawner.instanceID)
                    {
                        wssQueue.Enqueue(data);
                    }
                }
                else if (data.command == map_immunity_add || data.command == map_immunity_remove)
                {
                    if (data.immunity == pData.instance || data.instance == pData.instance)
                    {
                        wssQueue.Enqueue(data);
                    }
                }
                else if (data.command == map_portal_summon)
                {
                    wssQueue.Enqueue(data);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(json + " || " + e);
        }
    }

    IEnumerator ReadFromQueue()
    {
        while (canRun)
        {
            if (wssQueue.Count > 0)
                ManageData(wssQueue.Dequeue());
            yield return null;
        }
    }

    IEnumerator BootCharacterLocation(WSData data, float delay = 0)
    {
        yield return new WaitForSeconds(delay);
        var lm = LocationUIManager.Instance;
        if (LocationUIManager.isLocation)
        {
            if (SummoningManager.isOpen)
            {
                SummoningManager.Instance.Exit();
            }
            lm.Escape();
        }
        yield return null;
    }

    IEnumerator DelayExitIso()
    {
        yield return new WaitForSeconds(7);
        SpellManager.Instance.Exit();
    }

    public void ManageData(WSData data)
    {
        OnResponseParsedEvt(data);
        try
        {

            if (LoginAPIManager.FTFComplete && !CheckMsgState(data.timeStamp))
                return;

            var pData = PlayerDataManager.playerData;
            if (data.command == character_new_signature)
            {
                //			PlayerDataManager.playerData.signatures.Add (data.signature);
                //				print (data.json);
                SpellManager.Instance.OnSignatureDiscovered(data);

            }

            else if (data.command == character_death)
            {
                Debug.Log(data.json);
                string msg = "";

                if (data.displayName == pData.displayName)
                {
                    if (data.action.Contains("spell"))
                    {
                        msg = "You used the last of your energy with that spell.";
                    }
                    else if (data.action == "portal")
                    {
                        msg = "You used all of your energy attacking that portal.";
                    }
                    else if (data.action == "summon")
                    {
                        msg = "You used all of your energy in the summoning ritual.";
                    }
                    else if (data.action == "backfire")
                    {
                        msg = "Oh, dear. You were close to a Signature spell, but one wrong ingredient caused this spell to backfire.";
                    }
                }
                else
                {
                    if (data.spirit != "")
                    {
                        string s = "";
                        if (data.degree < 0)
                            s += " Shadow witch ";
                        else if (data.degree > 0)
                            s += " White witch ";
                        else
                            s = "Grey witch ";


                        msg = "The " + s + data.displayName + " has taken all your energy.";
                    }
                    else
                    {
                        msg = data.displayName + "'s " + DownloadedAssets.spiritDictData[data.spirit].spiritName + " has attacked you, taking all of your energy.";
                    }

                }
                PlayerManagerUI.Instance.ShowDeathReason(msg);

            }
            else if (data.command == character_new_spirit)
            {
                Debug.Log(data.json);
                HitFXManager.Instance.titleSpirit.text = DownloadedAssets.spiritDictData[data.spirit].spiritName;
                HitFXManager.Instance.titleDesc.text = "You now have the knowledge to summon " + DownloadedAssets.spiritDictData[data.spirit].spiritName;
                HitFXManager.Instance.isSpiritDiscovered = true;
                PlayerDataManager.playerData.KnownSpiritsList.Add(data.spirit);
                var k = new KnownSpirits();
                k.banishedOn = data.banishedOn;
                k.id = data.spirit;
                k.location = data.location;
                PlayerDataManager.playerData.knownSpirits.Add(k);
                //add data.spirit, data.banishedOn, data.location to character's knownSpirits list
            }
            else if (data.command == character_location_gained)
            {
                Utilities.Log(data.json);
                LocationUIManager.Instance.CharacterLocationGained(data.instance);
                //inform character data.locationName has been gained by data.displayName
                //remove data.instance from controlled PoP if viewing
            }
            else if (data.command == character_location_lost)
            {
                Utilities.Log(data.json);
                LocationUIManager.Instance.CharacterLocationLost(data.instance);
                //inform character the data.locationName has been lost
                //remove data.instance from controlled PoP if viewing
            }
            else if (data.command == map_location_lost)
            {
                print(data.json);
                LocationUIManager.controlledBy = data.controlledBy;
                LocationUIManager.Instance.LocationLost(data);
                if (ShowSelectionCard.isLocationCard && data.location == MarkerSpawner.instanceID)
                {
                    var mData = MarkerSpawner.SelectedMarker;
                    mData.controlledBy = data.controlledBy;
                    mData.spiritCount = data.spiritCount;
                    mData.isCoven = data.isCoven;
                    ShowSelectionCard.Instance.SetupLocationCard();
                }
            }
            else if (data.command == map_location_gained)
            {
                print(data.json);
                LocationUIManager.controlledBy = data.controlledBy;
                LocationUIManager.Instance.LocationGained(data);
                if (ShowSelectionCard.isLocationCard && data.location == MarkerSpawner.instanceID)
                {
                    print(data.json);
                    var mData = MarkerSpawner.SelectedMarker;
                    mData.controlledBy = data.controlledBy;
                    mData.spiritCount = data.spiritCount;
                    mData.isCoven = data.isCoven;
                    ShowSelectionCard.Instance.SetupLocationCard();
                }
            }
            else if (data.command == character_location_reward)
            {
                //inform character that data.locationName has rewarded them data.reward of gold
                PlayerDataManager.playerData.gold += data.reward;
                UILocationRewards.Instance.Show(data);
            }
            else if (data.command == character_spirit_banished)
            {
                //remove from active spirits if viewing
            }
            else if (data.command == character_spirit_expired)
            {
                //inform character that spirit (data.instance, data.spirit) has expired
                //remove from activie spirits if in view
            }
            else if (data.command == character_spirit_sentinel)
            {
                //inform character that spirit (data.instance, data.spirit) sees a new enemy
            }
            else if (data.command == character_spirit_summoned)
            {
                //inform character that spirit (data.instance, data.spirit) from portal (data.portalInstance) has been summoned
                //inform character of data.xpGain
                //			PlayerDataManager.playerData.xp += data.xpGain;
                //remove portal from active portals if in view
                //add spirit to active spirts if in view
            }
            else if (data.command == character_location_boot)
            {
                //				print ("Booting");
                if (MapSelection.currentView == CurrentView.IsoView)
                {
                    SpellManager.Instance.Exit();
                    StartCoroutine(BootCharacterLocation(data, 1.8f));
                    return;
                }
                StartCoroutine(BootCharacterLocation(data));
            }
            //MAP COMMANDS
            else if (data.command == map_condition_add)
            {

                if (data.condition.bearer == pData.instance)
                {
                    ConditionsManager.Instance.WSAddCondition(data.condition);
                    if (data.condition.status == "silenced")
                    {
                        Debug.Log("SILENCED!!");
                        BanishManager.silenceTimeStamp = data.expiresOn;
                        BanishManager.Instance.Silenced(data);
                    }
                    if (data.condition.status == "bound")
                    {
                        Debug.Log("BOUND!!");
                        BanishManager.bindTimeStamp = data.expiresOn;
                        BanishManager.Instance.Bind(data);
                        if (LocationUIManager.isLocation)
                        {
                            LocationUIManager.Instance.Bind(true);
                        }
                    }
                    if (MapSelection.currentView == CurrentView.IsoView)
                    {
                        ConditionsManagerIso.Instance.WSAddCondition(data.condition, true);
                    }

                }
                else if (data.condition.bearer == MarkerSpawner.instanceID)
                {

                    MarkerSpawner.SelectedMarker.conditionsDict.Add(data.condition.instance, data.condition);
                    if (MapSelection.currentView == CurrentView.IsoView)
                    {
                        ConditionsManagerIso.Instance.WSAddCondition(data.condition, false);
                    }

                }
            }
            else if (data.command == map_condition_remove)
            {

                if (data.condition.bearer == pData.instance)
                {

                    if (MapSelection.currentView == CurrentView.IsoView)
                    {
                        ConditionsManagerIso.Instance.WSRemoveCondition(data.condition.instance, true);
                    }
                    ConditionsManager.Instance.WSRemoveCondition(data.condition.instance);
                    bool isSilenced = false;
                    bool isBound = false;

                    foreach (var item in PlayerDataManager.playerData.conditionsDict)
                    {
                        if (item.Value.status == "silenced")
                        {
                            BanishManager.silenceTimeStamp = item.Value.expiresOn;
                            isSilenced = true;
                            break;
                        }
                        else
                            isSilenced = false;
                    }

                    foreach (var item in PlayerDataManager.playerData.conditionsDict)
                    {
                        if (item.Value.status == "bound")
                        {
                            BanishManager.bindTimeStamp = item.Value.expiresOn;
                            isBound = true;
                            break;
                        }
                        else
                            isBound = false;
                    }

                    if (data.condition.status == "silenced")
                    {
                        if (!isSilenced)
                        {
                            BanishManager.Instance.unSilenced();
                        }
                    }

                    if (!isBound && data.condition.status == "bound")
                    {
                        BanishManager.Instance.Unbind();
                        if (LocationUIManager.isLocation)
                        {
                            LocationUIManager.Instance.Bind(false);
                        }
                    }

                }
                else if (data.condition.bearer == MarkerSpawner.instanceID)
                {
                    //					print ("<color=red>" + data.json + "</color>");
                    if (MapSelection.currentView == CurrentView.IsoView)
                    {
                        ConditionsManagerIso.Instance.WSRemoveCondition(data.condition.instance, false);
                    }
                    if (MarkerSpawner.SelectedMarker.conditionsDict.ContainsKey(data.condition.instance))
                    {
                        //					print ("Contains Condition");
                        MarkerSpawner.SelectedMarker.conditionsDict.Remove(data.condition.instance);
                        //					print ("Removed Condition");
                    }
                    else
                    {
                        //					print ("<b>Did not contain the condition! >>>>></b> ==" + data.condition.instance);
                    }
                }
            }
            else if (data.command == map_condition_trigger)
            {
                if (data.condition.bearer == pData.instance && pData.conditionsDict.ContainsKey(data.condition.instance))
                {
                    ConditionsManager.Instance.ConditionTrigger(data.condition.instance);
                    if (MapSelection.currentView == CurrentView.IsoView)
                    {
                        ConditionsManagerIso.Instance.ConditionTrigger(data.condition.instance, true);
                    }
                }
                if (data.condition.bearer == MarkerSpawner.instanceID)
                {
                    if (MapSelection.currentView == CurrentView.IsoView)
                    {
                        ConditionsManagerIso.Instance.WSRemoveCondition(data.condition.instance, false);
                    }
                    else
                    {
                        if (MarkerSpawner.SelectedMarker.conditionsDict.ContainsKey(data.condition.instance))
                        {
                            MarkerSpawner.SelectedMarker.conditionsDict.Remove(data.condition.instance);
                        }
                    }
                }
            }
            else if (data.command == map_energy_change)
            {

                //			Debug.Log (logMessage);

                if (data.instance == pData.instance)
                {
                    pData.energy = data.newEnergy;
                    if (pData.state != "dead" && data.newState == "dead")
                    {
                        if (IsoPortalUI.isPortal)
                            IsoPortalUI.instance.DisablePortalCasting();

                        if (MapSelection.currentView == CurrentView.IsoView)
                        {
                            StartCoroutine(DelayExitIso());
                            pData.state = data.newState;
                            PlayerManagerUI.Instance.UpdateEnergy();
                            return;
                        }
                        else if (MapSelection.currentView == CurrentView.MapView && !LocationUIManager.isLocation)
                        {
                            DeathState.Instance.ShowDeath();
                        }
                    }
                    if (pData.state != "vulnerable" && data.newState == "vulnerable")
                    {
                        //						print ("Vulnerable!");
                        PlayerManagerUI.Instance.ShowElixirVulnerable(false);
                    }

                    if (pData.state == "dead" && data.newState != "dead")
                    {
                        DeathState.Instance.Revived();
                    }
                    pData.state = data.newState;
                    //				SpellCarouselManager.Instance.WSStateChange ();
                    PlayerManagerUI.Instance.UpdateEnergy();
                    if (MapSelection.currentView == CurrentView.IsoView)
                    {
                        ShowSelectionCard.Instance.ChangeSelfEnergy();
                    }
                }
                if (MarkerSpawner.instanceID == data.instance)
                {
                    if (MapSelection.currentView == CurrentView.IsoView)
                    {
                        if (MarkerSpawner.selectedType == MarkerSpawner.MarkerType.portal)
                        {
                            if (data.newState != "dead")
                            {
                                //								print ("Energy Change Portal to " + data.newEnergy);
                                IsoPortalUI.instance.PortalFX(data.newEnergy);
                                MarkerSpawner.SelectedMarker.energy = data.newEnergy;
                                IsoTokenSetup.Instance.ChangeEnergy();
                            }
                            else
                            {
                                IsoPortalUI.instance.Destroyed();
                            }
                            return;
                        }
                        if (MarkerSpawner.SelectedMarker.state != "dead" && data.newState == "dead")
                        {
                            if (MarkerSpawner.selectedType == MarkerSpawner.MarkerType.spirit)
                            {
                                HitFXManager.Instance.TargetDead(true);
                                //							print ("Closing Spirit Off!");
                                return;
                            }
                            else
                            {
                                HitFXManager.Instance.TargetDead();
                            }
                        }
                        else if (MarkerSpawner.SelectedMarker.state == "dead" && data.newState != "dead")
                        {
                            HitFXManager.Instance.TargetRevive();
                        }
                        string oldState = MarkerSpawner.SelectedMarker.state;
                        MarkerSpawner.SelectedMarker.state = data.newState;
                        MarkerSpawner.SelectedMarker.energy = data.newEnergy;
                        if (oldState != data.newState)
                        {
                            SpellManager.Instance.StateChanged();
                        }

                        IsoTokenSetup.Instance.ChangeEnergy();
                    }
                    else if (MapSelection.currentView == CurrentView.MapView)
                    {
                        ShowSelectionCard.Instance.ChangeEnergy();
                    }

                }
            }
            else if (data.command == map_immunity_add)
            {
                if (data.immunity == pData.instance)
                {

                    if (MarkerSpawner.ImmunityMap.ContainsKey(data.instance))
                        MarkerSpawner.ImmunityMap[data.instance].Add(data.immunity);
                    else
                        MarkerSpawner.ImmunityMap[data.instance] = new HashSet<string>() { data.immunity };

                    if (MapSelection.currentView == CurrentView.IsoView)
                    {
                        if (data.instance == MarkerSpawner.instanceID)
                        {
                            //							HitFXManager.Instance.SetImmune (true);
                            StartCoroutine(DelayWitchImmune());
                        }
                    }
                    if (MapSelection.currentView == CurrentView.MapView && MarkerSpawner.instanceID == data.instance)
                    {
                        ShowSelectionCard.Instance.SetCardImmunity(true);
                    }
                    MarkerManager.SetImmunity(true, data.instance);
                }
            }
            else if (data.command == map_immunity_remove)
            {
                if (data.immunity == pData.instance)
                {
                    string logMessage = "<color=#008bff> Map_immunity_remove</color>";
                    if (data.instance == MarkerSpawner.instanceID && data.instance == pData.instance)
                    {
                        logMessage += "\n <b>" + MarkerSpawner.SelectedMarker.displayName + " <color=#008bff> is no longer Immune to </color> " + pData.displayName + "</b>";
                    }
                    if (MarkerSpawner.ImmunityMap.ContainsKey(data.instance))
                    {
                        if (MarkerSpawner.ImmunityMap[data.instance].Contains(data.immunity))
                            MarkerSpawner.ImmunityMap[data.instance].Remove(data.immunity);
                    }

                    if (MapSelection.currentView == CurrentView.IsoView)
                    {
                        if (data.instance == MarkerSpawner.instanceID)
                        {
                            HitFXManager.Instance.SetImmune(false);
                        }
                    }
                    if (MapSelection.currentView == CurrentView.MapView && MarkerSpawner.instanceID == data.instance)
                    {
                        ShowSelectionCard.Instance.SetCardImmunity(false);
                    }
                    MarkerManager.SetImmunity(false, data.instance);
                }
            }
            else if (data.command == map_level_up)
            {
                print(data.json);
                if (data.instance == pData.instance)
                {
                    pData.xpToLevelUp = data.xpToLevelUp;
                    pData.level = data.newLevel;
                    pData.baseEnergy = data.newBaseEnergy;
                    PlayerManagerUI.Instance.playerlevelUp();
                    PlayerManagerUI.Instance.UpdateEnergy();
                    SoundManagerOneShot.Instance.PlayLevel();
                }
                if (data.instance == MarkerSpawner.instanceID)
                {
                    MarkerSpawner.SelectedMarker.level = data.newLevel;
                    if (MapSelection.currentView == CurrentView.MapView)
                    {
                        ShowSelectionCard.Instance.ChangeLevel();
                    }
                    else
                    {
                        IsoTokenSetup.Instance.ChangeLevel();
                    }
                }

            }
            else if (data.command == map_degree_change)
            {
                if (data.instance == pData.instance)
                {
                    pData.degree = data.newDegree;
                    PlayerManagerUI.Instance.playerDegreeChanged();

                    if (data.oldDegree < data.newDegree)
                    {
                        SoundManagerOneShot.Instance.PlayWhite();
                    }
                    else
                    {
                        SoundManagerOneShot.Instance.PlayShadow();
                    }

                }
                if (data.instance == MarkerSpawner.instanceID)
                {
                    MarkerSpawner.SelectedMarker.degree = data.newDegree;
                    if (MapSelection.currentView == CurrentView.MapView)
                    {
                        ShowSelectionCard.Instance.ChangeDegree();
                    }
                }
            }
            else if (data.command == map_shout)
            {
                if (data.instance == pData.instance)
                {
                    var g = Utilities.InstantiateObject(shoutBox, PlayerManager.marker.instance.transform);
                    g.GetComponent<ShoutBoxData>().Setup(data.displayName, data.shout);
                }
                else
                {
                    if (MarkerManager.Markers.ContainsKey(data.instance))
                    {
                        if (MapsAPI.Instance.zoom > 14)
                        {
                            if (MarkerManager.Markers[data.instance][0].inMapView)
                            {
                                var g = Utilities.InstantiateObject(shoutBox, MarkerManager.Markers[data.instance][0].instance.transform);
                                g.GetComponent<ShoutBoxData>().Setup(data.displayName, data.shout);
                            }
                        }
                    }
                }
            }
            else if (data.command == map_spell_cast)
            {
                if (data.casterInstance == pData.instance)
                {
                    if (data.target == "portal")
                        return;
                    //				SpellSpiralLoader.Instance.LoadingDone ();
                    SpellManager.Instance.loadingFX.SetActive(false);
                    SpellManager.Instance.mainCanvasGroup.interactable = true;
                    if (data.spell == "spell_banish" && data.result.effect == "success")
                    {
                        HitFXManager.Instance.Banish();
                        return;
                    }
                    if (data.targetInstance == MarkerSpawner.instanceID && MapSelection.currentView == CurrentView.IsoView)
                    {
                        HitFXManager.Instance.Attack(data);
                    }
                    if (MapSelection.currentView == CurrentView.IsoView)
                    {
                        if (data.result.effect == "fail" || data.result.effect == "fizzle")
                        {
                            HitFXManager.Instance.Attack(data);
                        }
                        if (data.result.effect == "backfire")
                        {
                            HitFXManager.Instance.Attack(data);
                        }
                    }


                }
                if (LocationUIManager.isLocation && MapSelection.currentView != CurrentView.IsoView)
                {
                    if (data.result.effect == "success")
                        MovementManager.Instance.AttackFXOther(data);
                    return;
                }
                if (data.targetInstance == pData.instance && MapSelection.currentView == CurrentView.MapView)
                {
                    MovementManager.Instance.AttackFXSelf(data);
                }
                if (data.targetInstance != pData.instance && MapSelection.currentView == CurrentView.MapView)
                {
                    MovementManager.Instance.AttackFXOther(data);
                }

                if (data.targetInstance == pData.instance)
                {

                    if (data.spell == "spell_banish")
                    {
                        BanishManager.banishCasterID = data.caster;
                    }

                    if (data.result.total < 0)
                    {
                        MarkerManager.StanceDict[data.casterInstance] = true;
                    }
                    else if (data.result.total > 0)
                    {
                        MarkerManager.StanceDict[data.casterInstance] = false;
                    }
                    if (MarkerManager.Markers.ContainsKey(data.casterInstance))
                    {
                        var tokenD = MarkerManager.Markers[data.casterInstance][0].customData as Token;
                        MarkerSpawner.Instance.SetupStance(MarkerManager.Markers[data.casterInstance][0].instance.transform, tokenD);
                    }

                    if (MapSelection.currentView == CurrentView.MapView)
                    {
                        string msg = "";
                        if (data.spell != "attack")
                        {
                            if (data.result.total > 0)
                            {
                                msg = " cast " + DownloadedAssets.spellDictData[data.spell].spellName + " on you. You gain " + data.result.total.ToString() + " Energy.";
                            }
                            else if (data.result.total < 0)
                            {
                                msg = " cast " + DownloadedAssets.spellDictData[data.spell].spellName + " on you. You lose " + data.result.total.ToString() + " Energy.";
                            }
                            else
                            {
                                msg = " cast " + DownloadedAssets.spellDictData[data.spell].spellName + " on you.";
                            }
                        }
                        else
                        {
                            msg = " attacked you, you lose " + data.result.total.ToString() + " Energy.";
                        }

                        if (data.casterType == "witch")
                        {
                            msg = data.caster + msg;
                        }
                        else if (data.casterType == "spirit")
                        {
                            msg = "Spirit " + DownloadedAssets.spiritDictData[data.caster].spiritName + msg;
                        }
                        else
                        {
                            return;
                        }
                        if (MarkerManager.Markers.ContainsKey(data.casterInstance))
                        {
                            var cData = MarkerManager.Markers[data.casterInstance][0].customData as Token;
                            var Sprite = PlayerNotificationManager.Instance.ReturnSprite(cData.male);
                            PlayerNotificationManager.Instance.showNotification(msg, Sprite);
                        }
                    }
                }

                if (data.casterInstance == MarkerSpawner.instanceID && data.targetInstance == pData.instance && MapSelection.currentView == CurrentView.IsoView)
                {
                    if (data.result.effect == "backfire")
                    {
                        HitFXManager.Instance.BackfireEnemy(data);
                    }
                    else
                    {
                        HitFXManager.Instance.Hit(data);
                    }
                }
            }
            else if (data.command == character_spell_move)
            {
                if (data.spell == "spell_banish")
                {
                    if (!LocationUIManager.isLocation)
                    {
                        BanishManager.Instance.Banish(data.longitude, data.latitude);
                    }
                    StartCoroutine(BanishWaitTillLocationLeave(data));
                } // handle magic dance;
            }
            else if (data.command == map_portal_summon)
            {
                if (MarkerSpawner.instanceID == data.instance && MapSelection.currentView == CurrentView.IsoView)
                {
                    IsoPortalUI.instance.Summoned();
                    MM.RemoveMarkerIso(data.instance);
                }
                else
                {
                    if (MapSelection.currentView == CurrentView.IsoView || MapSelection.currentView == CurrentView.TransitionView)
                        MM.RemoveMarkerIso(data.instance);
                    else
                        MM.RemoveMarker(data.instance);
                }

            }
            else if (data.command == map_token_add)
            {

                if (data.token.position == 0)
                {
                    var updatedData = MarkerManagerAPI.AddEnumValueSingle(data.token);
                    if (MapSelection.currentView == CurrentView.MapView)
                        MM.AddMarker(updatedData);
                    else
                        MM.AddMarkerIso(updatedData);
                }
                else
                {
                    LocationUIManager.Instance.AddToken(data.token);
                }

                //			print (data.token);
            }
            else if (data.command == character_xp_gain)
            {
                PlayerDataManager.playerData.xp = data.newXp;
                PlayerManagerUI.Instance.setupXP();
                //			print (data.token);
            }
            else if (data.command == map_token_move)
            {

                if (data.token.position == 0)
                {
                    if (MarkerManager.Markers.ContainsKey(data.token.instance))
                    {
                        double distance = MapsAPI.Instance.DistanceBetweenPointsD(PlayerDataManager.playerPos, ReturnVector2(data.token));
                        if (distance < PlayerDataManager.DisplayRadius)
                        {
                            MM.UpdateMarkerPosition(data.token);
                            if (distance > (PlayerDataManager.attackRadius))
                            {
                                if (MapSelection.currentView == CurrentView.IsoView)
                                {
                                    if (data.token.instance == MarkerSpawner.instanceID)
                                    {
                                        HitFXManager.Instance.Escape();
                                    }
                                }
                            }
                        }
                        else
                        {
                            MM.RemoveMarker(data.token.instance);
                        }
                    }
                    else
                    {
                        var updatedData = MarkerManagerAPI.AddEnumValueSingle(data.token);
                        MM.AddMarker(updatedData);
                    }
                }
            }
            else if (data.command == map_token_remove)
            {
                Utilities.Log(data.json);

                if (!LocationUIManager.isLocation)
                {
                    if (MapSelection.currentView == CurrentView.MapView)
                        MM.RemoveMarker(data.instance);
                    else
                    {

                        MM.RemoveMarkerIso(data.instance);
                    }
                }
                else
                {
                    Utilities.Log(data.json + "Not in loc");
                    LocationUIManager.Instance.RemoveToken(data.instance);
                }
            }
            else if (data.command == character_daily_progress)
            {
                //				Debug.Log (data.json);
                QuestLogUI.Instance.OnProgress(data.daily, data.count, data.silver);


                PlayerDataManager.playerData.silver += data.silver;
                PlayerManagerUI.Instance.UpdateDrachs();

            }
        }
        catch (System.Exception e)
        {
            print(data.json);
            Debug.LogError(e);
        }
    }

    Vector2 ReturnVector2(Token data)
    {
        return new Vector2(data.longitude, data.latitude);
    }

    IEnumerator BanishWaitTillLocationLeave(WSData data)
    {
        yield return new WaitUntil(() => LocationUIManager.isLocation == false);
        BanishManager.Instance.Banish(data.longitude, data.latitude);
    }

    IEnumerator DelayWitchImmune()
    {
        yield return new WaitForSeconds(4.4f);
        if (PlayerDataManager.playerData.state != "dead")
            HitFXManager.Instance.SetImmune(true);
    }


    static bool CheckMsgState(double javaTimeStamp)
    {
        System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        dtDateTime = dtDateTime.AddMilliseconds(javaTimeStamp).ToUniversalTime();
        var timeSpan = DateTime.UtcNow.Subtract(dtDateTime);
        return timeSpan.TotalSeconds < PlayerManager.reinitTime;
    }

#region wsCommands

    //CHARACTER
    string character_death = "character_death";

    string character_xp_gain = "character_xp_gain";
    string character_cooldown_add = "character_cooldown_add";
    string character_cooldown_remove = "character_cooldown_remove";

    string character_coven_invited = "character_coven_invited";
    string character_coven_kicked = "character_coven_kicked";
    string character_coven_rejected = "character_coven_rejected";

    string character_location_gained = "character_location_gained";
    string character_location_lost = "character_location_lost";
    string character_location_boot = "character_location_boot";
    string character_location_reward = "character_location_reward";

    string character_new_signature = "character_new_signature";
    string character_new_spirit = "character_new_spirit";

    string character_portal_destroyed = "character_portal_destroyed";
    string character_spell_move = "character_spell_move";

    string character_spirit_banished = "characer_spirit_banished";
    string character_daily_progress = "character_daily_progress";
    string character_spirit_expired = "character_spirit_expired";
    string character_spirit_sentinel = "character_spirit_sentinel";
    string character_spirit_summoned = "character_spirit_summoned";
    //	 string character_spirit_summoned= "character_spirit_summoned";

    //MAP
    string map_location_lost = "map_location_lost";
    string map_location_gained = "map_location_gained";
    string map_condition_add = "map_condition_add";
    string map_condition_remove = "map_condition_remove";
    string map_condition_trigger = "map_condition_trigger";

    string map_portal_summon = "map_portal_summon";
    string map_degree_change = "map_degree_change";

    string map_shout = "map_shout";

    string map_energy_change = "map_energy_change";

    string map_equipment_change = "map_equipment_change";

    string map_immunity_add = "map_immunity_add";
    string map_immunity_remove = "map_immunity_remove";

    string map_level_up = "map_level_up";

    string map_location_offer = "map_location_offer";

    string map_spell_cast = "map_spell_cast";

    string map_token_add = "map_token_add";
    string map_token_move = "map_token_move";
    string map_token_remove = "map_token_remove";

    //COVEN
    string coven_invite_requested = "coven_invite_requested";
    string coven_member_invited = "coven_member_invited";
    string coven_member_joined = "coven_member_joined";
    string coven_member_left = "coven_member_left";
    string coven_allied = "coven_allied";
    string coven_unallied = "coven_unallied";
    string coven_member_promoted = "coven_member_promoted";
    string coven_member_titled = "coven_member_titled";
    string coven_was_allied = "coven_was_allied";
    string coven_was_unallied = "coven_was_unallied";
    string coven_disbanded = "coven_disbanded";

#endregion

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
