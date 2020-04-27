using Raincrow.BattleArena.Controllers;
using Raincrow.BattleArena.Events;
using Raincrow.BattleArena.Model;
using Raincrow.BattleArena.Views;
using Raincrow.StateMachines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.BattleArena.Phases
{
    public class PlanningPhase : IState
    {
        private readonly static string BattleCellLayerName = "BattleCell";
        private readonly static float MaxRaycastDistance = 3000f;
        //private readonly static float DragSpeed = 30f;
        //private readonly static float MoveToTargetSpeed = 600f;

        // Variables
        private float _startTime = 0f;
        private IEnumerator<bool?> _sendFinishPlanningPhase;
        private bool? _isPlanningPhaseFinished;
        private ICoroutineHandler _coroutineStarter;
        private IGameMasterController _gameMaster;
        private ICountdownView _countdownView;
        private IQuickCastView _quickCastView;
        private IEnergyView _energyView;
        private IPlayerBadgeView _playerBadgeView;
        private ISummoningView _summoningView;
        private ICharactersTurnOrderView _charactersTurnOrderView;
        private ITurnModel _turnModel;
        private IBattleModel _battleModel;
        private ICellUIModel[,] _gridView;
        private IInputController _inputController;
        private ICameraTargetController _cameraTargetController;
        private IStatusEffectsView _statusEffectsView;
        private IPopupView _popupView;
        private BattleSlot? _selectedSlot;
        private string _objectId;
        private CollectableItem _herb;
        private CollectableItem _tool;
        private CollectableItem _gem;
        private float _moveSpeed;
        private float _dragSpeed;
        private float _dragDecceleration;
        private Vector3 _dragVelocity;

        // Readonly Variables
        private readonly int _battleCellLayer;

        // Properties
        public string Name => "Planning Phase";

        public PlanningPhase(ICoroutineHandler coroutineStarter,
                             IGameMasterController gameMaster,
                             IQuickCastView quickCastView,
                             ISummoningView summoningView,
                             ICharactersTurnOrderView charactersTurnOrderView,
                             ITurnModel turnModel,
                             IBattleModel battleModel,
                             ICellUIModel[,] gridView,
                             ICountdownView countdownView,
                             IEnergyView energyView,
                             IPlayerBadgeView playerBadgeView,
                             IInputController inputController,
                             ICameraTargetController cameraTargetController,
                             IStatusEffectsView statusEffectsView,
                             IPopupView popupView,
                             float moveSpeed,
                             float dragSpeed,
                             float dragDecceleration)
        //float cameraSpeed)
        {
            _coroutineStarter = coroutineStarter;
            _isPlanningPhaseFinished = null;
            _sendFinishPlanningPhase = null;
            _gameMaster = gameMaster;
            _summoningView = summoningView;
            _quickCastView = quickCastView;
            _charactersTurnOrderView = charactersTurnOrderView;
            _turnModel = turnModel;
            _battleModel = battleModel;
            _gridView = gridView;
            _countdownView = countdownView;
            _playerBadgeView = playerBadgeView;
            _energyView = energyView;
            //_cameraSpeed = cameraSpeed;
            _inputController = inputController;
            _cameraTargetController = cameraTargetController;
            _statusEffectsView = statusEffectsView;
            _popupView = popupView;
            _moveSpeed = moveSpeed;
            _dragSpeed = dragSpeed;
            _dragDecceleration = dragDecceleration;

            _battleCellLayer = LayerMask.GetMask(BattleCellLayerName);
        }

        public IEnumerator Enter(IStateMachine stateMachine)
        {
            //IList<IStatusEffect> statusEffects = _battleModel.GetStatusEffects();
            //_statusEffectsView.UpdateView(statusEffects);

            _statusEffectsView.Hide();

            _selectedSlot = null;

            // Show Character Turn Order View
            IList<IWitchModel> witches = new List<IWitchModel>();
            foreach (var item in _battleModel.GridUI.WitchesViews)
            {
                witches.Add(item.Model);
                yield return null;
            }

            IList<ISpiritModel> spirits = new List<ISpiritModel>();
            foreach (var item in _battleModel.GridUI.SpiritsViews)
            {
                spirits.Add(item.Model);
                yield return null;
            }

            IEnumerator showCharacterTurnOrderView = _charactersTurnOrderView.Show(_turnModel.PlanningOrder, _turnModel.MaxActionsAllowed, witches, spirits);
            yield return _coroutineStarter.Invoke(showCharacterTurnOrderView);

            _startTime = Time.realtimeSinceStartup;

            _quickCastView.Show(_battleModel, OnClickFly, OnClickSummon, OnClickFlee, OnCastSpell, OpenInventory);

            //Show countdown turn
            _countdownView.Show();
        }

        private void OnClickCell(ICellUIModel cellUIModel)
        {
            _statusEffectsView.Hide();

            if (_selectedSlot != null)
            {
                ICellUIModel currentCellUI = _gridView[_selectedSlot.Value.Row, _selectedSlot.Value.Col];
                currentCellUI.SetIsSelected(false);
            }

            if (!string.IsNullOrWhiteSpace(cellUIModel.CellModel.ObjectId) && cellUIModel.CellModel.ObjectId.Equals(PlayerDataManager.playerData.instance))
            {
                _quickCastView.OnClickYourself();
            }
            else
            {
                ICharacterController character = _battleModel.GridUI.GetCharacter(cellUIModel.CellModel.ObjectId);
                string objectType = character == null ? string.Empty : character.Model.ObjectType;

                _quickCastView.OnClickEnemy(objectType);
            }

            cellUIModel.SetIsSelected(true);

            _selectedSlot = new BattleSlot()
            {
                Row = cellUIModel.CellModel.X,
                Col = cellUIModel.CellModel.Y
            };

            _objectId = cellUIModel.CellModel.ObjectId;

            if (cellUIModel.CellModel.IsEmpty())
            {
                _quickCastView.OpenActionsMenu();
            }
            else
            {
                _quickCastView.OpenSpellMenu();

                ICharacterController character = _battleModel.GridUI.GetCharacter(_objectId);
                if (character != default && character.Model != default)
                {
                    IList<IStatusEffect> statusEffects = character.Model.StatusEffects;
                    _statusEffectsView.Show(statusEffects);
                }
            }
        }

        public IEnumerator Update(IStateMachine stateMachine)
        {
            float elapsedTime = Time.realtimeSinceStartup - _startTime;
            int time = Mathf.FloorToInt(_turnModel.PlanningMaxTime - elapsedTime);

            _countdownView.UpdateTime(time);

            if (elapsedTime > _turnModel.PlanningMaxTime || !HasActionsAvailable())
            {
                // copy actions to array
                int numActions = _turnModel.RequestedActions.Count;
                IActionRequestModel[] actions = new IActionRequestModel[numActions];
                _turnModel.RequestedActions.CopyTo(actions, 0);

                _sendFinishPlanningPhase = _gameMaster.SendFinishPlanningPhase(_battleModel.Id, actions, OnPlanningPhaseFinished);
                _coroutineStarter.Invoke(_sendFinishPlanningPhase);

                yield return stateMachine.ChangeState<ActionResolutionPhase>();
            }
            else
            {
                // Touch occured
                if (_inputController.Touch.HasValue) // check input
                {
                    _dragVelocity = Vector3.zero;

                    Ray touchRay = _inputController.Touch.Value;
                    if (Physics.Raycast(touchRay, out RaycastHit hitInfo, MaxRaycastDistance, _battleCellLayer))
                    {
                        ICellUIModel cellUIModel = hitInfo.transform.GetComponent<ICellUIModel>();
                        if (cellUIModel != null)
                        {
                            OnClickCell(cellUIModel);
                            IEnumerator moveTo = _cameraTargetController.MoveTo(cellUIModel.Transform.position, _moveSpeed);
                            _coroutineStarter.Invoke(moveTo);
                        }
                    }
                }
                else if (_inputController.DragVelocity.HasValue)
                {
                    _dragVelocity = new Vector3
                    {
                        x = -_inputController.DragVelocity.Value.x,
                        y = 0,
                        z = -_inputController.DragVelocity.Value.y
                    };
                    _cameraTargetController.Move(_dragVelocity * Time.deltaTime * _dragSpeed);
                }
                else if (_dragVelocity != Vector3.zero)
                {
                    // Decay velocity
                    _dragVelocity = Vector3.MoveTowards(_dragVelocity, Vector3.zero, _dragDecceleration * Time.deltaTime);
                    _cameraTargetController.Move(_dragVelocity * Time.deltaTime * _dragSpeed);
                }
            }
        }

        public IEnumerator Exit(IStateMachine stateMachine)
        {
            // update cooldowns
            _battleModel.UpdateCooldowns();
            _battleModel.GridUI.UpdateStatusEffects();

            _statusEffectsView.Hide();
            //_battleModel.UpdateStatusEffects();

            // Update status effects view
            //IList<IStatusEffect> statusEffects = _battleModel.GetStatusEffects();
            //_statusEffectsView.UpdateView(statusEffects);

            _quickCastView.Hide();
            _countdownView.Hide();
            _charactersTurnOrderView.Hide();

            _coroutineStarter.Invoke(_popupView.Hide());

            if (UIInventory.isOpen)
            {
                UIInventory.Instance.Close();
                OnCloseInventory();
            }

            _isPlanningPhaseFinished = null;
            _sendFinishPlanningPhase = null;

            if (_selectedSlot != null)
            {
                ICellUIModel currentCellUI = _gridView[_selectedSlot.Value.Row, _selectedSlot.Value.Col];
                currentCellUI.SetIsSelected(false);
            }

            // wait for planning phase finished event
            yield return new WaitUntil(() => _isPlanningPhaseFinished.GetValueOrDefault());
        }

        private bool CanFlee()
        {
            return _turnModel.RequestedActions.Count <= 0;
        }

        private bool HasActionsAvailable()
        {
            // Check for flee
            bool requestedFlee = false;
            if (_turnModel.RequestedActions.Count == 1)
            {
                IActionRequestModel actionRequestModel = _turnModel.RequestedActions[0];
                requestedFlee = actionRequestModel.Type == ActionRequestType.Flee;
            }
            return !requestedFlee && _turnModel.MaxActionsAllowed > _turnModel.RequestedActions.Count;
        }

        #region Socket Events

        private void OnPlanningPhaseFinished(PlanningPhaseFinishedEventArgs args)
        {
            _coroutineStarter.Invoke(OnPlanningPhaseFinishedCoroutine(args));
        }

        private IEnumerator OnPlanningPhaseFinishedCoroutine(PlanningPhaseFinishedEventArgs args)
        {
            foreach (BattleActor actor in args.Actors)
            {
                string characterId = actor.Id;
                foreach (BattleAction battleAction in actor.Actions)
                {
                    List<IActionResponseModel> actionsResults = new List<IActionResponseModel>(battleAction.Results);
                    if (!_turnModel.ResponseActions.ContainsKey(characterId))
                    {
                        _turnModel.ResponseActions.Add(characterId, actionsResults);
                    }
                    else
                    {
                        IList<IActionResponseModel> responseActions = _turnModel.ResponseActions[characterId];
                        responseActions.AddRange(actionsResults);
                        _turnModel.ResponseActions[characterId] = responseActions;
                    }

                    yield return null;
                }
            }
            _isPlanningPhaseFinished = true;
        }

        #endregion

        #region Actions

        private void OnClickFlee()
        {
            if (CanFlee())
            {
                string message = LocalizeLookUp.GetText("battle_flee_request");
                _coroutineStarter.Invoke(_popupView.Show(message, OnConfirmFlee, OnCancelFlee));
            }
        }

        private void OnConfirmFlee()
        {
            _turnModel.RequestedActions.Add(new FleeActionRequestModel());
            _charactersTurnOrderView.UpdateActionsPoints(_turnModel.RequestedActions.Count);
        }

        private void OnCancelFlee()
        {
            _coroutineStarter.Invoke(_popupView.Hide());
        }

        private void OnCastSpell(string spell)
        {
            if (HasActionsAvailable() && _selectedSlot.HasValue && Spellcasting.CanCast(spell))
            {
                CastActionRequestModel cast = new CastActionRequestModel()
                {
                    SpellId = spell,
                    TargetId = _objectId,
                    Ingredients = GetIngredients()
                };

                _turnModel.RequestedActions.Add(cast);
                _charactersTurnOrderView.UpdateActionsPoints(_turnModel.RequestedActions.Count);
                OnFinishAddCastRequest(spell);
            }
        }

        private void OnClickFly()
        {
            if (HasActionsAvailable() && _selectedSlot.HasValue)
            {
                _turnModel.RequestedActions.Add(new MoveActionRequestModel() { Position = _selectedSlot.Value });
                _charactersTurnOrderView.UpdateActionsPoints(_turnModel.RequestedActions.Count);

                Debug.LogFormat("Add Action Move to Slot X:{0} Y:{1}", _selectedSlot.Value.Row, _selectedSlot.Value.Col);
            }
        }

        private void OnClickSummon()
        {
            if (HasActionsAvailable() && _selectedSlot.HasValue)
            {
                _summoningView.Open(OnSummon);
            }
        }

        private void OnSummon(string spiritID)
        {
            _turnModel.RequestedActions.Add(new SummonActionRequestModel() { Position = _selectedSlot.Value, SpiritId = spiritID });
            _charactersTurnOrderView.UpdateActionsPoints(_turnModel.RequestedActions.Count);

            Debug.LogFormat("Summon Spirit {0} to Slot X:{1} Y:{2}", spiritID, _selectedSlot.Value.Row, _selectedSlot.Value.Col);
        }

        private void OpenInventory(string spell)
        {
            _inputController.SetActive(false);

            SpellData selectedSpell = DownloadedAssets.GetSpell(spell);

            SetIngredients(selectedSpell.ingredients.ToArray());

            _countdownView.Hide();
            _charactersTurnOrderView.SetActive(false);

            _quickCastView.SetOnMenuIngredient(true, spell);

            UIInventory.Instance.Show(OnClickInventoryItem, OnCloseInventory, false, true);

            //lock if necessary
            UIInventory.Instance.LockIngredients(selectedSpell.ingredients.ToArray(), 0);

            //set the ivnentory with the current ingredients
            List<CollectableItem> selected = new List<CollectableItem>()
            {
                _herb, _tool, _gem
            };
            UIInventory.Instance.SetSelected(selected);

            _playerBadgeView.Hide();
            _energyView.Hide();
        }

        private void OnClickInventoryItem(UIInventoryWheelItem item)
        {
            //just resets if clicking on an empty inventory item
            if (item.inventoryItemId == null)
            {
                //resets the picker
                item.SetIngredientPicker(0);
                if (item.type == "herb")
                    _herb = new CollectableItem();
                else if (item.type == "tool")
                    _tool = new CollectableItem();
                else if (item.type == "gem")
                    _gem = new CollectableItem();
                return;
            }

            //List<string> required = m_SelectedSpell.ingredients == null ? new List<string>() : new List<string>(m_SelectedSpell.ingredients);
            IngredientData itemData = item.itemData;
            int maxAmount = Mathf.Min(5, PlayerDataManager.playerData.GetIngredient(item.inventoryItemId));

            if (itemData.Type == IngredientType.herb)
            {
                if (string.IsNullOrEmpty(_herb.id))
                {
                    //select the ingredient
                    _herb = new CollectableItem
                    {
                        id = item.inventoryItemId,
                        count = Mathf.Min(1, maxAmount)
                    };
                    item.SetIngredientPicker(_herb.count);
                }
                else if (_herb.id != item.inventoryItemId)
                {
                    //unselect the previous ingredient
                    _herb = new CollectableItem();
                    UIInventory.Instance.herbsWheel.ResetPicker();
                }
                else
                {
                    //increment the selected ingredient
                    _herb.count = (int)Mathf.Repeat(_herb.count + 1, maxAmount + 1);
                    _herb.count = Mathf.Clamp(_herb.count, 1, maxAmount);
                    item.SetIngredientPicker(_herb.count);
                }
            }
            else if (itemData.Type == IngredientType.tool)
            {
                if (string.IsNullOrEmpty(_tool.id))
                {
                    _tool = new CollectableItem
                    {
                        id = item.inventoryItemId,
                        count = Mathf.Min(1, maxAmount)
                    };
                    item.SetIngredientPicker(_tool.count);
                }
                else if (_tool.id != item.inventoryItemId)
                {
                    _tool = new CollectableItem();
                    UIInventory.Instance.toolsWheel.ResetPicker();
                }
                else
                {
                    _tool.count = (int)Mathf.Repeat(_tool.count + 1, maxAmount + 1);
                    _tool.count = Mathf.Clamp(_tool.count, 1, maxAmount);
                    item.SetIngredientPicker(_tool.count);
                }
            }
            else if (itemData.Type == IngredientType.gem)
            {
                if (string.IsNullOrEmpty(_gem.id))
                {
                    _gem = new CollectableItem
                    {
                        id = item.inventoryItemId,
                        count = Mathf.Min(1, maxAmount)
                    };
                    item.SetIngredientPicker(_gem.count);
                }
                else if (_gem.id != item.inventoryItemId)
                {
                    _gem = new CollectableItem();
                    UIInventory.Instance.gemsWheel.ResetPicker();
                }
                else
                {
                    _gem.count = (int)Mathf.Repeat(_gem.count + 1, maxAmount + 1);
                    _gem.count = Mathf.Clamp(_gem.count, 1, maxAmount);
                    item.SetIngredientPicker(_gem.count);
                }
            }
        }

        private void SetIngredients(string[] ingredients)
        {
            _herb.id = _tool.id = _gem.id = null;
            _herb.count = _tool.count = _gem.count = 0;

            if (ingredients != null)
            {
                for (int i = 0; i < ingredients.Length; i++)
                {
                    IngredientType ingrType = DownloadedAssets.GetCollectable(ingredients[i]).Type;

                    if (ingrType == IngredientType.herb)
                    {
                        _herb = new CollectableItem
                        {
                            id = ingredients[i],
                            count = 1
                        };
                    }
                    else if (ingrType == IngredientType.tool)
                    {
                        _tool = new CollectableItem
                        {
                            id = ingredients[i],
                            count = 1
                        };
                    }
                    else if (ingrType == IngredientType.gem)
                    {
                        _gem = new CollectableItem
                        {
                            id = ingredients[i],
                            count = 1
                        };
                    }
                }
            }
        }

        private void OnCloseInventory()
        {
            if (Time.time - _startTime < _turnModel.PlanningMaxTime)
            {
                _charactersTurnOrderView.SetActive(true);
                _quickCastView.SetOnMenuIngredient(false, string.Empty);
                _countdownView.Show();
            }
            _inputController.SetActive(true);
            _playerBadgeView.Show();
            _energyView.Show();
            _quickCastView.SetOnMenuIngredient(false, "");

            _gem = new CollectableItem();
            _herb = new CollectableItem();
            _tool = new CollectableItem();
        }

        private InventoryItemModel[] GetIngredients()
        {
            int size = Mathf.Clamp(_tool.count, 0, 1) + Mathf.Clamp(_herb.count, 0, 1) + Mathf.Clamp(_gem.count, 0, 1);
            InventoryItemModel[] ingredients = new InventoryItemModel[size];
            int index = 0;

            if (_tool.count > 0)
            {
                ingredients[index] = new InventoryItemModel()
                {
                    Id = _tool.id,
                    Count = _tool.count
                };
                index++;
            }

            if (_gem.count > 0)
            {
                ingredients[index] = new InventoryItemModel()
                {
                    Id = _gem.id,
                    Count = _gem.count
                };
                index++;
            }

            if (_herb.count > 0)
            {
                ingredients[index] = new InventoryItemModel()
                {
                    Id = _herb.id,
                    Count = _herb.count
                };
            }

            return ingredients;
        }

        private void OnFinishAddCastRequest(string spell)
        {
            if (UIInventory.isOpen)
            {
                PlayerDataManager.playerData.SubIngredient(_gem.id, _gem.count);
                PlayerDataManager.playerData.SubIngredient(_tool.id, _tool.count);
                PlayerDataManager.playerData.SubIngredient(_herb.id, _herb.count);

                UIInventory.Instance.Close();
                OnCloseInventory();
            }
            else
            {
                SpellData selectedSpell = DownloadedAssets.GetSpell(spell);

                foreach (string ingredient in selectedSpell.ingredients)
                {
                    PlayerDataManager.playerData.SubIngredient(ingredient, 1);
                }
            }
        }
        #endregion
    }
}