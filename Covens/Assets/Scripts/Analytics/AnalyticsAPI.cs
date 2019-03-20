using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.Analytics
{
    public class AnalyticsAPI : MonoBehaviour
    {
        private static AnalyticsAPI m_Instance;
        public static AnalyticsAPI Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new GameObject("AnalyticsAPI").AddComponent<AnalyticsAPI>();
                }
                return m_Instance;
            }
        }

        [Header("Settings")]
        [SerializeField] private bool m_UseTimerToSend = true;
        [SerializeField] private float m_SendEventsCooldown = 30;

        private bool m_Initialized = false;
        private System.Int32 m_SessionStart;
        private IAnalyticsWrapper m_AnalyticsAPI;
        private List<Dictionary<string, object>> m_EventLog = new List<Dictionary<string, object>>();
        private int m_SendRetryCount;
        private string m_SessionId = null;

        private void Awake()
        {
            if (m_Instance != null && m_Instance != this)
            {
                Destroy(this.gameObject);
                return;
            }

            m_Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        public void InitSession()
        {
            if (m_Initialized)
                return;

            Debug.Log("initializing analytics session");

            m_SessionStart = Utilities.GetUnixTimestamp(System.DateTime.UtcNow);
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                { "displayName", PlayerDataManager.playerData.displayName },
                { "timestamp", m_SessionStart },
                { "platform", SystemInfo.operatingSystem }
            };

            APIManager.Instance.PostAnalytics("analytics/start", Newtonsoft.Json.JsonConvert.SerializeObject(data), (response, result) =>
            {
                if (result == 200)
                {
                    Debug.Log("analytics initialized");
                    m_Initialized = true;

                    AnalyticsSession session = Newtonsoft.Json.JsonConvert.DeserializeObject<AnalyticsSession>(response);
                    m_SessionId = session.SessionId;

                    StartCoroutine(ScheduleSend());
                    //Debug.LogError("todo: set session id");
                }
                else
                {
#if UNITY_EDITOR
                    Debug.Log("debug localization initialized");
                    m_Initialized = true;
                    StartCoroutine(ScheduleSend());
                    m_SessionId = "debug_session_id";
#endif
                }
            });
        }

        private void EndSession()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["sessionId"] = m_SessionId;
            data["log"] = m_EventLog;

            Debug.Log("ending analytics session");
            APIManager.Instance.PostAnalytics("analytics/end", Newtonsoft.Json.JsonConvert.SerializeObject(data), (response, result) =>
            {
                if (result == 200)
                {
                    Debug.Log("session ended succesfuly");
                }
                else
                {
                    Debug.Log("failed");
                    m_SendRetryCount += 1;
                    if (m_SendRetryCount < 3)
                        EndSession();
                }
            });
        }
        
        private void OnDestroy()
        {
            m_SendRetryCount = 0;
            EndSession();
        }

        private IEnumerator ScheduleSend()
        {
            while (m_UseTimerToSend)
            {
                yield return new WaitForSeconds(m_SendEventsCooldown);
                SendLogToServer();
            }
        }


        public void LogEvent(string id)
        {
            LogEvent(id, new Dictionary<string, object>());
        }

        public void LogEvent(string id, Dictionary<string, object> data)
        {
            if (m_Initialized == false)
                return;

            if (string.IsNullOrEmpty(id))
            {
                throw new System.ArgumentNullException();
            }

            if (data == null)
                data = new Dictionary<string, object>();

            data["event"] = id;
            data["time"] = Utilities.GetUnixTimestamp(System.DateTime.UtcNow);
            m_EventLog.Add(data);

            if (m_AnalyticsAPI != null)
                m_AnalyticsAPI.OnEvent(data);
        }


        public void SendLogToServer()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["sessionId"] = m_SessionId;
            data["log"] = m_EventLog;

            if (m_EventLog != null && m_EventLog.Count > 0)
            {
                string datastring = Newtonsoft.Json.JsonConvert.SerializeObject(data);

                m_SendRetryCount = 0;
                SendServerAPI("analytics/log", datastring);
            }            
        }

        private void SendServerAPI(string endpoint, string data)
        {
            APIManager.Instance.PostAnalytics(endpoint, data, (response, result) =>
            {
                if (result == 200)
                {
                    m_EventLog.Clear();
                }
                else
                {
                    m_SendRetryCount += 1;
                    if (m_SendRetryCount < 3)
                        SendServerAPI(endpoint, data);
                }
            });
        }

        [ContextMenu("log event list")]
        private void LogEvents()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["sessionId"] = m_SessionId;
            data["log"] = m_EventLog;
            string datastring = Newtonsoft.Json.JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented);
            Debug.Log(datastring);
        }
        [ContextMenu("cleat event list")]
        private void CLearEvents()
        {
            m_EventLog.Clear();
        }
    }
}