using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using TMPro;
public class FAQManager : MonoBehaviour
{
    public TextAsset json;

    public GameObject cellPrefab;
    public Transform container;
    public struct QAdata
    {
        public string Question;
        public string Answer;
        public float cellSize;
    }
    private List<FAQData> faqs = new List<FAQData>();
    private QAdata[] m_data; 
    private List<QAdata> m_data_updated;
    public TMP_InputField inputField;
    // Start is called before the first frame update
    void Start()
    {
        m_data = JsonConvert.DeserializeObject<QAdata[]>(json.text);

        m_data_updated = new List<QAdata>(m_data);

        while (faqs.Count < m_data.Length)
        {
            var k = Utilities.InstantiateObject(cellPrefab, container);
            var t = k.GetComponent<FAQData>();
            faqs.Add(t);
        }

        Create();

        inputField.onValueChanged.AddListener(s =>
        {
            if (s.Length == 0)
            {
                m_data_updated = new List<QAdata>(m_data);
                Create();
            }
            else
            {
                Disable();
                m_data_updated.Clear();
                foreach (var item in m_data)
                {
                    if (item.Question.ToLower().Contains(s.ToLower()))
                    {
                        m_data_updated.Add(item);
                    }
                }
                Create();
            }
        });
    }

    private void Create()
    {
        for (int i = 0; i < m_data_updated.Count; i++)
        {
            faqs[i].gameObject.SetActive(true);
            faqs[i].SetData(m_data_updated[i]);
        }
    }



    private void Disable()
    {
        foreach (var item in faqs)
        {
            item.gameObject.SetActive(false);
        }
    }
}
