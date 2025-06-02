using System;
using Normal.Realtime;
using UnityEngine;

namespace Normal.GorillaTemplate {
    /// <summary>
    /// Syncs finger poses across the network.
    /// </summary>
    public class FingerPoseSync : RealtimeComponent<FingerPoseSyncModel> {
        public event Action<FingerType, float> onFingerStateChanged;

        [SerializeField]
        private FingerPoseTracker _localPoseTracker;

        public void Awake() {
            if (_localPoseTracker != null) {
                _localPoseTracker.onFingerStateChanged += OnLocalFingerStateChanged;
            }
        }

        private void OnDestroy() {
            if (_localPoseTracker != null) {
                _localPoseTracker.onFingerStateChanged -= OnLocalFingerStateChanged;
                _localPoseTracker = null;
            }
        }

        protected override void OnRealtimeModelReplaced(FingerPoseSyncModel previousModel, FingerPoseSyncModel currentModel) {
            if (previousModel != null) {
                previousModel.indexClosedDidChange -= OnIndexClosedDidChange;
                previousModel.middleClosedDidChange -= OnMiddleClosedDidChange;
                previousModel.thumbClosedDidChange -= OnThumbClosedDidChange;
            }

            if (currentModel != null) {
                currentModel.indexClosedDidChange += OnIndexClosedDidChange;
                currentModel.middleClosedDidChange += OnMiddleClosedDidChange;
                currentModel.thumbClosedDidChange += OnThumbClosedDidChange;
            }
        }

        // Dispatch update for remote provider changes.
        private void OnIndexClosedDidChange(FingerPoseSyncModel fingerPoseSyncModel, float value) {
            onFingerStateChanged?.Invoke(FingerType.Index, value);
        }

        private void OnMiddleClosedDidChange(FingerPoseSyncModel fingerPoseSyncModel, float value) {
            onFingerStateChanged?.Invoke(FingerType.Middle, value);
        }

        private void OnThumbClosedDidChange(FingerPoseSyncModel fingerPoseSyncModel, float value) {
            onFingerStateChanged?.Invoke(FingerType.Thumb, value);
        }

        /// <summary>
        /// When the local provider dispatches an update, update the network values.
        /// </summary>
        private void OnLocalFingerStateChanged(FingerType fingerType, float value) {
            if (model == null || !isOwnedLocallyInHierarchy) {
                return;
            }

            switch (fingerType) {
                case FingerType.Index:
                    model.indexClosed = value;
                    break;
                case FingerType.Middle:
                    model.middleClosed = value;
                    break;
                case FingerType.Thumb:
                    model.thumbClosed = value;
                    break;
                default:
                    throw new ArgumentException($"Unexpected {nameof(FingerType)}: {fingerType}");
            }
        }
    }

    [RealtimeModel]
    public partial class FingerPoseSyncModel {
        [RealtimeProperty(1, true, true)]
        private float _indexClosed;

        [RealtimeProperty(2, true, true)]
        private float _middleClosed;

        [RealtimeProperty(3, true, true)]
        private float _thumbClosed;
    }
}
