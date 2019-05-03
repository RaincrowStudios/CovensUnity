using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Raincrow.Maps;
using Google.Maps;
using Google.Maps.Event;
using BuildingType = Google.Maps.Feature.StructureMetadata.UsageType;

public class BuildingIconsManager : MonoBehaviour
{
    [SerializeField] private MapsService m_Map;
    [SerializeField] private CovensMuskMap m_MapWrapper;
    [SerializeField] private SpriteRenderer m_IconPrefab;

    [Header("settings")]
    [SerializeField] private float m_IconScale;
    [SerializeField] private float m_BatchSize;

    [Header("sprites")]
    [SerializeField] private Sprite m_BankSprite;
    [SerializeField] private Sprite m_BarSprite;
    [SerializeField] private Sprite m_CafeSprite;
    [SerializeField] private Sprite m_EventVenueSprite;
    [SerializeField] private Sprite m_LodgingSprite;
    [SerializeField] private Sprite m_RestaurantSprite;
    [SerializeField] private Sprite m_SchoolSprite;
    [SerializeField] private Sprite m_ShoppingSprite;
    [SerializeField] private Sprite m_TouristDestinationSprite;

    private class BuildingComponent : MonoBehaviour
    {
        public System.Action onDestroy;
        private void OnDestroy()
        {
            onDestroy?.Invoke();
        }
    }

    private Sprite[] m_IconSpritesArray;
    private SimplePool<SpriteRenderer> m_IconPool;
    private List<Transform> m_IconsList;
    private int m_BatchIndex;

    private void Awake()
    {
        m_IconsList = new List<Transform>();
        m_IconPool = new SimplePool<SpriteRenderer>(m_IconPrefab, 20);

        m_IconSpritesArray = new Sprite[]
        {
            null,
            m_BarSprite,
            m_BankSprite,
            m_LodgingSprite,
            m_CafeSprite,
            m_RestaurantSprite,
            m_EventVenueSprite,
            m_TouristDestinationSprite
        };

        m_Map.Events.ExtrudedStructureEvents.DidCreate.AddListener(OnDidCreateExtrudedStructure);
        m_Map.Events.ModeledStructureEvents.DidCreate.AddListener(OnDidCreateModeledStructure);
    }

    private void OnDidCreateExtrudedStructure(DidCreateExtrudedStructureArgs e)
    {
        AddIcon(e.GameObject, e.MapFeature.Metadata.Usage, e.MapFeature.Shape.BoundingBox.size.y);
    }

    private void OnDidCreateModeledStructure(DidCreateModeledStructureArgs e)
    {
        AddIcon(e.GameObject, e.MapFeature.Metadata.Usage, e.MapFeature.Shape.BoundingBox.size.y);
    }

    private void AddIcon(GameObject building, BuildingType type, float height)
    {
        if (type == BuildingType.Unspecified)
            return;

        SpriteRenderer iconInstance = m_IconPool.Spawn();
        iconInstance.sprite = m_IconSpritesArray[(int)type];
        iconInstance.transform.SetParent(m_MapWrapper.itemContainer);
        iconInstance.transform.position = building.transform.position + new Vector3(0, height, 0);

        //building.AddComponent<BuildingComponent>().onDestroy = 
    }
}
