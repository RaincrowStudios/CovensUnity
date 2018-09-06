using UnityEngine;
using System.Collections;

public class LocationTokenData : MonoBehaviour
{
	public Token token;

	public void OnClick()
	{
		print ("Clicked");
		token = MarkerManagerAPI.AddEnumValueSingle (token);
		MarkerSpawner.instanceID = token.instance;
		MarkerSpawner.Instance.OnTokenSelect (token, true);
	}
}

