using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CooldownManager
{
    public struct Cooldown
    {
        public string id;
        public double timestamp;
        public float duration;

        public Cooldown(string id, double timestamp)
        {
            this.id = id;
            this.timestamp = timestamp;
            this.duration = (float)Utilities.TimespanFromJavaTime(timestamp).TotalSeconds;
            Debug.Log("new cooldown: " + id + " - " + duration + "s");
        } 
    }

    private static Dictionary<string, Cooldown?> m_CooldownDictionary = new Dictionary<string, Cooldown?>();
    public static event System.Action<string> OnCooldownEnd;

    public static void OnCooldownStart(WSData data)
    {
        m_CooldownDictionary[data.spell] = new Cooldown(data.spell, data.cooldownTime);
    }

    public static void OnCooldownFinish(WSData data)
    {
        if (m_CooldownDictionary.ContainsKey(data.spell))
            m_CooldownDictionary.Remove(data.spell);

        OnCooldownEnd?.Invoke(data.spell);
    }

    public static Cooldown? GetCooldown(string id)
    {
        if (m_CooldownDictionary.ContainsKey(id))
            return m_CooldownDictionary[id];
        else
            return null;
    }
}
