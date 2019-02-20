using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LocationSelectionCard : MonoBehaviour
{
	[Header("Location")]
	//public GameObject LocationCard;
	public TextMeshProUGUI locationTitle;
	public TextMeshProUGUI locOwnedBy;
	public TextMeshProUGUI defendedBy;
	public TextMeshProUGUI timeToTreasure;
	public TextMeshProUGUI ExitLocation;
	//public TextMeshProUGUI selfEnergy;
	public Button EnterLocation;

	public void Setup()
	{
		locationTitle.text = MarkerSpawner.SelectedMarker.displayName;

		EnterLocation.onClick.AddListener (() => ShowSelectionCard.Instance.Attack ());

		print("Setting Stuff");
		var mData = MarkerSpawner.SelectedMarker;
		print(mData.controlledBy);
		if (mData.controlledBy != "")
		{
			if (mData.isCoven)
				locOwnedBy.text = "This Place of Power is owned by the coven <color=#ffffff> " + mData.controlledBy + "</color>.";
			else
				locOwnedBy.text = "This Place of Power is owned by <color=#ffffff> " + mData.controlledBy + "</color>.";
			if (mData.spiritCount == 1)
				defendedBy.text = "It is defended by " + mData.spiritCount.ToString() + " spirit.";
			else
				defendedBy.text = "It is defended by " + mData.spiritCount.ToString() + " spirits.";
		}
		else
		{
			locOwnedBy.text = "This Place of Power is unclaimed.";
			defendedBy.text = "You can own this Place of Power by summoning a spirit inside it.";
		}
		timeToTreasure.text = GetTime(mData.rewardOn) + "until this Place of Power yields treasure.";
		if (mData.full)
		{
			EnterLocation.GetComponent<Text>().text = "Place of power is full.";
			ExitLocation.text = "Close";
		}
		else
		{
			ExitLocation.text = "Not Today";
			EnterLocation.GetComponent<Text>().text = "Enter the Place of Power";
		}
	}

//	public void SetupLocationCard()
//	{
//		print("Setting Stuff");
//		var mData = MarkerSpawner.SelectedMarker;
//		print(mData.controlledBy);
//		if (mData.controlledBy != "")
//		{
//			if (mData.isCoven)
//				locOwnedBy.text = "This Place of Power is owned by the coven <color=#ffffff> " + mData.controlledBy + "</color>.";
//			else
//				locOwnedBy.text = "This Place of Power is owned by <color=#ffffff> " + mData.controlledBy + "</color>.";
//			if (mData.spiritCount == 1)
//				defendedBy.text = "It is defended by " + mData.spiritCount.ToString() + " spirit.";
//			else
//				defendedBy.text = "It is defended by " + mData.spiritCount.ToString() + " spirits.";
//		}
//		else
//		{
//			locOwnedBy.text = "This Place of Power is unclaimed.";
//			defendedBy.text = "You can own this Place of Power by summoning a spirit inside it.";
//		}
//		timeToTreasure.text = GetTime(mData.rewardOn) + "until this Place of Power yields treasure.";
//		if (mData.full)
//		{
//			EnterLocation.GetComponent<Text>().text = "Place of power is full.";
//			ExitLocation.text = "Close";
//		}
//		else
//		{
//			ExitLocation.text = "Not Today";
//			EnterLocation.GetComponent<Text>().text = "Enter the Place of Power";
//		}
//	}


	string GetTime(double javaTimeStamp)
	{
		if (javaTimeStamp < 159348924)
		{
			string s = "unknown";
			return s;
		}

		System.DateTime dtDateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
		dtDateTime = dtDateTime.AddMilliseconds(javaTimeStamp).ToUniversalTime();
		var timeSpan = dtDateTime.Subtract(System.DateTime.UtcNow);
		string stamp = "";

		if (timeSpan.Days > 1)
		{
			stamp = timeSpan.Days.ToString() + " days, ";

		}
		else if (timeSpan.Days == 1)
		{
			stamp = timeSpan.Days.ToString() + " day, ";
		}
		if (timeSpan.Hours > 1)
		{
			stamp += timeSpan.Hours.ToString() + " hours ";
		}
		else if (timeSpan.Hours == 1)
		{
			stamp += timeSpan.Hours.ToString() + " hour ";
		}
		else
		{
			if (timeSpan.Minutes > 1)
			{
				stamp += timeSpan.Minutes.ToString() + " minutes ";
			}
			else if (stamp.Length < 4)
			{
				stamp.Remove(4);
			}
		}
		return stamp;
	}

	//Not sure what the logic is or where it is for entering a place of power
//	public EnterLocation()
//	{
//
//	}

	public void DestroySelf()
	{
		Destroy (ShowSelectionCard.currCard);
	}
}