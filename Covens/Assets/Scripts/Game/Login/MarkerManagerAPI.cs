using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Raincrow.Maps;
using Raincrow.GameEventResponses;

[RequireComponent(typeof(MarkerSpawner))]
public class MarkerManagerAPI : MonoBehaviour
{
    public class MapMoveResponse
    {
        public class Location
        {
            public string dominion;
            public string garden;
            public int zone;
            public int music;
        }

        public WitchToken[] characters;
        public SpiritToken[] spirits;
        public CollectableToken[] items;
        public Location location;
    }

    public static event System.Action<string> OnChangeDominion;
    public static bool IsSpiritForm { get; private set; }
    
    private static MarkerManagerAPI m_Instance;
    private static int m_MoveTweenId;
    private static Coroutine m_SpawnCoroutine;

    private void Awake()
    {
        if (m_Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        m_Instance = this;
    }

    public static void GetMarkers(float longitude, float latitude, System.Action callback = null, bool animateMap = true, bool showLoading = false, bool loadMap = false)
    {
        double dist = MapsAPI.Instance.DistanceBetweenPointsD(new Vector2(longitude, latitude), GetGPS.coordinates);
        bool physical = dist < PlayerDataManager.DisplayRadius;
        IsSpiritForm = !physical;

        if (PlayerDataManager.IsFTF)
            return;

        if (PlaceOfPower.IsInsideLocation)
            return;

        var data = new
        {
            physical,
            longitude,
            latitude,
        };
        string dataJson = JsonConvert.SerializeObject(data);

        if (showLoading)
            LoadingOverlay.Show();

        Debug.Log("<color=red>get markers</color>:\n" + dataJson);

        System.Action requestMarkers = () => { };
        requestMarkers = () => APIManager.Instance.Post("character/move", dataJson,
            (s, r) =>
            {
                LoadingOverlay.Hide();

                if (r == 200)
                    GetMarkersCallback(longitude, latitude, s, r);
                else
                    GetMarkersFailed(longitude, latitude, animateMap, loadMap, s, r);

                callback?.Invoke();
            });


        //pre-move the player marker and load the map at the target position
        if (loadMap)
        {
            MapsAPI.Instance.InitMap(
                longitude,
                latitude,
                MapsAPI.Instance.normalizedZoom,
                () =>
                {
                    LeanTween.cancel(m_MoveTweenId);
                    Vector3 targetPosition = MapsAPI.Instance.GetWorldPosition(longitude, latitude);
                    if (Vector3.Distance(targetPosition, PlayerManager.marker.gameObject.transform.position) < 200)
                        m_MoveTweenId = LeanTween.move(PlayerManager.marker.gameObject, targetPosition, 1f).setEaseOutCubic().uniqueId;
                    else
                        PlayerManager.marker.gameObject.transform.position = targetPosition;

                    requestMarkers();
                },
                animateMap);
        }
        else
        {
            requestMarkers();
        }
    }

    public static void GetMarkers(bool isPhysical = true, bool flyto = true, System.Action callback = null, bool animateMap = true, bool showLoading = false)
    {
        if (isPhysical)
        {
            IsSpiritForm = false;
            GetMarkers(GetGPS.longitude, GetGPS.latitude, callback, animateMap, showLoading, true);
        }
        else
        {
            if (flyto)
            {
                GetMarkers(MapsAPI.Instance.position.x, MapsAPI.Instance.position.y, callback, animateMap, showLoading, true);
            }
            else
            {
                GetMarkers(PlayerManager.marker.coords.x, PlayerManager.marker.coords.y, callback, animateMap, showLoading, false);
            }
        }
    }
    
    private static void GetMarkersFailed(float longitude, float latitude, bool animateMap, bool loadMap, string result, int response)
    {
        //move back to previous position
        LeanTween.cancel(m_MoveTweenId);
        Vector3 targetPosition = MapsAPI.Instance.GetWorldPosition(PlayerDataManager.playerData.longitude, PlayerDataManager.playerData.latitude);
        if (Vector3.Distance(targetPosition, PlayerManager.marker.gameObject.transform.position) < 200)
            m_MoveTweenId = LeanTween.move(PlayerManager.marker.gameObject, targetPosition, 1f).setEaseOutCubic().uniqueId;
        else
            PlayerManager.marker.gameObject.transform.position = targetPosition;

        //go back to previous position and show error
        if (loadMap)
        {
            UIGlobalErrorPopup.ShowError(null, LocalizeLookUp.GetText("error_" + result));
            MapsAPI.Instance.InitMap(
               PlayerDataManager.playerData.longitude,
               PlayerDataManager.playerData.latitude,
               MapsAPI.Instance.normalizedZoom,
               () => { },
               animateMap);
        }
    }

    private static void GetMarkersCallback(float longitude, float latitude, string result, int response)
    {
        PlayerManager.marker.coords = new Vector2(longitude, latitude);
        PlayerDataManager.playerData.longitude = longitude;
        PlayerDataManager.playerData.latitude = latitude;
        
        MapMoveResponse moveResponse = JsonConvert.DeserializeObject<MapMoveResponse>(result);

        //update soundtrack
        if (string.IsNullOrWhiteSpace(moveResponse.location.garden))
        {
            PlayerDataManager.soundTrack = moveResponse.location.music;
            SoundManagerOneShot.Instance.SetBGTrack(moveResponse.location.music);
        }
        else
        {
            PlayerDataManager.soundTrack = 1;
            SoundManagerOneShot.Instance.SetBGTrack(1);
        }

        //update zone and dominion
        PlayerDataManager.zone = moveResponse.location.zone;
        if (moveResponse.location.dominion != PlayerDataManager.currentDominion)
        {
            PlayerDataManager.currentDominion = moveResponse.location.dominion;
            OnChangeDominion?.Invoke(moveResponse.location.dominion);
            if (string.IsNullOrWhiteSpace(moveResponse.location.garden))
            {
                PlayerManagerUI.Instance.ShowDominion(PlayerDataManager.currentDominion);
            }
            else
            {
                PlayerManagerUI.Instance.ShowGarden(moveResponse.location.garden);
            }
        }

        //finaly add/update markers
        if (m_SpawnCoroutine != null)
            m_Instance.StopCoroutine(m_SpawnCoroutine);
            
        m_SpawnCoroutine = m_Instance.StartCoroutine(SpawnMarkersCoroutine(moveResponse.characters, moveResponse.spirits, moveResponse.items));
    }

    private static IEnumerator SpawnMarkersCoroutine(WitchToken[] witches, SpiritToken[] spirits, CollectableToken[] items)
    {
        HashSet<IMarker> updatedMarkers = new HashSet<IMarker>();

        IMarker aux;
        for (int i = 0; i < witches.Length; i++)
        {
            aux = MarkerSpawner.Instance.AddMarker(witches[i]);
            if (aux != null)
                updatedMarkers.Add(aux);
            yield return 1;
        }

        for (int i = 0; i < spirits.Length; i++)
        {
            if (spirits[i].energy == 0)
                continue;

            aux = MarkerSpawner.Instance.AddMarker(spirits[i]);
            if (aux != null)
                updatedMarkers.Add(aux);
        }

        for (int i = 0; i < items.Length; i++)
        {
            aux = MarkerSpawner.Instance.AddMarker(items[i]);
            if (aux != null)
                updatedMarkers.Add(aux);
        }

        Dictionary<string, List<IMarker>>.ValueCollection values = MarkerSpawner.Markers.Values;
        List<string> toRemove = new List<string>();
        foreach (List<IMarker> marker in values)
        {
            if (updatedMarkers.Contains(marker[0]))
                continue;

            //Debug.Log("<color=magenta>removing " + marker[0].gameObject.name + "</color>");
            toRemove.Add(marker[0].token.Id);
        }

        foreach (string id in toRemove)
            MarkerSpawner.DeleteMarker(id);
        
        m_SpawnCoroutine = null;
    }
}

