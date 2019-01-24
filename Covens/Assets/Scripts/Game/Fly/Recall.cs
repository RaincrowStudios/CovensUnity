using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recall : MonoBehaviour
{
    public static Recall Instance { get; private set; }

	 Vector2 pos,oldPos;
	float t;
	public float speed =1;
	bool move;

    private void Awake()
    {
        Instance = this;
    }

    public void RecallHome()
	{
		if (PlayerManager.physicalMarker != null) {
			pos = PlayerManager.physicalMarker.position;
			oldPos = MapsAPI.Instance.position;
			MapsAPI.Instance.position = PlayerManager.physicalMarker.position;
            MapsAPI.Instance.RemoveMarker(PlayerManager.physicalMarker);
			PlayerDataManager.playerPos = MapsAPI.Instance.position;
			PlayerManager.inSpiritForm = false;
			PlayerManager.physicalMarker = null;
			PlayerManager.Instance.returnphysicalSound ();
			PlayerManager.Instance.ReSnapMap ();
			GetComponent<PlayerManagerUI> ().home ();
			MarkerManagerAPI.GetMarkers (true);
		} else {
			MarkerManagerAPI.GetMarkers (true);
			PlayerManager.Instance.ReSnapMap ();
			MapsAPI.Instance.position = PlayerManager.marker.position;
		}
	}
		
}
