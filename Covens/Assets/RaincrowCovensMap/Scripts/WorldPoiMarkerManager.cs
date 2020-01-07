using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldPoiMarkerManager : MonoBehaviour
{
    private enum IconType
    {
        NONE = 0,
        PLACE_OF_POWER = 1,
        GREY_HAND = 2,
    }

    private struct IconData
    {
        public IconType type;
        public float longitude;
        public float latitude;
        public SpriteRenderer renderer;
    }

    [SerializeField] private Sprite[] m_PopSprites;
    [SerializeField] private Sprite m_GreyHandSprite;
    [SerializeField] private SpriteRenderer m_IconPrefab;
    [SerializeField] private float m_Scale = 0.6f;

    private SimplePool<SpriteRenderer> m_IconPool;
    private List<IconData> m_Icons = new List<IconData>();

    private IEnumerator Start()
    {
        while (MapsAPI.Instance.IsInitialized == false)
            yield return 0;

        MapsAPI.Instance.OnExitStreetLevel += OnStartFlying;
        MapsAPI.Instance.OnEnterStreetLevel += OnStopFlying;

        m_IconPool = new SimplePool<SpriteRenderer>(m_IconPrefab, 10);

        while (UINearbyLocations.CachedLocations == null)
            yield return 0;

        while (GardenMarkers.instance == null)
            yield return 0;

        SpriteRenderer renderer;
        foreach (var item in GardenMarkers.instance.greyHandOffices)
        {
            renderer = m_IconPool.Spawn();
            renderer.name = "[poi-greyhandoffice] " + item.officeLocation;
            renderer.sprite = m_GreyHandSprite;
            renderer.transform.SetParent(MapsAPI.Instance.trackedContainer);
            renderer.GetComponent<Raincrow.Maps.MuskMarker>().OnClick = (m) => OnClickGreyHand(item.officeLocation);
            BoxCollider col = renderer.gameObject.AddComponent<BoxCollider>();
            col.size = new Vector3(5, 5, 0.2f);

            m_Icons.Add(new IconData
            {
                longitude = item.officeLongitude,
                latitude = item.officeLatitude,
                type = IconType.GREY_HAND,
                renderer = renderer
            });
        }
        
        foreach (var pop in UINearbyLocations.CachedLocations)
        {
            renderer = m_IconPool.Spawn();
            renderer.name = "[poi-pop] " + pop.name;
            renderer.sprite = m_PopSprites[Mathf.Clamp(pop.tier, 0, m_PopSprites.Length - 1)];
            renderer.transform.SetParent(MapsAPI.Instance.trackedContainer);

            m_Icons.Add(new IconData
            {
                longitude = (float)pop.longitude,
                latitude = (float)pop.latitude,
                type = IconType.PLACE_OF_POWER,
                renderer = renderer
            });
        }

        OnStopFlying();
    }

    private void OnStartFlying()
    {
        MapsAPI.Instance.OnCameraUpdate += OnMapUpdate;

        int count = m_Icons.Count;

        for (int i = 0; i < count; i++)
            m_Icons[i].renderer.gameObject.SetActive(true);
    }

    private void OnStopFlying()
    {
        MapsAPI.Instance.OnCameraUpdate -= OnMapUpdate;

        int count = m_Icons.Count;

        for (int i = 0; i < count; i++)
        {
            m_Icons[i].renderer.gameObject.SetActive(false);
        }
    }

    private void OnMapUpdate(bool position, bool zoom, bool rotation)
    {
        Vector3 scale = Vector3.one * MapLineraScale.linearMultiplier * m_Scale;
        int count = m_Icons.Count;

        for (int i = 0; i < count; i++)
        {
            m_Icons[i].renderer.transform.localScale = scale;
            m_Icons[i].renderer.transform.position = MapsAPI.Instance.GetWorldPosition(m_Icons[i].longitude, m_Icons[i].latitude);
        }
    }

    private void OnClickGreyHand(string name)
    {
        GreyHandOffice.Show(name);
    }
}
