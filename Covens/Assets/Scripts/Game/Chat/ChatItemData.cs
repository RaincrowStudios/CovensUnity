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
	public Button whisperButton;
	ChatData CD;
	public void Setup(ChatData data, bool isLocation)
	{
		CD = data;
			if (data.Command == Commands.WhisperMessage || data.Command == Commands.WhisperLocation) {
				whisperButton.enabled = true;
				whisperButton.onClick.AddListener (() => ChatUI.Instance.ShowPvP (CD));
				whisperButton.onClick.AddListener (kill);
			} else {
				whisperButton.enabled = false;
			}

		timeStamp.text = Utilities.EpocToDateTimeChat (data.TimeStamp);
		profilePic.sprite = ChatUI.Instance.profilePics [data.Avatar];

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

		if (ChatUI.selectedPvPPlayer == null) {
			if (ChatUI.Instance.ActiveWindow == ChatUI.ChatWindows.Whispers && data.Name == ChatUI.playerName) {
					playerName.text = data.Receiver + " (Level " + data.Level.ToString () + ")";
			} else{
				playerName.text = data.Name + " (Level " + data.Level.ToString () + ")";
			}
			degree.text = Utilities.witchTypeControl (data.Degree);
		
		
		}else{
			Destroy (degree.gameObject);
			playerName.text = data.Name;
			whisperButton.enabled = false;
		}
	}

	void kill()
	{
		Destroy (gameObject);
	}
}

