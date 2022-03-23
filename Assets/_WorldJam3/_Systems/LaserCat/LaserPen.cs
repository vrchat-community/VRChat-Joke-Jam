using UdonSharp;
using VRC.SDK3.Components;
using VRC.SDKBase;

namespace VRC.Examples.LaserCat
{
    /// <summary>
    /// This pickup activates the laser the use trigger is being pressed. It uses
    /// 'FieldChangeCallback' to sync the trigger, and gives the owner of the
    /// laser pen ownership and control the 'LaserCat'
    /// </summary>
    public class LaserPen : UdonSharpBehaviour
    {
        public VRC_Pickup pickup;
        public LaserDot laserDot;
        public LaserCat laserCat;
        public int respawnTimeInSeconds = 60;
        public VRCObjectSync _sync;

        [UdonSynced, FieldChangeCallback(nameof(SyncedToggle))]
        private bool _syncedToggle;

        private float _respawnTime;

        public bool SyncedToggle
        {
            set
            {
                _syncedToggle = value;
                laserDot.gameObject.SetActive(value);
            }
            get => _syncedToggle;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnPickupUseDown()
        {
            SyncedToggle = true;
        }


        /// <summary>
        /// 
        /// </summary>
        public override void OnPickupUseUp()
        {
            SyncedToggle = false;
        }


        /// <summary>
        /// 
        /// </summary>
        public override void OnPickup()
        {
            VRCPlayerApi owner = Networking.GetOwner(pickup.gameObject);
            Networking.SetOwner(owner, laserDot.gameObject);
            Networking.SetOwner(owner, laserCat.gameObject);
        }


        /// <summary>
        /// Turns off Laser effect and runs timer logic to respawn pen after it hasn't been picked up for a while
        /// </summary>
        public override void OnDrop()
        {
            VRCPlayerApi owner = Networking.GetOwner(pickup.gameObject);
            if (owner.isLocal)
            {
                _respawnTime = UnityEngine.Time.time + respawnTimeInSeconds;
                SendCustomEventDelayedSeconds(nameof(_RespawnWhenAbandoned), respawnTimeInSeconds + 1); // add 1 second to avoid rounding errors in timing
            }
            SyncedToggle = false;
        }

        public void _RespawnWhenAbandoned()
        {
            // It may have been picked up since it was abandoned
            if (pickup.IsHeld || UnityEngine.Time.time < _respawnTime)
            {
                return;
            }

            if (Utilities.IsValid(_sync))
            {
                _sync.Respawn();
            }
        }

    }
    
}