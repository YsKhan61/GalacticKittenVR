using Cosmos.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Cosmos.Gameplay.GameplayObjects.Character
{
    /// <summary>
    /// The movement caused by the throttle movement input value
    /// </summary>
    public class ThrottleMovement : MonoBehaviour
    {
        [SerializeField, Tooltip("The value that will be used to calculate movement")] 
        private FloatDataSO _throttleMovementInput;

        [SerializeField, Tooltip("The rigidbody to move")]
        private Rigidbody _rigidbody = null;

        [SerializeField, Tooltip("The max force to input to the rigidbody")]
        private float _maxForceToInput = 1000f;

        [SerializeField, Tooltip("The max force to allow to the rigidbody")]
        [FormerlySerializedAs("_maxForceAllowed")]
        private float _maxLinearVelocityAllowed = 50f;

        [SerializeField, Tooltip("Damping factor at 0 force")]
        private float _damping = 5f;

        private void Start()
        {
            _rigidbody.maxLinearVelocity = _maxLinearVelocityAllowed;
        }

        private void FixedUpdate()
        {
            if (_throttleMovementInput.value == 0)
            {
                _rigidbody.velocity = Vector3.Lerp(_rigidbody.velocity, Vector3.zero, _damping * Time.fixedDeltaTime);
            }
            else
            {
                // Move the rigidbody forward or backward with a force proportional to the throttle value
                _rigidbody.AddForce(transform.forward * _throttleMovementInput.value * _maxForceToInput * Time.fixedDeltaTime);
            }
        }
    }
}

