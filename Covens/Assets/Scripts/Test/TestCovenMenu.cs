using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestCovenMenu : MonoBehaviour
{
    public string UserName;
    public string CovensName;
    public string LoginToken;
    public TestCreateAccounts m_TestCreateAccounts;

    private void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 200, 50), "Display ("+ UserName+") : " + CovensName))
        {
            StartCoroutine(DisplayProcess());
        }
    }


    IEnumerator DisplayProcess()
    {
        yield return StartCoroutine(GetToken());
        yield return StartCoroutine(DisplayCoven(CovensName));
    }


    public IEnumerator GetToken()
    {
        yield return null;
        bool bProcessing = true;
        if (string.IsNullOrEmpty(LoginToken))
        {
            TestCreateAccounts.UserData pData = new TestCreateAccounts.UserData();
            yield return StartCoroutine(
                m_TestCreateAccounts.LoginProcess(UserName, "1", pData)
                );

            LoginToken = pData.m_sLoginToken;
            bProcessing = false;
        }
        else
        {
            bProcessing = false;
        }
        while (bProcessing) { yield return null; }
    }


    public IEnumerator DisplayCoven(string sCovensName)
    {
        yield return null;
        bool bProcessing = true;
        Log("DisplayCoven : " + sCovensName);
        LoginAPIManager.loginToken = LoginToken;
        CovenManagerAPI.CovenDisplay(null, sCovensName, (CovenData pCovenData) =>
        {
            Log("DisplayCoven with success: " + sCovensName);
            TextRecord("Coven " + sCovensName);
            foreach ( CovenMember pMember in pCovenData.members)
            {
                TextRecord("\n - Member [" + pMember.displayName + "] level[" + pMember.level+ "] role[" + pMember.role + "]  title[" + pMember.title + "] ");
            }
            bProcessing = false;
        },
        (string sError) =>
        {
            Log("DisplayCoven with ERROR: " + sError);
            bProcessing = false;
        }
        );
        while (bProcessing) { yield return null; }
    }



    void Log(string sLog)
    {
        Debug.Log("[TestCovenMenu] " + sLog);
    }



    public static void TextRecord(string sLog)
    {
        GameObject pGO = GameObject.Find("Logger");
        Text pText = pGO.GetComponent<Text>();
        pText.text += sLog + "\n";
    }
    public static void TextSet(string sLog)
    {
        GameObject pGO = GameObject.Find("Logger");
        Text pText = pGO.GetComponent<Text>();
        pText.text = sLog + "\n";
    }
}
