using Raincrow.BattleArena.Model;
using UnityEngine;

namespace Raincrow.BattleArena.Views
{
    public abstract class AbstractCharacterView<T, U> : MonoBehaviour where T : ICharacterModel where U : ICharacterViewModel
    {
        public virtual T Model { get; protected set; }

        public virtual void Init(T characterModel, U characterViewModel, Camera battleCamera)
        {
            Model = characterModel;
        }

        public abstract void FaceCamera(Quaternion cameraRotation, Vector3 cameraForward);

        public abstract void UpdateView();
    }
}
