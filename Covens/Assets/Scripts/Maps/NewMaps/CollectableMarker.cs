using UnityEngine;
using System.Collections;
using Raincrow.Maps;

public class CollectableMarker : MuskMarker
{
    [SerializeField] private Transform m_ViewTransform;

    public override Transform characterTransform => m_ViewTransform;
    public CollectableToken collectableToken => m_Data as CollectableToken;

    public override void Setup(Token data)
    {
        base.Setup(data);
        //todo: setup icon )currently in markerspawner
    }
}
