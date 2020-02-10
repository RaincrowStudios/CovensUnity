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
        [SerializeField] private AbstractGameMasterController _gameMasterController;

        private GameObject[,] _grid = new GameObject[0, 0]; // Grid with all the game objects inserted
        private List<GameObject> _characters = new List<GameObject>(); // List with all characters
        private IStateMachine<IBattleModel> _stateMachine; // State machine with all phases
        private IGridModel _gridModel;
        private string _battleId;

        public virtual void OnEnable()
        {
            _battleId = System.Guid.NewGuid().ToString();
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
            // Construct grid builder
            GridBuilder gridBuilder;
            {
                gridBuilder = new GridBuilder()
                {
                    MaxCellsPerLine = 25,
                    MaxCellsPerColumn = 25,
                };

                gridBuilder.CellBuilders = new CellBuilder[gridBuilder.MaxCellsPerLine, gridBuilder.MaxCellsPerColumn];

                for (int i = 0; i < gridBuilder.MaxCellsPerLine; i++)
                {
                    for (int j = 0; j < gridBuilder.MaxCellsPerColumn; j++)
                    {
                        gridBuilder.CellBuilders[i, j] = new CellBuilder();
                    }
                }
            }

            // Create grid model
            _gridModel = new GridModel(gridBuilder);

            // Create grid
            Coroutine<GameObject[,]> createGrid = this.StartCoroutine<GameObject[,]>(_gridFactory.Create(_gridModel));
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
            IBattleModel battleModel = new BattleModel()
            {
                Id = _battleId,
                Grid = _gridModel,
                GameMaster = _gameMasterController
            };

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

            _gridModel = null;
        }
    }
}