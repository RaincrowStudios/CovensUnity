using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct PotionSprite
{
    public string potionId;
    public Sprite sprite;
}

public class UIApothecary : MonoBehaviour
{
    public static UIApothecary Instance { get; private set; }

    [Header("Setup")]
    [SerializeField] private UIItemsScroll m_pWheel;
    [SerializeField] private CanvasGroup m_pCanvasGroup;
    [SerializeField] private PotionSprite[] m_vSprites;

    [Header("UI")]
    [SerializeField] private Text m_pDescriptionText;
    [SerializeField] private Text m_pConsumeText;
    [SerializeField] private Button m_pConsumeButton;
    [SerializeField] private Button m_pCloseButton;
    [SerializeField] private Button m_pInventoryButton;
    [SerializeField] private TeamConfirmPopUp m_pConfirmPopup;
    [SerializeField] private GameObject m_pLoading;
    
    private Dictionary<string, Sprite> m_pPotionSprites;
    private List<UIApothecaryItem> Items;
    private int m_iTextTweenId;
        
    private void Awake()
    {
        Instance = this;

        //init the sprites dicitionary
        m_pPotionSprites = new Dictionary<string, Sprite>();
        foreach (PotionSprite entry in m_vSprites)
            m_pPotionSprites.Add(entry.potionId, entry.sprite);

        m_pWheel.OnChangeSelected = OnSelectionChange;
        m_pCanvasGroup.alpha = 0;

        m_pCloseButton.onClick.AddListener(OnClickClose);
        m_pInventoryButton.onClick.AddListener(OnClickReturn);
        m_pConsumeButton.onClick.AddListener(OnClickConsume);

        m_pCanvasGroup.gameObject.SetActive(false);
        m_pInventoryButton.gameObject.SetActive(false);
    }
    
    public void Show()
    {
        //setup the ui wheel
        LoadPotions(PlayerDataManager.playerData.inventory.consumables);

        //select the middle item
        m_pWheel.SetSelected(Mathf.CeilToInt(PlayerDataManager.playerData.inventory.consumables.Count / 2f));

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

        float fDelay = 0.2f;
        Items[iIndex].FadeContent(1f, 1f, fDelay, LeanTweenType.easeOutCubic);
        while (iLeft >= 0 || iRight < Items.Count)
        {
            fDelay += 0.2f;

            if (iLeft >= 0)
                Items[iLeft].FadeContent(0.7f, 1f, fDelay, LeanTweenType.easeOutCubic);
            if (iRight < Items.Count)
                Items[iRight].FadeContent(0.7f, 1f, fDelay, LeanTweenType.easeOutCubic);

            iLeft -= 1;
            iRight += 1;
        }

        //move the inventory wheel
        InventoryTransitionControl.Instance.OpenApothecary();

        //main view
        Image pAuxImage = m_pInventoryButton.image;
        Color pAuxColor = pAuxImage.color;
        LeanTween.value(m_pCanvasGroup.alpha, 1f, 0.4f)
            .setOnUpdate((float value) =>
            {
                pAuxColor.a = value;
                pAuxImage.color = pAuxColor;
                m_pCanvasGroup.alpha = value;
            })
            .setEaseOutCubic();
        m_pInventoryButton.gameObject.SetActive(true);
        m_pCanvasGroup.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.3f);
    }

    public void Return()
    {
        StopAllCoroutines();
        InventoryTransitionControl.Instance.ReturnFromApothecary();
        StartCoroutine(AnimateOutCoroutine());
    }

    public void Close()
    {
        StopAllCoroutines();
        StartCoroutine(AnimateOutCoroutine());
    }

    private IEnumerator AnimateOutCoroutine()
    { 
        //main view
        Image pAuxImage = m_pInventoryButton.image;
        Color pAuxColor = pAuxImage.color;
        LeanTween.value(m_pCanvasGroup.alpha, 0f, 0.2f)
            .setOnUpdate((float value) =>
            {
                pAuxColor.a = value;
                pAuxImage.color = pAuxColor;
                m_pCanvasGroup.alpha = value;
            })
            .setOnComplete(() =>
            {
                m_pCanvasGroup.gameObject.SetActive(false);
                m_pInventoryButton.gameObject.SetActive(false);
            })
            .setEaseOutCubic();
        yield return 0;
    }

    public void LoadPotions(List<ConsumableItem> consumables)
    {
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
            m_pDescriptionText.text = Items[index].ConsumableData.onBuyDescription;
            m_pConsumeText.text = "Consume (" + Items[index].Consumable.count + ")";
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

        m_pConfirmPopup.ShowPopUp(
            confirmAction: () => 
            {
                var data = new { consumable = Items[m_pWheel.SelectedIndex].Consumable.id };
                APIManager.Instance.PostData("inventory/consume", JsonConvert.SerializeObject(data), OnConsumeResponse);
                m_pLoading.SetActive(true);
            },
            cancelAction: () => 
            {
                m_pConsumeButton.interactable = true;
            },
            txt: "confirmation text"
        );
    }

    private void OnConsumeResponse(string response, int result)
    {
        if (result == 200)
        {
            m_pConfirmPopup.ShowPopUp(
                () => {
                    Items[m_pWheel.SelectedIndex].Consumable.count -= 1;
                    m_pConsumeText.text = "Consume (" + Items[m_pWheel.SelectedIndex].Consumable.count + ")";
                },
                Items[m_pWheel.SelectedIndex].ConsumableData.onConsumeDescription
            );
        }
        else
        {
            string sError = "code " + result;
            m_pConfirmPopup.Error(sError);
        }

        m_pConsumeButton.interactable = Items[m_pWheel.SelectedIndex].Consumable.count > 0;
        m_pLoading.SetActive(false);
    }

    private void OnClickReturn()
    {
        InventoryTransitionControl.Instance.ReturnFromApothecary();
        Close();
    }

    private void OnClickClose()
    {
        InventoryTransitionControl.Instance.CloseApothecary();
        Close();
    }

    public Sprite GetPotionSprite(string consumableId)
    {
        if (m_pPotionSprites.ContainsKey(consumableId))
            return m_pPotionSprites[consumableId];

        return null;
    }
}
