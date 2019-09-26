using System.Collections;
using UnityEngine;
using Newtonsoft.Json;
using Raincrow;
using System.Collections.Generic;
using Raincrow.FTF;
using UnityEngine.UI;
using Raincrow.GameEventResponses;
using Raincrow.Maps;
using System.Text.RegularExpressions;

public class FTFManager : MonoBehaviour
{
    [SerializeField] private TextAsset m_TutorialJson;
    [SerializeField] private FTFHighlight m_Highlight;
    [SerializeField] private FTFButtonArea m_Button;
    [SerializeField] private FTFMessageBox m_BotMessage;
    [SerializeField] private FTFMessageBox m_TopMessage;
    [SerializeField] private FTFPointerHand m_Pointer;
    [SerializeField] private FTFSpawn m_FXSpawner;

    [Header("Savannah")]
    [SerializeField] private Animator m_Savannah;
    [SerializeField] private CanvasGroup m_SavannahCanvasGroup;

    [Header("Fortuna")]
    [SerializeField] private Animator m_Fortuna;
    [SerializeField] private CanvasGroup m_FortunaCanvasGroup;

    [Header("Witch School")]
    [SerializeField] private CanvasGroup m_WitchSchool;
    [SerializeField] private Button m_OpenWitchSchool;
    [SerializeField] private Button m_SkipWitchSchool;
    
    private static FTFManager m_Instance;

    public static bool InFTF => PlayerDataManager.IsFTF;
    public static event System.Action OnBeginFTF;
    public static event System.Action OnFinishFTF;

    public static void StartFTF()
    {
        if (m_Instance != null)
        {
            m_Instance._StartFTF();
        }
        else
        {
            LoadingOverlay.Show();
            SceneManager.LoadSceneAsync(SceneManager.Scene.FTF, UnityEngine.SceneManagement.LoadSceneMode.Additive, null, () =>
            {
                LoadingOverlay.Hide();
                m_Instance._StartFTF();
            });
        }
    }

    public static void FinishFTF(System.Action<int, string> callback)
    {
        APIManager.Instance.Post(
            "character/finishTutorial",
            "{}",
            (response, result) =>
            {
                if (result == 200)
                {
                    Debug.Log("ftf complete");

                    FinishFTFResponse data = JsonConvert.DeserializeObject<FinishFTFResponse>(response);

                    foreach (var item in data.herbs)
                        PlayerDataManager.playerData.SetIngredient(item.id, item.count);
                    foreach (var item in data.tools)
                        PlayerDataManager.playerData.SetIngredient(item.id, item.count);
                    foreach (var item in data.gems)
                        PlayerDataManager.playerData.SetIngredient(item.id, item.count);

                    PlayerDataManager.playerData.xp = data.xp;
                    //PlayerDataManager.playerData.spirits = update.spirits;
                    PlayerDataManager.playerData.tutorial = true;

                    OnFinishFTF?.Invoke();
                }
                callback?.Invoke(result, response);

            });
    }

    public static void SkipFTF()
    {
        Debug.LogError("TODO: SKIP FTF");
    }

    private int m_PreviousStepIndex;
    private int m_CurrentStepIndex;
    private List<FTFStepData> m_Steps = null;
    private bool m_RunningSetup = false;
    private int m_WitchSchoolTweenId;

    private void Awake()
    {
        m_Instance = this;

        m_BotMessage.OnClick += () => StartCoroutine(NextStep());
        m_TopMessage.OnClick += () => StartCoroutine(NextStep());
        m_Button.OnClick += () => StartCoroutine(NextStep());

        m_WitchSchool.alpha = 0;
        m_WitchSchool.gameObject.SetActive(false);
        m_OpenWitchSchool.onClick.AddListener(OnWitchSchool);
        m_SkipWitchSchool.onClick.AddListener(OnSkipWitchSchool);

        //
        m_Savannah.gameObject.SetActive(false);
        m_SavannahCanvasGroup.alpha = 0;
        m_Fortuna.gameObject.SetActive(false);
        m_FortunaCanvasGroup.alpha = 0;
        

        //override store prices for FTF
        
        List<int> originalPrices = new List<int>();
        foreach (var item in PlayerDataManager.StoreData.bundles)
        {
            originalPrices.Add(item.silver);
            item.silver = 0;
        }

        OnFinishFTF += () =>
        {
            for (int i = 0; i < originalPrices.Count; i++)
            {
                PlayerDataManager.StoreData.bundles[i].silver = originalPrices[i];
            }
        };

        //retrieve nearby locations for FTF
        UINearbyLocations.GetLocations(null);
    }
    
    [ContextMenu("Start FTF")]
    private void _StartFTF()
    {
        //load json
        //start first step

        m_CurrentStepIndex = -1;
        m_PreviousStepIndex = -1;

        LoadJson(json =>
        {
            m_Steps = JsonConvert.DeserializeObject<List<FTFStepData>>(json);

#if UNITY_EDITOR
            Setup(0);
            OnBeginFTF?.Invoke();
            StartCoroutine(SkipUntil(UnityEditor.EditorPrefs.GetInt("FTFManager.StartFrom", 0)));
            return;
#endif
            Setup(0);
            OnBeginFTF?.Invoke();
        });
    }

    private IEnumerator SkipUntil(int index)
    {
        float previousTimeScale = Time.timeScale;
        Time.timeScale *= 4;
        while (m_CurrentStepIndex < index && m_CurrentStepIndex < m_Steps.Count)
        {
            yield return StartCoroutine(NextStep());
        }
        Time.timeScale = previousTimeScale;
    }

    private void Setup(int index)
    {
        StartCoroutine(SetupCoroutine(index));
    }

    private IEnumerator SetupCoroutine(int index)
    {
        Log("step " + index);
        
        m_RunningSetup = true;
        
        //execute previous state's exitActions
        if (m_CurrentStepIndex >= 0)
        {
            FTFStepData previousStep = m_Steps[m_CurrentStepIndex];

            m_BotMessage.Hide();
            m_TopMessage.Hide();

            if (previousStep.onExit != null)
            {
                List<Coroutine> coroutines = new List<Coroutine>();
                foreach (FTFActionData _action in previousStep.onExit)
                {
                    if (string.IsNullOrEmpty(_action.method))
                        continue;

                    if (_action.parameters == null)
                        coroutines.Add(StartCoroutine(_action.method, new string[0]));
                    else
                        coroutines.Add(StartCoroutine(_action.method, _action.parameters.ToArray()));
                }

                while(coroutines.Count > 0)
                {
                    yield return coroutines[0];
                    coroutines.RemoveAt(0);
                }
            }
        }

        //set current
        m_PreviousStepIndex = m_CurrentStepIndex;
        m_CurrentStepIndex = index;

        if (m_CurrentStepIndex >= m_Steps.Count)
        {
            Log("No more steps");
            yield break;
        }

        FTFStepData step = m_Steps[m_CurrentStepIndex];

        //highlight
        if (step.highlight.show)
        {
            m_Highlight.Show(step.highlight);
        }
        else
        {
            m_Highlight.Hide();
        }

        //button block
        if (step.button.show)
        {
            m_Button.Show(step.button);
            m_BotMessage.EnableButton(false);
            m_TopMessage.EnableButton(false);
        }
        else
        {
            m_Button.Hide();
        }

        //pointer
        if (step.pointer.show)
        {
            m_Pointer.Show(step.pointer);
        }
        else
        {
            m_Pointer.Hide();
        }

        //execute new state's enterActions
        if (step.onEnter != null)
        {
            List<Coroutine> coroutines = new List<Coroutine>();
            foreach (FTFActionData _action in step.onEnter)
            {
                if (string.IsNullOrEmpty(_action.method))
                    continue;

                if (_action.parameters == null)
                   coroutines.Add(StartCoroutine(_action.method, new string[0]));
                else
                    coroutines.Add(StartCoroutine(_action.method, _action.parameters.ToArray()));
            }

            while (coroutines.Count > 0)
            {
                yield return coroutines[0];
                coroutines.RemoveAt(0);
            }
        }
        
        //setup timer
        if (step.timer > 0)
        {
            yield return new WaitForSeconds(step.timer);
            m_RunningSetup = false;
            yield return StartCoroutine(NextStep());
        }
        else
        {
            m_RunningSetup = false;
        }
    }

    private void LoadJson(System.Action<string> onComplete)
    {
        onComplete?.Invoke(m_TutorialJson.text);
    }

    public void Log(string message)
    {
        Debug.Log("[<color=cyan>FTFManager</color>] " + message);
    }

    private void OnWitchSchool()
    {
        WitchSchoolManager.Open();
        ShowWitchSchool(false, () => SceneManager.UnloadScene(SceneManager.Scene.FTF, null, null));
    }

    private void OnSkipWitchSchool()
    {
        ShowWitchSchool(false, () => SceneManager.UnloadScene(SceneManager.Scene.FTF, null, null));
    }

    private void ShowWitchSchool(bool show, System.Action onComplete)
    {
        LeanTween.cancel(m_WitchSchoolTweenId);
        m_WitchSchool.gameObject.SetActive(true);
        float start = m_WitchSchool.alpha;
        float end = show ? 1 : 0;
        m_WitchSchoolTweenId = LeanTween.value(start, end, 1f)
            .setEaseOutCubic()
            .setOnUpdate((float v) => m_WitchSchool.alpha = v)
            .setOnComplete(() =>
            {
                m_WitchSchool.gameObject.SetActive(show);
                onComplete?.Invoke();
            })
            .uniqueId;
    }

    #region AVAILABLE ACTIONS

    private IEnumerator NextStep()
    {
        yield return StartCoroutine(SetupCoroutine(m_CurrentStepIndex + 1));
    }

    private IEnumerator ShowSavannah()
    {
        m_Savannah.gameObject.SetActive(true);
        LeanTween.alphaCanvas(m_SavannahCanvasGroup, 1f, 1f).setEaseOutCubic();
        m_Savannah.Play("LeftEnter");

        yield return 0;
    }

    private IEnumerator HideSavannah()
    {
        LeanTween.alphaCanvas(m_SavannahCanvasGroup, 0f, 0.25f).setEaseOutCubic().setOnComplete(() =>
        {
            m_Savannah.gameObject.SetActive(false);
        });
        m_Savannah.Play("LeftExit", 0, 0.3f);

        yield return 0;
    }

    private IEnumerator ShowFortuna()
    {
        m_Fortuna.gameObject.SetActive(true);
        LeanTween.alphaCanvas(m_FortunaCanvasGroup, 1f, 1f).setEaseOutCubic();
        m_Fortuna.Play("RightEnter");

        yield return 0;
    }

    private IEnumerator HideFortuna()
    {
        LeanTween.alphaCanvas(m_FortunaCanvasGroup, 0f, 0.25f).setEaseOutCubic().setOnComplete(() =>
        {
            m_Fortuna.gameObject.SetActive(false);
        });
        m_Fortuna.Play("RightExit", 0, 0.3f);

        yield return 0;
    }

    private IEnumerator ShowMessage(string[] parameters)
    {
        if (parameters.Length < 1)
            throw new System.Exception("[ShowMessage] missing param[0] (message)");

        string message = LocalizeLookUp.GetText(parameters[0]);
        bool top = parameters.Length > 1 ? bool.Parse(parameters[1]) : false;
        List<string> specialKeys = new List<string>();

        MatchCollection matches = Regex.Matches(message, @"{[^}]*}", RegexOptions.IgnoreCase);
        foreach (Match m in matches)
        {
            specialKeys.Add(m.Value);
        }
        
        if (top)
        {
            m_TopMessage.Show(message, specialKeys);
            m_TopMessage.EnableButton(!m_Button.IsShowing);
        }
        else
        {
            m_BotMessage.Show(message, specialKeys);
            m_BotMessage.EnableButton(!m_Button.IsShowing);
        }

        yield return 0;
    }

    private IEnumerator SpawnSpirit(string[] parameters)
    {
        if (parameters.Length < 1)
            throw new System.Exception("[SpawnSpirit] missing param[0] (spirit id)");
                
        string json = (string)parameters[0];
        SpiritToken token = JsonConvert.DeserializeObject<SpiritToken>(json);
        token.type = "spirit";
        token.longitude += PlayerDataManager.playerData.longitude;
        token.latitude += PlayerDataManager.playerData.latitude;
        AddSpiritHandler.HandleEvent(token);

        yield return 0;
    }

    private IEnumerator SpawnPop(string[] parameters)
    {
        string json = parameters[0];
        PopToken token = JsonConvert.DeserializeObject<PopToken>(json);
        token.longitude += PlayerDataManager.playerData.longitude;
        token.latitude += PlayerDataManager.playerData.latitude;
        AddCollectableHandler.HandleEvent(token);
        yield return 0;
    }

    private IEnumerator SelectMarker(string[] parameters)
    {
        if (parameters.Length < 1)
            throw new System.Exception("[SelectMarker] missing param[0] (marker id)");

        if (parameters.Length < 2)
            throw new System.Exception("[SelectMarker] missing param[1] (marker details json)");

        string id = parameters[0];
        string json = parameters[1];

        IMarker marker = MarkerSpawner.GetMarker(id);

        bool screenLoaded = false;
        bool quickcastLoaded = false;

        if (marker.Type == MarkerManager.MarkerType.SPIRIT)
        {
            SelectSpiritData_Map details = JsonConvert.DeserializeObject<SelectSpiritData_Map>(json);
            details.token = marker.Token as SpiritToken;

            
            UISpiritInfo.Show(marker as SpiritMarker, details.token, null, () => screenLoaded = true);
            UIQuickCast.Open(() => quickcastLoaded = true);
            yield return new WaitForSeconds(0.5f);
            UISpiritInfo.SetupDetails(details);
            UIQuickCast.UpdateCanCast(marker, details);
        }
        else if (marker.Type == MarkerManager.MarkerType.WITCH)
        {
            SelectWitchData_Map details = JsonConvert.DeserializeObject<SelectWitchData_Map>(json);
            details.token = marker.Token as WitchToken;

            UIPlayerInfo.Show(marker as WitchMarker, details.token, null, () => screenLoaded = true);
            UIQuickCast.Open(() => quickcastLoaded = true);
            yield return new WaitForSeconds(0.5f);
            UIPlayerInfo.SetupDetails(details);
            UIQuickCast.UpdateCanCast(marker, details);
        }
        else if (marker.Type == MarkerManager.MarkerType.PLACE_OF_POWER)
        {
            screenLoaded = true;
            quickcastLoaded = true;
        }

        while (screenLoaded == false || quickcastLoaded == false)
            yield return 0;
    }

    private IEnumerator SocketEvent(string[] parameters)
    {
        if (parameters.Length < 1)
            throw new System.Exception("[SocketEvent] missing param[0] (event id)");

        if (parameters.Length < 2)
            throw new System.Exception("[SocketEvent] missing param[1] (event data)");

        string id = parameters[0];
        string json = parameters[1];

        SocketClient.Instance.ManageData(new CommandResponse()
        {
            Command = id,
            Data = json
        });

        yield return 0;
    }

    private IEnumerator CastSpell(string[] parameters)
    {
        string spellId = parameters[0];
        string targetId = parameters[1];
        int damage = int.Parse(parameters[2]);

        //load
        IMarker target = MarkerSpawner.GetMarker(targetId);
        CharacterToken token = target.Token as CharacterToken;
        SpellData spell = DownloadedAssets.GetSpell(spellId);

        //show casting UI
        UIWaitingCastResult.Instance.Show(target, spell, new List<spellIngredientsData>(), null, null);

        //wait few seconds
        yield return new WaitForSecondsRealtime(0.2f);

        SpellCastHandler.SpellCastEventData data = new SpellCastHandler.SpellCastEventData();

        data.spell = spellId;

        data.caster.id = PlayerDataManager.playerData.instance;
        data.caster.name = PlayerDataManager.playerData.name;
        data.caster.type = "character";
        data.caster.energy = PlayerDataManager.playerData.energy - spell.cost;

        data.target.id = targetId;
        data.target.name = target.Name;
        data.target.energy = token.energy + damage;

        data.result.amount = damage;
        data.result.isSuccess = damage != 0;

        data.timestamp = Time.unscaledDeltaTime;

        SpellCastHandler.HandleEvent(data);

        UIWaitingCastResult.Instance.ShowResults(spell, data.result);

        yield return 0;
    }

    private IEnumerator TargetSpell(string[] parameters)
    {
        string spellId = parameters[0];
        string casterId = parameters[1];
        int damage = int.Parse(parameters[2]);

        IMarker caster = MarkerSpawner.GetMarker(casterId);
        CharacterToken token = caster.Token as CharacterToken;

        SpellCastHandler.SpellCastEventData data = new SpellCastHandler.SpellCastEventData();

        data.spell = spellId;

        data.target.id = PlayerDataManager.playerData.instance;
        data.target.name = PlayerDataManager.playerData.name;
        data.target.type = "character";

        data.caster.id = casterId;
        data.caster.name = caster.Name;
        data.caster.energy = token.energy - damage;

        data.result.amount = damage;
        data.result.isSuccess = damage != 0;

        data.timestamp = Time.unscaledDeltaTime;

        SpellCastHandler.HandleEvent(data);

        yield return 0;
    }

    private IEnumerator ShowSpellbook()
    {
        bool screenLoaded = false;

        if (UIPlayerInfo.IsShowing)
        {
            UISpellcastBook.Open(UIPlayerInfo.WitchMarkerDetails, UIPlayerInfo.WitchMarker, PlayerDataManager.playerData.UnlockedSpells, null, null, null, () => screenLoaded = true);
        }
        else if (UISpiritInfo.IsShowing)
        {
            UISpellcastBook.Open(UISpiritInfo.SpiritMarkerDetails, UISpiritInfo.SpiritMarker, PlayerDataManager.playerData.UnlockedSpells, null, null, null, () => screenLoaded = true);
        }
        else
        {
            Debug.LogError("cant open spellbook in ftf without selecting a witch or spirit first");
            screenLoaded = true;
        }

        while (screenLoaded == false)
            yield return 0;

        yield return 0;
    }

    private IEnumerator CloseSpellbook()
    {
        UISpellcastBook.Close();
        yield return 0;
    }

    private IEnumerator FocusSpellbook(string[] parameters)
    {
        string spellId = parameters[0];
        UISpellcastBook.FocusOn(spellId);
        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator CloseSpiritBanished()
    {
        UISpiritBanished.Instance.Close();
        yield return 0;
    }

    private IEnumerator MoveCamera(string[] parameters)
    {
        float longitude = PlayerDataManager.playerData.longitude + float.Parse(parameters[0]);
        float latitude = PlayerDataManager.playerData.latitude+ float.Parse(parameters[1]);
        float zoom = float.Parse(parameters[2]);

        Vector3 worldPosition = MapsAPI.Instance.GetWorldPosition(longitude, latitude);

        MapCameraUtils.FocusOnPosition(worldPosition, zoom, true);

        yield return new WaitForSeconds(1);
    }

    private IEnumerator CloseSpellResults()
    {
        UIWaitingCastResult.Instance.Close();
        yield return 0;
    }

    private IEnumerator ShowNearbyPops()
    {
        bool screenLoaded = false;
        UINearbyLocations.Open(() => screenLoaded = true);

        while (screenLoaded == false)
            yield return 0;
    }

    private IEnumerator CloseNearbyPops()
    {
        UINearbyLocations.Close();
        yield return 0;
    }

    private IEnumerator OpenStore()
    {
        bool loaded = false;
        ShopManager.OpenStore(() => loaded = true, false);

        while (loaded == false)
            yield return 0;
    }

    private IEnumerator CloseStore()
    {
        ShopManager.CloseStore();
        yield return 0;
    }

    private IEnumerator OpenIngredientStore()
    {
        ShopManager.ShowIngredients();
        yield return 0;
    }

    private IEnumerator StoreSelectIngredient(string[] parameters)
    {
        string id = parameters[0];
        ShopManager.SelectIngredient(id);
        yield return 0;
    }

    private IEnumerator ShowPurchaseSuccess(string[] parameters)
    {
        string id = parameters[0];
        ShopManager.ShowPurchaseSuccess(id);
        yield return 0;
    }

    private IEnumerator ClosePurchaseSuccess()
    {
        ShopManager.ClosePurchaseSuccess();
        yield return 0;
    }
       
    private IEnumerator CompleteFTF()
    {
        //show dominion and witch school prompt
        UIDominionSplash.Instance.Show(() =>
        {
            //show whitch school screen
            m_WitchSchool.gameObject.SetActive(true);
            ShowWitchSchool(true, null);
        });

        //remove all spawned markers
        List<string> ids = new List<string>(MarkerSpawner.Markers.Keys);
        foreach (string id in ids)
            RemoveTokenHandler.ForceEvent(id);

        //send finish ftf request
        FinishFTF((result, response) =>
        {
            if (result == 200)
            {
                MarkerManagerAPI.GetMarkers(PlayerDataManager.playerData.longitude, PlayerDataManager.playerData.latitude);
            }
            else
            {
                UIGlobalPopup.ShowError(() => Application.Quit(), APIManager.ParseError(response));
            }
        });

        yield return 0;
    }

    private IEnumerator SpawnFX(string[] parameters)
    {
        string id = parameters[0];
        float longitude = PlayerDataManager.playerData.longitude + float.Parse(parameters[1]);
        float latitude = PlayerDataManager.playerData.latitude + float.Parse(parameters[2]);

        m_FXSpawner.Spawn(id, longitude, latitude);
        yield return 0;
    }

    private IEnumerator OpenFakePoP()
    {
        string sceneName = "PlaceOfPowerFTF";
        bool done = false;

        LoadingOverlay.Show();
        AsyncOperation op = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Additive);
        op.completed += (a) => done = true;
        op.allowSceneActivation = true;

        MapsAPI.Instance.HideMap(true);
        //MapsAPI.Instance.ScaleBuildings(0);

        while (!done)
            yield return 0;
        
        LoadingOverlay.Hide();
    }

    private IEnumerator CloseFakePoP()
    {
        string sceneName = "PlaceOfPowerFTF";
        bool done = false;

        AsyncOperation op = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneName);
        op.completed += (a) => done = true;
        op.allowSceneActivation = true;

        MapsAPI.Instance.HideMap(false);
        //MapsAPI.Instance.ScaleBuildings(1);

        while (!done)
            yield return 0;
    }

    private IEnumerator FlyToNearbyPoP()
    {
        if (UINearbyLocations.CachedLocations.Count > 0)
        {
            double longitude = UINearbyLocations.CachedLocations[0].longitude - 0.0006;
            double latitude = UINearbyLocations.CachedLocations[0].latitude - 0.0003;
            bool done = false;
            MarkerManagerAPI.LoadMap(longitude, latitude, true, () => done = true);

            while (!done)
                yield return 0;
        }
    }

    private IEnumerator SelectNearbyPoP()
    {
        if (UINearbyLocations.CachedLocations.Count > 0)
        {
            bool done = false;
            LoadPOPManager.EnterPOP(UINearbyLocations.CachedLocations[0].id, () => done = true);

            while (!done)
                yield return 0;

            LocationPOPInfo.Instance.FTFOpen();
        }
        yield return 0;
    }

    private IEnumerator ClosePoPSelectedUI()
    {
        if (LocationPOPInfo.isShowing)
            LocationPOPInfo.Instance.Close();
        yield return 0;
    }

    #endregion

} 