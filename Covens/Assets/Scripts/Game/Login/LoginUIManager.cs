using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Raincrow;

public class LoginUIManager : MonoBehaviour
{
    private static LoginUIManager m_Instance;

    [SerializeField] private Button Helpcrow;
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
    [SerializeField] private Text m_LoginError;
    public InputField accountName;
    public InputField accountPassword;
    [SerializeField] private Button m_LoginBackButton;
    [SerializeField] private Button m_ForgotPassButton;
    [SerializeField] private Button loginButton;

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
    [SerializeField] private Button resetPasswordContinueButton;
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
    [SerializeField] private Button createAccountButton;
    [SerializeField] private Button m_CreatAccBackButton;

    [Header("Age gate")]
    public InputField dateYear;
    public InputField dateMonth;
    public InputField dateDay;
    [SerializeField] private CanvasGroup AgeGateCG;
    [SerializeField] private GameObject AgeGate;
    [SerializeField] private Button CheckAge;
    [SerializeField] private GameObject AgeError;


    [Header("Create character")]
    [SerializeField] private CanvasGroup createCharacter;
    public InputField createCharacterName;
    public Text createCharacterError;
    [SerializeField] private Button m_CreateCharConfirmButton;
    [SerializeField] private Button m_CreateCharBackButton;

    [Header("Choose character")]
    [SerializeField] private CanvasGroup CharSelectWindow;
    [SerializeField] private Button m_ChooseCharConfirmButton;
    [SerializeField] private Toggle[] toggles;


    public enum Screen
    {
        NONE = 0,
        WELCOME,
        SIGN_IN,
        MAIL_NOT_FOUND,
        RESET_A,
        RESET_B,
        CREATE_ACCOUNT,
        CREATE_CHARACTER,
        CHOOSE_CHARACTER,
    }

    private CanvasGroup[] m_Screens;
    private int m_AlphaTweenId;
    private Screen m_CurrentScreen;

    private void Awake()
    {
        m_Instance = this;

        Helpcrow.onClick.AddListener(SendEmail);
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

        AgeGateCG.alpha = 0;
        AgeGate.SetActive(false);
        //dateDay.text = "10";
        //dateYear.text = "1801";
        //dateMonth.text = "09";

        //setup buttons
        //main
        m_ExistingAccountBtn.onClick.AddListener(() => SetScreen(Screen.SIGN_IN));
        m_NewAccountBtn.onClick.AddListener(() => SetScreen(Screen.CREATE_ACCOUNT));

        //sign in
        m_LoginBackButton.onClick.AddListener(() => SetScreen(Screen.WELCOME));
        m_ForgotPassButton.onClick.AddListener(() => SetScreen(Screen.RESET_A));
        loginButton.onClick.AddListener(OnClickLogin);
        accountName.onEndEdit.AddListener(value => accountPassword.Select());
        //accountPassword.onEndEdit.AddListener(value => OnClickLogin());

        //age gate
        CheckAge.onClick.AddListener(() => AgeGateCheck());
        dateDay.onEndEdit.AddListener(value => dateMonth.Select());
        dateMonth.onEndEdit.AddListener(value => dateYear.Select());
        //dateYear.onEndEdit.AddListener(value => AgeGateCheck());

        //create acc
        m_CreatAccBackButton.onClick.AddListener(() => SetScreen(Screen.WELCOME));
        createAccountButton.onClick.AddListener(OnClickCreateAccount);
        createAccountName.onEndEdit.AddListener(value => createAccountEmail.Select());
        createAccountEmail.onEndEdit.AddListener(value => createAccountPassword.Select());
        //createAccountPassword.onEndEdit.AddListener(value => OnClickCreateAccount());

        //choose char
        m_ChooseCharConfirmButton.onClick.AddListener(OnConfirmCharacterBody);
        for (int i = 0; i < toggles.Length; i++)
        {
            Toggle toggle = toggles[i];
            int idx = i;
            toggle.onValueChanged.AddListener((on) => OnToggleBodyType(idx, on));
        }

        //create char
        m_CreateCharConfirmButton.onClick.AddListener(OnClickCreateCharacter);
        m_CreateCharBackButton.onClick.AddListener(() => SetScreen(Screen.CHOOSE_CHARACTER));
        //createCharacterName.onEndEdit.AddListener(value => OnClickCreateCharacter());

        //no mail found
        m_NoMainBackButton.onClick.AddListener(() => SetScreen(Screen.WELCOME));

        //reset pass A
        m_ResetInitBackButton.onClick.AddListener(() => SetScreen(Screen.WELCOME));
        resetPasswordContinueButton.onClick.AddListener(OnClickResetPassword);

        ////others
        m_LoginError.gameObject.SetActive(true);
        m_LoginError.text = "";
        createAccountError.gameObject.SetActive(true);
        createAccountError.text = "";
        createCharacterError.gameObject.SetActive(true);
        createCharacterError.text = "";


    }

    void SendEmail()
    {

        string email = "help@raincrowgames.com";
        string subject = MyEscapeURL("Covens Bug # New User");
        string body = MyEscapeURL($"***Enter Your Message Here*** \n\n\n Version: {Application.version} \n Platform: {Application.platform}");

        Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);

    }
    string MyEscapeURL(string url)
    {
        return WWW.EscapeURL(url).Replace("+", "%20");
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

        ShowLoading(false);
        LeanTween.cancel(m_AlphaTweenId);

        Screen previousScreen = m_CurrentScreen;
        m_CurrentScreen = screen;

        int idx = (int)screen;
        float start = m_Screens[idx].alpha;
        float end = 1f;

        m_AlphaTweenId = LeanTween.value(0, 1, 1f)
            .setOnStart(() =>
            {
                for (int i = 0; i < m_Screens.Length; i++)
                {
                    if (i == idx)
                    {
                        //show the gameobject and enable interaction
                        m_Screens[i].gameObject.SetActive(true);
                        m_Screens[i].interactable = true;
                        m_Screens[i].blocksRaycasts = true;
                    }
                    else
                    {
                        //disable interaction with the other uis
                        m_Screens[i].interactable = false;
                        m_Screens[i].blocksRaycasts = false;
                    }
                }
            })
            .setOnUpdate((float t) =>
            {
                //fade the uis
                float a = LeanTween.easeOutCubic(start, end, t);

                for (int i = 0; i < m_Screens.Length; i++)
                {
                    if (i == idx)
                        m_Screens[i].alpha = a;
                    else
                        m_Screens[i].alpha = 1 - a;
                }
            })
            .setOnComplete(() =>
            {
                //hide other gameobjects
                for (int i = 0; i < m_Screens.Length; i++)
                {
                    if (i != idx)
                    {
                        m_Screens[i].gameObject.SetActive(false);
                    }
                }
            })
            .uniqueId;

        switch (previousScreen)
        {

        }

        switch (screen)
        {
            case Screen.SIGN_IN:
                {
                    m_LoginError.text = "";
                    animSavannah.SetBool("Leave", true);
                    break;
                }
            case Screen.CREATE_ACCOUNT:
                {
                    AgeGateStart();
                    animSavannah.SetBool("Leave", true);
                    createAccountError.text = "";
                    break;
                }
            case Screen.CREATE_CHARACTER:
                {
                    createCharacterError.text = "";
                    break;
                }
            case Screen.CHOOSE_CHARACTER:
                {
                    foreach (Toggle toggle in toggles)
                        toggle.isOn = false;
                    m_ChooseCharConfirmButton.interactable = false;

                    if (previousScreen != Screen.CREATE_CHARACTER)
                        animSavannah.Play("out");

                    break;
                }
        }
    }

    public static void Open(Screen startScreen)
    {
        //load the login scene

        SceneManager.LoadSceneAsync(
            SceneManager.Scene.LOGIN,
            UnityEngine.SceneManagement.LoadSceneMode.Additive,
            (progress) => SplashManager.Instance.ShowLoading(progress),
            () =>
            {
                m_Instance.SetScreen(startScreen);
                BackButtonListener.AddCloseAction(m_Instance.OnPressReturn);
            });
    }

    public static void Close()
    {
        //unload the login scene

        if (m_Instance == null)
        {
            Debug.LogError("Login UI not open");
            return;
        }

        BackButtonListener.RemoveCloseAction();

        m_Instance.ShowLoading(false);
        m_Instance.mainCanvasGroup.interactable = false;
        m_Instance.animSavannah.Play("out");

        LeanTween.alphaCanvas(m_Instance.mainCanvasGroup, 0, 1.5f)
            .setEaseOutCubic()
            .setOnComplete(() => SceneManager.UnloadScene(SceneManager.Scene.LOGIN, null, null));
    }

    private void ShowLoading(bool loading)
    {
        m_Screens[(int)m_CurrentScreen].interactable = !loading;
        loadingObject.SetActive(loading);
    }

    private void OnClickLogin()
    {
        //check input fields
        //send login request to server

        m_LoginError.text = "";

        string username = accountName.text;
        string password = accountPassword.text;

        string accountError = LoginUtilities.ValidateUsername(username);

        if (string.IsNullOrEmpty(accountError) == false)
        {
            m_LoginError.text = accountError;
            return;
        }

        accountError = LoginUtilities.ValidatePassword(password);
        if (string.IsNullOrEmpty(accountError) == false)
        {
            m_LoginError.text = accountError;
            return;
        }

        ShowLoading(true);
        LoginAPIManager.Login(username, password, (result, response) =>
        {
            if (result == 200)
            {
                if (response.hasCharacter.HasValue)
                {
                    if (response.hasCharacter.Value)
                    {
                        LoginAPIManager.GetCharacter((charResult, charResponse) =>
                        {
                            if (LoginAPIManager.characterLoggedIn)
                                Close();
                            else
                            {
                                UIGlobalPopup.ShowError(() => SetScreen(Screen.WELCOME), APIManager.ParseError("error_" + charResponse));
                            }
                        });
                    }
                    else
                    {
                        SetScreen(Screen.CHOOSE_CHARACTER);
                    }
                }
                else
                {
                    SetScreen(Screen.WELCOME);
                }
            }
            else
            {
                ShowLoading(false);
                m_LoginError.text = APIManager.ParseError(response.error);
            }
        });
    }

    private void OnClickCreateAccount()
    {

        //check fields
        //send create account request

        createAccountError.text = "";

        string username = createAccountName.text;
        string password = createAccountPassword.text;
        string email = createAccountEmail.text;

        string accountError = LoginUtilities.ValidateUsername(username);

        if (string.IsNullOrEmpty(accountError) == false)
        {
            createAccountError.text = accountError;
            return;
        }

        accountError = LoginUtilities.ValidateEmail(email);
        if (string.IsNullOrEmpty(accountError) == false)
        {
            createAccountError.text = accountError;
            return;
        }

        accountError = LoginUtilities.ValidatePassword(password);
        if (string.IsNullOrEmpty(accountError) == false)
        {
            createAccountError.text = accountError;
            return;
        }

        ShowLoading(true);
        LoginAPIManager.CreateAccount(username, password, email, (result, response) =>
        {
            if (result == 200)
            {
                //go to chracter creation
                SetScreen(Screen.CHOOSE_CHARACTER);
            }
            else
            {
                //show error
                createAccountError.text = APIManager.ParseError(response.error);
                ShowLoading(false);
            }
        });
    }

    private int m_AgeGateTweenId;
    private bool m_AgeChecked = false;

    private void AgeGateStart()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
            return;

        if (m_AgeChecked)
            return;

        //Debug.Log("ageGateStart");
        AgeGate.SetActive(true);

        LeanTween.cancel(m_AgeGateTweenId);
        m_AgeGateTweenId = LeanTween.alphaCanvas(AgeGateCG, 1f, 0.7f).setEase(LeanTweenType.easeInCubic).uniqueId;

        BackButtonListener.AddCloseAction(() =>
        {
            CloseAgeGate();
            SetScreen(Screen.WELCOME);
        });
    }

    private void CloseAgeGate()
    {
        LeanTween.cancel(m_AgeGateTweenId);
        CheckAge.interactable = false;
        m_AgeGateTweenId = LeanTween.alphaCanvas(AgeGateCG, 0f, 0.5f).setEase(LeanTweenType.easeInCubic).setOnComplete(() =>
        {
            AgeGate.SetActive(false);
            CheckAge.interactable = true;
            AgeError.SetActive(false);
        }).uniqueId;
        BackButtonListener.RemoveCloseAction();
    }

    private void AgeGateCheck()
    {
        var dYear = int.Parse(dateYear.text);
        var dMonth = int.Parse(dateMonth.text);
        var dDay = int.Parse(dateDay.text);
        var today = System.DateTime.Today;
        var dob = (dYear * 10000) + (dMonth * 100) + dDay;
        var todayInt = (today.Year * 10000) + (today.Month * 100) + today.Day;
        var Age = (todayInt - dob) / 10000;
        //Debug.Log(Age);

        if (Age < 13)
        {
            //Debug.Log("too young");
            AgeError.SetActive(true);
        }
        else
        {
            m_AgeChecked = true;
            CloseAgeGate();
        }
    }
    private void OnConfirmCharacterBody()
    {
        SetScreen(Screen.CREATE_CHARACTER);
    }

    private void OnClickCreateCharacter()
    {
        //validate the character name
        //send the request to server

        createCharacterError.text = "";

        string characterName = createCharacterName.text;
        string nameError = LoginUtilities.ValidateCharacterName(characterName);
        if (string.IsNullOrEmpty(nameError) == false)
        {
            createCharacterError.text = nameError;
            return;
        }

        int bodyType = -1;
        bool male = false;
        for (int i = 0; i < toggles.Length; i++)
        {
            if (toggles[i].isOn)
            {
                bodyType = i;
                male = toggles[i].transform.name.Contains("female") == false;
                break;
            }
        }

        ShowLoading(true);
        LoginAPIManager.CreateCharacter(characterName, bodyType, male, (result, response) =>
        {
            if (result == 200)
            {
                Close();
            }
            else
            {
                createCharacterError.text = APIManager.ParseError(response.error);
                ShowLoading(false);

                if (response.error == "1006")
                {
                    UIGlobalPopup.ShowError(() => SetScreen(Screen.WELCOME), APIManager.ParseError(response.error));
                }
            }
        });
    }

    private void OnToggleBodyType(int idx, bool on)
    {
        m_ChooseCharConfirmButton.interactable = on;
    }

    private void OnClickResetPassword()
    {
        UIGlobalPopup.ShowError(null, "NOT IMPLEMENTED");
    }

    public void OnClickRaincrowToS()
    {
        Application.OpenURL(CovenConstants.TERMS_OF_SERVICE_URL);
    }

    public void OnClickRaincrowPP()
    {
        Application.OpenURL(CovenConstants.PRIVACY_POLICY_URL);
    }

    public void OnClickGoogleToS()
    {
        Application.OpenURL(CovenConstants.GOOGLE_TERMS_OF_SERVICE_URL);
    }

    public void OnClickGooglePP()
    {
        Application.OpenURL(CovenConstants.GOOGLE_PRIVACY_POLICY_URL);
    }

    private void OnPressReturn()
    {
        switch (m_CurrentScreen)
        {
            case Screen.WELCOME:
                UIGlobalPopup.ShowPopUp(Application.Quit, () => { }, LocalizeLookUp.GetText("close_app_prompt"));
                break;
            case Screen.SIGN_IN:
            case Screen.CREATE_ACCOUNT:
                SetScreen(Screen.WELCOME);
                break;
            case Screen.CREATE_CHARACTER:
                SetScreen(Screen.CHOOSE_CHARACTER);
                break;
        }
    }
}