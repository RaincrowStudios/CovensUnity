using System.Collections;
using UnityEngine;
using Newtonsoft.Json;
using Raincrow;
using System.Collections.Generic;
using Raincrow.FTF;
using UnityEngine.UI;

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

    private static FTFManager m_Instance;
    public static bool InFTF => PlayerDataManager.IsFTF;

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

    private void Start()
    {
        _StartFTF();
    }

    private void OnDestroy()
    {
        m_Message.OnClick -= () => StartCoroutine(NextStep(null));
        m_Button.OnClick -= () => StartCoroutine(NextStep(null));
    }
    
    private void _StartFTF()
    {
        //load json
        //start first step

        m_CurrentStepIndex = 0;
        m_PreviousStepIndex = -1;

        LoadJson(json =>
        {
            m_Steps = JsonConvert.DeserializeObject<List<FTFStepData>>(json);
            Setup(0);
        });
    }

    private void Setup(int index)
    {
        //execute previous state's exitActions
        if (m_CurrentStepIndex >= 0 && m_Steps[m_CurrentStepIndex].onExit != null)
        {
            List<FTFActionData> actions = m_Steps[m_CurrentStepIndex].onExit;
            foreach (FTFActionData _action in actions)
            {
                if (string.IsNullOrEmpty(_action.method))
                    return;

                StartCoroutine(_action.method, _action.parameters.ToArray());
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

        //execute new state's enterActions
        if (step.onEnter != null)
        {
            foreach (FTFActionData _action in step.onEnter)
            {
                if (string.IsNullOrEmpty(_action.method))
                    return;

                StartCoroutine(_action.method, _action.parameters.ToArray());
            }
        }

        //setup timer
        if (step.timer > 0)
            LeanTween.value(0, 0, 0).setDelay(step.timer).setOnStart(() => StartCoroutine(NextStep(null)));
    }

    private void LoadJson(System.Action<string> onComplete)
    {
        onComplete?.Invoke(m_TutorialJson.text);
    }

    #region AVAILABLE ACTIONS

    private IEnumerator SpawnMarker(object[] parameters)
    {
        //[0]: token type <string>
        //[1]: token json <string>

        yield return 0;
    }

    private IEnumerator GoToStep(object[] parameters)
    {
        if (parameters.Length < 1)
            throw new System.Exception("[GoTo] missing param[0] (step index)");

        if (parameters[0] is int == false)
            throw new System.ArgumentException("[GoTo] invalid param[0] type \"" + parameters[0].GetType());

        int stepIndex = (int)parameters[0];
        yield return 0;
    }

    private IEnumerator NextStep(object[] parameters)
    {
        Setup(m_CurrentStepIndex + 1);
        yield return 0;
    }

    private IEnumerator ShowSavannah(object[] parameters)
    {
        m_Savannah.gameObject.SetActive(true);
        LeanTween.alphaCanvas(m_SavannahCanvasGroup, 1f, 1f).setEaseOutCubic();
        m_Savannah.Play("SavannaEnter");

        yield return 0;
    }

    private IEnumerator HideSavannah(object[] parameters)
    {
        LeanTween.alphaCanvas(m_SavannahCanvasGroup, 0f, 0.2f).setOnComplete(() =>
        {
            m_Savannah.gameObject.SetActive(false);
        });
        m_Savannah.Play("out");

        yield return 0;
    }

    private IEnumerator ShowMessage(object[] parameters)
    {
        if (parameters.Length < 1)
            throw new System.Exception("[ShowMessage] missing param[0] (message)");

        if (parameters[0] is string == false)
            throw new System.ArgumentException("[ShowMessage] invalid param[0] type \"" + parameters[0].GetType());

        string message = (string)parameters[0];

        m_Message.Show(message);

        yield return 0;
    }

    #endregion

    public void Log(string message)
    {
        Debug.Log("[<color=cyan>FTFManager</color>] " + message);
    }

#if UNITY_EDITOR
    [ContextMenu("Test")]
    private void TestCoroutine()
    {
        StartCoroutine("DebugCoroutine", new object[] { 1, "test" });
    }

    private IEnumerator DebugCoroutine(params object[] parameters)
    {
        Debug.Log("len: " + parameters.Length);
        for (int i = 0; i < parameters.Length; i++)
            Debug.Log(parameters[i]);
        yield return 0;
    }

    [ContextMenu("Test parse")]
    private void TestParse()
    {
        string json = "{\"parameters\":[3, \"outro test\"]}";
        var action = JsonConvert.DeserializeObject<FTFActionData>(json);
        //var parameters = action.parameters;
        //Debug.Log("len: " + parameters.Length);
        //for (int i = 0; i < parameters.Length; i++)
        //    Debug.Log(parameters[i]);
        StartCoroutine("DebugCoroutine", action.parameters.ToArray());
    }
#endif
}