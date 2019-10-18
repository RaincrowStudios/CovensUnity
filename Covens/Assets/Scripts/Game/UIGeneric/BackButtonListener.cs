using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackButtonListener : MonoBehaviour
{
    private static Stack<System.Action> m_CloseStack = new Stack<System.Action>();
    public static BackButtonListener Instance { get; private set; }

    public static event System.Action<int> onPressBackBtn;
        
    public static void AddCloseAction(System.Action close)
    {
        m_CloseStack.Push(close);
    }
        
    public static void RemoveCloseAction()
    {
        if (m_CloseStack.Count > 0)
            m_CloseStack.Pop();
    }
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            int stackCount = m_CloseStack.Count;

            if (stackCount > 0)
                m_CloseStack.Peek()?.Invoke();

            onPressBackBtn?.Invoke(stackCount);
        }
    }
}
