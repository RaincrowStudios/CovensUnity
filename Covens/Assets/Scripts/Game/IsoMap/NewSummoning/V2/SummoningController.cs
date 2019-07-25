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
        GameResyncHandler.OnResyncStart += Close;
    }

    public void Open()
    {
        if (!FirstTapVideoManager.Instance.CheckSummon()) { summon = null; return; };

        if (summon == null)
        {
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