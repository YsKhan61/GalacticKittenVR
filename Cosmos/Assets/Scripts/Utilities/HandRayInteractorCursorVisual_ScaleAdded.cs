using Oculus.Interaction;
using Oculus.Interaction.Input;
using UnityEngine;


namespace Cosmos.Utilities
{
    /// <summary>
    /// This script also scales the cursor based on the distance from the ray origin to the collision point.
    /// </summary>
    public class HandRayInteractorCursorVisual_ScaleAdded : MonoBehaviour
    {
        [SerializeField, Interface(typeof(IHand))]
        private UnityEngine.Object _hand;
        private IHand Hand;

        [SerializeField]
        private RayInteractor _rayInteractor;

        [SerializeField]
        private GameObject _cursor;

        [SerializeField]
        private Renderer _renderer;

        [SerializeField]
        private Color _outlineColor = Color.black;

        [SerializeField]
        private float _offsetAlongNormal = 0.005f;

        [SerializeField, Tooltip("The amount by which this cursor will be scaled when it is too close to an pointable element")]
        private float _scaleMultiplier = 0.5f;

        #region Properties

        public Color OutlineColor
        {
            get
            {
                return _outlineColor;
            }
            set
            {
                _outlineColor = value;
            }
        }

        public float OffsetAlongNormal
        {
            get
            {
                return _offsetAlongNormal;
            }
            set
            {
                _offsetAlongNormal = value;
            }
        }

        #endregion

        private int _shaderRadialGradientScale = Shader.PropertyToID("_RadialGradientScale");
        private int _shaderRadialGradientIntensity = Shader.PropertyToID("_RadialGradientIntensity");
        private int _shaderRadialGradientBackgroundOpacity = Shader.PropertyToID("_RadialGradientBackgroundOpacity");
        private int _shaderOutlineColor = Shader.PropertyToID("_OutlineColor");

        [SerializeField]
        private GameObject _selectObject;

        protected bool _started = false;

        protected virtual void Start()
        {
            this.BeginStart(ref _started);
            Hand = _hand as IHand;
            this.AssertField(Hand, nameof(Hand));
            this.AssertField(_rayInteractor, nameof(_rayInteractor));
            this.AssertField(_renderer, nameof(_renderer));
            this.AssertField(_cursor, nameof(_cursor));
            this.AssertField(_selectObject, nameof(_selectObject));
            this.EndStart(ref _started);
        }

        protected virtual void OnEnable()
        {
            if (_started)
            {
                _rayInteractor.WhenPostprocessed += UpdateVisual;
                _rayInteractor.WhenStateChanged += UpdateVisualState;
                UpdateVisual();
            }
        }

        protected virtual void OnDisable()
        {
            if (_started)
            {
                _rayInteractor.WhenPostprocessed -= UpdateVisual;
                _rayInteractor.WhenStateChanged -= UpdateVisualState;
            }
        }

        private void UpdateVisual()
        {
            if (_rayInteractor.State == InteractorState.Disabled)
            {
                _cursor.SetActive(false);
                return;
            }

            if (_rayInteractor.CollisionInfo == null)
            {
                _cursor.SetActive(false);
                return;
            }

            if (!_cursor.activeSelf)
            {
                _cursor.SetActive(true);
            }

            Vector3 collisionNormal = _rayInteractor.CollisionInfo.Value.Normal;
            this.transform.position = _rayInteractor.End + collisionNormal * _offsetAlongNormal;
            this.transform.rotation = Quaternion.LookRotation(_rayInteractor.CollisionInfo.Value.Normal, Vector3.up);
            this.transform.localScale = Vector3.one * Mathf.Lerp(0f, 1f, _rayInteractor.CollisionInfo.Value.Distance * _scaleMultiplier);

            if (_rayInteractor.State == InteractorState.Select)
            {
                _selectObject.SetActive(true);
                _renderer.material.SetFloat(_shaderRadialGradientScale, 0.25f);
                _renderer.material.SetFloat(_shaderRadialGradientIntensity, 1f);
                _renderer.material.SetFloat(_shaderRadialGradientBackgroundOpacity, 1f);
                _renderer.material.SetColor(_shaderOutlineColor, _outlineColor);
            }
            else
            {
                _selectObject.SetActive(false);
                var mappedPinchStrength = Hand.GetFingerPinchStrength(HandFinger.Index);
                var radialScale = 1f - mappedPinchStrength;
                radialScale = Mathf.Max(radialScale, .11f);
                _renderer.material.SetFloat(_shaderRadialGradientScale, radialScale);
                _renderer.material.SetFloat(_shaderRadialGradientIntensity, mappedPinchStrength);
                _renderer.material.SetFloat(_shaderRadialGradientBackgroundOpacity, Mathf.Lerp(0.3f, 0.7f, mappedPinchStrength));
                _renderer.material.SetColor(_shaderOutlineColor, _outlineColor);
            }
        }

        private void UpdateVisualState(InteractorStateChangeArgs args) => UpdateVisual();

        #region Inject

        public void InjectAllHandRayInteractorCursorVisual(IHand hand,
            RayInteractor rayInteractor,
            GameObject cursor,
            Renderer renderer)
        {
            InjectHand(hand);
            InjectRayInteractor(rayInteractor);
            InjectCursor(cursor);
            InjectRenderer(renderer);
        }

        public void InjectHand(IHand hand)
        {
            _hand = hand as UnityEngine.Object;
            Hand = hand;
        }

        public void InjectRayInteractor(RayInteractor rayInteractor)
        {
            _rayInteractor = rayInteractor;
        }

        public void InjectCursor(GameObject cursor)
        {
            _cursor = cursor;
        }

        public void InjectRenderer(Renderer renderer)
        {
            _renderer = renderer;
        }

        #endregion
    }
}

