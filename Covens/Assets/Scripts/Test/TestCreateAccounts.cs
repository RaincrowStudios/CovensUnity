using System.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;



public class TestCreateAccounts : MonoBehaviour
{
    const string NameTemplate = "auto-{0}";

    public int Amount;
    public int StartIndex;
    bool m_bProcessing = false;


    public class UserData
    {
        public string m_sName;
        public string m_sLoginToken;
        public string m_sWssToken;
        public string JsonAccount;
        public string JsonCharacter;
        public string JsonLogin;
        public bool m_bAccountFailed;
        public bool m_bCharacterFailed;
        public bool m_bLoginFailed;
    }

    


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnGUI()
    {
        if (m_bProcessing)
            GUI.Label(new Rect(0, 300, 200, 50), "Processing...");
        if(GUI.Button(new Rect(0,50,200,50), "Create Chars"))
        {
            CreateAccounts(Amount, StartIndex, null);
        }
    }




    public void CreateAccounts(int iAmount, int iStartIndex, List<UserData> vUserData)
    {
        StartCoroutine(CreateAccountsProcess(iAmount, iStartIndex, vUserData));
    }

    public IEnumerator CreateAccountsProcess(int iAmount, int iStartIndex, List<UserData> vUserData)
    {
        m_bProcessing = true;
        if(vUserData == null)
            vUserData = new List<UserData>();
        for (int i = 0; i < iAmount; i++)
        {
            UserData pData = new UserData();
            string sName = string.Format(NameTemplate, (i + iStartIndex));
            string sPass = "1";
            string sMail = "autotest@test.com";

            pData.m_sName = sName;
            // create acc
            yield return StartCoroutine(CreateAccountProcess(sName, sPass, sMail, pData));
            // create char
            if (!pData.m_bAccountFailed)
            {
                yield return StartCoroutine(CreateCharacterProcess(sName, true, pData));
            }
            // login
            if (!pData.m_bCharacterFailed)
            {
                yield return StartCoroutine(LoginProcess(sName, sPass, pData));
            }

            string sRes = "Result:";
            sRes += "\n m_bAccountFailed: " + pData.m_bAccountFailed;
            sRes += "\n JsonAccount: " + pData.JsonAccount;
            sRes += "\n m_bCharacterFailed: " + pData.m_bCharacterFailed;
            sRes += "\n JsonCharacter: " + pData.JsonCharacter;
            sRes += "\n m_bLoginFailed: " + pData.m_bLoginFailed;
            sRes += "\n JsonLogin: " + pData.JsonLogin;
            sRes += "\n loginToken: " + pData.m_sLoginToken;
            sRes += "\n wsToken: " + pData.m_sWssToken;

            Log(sRes);
            if (!pData.m_bLoginFailed)
                WriteAccount(pData);
            vUserData.Add(pData);
        }
        Log("done");
        m_bProcessing = false;
        StartIndex += iAmount;
        yield return null;
    }

    public IEnumerator CreateAccountProcess(string Username, string Password, string Email, UserData pData)
    {
        bool bDone = false;
        Action<string, int> pOnResponse = (string result, int status) =>
        {
            Log("CreateAccountProcess status: " + status);
            pData.m_bAccountFailed = status != 200;
            if (status == 200)
            {
                pData.JsonAccount = result;
                var Json = JsonConvert.DeserializeObject<PlayerLoginCallback>(result);
                pData.m_sWssToken = Json.token;
                LoginAPIManager.wssToken = Json.token;
            }
            bDone = true;
        };

        Log("Creating account Username[" + Username + "]");
//        LoginAPIManager.CreateAccount(Username, Password, Email, pOnResponse);

        while (!bDone)
        {
            yield return null;
        }
    }

    public IEnumerator CreateCharacterProcess(string Username, bool isMale, UserData pData)
    {
        bool bDone = false;
        Action<string, int> pOnResponse = (string result, int status) =>
        {
            Log("CreateCharacterProcess status: " + status);
            pData.m_bCharacterFailed = !result.Contains("connect ECONNREFUSED") && status != 200;
            if (status == 200)
            {
                pData.JsonCharacter = result;
                var Json = JsonConvert.DeserializeObject<PlayerLoginCallback>(result);
                pData.m_sLoginToken = Json.token;
                LoginAPIManager.loginToken = Json.token;
            }

            bDone = true;
        };
        Log("Creating character Username[" + Username + "]");
//        LoginAPIManager.CreateCharacter(Username, isMale, pOnResponse);

        while (!bDone)
        {
            yield return null;
        }
    }
    
    public IEnumerator LoginProcess(string Username, string Password, UserData pData)
    {
        bool bDone = false;
        Action<string, int> pOnResponse = (string result, int status) =>
        {
            Log("LoginProcess status: " + status);
            pData.m_bLoginFailed = status != 200;
            if (status == 200)
            {
                pData.JsonLogin = result;
                var Json = JsonConvert.DeserializeObject<PlayerLoginCallback>(result);
                pData.m_sLoginToken = Json.token;
                pData.m_sWssToken = Json.wsToken;
                LoginAPIManager.loginToken = Json.token;
                LoginAPIManager.wssToken = Json.wsToken;
            }

            bDone = true;
        };
        Log("Login Username[" + Username + "]");
//        LoginAPIManager.Login(Username, Password, pOnResponse);

        while (!bDone)
        {
            yield return null;
        }
    }




    void Log(string sLog)
    {
        Debug.Log("[TestCreateAccount] " + sLog);
    }

    void WriteAccount(UserData pData)
    {
        string sRes = "---------------- ";
        sRes += DateTime.Now.ToString("dd-MM HH:mm");
        sRes += "\n++ Name: " + pData.m_sName;
        sRes += "\n++ loginToken: ";
        sRes += "\n" + pData.m_sLoginToken;
        sRes += "\n++ wsToken: ";
        sRes += "\n" + pData.m_sWssToken;
        Write(sRes);
    }
    void Write(string sLog)
    {
        //Debug.Log("Log write");
        if (!Directory.Exists("Logs"))
            Directory.CreateDirectory("Logs");
        string m_LogFile = "Logs/TestAccoutns.txt";
        var writer = new StreamWriter(m_LogFile, true);
        writer.Write(sLog);
        writer.Write("\n");
        writer.Flush();
        writer.Close();
    }
}