using System;
using System.Collections.Generic;
using UnityEngine;

public class LocationIslandBase : MonoBehaviour
{
    [SerializeField] protected Transform[] spots;
    protected Dictionary<int, string> tokens;

    protected void Add(KeyValuePair<int, string> token)
    {
        if (tokens.ContainsKey(token.Key))
        {
            throw new Exception("Position is already taken");
        }
        else
        {
            tokens.Add(token.Key, token.Value);
        }
    }

    protected void Remove(int position)
    {
        if (tokens.ContainsKey(position))
        {
            tokens.Remove(position);
        }
        else
        {
            throw new Exception("Position already empty");
        }
    }

    public List<Transform> GetMarkers()
    {
        List<Transform> markers = new List<Transform>();
        foreach (var item in tokens)
        {
            markers.Add(spots[item.Key].GetChild(2));
        }
        return markers;
    }

    protected Transform GetMarker(int position)
    {
        if (tokens.ContainsKey(position))
        {
            return spots[position].GetChild(1);
        }
        else
        {
            return null;
        }
    }
}

