using Raincrow.BattleArena.Model;
using UnityEngine;

namespace Raincrow.BattleArena.Views
{
    public interface ICharacterView<T, U> where T : ICharacterModel where U : ICharacterUIModel
    {
        T Model { get; }
        U UIModel { get; }
        Transform GetTransform();
        void Init(T characterModel, U characterViewModel);
        void FaceCamera(Quaternion cameraRotation, Vector3 cameraForward);
        void UpdateView();
    }
}
