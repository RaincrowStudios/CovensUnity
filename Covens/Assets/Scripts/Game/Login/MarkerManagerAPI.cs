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

    private static Vector3 m_LastMarkerPosition;

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
        if (loadMap)
        {
            Vector3 worldPos = MapsAPI.Instance.GetWorldPosition(longitude, latitude);
            float distance = Vector3.Distance(m_LastMarkerPosition, worldPos);

            if (distance > 1000)
            {
                Debug.Log("despawning old markers");
                MarkerSpawner.DeleteAllMarkers();
                m_LastMarkerPosition = worldPos;
            }
        }

        var data = new MapAPI();
        data.characterName = PlayerDataManager.playerData.displayName;
        data.physical = physical;
        data.longitude = longitude;
        data.latitude = latitude;
        data.Instances = instancesInRange;

        if (showLoading)
            LoadingOverlay.Show();

        System.Action requestMarkers = () => APIManager.Instance.PostCoven("map/move", JsonConvert.SerializeObject(data),
            (s, r) =>
            {
                LoadingOverlay.Hide();
                GetMarkersCallback(s, r);
                callback?.Invoke();
            });

        if(loadMap)
        {
            MapsAPI.Instance.InitMap(
                longitude,
                latitude, 
                MapsAPI.Instance.normalizedZoom,
                () => {
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
        if (LoginUIManager.isInFTF)
            return;

        if (PlayerDataManager.playerData.state == "dead" || PlayerDataManager.playerData.energy <= 0)
            return;

        if (isPhysical)
        {
            GetMarkers(PlayerDataManager.playerPos.x, PlayerDataManager.playerPos.y, isPhysical, callback, animateMap, showLoading, true);
        }
        else
        {
            if (flyto)
            {
                GetMarkers(MapsAPI.Instance.position.x, MapsAPI.Instance.position.y, isPhysical, callback, animateMap, showLoading, true);
            }
            else
            {
                GetMarkers(PlayerManager.marker.position.x, PlayerManager.marker.position.y, isPhysical, callback, animateMap, showLoading, false);
            }
        }
    }

    static void GetMarkersCallback(string result, int response)
    {
        if (response == 200)
        {
            try
            {
                var data = JsonConvert.DeserializeObject<MarkerAPI>(result);
                if (data.location.garden == "")
                    SoundManagerOneShot.Instance.SetBGTrack(data.location.music);
                else
                    SoundManagerOneShot.Instance.SetBGTrack(1);

                PlayerDataManager.zone = data.location.zone;
                if (data.location.dominion != PlayerDataManager.currentDominion)
                {
                    PlayerDataManager.currentDominion = data.location.dominion;
                    ChatConnectionManager.Instance.SendDominionChange();
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

    public static List<Token> AddEnumValue(List<Token> data)
    {
        var updatedData = new List<Token>();
        foreach (Token item in data)
        {
            try
            {
                item.Type = (MarkerSpawner.MarkerType)Enum.Parse(typeof(MarkerSpawner.MarkerType), item.type);
                updatedData.Add(item);
            }
            catch
            {
            }
        }
        return updatedData;
    }

    public static Token AddEnumValueSingle(Token data)
    {
        data.Type = (MarkerSpawner.MarkerType)Enum.Parse(typeof(MarkerSpawner.MarkerType), data.type);
        return data;
    }
}

