using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Raincrow.Maps;

[RequireComponent (typeof(MarkerSpawner))]
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
                Instance.m_LoadingParticles.transform.position = loadingReferenceMarker.instance.transform.position;
                yield return 0;
            }

            m_LoadingParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);

            //force for a few more seconds until the particles stops completely
            float disableTime = Time.time + 3.5f;
            while (Time.time < disableTime)
            {
                Instance.m_LoadingParticles.transform.position = loadingReferenceMarker.instance.transform.position;
                yield return 0;
            }

        }
    }

    public static void GetMarkers (bool isPhysical = true, bool move = true)
	{
		if (FTFManager.isInFTF)
			return;

//		print ("getMarkers");
		var data = new MapAPI ();
		data.characterName = PlayerDataManager.playerData.displayName; 
		data.physical = isPhysical;
        if (isPhysical)
        {
            if (move)
            {
                data.longitude = PlayerDataManager.playerPos.x;
                data.latitude = PlayerDataManager.playerPos.y;
            }
            else
            {
                data.longitude = PlayerManager.physicalMarker.position.x;
                data.latitude = PlayerManager.physicalMarker.position.y;
            }
        }
        else
        {
            if (move)
            {
                data.longitude = MapsAPI.Instance.position.x;
                data.latitude = MapsAPI.Instance.position.y;
            }
            else
            {
                data.longitude = PlayerManager.marker.position.x;
                data.latitude = PlayerManager.marker.position.y;
            }
        }

        if (MapsAPI.Instance != null && PlayerManager.marker != null)
        {
            if (lastPosition == Vector2.zero)
                lastPosition = PlayerManager.marker.position;

            if (isPhysical)
                MapsAPI.Instance.SetPosition(data.longitude, data.latitude);
            PlayerManager.marker.position = new Vector2((float)data.longitude, (float)data.latitude); //MapsAPI.Instance.position;

            //setup a marker to use it as a position reference and play the loading particle
            if (Instance != null && Instance.m_LoadingParticles != null)
            {
                if (Instance.loadingReferenceMarker == null)
                {
                    GameObject prefab = new GameObject();
                    Instance.loadingReferenceMarker = MapsAPI.Instance.AddMarker(new Vector2((float)data.longitude, (float)data.latitude), prefab);
                    Instance.loadingReferenceMarker.instance.name = "move loading particles";
                    Destroy(prefab);
                }
                Instance.StopAllCoroutines();
                Instance.loadingReferenceMarker.customData = "loading";
                Instance.loadingReferenceMarker.position = new Vector2((float)data.longitude, (float)data.latitude);
                Instance.StartCoroutine(Instance.EnableLoadingParticles());
            }
        }

        APIManager.Instance.PostCoven ("map/move", JsonConvert.SerializeObject (data), GetMarkersCallback);
	}

    static void GetMarkersCallback(string result, int response)
    {
        if (Instance != null && Instance.loadingReferenceMarker != null)
        {
            Instance.loadingReferenceMarker.customData = null;
        }

        if (response == 200)
        {
            try
            {
                var data = JsonConvert.DeserializeObject<MarkerAPI>(result);

                if (data.location.garden == "")
                    SoundManagerOneShot.Instance.SetBGTrack(data.location.music);
                else
                    SoundManagerOneShot.Instance.SetBGTrack(1);

                if (data.location.dominion != PlayerDataManager.currentDominion)
                {
                    PlayerDataManager.currentDominion = data.location.dominion;
                    ChatConnectionManager.Instance.SendDominionChannelRequest();
                    if (data.location.garden == "")
                        PlayerManagerUI.Instance.ShowDominion(PlayerDataManager.currentDominion);
                    else
                        PlayerManagerUI.Instance.ShowGarden(data.location.garden);
                }

                MarkerSpawner.Instance.CreateMarkers(AddEnumValue(data.tokens));

                lastPosition = new Vector2((float)data.location.longitude, (float)data.location.latitude);
                if (PlayerManager.Instance.IsFlying() == false)
                {
                    //MapsAPI.Instance.SetPosition(data.location.longitude, data.location.latitude);
                    PlayerManager.marker.position = new Vector2((float)data.location.longitude, (float)data.location.latitude); //MapsAPI.Instance.position;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }
        else
        {
            //rollback to last received position if move failed
            if (lastPosition != Vector2.zero)
            {
                if (PlayerManager.Instance.IsFlying() == false)
                {
                    MapsAPI.Instance.SetPosition(lastPosition.x, lastPosition.y);
                    PlayerManager.marker.position = lastPosition;
                }
            }
            UIGlobalErrorPopup.ShowError(() => { }, "Error while moving:\n[" + response + "] " + result);
        }
    }

	public static List<Token> AddEnumValue (List<Token> data)
	{
		var updatedData = new List<Token> ();
		foreach (Token item in data) {
			try{
				item.Type = (MarkerSpawner.MarkerType)Enum.Parse (typeof(MarkerSpawner.MarkerType), item.type);
			updatedData.Add (item);
			}catch{
			}
		}
		return updatedData;
	}

	public static Token AddEnumValueSingle (Token data)
	{
		data.Type = (MarkerSpawner.MarkerType)Enum.Parse (typeof(MarkerSpawner.MarkerType), data.type);
		return data;
	}
}

