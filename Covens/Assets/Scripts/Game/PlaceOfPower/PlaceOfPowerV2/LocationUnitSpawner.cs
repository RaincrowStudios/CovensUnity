using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Raincrow.Maps;
using UnityEngine;
using static MarkerManager;
using Newtonsoft.Json;
using static Raincrow.GameEventResponses.MoveTokenHandlerPOP;

public class LocationUnitSpawner : MonoBehaviour
{
    [Header("Witch")]
    public GameObject witchIcon;

    [Header("Spirits")]
    public GameObject spiritIcon;

    private static SimplePool<Transform> m_WitchPool;
    private static SimplePool<Transform> m_SpiritPool;
    private static SimplePool<Transform> m_EnergyPool;

    public static Dictionary<string, IMarker> Markers = new Dictionary<string, IMarker>();

    void Awake()
    {
        m_WitchPool = new SimplePool<Transform>(witchIcon.transform, 10);
        m_SpiritPool = new SimplePool<Transform>(witchIcon.transform, 2);
        m_EnergyPool = new SimplePool<Transform>(witchIcon.transform, 1);
    }

    public void AddMarker(Token token)
    {
        if (Markers.ContainsKey(token.instance))
        {
            if (!isPositionOccupied(GetIndex(token)))
            {
                Markers[token.instance].SetWorldPosition(GetWorldPosition(token));
                return;
            }
            else
            {
                string curInstance = GetTokenAtIndex(GetIndex(token)).instance;
                if (token.instance == curInstance)
                {
                    Debug.LogError("Marker already at position");
                }
                else
                {
                    Debug.LogError("Position is occupied");
                }
            }

            GameObject go = null;

            if (token.Type == MarkerType.WITCH)
            {
                go = m_WitchPool.Spawn().gameObject;
                go.name = "[witch] " + (token as WitchToken).displayName + " [" + token.instance + "]";
            }
            else if (token.Type == MarkerType.SPIRIT)
            {
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
            marker.interactable = true;
            marker.customData = token;
            marker.gameObject.SetActive(true);
            marker.Setup(token);
            marker.OnClick += onClickMarker;
        }

    }

    private void onClickMarker(IMarker m)
    {
        var token = m.customData as Token;
        if (token.Type == MarkerType.WITCH)
        {
            UIPlayerInfo.Instance.Show(m as WitchMarker, token as WitchToken);
        }
        else if (token.Type == MarkerType.SPIRIT)
        {
            UISpiritInfo.Instance.Show(m, token);
        }
        MarkerSpawner.GetMarkerDetails(token.instance, (result, response) =>
        {
            if (result == 200)
            {
                if (m.type == MarkerType.WITCH)
                {
                    MapWitchData witch = JsonConvert.DeserializeObject<MapWitchData>(response);
                    witch.token = token as WitchToken;

                    if (UIPlayerInfo.isShowing && UIPlayerInfo.Instance.Witch.instance == token.instance)
                        UIPlayerInfo.Instance.SetupDetails(witch);
                }
                else if (m.type == MarkerType.SPIRIT)
                {
                    MapSpiritData spirit = JsonConvert.DeserializeObject<MapSpiritData>(response);
                    spirit.token = token as SpiritToken;

                    if (UISpiritInfo.isOpen && UISpiritInfo.Instance.Spirit.instance == token.instance)
                        UISpiritInfo.Instance.SetupDetails(spirit);

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

    public async void RemoveMarker(string instance)
    {
        if (Markers.ContainsKey(instance))
        {
            var marker = Markers[instance];
            marker.SetAlpha(0, 1);
            marker.interactable = false;
            Markers.Remove(instance);
            await Task.Delay(2000);
            marker.OnDespawn();
            if (marker.type == MarkerType.WITCH) m_WitchPool.Despawn(marker.gameObject.transform);
            else if (marker.type == MarkerType.SPIRIT) m_SpiritPool.Despawn(marker.gameObject.transform);
            else throw new NotImplementedException("Unhandled Marker Type: " + marker.type);
        }
    }

    public void MoveMarker(MoveEventDataPOP data)
    {
        if (Markers.ContainsKey(data.instance))
        {
            var marker = GetMarker(data.instance);
            marker.SetAlpha(0, 1, () =>
            {
                // TODO add fx
                var mToken = marker.customData as Token;
                mToken.position = data.position;
                mToken.island = data.island;
                AddMarker(mToken);
                marker.SetAlpha(1, 0, () =>
                {
                    //fade in animation
                });

            });
        }
        else
        {
            Debug.LogError("Marker not found to move: " + data.instance);
        }
    }

    //Helper methods

    public static IMarker GetMarker(string instance)
    {
        if (Markers.ContainsKey(instance))
            return Markers[instance];
        return null;
    }

    private int GetIndex(Token token)
    {
        return token.island * 3 + token.position - 1;
    }

    private Vector3 GetWorldPosition(Token token)
    {
        return LocationIslandController.unitPositions[GetIndex(token)].position;
    }

    private bool isPositionOccupied(int index)
    {
        if (index > LocationIslandController.unitPositions.Count - 1)
            throw new System.Exception("Position Out of bounds");
        return LocationIslandController.unitPositions[index].childCount == 1;
    }

    private Token GetTokenAtIndex(int index)
    {
        if (isPositionOccupied(index))
        {
            return LocationIslandController.unitPositions[index].GetChild(0).GetComponent<MuskMarker>().m_Data;
        }
        return null;
    }
}