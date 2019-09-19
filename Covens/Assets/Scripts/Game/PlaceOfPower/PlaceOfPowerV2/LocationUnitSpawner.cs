using System.Security.Principal;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Raincrow.Maps;
using UnityEngine;
using static MarkerManager;
using Newtonsoft.Json;
using static Raincrow.GameEventResponses.MoveTokenHandlerPOP;
using UnityEngine.UI;

public class LocationUnitSpawner : MonoBehaviour
{
    private static LocationUnitSpawner Instance { get; set; }
    [Header("Witch")]
    public GameObject witchIcon;

    [Header("Spirits")]
    public GameObject spiritIcon;
    public static IMarker guardianMarker { get; private set; }

    public static string guardianInstance
    {
        get
        {
            return ((SpiritToken)guardianMarker.Token).instance;
        }
    }

    [SerializeField] private Transform m_CenterSpiritTransform;
    [SerializeField] private Transform m_PlayerHighlight;
    [SerializeField] private Transform m_SelfSelectionRing;
    [SerializeField] private Transform m_FlightFX;
    [SerializeField] private Transform m_CloakingFX;

    private static SimplePool<Transform> m_WitchPool;
    private static SimplePool<Transform> m_SpiritPool;
    private static SimplePool<Transform> m_EnergyPool;
    private static SimplePool<Transform> m_CloakingPool;
    private static Dictionary<string, GameObject> m_cloaks = new Dictionary<string, GameObject>();
    public static string currentSelection { get; private set; }
    public static Dictionary<string, IMarker> Markers = new Dictionary<string, IMarker>();


    void Awake()
    {
        Instance = this;
        m_PlayerHighlight.gameObject.SetActive(false);
        m_WitchPool = new SimplePool<Transform>(witchIcon.transform, 10);
        m_SpiritPool = new SimplePool<Transform>(spiritIcon.transform, 2);
        // m_EnergyPool = new SimplePool<Transform>(witchIcon.transform, 1);
        m_CloakingPool = new SimplePool<Transform>(m_CloakingFX, 2);
    }

    public static void EnableCloaking(string instance)
    {
        if (Markers.ContainsKey(instance))
        {
            var go = m_CloakingPool.Spawn().gameObject;
            int degree = ((WitchToken)Markers[instance].Token).degree;
            if (degree < 0)
            {
                go.transform.GetChild(0).gameObject.SetActive(true);
            }
            else if (degree > 0)
            {
                go.transform.GetChild(2).gameObject.SetActive(true);
            }
            else
            {
                go.transform.GetChild(1).gameObject.SetActive(true);
            }

            go.transform.SetParent(Markers[instance].GameObject.transform.GetChild(0));

            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.zero;
            go.SetActive(true);
            //  if (m_cloaks.ContainsKey(instance)) DisableCloaking(instance);
            m_cloaks[instance] = go;
            LeanTween.scale(go, Vector3.one, .5f).setEaseOutQuad();
        }
    }

    public static void DisableCloaking(string instance)
    {
        if (m_cloaks.ContainsKey(instance))
        {
            LeanTween.scale(m_cloaks[instance], Vector3.zero, .5f).setEaseOutQuad().setOnComplete(() =>
            {
                m_CloakingPool.Despawn(m_cloaks[instance].transform);
                m_cloaks.Remove(instance);
            });

        }
    }

    public static int GetIsland(string id)
    {
        if (Markers.ContainsKey(id))
        {
            return ((Token)Markers[id].Token).island;
        }
        else
        {
            return -1;
        }
    }

    public void AddMarker(Token token)
    {
        if (!Markers.ContainsKey(token.instance))
        {
            Debug.Log("Adding ");
            GameObject go = null;

            if (token.Type == MarkerType.WITCH)
            {

                go = m_WitchPool.Spawn().gameObject;
                go.name = "[witch] " + (token as WitchToken).displayName + " [" + token.instance + "]";
            }
            else if (token.Type == MarkerType.SPIRIT)
            {
                Debug.Log(JsonConvert.SerializeObject(token));
                go = m_SpiritPool.Spawn().gameObject;
                go.name = "[spirit] " + (token as SpiritToken).spiritId + " [" + token.instance + "]";
            }
            else if (token.Type == MarkerType.ENERGY)
            {
                throw new NotImplementedException("Energy tokens not implemented");
                // go = m_EnergyPool.Spawn().gameObject;
                // go.name = "[Energy] " + (token as CollectableToken).type + " [" + token.instance + "]";
            }
            IMarker marker = go.GetComponent<MuskMarker>();
            Debug.Log(token.popIndex + $" {token.type}");
            if (token.popIndex == -1)
            {
                SetupCenterSpirit(go, marker, token);
                guardianMarker = marker;
            }
            else
                Setup(go, marker, token);
            marker.OnClick += onClickMarker;
            Markers.Add(token.instance, marker);
            if (token.instance == PlayerDataManager.playerData.instance)
            {
                LocationPlayerAction.SetPlayerMarker(marker);
                m_SelfSelectionRing.gameObject.SetActive(true);
                m_SelfSelectionRing.SetParent(go.transform.GetChild(0));
                m_SelfSelectionRing.localPosition = Vector3.zero;
                m_SelfSelectionRing.localScale = Vector3.one * 1.5f;
                m_FlightFX.SetParent(go.transform.GetChild(0));
                m_FlightFX.localPosition = Vector3.zero;
                m_FlightFX.localScale = Vector3.one * 1.2f;
                SetSelfDegreeRing();
            }
        }
        else
        {
            if (!isPositionOccupied(token.popIndex))
            {
                Markers[token.instance].GameObject.transform.SetParent(GetTransform(token));
                Markers[token.instance].InitializePositionPOP();
                return;
            }
            else
            {
                string curInstance = GetTokenAtIndex(token.popIndex).instance;
                if (token.instance == curInstance)
                {
                    Debug.LogError("Marker already at position");
                }
                else
                {
                    Debug.LogError("Position is occupied");
                }
            }

        }

    }

    private void onClickMarker(IMarker m)
    {
        var token = m.Token as Token;
        currentSelection = token.instance;
        if (!LocationPlayerAction.CanSelectIsland(token))
        {
            LocationPlayerAction.ShowMoveCloser();
            DisableMarkers(token.instance);
            return;
        }
        LocationPlayerAction.HideActions();
        if (token.Type == MarkerType.WITCH)
        {
            LocationPlayerAction.ShowSpells();
            UIPlayerInfo.Show(m as WitchMarker, token as WitchToken);
        }
        else if (token.Type == MarkerType.SPIRIT)
        {
            LocationPlayerAction.ShowSpells();
            UISpiritInfo.Show(m as SpiritMarker, token as SpiritToken);
        }
        SetHighlight(token);
        LocationIslandController.moveCamera(m.AvatarTransform.position);
        DisableMarkers(token.instance);
        GetMarkerDetails(token.instance, (result, response) =>
        {
            if (result == 200)
            {
                Debug.Log(response);
                if (m.Type == MarkerType.WITCH)
                {
                    SelectWitchData_Map witch = JsonConvert.DeserializeObject<SelectWitchData_Map>(response);
                    witch.token = token as WitchToken;

                    if (UIPlayerInfo.IsShowing && UIPlayerInfo.WitchToken.instance == token.instance)
                    {
                        UIPlayerInfo.SetupDetails(witch);
                        UIQuickCast.UpdateCanCast(m, witch);
                    }
                }
                else if (m.Type == MarkerType.SPIRIT)
                {
                    SelectSpiritData_Map spirit = JsonConvert.DeserializeObject<SelectSpiritData_Map>(response);
                    spirit.token = token as SpiritToken;

                    if (UISpiritInfo.IsShowing && UISpiritInfo.SpiritToken.instance == token.instance)
                    {
                        UISpiritInfo.SetupDetails(spirit);
                        UIQuickCast.UpdateCanCast(m, spirit);
                    }

                    if (spirit.state == "dead")
                    {
                        throw new NotImplementedException("Handle dead spirit logic");
                        //handle remove logic
                    }
                }
            }
            else
            {
                Debug.LogError("select marker error [" + result + "] " + response);
            }
        });
    }

    public static void SetHighlight(Token token)
    {
        if (token.popIndex != -1)
        {
            Instance.m_PlayerHighlight.SetParent(Instance.GetTransform(token));
            Instance.m_PlayerHighlight.localScale = Vector3.one;
            Instance.m_PlayerHighlight.localPosition = Vector3.zero;
        }
        else
        {
            Instance.m_PlayerHighlight.SetParent(Instance.GetTransform(token).GetChild(0));
            Instance.m_PlayerHighlight.localPosition = Vector3.zero;
            Instance.m_PlayerHighlight.localScale = new Vector3(7.5f, 3.8f, 7.6f);
        }
        Instance.m_PlayerHighlight.gameObject.SetActive(true);
    }

    public static void GetMarkerDetails(string id, System.Action<int, string> callback)
    {
        APIManager.Instance.Get(
                    "character/select/" + id + "?selection=placeOfPower",
                    "",
                    (response, result) => callback(result, response));
    }

    public async void RemoveMarker(string instance, bool remove = true)
    {
        if (Markers.ContainsKey(instance))
        {
            var marker = Markers[instance];
            marker.SetAlpha(0, 1);
            marker.Interactable = false;
            if (remove)
                Markers.Remove(instance);
            await Task.Delay(2000);
            Debug.Log("Despawning");
            marker.OnDespawn();
            if (marker.Type == MarkerType.WITCH) m_WitchPool.Despawn(marker.GameObject.transform);
            else if (marker.Type == MarkerType.SPIRIT) m_SpiritPool.Despawn(marker.GameObject.transform);
            else throw new NotImplementedException("Unhandled Marker Type: " + marker.Type);
        }
    }

    public void RemoveAllMarkers()
    {
        foreach (var item in Markers)
        {
            RemoveMarker(item.Key, false);
        }
        Markers.Clear();
    }

    public static void OnEnergyChange(IMarker m, int energy)
    {
        string instance = ((Token)m.Token).instance;
        if (energy <= 0 && Markers.ContainsKey(instance) && PlayerDataManager.playerData.instance != instance)
        {
            Debug.Log("Removing Marker");
            Instance.RemoveMarker(instance);
        }
    }

    public static void UnloadScene()
    {
        Debug.Log("<color=green>Unloading POP Scene</color>");
        Debug.Log(LocationIslandController.isInBattle);
        m_WitchPool.DespawnAll();
        m_SpiritPool.DespawnAll();
        // m_EnergyPool.DespawnAll();
        Markers.Clear();
    }

    public async void MoveMarker(MoveEventDataPOP data)
    {
        if (Markers.ContainsKey(data.instance))
        {
            var marker = GetMarker(data.instance);
            // TODO add fx
            if (data.instance != PlayerDataManager.playerData.instance)
            {
                marker.SetAlpha(0, .7f);
                await Task.Delay(700);
                marker.SetAlpha(1, .7f);
            }
            var mToken = marker.Token as Token;
            mToken.position = data.position;
            mToken.island = data.island;

            if (!isPositionOccupied(mToken.popIndex))
            {
                marker.GameObject.transform.SetParent(GetTransform(mToken));
                marker.InitializePositionPOP();
            }
            else
            {
                Debug.LogError($"Index Not Empty: {data.instance} \n {mToken.popIndex} \n isl. {data.island} pos {data.position}");
            }
        }
        else
        {
            Debug.LogError("Marker not found to move: " + data.instance);
        }
    }

    public static void MoveWitch(int island, int position)
    {
        Debug.Log("requesting move");
        var data = new { island = island, position = position };
        APIManager.Instance.Post("character/move", JsonConvert.SerializeObject(data),
            async (s, r) =>
            {
                if (r == 200)
                {
                    var charScale = LocationPlayerAction.playerMarker.AvatarTransform.localScale;
                    LocationPlayerAction.playerMarker.SetAlpha(0, 1);
                    LeanTween.scale(LocationPlayerAction.playerMarker.AvatarTransform.gameObject, Vector3.zero, .5f).setEaseOutCubic();
                    ShowFlightFX();
                    SoundManagerOneShot.Instance.PlayWooshShort();

                    await Task.Delay(600);

                    MoveEventDataPOP moveData = new MoveEventDataPOP
                    {
                        instance = LocationPlayerAction.playerWitchToken.instance,
                        island = island,
                        position = position,
                    };
                    SoundManagerOneShot.Instance.PlayLandFX();
                    LocationPlayerAction.playerMarker.SetAlpha(1, 1);
                    LeanTween.scale(LocationPlayerAction.playerMarker.AvatarTransform.gameObject, charScale, .5f).setEaseOutCubic();

                    Instance.MoveMarker(moveData);
                    LocationIslandController.SetActiveIslands();
                }
            });
    }

    public static IMarker GetMarker(string instance)
    {
        if (Markers.ContainsKey(instance))
            return Markers[instance];
        return null;
    }

    private void Setup(GameObject g, IMarker m, Token t)
    {
        m.Interactable = true;
        m.Setup(t);
        m.EnableAvatar();
        m.AvatarTransform.localScale = Vector3.one * 1.3f;
        Debug.Log(g.name);
        g.transform.SetParent(GetTransform(t));
        m.InitializePositionPOP();
        // m.EnablePopSorting();
        m.GameObject.SetActive(true);

    }

    private async static void ShowFlightFX()
    {
        var SelectionRing = LocationPlayerAction.playerMarker.GameObject.transform.GetChild(0).GetChild(4);
        LeanTween.scale(SelectionRing.gameObject, Vector3.zero, .6f).setEase(LeanTweenType.easeInOutQuad);
        await Task.Delay(600);
        LeanTween.scale(SelectionRing.gameObject, Vector3.one * 1.5f, .6f).setEase(LeanTweenType.easeInOutQuad);
        var selfToken = LocationPlayerAction.playerWitchToken;
        var FlightFX = LocationPlayerAction.playerMarker.GameObject.transform.GetChild(0).GetChild(5);
        if (FlightFX.gameObject.activeInHierarchy)
            FlightFX.gameObject.SetActive(false);

        FlightFX.gameObject.SetActive(true);
        await Task.Delay(5000);
        FlightFX.gameObject.SetActive(false);
    }

    private static void SetSelfDegreeRing()
    {
        var selfToken = LocationPlayerAction.playerWitchToken;
        var SelectionRing = LocationPlayerAction.playerMarker.GameObject.transform.GetChild(0).GetChild(4);
        foreach (Transform item in SelectionRing)
        {
            item.gameObject.SetActive(false);
        }
        if (selfToken.degree > 0)
        {
            SelectionRing.GetChild(2).gameObject.SetActive(true);
        }
        else if (selfToken.degree < 0)
        {
            SelectionRing.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            SelectionRing.GetChild(1).gameObject.SetActive(true);
        }
    }

    private void SetupCenterSpirit(GameObject g, IMarker m, Token t)
    {
        m.Interactable = true;
        m.Setup(t);
        m.EnableAvatar();
        m.AvatarTransform.localScale = Vector3.one * 2.5f;
        g.transform.SetParent(m_CenterSpiritTransform);
        m.InitializePositionPOP();
        //  m.EnablePopSorting();
        m.GameObject.SetActive(true);
    }

    private Transform GetTransform(Token token)
    {
        try
        {
            return LocationIslandController.unitPositions[token.popIndex];

        }
        catch
        {
            return m_CenterSpiritTransform;
        }
    }

    private bool isPositionOccupied(int index)
    {
        if (index > LocationIslandController.unitPositions.Count - 1)
            throw new System.Exception("Position Out of bounds");
        return LocationIslandController.unitPositions[index].childCount == 3;
    }

    private Token GetTokenAtIndex(int index)
    {
        if (isPositionOccupied(index))
        {
            return LocationIslandController.unitPositions[index].GetChild(2).GetComponent<MuskMarker>().Token;
        }
        return null;
    }

    public static void DisableMarkers(string instance = null)
    {
        foreach (var item in Markers)
        {
            if (instance == null)
            {
                item.Value.SetAlpha(.22f, 1);
                item.Value.Interactable = false;
            }
            else
            {
                if (instance != item.Key)
                {
                    item.Value.SetAlpha(.22f, 1);
                    item.Value.Interactable = true;
                }
            }
        }
    }

    public static void EnableMarkers()
    {
        UIQuickCast.Close();
        foreach (var item in Markers)
        {
            item.Value.SetAlpha(1, 1);
            item.Value.Interactable = true;
        }
    }

}