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
            LOGIN,
            GAME,
            PLACE_OF_POWER,
            COVEN_MANAGEMENT,
            DAILY_QUESTS,
            SPELLCAST_BOOK,
            CHAT,
            STORE,
            DAILY_BLESSING,
            GREY_HAND_OFFICE,
            QUICKCAST,
            PLAYER_SELECT,
            SPIRIT_SELECT,
            NEARBY_POPS,
            SETTINGS,
            WITCH_SCHOOL,
            VIDEO_PLAYER,
            POPUP,
            FTF,
            SUMMONING,
            FIRST_TAP,
            WARDROBE,
            STORE_BUNDLE,
            REVIEW_POPUP,
            EXPLORE_LORE,
            HELP,
        }

        private static Dictionary<Scene, string> m_SceneNames = new Dictionary<Scene, string>
        {
            {Scene.START,               "StartScene" },
            {Scene.LOGIN,               "LoginScene" },
            {Scene.GAME,                "MainScene"},
            {Scene.PLACE_OF_POWER,      "PlaceOfPower"},
            {Scene.COVEN_MANAGEMENT,    "CovenManagement" },
            {Scene.DAILY_QUESTS,        "DailyQuests" },
            {Scene.SPELLCAST_BOOK,      "SpellcastBook" },
            {Scene.CHAT,                "Chat" },
            {Scene.STORE,               "Store" },
            {Scene.DAILY_BLESSING,      "DailyBlessing" },
            {Scene.GREY_HAND_OFFICE,    "GreyHandOffice" },
            {Scene.QUICKCAST,           "QuickCast" },
            {Scene.PLAYER_SELECT,       "PlayerSelect" },
            {Scene.SPIRIT_SELECT,       "SpiritSelect" },
            {Scene.NEARBY_POPS,         "NearbyPops" },
            {Scene.SETTINGS,            "Settings" },
            {Scene.WITCH_SCHOOL,        "WitchSchool" },
            {Scene.VIDEO_PLAYER,        "VideoPlayer" },
            {Scene.POPUP,               "Popup" },
            {Scene.FTF,                 "FTF" },
            {Scene.SUMMONING,           "Summoning" },
            {Scene.FIRST_TAP,           "FirstTap" },
            {Scene.WARDROBE,            "Wardrobe" },
            {Scene.STORE_BUNDLE,        "StoreBundle" },
            {Scene.REVIEW_POPUP,        "ReviewPopup" },
            {Scene.EXPLORE_LORE,        "ExploreLore" },
            {Scene.HELP,                "Help" },
        };

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

        public static void LoadScene(Scene scene, LoadSceneMode mode)
        {
            string sceneName = m_SceneNames[scene];
            UnityScene unityScene = UnitySceneManager.GetSceneByName(sceneName);

            if (unityScene.isLoaded)
            {
                Debug.LogError("Scene already loaded");
                return;
            }

            UnitySceneManager.LoadScene(sceneName, mode);
        }

        public static AsyncOperation LoadSceneAsync(Scene scene, LoadSceneMode mode, System.Action<float> onProgress, System.Action onComplete)
        {
            string sceneName = m_SceneNames[scene];
            UnityScene unityScene = UnitySceneManager.GetSceneByName(sceneName);

            if (unityScene.isLoaded)
            {
                Debug.Log(scene.ToString() + " already loaded");
                onProgress?.Invoke(1);
                onComplete?.Invoke();
                return null;
            }

            AsyncOperation asyncOp = UnitySceneManager.LoadSceneAsync(sceneName, mode);
            asyncOp.completed += op =>
            {
                onComplete?.Invoke();
            };
            Instance.StartCoroutine(AsyncOperationCoroutine(asyncOp, onProgress));
            return asyncOp;
        }

        public static void UnloadScene(Scene scene, System.Action<float> onProgress, System.Action onComplete)
        {
            string sceneName = m_SceneNames[scene];
            UnityScene unityScene = UnitySceneManager.GetSceneByName(sceneName);

            if (unityScene.isLoaded == false)
            {
                Debug.LogError(scene.ToString() + " not loaded");
                onProgress?.Invoke(1);
                return;
            }

            AsyncOperation asyncOp = UnitySceneManager.UnloadSceneAsync(unityScene);
            //asyncOp.allowSceneActivation = true;
            asyncOp.completed += op => onComplete?.Invoke();
            Instance.StartCoroutine(AsyncOperationCoroutine(asyncOp, onProgress));
        }

        public static bool IsSceneLoaded(Scene scene)
        {
            string sceneName = m_SceneNames[scene];
            UnityScene unityScene = UnitySceneManager.GetSceneByName(sceneName);
            return unityScene.isLoaded;
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