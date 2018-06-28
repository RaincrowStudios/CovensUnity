using UnityEditor;
using System;
using UnityEditor.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// setups the camera view to screnshots
/// </summary>
public class SetupCameraView 
{
    public const string GameViewName = "Screenshot";
    static GameViewSizeGroupType[] ePlatforms = new[] { GameViewSizeGroupType.Android, GameViewSizeGroupType.iOS, GameViewSizeGroupType.Standalone };



    static GameViewSizeGroupType CurrentGroup
    {
        get
        {
            GameViewSizeGroupType eGroup = GameViewSizeGroupType.Standalone;
#if UNITY_ANDROID
            eGroup = GameViewSizeGroupType.Android;
#elif UNITY_IOS
            eGroup = GameViewSizeGroupType.iOS;
#endif
            return eGroup;
        }
    }


    public static void Set(int iWidth, int iHeight)
    {
        string sViewName = string.Format("{0}_{1}x{2}", GameViewName, iWidth, iHeight);
        foreach (GameViewSizeGroupType ePlatform in ePlatforms)
        {
            GameViewUtils.AddCustomSize(GameViewUtils.GameViewSizeType.FixedResolution, ePlatform, iWidth, iHeight, sViewName);
        }
        Debug.Log("GROUP: " + CurrentGroup);
        GameViewUtils.SetSize(GameViewUtils.FindSize(CurrentGroup, sViewName));
    }


    [MenuItem("Tools/Set Screenshot View Landscape")]
    public static void SetScreenshotViewLandscape()
    {
        Set(1280, 800);
    }
    [MenuItem("Tools/Set Screenshot View Portrait")]
    public static void SetScreenshotViewPortrait()
    {
        Set(800, 1280);
    }

}