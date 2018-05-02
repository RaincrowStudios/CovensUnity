using UnityEngine;
using System.Collections;

public class EventManager : MonoBehaviour
{
	public static EventManager Instance { get; set;}

	#region Arena

	public delegate void ArenaPlayerHit(GameObject g);
	public static event ArenaPlayerHit OnArenaPlayerHit;

	public delegate void ArenaDamageInfoTap();
	public static event ArenaDamageInfoTap OnArenaDamageInfoTap;

	public delegate void SmoothZoom();
	public static event SmoothZoom OnSmoothZoom;

	#endregion

	public delegate void PlayerDataReceived();
	public static event PlayerDataReceived OnPlayerDataReceived;

	public delegate void NPCDataReceived();
	public static event NPCDataReceived OnNPCDataReceived;

	public delegate void InventoryDataReceived();
	public static event InventoryDataReceived OnInventoryDataReceived;

	public delegate void FreezeScale(bool canScale);
	public static event  FreezeScale OnFreezeScale;

	void Awake ()
	{
		Instance = this;
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
		if(OnInventoryDataReceived!=null)
			OnFreezeScale (scale );
	}

	#region Arena


	public void CallPlayerHitEvent(GameObject g)
	{
		if(OnArenaPlayerHit!=null)
			OnArenaPlayerHit (g);
	}

	public void CallSmoothZoom()
	{
		if(OnSmoothZoom!=null)
			OnSmoothZoom ();
	}


	public void CallArenaDamageInfoTapEvent()
	{
		if(OnArenaDamageInfoTap!=null)
			OnArenaDamageInfoTap ();
	}

	#endregion

}

