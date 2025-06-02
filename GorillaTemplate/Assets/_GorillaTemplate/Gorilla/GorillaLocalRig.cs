using System.Collections;
using GorillaLocomotion;
using Normal.Utility;
using Normal.XR;
using UnityEngine;

namespace Normal.GorillaTemplate {
    /// <summary>
    /// Represents the local player.
    /// It's a singleton so that it can be accessed conveniently from other systems.
    /// </summary>
    public class GorillaLocalRig : MonoBehaviourSingleton<GorillaLocalRig> {
        /// <summary>
        /// The local player (assigned in the scene).
        /// </summary>
        [SerializeField]
        private Player _player;

        /// <summary>
        /// The component that controls player turning.
        /// </summary>
        [SerializeField]
        private XRTurnLocomotion _turnLocomotion;

        /// <summary>
        /// Disables player locomotion for the specified amount of seconds.
        /// </summary>
        public void Stun(float durationSeconds) {
            StartCoroutine(DoStun(durationSeconds));
        }

        private IEnumerator DoStun(float durationSeconds) {
            _player.disableMovement = true;

            yield return new WaitForSeconds(durationSeconds);

            _player.disableMovement = false;
        }

        public XRTurnLocomotion.TurnMode turnMode {
            get => _turnLocomotion.turnMode;
            set => _turnLocomotion.turnMode = value;
        }

        public float snapTurnIncrement {
            get => _turnLocomotion.snapTurnIncrement;
            set => _turnLocomotion.snapTurnIncrement = value;
        }

        public float smoothTurnSpeed {
            get => _turnLocomotion.smoothTurnSpeed;
            set => _turnLocomotion.smoothTurnSpeed = value;
        }
    }
}
