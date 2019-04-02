using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class CircleScrollHerb : MonoBehaviour
{
    public float scrollSpeed = 2;
    public float slowDownSpeed = 2;
    private float lastDistance;
    private float speed;
    float movementSpeed;
    public GameObject prefab;
    public Dictionary<int, Transform> items = new Dictionary<int, Transform>();
    public int count = 12;
    public int offset = 15;
    bool canRotate;
    int length = 20;
    public List<InventoryItems> invItems = new List<InventoryItems>();

    [Header("hit detection")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private Transform circleCenterRef;
    [SerializeField] private Transform minRadiusRef;
    [SerializeField] private Transform maxRadiusRef;

    void Start()
    {

    }

    void OnEnable()
    {
        invItems = PlayerDataManager.playerData.ingredients.herbsDict.Values.ToList();
        length = invItems.Count;
        transform.localEulerAngles = Vector3.zero;
        if (length < count)
        {
            int diff = count - length;
            Debug.Log(diff);
            for (int i = 0; i < diff; i++)
            {
                var iT = new InventoryItems();
                iT.id = "null";
                invItems.Add(iT);
            }
        }
        length = invItems.Count;

        foreach (var item in items)
        {
            Destroy(item.Value.gameObject);
        }
        Spawn();
    }

    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            //canRotate = false;
            //PointerEventData ped = new PointerEventData (null);
            //ped.position = Input.mousePosition;
            //List<RaycastResult> results = new List<RaycastResult> ();
            //EventSystem.current.RaycastAll(ped, results);
            //foreach (var item in results) {

            //	if (item.gameObject.tag == "Herb") {
            //		canRotate = true;
            //	} 
            //	if (item.gameObject.tag == "Tool") {
            //		canRotate = false;
            //		return;
            //	}
            //}

            canRotate = CircleScroll.IsMouseInsideCircle(canvas, circleCenterRef, minRadiusRef, maxRadiusRef);
            if (canRotate == false)
                return;

            this.StopAllCoroutines();
            movementSpeed = 0;
            lastDistance = Input.mousePosition.y;
        }
        if (Input.GetMouseButton(0) && canRotate)
        {
            speed = (Mathf.Abs(Input.mousePosition.y) - Mathf.Abs(lastDistance)) / Time.deltaTime;
            lastDistance = Input.mousePosition.y;
            movementSpeed = speed * Time.deltaTime * scrollSpeed;
        }
        if (Input.GetMouseButtonUp(0) && canRotate)
        {
            StartCoroutine(WheelInertia());
        }
        if (movementSpeed != 0 && canRotate)
        {
            transform.Rotate(0, 0, movementSpeed);
        }

        ManageItems();
    }

    void ManageItems()
    {
        foreach (var item in items)
        {
            if (movementSpeed < 0 && canRotate)
            {
                if (item.Value.rotation.eulerAngles.y > 40 && item.Value.rotation.eulerAngles.y < 320)
                {
                    item.Value.Rotate(0, 0, 80);
                    var iData = item.Value.GetComponent<InventoryItemManager>();
                    int k = iData.Index;
                    k -= count;
                    if (k < 0)
                    {
                        k = length + k;
                    }
                    iData.Setup(invItems[k].count, invItems[k].id, k);
                }
            }
            if (movementSpeed > 0 && canRotate)
            {
                if (item.Value.rotation.eulerAngles.y > 40 && item.Value.rotation.eulerAngles.y < 320)
                {
                    item.Value.Rotate(0, 0, 280);
                    var iData = item.Value.GetComponent<InventoryItemManager>();
                    int k = iData.Index;
                    k += count;
                    if (k > length - 1)
                    {
                        k = k - length;
                    }
                    iData.Setup(invItems[k].count, invItems[k].id, k);
                }
            }
        }
    }

    IEnumerator WheelInertia()
    {
        float t = 0;
        while (t <= 1)
        {
            t += Time.deltaTime * slowDownSpeed;
            movementSpeed = Mathf.SmoothStep(movementSpeed, 0, t);
            yield return 0;
        }
    }

    void Spawn()
    {
        for (int i = 0; i < count; i++)
        {
            var g = Instantiate(prefab, transform);
            g.transform.localPosition = Vector3.zero;
            g.transform.localEulerAngles = new Vector3(0, 0, i * offset);
            g.transform.transform.localScale = Vector3.one;
            items[i] = g.transform;
            g.GetComponent<InventoryItemManager>().Setup(invItems[i].count, invItems[i].id, i);
        }
        transform.Rotate(0, 0, -40);
    }
}

