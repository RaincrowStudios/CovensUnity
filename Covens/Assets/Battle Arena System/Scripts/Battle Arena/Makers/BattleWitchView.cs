using UnityEngine;

namespace Raincrow.BattleArena.View
{
    public class BattleWitchView : AbstractCharacterView
    {
        [SerializeField] private Transform _avatarRoot;

        public override void FaceCamera(Quaternion cameraRotation, Vector3 cameraForward)
        {
            Vector3 worldPosition = transform.position + cameraRotation * Vector3.forward;
            _avatarRoot.transform.LookAt(worldPosition, cameraForward);
        }
    }
}