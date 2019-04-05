using UnityEngine;
using System.Collections;
using Mapbox.Map;
using Mapbox.Unity.Map;
using Mapbox.Unity.Map.TileProviders;
using System.Collections.Generic;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;

public class CircleRangeTileProvider : AbstractTileProvider
{
    [SerializeField] private Transform m_CameraPoint;
    [SerializeField] private float m_NearDistFromPoint = 300;
    [SerializeField] private float m_FarDistFromPoint = 600;
    [SerializeField] private float m_UnityTileSize = 100;

    public static float minViewDistance { get; private set; }

    private UnwrappedTileId m_CenterTile;
    //private Dictionary<string, UnwrappedTileId> m_TileDict;
    private bool m_Initialized = false;
    private int m_Radius = 8;

    private void Awake()
    {
        LoginAPIManager.OnCharacterInitialized += LoginAPIManager_OnCharacterInitialized;
    }

    private void LoginAPIManager_OnCharacterInitialized()
    {
        m_Radius = (int)((PlayerDataManager.DisplayRadius * GeoToKmHelper.OneKmInWorldspace) / m_UnityTileSize) + 2;
    }

    public override void OnInitialized()
    {
        m_Initialized = true;
        _currentExtent.activeTiles = new HashSet<UnwrappedTileId>();
        //m_TileDict = new Dictionary<string, UnwrappedTileId>();
        m_CameraPoint.hasChanged = true;
    }

    public override void UpdateTileExtent()
    {
        if (!m_Initialized)
            return;

        //m_TileDict.Clear();
        _currentExtent.activeTiles.Clear();

        m_CenterTile = TileCover.CoordinateToTileId(_map.CenterLatitudeLongitude, _map.AbsoluteZoom);

        //if (m_TileDict.ContainsKey(m_CenterTile.X + "" + m_CenterTile.Z) == false)
        //{
            //m_TileDict.Add(m_CenterTile.X + "" + m_CenterTile.Z, m_CenterTile);
            _currentExtent.activeTiles.Add(m_CenterTile);//new UnwrappedTileId(_map.AbsoluteZoom, centerTile.X, centerTile.Y));
        //}

        float t = MapController.Instance.m_StreetMap.normalizedZoom;
        minViewDistance = m_FarDistFromPoint * t + m_NearDistFromPoint * (1 - t);
        Vector3 aproxPos;
        int x;
        int y;

        for (int i = -m_Radius; i <= m_Radius; i++)
        {
            for (int j = -m_Radius; j <= m_Radius; j++)
            {
                if (IsTileInsideCircle(i, j))
                {
                    aproxPos = transform.position + new Vector3(i * m_UnityTileSize, 0, -j * m_UnityTileSize);
                    if (Vector3.Distance(m_CameraPoint.position, aproxPos) <= minViewDistance)
                    {
                        x = m_CenterTile.X + i;
                        y = m_CenterTile.Y + j;
                        //if (m_TileDict.ContainsKey(x + "" + y) == false)
                        {
                            UnwrappedTileId tile = new UnwrappedTileId(_map.AbsoluteZoom, x, y);
                            //m_TileDict.Add(x + "" + y, tile);
                            _currentExtent.activeTiles.Add(tile);
                        }
                    }
                }
            }
        }

        OnExtentChanged();
    }

    private void Update()
    {
        if (m_CameraPoint.hasChanged)
        {
            UpdateTileExtent();
            m_CameraPoint.hasChanged = false;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(m_CameraPoint.position, new Vector3(m_UnityTileSize, 1, m_UnityTileSize));
    }

    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying == false)
            return;
        
        float t = MapController.Instance.m_StreetMap.normalizedZoom;
        minViewDistance = m_FarDistFromPoint * t + m_NearDistFromPoint * (1 - t);

        for (int i = -m_Radius; i <= m_Radius; i++)
        {
            for (int j = -m_Radius; j <= m_Radius; j++)
            {
                Vector3 aproxPos = new Vector3(i * m_UnityTileSize, 0, -j * m_UnityTileSize);

                if (IsTileInsideCircle(i, j) == false)
                {
                    Gizmos.color = Color.red;
                }
                else
                {
                    if (Vector3.Distance(m_CameraPoint.localPosition, aproxPos) > minViewDistance)
                        Gizmos.color = Color.yellow;
                    else
                        Gizmos.color = Color.green;
                }
                Gizmos.DrawWireCube(aproxPos, new Vector3(m_UnityTileSize, 1, m_UnityTileSize));
            }
        }
    }
#endif

    private bool IsTileInsideCircle(int x, int y)
    {
        return Mathf.Sqrt(Mathf.Pow(x-0, 2) + Mathf.Pow(y-0, 2)) < m_Radius;
    }

    public override bool Cleanup(UnwrappedTileId tile)
    {
        return !IsTileInsideCircle(tile.X - m_CenterTile.X, tile.Y - m_CenterTile.Y);
        //return (!_currentExtent.activeTiles.Contains(tile));
    }
}
