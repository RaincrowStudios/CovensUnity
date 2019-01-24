using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using TMPro;

public class LoginUIManager : MonoBehaviour
{    
    private static LoginUIManager m_Instance;
	public static LoginUIManager Instance
    {
        get
        {
            if (m_Instance == null)
            {
                Instantiate(Resources.Load<LoginUIManager>("UI/Login"));
            }
            return m_Instance;
        }
    }

    [Header("Main")]
    [SerializeField] private CanvasGroup mainCanvasGroup;
    public GameObject loginObject;
    public Animator animSavannah;
    public List<CanvasGroup> bgFadeoutElements = new List<CanvasGroup>();
    public float fadeOutSpeed = 1;

    [Header("Home")]
	public GameObject chooseLoginTypeObject;
    [SerializeField] private Button home_SigninButton;
    [SerializeField] private Button home_NewgameButton;

    [Header("Sign in")]
    public GameObject signInObject;
    public GameObject passwordError;
    public TMP_InputField accountName;
    public TMP_InputField accountPassword;
    public Button loginButton;
    [SerializeField] private Button signin_BackButton;
    [SerializeField] private Button signin_ForgotpassButton;
    [SerializeField] private Button signin_ContinueButton;
    [SerializeField] private Button signin_TermsButton;
    
    [Header("Password reset - start")]
    public GameObject resetPasswordStartObject;
    public Text emailResetInfo;
    public InputField resetCodeInput;
    public InputField resetAccountName;
    public GameObject resetUserNullError;
    public GameObject resetCodeWrongError;
    public GameObject userResetObject;
    public GameObject codeResetObject;
    public Button resetPasswordContinueButton;
    [SerializeField] private Button resetstart_ContinueButton;
    [SerializeField] private Button resetstart_SubmitcodeButton;
    [SerializeField] private Button resetstart_BackButton;

    [Header("Password reset - end")]
    public GameObject resetPasswordEndObject;
    public Text passwordResetInfo;
    public InputField resetpass1;
    public InputField resetpass2;
    public GameObject loadingObject;
    public GameObject resetPassContinueButton;
    public GameObject resetPassbackButton;
    [SerializeField] private Button resetEnd_ResetButton;
    [SerializeField] private Button resetEnd_BackButton;

    [Header("Email !exist")]
    public GameObject emailNullObject;
    [SerializeField] private Button nomail_BackButton;
    [SerializeField] private Button nomail_MailButton;

    [Header("Create account")]
    public GameObject createAccount;
    public TMP_InputField createAccountName;
    public TMP_InputField createAccountEmail;
    public TMP_InputField createAccountPassword;
    public TextMeshProUGUI createAccountError;
    public Button createAccountButton;
    [SerializeField] private Button createacc_ContinueButton;
    [SerializeField] private Button createacc_BackButton;
    [SerializeField] private Button createacc_TermsButton;

    [Header("Create character")]
    public GameObject createCharacter;
    public TMP_InputField createCharacterName;
    public TextMeshProUGUI createCharacterError;
    public Button createCharButton;
    [SerializeField] private Button createcha_ContinueButton;

    [Header("Choose character")]
    public GameObject CharSelectWindow;
    public Toggle[] toggles;
    public CanvasGroup charSelectFinal;
    [SerializeField] private Button choosecha_ContinueButton;
    [SerializeField] private Button choosecha_SkipButton;


    public static string charUserName;


    private HashSet<char> NameCheck = new HashSet<char>() { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'd', 'b', 'c', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };
    private bool skipFTF = false;
    private string currentCharacter;


    private void Awake()
    {
        m_Instance = this;

        mainCanvasGroup.alpha = 0;
        mainCanvasGroup.interactable = false;
        GetComponentInChildren<Canvas>(true).worldCamera = GameObject.FindGameObjectWithTag("UICamera").GetComponent<Camera>();

        //setup init state
        chooseLoginTypeObject.SetActive(false);
        signInObject.SetActive(false);
        resetPasswordStartObject.SetActive(false);
        resetPasswordEndObject.SetActive(false);
        emailNullObject.SetActive(false);
        createAccount.SetActive(false);
        createCharacter.SetActive(false);
        CharSelectWindow.SetActive(false);

        // setup button callbacks
        //home
        home_SigninButton.onClick.AddListener(AlreadyLoggedIn);
        home_NewgameButton.onClick.AddListener(InitiateCreateAccount);

        //signin
        signin_BackButton.onClick.AddListener(() =>
            {
                signInObject.SetActive(false);
                chooseLoginTypeObject.SetActive(true);
                SoundManagerOneShot.Instance.MenuSound();
            });
        signin_ContinueButton.onClick.AddListener( doLogin);
        signin_ForgotpassButton.onClick.AddListener(ForgotPassword);
        signin_TermsButton.onClick.AddListener(() =>
        {
            openPP(); openTOS();
        });

        //reset start
        resetstart_BackButton.onClick.AddListener(() =>
            {
                resetPasswordStartObject.SetActive(false);
                chooseLoginTypeObject.SetActive(true);
                SoundManagerOneShot.Instance.MenuSound();
            });
        resetstart_ContinueButton.onClick.AddListener(DoReset);
        resetstart_SubmitcodeButton.onClick.AddListener(SubmitResetCode);

        //reset final
        resetEnd_BackButton.onClick.AddListener(() =>
            {
                resetPasswordEndObject.SetActive(false);
                chooseLoginTypeObject.SetActive(true);
                SoundManagerOneShot.Instance.MenuSound();
            });
        resetEnd_ResetButton.onClick.AddListener(SendFinalPasswordReset);

        //mail null
        nomail_BackButton.onClick.AddListener(() =>
        {
            emailNullObject.SetActive(false);
            chooseLoginTypeObject.SetActive(true);
            SoundManagerOneShot.Instance.MenuSound();
        });
        nomail_MailButton.onClick.AddListener(() =>
        {
            openPP(); openTOS();
        });

        //create account
        createacc_BackButton.onClick.AddListener(() =>
        {
            createAccount.SetActive(false);
            chooseLoginTypeObject.SetActive(true);
        });
        createacc_ContinueButton.onClick.AddListener(CreateAccount);
        createacc_TermsButton.onClick.AddListener(() =>
        {
            openPP(); openTOS();
        });

        //create char
        createcha_ContinueButton.onClick.AddListener(CreateCharacter);

        //choose char preset
        choosecha_ContinueButton.onClick.AddListener(() => SelectionStart(false));
        choosecha_SkipButton.onClick.AddListener(() => SelectionStart(true));


        //setup events
        LoginAPIManager.OnGetCharacter += CorrectPassword;
    }

    public void ShowHome()
    {
        chooseLoginTypeObject.SetActive(true);
        initiateLogin();

        mainCanvasGroup.alpha = 1;
        mainCanvasGroup.interactable = true;
    }

    public void ShowCreateCharacter()
    {
        initiateLogin();
        createCharacter.SetActive(true);

        mainCanvasGroup.alpha = 1;
        mainCanvasGroup.interactable = true;
    }

	public void initiateLogin()
	{
		createCharacter.SetActive (false);
		CharSelectWindow.SetActive (false);
		signInObject.SetActive (false);
		loadingObject.SetActive (false);
		print ("Initializing Login");
        UIMain.Instance.Hide();
		loginObject.SetActive (true);
		chooseLoginTypeObject.SetActive (true);
	}

	public void AlreadyLoggedIn()
	{
		SoundManagerOneShot.Instance.PlayButtonTap ();
		loadingObject.SetActive (false);
		chooseLoginTypeObject.SetActive (false);
		signInObject.SetActive (true);
		accountName.text = LoginAPIManager.StoredUserName;
		accountPassword.text = LoginAPIManager.StoredUserPassword;
        loginButton.interactable = true;
	}


	public void doLogin () {
		SoundManagerOneShot.Instance.PlayLoginButton ();
		loadingObject.SetActive (true);
		LoginAPIManager.isNewAccount = false;
		LoginAPIManager.  StoredUserName = accountName.text;
		LoginAPIManager.   StoredUserPassword = accountPassword.text;
		LoginAPIManager.Login( accountName.text, accountPassword.text);
		loginButton.interactable = false;
	}

	public void InitiateCreateAccount (){
		SoundManagerOneShot.Instance.PlayLoginButton ();
		chooseLoginTypeObject.SetActive (false);
		createAccount.SetActive (true);
	}

	public void CreateAccount (){
		SoundManagerOneShot.Instance.PlayLoginButton ();

		createCharacterError.gameObject.SetActive (false);

		if (createAccountName.text.Length < 4) {
			print ("less char");
			createAccountError.gameObject.SetActive (true);
			createAccountError.text = "Account name should have at least 4 letters";
			return;
		}

		foreach (var item in createAccountName.text) {
			if (!NameCheck.Contains (item)) {
				print ("fail char");

				createAccountError.gameObject.SetActive (true);
				createAccountError.text = "Account name cannot contain special characters";
				return;
			}
		}

		if (createAccountPassword.text.Length < 4) {
			createAccountError.gameObject.SetActive (true);
			createAccountError.text = "Password should have at least 4 letters.";
			return;
		}
		createAccountButton.interactable = false;
		LoginAPIManager.CreateAccount (createAccountName.text, createAccountPassword.text, createAccountEmail.text);
		loadingObject.SetActive (true);
	}

	public void CreateAccountResponse(bool success, string error){
		loadingObject.SetActive (false);
		if (!success) {
			createAccountError.gameObject.SetActive(true);
			createAccountError.text = error;
			createAccountButton.interactable = true;
			return;
		} else {
			createAccount.SetActive (false);
			LoginAPIManager.  StoredUserName = createAccountName.text;
			LoginAPIManager. StoredUserPassword = createAccountPassword.text;
			createCharacter.SetActive (true);

		}
	}

	public void CreateCharacter()
	{
		SoundManagerOneShot.Instance.PlayLoginButton ();

		createCharacterError.gameObject.SetActive (false);
		if (createCharacterName.text.Length < 4) {
			createCharacterError.gameObject.SetActive (true);
			createCharacterError.text = "Character name should have at least 4 letters.";
			return;
		}

		foreach (var item in createCharacterName.text) {
			if (!NameCheck.Contains (item) ) {
				if (item == ' ') {
					continue;
				}
				createCharacterError.gameObject.SetActive (true);
				createCharacterError.text = "character name cannot contain special characters";
				return;
			}
		}

		var checkName = new {displayName = createCharacterName.text}; 
		APIManager.Instance.Post ("check-name", JsonConvert.SerializeObject (checkName), CreateCharacterError, true, false);
	
		createCharButton.interactable = false;

		loadingObject.SetActive (true);
	}

	public void CreateCharacterError(string s, int r)
	{
		print (s);
		if (r == 200) {
			charUserName = createCharacterName.text;
			//			charSelect.StartAnimation ();
			createCharacter.SetActive (false);
			CharSelectWindow.SetActive (true);
			loadingObject.SetActive (false);
			charSelectFinal.interactable = false;
			animSavannah.Play ("out");
		} else {
			if (s == "4103") {
				createCharacterError.gameObject.SetActive (true);
				createCharacterError.text = "Character name is taken";
				createCharButton.interactable = true;
			} else if (s == "4104") {
				createCharacterError.gameObject.SetActive (true);
				createCharacterError.text = "Character name is invalid";
				createCharButton.interactable = true;
			} else if (s == "4105") {
				createCharacterError.gameObject.SetActive (true);
				createCharacterError.text = "Character name is Empty";
				createCharButton.interactable = true;
			} else {
				createCharacterError.gameObject.SetActive (true);
				createCharacterError.text = "Could not create character . . .";
				createCharButton.interactable = true;
			}
		}
	}


    #region password
    public void CorrectPassword()
    {
        if (!LoginAPIManager.isNewAccount)
        {

            if (!LoginAPIManager.FTFComplete)
            {
                loginObject.SetActive(false);
                signInObject.SetActive(false);
                return;
            }
            loginObject.SetActive(false);
            signInObject.SetActive(false);
        }
        else
        {
            CharacterSelectTransition();
        }
    }

    private void HideAndDestroy()
    {
        mainCanvasGroup.interactable = false;
        LeanTween.value(mainCanvasGroup.alpha, 0f, 0.4f)
            .setOnUpdate((float t) =>
            {
                mainCanvasGroup.alpha = t;
            })
            .setOnComplete(() =>
            {
                LoginAPIManager.OnGetCharacter -= CorrectPassword;
                Destroy(this.gameObject);
            });
    }

	public void WrongPassword()
	{
		SoundManagerOneShot.Instance.PlayLoginButton ();

		loginButton.interactable = true;
		loadingObject.SetActive (false);
		passwordError.SetActive (true);
	}

	public void ForgotPassword()
	{
		SoundManagerOneShot.Instance.PlayLoginButton ();

		resetPasswordStartObject.SetActive (true);
		userResetObject.SetActive (true);
		codeResetObject.SetActive (false);
		signInObject.SetActive (false);
	}

	public void DoReset()
	{
		SoundManagerOneShot.Instance.PlayLoginButton ();

		if (resetAccountName.text.Length == 0) {
			return;
		}
		loadingObject.SetActive (true);
		LoginAPIManager.ResetPasswordRequest (resetAccountName.text);
		resetUserNullError.SetActive (false);
	}

	public void EmailNull()
	{
		emailNullObject.SetActive (true);
		loadingObject.SetActive (false);
		resetPasswordStartObject.SetActive (false);
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
		SoundManagerOneShot.Instance.PlayLoginButton ();

		loadingObject.SetActive (true);
		LoginAPIManager.SendResetCode (resetCodeInput.text);
	}

	public void EnterResetCode(string msg)
	{
		loadingObject.SetActive (false);

		string s = msg [0].ToString() + msg [1].ToString() + msg [2].ToString() + msg [3].ToString();
		for (int i = 0; i < msg.Length-8; i++) {
			s+="*";
		}
		s += msg [msg.Length - 4].ToString () + msg [msg.Length - 3].ToString () + msg [msg.Length - 2].ToString () + msg [msg.Length-1].ToString ();
		print (s);
		userResetObject.SetActive (false);
		codeResetObject.SetActive (true);
		emailResetInfo.text = "Please enter the 4 digit rest code sent to "+ s;
	}

	public void FinishPasswordReset()
	{
		SoundManagerOneShot.Instance.PlayLoginButton ();

		loadingObject.SetActive (false);
		resetPasswordStartObject.SetActive (false);
		resetPasswordEndObject.SetActive (true);
		resetPassContinueButton.SetActive (true);
		resetPassbackButton.SetActive (false);
	}

	public void SendFinalPasswordReset()
	{
		SoundManagerOneShot.Instance.PlayLoginButton ();

		if (resetpass1.text.Length < 4) {
			passwordResetInfo.text = "Password cannot be less than 4 characters";
			return;
		}

		if(resetpass1.text != resetpass2.text){
			passwordResetInfo.text = "Passwords do not match";
			return;
		}
		LoginAPIManager.SendNewPassword (resetpass1.text);
		loadingObject.SetActive (true);
	}

	public void PostPasswordReset(string name, string pass)
	{
		accountName.text = name;
		accountPassword.text = pass;
		resetPasswordEndObject.SetActive (false);


		initiateLogin ();
	}

	public void resetUserNull()
	{
		resetUserNullError.SetActive (true);
		loadingObject.SetActive (false);
	}

	public void ResetCodeWrong()
	{
		resetCodeWrongError.SetActive (true);
		loadingObject.SetActive (false);
	}

	public void PasswordTokenError(string error)
	{
		passwordResetInfo.text = error;
		loadingObject.SetActive (false);
		resetPassContinueButton.SetActive (false);
		resetPassbackButton.SetActive (true);
	}

	public void BackToLogin()
	{

		passwordResetInfo.text = "";
		resetPasswordEndObject.SetActive (false);
		initiateLogin ();
	}


	#endregion


	public void SelectionStart(bool skipftf)
	{
		charSelectFinal.interactable= false;
		skipFTF = skipftf;
		SoundManagerOneShot.Instance.PlayLoginButton ();
		loadingObject.SetActive (true);
        LoginAPIManager.tryCount = 0;
		LoginAPIManager.CreateCharacter (currentCharacter);
	}

	public void SelectionDone()
	{
		loadingObject.SetActive (false);
	}

	public void ToggleSelect()
	{
		SoundManagerOneShot.Instance.PlayWhisperFX();
		SoundManagerOneShot.Instance.PlayButtonTap();
		charSelectFinal.interactable= true;
		foreach (var item in toggles) {
			if (item.isOn) {
				currentCharacter = item.name;	
			}
		}
	}

	public void CharacterSelectTransition()
	{
		RectTransform selected = null;
		foreach (var item in toggles) {
			if (item.isOn) {
				selected = item.GetComponent<RectTransform> ();
			} else {
				bgFadeoutElements.Add (item.GetComponent<CanvasGroup> ());				
			}
		}
		StartCoroutine (AnimateToMain (selected));

	}

	IEnumerator AnimateToMain (RectTransform selected)
	{

		float t = 0;
		float iniPos = selected.anchoredPosition.x;
		while (t <= 1) {
			t += Time.deltaTime*fadeOutSpeed;
			foreach (var item in bgFadeoutElements) {
				item.alpha = Mathf.SmoothStep (1, 0, t);
			}
			selected.anchoredPosition = new Vector2 (Mathf.SmoothStep( iniPos, 88, t),selected.anchoredPosition.y);
			yield return 0;
		}
	
		yield return new WaitForSeconds (.1f);
		SoundManagerOneShot.Instance.PlayWhisperFX ();
		t = 0;





		while (t <= 1) {
			t += Time.deltaTime*fadeOutSpeed;
			selected.localScale = Vector3.one * Mathf.SmoothStep (.815f, .35f, t);
			yield return 0;
		}
		if (skipFTF) {
			PlayerManager.Instance.CreatePlayerStart ();
//			print ("Skipping FTF!");
			LoginAPIManager.FTFComplete = true;
			FTFManager.isInFTF = false;
			MarkerManagerAPI.GetMarkers (true);
			APIManager.Instance.GetData ("ftf/complete", (string s, int r) => {
//				Debug.Log (s + " FTF RES");
				APIManager.Instance.GetData ("character/get", (string ss, int rr) => {
					print ("reinit");
					var rawData = JsonConvert.DeserializeObject<MarkerDataDetail> (ss); 
					PlayerDataManager.playerData = LoginAPIManager.DictifyData (rawData); 
					LoginAPIManager.loggedIn = true;
					PlayerManager.Instance.initStart ();
				}); 
			});
		} else {
			print ("Continuing FTF!");
			FTFManager.isInFTF = true;
            FTFManager.Instance.Show();
			PlayerManager.Instance.CreatePlayerStart ();
		}
		loginObject.SetActive (false); 
		signInObject.SetActive (false);
		yield return 	new WaitForSeconds (1);
		SoundManagerOneShot.Instance.PlayWelcome ();
		t = 0;

        HideAndDestroy();
    }

	public void GetMarkers()
	{
		MarkerManagerAPI.GetMarkers ();
	}

	public void openTOS()
	{
		Application.OpenURL ("https://www.raincrowstudios.com/terms-of-service");
	}

	public void openPP(){
		Application.OpenURL ("https://www.raincrowstudios.com/privacy");
	}

    public static void EnableCanvasGroup(bool enable)
    {
        if (m_Instance == null)
            return;

        m_Instance.mainCanvasGroup.interactable = enable;
    }
}