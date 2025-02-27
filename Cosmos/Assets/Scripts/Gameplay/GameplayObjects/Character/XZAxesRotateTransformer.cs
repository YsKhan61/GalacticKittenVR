
using Cosmos.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Cosmos.Gameplay.GameplayObjects.Character
{
    /// <summary>
    /// Used to rotate a transform on pivot's X Z axis based on RotateInput;
    /// </summary>
    public class XZAxesRotateTransformer : MonoBehaviour, IControlHandle
    {
        [SerializeField, Tooltip("The data based on which the visual will rotate")] 
        private Vector2DataSO _rotateInput;

        [SerializeField, Tooltip("This will be the pivot relative to calculate orientations of handle")]
        private Transform _pivotTransform;
        public Transform PivotTransform => _pivotTransform;

        [SerializeField, Tooltip("This will be the visual that will be oriented")]
        private Transform _visualTransform;

        [SerializeField, Tooltip("The sensitivity of throttle")]
        private float _sensitivity = 1;

        [SerializeField, Tooltip("The max angle between visual transform's axis of orientation and pivot transform's Y axis")] 
        private int _angleConstraint = 10;
        public int AngleConstraint => _angleConstraint;

        private Quaternion _initialVisualLocalRotation;

        private float _relativeAngleX;
        private float _relativeAngleZ;

        private void Awake()
        {
            _initialVisualLocalRotation = Quaternion.Inverse(_pivotTransform.rotation) * _visualTransform.rotation;

            _relativeAngleX = 0;
            _relativeAngleZ = 0;
        }

        private void Update()
        {
            if (_rotateInput.value != Vector2.zero)
            {
                ManualControl();
            }
            else
            {
                AutoReset();
            }
        }

        private void ManualControl()
        {
            Debug.Log($"_rotateInputValue: {_rotateInput.value}");

            _relativeAngleX = Mathf.Lerp(-_angleConstraint, _angleConstraint, Mathf.InverseLerp(1, -1, _rotateInput.value.x));
            _relativeAngleZ = Mathf.Lerp(-_angleConstraint, _angleConstraint, Mathf.InverseLerp(1, -1, _rotateInput.value.y));

            Debug.Log($"_relativeAngleX: {_relativeAngleX}, _relativeAngleZ: {_relativeAngleZ}");

            Quaternion inputRotationInPivotSpace = Quaternion.Euler(_relativeAngleX, 0f, _relativeAngleZ);

            _visualTransform.localRotation = Quaternion.Slerp(
                               _visualTransform.localRotation,
                                inputRotationInPivotSpace * _initialVisualLocalRotation,
                                5f* Time.deltaTime);
        }

        private void AutoReset()
        {
            _visualTransform.rotation = Quaternion.Slerp(
                    _visualTransform.rotation,
                    _pivotTransform.rotation * _initialVisualLocalRotation,
                    _sensitivity * Time.deltaTime);

            _relativeAngleX = _visualTransform.localEulerAngles.x;
            if (_relativeAngleX > 180)
                _relativeAngleX -= 360;


            _relativeAngleZ = _visualTransform.localEulerAngles.z;
            if (_relativeAngleZ > 180)
                _relativeAngleZ -= 360;
        }
    }

}
