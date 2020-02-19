using Raincrow.BattleArena.Model;
using UnityEngine;

namespace Raincrow.BattleArena.View
{
    public abstract class AbstractCharacterView<T> : MonoBehaviour where T : ICharacterModel
    {
        public virtual T Model { get; protected set; }

        public virtual void Init(T characterModel)
        {
            Model = characterModel;
        }

        public abstract void FaceCamera(Quaternion cameraRotation, Vector3 cameraForward);

        public abstract void ChangeCharacterTexture(Texture texture);        
    }
}
