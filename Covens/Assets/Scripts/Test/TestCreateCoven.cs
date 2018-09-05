using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCreateCoven : MonoBehaviour
{
    public int Amount;
    public int StartIndex;
    bool m_bProcessing = false;
    public TestCreateAccounts m_TestCreateAccounts;

    public string CovensName = "";
    public bool CreateNewCoven = true;

    private MemberInvite m_MemberInvite;


    private void OnGUI()
    {
        if (GUI.Button(new Rect(0, 100, 200, 50), "Create coven (" + CovensName + ")"))
        {
            CreateCovens(Amount, StartIndex);
        }
    }

    public void CreateCovens(int iAmount, int iStartIndex)
    {
        StartCoroutine(CreateCovensProcess(iAmount, iStartIndex));
    }



    public IEnumerator CreateCovensProcess(int iAmount, int iStartIndex)
    {
        m_bProcessing = true;

        // create accounts
        List<TestCreateAccounts.UserData> vUserData = new List<TestCreateAccounts.UserData>();
        yield return StartCoroutine(m_TestCreateAccounts.CreateAccountsProcess(iAmount, iStartIndex, vUserData));

        // 
        for (int i = 0; i < iAmount; i++)
        {
            if(i == 0)
            {
                // create, invite and collect pending
                yield return StartCoroutine(CreateCoven(vUserData[i]));
                yield return StartCoroutine(InviteCoven(vUserData));
                yield return StartCoroutine(ViewPendingCoven(vUserData[i]));                
            }
            else
            {
                yield return StartCoroutine(JoinCoven(vUserData[i]));
                
            }
        }

        // display coven
        yield return StartCoroutine(DisplayCoven(vUserData[0]));
        
        Log("done");
        m_bProcessing = false;
        StartIndex += iAmount;
        yield return null;
    }




    public IEnumerator DisplayCoven(TestCreateAccounts.UserData pData)
    {
        yield return null;
        bool bProcessing = true;
        Log("DisplayCoven : " + CovensName);
        LoginAPIManager.loginToken = pData.m_sLoginToken;
        CovenManagerAPI.CovenDisplay(null, CovensName, (CovenData pCovenData) =>
        {
            Log("DisplayCoven with success: " + CovensName);
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
    public IEnumerator CreateCoven(TestCreateAccounts.UserData pData)
    {
        yield return null;
        bool bProcessing = true;
        Log("CreateCoven : " + CovensName);
        LoginAPIManager.loginToken = pData.m_sLoginToken;
        CovenManagerAPI.CreateCoven(CovensName, (string sOk) =>
        {
            Log("CreateCoven with success: " + CovensName);
            bProcessing = false;
        },
        (string sError) =>
        {
            Log("CreateCoven with ERROR: " + sError);
            bProcessing = false;
        }
        );
        while (bProcessing) { yield return null; }
    }

    public IEnumerator InviteCoven(List<TestCreateAccounts.UserData> vData)
    {
        yield return null;
        for (int i = 1; i < vData.Count; i++)
        {
            bool bProcessing = true;
            Log("InviteCoven : " + vData[i].m_sName);
            CovenManagerAPI.CovenInvite(CovensName, vData[i].m_sName, (string sOk) =>
            {
                Log("InviteCoven with success: " + vData[i].m_sName);
                bProcessing = false;
            },
            (string sError) =>
            {
                Log("InviteCoven with ERROR: " + sError);
                bProcessing = false;
            }
            );
            while (bProcessing) { yield return null; }
        }
    }
    
    public IEnumerator ViewPendingCoven(TestCreateAccounts.UserData pData)
    {
        yield return null;
        bool bProcessing = true;
        Log("InviteCoven : " + pData.m_sName);
        CovenManagerAPI.CovenViewPending(CovensName, (MemberInvite pMemberInvite) =>
        {
            Log("CovenViewPending with success: " + CovensName);
            m_MemberInvite = pMemberInvite;
            bProcessing = false;
        },
        (string sError) =>
        {
            Log("InviteCoven with ERROR: " + sError);
            bProcessing = false;
        }
        );
        while (bProcessing) { yield return null; }
    }
    public IEnumerator JoinCoven(TestCreateAccounts.UserData pData)
    {
        yield return null;
        bool bProcessing = true;
        Log("CovenJoin : " + pData.m_sName);
        LoginAPIManager.loginToken = pData.m_sLoginToken;
        string sJoinToken = GetInviteToken(pData.m_sName);
        CovenManagerAPI.CovenJoin(sJoinToken, (string sOk) =>
        {
            Log("JoinCoven with success: " + pData.m_sName);
            bProcessing = false;
        },
        (string sError) =>
        {
            Log("JoinCoven with ERROR: " + sError);
            bProcessing = false;
        }
        );
        while (bProcessing) { yield return null; }
    }


    string GetInviteToken(string sName)
    {
        for(int i = 0; i < m_MemberInvite.invites.Length; i++)
        {
            if(m_MemberInvite.invites[i].displayName == sName)
            {
                return m_MemberInvite.invites[i].inviteToken;
            }
        }
        return "";
    }

    void Log(string sLog)
    {
        Debug.Log("[TestCreateCoven] " + sLog);
    }

}
