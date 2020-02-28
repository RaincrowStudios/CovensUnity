using Raincrow.BattleArena.Model;
using Raincrow.BattleArena.Views;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Raincrow.BattleArena.Factory
{
    public abstract class AbstractGridGameObjectFactory : MonoBehaviour
    {
        public abstract IEnumerator<GameObject[,]> Create(IGridModel gridModel, UnityAction<CellView> cellClickCallback);
    }
}