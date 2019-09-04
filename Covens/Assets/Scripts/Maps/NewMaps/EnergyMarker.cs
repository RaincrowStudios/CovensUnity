using UnityEngine;
using System.Collections;
using Raincrow.Maps;
using TMPro;

public class EnergyMarker : CollectableMarker
{
    [Header("Energy Marker")]
    [SerializeField] private TextMeshPro m_EnergyAmount;
    //public EnergyToken EnergyTokenData { get; private set; }

    public override void Setup(Token data)
    {
        base.Setup(data);
        //EnergyTokenData = data as EnergyToken;

#if UNITY_EDITOR
        Debug.Log("<color=red>todo</color>: add new socket message for add.token.energy instead of using add.token.item");
#endif
        if (data is EnergyToken)
            m_EnergyAmount.text = (data as EnergyToken).amount.ToString();
        else if (data is CollectableToken)
            m_EnergyAmount.text = (data as CollectableToken).amount.ToString();
    }
}
