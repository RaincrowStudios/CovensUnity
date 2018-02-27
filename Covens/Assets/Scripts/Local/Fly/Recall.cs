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
	void Update () {
		if (move) {
			t += Time.deltaTime * speed;
			if (t < 1) {
				OM.position = Vector2.Lerp (oldPos, pos, t);
				PlayerManager.marker.position = OM.position;
			} else {
				OM.position = pos;
				PlayerManager.marker.position = OM.position;
				OnlineMapsControlBase3D.instance.RemoveMarker3D (PlayerManager.physicalMarker);
				PlayerManager.inSpiritForm = false;
				PlayerManager.physicalMarker = null;
				PlayerManager.Instance.returnphysicalSound ();
				move = false;
			}
		}
	}

	public void RecallHome()
	{
		if (PlayerManager.physicalMarker != null) {
			pos = PlayerManager.physicalMarker.position;
			oldPos = OM.position;
			t = 0;
			move = true;
			GetComponent<PlayerManagerUI> ().home ();
		}
	}
}
