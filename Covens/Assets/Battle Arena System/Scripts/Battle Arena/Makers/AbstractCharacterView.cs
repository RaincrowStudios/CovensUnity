using UnityEngine;

namespace Raincrow.BattleArena.View
{
    public abstract class AbstractCharacterView : MonoBehaviour
    {
        public abstract void FaceCamera(Quaternion cameraRotation, Vector3 cameraForward);
    }
}
