using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRings : MonoBehaviour
{
    [SerializeField] private CanvasGroup m_Container;
    [SerializeField] private LayoutGroup m_ItemContainer;
    [SerializeField] private UIRingButton m_ButtonPrefab;

    public bool isOpen { get; private set; }
        
    private void Awake()
    {
        m_Container.gameObject.SetActive(false);
    }

    public void Show()
    {
        //Setup();
        FakeSetup();
        m_Container.gameObject.SetActive(true);
        isOpen = true;
    }

    private void Setup(List<KytelerData> rings)
    {
        for (int i = rings.Count; i < m_ItemContainer.transform.childCount; i++)
            m_ItemContainer.transform.GetChild(i).gameObject.SetActive(false);

        for (int i = 0; i < rings.Count; i++)
        {
            int index = i;
            UIRingButton ringButton;

            if (i < m_ItemContainer.transform.childCount)
                ringButton = m_ItemContainer.transform.GetChild(i).GetComponent<UIRingButton>();
            else
                ringButton = Instantiate(m_ButtonPrefab, m_ItemContainer.transform);
            
            ringButton.Setup(
                data: rings[index],
                onClick: () =>
                {
                    UIKytelerInfo.Instance.Show(rings[index]);
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
    
    [ContextMenu("FakeSetup")]
    private void FakeSetup()
    {
        List<KytelerData> list = new List<KytelerData>()
        {
            new KytelerData() {
                id = "Ring 01",
                iconId = "icon",
                owned = false
            },
            new KytelerData() {
                id = "Ring 02",
                iconId = "icon",
                owned = true
            },
            new KytelerData() {
                id = "Ring 03",
                iconId = "icon",
                owned = false
            },
            new KytelerData() {
                id = "Ring 04",
                iconId = "icon",
                owned = false
            },
            new KytelerData() {
                id = "Ring 05",
                iconId = "icon",
                owned = false
            },
            new KytelerData() {
                id = "Ring 06",
                iconId = "icon",
                owned = false
            },
            new KytelerData() {
                id = "Ring 07",
                iconId = "icon",
                owned = false
            },
            new KytelerData() {
                id = "Ring 08",
                iconId = "icon",
                owned = true
            }
        };

        Setup(list);
    }
}
