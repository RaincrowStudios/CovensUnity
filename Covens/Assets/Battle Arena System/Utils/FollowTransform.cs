using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.Utils
{
    public class FollowTransform : MonoBehaviour
    {
        // Serialized Fields
        [SerializeField] private Transform _target;

        // Private Variables
        private Vector3 _targetDelta;

        protected virtual void OnEnable()
        {
            _targetDelta = _target.position - transform.position;
        }

        protected virtual void Update()
        {
            Vector3 currentDelta = _target.position - transform.position;
            if (_targetDelta != currentDelta)
            {
                transform.position = _target.position - _targetDelta;
            }
        }
    }
}