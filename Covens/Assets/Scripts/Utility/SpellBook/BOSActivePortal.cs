using UnityEngine;

public class BOSActivePortal : MonoBehaviour
{
    [SerializeField] Transform container;
    [SerializeField] GameObject prefab;

    void Start()
    {

        foreach (Transform item in container)
        {
            Destroy(item.gameObject);
        }
        foreach (var item in BOSSpirit.activePortalsData)
        {
            var g = Utilities.InstantiateObject(prefab, container);
            g.GetComponent<BOSActivePortalItem>().Setup(item);
        }
    }
}