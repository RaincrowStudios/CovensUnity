using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CooldownManager
{
    public struct Cooldown
    {
        public string id;
        public float total;
        private double timestamp;
        public System.DateTime endDate;

        public float Remaining => (float)Utilities.TimespanFromJavaTime(timestamp).TotalSeconds;

        public Cooldown(string id, float total, double timestamp)
        {
            this.id = id;
            this.total = total;
            this.timestamp = timestamp;
            this.endDate = Utilities.FromJavaTime(timestamp);
        }
    }

    private static Dictionary<string, Cooldown> m_CooldownDictionary = new Dictionary<string, Cooldown>();

    public static void AddCooldown(string id, float total, double endDate)
    {
        id = id.ToLower();

        Cooldown cd = new Cooldown(id, total, endDate);
        m_CooldownDictionary[id] = cd;
        Debug.Log("<color=magenta>add cooldown\n" + cd.id + "(" + cd.Remaining + "/" + cd.total + "</color>");
    }
    
    public static Cooldown? GetCooldown(string id)
    {
        id = id.ToLower();


        if (m_CooldownDictionary.ContainsKey(id))
        {
            Cooldown cd = m_CooldownDictionary[id];

            if (cd.Remaining <= 0)
            {
                Debug.Log("<color=magenta>expired cooldown: " + id + "</color>");
                m_CooldownDictionary.Remove(id);
                return null;
            }

            Debug.Log("<color=magenta>cooldown: " + id + ": " + cd.Remaining + "</color>");
            return cd;
        }
        else
        {
            Debug.Log("<color=magenta>null cooldown: " + id + "</color>");
            return null;
        }
    }
}
