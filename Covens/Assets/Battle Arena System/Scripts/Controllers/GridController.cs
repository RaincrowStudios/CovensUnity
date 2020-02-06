using Raincrow.BattleArena.Factory;
using UnityEngine;

namespace Raincrow.BattleArena.Controller
{
    public class GridController : MonoBehaviour
    {
        [Header("Factories")]
        [SerializeField] private AbstractGridGameObjectFactory _gridFactory; // Factory class responsible for creating our Grid        
        [SerializeField] private AbstractCharacterGameObjectFactory _characterFactory; // Factory class responsible for creating our Characters        

        /// <summary>
        /// Grid with all the game objects inserted
        /// </summary>
        private GameObject[,] _grid = new GameObject[0, 0];

        public virtual void OnEnable()
        {
            _grid = _gridFactory.Create();

            
        }
    }
}