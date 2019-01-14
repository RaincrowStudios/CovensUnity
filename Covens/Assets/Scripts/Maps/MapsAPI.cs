using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Raincrow.Maps;

public class MapsAPI
{
    private static IMaps m_Instance;

    public static IMaps Instance
    {
        get
        {
            if (m_Instance == null)
            {
#if INFITYCODE_ONLINEMAPS
                m_Instance = new OMOnlineMaps();
#endif
            }
            return m_Instance;
        }
    }


}
