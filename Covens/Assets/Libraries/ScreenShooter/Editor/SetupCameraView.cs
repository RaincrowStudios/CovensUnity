using UnityEditor;
using System;
using UnityEditor.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupCameraView : MonoBehaviour
{
    public const string GameViewName = "CardsScreenshot";
    int m_iWidth;
    int m_iHeight;


    /*
    public void SetupCameraView()/*
    {
        // add sizes        
        //int iWidth = m_pArguments.iWidth;
        //int iHeight = m_pArguments.iHeight;
        string sViewName = GameViewName + "_" + m_iWidth + "x" + m_iHeight;
        GameViewSizeGroupType[] ePlatforms = new[] { GameViewSizeGroupType.Android, GameViewSizeGroupType.iOS, GameViewSizeGroupType.Standalone };
        foreach (GameViewSizeGroupType ePlatform in ePlatforms)
        {
            GameViewUtils.AddCustomSize(GameViewUtils.GameViewSizeType.FixedResolution, ePlatform, m_iWidth, m_iHeight, sViewName);
        }

        // set to created size
        GameViewSizeGroupType eGroup = GameViewSizeGroupType.Standalone;
#if UNITY_ANDROID
        eGroup = GameViewSizeGroupType.Android;
#elif UNITY_IOS
            eGroup = GameViewSizeGroupType.iOS;
#endif
        Debug.Log("GROUP: " + eGroup);
        GameViewUtils.SetSize(GameViewUtils.FindSize(eGroup, sViewName));
    }*/



}