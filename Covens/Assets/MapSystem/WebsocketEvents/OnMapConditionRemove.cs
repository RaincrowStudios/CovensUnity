using UnityEngine;
using System.Collections;

public static class OnMapConditionRemove
{
    public static event System.Action<Conditions> OnConditionRemoved;
    public static event System.Action<Conditions> OnPlayerConditionRemoved;

    public static void HandleEvent(WSData data)
    {
        Debug.Log(Newtonsoft.Json.JsonConvert.SerializeObject(data));

        if (data.condition.bearer == PlayerDataManager.playerData.instance)
        {
            ConditionsManager.WSRemoveCondition(data.condition.instance);
            OnPlayerConditionRemoved?.Invoke(data.condition);
        }
        else
        {
            OnConditionRemoved?.Invoke(data.condition);
        }
    }
}
