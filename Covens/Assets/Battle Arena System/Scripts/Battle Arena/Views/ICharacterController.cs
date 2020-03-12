using Raincrow.BattleArena.Model;
using UnityEngine;

namespace Raincrow.BattleArena.Controllers
{
    public interface ICharacterController<T, U> : ICharacterController where T : ICharacterModel where U : ICharacterUIModel
    {
        new T Model { get; }
        new U UIModel { get; }
        void Init(T characterModel, U characterViewModel);
    }

    public interface ICharacterController
    {
        ICharacterModel Model { get; }
        ICharacterUIModel UIModel { get; }
        Transform Transform { get; }
        GameObject GameObject { get; }
        void FaceCamera(Quaternion cameraRotation, Vector3 cameraForward);
        void UpdateView(int baseEnergy, int energy);
        void AddDamage(int damage);        
        void UpdateEnergy(int energy);        
    }
}