using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Raincrow.Store;

public class UIBundleButton : MonoBehaviour
{
    [SerializeField] private GameObject m_OwnedOverlay;
    [SerializeField] private Image m_PackIcon;
    [SerializeField] private Button m_Button;
    [SerializeField] private TextMeshProUGUI m_Text;
    [SerializeField] private GameObject m_Glow;

    private string m_PackId;
    private int m_TimerTweenId;

    private void Awake()
    {
        m_Button.onClick.AddListener(OnClick);
    }

    private void OnDestroy()
    {
        LeanTween.cancel(m_TimerTweenId);
    }

    public void Setup(string packId)
    {
        m_PackId = packId;
        PackData data = StoreManagerAPI.GetPackData(m_PackId);

        var expireTimer = Utilities.TimespanFromJavaTime(data.expiresOn);
        if (data.expiresOn != 0 && expireTimer.TotalSeconds <=0)
        {
            gameObject.SetActive(false);
            return;
        }

        DownloadedAssets.GetSprite(
            m_PackId + "_icon",
            spr => m_PackIcon.overrideSprite = spr,
            true
        );

        bool owned = PlayerDataManager.playerData.OwnedPacks.Contains(packId);

        m_OwnedOverlay.SetActive(data.isFree && owned);
        m_Glow.SetActive(!data.isFree && owned);
        m_Button.interactable = !owned;

        if (data.isFree)
            m_Text.text = owned ? LocalizeLookUp.GetText("store_gear_owned_upper") : LocalizeLookUp.GetText("ftf_claim");
        else
            UpdateTimer(data.expiresOn);

        gameObject.SetActive(true);
    }

    private void OnClick()
    {
        UIBundlePopup.Open(m_PackId, () => Setup(m_PackId));
    }

    private void UpdateTimer(double timestamp)
    {
        var timespan = Utilities.TimespanFromJavaTime(timestamp);
        if (timespan.TotalSeconds <= 0)
        {
            gameObject.SetActive(false);
            return;
        }
        m_Text.text = Utilities.GetSummonTime(timestamp);
        m_TimerTweenId = LeanTween.value(0, 0, 0).setDelay(1).setOnStart(() => UpdateTimer(timestamp)).uniqueId;
    }
}
