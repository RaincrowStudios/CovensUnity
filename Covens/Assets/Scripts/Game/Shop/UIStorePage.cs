using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Raincrow.Store;

public class UIStorePage : MonoBehaviour
{
    [SerializeField] private UIStoreItem m_ItemPrefab;
    public RectTransform rectTransform { get; private set; }

    private void Awake()
    {
        rectTransform = this.GetComponent<RectTransform>();
    }

    public void UpdateItems()
    {

    }

    public void LoadAssets()
    {

    }

    //public void UnloadAssets()
    //{

    //}
}
