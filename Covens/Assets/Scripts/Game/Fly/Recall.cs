using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recall : MonoBehaviour {

	OnlineMaps OM;

	 Vector2 pos,oldPos;
	float t;
	public float speed =1;
	bool move;
	// Use this for initialization
	void Start () {
		OM = OnlineMaps.instance;
	}
	
	// Update is called once per frame


	public void RecallHome()
	{
		if (PlayerManager.physicalMarker != null) {
			pos = PlayerManager.physicalMarker.position;
			oldPos = OM.position;
			OM.position = PlayerManager.physicalMarker.position;
			OnlineMapsControlBase3D.instance.RemoveMarker3D (PlayerManager.physicalMarker);
			PlayerDataManager.playerPos = OM.position;
			PlayerManager.inSpiritForm = false;
			PlayerManager.physicalMarker = null;
			PlayerManager.Instance.returnphysicalSound ();
			PlayerManager.Instance.ReSnapMap ();
			GetComponent<PlayerManagerUI> ().home ();
			MarkerManagerAPI.GetMarkers (true);
		} else {
			PlayerManager.Instance.ReSnapMap ();
			OnlineMaps.instance.position = PlayerManager.marker.position;
		}
	}
		
}
