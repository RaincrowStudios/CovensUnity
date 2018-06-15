using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(LoginAPIManager))]
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
	public Toggle male;
	public Toggle female;
	public Text createCharacterError;


	bool animate = false;
	public Animator anim;

	public GameObject Map;
	// Use this for initialization

	void Awake()
	{
		Instance = this;
	}

	void Start () {
		initiateLogin ();
		if (PlayerPrefs.GetString ("Username") != "") {
			accountName.Select ();
			accountName.text = PlayerPrefs.GetString ("Username");
			accountPassword.Select ();
			accountPassword.text = PlayerPrefs.GetString ("Password");
			doLogin ();
		}
	}

	void initiateLogin()
	{
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
	}

	public void doLogin () {
		loadingObject.SetActive (true);
		LoginAPIManager.Login (accountName.text, accountPassword.text);
	}

	public void InitiateCreateAccount (){
		chooseLoginTypeObject.SetActive (false);
		StartCoroutine (SetupDial ("Choose", "Create"));
		createAccount.SetActive (true);
	}

	public void CreateAccount (){
		createCharacterError.gameObject.SetActive (false);
		if (createAccountName.text.Length == 0) {
			createCharacterError.gameObject.SetActive (true);
			createCharacterError.text = "Account name cannot be empty.";
			return;
		}
		if (createAccountPassword.text.Length == 0) {
			createCharacterError.gameObject.SetActive (true);
			createCharacterError.text = "Password cannot be empty.";
			return;
		}
		if (createAccountEmail.text.Length == 0) {
			createCharacterError.gameObject.SetActive (true);
			createCharacterError.text = "Email cannot be empty.";
			return;
		}
		LoginAPIManager.CreateAccount (createAccountName.text, createAccountPassword.text, createAccountEmail.text);
		loadingObject.SetActive (true);
	}

	public void CreateAccountResponse(bool success, string error){
		loadingObject.SetActive (false);
		if (!success) {
			createCharacterError.gameObject.SetActive (true);
			createCharacterError.text = error;
			return;
		} else {
			createAccount.SetActive (false);
			StartCoroutine (SetupDial ("Choose", "Create"));
			PlayerPrefs.SetString ("Username", createAccountName.text);
			PlayerPrefs.SetString ("Password", createAccountPassword.text);
			createCharacter.SetActive (true);
		}
	}

	public void CreateCharacter()
	{
		createCharacterError.gameObject.SetActive (false);
		if (createCharacterName.text.Length == 0) {
			createCharacterError.gameObject.SetActive (true);
			createCharacterError.text = "Character name cannot be empty.";
			return;
		}
		if (!male.isOn && !female.isOn) {
			createCharacterError.gameObject.SetActive (true);
			createCharacterError.text = "Please choose a gender.";
			return;
		}
		bool ismale;
		if(male.isOn)
			ismale = true;
		else 
			ismale= false;
		LoginAPIManager.CreateCharacter (createCharacterName.text, ismale);
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
		if (resetpass1.text.Length < 6) {
			passwordResetInfo.text = "Password cannot be less than 6 characters";
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
        UIGenericPopup.ShowConfirmPopup("Login Error", cur, next, null);
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
	#endregion


}
