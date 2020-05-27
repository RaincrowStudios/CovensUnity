using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ElixirStoreUI : MonoBehaviour
{
    [Header("Infos")]
    [SerializeField] private Image _elixirImage;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _ownText;
    [SerializeField] private TextMeshProUGUI _currencyText;
    [SerializeField] private Image _currencyImage;

    [Header("Buttons")]
    [SerializeField] private Button _buttonConsume;
    [SerializeField] private Button _buttonPurchase;

    [Header("TimeExpire")]
    [SerializeField] private GameObject _timeToExpireObject;
    [SerializeField] private TextMeshProUGUI _timeToExpireText;

    int amountElixir;
    public void Setup(ElixirDataStore data, System.Action<string, string, Button> OnClickConsume, System.Action<Raincrow.Store.StoreItem, int, Sprite> OnClickPurcharse)
    {
        _nameText.text = data.ElixirName;
        _currencyText.text = data.Price.ToString();
        _elixirImage.sprite = data.IconElixir;
        _currencyImage.sprite = data.IconCurrency;

        _ownText.text = LocalizeLookUp.GetText("store_you_own").Replace("{{amount}}", data.OwnedAmount.ToString());
        amountElixir = data.OwnedAmount;

        _buttonConsume.interactable = data.OwnedAmount != 0;
        _timeToExpireObject.SetActive(data.ExpiresOn > 0);

        _buttonConsume.onClick.AddListener(()=>OnClickConsume(data.id, data.spellId, _buttonConsume));

        Raincrow.Store.StoreItem storeItem = new Raincrow.Store.StoreItem {
            id = data.id,
            gold = data.PriceType == ElixirDataStore.CurrencyType.gold ? data.Price : 0,
            silver = data.PriceType == ElixirDataStore.CurrencyType.silver ? data.Price : 0,
            spellId = data.spellId
        };

        _buttonPurchase.onClick.AddListener(() =>
            OnClickPurcharse(storeItem, amountElixir, data.IconElixir)
        ); ;

        UpdateTimer(data.ExpiresOn);
    }

    public void UpdateElixirs(int amount)
    {
        amountElixir += amount;
        _ownText.text = LocalizeLookUp.GetText("store_you_own").Replace("{{amount}}", amountElixir.ToString());
        _buttonConsume.interactable = amountElixir != 0;
    }

    public int GetAmount()
    {
        return amountElixir;
    }

    public void UpdateTimer(double expiresOn)
    {
        _timeToExpireObject.SetActive(expiresOn != 0);

        if(expiresOn != 0)
        {
            StopCoroutine(UpdateTimerCoroutine(expiresOn));

            StartCoroutine(UpdateTimerCoroutine(expiresOn));
        }
    }

    private IEnumerator UpdateTimerCoroutine(double expiresOn)
    {
        while (true)
        {
            if (expiresOn == 0)
            {
                _timeToExpireObject.SetActive(false);
                break;
            }
            else
            {
                System.TimeSpan timespan = Utilities.TimespanFromJavaTime(expiresOn);

                if (timespan.TotalSeconds <= 0)
                {
                    _timeToExpireObject.SetActive(false);
                    break;
                }
                else
                {
                    if (timespan.TotalDays >= 2)
                        _timeToExpireText.text = timespan.Days + " " + LocalizeLookUp.GetText("lt_time_days");
                    else if (timespan.TotalDays > 1)
                        _timeToExpireText.text = timespan.Days + " " + LocalizeLookUp.GetText("lt_time_day");
                    else if (timespan.TotalHours >= 1)
                        _timeToExpireText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", timespan.Hours, timespan.Minutes, timespan.Seconds);
                    else if (timespan.TotalMinutes >= 1)
                        _timeToExpireText.text = string.Format("{0:D2}:{1:D2}", timespan.Minutes, timespan.Seconds);
                    else
                        _timeToExpireText.text = string.Format("{0:D2}:{1:D2}", 0, timespan.Seconds);
                }
            }
            yield return new WaitForSeconds(1f);
        }
    }
}

public struct ElixirDataStore
{

    public enum CurrencyType
    {
        gold = 0,
        silver = 1
    }


    public string id;
    public string spellId;
    public string ElixirName;
    public int OwnedAmount;
    public int Price;
    public Sprite IconElixir;
    public Sprite IconCurrency;
    public CurrencyType PriceType;
    public double ExpiresOn;
}

