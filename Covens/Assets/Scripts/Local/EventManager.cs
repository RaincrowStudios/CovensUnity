using UnityEngine;
using System.Collections;

public class EventManager : MonoBehaviour
{
	public static EventManager Instance { get; set;}

	public delegate void ArenaPlayerHit(GameObject g);
	public static event ArenaPlayerHit OnArenaPlayerHit;

	public delegate void ArenaDamageInfoTap();
	public static event ArenaDamageInfoTap OnArenaDamageInfoTap;

	public delegate void SmoothZoom();
	public static event SmoothZoom OnSmoothZoom;

	public delegate void PlayerDataReceived();
	public static event PlayerDataReceived OnPlayerDataReceived;

	void Awake ()
	{
		Instance = this;
	}

	public void CallPlayerDataReceivedEvent( )
	{
		if(OnPlayerDataReceived!=null)
			OnPlayerDataReceived ( );
	}


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
}

