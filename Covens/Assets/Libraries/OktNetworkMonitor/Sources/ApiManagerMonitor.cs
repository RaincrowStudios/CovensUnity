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
        }

        public void Destroy()
        {
            m_pMonitor = null;
            APIManager.OnRequestEvt -= APIManager_OnRequestEvt;
            APIManager.OnResponseEvt -= APIManager_OnResponseEvt;
        }


        private void APIManager_OnRequestEvt(UnityEngine.Networking.UnityWebRequest obj, string sRequest)
        {
            // bake them
            OktNetworkMonitor.RecordData pData = new OktNetworkMonitor.RecordData();

            pData.Table = "UnityWebRequest";
            pData.Request = obj.url + "\n" + sRequest;
            pData.RequestType = obj.method;
            pData.SizeRequest = 0;// System.Text.ASCIIEncoding.ASCII.GetByteCount(sJsonRequest);
            pData.ResponseType = "";
            pData.ReferenceId = obj;

            // add it
            m_pMonitor.AddData(pData);
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

            pData.Table = "UnityWebRequest";
            pData.Request = obj.url + "\n" + sRequest;
            pData.RequestType = obj.method;
            pData.SizeRequest = 0;// System.Text.ASCIIEncoding.ASCII.GetByteCount(sJsonRequest);
#if LOCAL_REQUEST
            pData.Response = sResponse;
#else
            pData.Response = sResponse.Replace("{", "{\n").Replace("}", "\n}").Replace(",", ",\n");
#endif
            pData.ResponseType = "";
            pData.SizeResponse = sResponse != null ? sResponse.Length : 0;
#if UNITY_EDITOR
            // only collect stack on editor due to performance
            pData.Stack = UnityEngine.StackTraceUtility.ExtractStackTrace();
#endif
            // add it
            if (!bLoaded)
            {
                m_pMonitor.AddData(pData);
            }
        }



        

    }

}