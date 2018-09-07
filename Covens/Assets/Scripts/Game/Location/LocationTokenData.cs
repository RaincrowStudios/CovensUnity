using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LocationTokenData : MonoBehaviour
{
	public Token token;
	public GameObject disabled;
	public Button summonButton;
	public void OnClick()
	{
		if (token.instance == PlayerDataManager.playerData.instance)
			return;
		token = MarkerManagerAPI.AddEnumValueSingle (token);
		MarkerSpawner.instanceID = token.instance;
		MarkerSpawner.Instance.OnTokenSelect (token, true);
	}

	public void DisableButton(bool canSummon)
	{
		if (!canSummon) {
			summonButton.enabled = false;
			disabled.SetActive (true);
		} else {
			disabled.SetActive (false);
			summonButton.enabled = true;
		}
	}

	public void OnSummon()
	{
		LocationUIManager.Instance.OnSummon ();
	}
}

