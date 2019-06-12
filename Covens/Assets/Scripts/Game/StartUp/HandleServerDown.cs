using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class HandleServerDown : MonoBehaviour
{
    public static HandleServerDown Instance { get; set; }
    public GameObject serverDownContainer;
    public TextMeshProUGUI text;
    public TextMeshProUGUI subText;
    public Slider progress;
    public GameObject loading;
    public string pingURL = "www.google.com";
    List<string> MainServerErrors = new List<string>();
    List<string> BackupServerErrors = new List<string>();
    float timeElapsed = 0;
    bool runTimer = false;

    public void AssetDownloadError(string id)
    {
        serverDownContainer.SetActive(true);
        text.text = $"Error in loading {id} asset. Try again later.";
    }

    void Awake()
    {
        Instance = this;
    }
    void Update()
    {
        if (runTimer)
            timeElapsed += Time.deltaTime;
    }
    public void ShowMaintenance()
    {
        serverDownContainer.SetActive(true);
        text.text = "Servers will be down briefly for maintenance.";
    }


    public void ShowErrorParseDictionary()
    {
        serverDownContainer.SetActive(true);

        text.text = "Error in parsing language dictionary, please email help@raincrowgames.com and we will get you sorted out!";
        loading.SetActive(true);
    }

    public void ShowErrorDictionary()
    {
        serverDownContainer.SetActive(true);

        text.text = "Error in downloading language dictionary, please email help@raincrowgames.com and we will get you sorted out!";
        loading.SetActive(true);
    }

    public void ShowServerDown(bool isRelease)
    {
        serverDownContainer.SetActive(true);
        text.text = "Demons have infiltrated the servers. Give us a little time to sort this out. (Servers are down)";
        if (isRelease)
            StartCoroutine(ServerDownHelper());
    }

    IEnumerator ServerDownHelper()
    {
        //yield return new WaitForSeconds(2);
        //text.text = "Retrying connection to servers . . .";
        loading.SetActive(true);
        text.gameObject.SetActive(true);
        subText.gameObject.SetActive(true);
        //for (int i = 0; i < 5; i++)
        //{
        //    subText.text = $"Attempt {i.ToString()}/5 ";
        //    var www = MakeRequest();
        //    yield return www.SendWebRequest();
        //    if (www.isNetworkError || www.responseCode != 200)
        //    {
        //        MainServerErrors.Add(www.error + www.responseCode.ToString());
        //        yield return new WaitForSeconds(1.5f);
        //        continue;
        //    }
        //    else
        //    {
        //        loading.SetActive(false);
        //        serverDownContainer.SetActive(false);
        //        DownloadAssetBundle.Instance.HandleAssetResult(www.downloadHandler.text);
        //        yield break;
        //    }
        //}
        //CovenConstants.isBackUpServer = true;

        //text.text = "Connecting to backup servers . . .";

        //for (int i = 0; i < 5; i++)
        //{
        //    subText.text = $"Attempt {i.ToString()}/5 ";
        //    var www = MakeRequest();
        //    yield return www.SendWebRequest();
        //    if (www.isNetworkError || www.responseCode != 200)
        //    {
        //        BackupServerErrors.Add(www.error + www.responseCode.ToString());
        //        yield return new WaitForSeconds(1.5f);
        //        continue;
        //    }
        //    else
        //    {
        //        loading.SetActive(false);
        //        serverDownContainer.SetActive(false);
        //        DownloadAssetBundle.Instance.HandleAssetResult(www.downloadHandler.text);
        //        yield break;
        //    }
        //}
        subText.gameObject.SetActive(false);
        // text.text = "Verifying internet connection . . .";
        // Ping p = new Ping(pingURL);
        // while (p.isDone == false)
        // {
        //     yield return 0;
        // }
        float internetSpeed = 0;
        text.text = "Running internet speed tests . . .";

        //download 4mb file
        runTimer = true;
        progress.gameObject.SetActive(true);
        loading.SetActive(false);
        UnityWebRequest webRequest = UnityWebRequest.Head("https://storage.googleapis.com/raincrow-covens/SpeedTest.rar");
        yield return webRequest.SendWebRequest();
        progress.value = webRequest.downloadProgress;
        progress.gameObject.SetActive(false);
        subText.gameObject.SetActive(true);
        internetSpeed = (3.92f / timeElapsed);
        subText.text = (internetSpeed).ToString("F2") + " Mbps";

        serverDownContainer.SetActive(true);
        text.text = "Demons have infiltrated the servers. Give us a little time to sort this out. (Servers are down)";

        runTimer = false;
        //// try to connect once more
        //var request = MakeRequest();
        //yield return request.SendWebRequest();
        //if (!request.isNetworkError && request.responseCode == 200)
        //{
        //    serverDownContainer.SetActive(false);
        //    DownloadAssetBundle.Instance.HandleAssetResult(request.downloadHandler.text);
        //}
        //else
        //{
            SendEmail(subText.text);
        //}
    }

    //UnityWebRequest MakeRequest()
    //{
    //    var data = new { game = "covens" };
    //    Debug.Log(CovenConstants.hostAddress + "raincrow/assets");
    //    UnityWebRequest www = UnityWebRequest.Put(CovenConstants.hostAddress + "raincrow/assets", JsonConvert.SerializeObject(data));
    //    www.method = "POST";
    //    www.SetRequestHeader("Content-Type", "application/json");
    //    return www;
    //}

    void SendEmail(string speed)
    {

        // yield return new WaitForEndOfFrame();
        // var filepath = Application.persistentDataPath + "/debugscreenshot.png";
        // ScreenCapture.CaptureScreenshot("debugscreenshot.png");
        // Debug.Log(filepath);
        string email = "help@raincrowgames.com";
        string subject = MyEscapeURL("Covens Login Bug # " + (LoginAPIManager.StoredUserName == "" ? "New User" : LoginAPIManager.StoredUserName));
        Debug.Log(subject);
        string body = MyEscapeURL($" Version: {Application.version} \n Platform: {Application.platform}\n Location: {GetGPS.latitude},{GetGPS.longitude} \n speed: {speed} \n ErrorLogs: \n MainServer: \n{JsonConvert.SerializeObject(MainServerErrors)} \n\n  BackupServer: \n{JsonConvert.SerializeObject(BackupServerErrors)}");
        Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);

    }

    string GetHardwareInfo()
    {
        return "Graphics Device Name: " + SystemInfo.graphicsDeviceName + "\n"
                + "Graphics Device Type: " + SystemInfo.graphicsDeviceType.ToString() + "\n"
                + "Graphics Device Version: " + SystemInfo.graphicsDeviceVersion + "\n"
                + "Graphics Memory Size: " + (SystemInfo.graphicsMemorySize) + "\n"
                + "Operating System: " + SystemInfo.operatingSystem + "\n"
                + "Processor Type: " + SystemInfo.processorType + "\n"
                + "Processor Count: " + SystemInfo.processorCount.ToString() + "\n"
                + "Processor Frequency: " + SystemInfo.processorFrequency.ToString() + "\n"
                + "System Memory Size: " + (SystemInfo.systemMemorySize) + "\n"
                + "Screen Size: " + Screen.width.ToString() + "x" + Screen.height.ToString();
    }

    string MyEscapeURL(string url)
    {
        return WWW.EscapeURL(url).Replace("+", "%20");
    }
}