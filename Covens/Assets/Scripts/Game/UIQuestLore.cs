using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Raincrow;

public class UIQuestLore : MonoBehaviour
{
    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private GraphicRaycaster m_InputRaycaster;
    [SerializeField] private CanvasGroup m_CanvasGroup;

    [SerializeField] private Button m_CloseButton;
    [SerializeField] private TextMeshProUGUI m_Title;
    [SerializeField] private TextMeshProUGUI m_Desc;

    private static UIQuestLore m_Instance;
    private int m_AlphaTweenId;

    private void Awake()
    {
        m_Instance = this;
        m_InputRaycaster.enabled = false;
        m_Canvas.enabled = false;
        m_CanvasGroup.alpha = 0;

        m_CloseButton.onClick.AddListener(_Hide);
    }

    public static void Show(string id)
    {
        if (m_Instance == null)
        {
            LoadingOverlay.Show();
            SceneManager.LoadSceneAsync(SceneManager.Scene.EXPLORE_LORE, UnityEngine.SceneManagement.LoadSceneMode.Additive, null, () =>
            {
                LoadingOverlay.Hide();
                m_Instance._Show(id);
            });
        }
        else
        {
            m_Instance._Show(id);
        }
    }

    private void _Show(string id)
    {
        LeanTween.cancel(m_AlphaTweenId);

        m_Title.text = LocalizeLookUp.GetExploreTitle(id);
        m_Desc.text = LocalizeLookUp.GetExploreLore(id);

        m_AlphaTweenId = LeanTween.alphaCanvas(m_CanvasGroup, 1, 0.7f).setEaseOutCubic().uniqueId;
        m_Canvas.enabled = true;
        m_InputRaycaster.enabled = true;
    }

    private void _Hide()
    {
        LeanTween.cancel(m_AlphaTweenId);

        m_InputRaycaster.enabled = false;
        m_AlphaTweenId = LeanTween.alphaCanvas(m_CanvasGroup, 0, 1)
            .setEaseOutCubic()
            .setOnComplete(() =>
            {
                m_Canvas.enabled = false;
                //SceneManager.UnloadScene(SceneManager.Scene.EXPLORE_LORE, null, null);
            })
            .uniqueId;

        if (PlayerDataManager.playerData.quest.explore.completed)
            return;

        QuestsController.CompleteExplore(error =>
        {
            if (string.IsNullOrEmpty(error))
            {

            }
            else
            {
                UIGlobalPopup.ShowError(null, APIManager.ParseError(error));
            }
        });
    }
}
