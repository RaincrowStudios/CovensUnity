using UnityEngine;
using System.Collections;
using Newtonsoft.Json;
using WebSocketSharp;
using System.Collections.Generic;

public class GetLabels : MonoBehaviour
{
	 WebSocket client;
	public static GetLabels instance {get;set;}
	SpriteMapsController sp;
	HeatMapManager hp;
	public DynamicLabelManager DLM;
	void Awake()
	{
		instance = this;
	}
		

	// Use this for initialization
	IEnumerator Start ()
	{

		sp = GetComponent<SpriteMapsController> ();
		hp = HeatMapManager.instance;
//		client = new WebSocket(new System.Uri("ws://192.168.11.214:1337"));
		client = new WebSocket(new System.Uri("ws://localhost:1337"));
		yield return StartCoroutine (client.Connect ());
//		client.Send (System.Text.Encoding.UTF8.GetBytes("hmap"));
		while (true) {
			string reply = client.RecvString ();
			if (reply != null) {
//				print (reply);
				var data = JsonConvert.DeserializeObject<WSResponse> (reply);
				DLM.GenerateLabels (data);
			}
			yield return 0;
		}
	}

	public void RequestLabel(Vector2 pos,float zoom){
//		LabelRequest
		string type = "";
		int distance = 0;

		if (zoom == 1) {
			type = "city";
			distance = 350000;
		} else if (zoom == 2) {
			type = "town";
			distance = 150000;
		}else if (zoom == 3) {
			type = "markers";
			distance = 80000;
		}
		LabelRequest req = new LabelRequest {
			latitude = pos.x,
			longitude = pos.y,
			type = type,
			distance = distance
		};
		string k = JsonConvert.SerializeObject (req);
		client.Send (System.Text.Encoding.UTF8.GetBytes(k ));
	}

}

public class WSResponse{
	public List<LabelResponse> labels{ get; set;}
	public string command {get;set;}
//	List<LabelResponse> labels{ get; set;}
}

public class LabelRequest{
	public float latitude;
	public float longitude;
	public string type;
	public int distance;
}

public class LabelResponse{
	public string name;
	public string type;
	public float latitude;
	public float longitude;
	public int zoom;
	public int count;
}

