using UnityEngine;
using UnityEngine.PostProcessing;

public class DomeControl : MonoBehaviour
{
    [SerializeField] private GameObject dome;
    private Transform centerPoint;
    [SerializeField] private float domeEnableRadius;
    [SerializeField] private float boundsRadius;
    [SerializeField] private float time;
    private MapController MC;
    private bool isInside = true;
    [SerializeField] private Color baseFogColor;
   // [SerializeField] private PostProcessingBehaviour CC;
    private void Start()
    {
        dome.SetActive(false);
        MC = MapController.Instance;

        centerPoint = MapController.Instance.m_StreetMap.cameraCenter;
        MC.m_StreetMap.OnChangePosition += ManageDome;
        MC.m_StreetMap.OnChangePosition += ManageBounds;
    }

    void ManageDome()
    {
        if (!MC.isStreet)
        {
            if (dome.activeInHierarchy)
                dome.SetActive(false);
        }
        else
        {
            if (Vector3.Distance(centerPoint.position, PlayerManager.marker.gameObject.transform.position) > domeEnableRadius)
            {
                if (!dome.activeInHierarchy)
                    dome.SetActive(true);
            }
            else
            {
                if (dome.activeInHierarchy)
                    dome.SetActive(false);
            }
        }
    }

    void ManageBounds()
    {
        if (isInside)
        {
            if (Vector3.Distance(centerPoint.position, PlayerManager.marker.gameObject.transform.position) > boundsRadius)
            {
//                CC.enabled = true;
//                var prof = CC.profile;
//                var temperature = prof.colorGrading.settings;
//                LeanTween.value(0, -37.0f, time).setOnUpdate((float f) =>
//                {
//                    temperature.basic.temperature = f;
//                    prof.colorGrading.settings = temperature;
//                });
//                var contrast = prof.colorGrading.settings;
//                LeanTween.value(1, 1.1f, time).setOnUpdate((float f) =>
//                {
//                    contrast.basic.contrast = f;
//                    prof.colorGrading.settings = contrast;
//                });


                LeanTween.value(gameObject, baseFogColor, Color.black, time).setOnUpdate((Color col) =>
                 {
                     RenderSettings.fogColor = col;
                 });


                isInside = false;
            }
        }
        else
        {
            if (Vector3.Distance(centerPoint.position, PlayerManager.marker.gameObject.transform.position) <= boundsRadius)
            {
//                var prof = CC.profile;
//                var temperature = prof.colorGrading.settings;
//                LeanTween.value(-37.0f, 0, time).setOnUpdate((float f) =>
//                {
//                    temperature.basic.temperature = f;
//                    prof.colorGrading.settings = temperature;
//                }).setOnComplete(() => CC.enabled = false);
//
//                var contrast = prof.colorGrading.settings;
//                LeanTween.value(1.1f, 1, time).setOnUpdate((float f) =>
//                {
//                    contrast.basic.contrast = f;
//                    prof.colorGrading.settings = contrast;
//                });

                LeanTween.value(gameObject, Color.black, baseFogColor, time).setOnUpdate((Color col) =>
                 {
                     RenderSettings.fogColor = col;
                 });



                isInside = true;
            }
        }
    }
}