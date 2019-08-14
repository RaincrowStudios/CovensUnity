using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Collections;

public class BOSActiveSpirit : MonoBehaviour
{
    [SerializeField] Transform container;
    [SerializeField] GameObject prefab;

    [SerializeField] private GameObject m_Loading;
    [SerializeField] private TextMeshProUGUI m_Error;

    private void Awake()
    {
        m_Error.gameObject.SetActive(false);
        GetActiveSpirits();
    }

    private void GetActiveSpirits()
    {
        m_Loading.SetActive(true);
        APIManager.Instance.Get("character/spirits", (response, result) =>
        {
            if (m_Loading == null)
                return;

            if (result == 200)
            {
                SpiritInstance[] spirits = JsonConvert.DeserializeObject<SpiritInstance[]>(response);
                StartCoroutine(SetupSpiritsCoroutine(spirits));
            }
            else
            {
                m_Error.text = APIManager.ParseError(response);
                m_Error.gameObject.SetActive(true);
            }
            m_Loading.SetActive(false);
        });
    }

    private IEnumerator SetupSpiritsCoroutine(SpiritInstance[] spirits)
    {
        List<BOSActiveSpiritItem> items = new List<BOSActiveSpiritItem>();
        for (int i = 0; i < spirits.Length; i++)
        {
            items.Add(Utilities.InstantiateObject(prefab, container).GetComponent<BOSActiveSpiritItem>());
        }

        for(int i = 0; i < items.Count; i++)
        {
            items[i].Setup(spirits[i]);
            yield return 0;
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}