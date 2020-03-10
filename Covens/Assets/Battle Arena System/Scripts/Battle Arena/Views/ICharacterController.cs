using Raincrow.BattleArena.Model;
using System.Collections;
using UnityEngine;

namespace Raincrow.BattleArena.Controllers
{
    public interface ICharacterController<T, U> : ICharacterController where T : ICharacterModel where U : ICharacterUIModel
    {
        T Model { get; }
        U UIModel { get; }
        void Init(T characterModel, U characterViewModel);
    }

    public interface ICharacterController
    {
        Transform Transform { get; }
        void FaceCamera(Quaternion cameraRotation, Vector3 cameraForward);
        void UpdateView(int baseEnergy, int energy);
        IEnumerator AddDamage(int damage);        
    }
}
