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
        [SerializeField] private bool m_UseTimerToSend = false;
        [SerializeField] private float m_SendEventsCooldown = 600;

        private bool m_Initialized = false;
        private System.DateTime m_SessionStart;
        private IAnalyticsWrapper m_AnalyticsAPI;
        private List<Dictionary<string, object>> m_EventLog = new List<Dictionary<string, object>>();
        private int m_SendRetryCount;

        private void Awake()
        {
            if (m_Instance != null && m_Instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            DontDestroyOnLoad(this.gameObject);
        }

        public void Init()
        {
            m_SessionStart = System.DateTime.UtcNow;

            Debug.Log("TODO: Init analytics");
            m_Initialized = true;
            StartCoroutine(ScheduleSend());
        }
        
        private void OnDestroy()
        {
            SendLogToServer();
        }

        private IEnumerator ScheduleSend()
        {
            while (m_UseTimerToSend)
            {
                yield return new WaitForSeconds(m_SendEventsCooldown);
                SendLogToServer();
            }
        }

        public void SetUserProperty(string id, object value)
        {
            if (m_AnalyticsAPI != null)
                m_AnalyticsAPI.OnUserProperty(id, value);
        }

        public void LogEvent(string id)
        {
            LogEvent(id, new Dictionary<string, object>());
        }

        public void LogEvent(string id, Dictionary<string, object> data)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new System.ArgumentNullException();
            }

            if (data == null)
                data = new Dictionary<string, object>();

            data["id"] = id;
            data["time"] = Time.time;
            m_EventLog.Add(data);

            if (m_AnalyticsAPI != null)
                m_AnalyticsAPI.OnEvent(data);
        }

        public void SendLogToServer()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["session"] = m_SessionStart.Ticks;
            data["log"] = m_EventLog;
            string datastring = Newtonsoft.Json.JsonConvert.SerializeObject(data);

            m_SendRetryCount = 0;
            SendServerAPI("/analytics/log", datastring);
        }

        private void SendServerAPI(string endpoint, string data)
        {
            APIManager.Instance.PostCoven(endpoint, data, (response, result) =>
            {
                if (result == 200)
                {
                    m_EventLog.Clear();
                }
                else
                {
                    m_SendRetryCount += 1;
                    SendServerAPI(endpoint, data);
                }
            });
        }
    }
}