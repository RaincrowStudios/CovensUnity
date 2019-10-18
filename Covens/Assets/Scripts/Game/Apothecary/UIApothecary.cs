using Newtonsoft.Json;
using Raincrow.GameEventResponses;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIApothecary : MonoBehaviour
{
    private static UIApothecary m_Instance;
    public static UIApothecary Instance
    {
        get
        {
            if (m_Instance == null)
                m_Instance = Instantiate(Resources.Load<UIApothecary>("UIApothecary"));
            return m_Instance;
        }
    }

    [Header("Setup")]
    [SerializeField] private UIItemsScroll m_pWheel;
    [SerializeField] private CanvasGroup m_pCanvasGroup;

    [Header("UI")]
    [SerializeField] private Text m_pDescriptionText;
    [SerializeField] private Text m_pConsumeText;
    [SerializeField] private Button m_pConsumeButton;
    [SerializeField] private Button m_pCloseButton;
    [SerializeField] private Button m_pInventoryButton;
    [SerializeField] private GameObject m_pLoading;
    
    private List<UIApothecaryItem> Items;
    private int m_iTextTweenId;
    private System.Action m_OnOpen;
    private System.Action m_OnReturn;
    private System.Action m_OnClose;

    private void Awake()
    {
        m_Instance = this;

        m_pWheel.OnChangeSelected = OnSelectionChange;
        m_pCanvasGroup.alpha = 0;
        m_pCanvasGroup.interactable = false;
        m_pCanvasGroup.blocksRaycasts = false;

        m_pCloseButton.onClick.AddListener(OnClickClose);
        m_pInventoryButton.onClick.AddListener(OnClickReturn);
        m_pConsumeButton.onClick.AddListener(OnClickConsume);

        m_pCanvasGroup.gameObject.SetActive(false);
        m_pInventoryButton.gameObject.SetActive(false);

        m_pDescriptionText.text = "";
        m_pConsumeText.text = "";

        DownloadedAssets.OnWillUnloadAssets += OnWillUnloadAssets;
    }

    private void OnWillUnloadAssets()
    {
        if (m_pCanvasGroup.gameObject.activeSelf)
            return;

        DownloadedAssets.OnWillUnloadAssets -= OnWillUnloadAssets;
        LeanTween.cancel(m_iTextTweenId);
        StopAllCoroutines();
        Destroy(this.gameObject);
    }

    public void Show(System.Action onOpen, System.Action onReturn, System.Action onClose)
    {
        BackButtonListener.AddCloseAction(OnClickReturn);

        m_OnOpen = onOpen;
        m_OnReturn = onReturn;
        m_OnClose = onClose;

        //setup the ui wheel
        LoadPotions(PlayerDataManager.playerData.inventory.consumables);

        //select the middle item
        m_pWheel.SetSelected(Mathf.RoundToInt((PlayerDataManager.playerData.inventory.consumables.Count - 1) / 2f));

        //animate the ui
        StartCoroutine(AnimateInCoroutine());
    }

    private IEnumerator AnimateInCoroutine()
    {
        //set all potions transparent
        for (int i = 0; i < Items.Count; i++)
            Items[i].FadeContent(0, 0);
        yield return 1;

        //fade potions in by steps
        int iIndex = m_pWheel.SelectedIndex;
        int iLeft = iIndex - 1;
        int iRight = iIndex + 1;

        m_pWheel.transform.localScale = Vector3.zero;
        LeanTween.scale(m_pWheel.gameObject, Vector3.one, 1.5f).setEaseOutCubic();

        float fDelay = 0.4f;
        if (iIndex >= 0 && iIndex < Items.Count)
            Items[iIndex].FadeContent(1f, 1.5f, fDelay, LeanTweenType.easeOutCubic);

        while (iLeft >= 0 || iRight < Items.Count)
        {
            fDelay += 0.2f;

            if (iLeft >= 0)
                Items[iLeft].FadeContent(0.7f, 2f, fDelay, LeanTweenType.easeOutCubic);
            if (iRight < Items.Count)
                Items[iRight].FadeContent(0.7f, 2f, fDelay, LeanTweenType.easeOutCubic);

            iLeft -= 1;
            iRight += 1;
        }

        //move the inventory wheel
        //InventoryTransitionControl.Instance.OpenApothecary();

        //main view
        Image pAuxImage = m_pInventoryButton.image;
        Color pAuxColor = pAuxImage.color;
        LeanTween.value(m_pCanvasGroup.alpha, 1f, 0.4f)
            .setOnUpdate((float value) =>
            {
                pAuxColor.a = value;
                pAuxImage.color = pAuxColor;
                m_pCanvasGroup.alpha = value;
                m_pCanvasGroup.transform.localScale = new Vector3(value, value, value);
            })
            .setOnComplete(() =>
            {
                m_OnOpen?.Invoke();
            })
            .setEaseOutCubic();
        m_pInventoryButton.gameObject.SetActive(true);
        m_pCanvasGroup.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.2f);

        m_pCanvasGroup.interactable = true;
        m_pCanvasGroup.blocksRaycasts = true;
        m_pInventoryButton.interactable = true;
    }

    private IEnumerator AnimateOutCoroutine(float duration, LeanTweenType easeType, float delay = 0)
    {
        m_pInventoryButton.interactable = false;
        m_pCanvasGroup.interactable = false;
        yield return new WaitForSeconds(delay);
        m_pCanvasGroup.blocksRaycasts = false;

        //main view
        Image pAuxImage = m_pInventoryButton.image;
        Color pAuxColor = pAuxImage.color;
        LeanTween.value(m_pCanvasGroup.alpha, 0f, duration)
            .setOnUpdate((float value) =>
            {
                pAuxColor.a = value;
                pAuxImage.color = pAuxColor;
                m_pCanvasGroup.alpha = value;
                m_pCanvasGroup.transform.localScale = new Vector3(value, value, value);
            })
            .setOnComplete(() =>
            {
                m_pCanvasGroup.gameObject.SetActive(false);
                m_pInventoryButton.gameObject.SetActive(false);
            })
            .setEase(easeType);
        yield return 0;
    }

    public void LoadPotions(List<Item> consumables)
    {
        //remove empty
        for (int i = 0; i < consumables.Count; i++)
        {
            if (consumables[i].count <= 0)
            {
                consumables.RemoveAt(i);
                i--;
            }
        }

        consumables.Sort((a, b) => b.id.CompareTo(a.id));

        //load the wheel with the appropriate amount of prefabs:
        m_pWheel.Load(consumables.Count);

        //setup each item
        Items = new List<UIApothecaryItem>();
        for (int i = 0; i < consumables.Count; i++)
        {
            int idx = i;
            m_pWheel.Items[i].Setup(consumables[i]);
            Items.Add(m_pWheel.Items[i] as UIApothecaryItem);
        }
    }

    private void OnSelectionChange(int index)
    {
        //fade and update the texts
        LeanTween.cancel(m_iTextTweenId);

        Color pTextColor = m_pDescriptionText.color;
        System.Action pOnFinishTween = () =>
        {
            m_pDescriptionText.text = LocalizeLookUp.GetStoreDesc(Items[index].Consumable.id);
            m_pConsumeText.text = LocalizeLookUp.GetText("consume_amount").Replace("{{Count}}", Items[index].Consumable.count.ToString());
            m_pConsumeButton.interactable = Items[m_pWheel.SelectedIndex].Consumable.count > 0;

            m_iTextTweenId = LeanTween.value(0, 1, 1f)
                .setEaseOutCubic()
                .setOnUpdate((float value) =>
                {
                    pTextColor.a = value;
                    m_pConsumeText.color = m_pDescriptionText.color = pTextColor;
                })
                .uniqueId;
        };

        m_iTextTweenId = LeanTween.value(pTextColor.a, 0f, 0.2f)
            .setOnUpdate((float value) =>
            {
                pTextColor.a = value;
                m_pConsumeText.color = m_pDescriptionText.color = pTextColor;
            })
            .setOnComplete(pOnFinishTween)
            .uniqueId;
    }
    
    private void OnClickConsume()
    {
        m_pConsumeButton.interactable = false;

        UIGlobalPopup.ShowPopUp(
            confirmAction: (System.Action)(() =>
            {
                //var data = new { consumable = Items[m_pWheel.SelectedIndex].Consumable.id };
                APIManager.Instance.Post("character/consume/" + Items[m_pWheel.SelectedIndex].Consumable.id, "{}", OnConsumeResponse);
                m_pLoading.SetActive(true);
            }),
            cancelAction: () =>
            {
                m_pConsumeButton.interactable = true;
            },
            txt: LocalizeLookUp.GetText("ui_drink_potion")// "Drink the potion?"
        );
    }

    private void OnConsumeResponse(string response, int result)
    {
        m_pLoading.SetActive(false);

        if (result == 200)
        {
            UIGlobalPopup.ShowPopUp(
                () => {
                    Items[m_pWheel.SelectedIndex].Consumable.count -= 1;
                    m_pConsumeButton.interactable = Items[m_pWheel.SelectedIndex].Consumable.count > 0;
                    m_pConsumeText.text = LocalizeLookUp.GetText("consume_amount").Replace("{{Count}}", Items[m_pWheel.SelectedIndex].Consumable.count.ToString());// + ")";
                },
                LocalizeLookUp.GetStoreDesc(Items[m_pWheel.SelectedIndex].Consumable.id)
            );

            StatusEffect effect = JsonConvert.DeserializeObject<StatusEffect>(response);
            PlayerConditionManager.OnPlayerApplyStatusEffect?.Invoke(effect);
        }
        else
        {
            UIGlobalPopup.ShowError(null, APIManager.ParseError(response));
        }
    }

    private void OnClickReturn()
    {
        BackButtonListener.RemoveCloseAction();

        m_OnReturn?.Invoke();
        StopAllCoroutines();
        StartCoroutine(AnimateOutCoroutine(0.4f, LeanTweenType.linear, 0.1f));
    }

    private void OnClickClose()
    {
        BackButtonListener.RemoveCloseAction();

        m_OnClose?.Invoke();
        StopAllCoroutines();
        StartCoroutine(AnimateOutCoroutine(0.3f, LeanTweenType.easeOutSine));
    }
}
