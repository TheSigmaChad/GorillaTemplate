using UnityEngine;

namespace Normal.GorillaTemplate.Keyboard {
    /// <summary>
    /// A base class for a pressable button.
    /// Override <see cref="HandlePress"/> to implement specific behavior on button press.
    /// </summary>
    public abstract class ButtonBase : MonoBehaviour {
        /// <summary>
        /// The child GameObject that holds the visual representation of the button.
        /// </summary>
        [SerializeField]
        private GameObject _visuals;

        /// <summary>
        /// How far the button moves after being pressed.
        /// </summary>
        [SerializeField]
        private float _pushOffset = 0.1f;

        /// <summary>
        /// A factor that controls how the button returns to its initial position after being pressed.
        /// </summary>
        [SerializeField]
        private float _lerpFactor = 20.0f;

        /// <summary>
        /// The original position of the button that it will return to after being pressed.
        /// </summary>
        private Vector3 _originalPosition;

        /// <summary>
        /// To resolve ex multiple fingers resting on the same keyboard button.
        /// </summary>
        private int _triggerCount;

        protected virtual void Awake() {
            _originalPosition = _visuals.transform.localPosition;
        }

        /// <summary>
        /// The button should have a trigger collider on its root GameObject,
        /// which won't move when the visuals GameObject (which is a child) is animated.
        ///
        /// The collider should be on a layer that interacts with non-trigger colliders on
        /// the local player's hands. The hand colliders need to be non-triggers, but
        /// they can have a layer specific to them to ensure they only collide with
        /// buttons and no other layers. All of this is managed at the prefab
        /// level, and not in the code.
        /// </summary>
        private void OnTriggerEnter(Collider other) {
            // Ignore presses from other fingers
            if (_triggerCount == 0) {
                OnPressed();
            }

            _triggerCount++;
        }

        private void OnTriggerExit(Collider other) {
            _triggerCount--;
        }

        private void OnPressed() {
            HandlePress();
            ApplyOnPressedFX();
        }

        /// <summary>
        /// Override this method to react to a button press.
        /// </summary>
        protected abstract void HandlePress();

        protected virtual void ApplyOnPressedFX() {
            // Push button back
            _visuals.transform.localPosition = _originalPosition + Vector3.forward * _pushOffset;
        }

        protected virtual void Update() {
            UpdateFX();
        }

        protected virtual void UpdateFX() {
            // Animate towards original position
            _visuals.transform.localPosition = Vector3.Lerp(_visuals.transform.localPosition, _originalPosition, Time.deltaTime * _lerpFactor);
        }
    }
}
