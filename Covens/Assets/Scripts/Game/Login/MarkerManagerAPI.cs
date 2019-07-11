using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Raincrow.Maps;

[RequireComponent(typeof(MarkerSpawner))]
public class MarkerManagerAPI : MonoBehaviour
{
    private static MarkerManagerAPI Instance;
    private static Vector2 lastPosition = Vector2.zero;
    [SerializeField] private ParticleSystem m_LoadingParticles;
    private IMarker loadingReferenceMarker;
    public static List<string> instancesInRange = new List<string>();
    private static int m_MoveTweenId;

    public static bool inSpiritForm { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;

        if (Instance.m_LoadingParticles)
            m_LoadingParticles.Stop();
    }

    private IEnumerator EnableLoadingParticles()
    {
        if (m_LoadingParticles != null)
        {
            m_LoadingParticles.Play();

            //force the loading particles position
            while (loadingReferenceMarker.customData != null)
            {
                Instance.m_LoadingParticles.transform.position = loadingReferenceMarker.gameObject.transform.position;
                yield return 0;
            }

            m_LoadingParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);

            //force for a few more seconds until the particles stops completely
            float disableTime = Time.time + 3.5f;
            while (Time.time < disableTime)
            {
                Instance.m_LoadingParticles.transform.position = loadingReferenceMarker.gameObject.transform.position;
                yield return 0;
            }

        }
    }

    public static void GetMarkers(float longitude, float latitude, bool physical, System.Action callback = null, bool animateMap = true, bool showLoading = false, bool loadMap = false)
    {
        double dist = MapsAPI.Instance.DistanceBetweenPointsD(new Vector2(longitude, latitude), GetGPS.coordinates);
        if (!physical)
        {
            physical = dist < PlayerDataManager.DisplayRadius;
            inSpiritForm = !physical;
            Debug.Log("is Spirit form " + inSpiritForm);
        }
        if (LoginUIManager.isInFTF)
            return;

        if (PlaceOfPower.IsInsideLocation)
            return;

        var data = new MapAPI();
        data.characterName = PlayerDataManager.playerData.name;
        data.physical = physical;
        data.longitude = longitude;
        data.latitude = latitude;
        data.Instances = instancesInRange;
        string dataJson = JsonConvert.SerializeObject(data);

        if (showLoading)
            LoadingOverlay.Show();

        Debug.Log("<color=red>get markers</color>:\n" + dataJson);

        System.Action requestMarkers = () => { };
        requestMarkers = () => APIManager.Instance.Post("map/move", dataJson,
            (s, r) =>
            {
                if (r != 200)
                {
                    Debug.LogError("map/move failed with error [" + r + "]\"" + s + "\". Retrying.");
                    requestMarkers();
                }
                else
                {
                    Instance.StartCoroutine(RemoveOldMarkers());

                    LoadingOverlay.Hide();
                    GetMarkersCallback(s, r);
                    callback?.Invoke();
                }
            });

        if (loadMap)
        {
            MapsAPI.Instance.InitMap(
                longitude,
                latitude,
                MapsAPI.Instance.normalizedZoom,
                () =>
                {
                    requestMarkers();
                },
                animateMap);
        }
        else
        {
            requestMarkers();
        }

        PlayerManager.marker.coords = new Vector2(longitude, latitude);

        Vector3 targetPosition = MapsAPI.Instance.GetWorldPosition(longitude, latitude);
        LeanTween.cancel(m_MoveTweenId);
        if (Vector3.Distance(targetPosition, PlayerManager.marker.gameObject.transform.position) < 200)
            m_MoveTweenId = LeanTween.move(PlayerManager.marker.gameObject, targetPosition, 1f).setEaseOutCubic().uniqueId;
        else
            //PlayerManager.marker.SetWorldPosition(MapsAPI.Instance.GetWorldPosition(longitude, latitude));
            PlayerManager.marker.gameObject.transform.position = targetPosition;
    }

    public static void GetMarkers(bool isPhysical = true, bool flyto = true, System.Action callback = null, bool animateMap = true, bool showLoading = false)
    {
        // if (PlayerDataManager.playerData.state == "dead" || PlayerDataManager.playerData.energy <= 0)
        //     return;
        if (isPhysical)
        {
            inSpiritForm = false;
            Debug.Log("setting in spirit form false");
            GetMarkers(GetGPS.longitude, GetGPS.latitude, isPhysical, callback, animateMap, showLoading, true);
        }
        else
        {
            if (flyto)
            {
                GetMarkers(MapsAPI.Instance.position.x, MapsAPI.Instance.position.y, isPhysical, callback, animateMap, showLoading, true);
            }
            else
            {
                GetMarkers(PlayerManager.marker.coords.x, PlayerManager.marker.coords.y, isPhysical, callback, animateMap, showLoading, false);
            }
        }
    }

    public static event System.Action<string> OnChangeDominion;
    static void GetMarkersCallback(string result, int response)
    {
        if (response == 200)
        {
            try
            {
                var data = JsonConvert.DeserializeObject<MarkerAPI>(result);

                Debug.Log("get markers result:\n" + result);

                if (data.location.garden == "")
                {
                    PlayerDataManager.soundTrack = data.location.music;
                    SoundManagerOneShot.Instance.SetBGTrack(data.location.music);
                }
                else
                {
                    PlayerDataManager.soundTrack = 1;
                    SoundManagerOneShot.Instance.SetBGTrack(1);
                }

                PlayerDataManager.zone = data.location.zone;
                if (data.location.dominion != PlayerDataManager.currentDominion)
                {
                    PlayerDataManager.currentDominion = data.location.dominion;
                    OnChangeDominion?.Invoke(data.location.dominion);
                    //ChatConnectionManager.Instance.SendDominionChange();
                    if (data.location.garden == "")
                        PlayerManagerUI.Instance.ShowDominion(PlayerDataManager.currentDominion);
                    else
                        PlayerManagerUI.Instance.ShowGarden(data.location.garden);
                }

                PlayerDataManager.playerData.latitude = (float)data.location.latitude;
                PlayerDataManager.playerData.longitude = (float)data.location.longitude;
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }
        else
        {
            if (result == "4700") //player is dead
            {

            }
            else
            {
                UIGlobalErrorPopup.ShowError(() => { }, "Error while moving: " + result);
            }
        }
    }

    private static IEnumerator RemoveOldMarkers()
    {
        int batch = 0;
        float distance;
        double lng, lat;
        MapsAPI.Instance.GetPosition(out lng, out lat);
        Vector2 curPosition = new Vector2((float)lng, (float)lat);

        List<List<IMarker>> allMarkers = new List<List<IMarker>>(MarkerSpawner.Markers.Values);

        foreach (var marker in allMarkers)
        {
            if (marker[0].isNull)
                continue;

            distance = (float)MapsAPI.Instance.DistanceBetweenPointsD(marker[0].coords, curPosition);
            if (distance > PlayerDataManager.DisplayRadius * 0.9f)
            {
                marker[0].gameObject.SetActive(false);
                MarkerSpawner.DeleteMarker(marker[0].token.instance);
            }

            batch++;
            if (batch % 10 == 0)
                yield return 0;
        }
        yield return 0;
    }
}

