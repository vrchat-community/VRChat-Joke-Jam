using UdonSharp;
using UnityEngine;
using VRC.Examples.Helpers;
using VRC.SDKBase;

namespace VRC.Examples.LaserCat
{
    public enum State
    {
        Idle,
        Alert,
        Jump
    }

    /// <summary>
    /// A simple AI class with 3 states: Idle, Alert and Jump. Uses Rigidbody
    /// physics to hurl LaserCat at the laser dot
    /// </summary>
    public class LaserCat : UdonSharpBehaviour
    {
        public GameObject laser;
        public new Rigidbody rigidbody;

        // AI states
        public float stateRefreshInterval = 3f;
        public float jumpInterval = 0.5f;
        public float lookInterval = 1f;

        // Vision
        [Range(0, 180)] public float fieldOfView = 120; // degrees
        [Range(0, 1000)] public float viewRange = 20f; // meters

        // Jumping
        [Range(0, 10)] public float jumpForceMin = 2f;
        [Range(0, 10)] public float jumpForceMax = 10f;

        // State
        private State _currentState;
        private Vector3 _lookPos;

        // Timers
        private float _nextAllowedRefreshStateTime;
        private float _nextAllowedLookTime;
        private float _nextAllowedJumpTime;

        private const float JUMP_DISTANCE_MULTIPLIER = 0.05f;


        /// <summary>
        /// Updates LaserCat's AI if local player is its owner (otherwise we just rely on VRC_ObjectSync)
        /// </summary>
        void Update()
        {
            VRCPlayerApi owner = Networking.GetOwner(gameObject);
            if (owner != Networking.LocalPlayer)
                return;

            // Run the appropriate update method depending on LaserCat's current AI state
            switch (_currentState)
            {
                case State.Idle:
                    UpdateIdle();
                    break;
                case State.Alert:
                    UpdateAlert();
                    break;
                case State.Jump:
                    UpdateJump();
                    break;
            }

            // Trigger a state refresh at regular intervals, OR if LaserCat is idle and suddenly sees the laser
            if ((Time.time > _nextAllowedRefreshStateTime) || (_currentState == State.Idle && CanSeeLaser))
                RefreshState();
        }

        /// <summary>
        /// This is a simple state machine to switch between 3 AI states: Idle, Alert and Jump
        /// </summary>
        private void RefreshState()
        {
            switch (_currentState)
            {
                case State.Idle:
                    if (CanSeeLaser)
                        _currentState = State.Alert;
                    break;
                case State.Alert:
                    if (CanSeeLaser)
                        _currentState = State.Jump;
                    else
                        _currentState = State.Idle;
                    break;
                case State.Jump:
                    if (!CanSeeLaser)
                        _currentState = State.Alert;
                    else
                        _currentState = State.Idle;
                    break;
            }

            _nextAllowedRefreshStateTime = (Time.time + stateRefreshInterval);
        }

        /// <summary>
        /// In this state LaserCat hasn't seen the laser for a while. It just sits still
        /// and turns sleepily to a new random position every now and then
        /// </summary>
        private void UpdateIdle()
        {
            if (Time.time > _nextAllowedLookTime)
            {
                Vector2 target = Random.insideUnitCircle;
                _lookPos = transform.position + new Vector3(target.x, 0, target.y).normalized;
                _nextAllowedLookTime = (Time.time + lookInterval);
            }

            LookAtTarget(50, true);
        }

        /// <summary>
        /// In this state LaserCat is aware of the laser and is looking for it.
        /// If the laser is not visible, LaserCat scans the environment by turning quickly to
        /// a new random position every second. Upon seeing the laser, LaserCat locks onto it
        /// </summary>
        private void UpdateAlert()
        {
            if (CanSeeLaser)
            {
                _lookPos = laser.transform.position;
            }
            else
            {
                if (Time.time > _nextAllowedLookTime)
                {
                    Vector2 target = Random.insideUnitCircle;
                    _lookPos = transform.position + new Vector3(target.x, 0, target.y).normalized;
                    _nextAllowedLookTime = (Time.time + lookInterval);
                }
            }

            LookAtTarget(300, true);
        }

        /// <summary>
        /// In this state LaserCat has seen the laser and will jump frantically at it
        /// with short intervals. LaserCat will only jump if standing on the ground.
        /// NOTE: Due to network latency, LaserCat may appear to hover sluggishly for remote
        /// clients but the cat's current owner will always have responsive physics
        /// </summary>
        private void UpdateJump()
        {
            if (Time.time > _nextAllowedJumpTime)
            {
                if (CanSeeLaser && IsGrounded)
                {
                    float distance = Vector3.Distance(laser.transform.position, this.transform.position);
                    Vector3 JumpDir = (laser.transform.position - this.transform.position).normalized +
                                      (Vector3.up * (distance * JUMP_DISTANCE_MULTIPLIER));

                    rigidbody.AddForce(JumpDir * Random.Range(jumpForceMin, jumpForceMax), ForceMode.Impulse);
                }

                _nextAllowedJumpTime = (Time.time + jumpInterval);
            }

            _lookPos = laser.transform.position;
            LookAtTarget(300, false);
        }

        /// <summary>
        /// Returns true if a 0.5m raycast straight down from LaserCat's center hits an object.
        /// Used for determining if LaserCat should be able to start a jump
        /// </summary>
        private bool IsGrounded
        {
            get { return (Physics.Raycast(this.transform.position, Vector3.down, 0.5f)); }
        }

        /// <summary>
        /// Returns true if LaserCat has a clear line of sight to the laser dot within
        /// a field of view of approximately 120 degrees
        /// </summary>
        private bool CanSeeLaser
        {
            get
            {
                // Abort if laser is not turned on
                if (!laser.activeInHierarchy)
                {
                    return false;
                }

                return VisionHelpers.CanSee(this.transform.position, this.transform.forward, laser.transform.position,
                    fieldOfView, viewRange);
            }
        }

        /// <summary>
        /// Rotates LaserCat's transform at a chosen speed. If 'resetRoll' is true,
        /// physics will be disabled until LaserCat has had a chance to get up (in case
        /// it has tumbled over)
        /// </summary>
        private void LookAtTarget(float speed, bool resetRoll)
        {
            if (resetRoll)
            {
                rigidbody.freezeRotation = true;
            }
            else
            {
                rigidbody.freezeRotation = false;
            }

            VisionHelpers.LookAt(_lookPos, rigidbody.transform, speed, resetRoll);
        }

    }
}