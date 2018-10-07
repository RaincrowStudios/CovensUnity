using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpChatData : MonoBehaviour {

	public Text content;

	public Text timestamp;

	public void Setup(ChatData data){ 
		content.text = data.Content;
		timestamp.text = Utilities.EpocToDateTimeChat (data.TimeStamp);
	}
}
