using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackButtonListener : MonoBehaviour
{
    // public static event System.Action onPressBackBtn;

    private static Stack<System.Action> m_CloseStack = new Stack<System.Action>();

    private bool m_IsExitPrompt = false;

    public static void AddCloseAction(System.Action close)
    {
        m_CloseStack.Push(close);
    }



    public static void RemoveCloseAction()
    {
        if (m_CloseStack.Count > 0)
        {
            m_CloseStack.Pop();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (m_CloseStack.Count > 0)
            {
                m_CloseStack.Peek().Invoke();
                m_CloseStack.Pop();
            }
            else
            {
                if (!m_IsExitPrompt)
                {
                    UIGlobalPopup.ShowPopUp(() =>
                    {
                        Application.Quit();
                    }, () => { m_IsExitPrompt = false; }, "Are you sure you want to exit the game?");
                    m_IsExitPrompt = true;
                }
                else
                {
                    Debug.Log("application quit");
                    Application.Quit();
                }
            }
        }
    }
}
