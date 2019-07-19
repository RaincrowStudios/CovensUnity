using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DebugMoveTo : MonoBehaviour
{
    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private GraphicRaycaster m_InputRaycaster;
    [SerializeField] private RectTransform m_Panel;

    [SerializeField] private Button m_DebugButton;
    [SerializeField] private Button m_ToggleParticlesButton;
    [SerializeField] private Button m_KillMemoryButton;
    [SerializeField] private Button m_SkipTutorialButton;

    [SerializeField] private Button m_MoveButton;
    [SerializeField] private Button m_ResetButton;
    [SerializeField] private TMP_InputField m_Longitude;
    [SerializeField] private TMP_InputField m_Latitude;

    private bool m_Showing = false;
    private bool m_Particles = true;
    private int m_TweenId;

    private void Awake()
    {
#if PRODUCTION
        Destroy(this.gameObject);
#else
        DontDestroyOnLoad(this.gameObject);

        m_Canvas.enabled = false;
        m_InputRaycaster.enabled = false;
        m_Panel.anchoredPosition = new Vector2(0, 0);

        m_DebugButton.onClick.AddListener(OnClickOpen);

        m_KillMemoryButton.onClick.AddListener(ForceLowMemory);
        m_SkipTutorialButton.onClick.AddListener(FTFManager.SkipFTF);

        m_MoveButton.onClick.AddListener(() =>
        {
            double longitude = double.Parse(m_Longitude.text);
            double latitude = double.Parse(m_Latitude.text);

            MarkerManagerAPI.GetMarkers((float)longitude, (float)latitude, null, true, false, true);
        });

        m_ResetButton.onClick.AddListener(() =>
        {
            m_Longitude.text = "-122.3224";
            m_Latitude.text = "47.70168";
        });

        m_ToggleParticlesButton.onClick.AddListener(() =>
        {
            string toggled = "";
            string skipped = "";
            int toggleCount = 0;
            int skipedCount = 0;

            Object[] particles = Resources.FindObjectsOfTypeAll(typeof(ParticleSystem));
            m_Particles = !m_Particles;

            foreach(ParticleSystem _obj in particles)
            {
#if UNITY_EDITOR
                if (UnityEditor.EditorUtility.IsPersistent(_obj.transform.root.gameObject))
                {
                    skipped += _obj.transform.name + "\n";
                    skipedCount += 1;
                    continue;
                }
#endif
                if (!_obj.gameObject.activeInHierarchy)
                    continue;

                if (m_Particles)
                    _obj.Play();
                else
                    _obj.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);

                toggled += _obj.transform.name + "\n";
                toggleCount += 1;
            }
            Debug.Log($"{(m_Particles? "Enabled" : "Disabled")} {toggleCount} particle systems:\n{toggled}");
            Debug.Log($"Ignored {skipedCount} particle systems:\n{skipped}");
        });
#endif
    }

    private bool m_GeneratingTextures = false;
    private void ForceLowMemory()
    {
        if (m_GeneratingTextures)
        {
            Debug.LogError("already killing the memory");
            return;
        }

        if (DownloadedAssets.UnloadingMemory)
        {
            Debug.LogError("already unloading");
            return;
        }

        m_GeneratingTextures = true;
        StartCoroutine(ForceLowMemoryCoroutine());
    }

    private IEnumerator ForceLowMemoryCoroutine()
    {
        List<Texture2D> textures = new List<Texture2D>();

        System.Action onLowMemory = () =>
        {
            m_GeneratingTextures = false;
        };

        DownloadedAssets.OnWillUnloadAssets += onLowMemory;

        while (m_GeneratingTextures)
        {
            textures.Add(new Texture2D(2048, 2048));
            Debug.Log(textures.Count + " textures");
            yield return 0;
        }

        DownloadedAssets.OnWillUnloadAssets -= onLowMemory;
    }

    private void OnClickOpen()
    {
        LeanTween.cancel(m_TweenId);

        m_Showing = !m_Showing;

        if (m_Showing)
            m_Canvas.enabled = true;

        float start = m_Showing ? 0 : 1;

        m_TweenId = LeanTween.value(start, 1 - start, 0.25f)
        .setEaseOutCubic()
        .setOnUpdate((float t) =>
        {
            m_Panel.anchoredPosition = new Vector2(-m_Panel.sizeDelta.x * t, 0);
        })
        .setOnComplete(() =>
        {
            m_Canvas.enabled = m_InputRaycaster.enabled = m_Showing;
        })
        .uniqueId;
    }

    private float m_Delta;
    private float m_LastPosition;
    private float m_StartTime;

    private void Update()
    {
        if (Application.isEditor && Input.GetKeyDown(KeyCode.BackQuote))
        {
            OnClickOpen();
        }
        else if (Input.GetMouseButtonDown(0))
        {
            if (Input.GetKey(KeyCode.LeftAlt) || Input.touchCount == 3)
            {
                m_Delta = 0;
                m_LastPosition = Input.mousePosition.x;
                m_StartTime = Time.time;
            }
        }
        else if (Input.GetMouseButton(0))
        {
            if (Input.GetKey(KeyCode.LeftAlt) || Input.touchCount == 3)
            {
                m_Delta += Input.mousePosition.x - m_LastPosition;
                m_LastPosition = Input.mousePosition.x;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (Mathf.Abs(m_Delta) > Screen.width / 8f && Time.time - m_StartTime < 0.3f)
            {
                m_Delta = 0;
                OnClickOpen();
            }
        }
    }
}
