using UnityEngine;

namespace Raincrow.BattleArena.View
{
    public abstract class AbstractCharacterView : MonoBehaviour
    {
        [SerializeField] protected SpriteRenderer m_EnergyRing;

        [SerializeField] protected MeshRenderer m_AvatarRender;
        
        public abstract void FaceCamera(Quaternion cameraRotation, Vector3 cameraForward);

        public virtual void UpdateEnergy(float time = 1f)
        {

        }
    }
}
