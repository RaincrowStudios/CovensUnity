using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class QuestsController : MonoBehaviour
{
    public static QuestsController instance { get; set; }
    public GameObject Notification;
    public Transform NotificationTransform;
    [SerializeField] private Sprite m_NotificationIcon;
    // public Text notiTitle;
    // public Text notiProgress;

    public GameObject ExploreQuestObject;

    void Awake()
    {
        instance = this;
    }

    public void ExploreQuestDone(string id)
    {
        SoundManagerOneShot.Instance.MenuSound();
        SoundManagerOneShot.Instance.PlayReward();
        ExploreQuestObject.SetActive(true);
        var g = Utilities.InstantiateUI(ExploreQuestObject, NotificationTransform);
        g.GetComponentInChildren<Button>().onClick.AddListener(() => { Destroy(g); });
        g.transform.GetChild(2).GetComponent<Text>().text = DownloadedAssets.questsDict[id].title;
        g.transform.GetChild(3).GetComponent<Text>().text = DownloadedAssets.questsDict[id].description;
    }

    public void OnProgress(string quest, int count, int silver)
    {
        StartCoroutine(OnProgressHelper(quest, count, silver));
    }

    IEnumerator OnProgressHelper(string quest, int count, int silver)
    {
        //		Debug.Log (quest);
        //		Debug.Log (count);
        //		Debug.Log (silver);
        yield return new WaitForSeconds(3.5f);
        var pQuest = PlayerDataManager.playerData.dailies;
        //var g = Utilities.InstantiateObject(Notification, NotificationTransform);
        //g.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -1209);
        //Text notiTitle = g.transform.GetChild(0).GetChild(4).GetComponent<Text>();
        //Text notiProgress = g.transform.GetChild(0).GetChild(5).GetComponent<Text>();

        string message = null;

        if (silver == 0)
        {
            if (quest == "gather")
            {
				//message = "Quest Progress : Gather\n" + "Completed : " + count.ToString() + "/" + pQuest.gather.amount.ToString();
				message = LocalizeLookUp.GetText("daily_quest_progress") + " " + LocalizeLookUp.GetText("daily_gather") + "\n" + LocalizeLookUp.GetText("daily_completed") + " " + /*"Quest Progress : Gather\n" + "Completed : " + */count.ToString() + "/" + pQuest.gather.amount.ToString();
                //notiTitle.text = "Quest Progress : Gather";
                //notiProgress.text = "Completed : " + count.ToString() + "/" + pQuest.gather.amount.ToString();
                pQuest.gather.count = count;
            }
            else if (quest == "spellcraft")
            {
                //notiTitle.text = "Quest Progress : Spellcraft";
                //notiProgress.text = "Completed : " + count.ToString() + "/" + pQuest.spellcraft.amount.ToString();
				message = LocalizeLookUp.GetText("daily_quest_progress") + " " + LocalizeLookUp.GetText("daily_spellcraft") + "\n" + LocalizeLookUp.GetText("daily_completed") + " " + /*"Quest Progress : Gather\n" + "Completed : " + */count.ToString() + "/" + pQuest.gather.amount.ToString();
               // message = "Quest Progress : Spellcraft\n" + "Completed : " + count.ToString() + "/" + pQuest.spellcraft.amount.ToString();
                pQuest.spellcraft.count = count;
            }
        }
        else
        {
            if (quest == "gather")
            {
				message = LocalizeLookUp.GetText ("daily_completed_gather") + "\n" + "+ " + silver.ToString () +  " " + LocalizeLookUp.GetText ("store_silver");
               // message = "Gather Quest Completed!\n" + "+ " + silver.ToString() + " Silver";
                //notiTitle.text = "Gather Quest Completed!";
                //notiProgress.text = "+ " + silver.ToString() + " Silver";
                pQuest.gather.count = count;
                pQuest.explore.complete = true;

            }
            else if (quest == "spellcraft")
            {
				message = LocalizeLookUp.GetText ("daily_completed_spellcraft") + "\n" + "+ " + silver.ToString () +  " " + LocalizeLookUp.GetText ("store_silver");
              //  message = "Spellcraft Quest Completed!\n" + "+ " + silver.ToString() + " Silver";
                //notiTitle.text = "Spellcraft Quest Completed!";
                //notiProgress.text = "+ " + silver.ToString() + " Silver";
                pQuest.spellcraft.count = count;
                pQuest.spellcraft.complete = true;
            }
            else
            {
				message = LocalizeLookUp.GetText ("daily_completed_explore") + "\n" + "+ " + silver.ToString () +  " " + LocalizeLookUp.GetText ("store_silver");
              //  message = "Explore Quest Completed!\n" + "+ " + silver.ToString() + " Silver";
                //notiTitle.text = "Explore Quest Completed!";
                //notiProgress.text = "+ " + silver.ToString() + " Silver";
                pQuest.explore.count = 1;
                pQuest.explore.complete = true;
            }
        }

        if (message != null)
        {
            PlayerNotificationManager.Instance.ShowNotification(message, m_NotificationIcon);
        }
        else
        {
            Debug.LogError("null quest progress text");
        }

        //yield return new WaitForSeconds(5f);
        //Destroy(g);

    }

    public static void GetQuests(System.Action<int, string> callback)
    {
        Debug.Log("Getting Quests...");

        APIManager.Instance.GetData("daily/get",
            (string result, int response) =>
            {
                Debug.Log(response);
                if (response == 200)
                    PlayerDataManager.playerData.dailies = Newtonsoft.Json.JsonConvert.DeserializeObject<Dailies>(result);
                else
                    Debug.Log(result + response);

                callback?.Invoke(response, result);
            });
    }
}