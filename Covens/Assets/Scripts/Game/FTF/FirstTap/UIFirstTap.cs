using Raincrow;
using Raincrow.FTF;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Raincrow.FTF
{
    public class UIFirstTap : MonoBehaviour
    {
        [SerializeField] private Canvas m_Canvas;
        [SerializeField] private GraphicRaycaster m_InputRaycaster;
        [SerializeField] private CanvasGroup m_CanvasGroup;

        [SerializeField] private FTFHighlight m_Highlight;
        [SerializeField] private FTFPointerHand m_Glow;

        [SerializeField] private TextMeshProUGUI m_Title;
        [SerializeField] private TextMeshProUGUI m_Message;
        [SerializeField] private Button m_ContinueButton;
               
        private static UIFirstTap m_Instance;

        private List<GameObject> m_InstantiatedGlow = new List<GameObject>();
        private System.Action m_OnClose;
        private int m_AnimTweenId;
        private int m_ButtonTweenId;
        
        public static void Show(string id, FirstTapEntry entry, System.Action onComplete)
        {
            if (m_Instance != null)
            {
                m_Instance._Show(id, entry, onComplete);
            }
            else
            {
                LoadingOverlay.Show();
                SceneManager.LoadSceneAsync(SceneManager.Scene.FIRST_TAP, UnityEngine.SceneManagement.LoadSceneMode.Additive, null, () =>
                {
                    LoadingOverlay.Hide();
                    m_Instance._Show(id, entry, onComplete);
                });
            }
        }

        public static void Unload()
        {
            SceneManager.UnloadScene(SceneManager.Scene.FIRST_TAP, null, null);
        }

        private void Awake()
        {
            m_Instance = this;
            m_CanvasGroup.alpha = 0;
            m_Canvas.enabled = false;
            m_InputRaycaster.enabled = false;
            m_ContinueButton.onClick.AddListener(_Close);
        }

        private void _Show(string id, FirstTapEntry entry, System.Action onComplete)
        {
            m_OnClose = onComplete;

            //setup screen
            m_Title.text = LocalizeLookUp.GetText("first_tap_info_" + id + "_title");
            m_Message.text = LocalizeLookUp.GetText("first_tap_info_" + id + "_desc");
            m_Highlight.Show(entry.highlight);

            for(int i = 0; i < entry.glow?.Count; i++)
            {
                FTFPointerHand instance;
                if (i == 0)
                {
                    instance = m_Glow;
                }
                else
                {
                    instance = GameObject.Instantiate(m_Glow, m_Glow.transform.parent);
                    m_InstantiatedGlow.Add(instance.gameObject);
                }

                instance.Show(entry.glow[i]);
            }
                
            LeanTween.cancel(m_AnimTweenId);
            LeanTween.cancel(m_ButtonTweenId);

            //aniamte screen
            m_Canvas.enabled = true;
            m_InputRaycaster.enabled = true;
            m_ContinueButton.interactable = false;

            m_AnimTweenId = LeanTween.alphaCanvas(m_CanvasGroup, 1f, 1f).setEaseOutCubic().uniqueId;
            m_ButtonTweenId = LeanTween.value(0, 0, 0.5f).setOnComplete(() => m_ContinueButton.interactable = true).uniqueId;
        }

        private void _Close()
        {
            m_InputRaycaster.enabled = false;

            LeanTween.cancel(m_AnimTweenId);
            LeanTween.cancel(m_ButtonTweenId);
            m_AnimTweenId = LeanTween.alphaCanvas(m_CanvasGroup, 0f, 0.5f)
                .setOnComplete(() =>
                {
                    m_Canvas.enabled = false;

                    for (int i = 0; i < m_InstantiatedGlow.Count; i++)
                        Destroy(m_InstantiatedGlow[i]);

                    if (FirstTapManager.CompletedAll())
                        Unload();
                })
                .setEaseOutCubic()
                .uniqueId;
            m_OnClose?.Invoke();
            //m_OnClose = null;
        }
    }
}