using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HelpChatData : MonoBehaviour {

	public TextMeshProUGUI content;
	public TextMeshProUGUI timestamp;

	public void Setup(ChatData data){ 
		content.text = data.Content;
		timestamp.text = Utilities.EpochToDateTimeChat (data.TimeStamp);
	}
}
