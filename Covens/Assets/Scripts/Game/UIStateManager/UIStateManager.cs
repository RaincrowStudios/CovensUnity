using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class UIStateManager : MonoBehaviour
{
    public static UIStateManager Instance { get; set; }
	//!!False turns UI off, true turns UI on!!\\

    public delegate void WindowChanged(bool isMainWindow);
    public static event WindowChanged windowChanged;
    public static bool isMain = true;
    public void CallWindowChanged(bool isMain)
    {
        if (windowChanged != null)
            windowChanged(isMain);
    }

    public CanvasGroup[] DisableButtons;

    void Start()
    {
        windowChanged += SetMainUI;
    }

    void Awake()
    {
        Instance = this;
    }

    void SetMainUI(bool isMain)
    {
        StartCoroutine(SetMainUIHelper(isMain));
    }

    IEnumerator SetMainUIHelper(bool isMainUI)
    {
		yield return 0;
        isMain = isMainUI;
        foreach (var item in DisableButtons)
        {
            if (item)
                item.interactable = isMainUI;
        }
        if (isMainUI)
        {
            try
            {
                if (MapSelection.marker != null)
                {
                    MapsAPI.Instance.RemoveMarker(MapSelection.marker);
                }
            }
            catch { }


        }
    }

}

