using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class CircleScroll : MonoBehaviour
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
    public List<CollectableItem> invItems = new List<CollectableItem>();

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
        transform.localEulerAngles = Vector3.zero;
        invItems = PlayerDataManager.playerData.ingredients.toolsDict.Values.ToList();
        length = invItems.Count;
        if (length < count)
        {
            int diff = count - length;
            Debug.Log(diff);
            for (int i = 0; i < diff; i++)
            {
                var iT = new CollectableItem();
                iT.collectible = "null";
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

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //canRotate = false;
            //			PointerEventData ped = new PointerEventData (null);
            //			ped.position = Input.mousePosition;
            //			List<RaycastResult> results = new List<RaycastResult> ();
            //			EventSystem.current.RaycastAll(ped, results);
            //			foreach (var item in results) {
            ////				Debug.Log (item.gameObject.tag);
            //					if (item.gameObject.tag == "Tool") {
            //						canRotate = true;
            //					} 
            //				if (item.gameObject.tag == "Gem") {
            //					canRotate = false;
            //					return;
            //				}
            //			}
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

        foreach (var item in items)
        {
            if (movementSpeed < 0 && canRotate)
            {
                if (item.Value.rotation.eulerAngles.y > 90 && item.Value.rotation.eulerAngles.y < 270)
                {
                    item.Value.Rotate(0, 0, 180);
                    var iData = item.Value.GetComponent<InventoryItemManager>();
                    int k = iData.Index;
                    k -= count;
                    if (k < 0)
                    {
                        k = length + k;
                    }
                    iData.Setup(invItems[k].count, invItems[k].collectible, k);
                }
            }
            if (movementSpeed > 0 && canRotate)
            {
                if (item.Value.rotation.eulerAngles.y > 90 && item.Value.rotation.eulerAngles.y < 270)
                {
                    item.Value.Rotate(0, 0, 180);
                    var iData = item.Value.GetComponent<InventoryItemManager>();
                    int k = iData.Index;
                    k += count;
                    if (k > length - 1)
                    {
                        k = k - length;
                    }
                    iData.Setup(invItems[k].count, invItems[k].collectible, k);
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

    IEnumerator WheelInertiaNegetive()
    {
        while (movementSpeed < 0)
        {
            movementSpeed += Time.deltaTime * slowDownSpeed;
            yield return 0;
        }
        movementSpeed = 0;
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
            g.GetComponent<InventoryItemManager>().Setup(invItems[i].count, invItems[i].collectible, i);
        }
        transform.Rotate(0, 0, -83);
    }

    public static bool IsMouseInsideCircle(Canvas canvas, Transform centerRef, Transform minRadiusRef, Transform maxRadiusRef)
    {
        Camera cam = canvas.worldCamera;
        Vector2 mousePos = Input.mousePosition;
        Vector2 ccenterCenter = RectTransformUtility.WorldToScreenPoint(cam, centerRef.position);
        Vector2 minRadius = RectTransformUtility.WorldToScreenPoint(cam, minRadiusRef.position);
        Vector2 maxRadius = RectTransformUtility.WorldToScreenPoint(cam, maxRadiusRef.position);

        float pos = Mathf.Sqrt(Mathf.Pow(mousePos.x - ccenterCenter.x, 2) + Mathf.Pow(mousePos.y - ccenterCenter.y, 2));
        float min = Mathf.Sqrt(Mathf.Pow(minRadius.x - ccenterCenter.x, 2) + Mathf.Pow(minRadius.y - ccenterCenter.y, 2));
        float max = Mathf.Sqrt(Mathf.Pow(maxRadius.x - ccenterCenter.x, 2) + Mathf.Pow(maxRadius.y - ccenterCenter.y, 2));
        bool inside = pos >= min && pos < max;

        return inside;
    }
}

