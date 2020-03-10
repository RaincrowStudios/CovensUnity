using Raincrow.BattleArena.Controllers;
using Raincrow.BattleArena.Model;
using Raincrow.BattleArena.Views;
using Raincrow.StateMachines;
using Raincrow.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.BattleArena.Phases
{
    public class EndPhase : IState
    {
        // Variables
        private ICoroutineHandler _coroutineHandler;
        private IBattleResultModel _battleResult;
        private ICelebratoryView _celebratoryView;
        private IDebriefView _debriefView;
        private ICameraTargetController _cameraTargetController;
        private ISmoothCameraFollow _smoothCameraFollow;
        private IBattleModel _battleModel;
        private DebriefAnimationModel _debriefAnimationValues;
        private IRewardsBatttleView _rewardsBatttleView;


        private IDictionary<string, ICharacterController<IWitchModel, IWitchUIModel>> _witches =
            new Dictionary<string, ICharacterController<IWitchModel, IWitchUIModel>>();
        private IDictionary<string, ICharacterController<ISpiritModel, ISpiritUIModel>> _spirits =
            new Dictionary<string, ICharacterController<ISpiritModel, ISpiritUIModel>>();

        // Properties
        public string Name => "End Phase";

        public EndPhase(IBattleResultModel battleResultModel,
                        ICelebratoryView celebratoryView,
                        IDebriefView debriefView,
                        ICoroutineHandler coroutineHandler,
                        ICameraTargetController cameraTargetController,
                        ISmoothCameraFollow smoothCameraFollow,
                        IBattleModel battleModel,
                        DebriefAnimationModel debriefAnimationValues,
                        IRewardsBatttleView rewardsBatttleView
            )
        {
            _battleResult = battleResultModel;
            _celebratoryView = celebratoryView;
            _debriefView = debriefView;
            _coroutineHandler = coroutineHandler;
            _cameraTargetController = cameraTargetController;
            _smoothCameraFollow = smoothCameraFollow;
            _battleModel = battleModel;
            _debriefAnimationValues = debriefAnimationValues;
            _rewardsBatttleView = rewardsBatttleView;
        }

        public IEnumerator Enter(IStateMachine stateMachine)
        {
            _witches.Clear();
            foreach (var view in _battleModel.GridUI.WitchesViews)
            {
                _witches.Add(view.Model.Id, view);
                yield return null;
            }

            _spirits.Clear();
            foreach (var view in _battleModel.GridUI.SpiritsViews)
            {
                _spirits.Add(view.Model.Id, view);
                yield return null;
            }

            _celebratoryView.Show(_battleResult.Type == BattleResultType.PlayerWins);

            yield return new WaitForSeconds(5f);

            _celebratoryView.Hide();

            yield return _coroutineHandler.Invoke(DebriefPhase());

            yield return _rewardsBatttleView.Show(
                _battleResult.Type == BattleResultType.PlayerWins ? "You Won!" : "You Lost!",
                "Battle is over",
                _battleResult.Reward
            ).WaitEndScreen();

            yield return null;
        }

        public IEnumerator Update(IStateMachine stateMachine)
        {
            yield return null;
        }

        public IEnumerator Exit(IStateMachine stateMachine)
        {
            yield return null;
        }

        private IEnumerator DebriefPhase()
        {
            for (int i = 0; i < _battleResult.Ranking.Length; i++)
            {
                ICharacterUIModel characterUI = default;
                ICharacterModel character = default;
                Vector3 position = Vector3.zero;

                if (_witches.TryGetValue(_battleResult.Ranking[i], out ICharacterController<IWitchModel, IWitchUIModel> witchView))
                {
                    character = witchView.Model;
                    characterUI = witchView.UIModel;
                    position = witchView.Transform.position;
                }
                else if (_spirits.TryGetValue(_battleResult.Ranking[i], out ICharacterController<ISpiritModel, ISpiritUIModel> spiritView))
                {
                    character = spiritView.Model;
                    characterUI = spiritView.UIModel;
                    position = spiritView.Transform.position;
                }

                if (characterUI != default && character != default)
                {
                    position.y = _debriefAnimationValues.TargetY;
                    //int idTargetPosition = _cameraTargetController.SetTargetPosition(position, _debriefAnimationValues.CameraSpeed);
                    int idCameraDistance = _smoothCameraFollow.SetCameraDistance(_debriefAnimationValues.CameraDistance, _debriefAnimationValues.TimeAnimation);
                    int idCameraHeight = _smoothCameraFollow.SetCameraHeight(_debriefAnimationValues.CameraHeight, _debriefAnimationValues.TimeAnimation);

                    while (_cameraTargetController.IsMoving())
                    {
                        yield return new WaitForEndOfFrame();
                    }

                    if (character.ObjectType == ObjectType.Witch)
                    {
                        _debriefView.Show(i + 1, 500, "Default", "Default", "Default", "Default");
                    }
                    else
                    {
                        _debriefView.Show(i + 1, 200);
                    }

                    yield return new WaitForSeconds(10f);

                    _debriefView.Hide();

                    idCameraDistance = _smoothCameraFollow.ResetCameraDistance(_debriefAnimationValues.TimeAnimation);
                    idCameraHeight = _smoothCameraFollow.ResetCameraHeight(_debriefAnimationValues.TimeAnimation);

                    while (LeanTween.isTweening(idCameraDistance) || LeanTween.isTweening(idCameraHeight))
                    {
                        yield return new WaitForEndOfFrame();
                    }

                }

            }

            yield return null;
        }
    }
}