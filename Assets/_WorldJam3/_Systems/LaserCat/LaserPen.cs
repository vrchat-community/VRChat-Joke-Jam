using UdonSharp;
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

        [UdonSynced, FieldChangeCallback(nameof(SyncedToggle))]
        private bool _syncedToggle;

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
            RequestSerialization();
        }


        /// <summary>
        /// 
        /// </summary>
        public override void OnPickupUseUp()
        {
            SyncedToggle = false;
            RequestSerialization();
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
        /// 
        /// </summary>
        public override void OnDrop()
        {
            SyncedToggle = false;
            RequestSerialization();

        }

    }
    
}