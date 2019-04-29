using UnityEngine;
using System.Collections;
using Raincrow.Maps;

public static class OnMapShout
{
    public static SimplePool<ShoutBoxData> m_ShoutPool = new SimplePool<ShoutBoxData>("UI/ShoutBox");

    public static void HandleEvent(WSData data)
    {
        if (data.instance == PlayerDataManager.playerData.instance)
        {
            ShoutBoxData shoutBox = m_ShoutPool.Spawn();
            shoutBox.Setup(PlayerManager.marker, data.displayName, data.shout, () => m_ShoutPool.Despawn(shoutBox));
            shoutBox.transform.localPosition = new Vector3(0, 32, 0);
        }
        else
        {
            if (MarkerManager.Markers.ContainsKey(data.instance))
            {
                IMarker marker = MarkerManager.GetMarker(data.instance);

                if (marker.inMapView)
                {
                    ShoutBoxData shoutBox = m_ShoutPool.Spawn();
                    shoutBox.Setup(marker, data.displayName, data.shout, () => m_ShoutPool.Despawn(shoutBox));
                    marker.AddCharacterChild(shoutBox.transform, () => m_ShoutPool.Despawn(shoutBox));
                    shoutBox.transform.localPosition = new Vector3(0, 32, 0);
                }
            }
        }
    }
}
