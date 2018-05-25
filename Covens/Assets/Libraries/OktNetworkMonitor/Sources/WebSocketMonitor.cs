using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Oktagon.Network
{

    public class WebSocketMonitor : IMonitor
    {
        private OktNetworkMonitor m_pMonitor;
        private bool m_bRecord = true;

        public bool Record
        {
            get { return m_bRecord; }
            set { m_bRecord = value; }
        }


        /// <summary>
        /// set it up
        /// </summary>
        /// <param name="pMonitor"></param>
        public void SetupMonitor(OktNetworkMonitor pMonitor)
        {
            m_pMonitor = pMonitor;
            WebSocketClient.OnResponseEvt += WebSocketClient_OnResponseEvt;
        }
        public void Destroy()
        {
            m_pMonitor = null;
            WebSocketClient.OnResponseEvt -= WebSocketClient_OnResponseEvt;
        }

        private void WebSocketClient_OnResponseEvt(string obj)
        {
            // bake them
            OktNetworkMonitor.RecordData pData = new OktNetworkMonitor.RecordData();

            pData.Table = "WebSocketClient";
            pData.Request = "";
            pData.RequestType = "";
            pData.SizeRequest = 0;// System.Text.ASCIIEncoding.ASCII.GetByteCount(sJsonRequest);
            pData.ResponseType = "";
            pData.ReferenceId = obj;
#if LOCAL_REQUEST
            pData.Response = obj;
#else
            pData.Response = obj.Replace("{", "{\n").Replace("}", "\n}").Replace(",", ",\n");
#endif
            pData.ResponseType = "";
            pData.SizeResponse = obj != null ? obj.Length : 0;
#if UNITY_EDITOR
            // only collect stack on editor due to performance
            pData.Stack = UnityEngine.StackTraceUtility.ExtractStackTrace();
#endif

            // add it
            m_pMonitor.AddData(pData);
        }


    }
}