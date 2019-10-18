using Raincrow;
using Raincrow.FTF;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text.RegularExpressions;

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

        private List<FTFPointerHand> m_InstantiatedGlow = new List<FTFPointerHand>();
        private System.Action m_OnClose;
        private int m_AnimTweenId;
        private int m_ButtonTweenId;
        private delegate string ReplaceStringDelegate();

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
            BackButtonListener.AddCloseAction(_Close);

            m_OnClose = onComplete;

            //setup screen

            m_Title.text = LocalizeLookUp.GetText("first_tap_info_" + id + "_title");

            string message = LocalizeLookUp.GetText("first_tap_info_" + id + "_desc");
            MatchCollection matches = Regex.Matches(message, @"{[^}]*}", RegexOptions.IgnoreCase);
            List<string> dynamicKeys = new List<string>();
            foreach (Match m in matches)
                dynamicKeys.Add(m.Value);

            if (dynamicKeys.Count > 0)
            {
                var replacers = new Dictionary<string, ReplaceStringDelegate>()
                {
                    { "{nextPoPName}", GetNearbyPoPName },
                    { "{nextPoPTime}", GetNearbyPoPTime }
                };
                foreach(string key in dynamicKeys)
                {
                    if (replacers.ContainsKey(key))
                        message = message.Replace(key, replacers[key].Invoke());
                }
            }

            m_Message.text = message;
            m_Highlight.Show(entry.highlight);

            for (int i = 0; i < m_InstantiatedGlow.Count; i++)
                Destroy(m_InstantiatedGlow[i].gameObject);
            m_InstantiatedGlow.Clear();

            for (int i = 0; i < entry.glow?.Count; i++)
            {
                FTFPointerHand instance;
                if (i == 0)
                {
                    instance = m_Glow;
                }
                else
                {
                    instance = GameObject.Instantiate(m_Glow, m_Glow.transform.parent);
                    m_InstantiatedGlow.Add(instance);
                }

                instance.Show(entry.glow[i]);
            }

            if (entry.glow == null)
                m_Glow.gameObject.SetActive(false);
                
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
            BackButtonListener.RemoveCloseAction();

            m_InputRaycaster.enabled = false;

            LeanTween.cancel(m_AnimTweenId);
            LeanTween.cancel(m_ButtonTweenId);
            m_AnimTweenId = LeanTween.alphaCanvas(m_CanvasGroup, 0f, 0.5f)
                .setOnComplete(() =>
                {
                    m_Canvas.enabled = false;
                    
                    if (FirstTapManager.CompletedAll())
                        Unload();
                })
                .setEaseOutCubic()
                .uniqueId;

            m_OnClose?.Invoke();
        }

        private string GetNearbyPoPName()
        {
            if (UINearbyLocations.CachedLocations != null)
            {
                foreach(var pop in UINearbyLocations.CachedLocations)
                {
                    if (pop.isOpen || pop.isActive)
                        continue;
                    return "<color=#4FD5FF>" + pop.name + "</color>";
                }
            }

            return "<color=#FF3939></color>";
        }

        private string GetNearbyPoPTime()
        {
            if (UINearbyLocations.CachedLocations != null)
            {
                foreach (var pop in UINearbyLocations.CachedLocations)
                {
                    if (pop.isOpen || pop.isActive)
                        continue;
                    return "<color=#4FD5FF>" + Utilities.GetSummonTime(pop.openOn) + "</color>";
                }
            }

            return "<color=#FF3939></color>";
        }
    }
}