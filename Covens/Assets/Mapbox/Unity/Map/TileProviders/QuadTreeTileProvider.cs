﻿using System;
using System.Collections.Generic;
using Mapbox.Map;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;
using UnityEngine;

namespace Mapbox.Unity.Map.TileProviders
{
    [DisallowMultipleComponent]
	public class QuadTreeTileProvider : AbstractTileProvider
	{
		private Plane _groundPlane;
		private bool _shouldUpdate;
		private CameraBoundsTileProviderOptions _cbtpOptions;

		//private List<UnwrappedTileId> _toRemove;
		//private HashSet<UnwrappedTileId> _tilesToRequest;
		private Vector2dBounds _viewPortWebMercBounds;

        private UnwrappedTileId _currentTile;

		#region Tile decision and raycasting fields
		private HashSet<UnwrappedTileId> _tiles;
		private HashSet<CanonicalTileId> _canonicalTiles;

		private Ray _ray00;
		private Ray _ray01;
		private Ray _ray10;
		private Ray _ray11;
		private Vector3[] _hitPnt = new Vector3[4];
		private bool _isFirstLoad;
		#endregion

		public override void OnInitialized()
		{
			_tiles = new HashSet<UnwrappedTileId>();
			_canonicalTiles = new HashSet<CanonicalTileId>();
			_cbtpOptions = (CameraBoundsTileProviderOptions)_options;

			if (_cbtpOptions.camera == null)
			{
				_cbtpOptions.camera = Camera.main;
			}
			_cbtpOptions.camera.transform.hasChanged = false;
			_groundPlane = new Plane(Vector3.up, 0);
			_shouldUpdate = true;
			_currentExtent.activeTiles = new HashSet<UnwrappedTileId>();
		}

		public override void UpdateTileExtent()
		{
			if (!_shouldUpdate)
			{
				return;
			}

			//update viewport in case it was changed by switching zoom level
			_viewPortWebMercBounds = getcurrentViewPortWebMerc();
			_currentExtent.activeTiles = GetWithWebMerc(_viewPortWebMercBounds, _map.AbsoluteZoom);

			OnExtentChanged();
		}

		public HashSet<UnwrappedTileId> GetWithWebMerc(Vector2dBounds bounds, int zoom)
		{
			_tiles.Clear();
			_canonicalTiles.Clear();

			if (bounds.IsEmpty()) { return _tiles; }

			//stay within WebMerc bounds
			Vector2d swWebMerc = new Vector2d(Math.Max(bounds.SouthWest.x, -Utils.Constants.WebMercMax), Math.Max(bounds.SouthWest.y, -Utils.Constants.WebMercMax));
			Vector2d neWebMerc = new Vector2d(Math.Min(bounds.NorthEast.x, Utils.Constants.WebMercMax), Math.Min(bounds.NorthEast.y, Utils.Constants.WebMercMax));

			//UnityEngine.Debug.LogFormat("swWebMerc:{0}/{1} neWebMerc:{2}/{3}", swWebMerc.x, swWebMerc.y, neWebMerc.x, neWebMerc.y);

			UnwrappedTileId swTile = WebMercatorToTileId(swWebMerc, zoom);
			UnwrappedTileId neTile = WebMercatorToTileId(neWebMerc, zoom);
            _currentTile = TileCover.CoordinateToTileId(_map.WorldToGeoPosition(_cbtpOptions.cameraCenterPoint.position), zoom);

            int right = neTile.X + _cbtpOptions.visibleBuffer;
            int top = swTile.Y + _cbtpOptions.visibleBuffer;
            
            if (right - _currentTile.X > _cbtpOptions.maxRangeFromCenter) right = _currentTile.X + _cbtpOptions.maxRangeFromCenter;
            if (Mathf.Abs(top - _currentTile.Y) > _cbtpOptions.maxRangeFromCenter) top = _currentTile.Y + _cbtpOptions.maxRangeFromCenter;

#if UNITY_EDITOR
            m_Debug_CenterTilePos = _map.GeoToWorldPosition(_map.WorldToGeoPosition(_cbtpOptions.cameraCenterPoint.position), false);
#endif

            for (int i = 0; i < right - _currentTile.X; i++)
            {
                for (int j = 0; j < top - _currentTile.Y; j++)
                {
                    AddTile(new UnwrappedTileId(zoom, _currentTile.X + i, _currentTile.Y + j));
                    AddTile(new UnwrappedTileId(zoom, _currentTile.X + i, _currentTile.Y - j));
                    AddTile(new UnwrappedTileId(zoom, _currentTile.X - i, _currentTile.Y + j));
                    AddTile(new UnwrappedTileId(zoom, _currentTile.X - i, _currentTile.Y - j));
                }
            }

            return _tiles;
		}

        private void AddTile(UnwrappedTileId uwtid)
        {
            if (!_canonicalTiles.Contains(uwtid.Canonical))
            {
                _tiles.Add(uwtid);
                _canonicalTiles.Add(uwtid.Canonical);
            }
        }

        public UnwrappedTileId WebMercatorToTileId(Vector2d webMerc, int zoom)
		{
			var tileCount = Math.Pow(2, zoom);

			var dblX = webMerc.x / Utils.Constants.WebMercMax;
			var dblY = webMerc.y / Utils.Constants.WebMercMax;

			int x = (int)Math.Floor((1 + dblX) / 2 * tileCount);
			int y = (int)Math.Floor((1 - dblY) / 2 * tileCount);
			return new UnwrappedTileId(zoom, x, y);
		}
		private Vector2dBounds getcurrentViewPortWebMerc(bool useGroundPlane = true)
		{
			if (useGroundPlane)
			{
				// rays from camera to groundplane: lower left and upper right
				_ray00 = _cbtpOptions.camera.ViewportPointToRay(new Vector3(0, 0));
				_ray01 = _cbtpOptions.camera.ViewportPointToRay(new Vector3(0, 1));
				_ray10 = _cbtpOptions.camera.ViewportPointToRay(new Vector3(1, 0));
				_ray11 = _cbtpOptions.camera.ViewportPointToRay(new Vector3(1, 1));
				_hitPnt[0] = getGroundPlaneHitPoint(_ray00);
				_hitPnt[1] = getGroundPlaneHitPoint(_ray01);
				_hitPnt[2] = getGroundPlaneHitPoint(_ray10);
				_hitPnt[3] = getGroundPlaneHitPoint(_ray11);
			}

			// Find min max bounding box.
			// TODO : Find a better way of doing this.
			float minX = float.MaxValue;
			float minZ = float.MaxValue;
			float maxX = float.MinValue;
			float maxZ = float.MinValue;
			for (int i = 0; i < 4; i++)
			{
				if (_hitPnt[i] == Vector3.zero)
				{
					continue;
				}
				else
				{
					if (minX > _hitPnt[i].x)
					{
						minX = _hitPnt[i].x;
					}

					if (minZ > _hitPnt[i].z)
					{
						minZ = _hitPnt[i].z;
					}

					if (maxX < _hitPnt[i].x)
					{
						maxX = _hitPnt[i].x;
					}

					if (maxZ < _hitPnt[i].z)
					{
						maxZ = _hitPnt[i].z;
					}
				}
			}

			Vector3 hitPntLL = new Vector3(minX, 0, minZ);
			Vector3 hitPntUR = new Vector3(maxX, 0, maxZ);

			//Debug.Log(hitPntLL + " - " + hitPntUR);

			var llLatLong = _map.WorldToGeoPosition(hitPntLL);
			var urLatLong = _map.WorldToGeoPosition(hitPntUR);

			Vector2dBounds tileBounds = new Vector2dBounds(Conversions.LatLonToMeters(llLatLong), Conversions.LatLonToMeters(urLatLong));

			// Bounds debugging.
#if UNITY_EDITOR
			Debug.DrawLine(_cbtpOptions.camera.transform.position, hitPntLL, Color.blue);
			Debug.DrawLine(_cbtpOptions.camera.transform.position, hitPntUR, Color.red);
#endif
			return tileBounds;
		}
		private Vector3 getGroundPlaneHitPoint(Ray ray)
		{
			float distance;
			if (!_groundPlane.Raycast(ray, out distance)) { return Vector3.zero; }
			return ray.GetPoint(distance);
		}

		public virtual void Update()
		{
			if (_cbtpOptions != null && _cbtpOptions.camera != null && _cbtpOptions.camera.transform.hasChanged)
			{
				UpdateTileExtent();
				_cbtpOptions.camera.transform.hasChanged = false;
			}
		}

		public override bool Cleanup(UnwrappedTileId tile)
		{
            bool dispose = false;
            dispose = tile.X > _currentTile.X + _cbtpOptions.disposeBuffer || tile.X < _currentTile.X - _cbtpOptions.disposeBuffer;
            dispose = dispose || tile.Y > _currentTile.Y + _cbtpOptions.disposeBuffer || tile.Y < _currentTile.Y - _cbtpOptions.disposeBuffer;

            return (dispose);
        }

#if UNITY_EDITOR
        private Vector3 m_Debug_CenterTilePos;

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(m_Debug_CenterTilePos, new Vector3(100, 2, 100));
        }
#endif
    }
}
