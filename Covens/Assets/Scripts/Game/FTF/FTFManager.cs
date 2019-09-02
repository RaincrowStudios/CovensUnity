using System.Collections;
using UnityEngine;
using Newtonsoft.Json;
using Raincrow;
using System.Collections.Generic;
using Raincrow.FTF;
using UnityEngine.UI;
using Raincrow.GameEventResponses;
using Raincrow.Maps;

public class FTFManager : MonoBehaviour
{
    [SerializeField] private TextAsset m_TutorialJson;
    [SerializeField] private FTFHighlight m_Highlight;
    [SerializeField] private FTFButtonArea m_Button;
    [SerializeField] private FTFMessageBox m_Message;
    [SerializeField] private FTFPointerHand m_Pointer;

    [Header("Savannah")]
    [SerializeField] private Animator m_Savannah;
    [SerializeField] private CanvasGroup m_SavannahCanvasGroup;

    [Header("Debug")]
    [SerializeField] private int m_StartFrom = 0;

    private static FTFManager m_Instance;

    public static bool InFTF => PlayerDataManager.IsFTF;
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
    private int m_TimerTweenId;

    private void Awake()
    {
        m_Instance = this;

        if (UnityEngine.EventSystems.EventSystem.current == null)
        {
            new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem), typeof(UnityEngine.EventSystems.StandaloneInputModule));
        }

        m_Message.OnClick += () => StartCoroutine(NextStep(null));
        m_Button.OnClick += () => StartCoroutine(NextStep(null));

        m_Savannah.gameObject.SetActive(false);
        m_SavannahCanvasGroup.alpha = 0;
    }

    private void OnDestroy()
    {
        m_Message.OnClick -= () => StartCoroutine(NextStep(null));
        m_Button.OnClick -= () => StartCoroutine(NextStep(null));
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
            StartCoroutine(SkipUntil(m_StartFrom));
            return;
#endif
            Setup(0);
        });
    }

    private IEnumerator SkipUntil(int index)
    {
        while (m_CurrentStepIndex != m_StartFrom)
        {
            yield return NextStep(new string[0]);
            yield return 0;
            yield return new WaitForSeconds(0.25f);
        }
    }

    private void Setup(int index)
    {
        Log("step " + index);

        //cancel previous running timer
        LeanTween.cancel(m_TimerTweenId);

        //execute previous state's exitActions
        if (m_CurrentStepIndex >= 0)
        {
            FTFStepData previousStep = m_Steps[m_CurrentStepIndex];

            m_Button.Hide();
            m_Message.Hide();
            m_Pointer.Hide();

            if (previousStep.onExit != null)
            {
                foreach (FTFActionData _action in previousStep.onExit)
                {
                    if (string.IsNullOrEmpty(_action.method))
                        return;

                    if (_action.parameters == null)
                        StartCoroutine(_action.method, new string[0]);
                    else
                        StartCoroutine(_action.method, _action.parameters.ToArray());
                }
            }
        }

        //set current
        m_PreviousStepIndex = m_CurrentStepIndex;
        m_CurrentStepIndex = index;

        if (m_CurrentStepIndex >= m_Steps.Count)
        {
            Log("No more steps");
            return;
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
            m_Message.EnableButton(false);
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
            foreach (FTFActionData _action in step.onEnter)
            {
                if (string.IsNullOrEmpty(_action.method))
                    return;

                if (_action.parameters == null)
                    StartCoroutine(_action.method, new string[0]);
                else
                    StartCoroutine(_action.method, _action.parameters.ToArray());
            }
        }

        //setup timer
        if (step.timer > 0)
        {
            m_TimerTweenId = LeanTween.value(0, 0, 0)
                .setDelay(step.timer)
                .setOnStart(() =>
                {
                    StartCoroutine(NextStep(null));
                })
                .uniqueId;
        }
    }

    private void LoadJson(System.Action<string> onComplete)
    {
        onComplete?.Invoke(m_TutorialJson.text);
    }

    #region AVAILABLE ACTIONS
    
    private IEnumerator GoToStep(string[] parameters)
    {
        if (parameters.Length < 1)
            throw new System.Exception("[GoTo] missing param[0] (step index)");
        
        int stepIndex = int.Parse(parameters[0]);
        yield return 0;
    }

    private IEnumerator NextStep(string[] parameters)
    {
        Setup(m_CurrentStepIndex + 1);
        yield return 0;
    }

    private IEnumerator ShowSavannah(string[] parameters)
    {
        m_Savannah.gameObject.SetActive(true);
        LeanTween.alphaCanvas(m_SavannahCanvasGroup, 1f, 1f).setEaseOutCubic();
        m_Savannah.Play("SavannaEnter");

        yield return 0;
    }

    private IEnumerator HideSavannah(string[] parameters)
    {
        LeanTween.alphaCanvas(m_SavannahCanvasGroup, 0f, 0.25f).setEaseOutCubic().setOnComplete(() =>
        {
            m_Savannah.gameObject.SetActive(false);
        });
        m_Savannah.Play("out", 0, 0.3f);

        yield return 0;
    }

    private IEnumerator ShowMessage(string[] parameters)
    {
        if (parameters.Length < 1)
            throw new System.Exception("[ShowMessage] missing param[0] (message)");
        
        string message = parameters[0];

        m_Message.Show(LocalizeLookUp.GetText(message));
        m_Message.EnableButton(!m_Button.IsShowing);

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

        if (marker.Type == MarkerManager.MarkerType.SPIRIT)
        {
            SelectSpiritData_Map details = JsonConvert.DeserializeObject<SelectSpiritData_Map>(json);
            details.token = marker.Token as SpiritToken;

            UISpiritInfo.Show(marker as SpiritMarker, details.token);
            UIQuickCast.Open();
            yield return new WaitForSeconds(0.5f);
            UISpiritInfo.SetupDetails(details);
            UIQuickCast.UpdateCanCast(marker, details);
        }
        else if (marker.Type == MarkerManager.MarkerType.WITCH)
        {
            SelectWitchData_Map details = JsonConvert.DeserializeObject<SelectWitchData_Map>(json);
            details.token = marker.Token as WitchToken;

            UIPlayerInfo.Show(marker as WitchMarker, details.token);
            UIQuickCast.Open();
            yield return new WaitForSeconds(0.5f);
            UIPlayerInfo.SetupDetails(details);
            UIQuickCast.UpdateCanCast(marker, details);
        }
        else if (marker.Type == MarkerManager.MarkerType.PLACE_OF_POWER)
        {
            
        }

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
        yield return new WaitForSecondsRealtime(1.5f);

        SpellCastHandler.SpellCastEventData data = new SpellCastHandler.SpellCastEventData();

        data.spell = spellId;

        data.caster.id = PlayerDataManager.playerData.instance;
        data.caster.name = PlayerDataManager.playerData.name;
        data.caster.type = "character";
        data.caster.energy = PlayerDataManager.playerData.energy - spell.cost;

        data.target.id = targetId;
        data.target.name = target.Name;
        data.target.energy = token.energy - damage;

        data.result.damage = damage;
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

        data.result.damage = damage;
        data.result.isSuccess = damage != 0;

        data.timestamp = Time.unscaledDeltaTime;

        SpellCastHandler.HandleEvent(data);

        yield return 0;
    }

    private IEnumerator ShowSpellbook(string[] parameters)
    {
        if (UIPlayerInfo.isShowing)
            UISpellcastBook.Open(UIPlayerInfo.WitchMarkerDetails, UIPlayerInfo.WitchMarker, PlayerDataManager.playerData.UnlockedSpells, null, null, null);

        if (UISpiritInfo.isOpen)
            UISpellcastBook.Open(UISpiritInfo.SpiritMarkerDetails, UISpiritInfo.SpiritMarker, PlayerDataManager.playerData.UnlockedSpells, null, null, null);

        yield return 0;
    }

    private IEnumerator CloseSpellbook(string[] parameters)
    {
        UISpellcastBook.Close();
        yield return 0;
    }

    private IEnumerator FocusSpellbook(string[] parameters)
    {
        string spellId = parameters[0];
        UISpellcastBook.FocusOn(spellId);
        yield return 0;
    }

    private IEnumerator CloseSpiritBanished(string[] parameters)
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

        yield return 0;
    }

    private IEnumerator CloseSpellResults(string[] parameters)
    {
        UIWaitingCastResult.Instance.Close();
        yield return 0;
    }

    #endregion

    public void Log(string message)
    {
        Debug.Log("[<color=cyan>FTFManager</color>] " + message);
    }
}