using UnityEngine;
using System.Collections;

public static class OnMapLocationLost
{
    public static void HandleEvent(WSData data)
    {
        //LocationUIManager.controlledBy = data.controlledBy;
        //LocationUIManager.Instance.LocationLost(data);
        //if (ShowSelectionCard.selectedType == MarkerSpawner.MarkerType.location && data.location == MarkerSpawner.instanceID)
        //{
        //    var mData = MarkerSpawner.SelectedMarker;
        //    mData.controlledBy = data.controlledBy;
        //    mData.spiritCount = data.spiritCount;
        //    mData.isCoven = data.isCoven;
        //    //ShowSelectionCard.Instance.SetupLocationCard();
        //}
        Debug.LogError("TODO: ONMAPLOCATIONLOST");
    }
}
