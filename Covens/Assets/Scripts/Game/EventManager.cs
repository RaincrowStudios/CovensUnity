using UnityEngine;
using System.Collections;

public class EventManager : MonoBehaviour
{
	public static EventManager Instance { get; set;}

	public delegate void SmoothZoom();
	public static event SmoothZoom OnSmoothZoom;

	public delegate void PlayerDataReceived();
	public static event PlayerDataReceived OnPlayerDataReceived;

	public delegate void NPCDataReceived();
	public static event NPCDataReceived OnNPCDataReceived;

	public delegate void InventoryDataReceived();
	public static event InventoryDataReceived OnInventoryDataReceived;

	public delegate void FreezeScale(bool canScale);
	public static event  FreezeScale OnFreezeScale;

	public delegate void MapViewSet();
	public static event MapViewSet OnMapViewSet;

	public delegate void CastingState(SpellCastStates state);
	public static event  CastingState CastingStateChange;

	void Awake ()
	{
		Instance = this;
	}

	public void CallMapViewSet( )
	{
		print ("Game back in map view");
		if(OnMapViewSet!=null)
			OnMapViewSet ( );
	}

	public void CallPlayerDataReceivedEvent( )
	{
		if(OnPlayerDataReceived!=null)
			OnPlayerDataReceived ( );
	}


	public void CallNPCDataReceivedEvent( )
	{
		if(OnNPCDataReceived!=null)
			OnNPCDataReceived ( );
	}

	public void CallInventoryDataReceived()
	{
		if(OnInventoryDataReceived!=null)
			OnInventoryDataReceived ( );
	}

	public void CallFreezeScale(bool scale)
	{
		if(OnFreezeScale!=null)
			OnFreezeScale (scale );
	}

	public void CallCastingStateChange(SpellCastStates state)
	{
		if(CastingStateChange!=null)
			CastingStateChange (state);
	}

	public void CallSmoothZoom()
	{
		if(OnSmoothZoom!=null)
			OnSmoothZoom ();
	}

}

