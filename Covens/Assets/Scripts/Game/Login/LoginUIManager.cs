using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
[RequireComponent(typeof(APIManager))]

public class LoginUIManager : MonoBehaviour
{

    [SerializeField] private CanvasGroup mainCanvasGroup;

    public static LoginUIManager Instance { get; set; }
    public string testUser;

    public GameObject loginObject;
    public GameObject chooseLoginTypeObject;

    public Text passwordResetInfo;
    public Text emailResetInfo;

    public GameObject signInObject;
    public GameObject passwordError;

    public InputField accountName;
    public InputField accountPassword;
    public InputField resetCodeInput;
    public InputField resetAccountName;

    public InputField resetpass1;
    public InputField resetpass2;

    public GameObject loadingObject;
    public static bool isInFTF;
    public GameObject resetUserNullError;
    public GameObject resetCodeWrongError;
    public GameObject emailNullObject;
    public GameObject resetPasswordStartObject;
    public GameObject userResetObject;
    public GameObject codeResetObject;
    public GameObject resetPasswordEndObject;
    public GameObject resetPassContinueButton;
    public GameObject resetPassbackButton;
    public Button resetPasswordContinueButton;

    public GameObject mainUI;

    public GameObject createAccount;
    public InputField createAccountName;
    public InputField createAccountEmail;
    public InputField createAccountPassword;
    public Text createAccountError;

    public GameObject createCharacter;
    public InputField createCharacterName;
    //	public Toggle male;
    //	public Toggle female;
    public Text createCharacterError;

    public Button createCharButton;
    public Button createAccountButton;
    public Button loginButton;
    //	public Button createCharButton;
    //	public static bool playerGender;
    public static string charUserName;
    HashSet<char> NameCheck = new HashSet<char>() { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'd', 'b', 'c', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };
    // Use this for initialization

    public Toggle[] toggles;
    public Animator animSavannah;
    string currentCharacter;
    public CanvasGroup charSelectFinal;
    public GameObject CharSelectWindow;
    public List<CanvasGroup> bgFadeoutElements = new List<CanvasGroup>();
    public float fadeOutSpeed = 1;
    public GameObject FTFobject;
    public CanvasGroup playerFocus;

    public GameObject clickBlocker;
    public GameObject witchSchool;


    bool skipFTF = false;
    #region player prefs


    #endregion


    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        LoginAPIManager.sceneLoaded = true;

        /*
        createAccountName.Select();
        createAccountName.text = Random.Range(1000, 9999999).ToString();

        createAccountPassword.Select();
        createAccountPassword.text = "1234";

        createCharacterName.Select();
        createCharacterName.text = createAccountName.text;

        accountName.Select();
        accountName.text = "123456";

        accountPassword.Select();
        accountPassword.text = "123456";
        */

        Debug.Log("has character" + LoginAPIManager.hasCharacter);

        if (!LoginAPIManager.loggedIn)
        {
            MapsAPI.Instance.HideMap();
            initiateLogin();
        }
        else
        {
            if (!LoginAPIManager.hasCharacter)
            {
                initiateLogin();
                chooseLoginTypeObject.SetActive(false);
                createCharacter.SetActive(true);

            }
            else
            {
                LoginAPIManager.InitiliazingPostLogin();
                if (PlayerDataManager.playerData.energy == 0)
                {
                    DeathState.Instance.ShowDeath();
                }
                // Invoke("enableSockets", 2f);
                enableSockets();
            }
        }
    }

    void enableSockets()
    {
        WebSocketClient.websocketReady = true;

    }

    public void initiateLogin()
    {
        createCharacter.SetActive(false);
        CharSelectWindow.SetActive(false);
        signInObject.SetActive(false);
        loadingObject.SetActive(false);
        Debug.Log("Initializing Login");
        mainUI.SetActive(false);
        loginObject.SetActive(true);
        chooseLoginTypeObject.SetActive(true);
    }

    public void AlreadyLoggedIn()
    {
        SoundManagerOneShot.Instance.PlayButtonTap();
        loadingObject.SetActive(false);
        chooseLoginTypeObject.SetActive(false);
        signInObject.SetActive(true);
        accountName.text = LoginAPIManager.StoredUserName;
        accountPassword.text = LoginAPIManager.StoredUserPassword;
        loginButton.interactable = true;
    }


    public void doLogin()
    {
        SoundManagerOneShot.Instance.PlayLoginButton();
        loadingObject.SetActive(true);
        LoginAPIManager.isNewAccount = false;
        LoginAPIManager.StoredUserName = accountName.text;
        LoginAPIManager.StoredUserPassword = accountPassword.text;
        LoginAPIManager.Login(accountName.text, accountPassword.text);
        loginButton.interactable = false;
    }

    public void InitiateCreateAccount()
    {
        SoundManagerOneShot.Instance.PlayLoginButton();
        chooseLoginTypeObject.SetActive(false);
        createAccount.SetActive(true);
    }

    public void CreateAccount()
    {
        SoundManagerOneShot.Instance.PlayLoginButton();

        createCharacterError.gameObject.SetActive(false);

        if (createAccountName.text.Length < 4)
        {
            Debug.Log("less char");
            createAccountError.gameObject.SetActive(true);
            createAccountError.text = "Account name should have at least 4 letters";
            return;
        }

        foreach (var item in createAccountName.text)
        {
            if (!NameCheck.Contains(item))
            {
                Debug.Log("fail char");

                createAccountError.gameObject.SetActive(true);
                createAccountError.text = "Account name cannot contain special characters";
                return;
            }
        }

        if (createAccountPassword.text.Length < 4)
        {
            createAccountError.gameObject.SetActive(true);
            createAccountError.text = "Password should have at least 4 letters.";
            return;
        }
        createAccountButton.interactable = false;
        LoginAPIManager.CreateAccount(createAccountName.text, createAccountPassword.text, createAccountEmail.text);
        loadingObject.SetActive(true);
    }

    public void CreateAccountResponse(bool success, string error)
    {
        loadingObject.SetActive(false);
        if (!success)
        {
            createAccountError.gameObject.SetActive(true);
            createAccountError.text = error;
            createAccountButton.interactable = true;
            return;
        }
        else
        {
            createAccount.SetActive(false);
            LoginAPIManager.StoredUserName = createAccountName.text;
            LoginAPIManager.StoredUserPassword = createAccountPassword.text;
            createCharacter.SetActive(true);

        }
    }

    public void CreateCharacter()
    {
        SoundManagerOneShot.Instance.PlayLoginButton();

        createCharacterError.gameObject.SetActive(false);
        if (createCharacterName.text.Length < 4)
        {
            createCharacterError.gameObject.SetActive(true);
            createCharacterError.text = "Character name should have at least 4 letters.";
            return;
        }

        foreach (var item in createCharacterName.text)
        {
            if (!NameCheck.Contains(item))
            {
                if (item == ' ')
                {
                    continue;
                }
                createCharacterError.gameObject.SetActive(true);
                createCharacterError.text = "character name cannot contain special characters";
                return;
            }
        }

        var checkName = new { displayName = createCharacterName.text };
        APIManager.Instance.Post("check-name", JsonConvert.SerializeObject(checkName), CreateCharacterError, true, false);

        createCharButton.interactable = false;

        loadingObject.SetActive(true);
    }

    public void CreateCharacterError(string s, int r)
    {
        Debug.Log(s);
        if (r == 200)
        {
            charUserName = createCharacterName.text;
            //			charSelect.StartAnimation ();
            createCharacter.SetActive(false);
            CharSelectWindow.SetActive(true);
            loadingObject.SetActive(false);
            charSelectFinal.interactable = false;
            animSavannah.Play("out");
        }
        else
        {
            if (s == "4103")
            {
                createCharacterError.gameObject.SetActive(true);
                createCharacterError.text = "Character name is taken";
                createCharButton.interactable = true;
            }
            else if (s == "4104")
            {
                createCharacterError.gameObject.SetActive(true);
                createCharacterError.text = "Character name is invalid";
                createCharButton.interactable = true;
            }
            else if (s == "4105")
            {
                createCharacterError.gameObject.SetActive(true);
                createCharacterError.text = "Character name is Empty";
                createCharButton.interactable = true;
            }
            else
            {
                createCharacterError.gameObject.SetActive(true);
                createCharacterError.text = "Could not create character . . .";
                createCharButton.interactable = true;
            }
        }
    }


    #region password
    public void CorrectPassword()
    {
        MapsAPI.Instance.InitMap();
        SoundManagerOneShot.Instance.PlayLoginButton();
        MapsAPI.Instance.position = MapsAPI.Instance.physicalPosition;

        if (!LoginAPIManager.isNewAccount)
        {
            if (skipFTF)
            {
                Debug.Log("skiping ftf on login");
                PlayerManager.Instance.CreatePlayerStart();

                LoginAPIManager.FTFComplete = true;
                LoginUIManager.isInFTF = false;
                FTFobject.SetActive(false);
                MarkerManagerAPI.GetMarkers(true);
                APIManager.Instance.GetData("ftf/complete", (string s, int r) =>
                {
                    APIManager.Instance.GetData("character/get", (string ss, int rr) =>
                    {
                        var rawData = JsonConvert.DeserializeObject<MarkerDataDetail>(ss);
                        PlayerDataManager.playerData = LoginAPIManager.DictifyData(rawData);
                        LoginAPIManager.loggedIn = true;
                        PlayerManager.Instance.initStart();
                    });
                });
            }
            else
            {
                if (!LoginAPIManager.FTFComplete)
                {
                    LoginUIManager.isInFTF = true;
                    // FTFobject.SetActive(true);
                    StartCoroutine(LoadFTF());

                    PlayerManager.Instance.CreatePlayerStart();
                    loginObject.SetActive(false);
                    signInObject.SetActive(false);
                    SoundManagerOneShot.Instance.PlayWelcome();

                    mainUI.SetActive(true);
                    PlayerManagerUI.Instance.SetupUI();
                    return;
                }
                //MarkerManagerAPI.GetMarkers ();
                PlayerManager.Instance.CreatePlayerStart();
                mainUI.SetActive(true);
                PlayerManagerUI.Instance.SetupUI();
                loginObject.SetActive(false);
                signInObject.SetActive(false);
            }
        }
        else
        {
            Debug.Log("New account");
            //Matt is changing this temporarily
            //mainUI.SetActive(true);
            mainUI.SetActive(false);
            PlayerManagerUI.Instance.SetupUI();
            CharacterSelectTransition();
        }
    }

    public void WrongPassword()
    {
        SoundManagerOneShot.Instance.PlayLoginButton();

        loginButton.interactable = true;
        loadingObject.SetActive(false);
        passwordError.SetActive(true);
    }

    public void ForgotPassword()
    {
        SoundManagerOneShot.Instance.PlayLoginButton();

        resetPasswordStartObject.SetActive(true);
        userResetObject.SetActive(true);
        codeResetObject.SetActive(false);
        signInObject.SetActive(false);
    }

    public void DoReset()
    {
        SoundManagerOneShot.Instance.PlayLoginButton();

        if (resetAccountName.text.Length == 0)
        {
            return;
        }
        loadingObject.SetActive(true);
        LoginAPIManager.ResetPasswordRequest(resetAccountName.text);
        resetUserNullError.SetActive(false);
    }

    public void EmailNull()
    {
        emailNullObject.SetActive(true);
        loadingObject.SetActive(false);
        resetPasswordStartObject.SetActive(false);
    }

    public void CheckResetCode()
    {
        if (resetCodeInput.text.Length == 4)
            resetPasswordContinueButton.interactable = true;
        else
            resetPasswordContinueButton.interactable = false;
    }

    public void SubmitResetCode()
    {
        SoundManagerOneShot.Instance.PlayLoginButton();

        loadingObject.SetActive(true);
        LoginAPIManager.SendResetCode(resetCodeInput.text);
    }

    public void EnterResetCode(string msg)
    {
        loadingObject.SetActive(false);

        string s = msg[0].ToString() + msg[1].ToString() + msg[2].ToString() + msg[3].ToString();
        for (int i = 0; i < msg.Length - 8; i++)
        {
            s += "*";
        }
        s += msg[msg.Length - 4].ToString() + msg[msg.Length - 3].ToString() + msg[msg.Length - 2].ToString() + msg[msg.Length - 1].ToString();
        Debug.Log(s);
        userResetObject.SetActive(false);
        codeResetObject.SetActive(true);
        emailResetInfo.text = "Please enter the 4 digit rest code sent to " + s;
    }

    public void FinishPasswordReset()
    {
        SoundManagerOneShot.Instance.PlayLoginButton();

        loadingObject.SetActive(false);
        resetPasswordStartObject.SetActive(false);
        resetPasswordEndObject.SetActive(true);
        resetPassContinueButton.SetActive(true);
        resetPassbackButton.SetActive(false);
    }

    public void SendFinalPasswordReset()
    {
        SoundManagerOneShot.Instance.PlayLoginButton();

        if (resetpass1.text.Length < 4)
        {
            passwordResetInfo.text = "Password cannot be less than 4 characters";
            return;
        }

        if (resetpass1.text != resetpass2.text)
        {
            passwordResetInfo.text = "Passwords do not match";
            return;
        }
        LoginAPIManager.SendNewPassword(resetpass1.text);
        loadingObject.SetActive(true);
    }

    public void PostPasswordReset(string name, string pass)
    {
        accountName.text = name;
        accountPassword.text = pass;
        resetPasswordEndObject.SetActive(false);


        initiateLogin();
    }

    public void resetUserNull()
    {
        resetUserNullError.SetActive(true);
        loadingObject.SetActive(false);
    }

    public void ResetCodeWrong()
    {
        resetCodeWrongError.SetActive(true);
        loadingObject.SetActive(false);
    }

    public void PasswordTokenError(string error)
    {
        passwordResetInfo.text = error;
        loadingObject.SetActive(false);
        resetPassContinueButton.SetActive(false);
        resetPassbackButton.SetActive(true);
    }

    public void BackToLogin()
    {

        passwordResetInfo.text = "";
        resetPasswordEndObject.SetActive(false);
        initiateLogin();
    }


    #endregion


    public void SelectionStart(bool skipftf)
    {
        Instantiate(clickBlocker, witchSchool.transform);
        charSelectFinal.interactable = false;
        skipFTF = skipftf;
        SoundManagerOneShot.Instance.PlayLoginButton();
        loadingObject.SetActive(true);
        LoginAPIManager.tryCount = 0;
        LoginAPIManager.CreateCharacter(currentCharacter);
    }

    public void SelectionDone()
    {
        loadingObject.SetActive(false);
    }

    public void ToggleSelect()
    {
        SoundManagerOneShot.Instance.PlayWhisperFX();
        SoundManagerOneShot.Instance.PlayButtonTap();
        charSelectFinal.interactable = true;
        foreach (var item in toggles)
        {
            if (item.isOn)
            {
                currentCharacter = item.name;
            }
        }
    }

    public void CharacterSelectTransition()
    {
        RectTransform selected = null;
        foreach (var item in toggles)
        {
            if (item.isOn)
            {
                selected = item.GetComponent<RectTransform>();
            }
            else
            {
                bgFadeoutElements.Add(item.GetComponent<CanvasGroup>());
            }
        }
        StartCoroutine(AnimateToMain(selected));

    }

    IEnumerator AnimateToMain(RectTransform selected)
    {

        float t = 0;
        float iniPos = selected.anchoredPosition.x;
        while (t <= 1)
        {
            t += Time.deltaTime * fadeOutSpeed;
            foreach (var item in bgFadeoutElements)
            {
                item.alpha = Mathf.SmoothStep(1, 0, t);
            }
            selected.anchoredPosition = new Vector2(Mathf.SmoothStep(iniPos, 88, t), selected.anchoredPosition.y);
            yield return 0;
        }

        yield return new WaitForSeconds(.1f);
        SoundManagerOneShot.Instance.PlayWhisperFX();
        t = 0;





        while (t <= 1)
        {
            t += Time.deltaTime * fadeOutSpeed;
            selected.localScale = Vector3.one * Mathf.SmoothStep(.815f, .35f, t);
            playerFocus.alpha = Mathf.SmoothStep(0, 1, t);
            yield return 0;
        }
        if (skipFTF)
        {
            isInFTF = false;
            PlayerManager.Instance.CreatePlayerStart();
            //			Debug.Log ("Skipping FTF!");
            LoginAPIManager.FTFComplete = true;
            // LoginUIManager.isInFTF = false;
            // FTFobject.SetActive(false);
            //  MarkerManagerAPI.GetMarkers(true);
            APIManager.Instance.GetData("ftf/complete", (string s, int r) =>
            {
                //				Debug.Log (s + " FTF RES");
                APIManager.Instance.GetData("character/get", (string ss, int rr) =>
                {
                    Debug.Log("reinit");
                    var rawData = JsonConvert.DeserializeObject<MarkerDataDetail>(ss);
                    PlayerDataManager.playerData = LoginAPIManager.DictifyData(rawData);
                    LoginAPIManager.loggedIn = true;
                    PlayerManager.Instance.initStart();
                });
            });
        }
        else
        {
            isInFTF = true;
            StartCoroutine(LoadFTF());
            // Debug.Log("Continuing FTF!");
            // LoginUIManager.isInFTF = true;
            // FTFobject.SetActive(true);
            PlayerManager.Instance.CreatePlayerStart();
        }
        loginObject.SetActive(false);
        signInObject.SetActive(false);
        yield return new WaitForSeconds(1);
        SoundManagerOneShot.Instance.PlayWelcome();
        t = 0;
        //		yield return new WaitForSeconds (12);
        //		t = 0;
        //		while (t <= 1) {
        //			t += Time.deltaTime*fadeOutSpeed;
        //			playerFocus.alpha = Mathf.SmoothStep(1,0,t);
        //			yield return 0;
        //		}
    }

    IEnumerator LoadFTF()
    {
        var request = Resources.LoadAsync<GameObject>("FTF/FTFCanvas");
        yield return request;
        Instantiate(request.asset);
    }

    public void openTOS()
    {
        Application.OpenURL("https://www.raincrowstudios.com/terms-of-service");
    }

    public void openPP()
    {
        Application.OpenURL("https://www.raincrowstudios.com/privacy");
    }

    public void EnableCanvasGroup(bool enable)
    {
        mainCanvasGroup.interactable = enable;
    }
}