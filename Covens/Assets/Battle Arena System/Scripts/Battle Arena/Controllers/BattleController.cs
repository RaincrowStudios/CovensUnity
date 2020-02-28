using Raincrow.BattleArena.Factory;
using Raincrow.BattleArena.Views;
using Raincrow.BattleArena.Model;
using Raincrow.BattleArena.Phase;
using Raincrow.StateMachines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Raincrow.Loading.View;
using Raincrow.Services;
using Raincrow.BattleArena.UI;

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
        [SerializeField] private QuickCastUI _quickCastView;

        private GameObject[,] _grid = new GameObject[0, 0]; // Grid with all the game objects inserted
        private List<AbstractCharacterView<IWitchModel, IWitchViewModel>> _witches = new List<AbstractCharacterView<IWitchModel, IWitchViewModel>>(); // List with all witches
        private List<AbstractCharacterView<ISpiritModel, ISpiritViewModel>> _spirits = new List<AbstractCharacterView<ISpiritModel, ISpiritViewModel>>(); // List with all spirits
        private IStateMachine _stateMachine; // State machine with all phases
        private ITurnModel _turnModel;
        private CellView _selectedView;


        protected virtual void OnValidate()
        {
            if (_serviceLocator == null)
            {
                _serviceLocator = FindObjectOfType<ServiceLocator>();
            }

            // Could not lazily initialize Service Locator
            if (_serviceLocator == null)
            {
                Debug.LogError("Could not find Service Locator!");
            }
        }

        public IEnumerator StartBattle(string battleId, IGridModel gridModel, IList<IWitchModel> witches, IList<ISpiritModel> spirits, ILoadingView loadingView = null)
        {
            if (!isActiveAndEnabled)
            {
                gameObject.SetActive(true);
            }

            _quickCastView.Init(OnClickFly, OnClickSummon);

            loadingView?.UpdateMessage("Instantiang grid");
            yield return StartCoroutine(InstantiateGrid(gridModel));

            loadingView?.UpdateMessage("Placing characters");
            yield return StartCoroutine(PlaceCharacters(gridModel, witches, spirits));

            loadingView?.UpdateMessage("Starting state machine");
            yield return StartCoroutine(StartStateMachine(battleId, gridModel, witches, spirits, _gameMasterController));

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
            _witches = new List<AbstractCharacterView<IWitchModel, IWitchViewModel>>();
            _spirits = new List<AbstractCharacterView<ISpiritModel, ISpiritViewModel>>();
            Camera battleCamera = _serviceLocator.GetBattleCamera();

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
                            Coroutine<AbstractCharacterView<ISpiritModel, ISpiritViewModel>> createCharacter =
                                this.StartCoroutine<AbstractCharacterView<ISpiritModel, ISpiritViewModel>>(_spiritFactory.Create(cellGameObject.transform, spirit));
                            yield return createCharacter;

                            // add spirit and init
                            //AbstractCharacterView<ISpiritModel> spiritView = ;
                            //spiritView.Init(spirit, battleCamera);
                            _spirits.Add(createCharacter.ReturnValue);
                        }
                        else if (dictWitches.TryGetValue(cell.ObjectId, out IWitchModel witch)) // has a character/item
                        {
                            GameObject cellGameObject = _grid[i, j];
                            Coroutine<AbstractCharacterView<IWitchModel, IWitchViewModel>> createCharacter =
                                this.StartCoroutine<AbstractCharacterView<IWitchModel, IWitchViewModel>>(_witchFactory.Create(cellGameObject.transform, witch));
                            yield return createCharacter;

                            // add a witch and init
                            //AbstractCharacterView<IWitchModel> witchModel = createCharacter.ReturnValue;
                            //witchModel.Init(witch, battleCamera);
                            _witches.Add(createCharacter.ReturnValue);
                        }
                    }

                    yield return null;
                }
            }
        }

        private IEnumerator StartStateMachine(string battleId, IGridModel gridModel, IList<IWitchModel> witches, IList<ISpiritModel> spirits, IGameMasterController gameMasterController)
        {
            // We yield at each allocation to avoid a lot of allocations on a single frame
            _turnModel = new TurnModel();

            IBattleModel battleModel = new BattleModel()
            {
                Id = battleId,
                Grid = gridModel,
                Witches = witches,
                Spirits = spirits
            };
            yield return null;

            InitiativePhase initiativePhase = new InitiativePhase(this, _gameMasterController, _turnModel, battleModel);
            yield return null;

            PlanningPhase planningPhase = new PlanningPhase(this, _serviceLocator.GetCharactersTurnOrderView(), _turnModel, battleModel);
            yield return null;

            ActionResolutionPhase actionResolutionPhase = new ActionResolutionPhase(this);
            yield return null;

            BanishmentPhase banishmentPhase = new BanishmentPhase(this);
            yield return null;

            IState[] battlePhases = new IState[4]
            {
                initiativePhase,
                planningPhase,
                actionResolutionPhase,
                banishmentPhase
            };

            _stateMachine = new StateMachine(battlePhases);
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
                foreach (AbstractCharacterView<ISpiritModel, ISpiritViewModel> spirit in _spirits)
                {
                    spirit.FaceCamera(battleCamera.transform.rotation, forward);
                }
                foreach (AbstractCharacterView<IWitchModel, IWitchViewModel> witch in _witches)
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
            QuickCastUI.QuickCastMenus menu = cellView.IsEmpty ? QuickCastUI.QuickCastMenus.Action : QuickCastUI.QuickCastMenus.Spell;
            _selectedView = cellView;
            _quickCastView.OnClickCell(menu);
        }

        public void OnClickFlee()
        {
            if (_turnModel.MaxActionsAllowed - _turnModel.ActionsRequested.Count <= 0)
            {
                return;
            }

            _turnModel.AddAction(new FleeActionModel());
            _serviceLocator.GetCharactersTurnOrderView().UpdateActionsPoints(_turnModel.ActionsRequested.Count);
        }

        private void OnClickFly()
        {
            if (_turnModel.MaxActionsAllowed - _turnModel.ActionsRequested.Count <= 0)
            {
                return;
            }

            BattleSlot slot = new BattleSlot() { Col = _selectedView.CellModel.Y, Row = _selectedView.CellModel.X };
            _turnModel.AddAction(new MoveActionModel() { Position = slot });
            _serviceLocator.GetCharactersTurnOrderView().UpdateActionsPoints(_turnModel.ActionsRequested.Count);
        }

        private void OnClickSummon()
        {
            if (_turnModel.MaxActionsAllowed - _turnModel.ActionsRequested.Count <= 0)
            {
                return;
            }

            UIMainScreens.PushEventAnalyticUI(UIMainScreens.Arena, UIMainScreens.SummonArena);
            Views.UISummoning.Open(OnSummon);
        }

        private void OnSummon(string spiritID)
        {
            BattleSlot slot = new BattleSlot() { Col = _selectedView.CellModel.Y, Row = _selectedView.CellModel.X };
            _turnModel.AddAction(new SummonActionModel() { Position = slot, SpiritId = spiritID });
            _serviceLocator.GetCharactersTurnOrderView().UpdateActionsPoints(_turnModel.ActionsRequested.Count);
        }

        #region ICoroutineStarter

        public Coroutine Invoke(IEnumerator routine)
        {
            return StartCoroutine(routine);
        }

        public void StopInvoke(IEnumerator routine)
        {
            StopCoroutine(routine);
        }

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

    public interface ITurnModel
    {
        //IBattleModel Battle { get; set; }
        //IGameMasterController GameMaster { get; set; }
        string[] PlanningOrder { get; set; }
        float PlanningMaxTime { get; set; }
        int MaxActionsAllowed { get; set; }
        IList<IActionModel> ActionsRequested { get; }
        void AddAction(IActionModel action);
    }

    public class TurnModel : ITurnModel
    {
        //public IBattleModel Battle { get; set; }
        //public IGameMasterController GameMaster { get; set; }
        public string[] PlanningOrder { get; set; }
        public float PlanningMaxTime { get; set; }
        public int MaxActionsAllowed { get; set; }
        public IList<IActionModel> ActionsRequested { get; set; }

        public TurnModel()
        {
            PlanningOrder = new string[0];
            ActionsRequested = new List<IActionModel>();
        }

        public void AddAction(IActionModel action)
        {
            ActionsRequested.Add(action);
        }
    }
}