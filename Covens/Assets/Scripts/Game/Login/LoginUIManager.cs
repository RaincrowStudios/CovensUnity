using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
[RequireComponent(typeof(APIManager))]

public class LoginUIManager : MonoBehaviour {

	public static LoginUIManager Instance { get; set;}
	public string testUser;

	public GameObject loginObject;
	public GameObject chooseLoginTypeObject; 

	public Text currentText;
	public Text nextText;
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
	public CharacterSelection charSelect;
//	public static bool playerGender;
	public static string charUserName;
	HashSet<char> NameCheck = new HashSet<char>(){  'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z','a','d','b','c','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z','1','2','3','4','5','6','7','8','9','0' };

	bool animate = false;
	public Animator anim;

	public GameObject Map;
    // Use this for initialization
	

    #region player prefs


    #endregion


    void Awake()
	{
		Instance = this;
	}

	void Start()
	{
		LoginAPIManager.sceneLoaded = true;
		OnlineMaps.instance.position = PlayerDataManager.playerPos;
		OnlineMaps.instance.zoom = 16;
		if (!LoginAPIManager.loggedIn) {
			initiateLogin ();
		} else {
			if (!LoginAPIManager.hasCharacter) {
				initiateLogin ();
				chooseLoginTypeObject.SetActive (false);
				createCharacter.SetActive (true);
				StartCoroutine (SetupDial ("Choose", "Create"));

			} else {
				LoginAPIManager.InitiliazingPostLogin ();
				OnlineMaps.instance.transform.GetChild (0).gameObject.SetActive (false);
				charSelect.SkipCharSelect ();
				if (PlayerDataManager.playerData.energy == 0) {
					DeathState.Instance.ShowDeath ();
				}
				Invoke ("enableSockets", 2f);
			}
		}
	}

	void enableSockets()
	{
		WebSocketClient.websocketReady = true;

	}

   public void initiateLogin()
	{
		loadingObject.SetActive (false);
		print ("Initializing Login");  
		mainUI.SetActive (false);
		loginObject.SetActive (true);
		Map.SetActive (true);
		chooseLoginTypeObject.SetActive (true);
		StartCoroutine (SetupDial ("", "Choose"));
	}

	public void AlreadyLoggedIn()
	{
		StartCoroutine (SetupDial ("Choose", "Sign In"));
		loadingObject.SetActive (false);
		chooseLoginTypeObject.SetActive (false);
		signInObject.SetActive (true);
		accountName.text = LoginAPIManager.StoredUserName;
		accountPassword.text = LoginAPIManager.StoredUserPassword;
    }

	public void doLogin () {
		loadingObject.SetActive (true);
		LoginAPIManager.isNewAccount = false;
		LoginAPIManager.  StoredUserName = accountName.text;
		LoginAPIManager.   StoredUserPassword = accountPassword.text;
        LoginAPIManager.Login( accountName.text, accountPassword.text);
		loginButton.interactable = false;
	}

	public void InitiateCreateAccount (){
		chooseLoginTypeObject.SetActive (false);
		StartCoroutine (SetupDial ("Choose", "Create"));
		createAccount.SetActive (true);
	}

	public void CreateAccount (){
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
			StartCoroutine (SetupDial ("Choose", "Create"));
			LoginAPIManager.  StoredUserName = createAccountName.text;
			LoginAPIManager. StoredUserPassword = createAccountPassword.text;
			createCharacter.SetActive (true);

		}
	}

	public void CreateCharacter()
	{
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
//
//		if (!male.isOn && !female.isOn) {
//			createCharacterError.gameObject.SetActive (true);
//			createCharacterError.text = "Please choose a gender";
//			return;
//		}
//		bool ismale = false;
//		if (male.isOn) {
//			ismale = true;
//		}
//
//		playerGender = ismale;
		var checkName = new {displayName = createCharacterName.text}; 
		APIManager.Instance.Post ("check-name", JsonConvert.SerializeObject (checkName), CreateCharacterError, true, false);
//			LoginAPIManager.CreateCharacter (createCharacterName.text, JsonConvert.SerializeObject());
		createCharButton.interactable = false;
		loadingObject.SetActive (true);
	}

	public void CreateCharacterError(string s, int r)
	{
		print (s);
		if (r == 200) {
			charUserName = createCharacterName.text;
			charSelect.StartAnimation ();
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
		MarkerManagerAPI.GetMarkers ();
		PlayerManager.Instance.CreatePlayerStart ();
		mainUI.SetActive (true);
		PlayerManagerUI.Instance.SetupUI ();
		loginObject.SetActive (false);
		signInObject.SetActive (false);
	}

	public void WrongPassword()
	{
		loginButton.interactable = true;
		StartCoroutine (SetupDial ("Sign In", "Try Again"));
		loadingObject.SetActive (false);
		passwordError.SetActive (true);
	}

	public void ForgotPassword()
	{
		StartCoroutine (SetupDial ("Sign In", "Reset"));
		resetPasswordStartObject.SetActive (true);
		userResetObject.SetActive (true);
		codeResetObject.SetActive (false);
		signInObject.SetActive (false);
	}

	public void DoReset()
	{

		if (resetAccountName.text.Length == 0) {
			StartCoroutine (SetupDial (currentText.text, "Empty Name"));
			return;
		}
		loadingObject.SetActive (true);
		LoginAPIManager.ResetPasswordRequest (resetAccountName.text);
		resetUserNullError.SetActive (false);
	}

	public void EmailNull()
	{
		StartCoroutine (SetupDial (currentText.text, "Empty Email"));
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
		loadingObject.SetActive (true);
		LoginAPIManager.SendResetCode (resetCodeInput.text);
	}

	public void EnterResetCode(string msg)
	{
		loadingObject.SetActive (false);
		StartCoroutine (SetupDial (currentText.text, "Enter Code"));

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
		loadingObject.SetActive (false);
		resetPasswordStartObject.SetActive (false);
		StartCoroutine (SetupDial (currentText.text, "Set Password"));
		resetPasswordEndObject.SetActive (true);
		resetPassContinueButton.SetActive (true);
		resetPassbackButton.SetActive (false);
	}

	public void SendFinalPasswordReset()
	{
		if (resetpass1.text.Length < 4) {
			passwordResetInfo.text = "Password cannot be less than 4 characters";
			StartCoroutine (SetupDial (currentText.text, "Try Again"));
			return;
		}

		if(resetpass1.text != resetpass2.text){
			StartCoroutine (SetupDial (currentText.text, "Try Again"));
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

	IEnumerator SetupDial(string cur, string next)
	{
		animate = !animate;
		anim.SetBool ("move", animate);
		yield return new WaitForSeconds (.4f);
		currentText.text = cur;
		nextText.text = next;
        //UIGenericPopup.ShowConfirmPopup("Login Error", cur, next, null);
	}

	public void resetUserNull()
	{
		StartCoroutine (SetupDial (currentText.text, "Try Again"));
		resetUserNullError.SetActive (true);
		loadingObject.SetActive (false);
	}

	public void ResetCodeWrong()
	{
		StartCoroutine (SetupDial (currentText.text, "Try Again"));
		resetCodeWrongError.SetActive (true);
		loadingObject.SetActive (false);
	}

	public void PasswordTokenError(string error)
	{
		StartCoroutine (SetupDial (currentText.text, "Try Again"));
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

	public void setTitle(string title)
	{
		StartCoroutine (SetupDial (currentText.text, title));
	}

	#endregion


}
