using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Facebook.Unity;
using System.Collections.Generic;
using Facebook.Unity.Example;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; set; }

    public static string IsFb
    {
        get { return PlayerPrefs.GetString("fb", ""); }
        set { PlayerPrefs.SetString("fb", value); }
    }
    public Animator anim;
    //	public GameObject loginButton;
    public GameObject profileObject;
    public Text playerFBName;
    public Image DisplayPic;

    [SerializeField]
    private Text m_AppVersion;

    void Awake()
    {
        Instance = this;
    }
    // Use this for initialization
    public void FbLoginSetup()
    {
        if (!FB.IsInitialized)
        {
            FB.Init(InitCallBack);
        }
    }

    void InitCallBack()
    {
        //		loginButton.SetActive (true);
        profileObject.SetActive(false);
        if (IsFb != "")
        {
            if (!Application.isEditor)
                LoginFB();
        }
    }

    public void LoginFB()
    {
        FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email", "user_friends" }, HandleResult);
    }

    public void HandleResult(IResult result)
    {
        if (result == null)
        {
            Debug.Log("Login Failed");
            return;
        }

        if (!string.IsNullOrEmpty(result.Error))
        {
            Debug.Log("Error - Check log for details");
            //			this.LastResponse = "Error Response:\n" + result.Error;
        }
        else if (result.Cancelled)
        {
            Debug.Log("Cancelled - Check log for details");
        }
        else if (!string.IsNullOrEmpty(result.RawResult))
        {
            Debug.Log("FB Logged in Success!");
            IsFb = "true";
            FB.API("/me/picture?type=square&height=128&width=128", HttpMethod.GET, FBPicCallBack);
            FB.API("/me?fields=first_name", HttpMethod.GET, FBNameCallBack);


        }
        else
        {
            Debug.Log("Empty Response\n");
        }
    }

    void FBPicCallBack(IGraphResult result)
    {
        if (string.IsNullOrEmpty(result.Error) && result.Texture != null)
        {
            DisplayPic.sprite = Sprite.Create(result.Texture, new Rect(0, 0, 128, 128), new Vector2(0.5f, 0.5f));
            //			loginButton.SetActive (false);
            profileObject.SetActive(true);
        }
    }

    void FBNameCallBack(IGraphResult result)
    {
        IDictionary<string, object> profile = result.ResultDictionary;
        playerFBName.text = profile["first_name"].ToString();
        Debug.Log(playerFBName.text);
    }

    public void Show()
    {
        Debug.Log("showing settings");
        anim.SetBool("animate", true);

        m_AppVersion.text = string.Concat("App Version: ", DownloadedAssets.AppVersion);
    }

    public void Hide()
    {
        anim.SetBool("animate", false);
        m_AppVersion.text = string.Empty;
    }
}

