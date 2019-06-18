using UnityEngine;

public class SummoningController : MonoBehaviour
{
    public static SummoningController Instance { get; set; }

    public GameObject SummonObject;
    public Camera summonCamera;
    public static SummoningManager summon = null;
    void Awake()
    {
        Instance = this;
    }

    public void Open()
    {
        if (!FirstTapVideoManager.Instance.CheckSummon()) { summon = null; return; };
        Debug.Log(summon);
        if (summon == null)
        {
            Debug.Log("opening");
            summon = Instantiate(SummonObject).GetComponent<SummoningManager>();
            summon.GetComponent<Canvas>().worldCamera = summonCamera;
        }
        else
        {
            summon.InitSummon();
        }

    }
    public void Close()
    {

        if (summon != null)
            summon.Close();

    }
}