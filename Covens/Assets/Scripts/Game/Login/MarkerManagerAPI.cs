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

    public static void GetMarkers(float longitude, float latitude, bool physical, System.Action callback = null, bool animateMap = true, bool showLoading = true)
    {
        // #if UNITY_EDITOR
        //         Debug.LogError("GetMarkers");
        // #endif

        var data = new MapAPI();
        data.characterName = PlayerDataManager.playerData.displayName;
        data.physical = physical;
        data.longitude = longitude;
        data.latitude = latitude;

        if (showLoading)
            LoadingOverlay.Show();

        APIManager.Instance.PostCoven("map/move", JsonConvert.SerializeObject(data),
            (s, r) =>
            {
                GetMarkersCallback(s, r, animateMap);
                callback?.Invoke();
                LoadingOverlay.Hide();
            });
    }

    public static void GetMarkers(bool isPhysical = true, bool flyto = true, System.Action callback = null, bool animateMap = true, bool showLoading = true)
    {
        if (LoginUIManager.isInFTF)
            return;

        if (PlayerDataManager.playerData.state == "dead" || PlayerDataManager.playerData.energy <= 0)
            return;

        if (isPhysical)
        {
            GetMarkers(PlayerDataManager.playerPos.x, PlayerDataManager.playerPos.y, isPhysical, callback, animateMap, showLoading);
        }
        else
        {
            if (flyto)
            {
                GetMarkers(MapsAPI.Instance.position.x, MapsAPI.Instance.position.y, isPhysical, callback, animateMap, showLoading);
            }
            else
            {
                GetMarkers(PlayerManager.marker.position.x, PlayerManager.marker.position.y, isPhysical, callback, animateMap, showLoading);
            }
        }
    }

    static void GetMarkersCallback(string result, int response, bool animateMap)
    {
        //if (Instance != null && Instance.loadingReferenceMarker != null)
        //{
        //    Instance.loadingReferenceMarker.customData = null;
        //}

        if (response == 200)
        {
            try
            {
                Debug.Log("success");
                var data = JsonConvert.DeserializeObject<MarkerAPI>(result);
                Debug.Log(result);
                if (Application.isEditor)
                {
                    TextEditor te = new TextEditor();
                    te.text = result;
                    te.SelectAll();
                    te.Copy();
                }
                if (data.location.garden == "")
                    SoundManagerOneShot.Instance.SetBGTrack(data.location.music);
                else
                    SoundManagerOneShot.Instance.SetBGTrack(1);

                PlayerDataManager.zone = data.location.zone;
                if (data.location.dominion != PlayerDataManager.currentDominion)
                {
                    PlayerDataManager.currentDominion = data.location.dominion;
                    Debug.Log("DOMINION CHANGED");
                    ChatConnectionManager.Instance.SendDominionChange();
                    if (data.location.garden == "")
                        PlayerManagerUI.Instance.ShowDominion(PlayerDataManager.currentDominion);
                    else
                        PlayerManagerUI.Instance.ShowGarden(data.location.garden);
                }

                MapsAPI.Instance.ShowStreetMap(
                    data.location.longitude,
                    data.location.latitude, () =>
                    {
                        PlayerManager.marker.position = new Vector2((float)data.location.longitude, (float)data.location.latitude);
                        //spawn the markers after the street map is loaded
                        // MarkerSpawner.Instance.CreateMarkers(AddEnumValue(data.tokens));
                    },
                    animateMap);
                //}
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

