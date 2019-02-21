using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BOSActiveSpirit : MonoBehaviour
{
    [SerializeField] Transform container;
    [SerializeField] GameObject prefab;

    void Start()
    {
        foreach (Transform item in container)
        {
            Destroy(item.gameObject);
        }
        foreach (var item in BOSSpirit.activeSpiritsData)
        {
            var g = Utilities.InstantiateObject(prefab, container);
            g.GetComponent<BOSActiveSpiritItem>().Setup(item);
        }
    }
}