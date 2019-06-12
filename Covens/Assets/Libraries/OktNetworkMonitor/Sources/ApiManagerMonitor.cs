using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Oktagon.Network
{
    public class ApiManagerMonitor : IMonitor
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
            APIManager.OnRequestEvt += APIManager_OnRequestEvt;
            APIManager.OnResponseEvt += APIManager_OnResponseEvt;
            WebSocketClient.OnResponseParsedEvt += WebSocketClient_OnResponseEvt;
            WorldMapMarkerManager.OnRequest += Wordmap_OnRequest;
            WorldMapMarkerManager.OnResponse += Wordmap_Onresponse;
        }

        public void Destroy()
        {
            m_pMonitor = null;
            APIManager.OnRequestEvt -= APIManager_OnRequestEvt;
            APIManager.OnResponseEvt -= APIManager_OnResponseEvt;
            WebSocketClient.OnResponseParsedEvt -= WebSocketClient_OnResponseEvt;
            WorldMapMarkerManager.OnRequest -= Wordmap_OnRequest;
            WorldMapMarkerManager.OnResponse -= Wordmap_Onresponse;
        }

        private void Wordmap_OnRequest(string request)
        {
            OktNetworkMonitor.RecordData pData = new OktNetworkMonitor.RecordData();

            pData.Table = "WebSocketClient";
            pData.Request = request;
            pData.RequestType = "";
            pData.Response = "";
            pData.SizeResponse = request != null ? request.Length : 0;

#if UNITY_EDITOR
            pData.Stack = UnityEngine.StackTraceUtility.ExtractStackTrace();
#endif
            m_pMonitor.AddDataResponse(pData);
        }

        private void Wordmap_Onresponse(string response)
        {
            OktNetworkMonitor.RecordData pData = new OktNetworkMonitor.RecordData();

            pData.Table = "WebSocketClient";
            pData.RequestType = "";
            pData.Response = response;
            pData.SizeResponse = response != null ? response.Length : 0;

#if UNITY_EDITOR
            pData.Stack = UnityEngine.StackTraceUtility.ExtractStackTrace();
#endif
            m_pMonitor.AddDataResponse(pData);
        }


        private void APIManager_OnRequestEvt(UnityEngine.Networking.UnityWebRequest obj, string sRequest)
        {
            // bake them
            OktNetworkMonitor.RecordData pData = new OktNetworkMonitor.RecordData();

            string sHead = obj.GetRequestHeader("Authorization");
            pData.Table = "UnityWebRequest";
            pData.Request = obj.url + "\n" + sRequest + "\nAuthorization:\n" + sHead;
            pData.RequestType = obj.method;
            pData.SizeRequest = 0;
            pData.ResponseType = "";
            pData.ReferenceId = obj;

#if UNITY_EDITOR
            // only collect stack on editor due to performance
            pData.Stack = UnityEngine.StackTraceUtility.ExtractStackTrace();
#endif

            // add it
            m_pMonitor.AddDataRequest(pData);
        }

        private void APIManager_OnResponseEvt(UnityEngine.Networking.UnityWebRequest obj, string sRequest, string sResponse)
        {
            //just tracking the response
            if (!Record)
                return;

            bool bLoaded = true;
            OktNetworkMonitor.RecordData pData = m_pMonitor.GetDataById(obj);
            // bake them
            if (pData == null)
            {
                pData = new OktNetworkMonitor.RecordData();
                bLoaded = false;
            }

            string sHead = obj.GetRequestHeader("Authorization");
            pData.Table = "UnityWebRequest";
            pData.Request = obj.url + "\n" + sRequest + "\nAuthorization:\n" + sHead;
            if (obj.isNetworkError)
                pData.Request = $"<color=red>{pData.Request}</color>";
            pData.RequestType = obj.method;
            pData.SizeRequest = 0;// System.Text.ASCIIEncoding.ASCII.GetByteCount(sJsonRequest);
#if SERVER_FAKE
            pData.Response = sResponse;
#else
            pData.Response = sResponse.Replace("{", "{\n").Replace("}", "\n}").Replace(",", ",\n");
#endif
            pData.ResponseType = "";
            pData.SizeResponse = sResponse != null ? sResponse.Length : 0;
            m_pMonitor.AddDataResponse(pData);
        }


        private void WebSocketClient_OnResponseEvt(WSData obj)
        {
            // bake them
            OktNetworkMonitor.RecordData pData = new OktNetworkMonitor.RecordData();

            pData.Table = "WebSocketClient";
            pData.RequestType = obj.command;
            pData.Response = obj.json;
            pData.SizeResponse = obj.json != null ? obj.json.Length : 0;

#if UNITY_EDITOR
            // only collect stack on editor due to performance
            pData.Stack = UnityEngine.StackTraceUtility.ExtractStackTrace();
#endif

            // add it
            m_pMonitor.AddDataResponse(pData);
        }
    }

}