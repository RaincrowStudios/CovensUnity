using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackButtonListener : MonoBehaviour
{
    // public static event System.Action onPressBackBtn;

    private static Stack<System.Action> m_CloseStack = new Stack<System.Action>();

    public static void AddCloseAction(System.Action close)
    {
        m_CloseStack.Push(close);
    }

    public static void RemoveCloseAction()
    {
        PopStack();
    }

    private static void PopStack()
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
            m_CloseStack.Peek().Invoke();
            PopStack();
        }
    }
}
