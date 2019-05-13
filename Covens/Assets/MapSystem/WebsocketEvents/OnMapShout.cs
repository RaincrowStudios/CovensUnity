using UnityEngine;
using System.Collections;
using Raincrow.Maps;

public static class OnMapShout
{
    public static SimplePool<Transform> m_ShoutPool = new SimplePool<Transform>("UI/ShoutBox");

    public static void HandleEvent(WSData data)
    {
        if (data.instance == PlayerDataManager.playerData.instance)
        {
            ShoutBoxData shoutBox = m_ShoutPool.Spawn().GetComponent<ShoutBoxData>();
            PlayerManager.marker.AddChild(shoutBox.transform, PlayerManager.marker.characterTransform, m_ShoutPool);
            shoutBox.Setup(PlayerManager.marker, data.displayName, data.shout, () => PlayerManager.marker.RemoveChild(shoutBox.transform));
            shoutBox.transform.localPosition = new Vector3(0, 32, 0);
        }
        else
        {
            if (MarkerManager.Markers.ContainsKey(data.instance))
            {
                IMarker marker = MarkerManager.GetMarker(data.instance);
                if (marker != null)
                {
                    marker.SpawnFX(m_ShoutPool, true, 5, false, (boxTransform) =>
                    {
                        ShoutBoxData shoutBox = boxTransform.GetComponent<ShoutBoxData>();
                        shoutBox.Setup(marker, data.displayName, data.shout, null);
                        shoutBox.transform.localPosition = new Vector3(0, 32, 0);
                    });
                }
            }
        }
    }
}
