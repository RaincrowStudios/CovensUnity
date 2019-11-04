using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Raincrow;

public class GreyHandOffice : MonoBehaviour
{

    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private GraphicRaycaster m_InputRaycaster;

    [SerializeField] private Button m_AcceptButton;
    [SerializeField] private Button m_DeclineButton;

    [SerializeField] private Button m_RewardContinueButton;
    [SerializeField] private Button m_WarningCloseButton;

    private static GreyHandOffice m_Instance;

    //
    public GameObject NotToday;
    public Image SilverDrachs;

    public TextMeshProUGUI greyHandOffice;
    public TextMeshProUGUI toolNum;
    public TextMeshProUGUI drachNum;

    public CanvasGroup SavCG;
    public GameObject Sav;

    public CanvasGroup BGCG;

    public CanvasGroup ToolsCG;

    public CanvasGroup DrachsCG;

    public CanvasGroup TextContainer;

    public CanvasGroup WarningTextCont;
    public CanvasGroup WarningBG;

    public GameObject accept;

    public Vector3 SavScale;

    public GameObject close;

    //public Button rewardContinue;
    public Text rewardText;

    private List<CollectableItem> inventoryItems = new List<CollectableItem>();
    private int forbidTool = 0;
    private int forbidToolValue = 0;

    public static void Show(string officeName)
    {
        if (m_Instance != null)
        {
            m_Instance.Open(officeName);
        }
        else
        {
            LoadingOverlay.Show();
            SceneManager.LoadSceneAsync(
                SceneManager.Scene.GREY_HAND_OFFICE,
                UnityEngine.SceneManagement.LoadSceneMode.Additive,
                (progress) =>
                {

                },
                () =>
                {
                    m_Instance.Open(officeName);
                    LoadingOverlay.Hide();
                }
            );
        }
    }

    private void Awake()
    {
        m_Instance = this;

        m_Canvas.enabled = false;
        m_InputRaycaster.enabled = false;

        m_AcceptButton.onClick.AddListener(() =>
        {
            APIManager.Instance.Get("vendors", TurnInCallback);
        });

        m_DeclineButton.onClick.AddListener(() =>
        {
            ShowWarning();
        });
        DownloadedAssets.GetSprite("silver3", spr =>
        {
            SilverDrachs.overrideSprite = spr;
        }, true);


        m_WarningCloseButton.onClick.AddListener(() =>
        {
            m_InputRaycaster.enabled = false;
            LeanTween.alphaCanvas(SavCG, 0f, 0.4f);
            LeanTween.alphaCanvas(TextContainer, 0f, 0.4f);
            LeanTween.alphaCanvas(NotToday.GetComponent<CanvasGroup>(), 0f, 0.4f);
            LeanTween.alphaCanvas(BGCG, 0.01f, 0.5f).setOnComplete(() =>
            {
                LeanTween.alphaCanvas(BGCG, 0f, 0.2f).setOnComplete(() =>
                {
                    m_Canvas.enabled = false;
                });
            });
        });

        m_RewardContinueButton.onClick.AddListener(() =>
        {
            accept.SetActive(false);
            m_Canvas.enabled = false;
            m_InputRaycaster.enabled = false;
            //m_InputRaycaster.enabled = false;
            //LeanTween.alphaCanvas(SavCG, 0f, 0.4f);
            //LeanTween.alphaCanvas(TextContainer, 0f, 0.4f);
            //LeanTween.alphaCanvas(BGCG, 0.01f, 0.5f).setOnComplete(() => 
            //{
            //    LeanTween.alphaCanvas(BGCG, 0f, 0.2f).setOnComplete(() => 
            //    {
            //        m_Canvas.enabled = false;
            //    });
            //});
        });
    }

    private void Open(string officeName)
    {
        TextSetup(officeName);

        m_InputRaycaster.enabled = true;
        m_Canvas.enabled = true;
        InitAnims();
    }

    void InitAnims()
    {
        close.SetActive(false);
        NotToday.SetActive(false);
        accept.SetActive(false);
        WarningBG.alpha = 0;
        SavScale = Vector3.one * 0.3f;
        BGCG.alpha = 0;
        WarningTextCont.alpha = 1;
        WarningTextCont.transform.GetChild(0).GetComponent<CanvasGroup>().alpha = 0;
        WarningTextCont.transform.GetChild(1).GetComponent<CanvasGroup>().alpha = 0;
        SavCG.alpha = 0;
        ToolsCG.alpha = 0;
        DrachsCG.alpha = 0;
        TextContainer.alpha = 0;
        LeanTween.alphaCanvas(BGCG, 1f, 0.5f).setEase(LeanTweenType.easeOutCubic);
        LeanTween.alphaCanvas(TextContainer, 1f, 2f).setEase(LeanTweenType.easeOutCubic);
        Anim();
        Anim2();
    }

    // Update is called once per frame
    void Anim()
    {
        LeanTween.scale(Sav, Vector3.one * 0.7f, 7f).setEase(LeanTweenType.easeOutCubic);
        LeanTween.alphaCanvas(SavCG, 1f, 1f).setEase(LeanTweenType.easeOutCubic).setOnComplete(() =>
        {
            //Anim2();
        });
        //callOnCompletes(Anim2());
        //Invoke ("Anim2", 1f);
    }

    void Anim2()
    {
        LeanTween.alphaCanvas(ToolsCG, 1f, 1f).setEase(LeanTweenType.easeOutCubic);
        LeanTween.alphaCanvas(DrachsCG, 1f, 1f).setEase(LeanTweenType.easeOutCubic);
        //	.setEase (LeanTweenType.easeOutCubic);

    }

    public void ShowWarning()
    {
        close.SetActive(true);

        LeanTween.alphaCanvas(TextContainer, 0f, .5f).setEase(LeanTweenType.easeOutCubic).setOnComplete(() =>
        {
            NotToday.SetActive(true);
            LeanTween.alphaCanvas(NotToday.GetComponent<CanvasGroup>(), 1f, 0.4f);
            LeanTween.alphaCanvas(WarningBG, 1f, 1f).setEase(LeanTweenType.easeOutCubic);
            LeanTween.alphaCanvas(WarningTextCont.transform.GetChild(0).GetComponent<CanvasGroup>(), 1f, 1f).setEase(LeanTweenType.easeInCubic).setOnComplete(() =>
            {
                LeanTween.alphaCanvas(WarningTextCont.transform.GetChild(1).GetComponent<CanvasGroup>(), 1f, 0.5f).setEase(LeanTweenType.easeInCubic);
            });
        });
    }

    public void TextSetup(string officeName)
    {
        forbidTool = 0;
        forbidToolValue = 0;

        List<CollectableItem> tools = PlayerDataManager.playerData.GetAllIngredients(IngredientType.tool);
        foreach (var item in tools)
        {
            IngredientData data = DownloadedAssets.GetCollectable(item.id);
            if (data.forbidden)
            {
                inventoryItems.Add(item);
                forbidTool += item.count;

                int idx = data.rarity - 1;
                if (idx >= 0 && idx < PlayerDataManager.forbiddenValue?.Length)
                {
                    forbidToolValue += PlayerDataManager.forbiddenValue[idx];
                }
                else
                {
                    Debug.LogException(new System.Exception("Invalid raririty [" + data.rarity + "] for tool \"" + item.id + "\""));
                }
            }
        }

        if (forbidTool == 0)
        {
            transform.GetChild(2).GetChild(4).GetComponent<TextMeshProUGUI>().color = Color.red;
            transform.GetChild(2).GetChild(4).GetComponent<Button>().interactable = false;
        }
        else
        {
            transform.GetChild(2).GetChild(4).GetComponent<TextMeshProUGUI>().color = Color.white;
            transform.GetChild(2).GetChild(4).GetComponent<Button>().interactable = true;
        }

        greyHandOffice.text = officeName;
        toolNum.text = forbidTool.ToString();
        drachNum.text = forbidToolValue.ToString();
        rewardText.text = forbidToolValue.ToString();
    }

    void TurnInCallback(string result, int response)
    {
        if (response == 200)
        {
            Debug.Log("turn in was a success");
            accept.SetActive(true);
            foreach (CollectableItem tool in inventoryItems)
                PlayerDataManager.playerData.SetIngredient(tool.id, 0);
            PlayerDataManager.playerData.silver += forbidToolValue;
            PlayerManagerUI.Instance.UpdateDrachs();
        }
        else
        {
            Debug.LogError(result);
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Unload screen")]
    private void Unload()
    {
        SceneManager.UnloadScene(SceneManager.Scene.GREY_HAND_OFFICE, null, null);
    }
#endif
}
