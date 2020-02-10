using Raincrow.BattleArena.Factory;
using Raincrow.BattleArena.Model;
using Raincrow.BattleArena.Phase;
using Raincrow.StateMachines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.BattleArena.Controller
{
    public class BattleController : MonoBehaviour
    {
        [SerializeField] private Camera _battleCamera;
        [SerializeField] private Transform _cellsTransform;  
        [SerializeField] private AbstractGridGameObjectFactory _gridFactory; // Factory class responsible for creating our Grid        
        [SerializeField] private AbstractCharacterGameObjectFactory _characterFactory; // Factory class responsible for creating our Characters        

        /// <summary>
        /// Grid with all the game objects inserted
        /// </summary>
        private GameObject[,] _grid = new GameObject[0, 0];

        /// <summary>
        /// List with all characters
        /// </summary>
        private List<GameObject> _characters = new List<GameObject>();

        /// <summary>
        /// State machine with all phases
        /// </summary>
        private IStateMachine<IBattleModel> _stateMachine;

        public virtual void OnEnable()
        {
            StartCoroutine(OnEnableCoroutine());
        }        

        public virtual void OnDisable()
        {
            DestroyGrid();
        }

        private IEnumerator OnEnableCoroutine()
        {
            yield return StartCoroutine(InstantiateGrid());

            yield return StartCoroutine(PlaceCharacters());

            yield return StartCoroutine(StartStateMachine());

            // Update Loop
            StartCoroutine(UpdateCharacters());
            StartCoroutine(UpdateStateMachine());
        }

        private IEnumerator InstantiateGrid()
        {
            // Create grid
            Coroutine<GameObject[,]> createGrid = this.StartCoroutine<GameObject[,]>(_gridFactory.Create());
            yield return createGrid;
            _grid = createGrid.ReturnValue;
        }

        private IEnumerator PlaceCharacters()
        {
            // Create characters
            int maxCellsPerLine = _grid.GetLength(0);
            int maxCellsPerColumn = _grid.GetLength(1);

            // Initialize list of characters
            _characters = new List<GameObject>(maxCellsPerColumn * maxCellsPerLine);

            for (int i = 0; i < maxCellsPerLine; i++)
            {
                for (int j = 0; j < maxCellsPerColumn; j++)
                {
                    GameObject cellGameObject = _grid[i, j];
                    if (cellGameObject != null && Random.Range(0f, 1f) < 0.1f)
                    {
                        Coroutine<GameObject> createCharacter = this.StartCoroutine<GameObject>(_characterFactory.Create(cellGameObject.transform));
                        yield return createCharacter;

                        // add a character
                        _characters.Add(createCharacter.ReturnValue);
                    }
                }
            }
        }

        private IEnumerator StartStateMachine()
        {
            IBattleModel battleModel = new BattleModel();

            IState<IBattleModel>[] battlePhases = new IState<IBattleModel>[4]
            {
                new InitiativePhase(),
                new PlanningPhase(),
                new ActionResolutionPhase(),
                new BanishmentPhase()
            };

            _stateMachine = new StateMachine<IBattleModel>(battleModel, battlePhases);
            yield return _stateMachine.Start<InitiativePhase>(); // initiativePhase
        }

        private IEnumerator UpdateCharacters()
        {
            while (enabled)
            {
                yield return new WaitForEndOfFrame();                

                // Update Characters
                Vector3 forward = _battleCamera.transform.rotation * Vector3.up;
                foreach (GameObject character in _characters)
                {
                    Vector3 worldPosition = character.transform.position + _battleCamera.transform.rotation * Vector3.forward;
                    character.transform.LookAt(worldPosition, forward);
                }                
            }
        }

        private IEnumerator UpdateStateMachine()
        {
            while (enabled)
            {
                // Update state machine
                yield return StartCoroutine(_stateMachine.UpdateState());
            }
        }

        private void DestroyGrid()
        {
            // Destroy characters
            for (int i = _characters.Count - 1; i >= 0; i--)
            {
                Destroy(_characters[i]);
            }
            _characters.Clear();

            // Destroy grid 
            for (int i = 0; i < _cellsTransform.childCount; i++)
            {
                Destroy(_cellsTransform.GetChild(i).gameObject);
            }
            _grid = new GameObject[0, 0];
        }
    }
}