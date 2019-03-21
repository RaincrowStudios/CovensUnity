using UnityEngine;
using System.Collections;

public static class OnMapConditionRemove
{
    public static event System.Action<Conditions> OnConditionRemoved;

    public static void HandleEvent(WSData data)
    {
        if (data.condition.bearer == PlayerDataManager.playerData.instance)
        {
            ConditionsManager.WSRemoveCondition(data.condition.instance);
        }

        OnConditionRemoved?.Invoke(data.condition);
    }
}
