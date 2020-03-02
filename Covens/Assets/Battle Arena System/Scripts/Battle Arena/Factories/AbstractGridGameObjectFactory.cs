using Raincrow.BattleArena.Model;
using System.Collections.Generic;
using Raincrow.BattleArena.Views;
using UnityEngine;

namespace Raincrow.BattleArena.Factory
{
    public abstract class AbstractGridGameObjectFactory : MonoBehaviour
    {
        public abstract IEnumerator<ICellView[,]> Create(IGridModel gridModel);
    }
}