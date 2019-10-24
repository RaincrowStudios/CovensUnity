using System.Data.SqlTypes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LocationTutorial : UIInfoPanel
{
    private List<string> tips = new List<string>();
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private CanvasGroup descCG;
    [SerializeField] private Button closeBtn;
    [SerializeField] private Sprite filled;
    [SerializeField] private Sprite empty;
    [SerializeField] private GameObject item;
    [SerializeField] private Transform container;
    private List<Image> circles = new List<Image>();

    private RectTransform rectTransform;
    private SwipeDetector swipeDetector;
    private int currentIndex = 0;
    private static LocationTutorial m_Instance;

    public static LocationTutorial Instance
    {
        get
        {
            if (m_Instance == null)
                m_Instance = Instantiate(Resources.Load<LocationTutorial>("LocationTutorial"));
            return m_Instance;

        }
    }

    public static bool isShowing
    {
        get
        {
            if (m_Instance == null) return false;
            else return m_Instance.m_IsShowing;
        }
    }

    void Start()
    {
        LocationTutorial lt = LocationTutorial.Instance;
        lt.Open();
    }

    protected override void Awake()
    {
        base.Awake();
        tips.Add(DownloadedAssets.LocalizationDictionary["first_pop_lms"]);
        tips.Add(DownloadedAssets.LocalizationDictionary["first_pop_fly"]);
        tips.Add(DownloadedAssets.LocalizationDictionary["first_pop_cloak"]);
        tips.Add(DownloadedAssets.LocalizationDictionary["first_pop_summon"]);
        tips.Add(DownloadedAssets.LocalizationDictionary["first_pop_islands"]);
        tips.Add(DownloadedAssets.LocalizationDictionary["first_pop_guardian"]);
        var title = this.transform.GetChild(0).GetChild(1).GetComponentInChildren<TextMeshProUGUI>();
        title.text = LocalizeLookUp.GetText("school_title") + ": " + LocalizeLookUp.GetText("pop_title");
        descCG = description.GetComponent<CanvasGroup>();
        rectTransform = description.GetComponent<RectTransform>();
        swipeDetector = GetComponent<SwipeDetector>();
        closeBtn.onClick.AddListener(() =>
        {
            swipeDetector.canSwipe = false;
            Close();
        });
        swipeDetector.SwipeLeft += () =>
        {
            OnSwipe(true);
        };

        swipeDetector.SwipeRight += () =>
        {
            OnSwipe(false);
        };
    }

    void OnSwipe(bool direction)
    {
        if (direction)
        {
            currentIndex = currentIndex == tips.Count - 1 ? 0 : ++currentIndex;
        }
        else
        {
            currentIndex = currentIndex == 0 ? tips.Count - 1 : --currentIndex;
        }

        LeanTween.value(1, 0, .2f).setOnUpdate((float v) =>
        {
            if (direction)
                rectTransform.anchoredPosition = new Vector2(Mathf.Lerp(-400, 0, v), rectTransform.anchoredPosition.y);
            else
                rectTransform.anchoredPosition = new Vector2(Mathf.Lerp(400, 0, v), rectTransform.anchoredPosition.y);

            descCG.alpha = v;

        }).setOnComplete(() =>
        {
            description.text = tips[currentIndex];
            LeanTween.value(0, 1, .2f).setOnUpdate((float v) =>
            {
                if (direction)
                    rectTransform.anchoredPosition = new Vector2(Mathf.Lerp(400, 0, v), rectTransform.anchoredPosition.y);
                else
                    rectTransform.anchoredPosition = new Vector2(Mathf.Lerp(-400, 0, v), rectTransform.anchoredPosition.y);

                descCG.alpha = v;

            }).setEaseInOutQuad();
        }).setEaseInOutQuad();

        UpdateCircles();

    }

    public void Open()
    {
        if (isShowing) return;
        swipeDetector.canSwipe = true;
        description.text = tips[currentIndex];
        if (circles.Count != tips.Count)
        {
            foreach (Transform item in container)
            {
                Destroy(item.gameObject);
            }
            foreach (var t in tips)
            {
                var g = Utilities.InstantiateObject(item, container);
                g.SetActive(true);
                var i = g.GetComponent<Image>();
                circles.Add(i);
            }

        }
        UpdateCircles();
        Show();
    }

    void UpdateCircles()
    {
        for (int i = 0; i < circles.Count; i++)
        {
            if (circles[i] != null)
            {
                if (i == currentIndex)
                    circles[i].sprite = filled;
                else
                    circles[i].sprite = empty;
            }
        }
    }

}
