using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;
using UnityScene = UnityEngine.SceneManagement.Scene;

namespace Raincrow
{
    public class SceneManager : MonoBehaviour
    {
        public enum Scene
        {
            START = 0,
            LOGIN = 1,
            GAME = 2,
            PLACE_OF_POWER = 3,
        }

        private static SceneManager m_Instance;
        private static SceneManager Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new GameObject().AddComponent<SceneManager>();
                    m_Instance.hideFlags = HideFlags.HideAndDontSave;
                    DontDestroyOnLoad(m_Instance.gameObject);
                }
                return m_Instance;
            }
        }

        private static string[] m_SceneNames = new string[]
        {
            "StartScene",
            "LoginScene",
            "MainScene",
            "PlaceOfPower",
        };

        public static void LoadScene(Scene scene, LoadSceneMode mode)
        {
            string sceneName = m_SceneNames[(int)scene];
            UnityScene unityScene = UnitySceneManager.GetSceneByName(sceneName);

            if (unityScene.isLoaded)
            {
                Debug.LogError("Scene already loaded");
                return;
            }

            UnitySceneManager.LoadScene(sceneName, mode);
        }

        public static void LoadSceneAsync(Scene scene, LoadSceneMode mode, System.Action<float> onProgress, System.Action onComplete)
        {
            string sceneName = m_SceneNames[(int)scene];
            UnityScene unityScene = UnitySceneManager.GetSceneByName(sceneName);

            if (unityScene.isLoaded)
            {
                Debug.LogError(scene.ToString() + " already loaded");
                onProgress?.Invoke(1);
                return;
            }

            AsyncOperation asyncOp = UnitySceneManager.LoadSceneAsync(sceneName, mode);
            asyncOp.completed += op => onComplete?.Invoke();

            Instance.StartCoroutine(AsyncOperationCoroutine(asyncOp, onProgress));
        }

        public static void UnloadScene(Scene scene, System.Action<float> onProgress, System.Action onComplete)
        {
            string sceneName = m_SceneNames[(int)scene];
            UnityScene unityScene = UnitySceneManager.GetSceneByName(sceneName);

            if (unityScene.isLoaded == false)
            {
                Debug.LogError(scene.ToString() + " not loaded");
                onProgress?.Invoke(1);
                return;
            }

            AsyncOperation asyncOp = UnitySceneManager.UnloadSceneAsync(unityScene);
            asyncOp.completed += op => onComplete?.Invoke();

            Instance.StartCoroutine(AsyncOperationCoroutine(asyncOp, onProgress));
        }

        private static IEnumerator AsyncOperationCoroutine(AsyncOperation op, System.Action<float> onProgress)
        {
            while (op.progress < 0.9f)
            {
                onProgress?.Invoke(op.progress);
                yield return null;
            }
            onProgress?.Invoke(1);
            op.allowSceneActivation = true;
        }
    }
}