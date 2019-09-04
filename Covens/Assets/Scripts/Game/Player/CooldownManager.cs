using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CooldownManager
{
    public struct Cooldown
    {
        public string id;
        public float total;

        public System.DateTime startDate;
        public System.DateTime endDate;

        public float Remaining => (float)(endDate - System.DateTime.UtcNow).TotalSeconds;

        public Cooldown(string id, double end, float total)
        {
            this.id = id;
            this.total = total;
            this.endDate = Utilities.FromJavaTime(end);
            this.startDate = endDate.AddSeconds(-total);
        }

        public Cooldown(string id, double start, double end)
        {
            this.id = id;
            this.startDate = Utilities.FromJavaTime(start);
            this.endDate = Utilities.FromJavaTime(end);
            this.total = (float)(endDate - startDate).TotalSeconds;

            float _remainig = Remaining;
            if (_remainig > total)
                endDate = endDate.AddSeconds(total - _remainig);
        }
    }

    private static Dictionary<string, Cooldown> m_CooldownDictionary = new Dictionary<string, Cooldown>();

    public static void AddCooldown(string id, double endDate, float total)
    {
        id = id.ToLower();
        Cooldown cd = new Cooldown(id, endDate, total);

        if (m_CooldownDictionary.ContainsKey(id))
            Debug.Log("[<color=magenta>Cooldown</color>] Update " + cd.id + "[" + cd.Remaining + "/" + cd.total + "]");
        else
            Debug.Log("[<color=magenta>Cooldown</color>] Add " + cd.id + "[" + cd.Remaining + "/" + cd.total + "]");

        m_CooldownDictionary[id] = cd;
    }

    public static void AddCooldown(string id, double start, double end)
    {
        id = id.ToLower();
        Cooldown cd = new Cooldown(id, start, end);

        if (m_CooldownDictionary.ContainsKey(id))
            Debug.Log("[<color=magenta>Cooldown</color>] Update " + cd.id + "(" + cd.Remaining + "/" + cd.total + ")");
        else
            Debug.Log("[<color=magenta>Cooldown</color>] Add " + cd.id + "(" + cd.Remaining + "/" + cd.total + ")");

        m_CooldownDictionary[id] = cd;
    }
    
    public static Cooldown? GetCooldown(string id)
    {
        id = id.ToLower();


        if (m_CooldownDictionary.ContainsKey(id))
        {
            Cooldown cd = m_CooldownDictionary[id];

            if (cd.Remaining <= 0)
            {
                m_CooldownDictionary.Remove(id);
                return null;
            }

            return cd;
        }
        else
        {
            return null;
        }
    }

    public static void RemoveCooldown(string id)
    {
        m_CooldownDictionary.Remove(id);
    }
}
