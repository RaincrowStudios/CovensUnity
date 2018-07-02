using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChatItemData : MonoBehaviour
{
	public Text playerName;
	public Text degree;
	public Text content;
	public Text timeStamp;
	public Image profilePic;
	public Image alignment;
	ChatData CD;
	public void Setup(ChatData data, bool isLocation)
	{
		CD = data;

		timeStamp.text = Utilities.EpocToDateTimeChat (data.TimeStamp);
		profilePic.sprite = ChatUI.Instance.profilePics [data.Avatar];
		playerName.text = data.Name + "(level" + CD.Level.ToString() + ")";
		degree.text = Utilities.witchTypeControl (CD.Degree, false);

		if (data.Degree > 0)
			alignment.color = Utilities.Orange;
		else if (data.Degree < 0)
			alignment.color = Utilities.Purple;
		else
			alignment.color = Utilities.Grey;
		if (!isLocation) {
			content.text = data.Content;
		} else {
			// add location logic
		}
//
//		if (ChatUI.selectedPvPPlayer == null) {
//			if (ChatUI.Instance.ActiveWindow == ChatUI.ChatWindows.Whispers && data.Name == ChatUI.playerName) {
//					playerName.text = data.Receiver + " (Level " + data.Level.ToString () + ")";
//			} else{
//				playerName.text = data.Name + " (Level " + data.Level.ToString () + ")";
//			}
//			degree.text = Utilities.witchTypeControl (data.Degree);
//		
//		
//		}else{
//			Destroy (degree.gameObject);
//			playerName.text = data.Name;
//			whisperButton.enabled = false;
//		}
	}

	void kill()
	{
		Destroy (gameObject);
	}

	public void MoveToPos()
	{
		PlayerManager.Instance.Fly ();
		OnlineMaps.instance.SetPosition (CD.Longitude, CD.Latitude);
		PlayerManager.inSpiritForm = false;
		PlayerManager.Instance.Fly ();
		ChatUI.Instance.HideChat ();
	}
}

