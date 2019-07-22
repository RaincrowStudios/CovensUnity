using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIKytelerGrid : MonoBehaviour
{
    [SerializeField] private CanvasGroup m_Container;
    [SerializeField] private LayoutGroup m_ItemContainer;
    [SerializeField] private UIKytelerButton m_ButtonPrefab;
    [SerializeField] private GameObject m_Loading;

    public bool isOpen { get; private set; }
        
    private void Awake()
    {
        m_Container.gameObject.SetActive(false);
    }

    public void Show()
    {
        SetLoading(true);
        KytelerManager.GetKnownRings(OnGetRingsCallback);

        m_Container.gameObject.SetActive(true);
        isOpen = true;
    }

    private void OnGetRingsCallback(int result, List<KytelerItem> rings)
    {
        if(result == 200)
        {
            Dictionary<string, KytelerItem> ringsDict = new Dictionary<string, KytelerItem>();
            foreach (KytelerItem item in rings)
                ringsDict.Add(item.id, item);
            Setup(KytelerManager.GetAllRings(), ringsDict);
        }
        else
        {
            //TODO: parse error code
            Close();
        }
        SetLoading(false);
    }

    private void Setup(KytelerData[] rings, Dictionary<string, KytelerItem> known)
    {
        for (int i = rings.Length; i < m_ItemContainer.transform.childCount; i++)
            m_ItemContainer.transform.GetChild(i).gameObject.SetActive(false);

        for (int i = 0; i < rings.Length; i++)
        {
            int index = i;
            UIKytelerButton ringButton;

            if (i < m_ItemContainer.transform.childCount)
                ringButton = m_ItemContainer.transform.GetChild(i).GetComponent<UIKytelerButton>();
            else
                ringButton = Instantiate(m_ButtonPrefab, m_ItemContainer.transform);

            KytelerData data = rings[index];
            KytelerItem info = known.ContainsKey(data.id) ? known[data.id] : null;

            ringButton.Setup(
                data: data,
                info: info,
                onClick: () =>
                {
                    UIKytelerInfo.Instance.Show(data, info);
                },
                onClickClose: () =>
                {

                }
            );

            ringButton.SetEquiped(false);
        }
    }

    public void Close()
    {
        m_Container.gameObject.SetActive(false);
        isOpen = false;
    }

    public void SetLoading(bool loading)
    {
        if (m_Loading != null)
        {
            m_Loading.gameObject.SetActive(loading);
        }
    }
}
