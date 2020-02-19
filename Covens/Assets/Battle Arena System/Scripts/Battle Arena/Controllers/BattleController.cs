using Raincrow.BattleArena.Factory;
using Raincrow.BattleArena.View;
using Raincrow.BattleArena.Model;
using Raincrow.BattleArena.Phase;
using Raincrow.BattleArena.UI;
using Raincrow.StateMachines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Raincrow.Loading.View;
using Raincrow.Services;

namespace Raincrow.BattleArena.Controller
{
    public class BattleController : MonoBehaviour, ICoroutineHandler
    {        
        [SerializeField] private Transform _cellsTransform;
        [SerializeField] private ServiceLocator _serviceLocator;
        [SerializeField] private AbstractGridGameObjectFactory _gridFactory; // Factory class responsible for creating our Grid        
        [SerializeField] private SpiritGameObjectFactory _spiritFactory; // Factory class responsible for creating our Spirits   
        [SerializeField] private WitchGameObjectFactory _witchFactory; // Factory class responsible for creating our Witchs   
        [SerializeField] private AbstractGameMasterController _gameMasterController;
        [SerializeField] private QuickCastUI _quickCastUI;

        private GameObject[,] _grid = new GameObject[0, 0]; // Grid with all the game objects inserted
        private List<AbstractCharacterView<IWitchModel>> _witches = new List<AbstractCharacterView<IWitchModel>>(); // List with all witches
        private List<AbstractCharacterView<ISpiritModel>> _spirits = new List<AbstractCharacterView<ISpiritModel>>(); // List with all spirits
        private IStateMachine<ITurnController> _stateMachine; // State machine with all phases

        public TurnController TurnController { get; private set; }
        public QuickCastUI QuickCastUI { get => _quickCastUI; }

        public IEnumerator StartBattle(string battleId, IGridModel gridModel, IList<IWitchModel> witches, IList<ISpiritModel> spirits, ILoadingView loadingView = null)
        {
            if (!isActiveAndEnabled)
            {
                gameObject.SetActive(true);
            }

            loadingView?.UpdateMessage("Instantiang grid");
            yield return StartCoroutine(InstantiateGrid(gridModel));
          
            loadingView?.UpdateMessage("Placing characters");
            yield return StartCoroutine(PlaceCharacters(gridModel, witches, spirits));

            loadingView?.UpdateMessage("Starting state machine");
            yield return StartCoroutine(StartStateMachine(battleId, gridModel, _gameMasterController));

            loadingView?.UpdateMessage("Starting battle");

            // Update Loop
            StartCoroutine(UpdateCharacters());
            StartCoroutine(UpdateStateMachine());            
        }

        private IEnumerator InstantiateGrid(IGridModel gridModel)
        {
            // Create grid
            Coroutine<GameObject[,]> createGrid = this.StartCoroutine<GameObject[,]>(_gridFactory.Create(gridModel, OnCellClick));
            yield return createGrid;
            _grid = createGrid.ReturnValue;
        }        

        private IEnumerator PlaceCharacters(IGridModel gridModel, IList<IWitchModel> witches, IList<ISpiritModel> spirits)
        {
            // Initialize list of characters            
            _witches = new List<AbstractCharacterView<IWitchModel>>();
            _spirits = new List<AbstractCharacterView<ISpiritModel>>();

            Dictionary<string, IWitchModel> dictWitches = new Dictionary<string, IWitchModel>();
            foreach (IWitchModel witch in witches)
            {
                dictWitches.Add(witch.Id, witch);
            }

            Dictionary<string, ISpiritModel> dictSpirits = new Dictionary<string, ISpiritModel>();
            foreach (ISpiritModel spirit in spirits)
            {
                dictSpirits.Add(spirit.Id, spirit);
            }            

            for (int i = 0; i < gridModel.MaxCellsPerRow; i++)
            {
                for (int j = 0; j < gridModel.MaxCellsPerColumn; j++)
                {
                    ICellModel cell = gridModel.Cells[i, j];
                    if (!string.IsNullOrEmpty(cell.ObjectId))
                    {
                        if (dictSpirits.TryGetValue(cell.ObjectId, out ISpiritModel spirit)) // has a character/item
                        {
                            GameObject cellGameObject = _grid[i, j];
                            Coroutine<AbstractCharacterView<ISpiritModel>> createCharacter = this.StartCoroutine<AbstractCharacterView<ISpiritModel>>(_spiritFactory.Create(cellGameObject.transform, spirit));
                            yield return createCharacter;

                            // add spirit and init
                            AbstractCharacterView<ISpiritModel> spiritView = createCharacter.ReturnValue;
                            spiritView.Init(spirit);
                            _spirits.Add(spiritView);
                        }
                        else if (dictWitches.TryGetValue(cell.ObjectId, out IWitchModel witch)) // has a character/item
                        {
                            GameObject cellGameObject = _grid[i, j];
                            Coroutine<AbstractCharacterView<IWitchModel>> createCharacter = this.StartCoroutine<AbstractCharacterView<IWitchModel>>(_witchFactory.Create(cellGameObject.transform, witch));
                            yield return createCharacter;

                            // add a witch and init
                            AbstractCharacterView<IWitchModel> witchModel = createCharacter.ReturnValue;
                            witchModel.Init(witch);
                            _witches.Add(witchModel);
                        }
                    }

                    yield return null;
                }
            }
        }

        private IEnumerator StartStateMachine(string battleId, IGridModel gridModel, IGameMasterController gameMasterController)
        {
            // We yield at each allocation to avoid a lot of allocations on a single frame
            TurnController = new TurnController()
            {
                Battle = new BattleModel()
                {
                    Id = battleId,
                    Grid = gridModel
                },
                GameMaster = gameMasterController,
                PlanningOrder = new string[0],
                MaxActionsAllowed = 3,
                Actions = new List<ActionModel>()
            };
            yield return null;

            InitiativePhase initiativePhase = new InitiativePhase(this);
            yield return null;

            PlanningPhase planningPhase = new PlanningPhase(this);
            yield return null;

            ActionResolutionPhase actionResolutionPhase = new ActionResolutionPhase(this);
            yield return null;

            BanishmentPhase banishmentPhase = new BanishmentPhase(this);
            yield return null;

            IState<ITurnController>[] battlePhases = new IState<ITurnController>[4]
            {
                initiativePhase,
                planningPhase,
                actionResolutionPhase,
                banishmentPhase
            };

            _stateMachine = new StateMachine<ITurnController>(TurnController, battlePhases);
            yield return _stateMachine.Start<InitiativePhase>();
        }

        private IEnumerator UpdateCharacters()
        {
            Camera battleCamera = _serviceLocator.GetBattleCamera();
            while (enabled)
            {
                yield return new WaitForEndOfFrame();

                // Update Characters
                Vector3 forward = battleCamera.transform.rotation * Vector3.up;
                foreach (AbstractCharacterView<ISpiritModel> spirit in _spirits)
                {                    
                    spirit.FaceCamera(battleCamera.transform.rotation, forward);
                }
                foreach (AbstractCharacterView<IWitchModel> witch in _witches)
                {
                    witch.FaceCamera(battleCamera.transform.rotation, forward);
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

        public void EndBattle()
        {
            // Destroy characters
            for (int i = _spirits.Count - 1; i >= 0; i--)
            {
                Destroy(_spirits[i]);
            }
            _spirits.Clear();

            // Destroy grid 
            for (int i = 0; i < _cellsTransform.childCount; i++)
            {
                Destroy(_cellsTransform.GetChild(i).gameObject);
            }
            _grid = new GameObject[0, 0];

            if (isActiveAndEnabled)
            {
                gameObject.SetActive(false);
            }
        }

        private void OnCellClick(CellView cellView)
        {
            _quickCastUI.OnClickCell(cellView);
        }

        public void OnClickFlee()
        {
            QuickCastUI.OnClickFlee();
        }

        #region ICoroutineStarter

        public Coroutine<T> Invoke<T>(IEnumerator<T> routine)
        {
            return this.StartCoroutine<T>(routine);
        }

        public void StopInvoke<T>(IEnumerator<T> routine)
        {
            StopCoroutine(routine);
        }

        #endregion
    }

    public interface ITurnController
    {
        IBattleModel Battle { get; set; }
        IGameMasterController GameMaster { get; set; }
        string[] PlanningOrder { get; set; }
        float PlanningMaxTime { get; set; }
        int MaxActionsAllowed { get; set; }
        int RemainingActions { get; }
        List<ActionModel> Actions { get; }
    }

    public struct TurnController : ITurnController
    {
        public IBattleModel Battle { get; set; }
        public IGameMasterController GameMaster { get; set; }
        public string[] PlanningOrder { get; set; }
        public float PlanningMaxTime { get; set; }
        public int MaxActionsAllowed { get; set; }
        public int RemainingActions { get { return Actions ==  null ? MaxActionsAllowed : MaxActionsAllowed - Actions.Count; } }
        public List<ActionModel> Actions { get; set; }

        public void AddAction(ActionModel action)
        {
            Actions.Add(action);
        }
    }
}