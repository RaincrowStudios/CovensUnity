using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashboardToolbox : MonoBehaviour
{
    public GameObject ButtonTemplate;
    public PanelInstance PanelTemplate;

    public int m_iInstances = 0;
    public List<GameObject> m_Buttons = new List<GameObject>();
    public Dictionary<int, PanelInstance> m_Players = new Dictionary<int, PanelInstance>();


    // Use this for initialization
    void Start()
    {
        PanelTemplate.gameObject.SetActive(false);
        ButtonTemplate.GetComponentInChildren<Button>().onClick.AddListener(() => { OnClickCreate(); });
    }


    public void OnClickCreate()
    {
        GameObject pNewGo = GameObject.Instantiate(ButtonTemplate, ButtonTemplate.transform.parent);
        PanelInstance pPanel = GameObject.Instantiate<PanelInstance>(PanelTemplate, PanelTemplate.transform.parent);
        pPanel.gameObject.SetActive(true);

        int iPlayer = m_iInstances;
        CreatePlayer(iPlayer, pNewGo, pPanel);
        m_iInstances++;
    }


    public void CreatePlayer(int iPlayer, GameObject pNewGo, PanelInstance pPanel)
    {
        m_Players.Add(iPlayer, pPanel);
        m_Buttons.Add(pNewGo);


        pPanel.Index = iPlayer;
        pPanel.m_Toolbox = this;
        pNewGo.GetComponent<Button>().onClick.RemoveAllListeners();
        pNewGo.GetComponent<Button>().onClick.AddListener(() =>
        {
            OnClickPlayer(iPlayer);
        });

        OnClickPlayer(iPlayer);
    }
    public void OnClickPlayer(int iIndex)
    {
        // will select the player
        foreach(var kvp in m_Players)
        {
            kvp.Value.gameObject.SetActive(kvp.Key == iIndex);
        }
    }
    public void SetText(int iIndex, string sText)
    {
        m_Buttons[iIndex].GetComponentInChildren<Text>().text = sText;
    }


    // Update is called once per frame
    void Update()
    {

    }
}