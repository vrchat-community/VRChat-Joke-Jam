using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;

namespace VRC.Examples.Bops
{
    /// <summary>
    /// Sends Bops to Receivers who match a targetPrefix.
    /// Plays effects whenever a bop is triggered.
    /// Regulates min velocity and max bop rate.
    /// </summary>
    public class BopSender : UdonSharpBehaviour
    {
        public string targetPrefix;
        public BopTracker bopTracker;
        public ParticleSystem particles;
        public AudioSource audioSource;
        public VRCPickup pickup;
        public float minVelocity = 0.01f;
        public float cooldown = 0.25f;

        private float _nextAllowedBopTime;
        private Vector3 _prevPos;
        private Vector3 _velocity;

        private void Start()
        {
            // If this BopSender has a pickup and the player is in VR, let them pick it up any way they want instead of the default "Grip" orientation
            if (Utilities.IsValid(pickup) && Utilities.IsValid(Networking.LocalPlayer) && Networking.LocalPlayer.IsUserInVR())
            {
                pickup.orientation = VRC_Pickup.PickupOrientation.Any;
            }
        }

        /// <summary>
        /// Tracks velocity for enforcing minimum velocity in OnTriggerEnter.
        /// </summary>
        private void LateUpdate()
        {

            _velocity = (_prevPos - transform.position);
            _prevPos = transform.position;

        }


        /// <summary>
        /// Upon collision with a properly tagged target object, if this object
        /// is moving fast enough and bop cooldown has ended, executes 'ReceiveBop'
        /// on the 'BopReceiver' (if any) of the other object.
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            if (Time.time < _nextAllowedBopTime)
                return;

            if (_velocity.magnitude < minVelocity)
                return;

            if (Networking.GetOwner(this.gameObject) != Networking.LocalPlayer)
                return;

            if (!Utilities.IsValid(other))
                return;

            if (!other.transform.name.StartsWith(targetPrefix))
                return;

            BopReceiver hpr = other.GetComponent<BopReceiver>();
            if (Utilities.IsValid(hpr))
            {
                SendCustomNetworkEvent(NetworkEventTarget.All, nameof(PlayBopFX));
                
                // Set Bop Tracker on receiver if it's invalid and the sender has a good reference
                if (Utilities.IsValid(bopTracker) && !Utilities.IsValid(hpr.bopTracker))
                {
                    hpr.bopTracker = bopTracker;
                }
                hpr.ReceiveBop();
                _nextAllowedBopTime = (Time.time + cooldown);
            }
        }

        /// <summary>
        /// Play Particle and Audio FX if they've been set
        /// </summary>
        public void PlayBopFX()
        {
            if (Utilities.IsValid(particles))
            {
                particles.Play();
            }

            if (Utilities.IsValid(audioSource) && Utilities.IsValid(audioSource.clip))
            {
                audioSource.Play();
            }
        }


    }
}