using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Raincrow.Maps;
using Google.Maps;
using Google.Maps.Event;
using BuildingType = Google.Maps.Feature.StructureMetadata.UsageType;

namespace Raincrow.Maps
{
    public class BuildingIconManager : MonoBehaviour
    {
        [SerializeField] private MapsService m_Map;
        [SerializeField] private CovensMuskMap m_MapWrapper;
        [SerializeField] private MapCameraController m_Controller;
        [SerializeField] private SpriteRenderer m_IconPrefab;

        [Header("settings")]
        [SerializeField] private float m_IconScale;
        [SerializeField] private float m_MinScale = 6000;
        [SerializeField] private float m_MaxScale = 5;

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
            public Transform iconTransform;
            public new SpriteRenderer renderer;

            private void OnDestroy()
            {
                onDestroy?.Invoke();
            }
        }

        private Sprite[] m_IconSpritesArray;
        private SimplePool<SpriteRenderer> m_IconPool;
        private HashSet<BuildingComponent> m_IconsDict;

        private bool m_IconsEnabled = true;
        private float m_CurrentAlpha = 1f;
        private int m_AlphaTweenId;

        public bool initialized { get; private set; }

        private void Awake()
        {
            m_IconsDict = new HashSet<BuildingComponent>();
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
            m_TouristDestinationSprite,
            m_ShoppingSprite,
            m_SchoolSprite
            };

            m_Map.Events.ExtrudedStructureEvents.DidCreate.AddListener(OnDidCreateExtrudedStructure);
            m_Map.Events.ModeledStructureEvents.DidCreate.AddListener(OnDidCreateModeledStructure);

            initialized = true;
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

            int typeIdx = (int)type;
            if (typeIdx >= m_IconSpritesArray.Length)
                return;

            if (height > 50)
                height = 50;

            SpriteRenderer iconInstance = m_IconPool.Spawn();
            iconInstance.sprite = m_IconSpritesArray[(int)type];
            iconInstance.transform.SetParent(m_MapWrapper.itemContainer);
            iconInstance.transform.position = building.transform.position + new Vector3(0, height + 10, 0);
            iconInstance.transform.localScale = Vector3.zero;
            iconInstance.color = m_IconsEnabled ? new Color(1, 1, 1, m_CurrentAlpha) : new Color(1, 1, 1, 0);

            BuildingComponent bld = building.AddComponent<BuildingComponent>();
            bld.iconTransform = iconInstance.transform;
            bld.renderer = iconInstance;
            m_IconsDict.Add(bld);

            bld.onDestroy = () =>
            {
                m_IconPool.Despawn(iconInstance);
                m_IconsDict.Remove(bld);
            };
        }

        private void OnApplicationQuit()
        {
            foreach (var icon in m_IconsDict)
            {
                icon.transform.SetParent(null);
                icon.onDestroy = null;
            }
        }

        private void Update()
        {
            if (m_IconsEnabled)
            {
                Vector3 scale = Vector3.one * LeanTween.easeOutCubic(m_MinScale, m_MaxScale, m_MapWrapper.normalizedZoom) * m_IconScale;
                foreach (BuildingComponent _building in m_IconsDict)
                {
                    _building.iconTransform.rotation = m_Controller.camera.transform.rotation;
                    _building.iconTransform.localScale = scale;
                }
            }
        }

        public void EnableIcons(bool enable)
        {
            if (enable == m_IconsEnabled)
                return;

            m_IconsEnabled = enable;

            LeanTween.cancel(m_AlphaTweenId);
            m_AlphaTweenId = LeanTween.value(m_CurrentAlpha, enable ? 1 : 0, 1f)
                .setEaseOutCubic()
                .setOnUpdate((float t) =>
                {
                    m_CurrentAlpha = t;
                    foreach (BuildingComponent _building in m_IconsDict)
                        _building.renderer.color = new Color(1, 1, 1, t);
                })
                .uniqueId;
        }

        [ContextMenu("EnableIcons")]
        private void DebugEnableIcons()
        {
            EnableIcons(true);
        }
        [ContextMenu("DisableIcons")]
        private void DebugDisableIcons()
        {
            EnableIcons(false);
        }
    }
}