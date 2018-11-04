using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
public class AddTestMarker : MonoBehaviour
{

	public MarkerSpawner.MarkerType TypeToSpawn;

	public float latitude;
	public float longitude;

	public bool isPhysical = true;
	bool isDeath = false;
	public void CastPortal()
	{
	}

	public void DeathToggle()
	{
		isDeath = !isDeath;
		if (isDeath)
			DeathState.Instance.ShowDeath ();
//		else
//			DeathState.Instance.HideDeath ();


	}
	
	public void ClearAllPrefs()
	{
		PlayerPrefs.DeleteAll ();
	}

	public void AddSpiritItem()
	{
		PlayerManagerUI.Instance.SwitchMapStyle ();
	}

	void Update()
	{
	}
}

