using Raincrow.BattleArena.Factory;
using Raincrow.BattleArena.Views;
using Raincrow.BattleArena.Model;
using Raincrow.BattleArena.Phases;
using Raincrow.StateMachines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Raincrow.Loading.View;
using Raincrow.Services;
using Raincrow.BattleArena.Controllers;

namespace Raincrow.BattleArena.Controller
{
    public class BattleController : MonoBehaviour, ICoroutineHandler, IGridUIModel
    {
        //[SerializeField] private Transform _cellsTransform;
        [SerializeField] private ServiceLocator _serviceLocator;
        [SerializeField] private AbstractGridGameObjectFactory _gridFactory; // Factory class responsible for creating our Grid        
        [SerializeField] private SpiritGameObjectFactory _spiritFactory; // Factory class responsible for creating our Spirits   
        [SerializeField] private WitchGameObjectFactory _witchFactory; // Factory class responsible for creating our Witchs   
        [SerializeField] private AbstractGameMasterController _gameMasterController;

        [Header("Camera Movement")]
        [SerializeField] private float _cameraSpeed = 20f;
        [SerializeField] private float _cameraDecceleration = 0.15f;
        [SerializeField] private float _cameraTargetHeight = 1f;

        private IStateMachine _stateMachine; // State machine with all phases
        private IGridModel _gridModel;
        private ITurnModel _turnModel;
        private IQuickCastView _quickCastView;
        private IEnergyView _energyView;
        private IPlayerBadgeView _playerBadgeView;
        private ICameraTargetController _cameraTargetController;
        private IDictionary<string, ICharacterController<IWitchModel, IWitchUIModel>> _dictWitchesViews = new Dictionary<string, ICharacterController<IWitchModel, IWitchUIModel>>();
        private IDictionary<string, ICharacterController<ISpiritModel, ISpiritUIModel>> _dictSpiritViews = new Dictionary<string, ICharacterController<ISpiritModel, ISpiritUIModel>>();

        // Properties

        public ICellUIModel[,] Cells { get; private set; } = new ICellUIModel[0, 0]; // Grid with all the game objects inserted
        public ICollection<ICharacterController<IWitchModel, IWitchUIModel>> WitchesViews => _dictWitchesViews.Values; // List with all witches
        public ICollection<ICharacterController<ISpiritModel, ISpiritUIModel>> SpiritsViews => _dictSpiritViews.Values; // List with all spirits
        public int MaxCellsPerRow => _gridModel.MaxCellsPerRow;
        public int MaxCellsPerColumn => _gridModel.MaxCellsPerColumn;        

        protected virtual void OnEnable()
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

            if (_quickCastView == null)
            {
                _quickCastView = _serviceLocator.GetQuickCastView();
            }

            if (_energyView == null)
            {
                _energyView = _serviceLocator.GetEnergyView();
            }

            if (_playerBadgeView == null)
            {
                _playerBadgeView = _serviceLocator.GetPlayerBadgeView();
            }

            if (_cameraTargetController == null)
            {
                _cameraTargetController = _serviceLocator.GetCameraTargetController();
            }
        }

        public IEnumerator StartBattle(string battleId, string playerId, IGridModel gridModel, IList<IWitchModel> witches, IList<ISpiritModel> spirits, ILoadingView loadingView = null)
        {
            if (!isActiveAndEnabled)
            {
                gameObject.SetActive(true);
            }

            _gridModel = gridModel;

            loadingView?.UpdateMessage("Instantiang grid");
            yield return StartCoroutine(InstantiateGrid());

            loadingView?.UpdateMessage("Setup Camera");
            yield return StartCoroutine(SetupCamera());

            loadingView?.UpdateMessage("Placing characters");
            yield return StartCoroutine(PlaceCharacters(witches, spirits));

            loadingView?.UpdateMessage("Initialize Player Interface");
            IWitchModel witchModel = _dictWitchesViews[playerId].Model;
            yield return StartCoroutine(InitializePlayerUI(witchModel));

            loadingView?.UpdateMessage("Starting state machine");
            yield return StartCoroutine(StartStateMachine(battleId, _gameMasterController));

            loadingView?.UpdateMessage("Starting battle");

            // Update Loop
            StartCoroutine(UpdateCharacters());
            StartCoroutine(UpdateStateMachine());
            StartCoroutine(UpdatePlayerUI(witchModel));
            StartCoroutine(UpdateCamera());
        }

        private IEnumerator InstantiateGrid()
        {
            // Create grid
            Coroutine<ICellUIModel[,]> createGrid = this.StartCoroutine<ICellUIModel[,]>(_gridFactory.Create(_gridModel));
            yield return createGrid;
            Cells = createGrid.ReturnValue;
        }

        private IEnumerator PlaceCharacters(IList<IWitchModel> witches, IList<ISpiritModel> spirits)
        {
            // Initialize list of characters            
            //WitchesViews = new List<ICharacterView<IWitchModel, IWitchUIModel>>();
            //SpiritsViews = new List<ICharacterView<ISpiritModel, ISpiritUIModel>>();
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

            for (int i = 0; i < _gridModel.MaxCellsPerRow; i++)
            {
                for (int j = 0; j < _gridModel.MaxCellsPerColumn; j++)
                {
                    ICellModel cell = _gridModel.Cells[i, j];
                    if (!string.IsNullOrEmpty(cell.ObjectId))
                    {
                        if (dictSpirits.TryGetValue(cell.ObjectId, out ISpiritModel spirit)) // has a character/item
                        {
                            ICellUIModel cellView = Cells[i, j];
                            Coroutine<ICharacterController<ISpiritModel, ISpiritUIModel>> createCharacter =
                                this.StartCoroutine<ICharacterController<ISpiritModel, ISpiritUIModel>>(_spiritFactory.Create(cellView.Transform, spirit));
                            yield return createCharacter;

                            // add spirit and init
                            //AbstractCharacterView<ISpiritModel> spiritView = ;
                            //spiritView.Init(spirit, battleCamera);
                            _dictSpiritViews.Add(spirit.Id, createCharacter.ReturnValue);
                        }
                        else if (dictWitches.TryGetValue(cell.ObjectId, out IWitchModel witch)) // has a character/item
                        {
                            ICellUIModel cellView = Cells[i, j];
                            Coroutine<ICharacterController<IWitchModel, IWitchUIModel>> createCharacter =
                                this.StartCoroutine<ICharacterController<IWitchModel, IWitchUIModel>>(_witchFactory.Create(cellView.Transform, witch));
                            yield return createCharacter;

                            // add a witch and init
                            //AbstractCharacterView<IWitchModel> witchModel = createCharacter.ReturnValue;
                            //witchModel.Init(witch, battleCamera);
                            _dictWitchesViews.Add(witch.Id, createCharacter.ReturnValue);
                        }
                    }

                    yield return null;
                }
            }
        }

        private IEnumerator StartStateMachine(string battleId, IGameMasterController gameMasterController)
        {
            // We yield at each allocation to avoid a lot of allocations on a single frame
            _turnModel = new TurnModel();

            IBattleModel battleModel = new BattleModel()
            {
                Id = battleId,
                GridUI = this
            };
            yield return null;

            InitiativePhase initiativePhase = new InitiativePhase(this, _gameMasterController, _turnModel, battleModel);
            yield return null;

            PlanningPhase planningPhase = new PlanningPhase(this,
                _gameMasterController,
                _quickCastView,
                _serviceLocator,
                _serviceLocator.GetCharactersTurnOrderView(),
                _turnModel,
                battleModel,
                Cells,
                _serviceLocator.GetCountdownView(),
                _serviceLocator.GetEnergyView(),
                _serviceLocator.GetPlayerBadgeView(),
                _serviceLocator.GetCameraTargetController(),
                _cameraSpeed);
            yield return null;

            ActionResolutionPhase actionResolutionPhase = new ActionResolutionPhase(this, battleModel, _turnModel, _serviceLocator.GetBarEventLogView());
            yield return null;

            BanishmentPhase banishmentPhase = new BanishmentPhase(this, battleModel, _turnModel, _serviceLocator.GetBarEventLogView());
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
                foreach (ICharacterController<ISpiritModel, ISpiritUIModel> spirit in SpiritsViews)
                {
                    spirit.FaceCamera(battleCamera.transform.rotation, forward);
                }
                foreach (ICharacterController<IWitchModel, IWitchUIModel> witch in WitchesViews)
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

        private IEnumerator InitializePlayerUI(IWitchModel witchModel)
        {
            // Show Player Badge View
            yield return StartCoroutine(_playerBadgeView.Init(witchModel));
        }

        private IEnumerator SetupCamera()
        {
            IGridGameObjectModel model = _gridFactory.GridGameObjectModel;
            Vector2 cellScale = model.CellScale;
            Vector2 spacing = model.Spacing;

            Vector3 cameraBounds = Vector3.zero;

            // Set X camera bounds
            cameraBounds.x = (_gridModel.MaxCellsPerColumn - 1) * (cellScale.x * 0.5f);
            cameraBounds.x += spacing.x * (_gridModel.MaxCellsPerColumn - 1) * 0.5f;

            // Set Y camera bounds
            cameraBounds.y = 0f;

            // Set Z camera bounds
            cameraBounds.z = (_gridModel.MaxCellsPerRow - 1) * (cellScale.y * 0.5f);
            cameraBounds.z += spacing.y * (_gridModel.MaxCellsPerColumn - 1) * 0.5f;

            Vector3 origin = transform.position;
            origin.y = _cameraTargetHeight;
            _cameraTargetController.SetBounds(origin, cameraBounds);
            yield return null;
        }        

        private IEnumerator UpdateCamera()
        {
            Vector3 dragMovement = Vector3.zero;            
            while (enabled)
            {
                yield return new WaitForEndOfFrame();                

                if (Input.GetMouseButton(0))
                {
                    dragMovement.x = -Input.GetAxis("Mouse X") * _cameraSpeed * Time.deltaTime;
                    dragMovement.z = -Input.GetAxis("Mouse Y") * _cameraSpeed * Time.deltaTime;
                }     
                else
                {
                    dragMovement = Vector3.MoveTowards(dragMovement, Vector3.zero, _cameraDecceleration * Time.deltaTime);
                }

                if (dragMovement.sqrMagnitude > Mathf.Epsilon)
                {
                    _cameraTargetController.Move(dragMovement);
                }              
            }
        }

        private IEnumerator UpdatePlayerUI(IWitchModel witchModel)
        {
            // Show Energy View
            _energyView.Show();
            _playerBadgeView.Show();

            while (enabled)
            {
                _energyView.UpdateView(witchModel.Energy, witchModel.BaseEnergy);
                yield return null;
            }

            _playerBadgeView.Hide();
            _energyView.Hide();
        }

        public void EndBattle()
        {
            //// Destroy characters
            //for (int i = _spirits.Count - 1; i >= 0; i--)
            //{
            //    Destroy(_spirits[i]);
            //}
            //_spirits.Clear();            

            //for (int i = _spirits.Count - 1; i >= 0; i--)
            //{
            //    Destroy(_spirits[i]);
            //}
            //_spirits.Clear();

            //// Destroy grid 
            //for (int i = 0; i < _cellsTransform.childCount; i++)
            //{
            //    Destroy(_cellsTransform.GetChild(i).gameObject);
            //}
            //_grid = new ICellView[0, 0];            

            if (isActiveAndEnabled)
            {
                gameObject.SetActive(false);
            }
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

        #region IGridView

        public void SetObjectToGrid(IObjectUIModel objectUIModel, IObjectModel objectModel, int row, int col)
        {
            // Set cell transform position to object UI Model position
            ICellUIModel cellUIModel = Cells[row, col];
            objectUIModel.Transform.position = cellUIModel.Transform.position;

            // Add object model in the grid model
            _gridModel.SetObjectToGrid(objectModel, row, col);

            // Get transform of our character
            Transform characterTransform = objectUIModel.Transform;
            ICellUIModel targetCellView = Cells[row, col];
            Transform cellTransform = targetCellView.Transform;
        }

        public void RemoveObjectFromGrid(IObjectUIModel objectUIModel, IObjectModel objectModel)
        {
            _gridModel.RemoveObjectFromGrid(objectModel);

            if (objectModel.ObjectType == ObjectType.Witch)
            {
                _dictWitchesViews.Remove(objectModel.Id);
            }
            else if (objectModel.ObjectType == ObjectType.Spirit)
            {
                _dictSpiritViews.Remove(objectModel.Id);
            }
        }

        public void RecycleCharacter(GameObject character)
        {
            _serviceLocator.GetObjectPool().Recycle(character);
        }

        public IEnumerator SpawnObjectOnGrid(IObjectModel objectModel, int row, int col)
        {
            if (objectModel.ObjectType == ObjectType.Spirit)
            {
                // Create the new spirit
                ICellUIModel targetCellView = Cells[row, col];
                IEnumerator<ICharacterController<ISpiritModel, ISpiritUIModel>> createSpirit = _spiritFactory.Create(targetCellView.Transform, objectModel as ISpiritModel);
                yield return createSpirit;

                // Add the new spirit
                ICharacterController<ISpiritModel, ISpiritUIModel> spiritView = createSpirit.Current;
                _dictSpiritViews.Add(spiritView.Model.Id, spiritView);

                // Set cell transform position to object UI Model position
                spiritView.UIModel.Transform.position = targetCellView.Transform.position;

                // Add object model in the grid model
                _gridModel.SetObjectToGrid(spiritView.Model, row, col);
            }
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
        IList<IActionRequestModel> RequestedActions { get; }
        IDictionary<string, IList<IActionResponseModel>> ResponseActions { get; }
        void Reset();
        //BattleSlot SelectedSlot { get; set; }
    }

    public class TurnModel : ITurnModel
    {
        //public IBattleModel Battle { get; set; }
        //public IGameMasterController GameMaster { get; set; }
        public string[] PlanningOrder { get; set; } = new string[0];
        public float PlanningMaxTime { get; set; }
        public int MaxActionsAllowed { get; set; }
        public IList<IActionRequestModel> RequestedActions { get; private set; } = new List<IActionRequestModel>();
        public IDictionary<string, IList<IActionResponseModel>> ResponseActions { get; } = new Dictionary<string, IList<IActionResponseModel>>();

        //public BattleSlot SelectedSlot { get; set; }

        public void Reset()
        {
            PlanningOrder = new string[0];
            RequestedActions.Clear();
            ResponseActions.Clear();
            PlanningMaxTime = 0f;
            MaxActionsAllowed = 0;
        }
    }
}