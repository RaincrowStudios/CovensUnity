using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class QuestsController : MonoBehaviour
{
    public static QuestsController instance { get; set; }
    public GameObject Notification;
    public Transform NotificationTransform;
    // public Text notiTitle;
    // public Text notiProgress;
    public static Dailies currentQuests;

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
        g.transform.GetChild(3).GetComponent<Text>().text = DownloadedAssets.questsDict[id].title;
        g.transform.GetChild(4).GetComponent<Text>().text = DownloadedAssets.questsDict[id].description;
    }

    public void OnProgress(string quest, int count, int silver)
    {
        StartCoroutine(OnProgressHelper(quest, count, silver));
    }

    IEnumerator OnProgressHelper(string quest, int count, int silver)
    {
        //		print (quest);
        //		print (count);
        //		print (silver);
        yield return new WaitForSeconds(3.5f);
        var pQuest = PlayerDataManager.playerData.dailies;
        var g = Utilities.InstantiateObject(Notification, NotificationTransform);
        Text notiTitle = g.transform.GetChild(0).GetChild(4).GetComponent<Text>();
        Text notiProgress = g.transform.GetChild(0).GetChild(5).GetComponent<Text>();

        if (silver == 0)
        {
            if (quest == "gather")
            {
                notiTitle.text = "Quest Progress : Gather";
                notiProgress.text = "Completed : " + count.ToString() + "/" + currentQuests.gather.amount.ToString();
                pQuest.gather.count = count;
            }
            else if (quest == "spellcraft")
            {
                notiTitle.text = "Quest Progress : Spellcraft";
                notiProgress.text = "Completed : " + count.ToString() + "/" + currentQuests.spellcraft.amount.ToString();
                pQuest.spellcraft.count = count;
            }
        }
        else
        {
            if (quest == "gather")
            {
                notiTitle.text = "Gather Quest Completed!";
                notiProgress.text = "+ " + silver.ToString() + " Silver";
                pQuest.gather.count = count;
                pQuest.explore.complete = true;

            }
            else if (quest == "spellcraft")
            {
                notiTitle.text = "Spellcraft Quest Completed!";
                notiProgress.text = "+ " + silver.ToString() + " Silver";
                pQuest.spellcraft.count = count;
                pQuest.spellcraft.complete = true;
            }
            else
            {
                notiTitle.text = "Explore Quest Completed!";
                notiProgress.text = "+ " + silver.ToString() + " Silver";
                pQuest.explore.count = 1;
                pQuest.explore.complete = true;
            }
        }

        yield return new WaitForSeconds(5f);
        Destroy(g);

    }

}