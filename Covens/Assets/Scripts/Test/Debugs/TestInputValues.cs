using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInputValues : MonoBehaviour
{
    public string m_String_1;
    public string m_String_2;
    public string m_String_3;

    public static string String1()
    {
        TestInputValues v1 = GameObject.FindObjectOfType<TestInputValues>();
        return v1.m_String_1;
    }
    public static string String2()
    {
        TestInputValues v1 = GameObject.FindObjectOfType<TestInputValues>();
        return v1.m_String_2;
    }
    public static string String3()
    {
        TestInputValues v1 = GameObject.FindObjectOfType<TestInputValues>();
        return v1.m_String_3;
    }
    public static void String1(string sVal)
    {
        TestInputValues v1 = GameObject.FindObjectOfType<TestInputValues>();
        v1.m_String_1 = sVal; ;
    }
    public static void String2(string sVal)
    {
        TestInputValues v1 = GameObject.FindObjectOfType<TestInputValues>();
        v1.m_String_2 = sVal;
    }
    public static void String3(string sVal)
    {
        TestInputValues v1 = GameObject.FindObjectOfType<TestInputValues>();
        v1.m_String_3 = sVal;
    }
}
