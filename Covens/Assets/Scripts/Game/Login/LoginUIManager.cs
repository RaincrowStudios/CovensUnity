using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Raincrow;

public class LoginUIManager : MonoBehaviour
{
    private static LoginUIManager m_Instance;

    [Header("General")]
    [SerializeField] private CanvasGroup mainCanvasGroup;
    public GameObject loadingObject;
    public Animator animSavannah;
    public List<CanvasGroup> bgFadeoutElements = new List<CanvasGroup>();
    public GameObject clickBlocker;
    public float fadeOutSpeed = 1;

    [Header("Main screen")]
    [SerializeField] private CanvasGroup chooseLoginTypeObject;
    [SerializeField] private Button m_ExistingAccountBtn;
    [SerializeField] private Button m_NewAccountBtn;

    [Header("Login screen")]
    [SerializeField] private CanvasGroup signInObject;
    public GameObject passwordError;
    public InputField accountName;
    public InputField accountPassword;
    public Button loginButton;
    [SerializeField] private Button m_LoginBackButton;

    [Header("Email not found")]
    [SerializeField] private CanvasGroup emailNullObject;
    [SerializeField] private Button m_NoMainBackButton;

    [Header("Reset pass A")]
    [SerializeField] private CanvasGroup resetPasswordStartObject;
    public Text emailResetInfo;
    public InputField resetCodeInput;
    public InputField resetAccountName;
    public GameObject resetUserNullError;
    public GameObject resetCodeWrongError;
    public GameObject userResetObject;
    public GameObject codeResetObject;
    public Button resetPasswordContinueButton;
    [SerializeField] private Button m_ResetInitBackButton;

    [Header("Reset pass B")]
    [SerializeField] private CanvasGroup resetPasswordEndObject;
    public Text passwordResetInfo;
    public InputField resetpass1;
    public InputField resetpass2;
    public GameObject resetPassContinueButton;
    [SerializeField] private Button m_ResetPassBackButton;

    [Header("Create account")]
    [SerializeField] private CanvasGroup createAccount;
    public InputField createAccountName;
    public InputField createAccountEmail;
    public InputField createAccountPassword;
    public Text createAccountError;
    public Button createCharButton;
    public Button createAccountButton;
    [SerializeField] private Button m_CreatAccBackButton;

    [Header("Create character")]
    [SerializeField] private CanvasGroup createCharacter;
    public InputField createCharacterName;
    public Text createCharacterError;
    [SerializeField] private Button m_CreateCharConfirmButton;

    [Header("Choose character")]
    [SerializeField] private CanvasGroup CharSelectWindow;
    [SerializeField] private Button m_CharacterConfirmButton;
    public Toggle[] toggles;


    public enum Screen
    {
        NONE = 0,
        WELCOME,
        SIGN_IN,
        MAIL_NOT_FOUND,
        RESET_A,
        RESET_B,
        CREATE_ACC,
        CREATE_CHAR,
        CHOOSE_CHAR,
    }
         
    public static bool isInFTF;
    private HashSet<char> NameCheck = new HashSet<char>() { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'd', 'b', 'c', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };

    private CanvasGroup[] m_Screens;
    private int m_AlphaTweenId;
    private Screen m_CurrentScreen;

    private void Awake()
    {
        m_Instance = this;

        CanvasGroup emptyCg = new GameObject().AddComponent<CanvasGroup>();
        emptyCg.transform.SetParent(this.transform);
        emptyCg.alpha = 1;

        m_Screens = new CanvasGroup[] {
            emptyCg,
            chooseLoginTypeObject,
            signInObject,
            emailNullObject,
            resetPasswordStartObject,
            resetPasswordEndObject,
            createAccount,
            createCharacter,
            CharSelectWindow
        };

        //set initial alpha for all screens
        mainCanvasGroup.alpha = 0;

        chooseLoginTypeObject.alpha = 0;
        signInObject.alpha = 0;
        emailNullObject.alpha = 0;
        resetPasswordStartObject.alpha = 0;
        resetPasswordEndObject.alpha = 0;
        createAccount.alpha = 0;
        createCharacter.alpha = 0;
        CharSelectWindow.alpha = 0;
        
        chooseLoginTypeObject.gameObject.SetActive(false);
        signInObject.gameObject.SetActive(false);
        emailNullObject.gameObject.SetActive(false);
        resetPasswordStartObject.gameObject.SetActive(false);
        resetPasswordEndObject.gameObject.SetActive(false);
        createAccount.gameObject.SetActive(false);
        createCharacter.gameObject.SetActive(false);
        CharSelectWindow.gameObject.SetActive(false);

        m_CurrentScreen = Screen.NONE;
    }

    private void Start()
    {
        //tween the main alpha
        mainCanvasGroup.interactable = true;
        LeanTween.alphaCanvas(mainCanvasGroup, 1f, 1f).setEaseOutCubic();
    }

    public void SetScreen(Screen screen)
    {
        if (screen == m_CurrentScreen)
            return;

        m_CurrentScreen = screen;

        LeanTween.cancel(m_AlphaTweenId);

        int idx = (int)screen;
        float start = m_Screens[idx].alpha;
        float end = 1f;

        float startOther = 1 - start;
        float endOther = 1 - end;

        m_AlphaTweenId = LeanTween.value(0, 1, 0.5f)
            .setOnStart(() =>
            {
                for (int i = 0; i < m_Screens.Length; i++)
                {
                    if (i == idx)
                    {
                        //show the gameobject and enable interaction
                        m_Screens[i].gameObject.SetActive(true);
                        m_Screens[i].interactable = true;
                    }
                    else
                    {
                        //disable interaction with the other uis
                        m_Screens[i].interactable = false;
                    }
                }
            })
            .setOnUpdate((float t) =>
            {
                //fade the uis
                for (int i = 0; i < m_Screens.Length; i++)
                {
                    if (i == idx)
                        m_Screens[i].alpha = LeanTween.easeOutCubic(start, end, t);
                    else
                        m_Screens[i].alpha = LeanTween.easeInCubic(startOther, endOther, t);
                }
            })
            .setOnComplete(() =>
            {
                //hide other gameobjects
                for (int i = 0; i < m_Screens.Length; i++)
                {
                    if (i != idx)
                        m_Screens[i].gameObject.SetActive(false);
                }
            })
            .uniqueId;
    }

    public static void Open(Screen startScreen)
    {
        SceneManager.LoadSceneAsync(
            SceneManager.Scene.LOGIN,
            UnityEngine.SceneManagement.LoadSceneMode.Additive, 
            (progress) => SplashManager.Instance.ShowLoading(progress), 
            () => m_Instance.SetScreen(startScreen));
    }

    public static void Close()
    {
        if (m_Instance == null)
        {
            Debug.LogError("Login UI not open");
            return;
        }

        m_Instance.mainCanvasGroup.interactable = false;
        LeanTween.alphaCanvas(m_Instance.mainCanvasGroup, 0, 1f)
            .setEaseOutCubic()
            .setOnComplete(() => SceneManager.UnloadScene(SceneManager.Scene.LOGIN, null, null));
    }

#if UNITY_EDITOR
    [ContextMenu("Next screen")]
    private void NextScreen()
    {
        Screen next = (Screen)(((int)m_CurrentScreen + 1) % m_Screens.Length);
        SetScreen(next);
    }
#endif
}