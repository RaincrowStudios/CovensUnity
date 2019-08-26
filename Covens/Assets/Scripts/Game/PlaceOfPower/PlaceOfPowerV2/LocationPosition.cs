using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationPosition : MonoBehaviour
{
    public int position { get; private set; }
    public int island { get; private set; }

    public void Setup(int position, int island)
    {
        this.position = position;
        this.island = island;
    }

    public void OnClick()
    {
        SoundManagerOneShot.Instance.PlayButtonTap();
        Debug.Log("onclick");
        if (CheckEmpty())
        {
            var token = new WitchToken();
            token.position = position;
            token.island = island;

            if (LocationPlayerAction.CanSelectIsland(token))
            {
                LocationUnitSpawner.SetHighlight(token);
                LocationPlayerAction.ShowActions();
                LocationPlayerAction.SetSelectedPosition(this);
            }
            else
            {
                LocationPlayerAction.ShowMoveCloser();
            }
        }
        else
        {
            LocationPlayerAction.ShowActions();
        }
    }

    private bool CheckEmpty()
    {
        return transform.parent.childCount == 1;
    }

}
