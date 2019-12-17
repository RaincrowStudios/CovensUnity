using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Raincrow.Maps;
using Raincrow.GameEventResponses;

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

        public List<WitchToken> characters;
        public List<SpiritToken> spirits;
        public List<CollectableToken> items;
        public List<EnergyToken> energies;
        public List<PopToken> placesOfPower;
        public List<BossToken> boss;
        public List<LootToken> loots;

        public Location location;
        public double longitude;
        public double latitude;
    }

    public static event System.Action<string> OnChangeDominion;
    public static event System.Action<List<WitchToken>, List<SpiritToken>, List<CollectableToken>, List<EnergyToken>, List<PopToken>, List<BossToken>, List<LootToken>> OnWillSpawnMarkers;
    public static event System.Action<List<WitchMarker>, List<SpiritMarker>, List<CollectableMarker>, List<EnergyMarker>, List<LocationMarker>, List<WorldBossMarker>, List<MuskMarker>> OnSpawnMarkers;

    public static bool IsSpiritForm { get; private set; }
    public static bool IsSpawningTokens { get; private set; }

    public static bool IsGarden { get; private set; }

    private static int m_MoveTweenId;
    private static Coroutine m_SpawnCoroutine;

    private static string m_LastRequestTime;

    public static int WitchCount { get; private set; }

    private static MarkerManagerAPI m_CoroutineBehavior;
    private static MarkerManagerAPI Instance
    {
        get
        {
            if (m_CoroutineBehavior == null)
                m_CoroutineBehavior = new GameObject("MarkerManagerAPI").AddComponent<MarkerManagerAPI>();
            return m_CoroutineBehavior;
        }
    }
    
    public static void GetMarkers(float longitude, float latitude, System.Action callback = null, bool animateMap = true, bool showLoading = false, bool loadMap = false)
    {
        double dist = MapsAPI.Instance.DistanceBetweenPointsD(new Vector2(longitude, latitude), GetGPS.coordinates);
        bool wasPhysical = !IsSpiritForm;
        bool isPhysical = dist < PlayerDataManager.DisplayRadius;

        if (wasPhysical && !isPhysical)
            GetGPS.SetNoise();

        if (PlayerDataManager.IsFTF || LocationIslandController.isInBattle)
        {
            return;
        }

        if (PlayerDataManager.playerData.state == "dead")
        {
            isPhysical = true;
            longitude = GetGPS.longitude;
            latitude = GetGPS.latitude;
            IsSpiritForm = !isPhysical;
        }

        if (isPhysical)
        {
            longitude += GetGPS.noise.x;
            latitude += GetGPS.noise.y;
        }

        var data = new
        {
            physical = isPhysical,
            longitude,
            latitude,
        };

        string dataJson = JsonConvert.SerializeObject(data);

        if (showLoading)
            LoadingOverlay.Show();

        Debug.Log("<color=red>get markers</color>:\n" + dataJson);

        System.Action requestMarkers = () => { };
        IsSpawningTokens = true;
        IsSpiritForm = !isPhysical;

        //////////////////////////////////////////remove any other markers

        //Dictionary<string, List<IMarker>>.ValueCollection values = MarkerSpawner.Markers.Values;
        //List<string> toRemove = new List<string>();

        //foreach (List<IMarker> marker in values)
        //    toRemove.Add(marker[0].Token.Id);

        //foreach (string id in toRemove)
        //    MarkerSpawner.DeleteMarker(id);

        ////////////////////////////////////////

        string timestamp = m_LastRequestTime = Time.time.ToString();
        requestMarkers = () =>
        {
            //move markers to new map position
            foreach (var marker in MarkerSpawner.Markers.Values)
            {
                var token = marker[0].Token;
                marker[0].SetWorldPosition(MapsAPI.Instance.GetWorldPosition(token.longitude, token.latitude));
                MarkerSpawner.Instance.UpdateMarker(marker[0]);
            }

            APIManager.Instance.Post("character/move", dataJson,
            (s, r) =>
            {
                LoadingOverlay.Hide();
                IsSpawningTokens = false;

                if (r == 200)
                    GetMarkersSuccess(timestamp, longitude, latitude, s, r);
                else
                    GetMarkersFailed(longitude, latitude, animateMap, loadMap, s, r);

                callback?.Invoke();
            });
        };

        //stop despawning markers
        if (m_SpawnCoroutine != null)
            Instance.StopCoroutine(m_SpawnCoroutine);

        //pre-move the player marker and load the map at the target position
        if (loadMap)
        {
            LoadMap(longitude, latitude, animateMap, requestMarkers);
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
                GetMarkers(PlayerManager.marker.Coords.x, PlayerManager.marker.Coords.y, callback, animateMap, showLoading, false);
            }
        }
    }

    private static void GetMarkersFailed(float longitude, float latitude, bool animateMap, bool loadMap, string result, int response)
    {
        LeanTween.cancel(m_MoveTweenId);
        Vector3 targetPosition = MapsAPI.Instance.GetWorldPosition(PlayerDataManager.playerData.longitude, PlayerDataManager.playerData.latitude);

        if (Vector3.Distance(targetPosition, PlayerManager.marker.GameObject.transform.position) < 200)
            m_MoveTweenId = LeanTween.move(PlayerManager.marker.GameObject, targetPosition, 1f).setEaseOutCubic().uniqueId;
        else
            PlayerManager.marker.GameObject.transform.position = targetPosition;

        //go back to previous position and show error
        if (loadMap)
        {
            UIGlobalPopup.ShowError(null, LocalizeLookUp.GetText(APIManager.ParseError(result)));
            MapsAPI.Instance.InitMap(
               PlayerDataManager.playerData.longitude,
               PlayerDataManager.playerData.latitude,
               MapsAPI.Instance.normalizedZoom,
               () =>
               {
                   //move markers back
                   foreach (var marker in MarkerSpawner.Markers.Values)
                   {
                       var token = marker[0].Token;
                       marker[0].SetWorldPosition(MapsAPI.Instance.GetWorldPosition(token.longitude, token.latitude));
                   }
               },
               animateMap);
        }
    }

    private static void GetMarkersSuccess(string timestamp, float longitude, float latitude, string result, int response)
    {
        if (timestamp != m_LastRequestTime)
        {
            Debug.LogError("outdated move response");
            return;
        }

        MapMoveResponse moveResponse = JsonConvert.DeserializeObject<MapMoveResponse>(result);

        PlayerManager.marker.Coords = new Vector2(longitude, latitude);
        PlayerDataManager.playerData.longitude = longitude;
        PlayerDataManager.playerData.latitude = latitude;

        //Vector3 worldPos = MapsAPI.Instance.GetWorldPosition(longitude, latitude);
        //PlayerManager.marker.SetWorldPosition(worldPos, 2f);
        //MapCameraUtils.FocusOnPosition(worldPos, true, 3f);

        UpdateDominion(moveResponse.location);

        SpawnMarkers(
            moveResponse.characters,
            moveResponse.spirits,
            moveResponse.items,
            moveResponse.energies,
            moveResponse.placesOfPower,
            moveResponse.boss,
            moveResponse.loots);
    }

    public static void LoadMap(double longitude, double latitude, bool animate, System.Action onComplete = null)
    {
        MapsAPI.Instance.InitMap(
                longitude,
                latitude,
                MapsAPI.Instance.normalizedZoom,
                () =>
                {
                    LeanTween.cancel(m_MoveTweenId);
                    Vector3 targetPosition = MapsAPI.Instance.GetWorldPosition(longitude, latitude);

                    if (PlayerManager.marker != null)
                    {
                        if (Vector3.Distance(targetPosition, PlayerManager.marker.GameObject.transform.position) < 200)
                            m_MoveTweenId = LeanTween.move(PlayerManager.marker.GameObject, targetPosition, 1f).setEaseOutCubic().uniqueId;
                        else
                            PlayerManager.marker.GameObject.transform.position = targetPosition;
                    }

                    onComplete?.Invoke();
                },
                animate);
    }

    public static void UpdateDominion(MapMoveResponse.Location location)
    {
        //update soundtrack
        if (string.IsNullOrWhiteSpace(location.garden))
        {
            IsGarden = false;
            PlayerDataManager.soundTrack = location.music;
            SoundManagerOneShot.Instance.SetBGTrack(location.music);
        }
        else
        {
            IsGarden = true;
            PlayerDataManager.soundTrack = 1;
            SoundManagerOneShot.Instance.SetBGTrack(1);
            PlayerManagerUI.Instance.ShowGardenUI();
        }

        //update zone and dominion
        PlayerDataManager.zone = location.zone;
        if (location.dominion != PlayerDataManager.currentDominion)
        {
            PlayerDataManager.currentDominion = string.IsNullOrEmpty(location.dominion) ? "Ronin" : location.dominion;
            OnChangeDominion?.Invoke(location.dominion);
            if (string.IsNullOrWhiteSpace(location.garden))
            {
                PlayerManagerUI.Instance.ShowDominion(PlayerDataManager.currentDominion);
            }
            else
            {
                PlayerManagerUI.Instance.ShowGarden(location.garden);
            }
        }
    }

    public static void SpawnMarkers(List<WitchToken> witches, List<SpiritToken> spirits, List<CollectableToken> items, List<EnergyToken> energies, List<PopToken> pops, List<BossToken> bosses, List<LootToken> loot)
    {
        //finaly add/update markers
        WitchCount = witches.Count;

        //stop avatar generation
        AvatarSpriteUtil.Instance.ClearQueues();

        m_SpawnCoroutine = Instance.StartCoroutine(SpawnMarkersCoroutine(witches, spirits, items, energies, pops, bosses, loot));
    }

    private static IEnumerator SpawnMarkersCoroutine(
        List<WitchToken> witches,
        List<SpiritToken> spirits,
        List<CollectableToken> items,
        List<EnergyToken> energies,
        List<PopToken> pops,
        List<BossToken> bosses,
        List<LootToken> loot)
    {
        IsSpawningTokens = true;

        OnWillSpawnMarkers?.Invoke(witches, spirits, items, energies, pops, bosses, loot);

        List<WitchMarker> spawnedWitches = new List<WitchMarker>();
        List<SpiritMarker> spawnedSpirits = new List<SpiritMarker>();
        List<CollectableMarker> spawnedCollectables = new List<CollectableMarker>();
        List<EnergyMarker> spawnedEnergies = new List<EnergyMarker>();
        List<LocationMarker> spawnedPops = new List<LocationMarker>();
        List<WorldBossMarker> spawnedBosses = new List<WorldBossMarker>();
        List<MuskMarker> spawnedLoot = new List<MuskMarker>();

        HashSet<IMarker> updatedMarkers = new HashSet<IMarker>();

        IMarker aux;

        Debug.Log($"spawning pops: {pops.Count}");
        for (int i = 0; i < pops.Count; i++)
        {
            aux = MarkerSpawner.Instance.AddMarker(pops[i]);
            if (aux != null)
            {
                updatedMarkers.Add(aux);
                spawnedPops.Add(aux as LocationMarker);
            }
        }
        yield return null;
        
        Debug.Log($"spawning bosses: {bosses?.Count}");
        for (int i = 0; i < bosses?.Count; i++)
        {
            aux = MarkerSpawner.Instance.AddMarker(bosses[i]);
            if (aux != null)
            {
                updatedMarkers.Add(aux);
                spawnedBosses.Add(aux as WorldBossMarker);
            }
        }

        Debug.Log($"spawning loot: {loot?.Count}");
        for (int i = 0; i < loot?.Count; i++)
        {
            aux = MarkerSpawner.Instance.AddMarker(loot[i]);
            if (aux != null)
            {
                updatedMarkers.Add(aux);
                spawnedLoot.Add(aux as MuskMarker);
            }
        }

        yield return null;

        Debug.Log($"spawning collectables: {items.Count}");
        for (int i = 0; i < items.Count; i++)
        {
            aux = MarkerSpawner.Instance.AddMarker(items[i]);
            if (aux != null)
            {
                updatedMarkers.Add(aux);
                spawnedCollectables.Add(aux as CollectableMarker);
            }
        }
        yield return null;

        Debug.Log($"spawning spirits: {spirits.Count}");
        for (int i = 0; i < spirits.Count; i++)
        {
            if (spirits[i].energy == 0)
                continue;
            aux = MarkerSpawner.Instance.AddMarker(spirits[i]);
            if (aux != null)
            {
                updatedMarkers.Add(aux);
                spawnedSpirits.Add(aux as SpiritMarker);
            }
        }

        Debug.Log($"spawning witches: {witches.Count}");
        for (int i = 0; i < witches.Count; i++)
        {
            aux = MarkerSpawner.Instance.AddMarker(witches[i]);
            if (aux != null)
            {
                updatedMarkers.Add(aux);
                spawnedWitches.Add(aux as WitchMarker);
            }

            //yield return null;
        }

        Debug.Log($"spawning energy: {energies.Count}");
        for (int i = 0; i < energies.Count; i++)
        {
            aux = MarkerSpawner.Instance.AddMarker(energies[i]);
            if (aux != null)
            {
                updatedMarkers.Add(aux);
                spawnedEnergies.Add(aux as EnergyMarker);
            }

            if (i % 10 == 0)
                yield return null;
        }

        yield return null;

        OnSpawnMarkers?.Invoke(
            spawnedWitches,
            spawnedSpirits,
            spawnedCollectables,
            spawnedEnergies,
            spawnedPops,
            spawnedBosses,
            spawnedLoot);

        ////////////////////////////////////////remove any other markers

        Dictionary<string, List<IMarker>>.ValueCollection values = MarkerSpawner.Markers.Values;
        List<string> toRemove = new List<string>();

        foreach (List<IMarker> marker in values)
        {
            if (!updatedMarkers.Contains(marker[0]))
                toRemove.Add(marker[0].Token.Id);
        }

        Debug.Log($"despawning tokens: {toRemove.Count}");

        foreach (string id in toRemove)
            MarkerSpawner.DeleteMarker(id, 0);

        //////////////////////////////////////

        m_SpawnCoroutine = null;
        IsSpawningTokens = false;
        yield return null;
    }
}

