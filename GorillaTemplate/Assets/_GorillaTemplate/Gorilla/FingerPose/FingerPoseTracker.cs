using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Normal.GorillaTemplate {
    /// <summary>
    /// Uses InputSystem actions to get finger poses from the hardware device.
    /// </summary>
    public class FingerPoseTracker : MonoBehaviour {
        /// <summary>
        /// A float value action for the index finger (0 = open, 1 = closed).
        /// </summary>
        [SerializeField]
        private InputActionReference _indexFingerClosed;

        /// <summary>
        /// A float value action for the middle finger (0 = open, 1 = closed).
        /// </summary>
        [SerializeField]
        private InputActionReference _middleFingerClosed;

        /// <summary>
        /// A float value action for the thumb finger (0 = open, 1 = closed).
        /// </summary>
        [SerializeField]
        private InputActionReference _thumbFingerClosed;

        public event Action<FingerType, float> onFingerStateChanged;

        private void OnEnable() {
            _indexFingerClosed.action.Enable();
            _middleFingerClosed.action.Enable();
            _thumbFingerClosed.action.Enable();

            // These are value actions, so we want to listen to both performed and canceled
            _indexFingerClosed.action.performed += OnIndexFingerPerformed;
            _indexFingerClosed.action.canceled += OnIndexFingerCanceled;
            _middleFingerClosed.action.performed += OnMiddleFingerPerformed;
            _middleFingerClosed.action.canceled += OnMiddleFingerCanceled;
            _thumbFingerClosed.action.performed += OnThumbFingerPerformed;
            _thumbFingerClosed.action.canceled += OnThumbFingerCanceled;
        }

        private void OnIndexFingerPerformed(InputAction.CallbackContext ctx) {
            onFingerStateChanged?.Invoke(FingerType.Index, ctx.ReadValue<float>());
        }

        private void OnIndexFingerCanceled(InputAction.CallbackContext ctx) {
            onFingerStateChanged?.Invoke(FingerType.Index, ctx.ReadValue<float>());
        }

        private void OnMiddleFingerPerformed(InputAction.CallbackContext ctx) {
            onFingerStateChanged?.Invoke(FingerType.Middle, ctx.ReadValue<float>());
        }

        private void OnMiddleFingerCanceled(InputAction.CallbackContext ctx) {
            onFingerStateChanged?.Invoke(FingerType.Middle, ctx.ReadValue<float>());
        }

        private void OnThumbFingerPerformed(InputAction.CallbackContext ctx) {
            onFingerStateChanged?.Invoke(FingerType.Thumb, ctx.ReadValue<float>());
        }

        private void OnThumbFingerCanceled(InputAction.CallbackContext ctx) {
            onFingerStateChanged?.Invoke(FingerType.Thumb, ctx.ReadValue<float>());
        }
    }
}
