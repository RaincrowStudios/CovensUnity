using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Raincrow.Maps;

public class PortalMarker : MuskMarker {

    [SerializeField] private Transform m_ViewTransform;

    public override Transform AvatarTransform => m_ViewTransform;

    public override void Setup(Token data)
    {
        base.Setup(data);
    }
}
