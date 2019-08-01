using System.Collections.Generic;
using UnityEngine;

public class LocationIsland : LocationIslandBase
{

    [SerializeField] private LeanTweenType leanType;
    [SerializeField] private LineRenderer m_Renderer;
    public void Setup(Dictionary<int, string> tokens, float distance, Camera cam)
    {
        if (tokens != null)
            this.tokens = tokens;
        else
            this.tokens = new Dictionary<int, string>();

        Transform moveTransform = transform.GetChild(0);
        distance += Random.Range(-30, 30);
        m_Renderer.positionCount = 2;
        LeanTween.value(0, 1, 1).setOnUpdate((float value) =>
         {
             m_Renderer.SetPosition(1, moveTransform.position);
             moveTransform.localPosition = new Vector3(Mathf.Lerp(0, distance, value), 0, 0);
             moveTransform.localScale = Vector3.one * Mathf.Lerp(.2f, 1, value);
         }).setEase(leanType);
        UpdateMarkers();
        LocationIslandController.instance.popCameraController.onUpdate += (x, y, z) =>
        {
            FaceMarkers(cam);
        };
    }

    public void AddToken(KeyValuePair<int, string> token)
    {
        Add(token);
        UpdateMarkers();
    }

    public void RemoveToken(int position)
    {
        Remove(position);
        UpdateMarkers();
    }

    public void UpdateMarkers()
    {
        foreach (var item in tokens)
        {
            var GO = Instantiate(Resources.Load<GameObject>("WitchPrefab"));
            GO.SetActive(true);
            GO.transform.parent = spots[item.Key];
            GO.transform.localPosition = Vector3.zero;
        }
    }

    void FaceMarkers(Camera camera)
    {
        foreach (var item in GetMarkers())
        {
            item.GetChild(0).GetChild(0).rotation = camera.transform.rotation;
        }
    }
}