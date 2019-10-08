using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FTFLocationController : MonoBehaviour
{
    public float rotateSpeed = 2f;
    public float zoomOutSpeed = 1f;
    public List<Transform> markers;
    public Transform[] movableMarkers;
    public Transform[] slots;
    Transform centerSpirit;
    public Transform[] trail;
    public Transform[] hits;
    public Transform[] charge;
    public GameObject[] highlights;
    public TextMeshPro textPopUp;
    public Camera camera;
    public Transform cameraRotation;
    public Transform cameraTilt;
    public Transform cameraZoom;
    public GameObject flyFX;
    bool otherAttack = true;
    public Transform spiritFX;

    public Button nextBtn;
    public TextMeshProUGUI dialogueTxt;
    public GameObject videoFX;
    bool shouldRotate = true;

    public static event System.Action OnVideoCompleted;

    // Start is called before the first frame update
    void Start()
    {
        nextBtn.transform.parent.gameObject.SetActive(false);
        LeanTween.value(0, 1, 3f).setOnUpdate((float v) =>
        {
            cameraZoom.localPosition = new Vector3(0, 0, Mathf.Lerp(-250, -850, v));
            cameraTilt.localRotation = Quaternion.Euler(Mathf.Lerp(17, 30, v), 0, 0);
        }).setEaseOutQuad().setOnComplete(() =>
        {
            nextBtn.transform.parent.gameObject.SetActive(true);
            StartCoroutine(HandleAttack());
            nextBtn.onClick.AddListener(() =>
            {
                dialogueTxt.text = "Savannah Grey: \"This Place of Power is a Last Witch Standing battle, they must defeat the Guardian and each other to be victorious.\"";
                nextBtn.transform.parent.gameObject.SetActive(false);
                StartCoroutine(ActivateSpirit());
            });
        });
        centerSpirit = markers[0];
        moveMarkers(0);

        LoadFakeMarkers();
    }

    IEnumerator ActivateSpirit()
    {
        yield return new WaitForSeconds(1);
        spiritFX.gameObject.SetActive(true);
        HighlightShift();

        LeanTween.value(0, 1, .8f).setOnUpdate((float v) =>
        {
            spiritFX.localScale = Vector3.one * v;
        }).setOnComplete(() =>
        {
            StartCoroutine(SpiritAttack());
            nextBtn.onClick.RemoveAllListeners();

            nextBtn.transform.parent.gameObject.SetActive(true);

            var id = LeanTween.value(1, 0, 4.5f).setOnUpdate((float v) =>
            {
                otherAttack = false;
                Time.timeScale = Mathf.Lerp(.05f, 1, v);
                centerSpirit.localScale = Vector3.one * Mathf.Lerp(5, 3, v);
                cameraZoom.localPosition = new Vector3(0, 0, Mathf.Lerp(-620, -850, v));
                cameraTilt.localRotation = Quaternion.Euler(Mathf.Lerp(23, 30, v), 0, 0);
            }).setEaseOutQuad().id;

            nextBtn.onClick.AddListener(() =>
            {
                videoFX.SetActive(true);
                LeanTween.cancel(id);
                Time.timeScale = 1;
                shouldRotate = false;
                id = LeanTween.value(0, 0, 8).setOnComplete(() => OnVideoCompleted?.Invoke()).uniqueId;
            });

        });
    }

    IEnumerator SpiritAttack()
    {
        for (int i = 0; i < 15; i++)
        {
            yield return new WaitForSeconds(.1f);
            Attack(true);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (shouldRotate)
        {
            cameraRotation.Rotate(0, rotateSpeed * Time.deltaTime, 0);
            updateMarkers();
        }
    }

    void updateMarkers()
    {
        foreach (var item in markers)
        {
            item.rotation = camera.transform.rotation;
        }
    }

    IEnumerator HandleAttack()
    {
        yield return new WaitForSeconds(Random.Range(.5f, 2));

        int times = Random.Range(2, 7);

        for (int i = 0; i < times; i++)
        {
            Attack();
            yield return new WaitForSeconds(Random.Range(.1f, .4f));
        }
        if (otherAttack)
            StartCoroutine(HandleAttack());
    }
    void HighlightShift()
    {
        foreach (var item in highlights)
        {
            var highlight0 = item.transform.GetChild(0).gameObject;
            var highlight1 = item.transform.GetChild(1).gameObject;
            var highlight2 = item.transform.GetChild(2).gameObject;
            LeanTween.color(highlight0, Color.red, 1f);
            LeanTween.color(highlight1, Color.red, 1f);
            LeanTween.color(highlight2, Color.red, 1f);
        }
    }
    void Attack(bool isCenterSpirit = false)
    {
        markers.Shuffle();

        int color = Random.Range(0, 3);
        if (isCenterSpirit) color = 2;
        Transform tr = Instantiate(trail[color]);
        tr.localScale = Vector3.one * 3.5f;
        Vector3 endControl = markers[1].position;
        endControl.x += Random.Range(-130, 130);
        endControl.y += Random.Range(-5, 130);
        endControl.z += Random.Range(-130, 130);

        Vector3 startControl = markers[0].position;

        if (isCenterSpirit)
            startControl = centerSpirit.position;

        startControl.x += Random.Range(-130, 130);
        startControl.y += Random.Range(-5, 130);
        startControl.z += Random.Range(-130, 130);


        Vector3 startPos = markers[0].position;


        if (isCenterSpirit)
            startPos = centerSpirit.position;
        startPos.y += 40;
        Vector3 endPos = markers[1].position;
        endPos.y += 40;
        LTBezierPath ltPath = new LTBezierPath(new Vector3[] { startPos, endControl, startControl, endPos });

        LeanTween.value(0, 1, 1f).setOnUpdate((float v) =>
        {
            tr.position = ltPath.point(v);
        }).setOnComplete(() =>
       {
           Destroy(tr.gameObject, 2);
           Transform hit = Instantiate(hits[color], endPos, Quaternion.identity);
           hit.rotation = Quaternion.LookRotation(startPos - endPos);
           var txt = Instantiate(textPopUp, markers[1]);

           var textObj = txt.GetComponent<TextMeshPro>();

           textObj.fontSize = 45;
           textObj.color = new Color(1, 1, 1);
           textObj.transform.localPosition = new Vector3(0, 9, 0);

           textObj.transform.localScale = Vector3.one;
           textObj.transform.localRotation = Quaternion.identity;

           textObj.text = Random.Range(2, 80).ToString();

           textObj.color = Utilities.Red; //make it red for damage
           if (Random.Range(0, 2) > 0)
           {
               textObj.fontSize = 65; //big text for crit
           }
           //animate the text

           var RandomSpacing = new Vector3(Random.Range(-7, 7), textObj.transform.localPosition.y + Random.Range(20, 24), textObj.transform.localPosition.z);
           textObj.transform.Translate(RandomSpacing);
           LeanTween.moveLocalY(textObj.gameObject, textObj.transform.localPosition.y + Random.Range(8, 11), 2f).setEaseOutCubic();
           LeanTween.value(1f, 0f, 2f).setOnUpdate((float a) =>
           {
               if (textObj != null)
                   textObj.alpha = a;
           });

           Destroy(textObj.gameObject, 2);
           Destroy(hit.gameObject, 3);
       });
    }



    async void moveMarkers(int count)
    {
        if (count < slots.Length)
        {
            await Task.Delay(Random.Range(2000, 4000));
            int m = Random.Range(0, movableMarkers.Length);
            Transform t = movableMarkers[m];
            LeanTween.value(1, 0, .55f).setOnUpdate((float v) =>
            {
                t.localScale = Vector3.one * .666f * v;
            }).setEaseOutQuad().setOnComplete(() =>
            {
                flyFX.SetActive(false);
                flyFX.transform.parent = t;
                flyFX.transform.localPosition = Vector3.zero;
                flyFX.transform.localScale = Vector3.one * .4f;
                t.parent = slots[count];
                t.localPosition = Vector3.zero;
                flyFX.SetActive(true);
                LeanTween.value(0, 1, .55f).setOnUpdate((float v) =>
                           {
                               t.localScale = Vector3.one * .666f * v;
                           }).setEaseOutQuad().setOnComplete(() =>
                           {
                               if (otherAttack)
                                   moveMarkers(++count);
                           });
            });
        }

    }


    [Space()]
    [SerializeField] private SpriteRenderer[] m_Spirits;
    [SerializeField] private SpriteRenderer[] m_Witches;

    [ContextMenu("LoadAssets")]
    private void LoadFakeMarkers()
    {
        string[] styles = new string[]
        {
            "f_S_HHO",
            "f_S_HKO",
            "f_S_HLO",
            "f_S_SNOWQUEEN",
            "f_S_LILYOV",
            "f_S_MIDSUMMER",
            "f_S_CALYPSO",
            "m_S_HKO",
            "m_S_HHO",
            "m_S_HLO",
            "m_S_JACKFROST",
            "m_S_MIDSUMMER",
            "m_S_CALYPSO",
            "m_S_FIREDANCER"
        };

        string[] races = new string[] { "A_", "O_", "E_" };

        string[] spirits = new string[]
        {
            "spirit_barghest","spirit_maximon","spirit_alakshmi","spirit_banshee","spirit_ayizan","spirit_quetzalcoatl"
        };

        foreach(var renderer in m_Witches)
        {
            if (renderer == null)
                continue;

            string id = styles[Random.Range(0, styles.Length)].Replace("_S_", "_S_" + races[Random.Range(0, races.Length)]) + "_Relaxed";
            DownloadedAssets.GetSprite(id, spr =>
            {
                renderer.sprite = spr;
            });
        }

        foreach (var renderer in m_Spirits)
        {
            if (renderer == null)
                continue;

            DownloadedAssets.GetSprite(spirits[Random.Range(0, spirits.Length)], spr =>
            {
                renderer.sprite = spr;
            });
        }
    }
}
