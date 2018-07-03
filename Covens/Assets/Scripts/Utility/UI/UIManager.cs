using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Patterns.SingletonComponent<UIManager>
{
    private List<UIBase> m_UIList;

    List<UIBase> UIList
    {
        get
        {
            if (m_UIList == null)
                m_UIList = ScanForUIs();
            return m_UIList;
        }
    }


    // Use this for initialization
    public override void Awake()
    {
        base.Awake();
        Application.targetFrameRate = 60;
    }


    private List<UIBase> ScanForUIs()
    {
        List<UIBase> vUIList = new List<UIBase>();// GameObject.FindObjectsOfType<UIBase>());
        Canvas[] vCanvas = GameObject.FindObjectsOfType<Canvas>();
        foreach (var pCanvas in vCanvas)
        {
            MonoBehaviour[] vMono = pCanvas.GetComponentsInChildren<MonoBehaviour>(true);
            foreach (var pMono in vMono)
            {
                if (pMono is UIBase && !vUIList.Contains((UIBase)pMono))
                {
                    vUIList.Add((UIBase)pMono);
                }
            }
        }
        return vUIList;
    }


    #region main methods

    /// <summary>
    /// gets the first found UIBase instance in scene
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T Get<T>() where T: UIBase
    {
        foreach(UIBase pUI in Instance.UIList)
        {
            if (pUI is T)
                return (T)pUI;
        }
        return null;
    }
    /// <summary>
    /// shows the first found UIBase instance in scene
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T Show<T>() where T : UIBase
    {
        foreach (UIBase pUI in Instance.UIList)
        {
            if (pUI is T)
            {
                pUI.Show();
                return (T)pUI;
            }
        }
        return null;
    }
    /// <summary>
    /// closes the first found UIBase instance in scene
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T Close<T>() where T : UIBase
    {
        foreach (UIBase pUI in Instance.UIList)
        {
            if (pUI is T)
            {
                pUI.Close();
                return (T)pUI;
            }
        }
        return null;
    }

    #endregion

}
