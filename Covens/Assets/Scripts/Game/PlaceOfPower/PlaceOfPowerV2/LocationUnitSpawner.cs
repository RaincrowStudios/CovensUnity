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
            if (!isPositionOccupied(token.popIndex))
            {
                Markers[token.instance].SetWorldPosition(GetWorldPosition(token));
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
            marker.Interactable = true;
            marker.Setup(token);
            marker.GameObject.SetActive(true);
            marker.OnClick += onClickMarker;
        }

    }

    private void onClickMarker(IMarker m)
    {
        var token = m.Token as Token;
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
                if (m.Type == MarkerType.WITCH)
                {
                    SelectWitchData_Map witch = JsonConvert.DeserializeObject<SelectWitchData_Map>(response);
                    witch.token = token as WitchToken;

                    if (UIPlayerInfo.isShowing && UIPlayerInfo.Instance.WitchToken.instance == token.instance)
                        UIPlayerInfo.Instance.SetupDetails(witch);
                }
                else if (m.Type == MarkerType.SPIRIT)
                {
                    SelectSpiritData_Map spirit = JsonConvert.DeserializeObject<SelectSpiritData_Map>(response);
                    spirit.token = token as SpiritToken;

                    if (UISpiritInfo.isOpen && UISpiritInfo.Instance.SpiritToken.instance == token.instance)
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
            marker.Interactable = false;
            Markers.Remove(instance);
            await Task.Delay(2000);
            marker.OnDespawn();
            if (marker.Type == MarkerType.WITCH) m_WitchPool.Despawn(marker.GameObject.transform);
            else if (marker.Type == MarkerType.SPIRIT) m_SpiritPool.Despawn(marker.GameObject.transform);
            else throw new NotImplementedException("Unhandled Marker Type: " + marker.Type);
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
                var mToken = marker.Token as Token;
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

    private Vector3 GetWorldPosition(Token token)
    {
        return LocationIslandController.unitPositions[token.popIndex].position;
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
            return LocationIslandController.unitPositions[index].GetChild(0).GetComponent<MuskMarker>().Token;
        }
        return null;
    }
}