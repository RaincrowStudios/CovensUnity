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
        if (summon == null)
        {
            summon = Instantiate(SummonObject).GetComponent<SummoningManager>();
            summon.GetComponent<Canvas>().worldCamera = summonCamera;
        }
    }
    public void Close()
    {
        if (summon != null)
            summon.Close();

        summon = null;
    }
}