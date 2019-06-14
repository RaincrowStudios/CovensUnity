using UnityEngine;
using TMPro;

public class HelpChatData : MonoBehaviour {

	public TextMeshProUGUI content;
	public TextMeshProUGUI timestamp;

	public void Setup(ChatData data){ 
		content.text = data.Content;
		timestamp.text = Utilities.EpochToDateTimeChat (data.TimeStamp);
	}
}
