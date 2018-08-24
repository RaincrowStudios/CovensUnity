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
		SpellCastAPI.CastSummon ();
	}

	public void DeathToggle()
	{
		isDeath = !isDeath;
		if (isDeath)
			DeathState.Instance.ShowDeath ();
		else
			DeathState.Instance.HideDeath ();

	}
	
	public void ClearAllPrefs()
	{
		PlayerPrefs.DeleteAll ();
	}

	public void AddSpiritItem()
	{
		var t = new Token ();
		t.displayName = "Baba Yaga";
		t.Type = MarkerSpawner.MarkerType.spirit;
		if (isPhysical) {
			print (latitude);
			latitude = OnlineMapsLocationService.instance.position.y + Random.Range(-.003f,.003f);
			longitude = OnlineMapsLocationService.instance.position.x + Random.Range(-.003f,.003f);;
		}
		t.instance = "asd" + latitude.ToString ();
		t.latitude = latitude;
		t.longitude = longitude;
		t.degree = Random.Range (-1, 2);
		MarkerSpawner.Instance.AddMarker (t);
	}

	void Update()
	{
	}
}

