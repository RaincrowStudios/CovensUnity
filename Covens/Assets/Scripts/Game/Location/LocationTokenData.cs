using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LocationTokenData : MonoBehaviour
{
	public Token token;

	public void OnClick()
	{
		if (token.instance == PlayerDataManager.playerData.instance)
			return;
		token = MarkerManagerAPI.AddEnumValueSingle (token);
		MarkerSpawner.instanceID = token.instance;
		MarkerSpawner.Instance.OnTokenSelect (token, true);
	}


}

