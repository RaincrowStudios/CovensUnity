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


        private void APIManager_OnRequestEvt(UnityEngine.Networking.UnityWebRequest obj)
        {
            //throw new System.NotImplementedException();
        }

        private void APIManager_OnResponseEvt(UnityEngine.Networking.UnityWebRequest obj, string sData)
        {
            //just tracking the response

            if (!Record)
                return;

            // bake them
            OktNetworkMonitor.RecordData pData = new OktNetworkMonitor.RecordData();

            pData.Table = "UnityWebRequest";
            pData.Request = obj.url + "\n" + sData;
            pData.RequestType = obj.method;
            pData.SizeRequest = 0;// System.Text.ASCIIEncoding.ASCII.GetByteCount(sJsonRequest);
            pData.Response = obj.downloadHandler.text.Replace("{","{\n").Replace("}", "\n}").Replace(",", ",\n");
            pData.ResponseType = "";
            pData.SizeResponse = obj.downloadHandler.data.Length;// System.Text.ASCIIEncoding.ASCII.GetByteCount("");
#if UNITY_EDITOR
            // only collect stack on editor due to performance
            pData.Stack = UnityEngine.StackTraceUtility.ExtractStackTrace();
#endif
            // add it
            m_pMonitor.AddData(pData);
        }



        

    }

}