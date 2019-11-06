using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Raincrow;

[RequireComponent(typeof(Canvas))]
public class EmptyTestScene : MonoBehaviour
{
    public Button m_TestSceneButton;
    public Button m_MainSceneButton_A;
    public Button m_MainSceneButton_B;
    public Button m_StoreSceneButton;
    public Button m_CloseButton;

    private EmptyTestScene m_Instance;
    private Canvas m_Canvas;
    private bool m_IsOpen => m_Canvas.enabled;

    private void Awake()
    {
        if (m_Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        m_Instance = this;
        DontDestroyOnLoad(this.gameObject);
        m_Canvas = this.GetComponent<Canvas>();

        m_CloseButton.onClick.AddListener(() => m_Canvas.enabled = false);

        m_TestSceneButton.onClick.AddListener(() => UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("EmptyTestScene"));

        m_MainSceneButton_A.onClick.AddListener(() => UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("MainScene"));

        m_MainSceneButton_B.onClick.AddListener(() =>
        {
            SceneManager.LoadSceneAsync(
                SceneManager.Scene.GAME,
                UnityEngine.SceneManagement.LoadSceneMode.Single,
                (progress) => Debug.Log(progress),
                () => Debug.Log("game loaded"));
        });

        m_StoreSceneButton.onClick.AddListener(() =>
        {
            UIStore.OpenStore();
        });
    }

    private void Update()
    {
        if (!m_IsOpen)
        {
            if (Input.touchCount >= 2 || Input.GetKeyDown(KeyCode.Z))
                m_Canvas.enabled = true;
        }
    }
}
