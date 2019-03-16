using UnityEngine;
using System.Collections;
using Newtonsoft.Json;
using WebSocketSharp;
using System.Collections.Generic;

public class GetLabels : MonoBehaviour
{
    WebSocket client;
    public static GetLabels instance { get; set; }
    HeatMapManager hp;
    public DynamicLabelManager DLM;
    void Awake()
    {
        instance = this;
    }

    //ToDo add reconnect feature
    // Use this for initialization
    IEnumerator Start()
    {

        hp = HeatMapManager.instance;

        client = new WebSocket(new System.Uri("ws://35.237.95.2:8081"));
        //  client = new WebSocket(new System.Uri("ws://localhost:90"));
        yield return client.Connect();
        while (true)
        {
            string reply = client.RecvString();
            if (reply != null)
            {
                var data = JsonConvert.DeserializeObject<WSResponse>(reply);
                if (data.command == "markers")
                    DLM.GenerateLabels(data);
            }
            yield return 0;
        }
    }

    public void RequestLabel(Vector2 pos, int distance)
    {
        LabelRequest req = new LabelRequest
        {
            latitude = pos.x,
            longitude = pos.y,
            type = "markers",
            distance = distance
        };
        string k = JsonConvert.SerializeObject(req);
        client.Send(System.Text.Encoding.UTF8.GetBytes(k));
    }

}

public class WSResponse
{
    public List<LabelResponse> labels { get; set; }
    public string command { get; set; }
    //	List<LabelResponse> labels{ get; set;}
}

public class LabelRequest
{
    public float latitude;
    public float longitude;
    public string type;
    public int distance;
}

public class LabelResponse
{
    public string name;
    public string type;
    public float latitude;
    public float longitude;
    public int zoom;
    public int count;
}

