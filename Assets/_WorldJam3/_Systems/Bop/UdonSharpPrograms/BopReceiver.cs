using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace VRC.Examples.Bops
{
    /// <summary>
    /// Makes a collider track a player's head position every frame.
    /// Sends a CustomEvent when a 'bop' is initiated on the head.
    /// Returns the head collider to the object pool when player leaves.
    /// NOTE: This script will also work in the scene on a non-player object.
    /// </summary>
    [System.Serializable]
    public class BopReceiver : UdonSharpBehaviour
    {
        public BopTracker bopTracker;
        public VRCPlayerApi player;

        private bool _isPlayerAttached;


        /// <summary>
        /// Makes this object track position of the target player's head every frame.
        /// </summary>
        public void LateUpdate()
        {
            if (Utilities.IsValid(player))
            {
                _isPlayerAttached = true;
                VRCPlayerApi.TrackingData head = player.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
                transform.position = head.position;
            }
            else if(_isPlayerAttached)
            {
                // 'Player' has left: return this object to the ObjectPool.
                gameObject.SetActive(false);
                _isPlayerAttached = false;
            }
        }

        /// <summary>
        /// When someone bops us, sends a CustomEvent to track number of
        /// bops in the world.
        /// </summary>
        public void ReceiveBop()
        {

            // Try to find the tracker if it's null
            if (!Utilities.IsValid(bopTracker))
            {
                GameObject go = GameObject.Find("BopTracker");
                bopTracker = go.GetComponent<BopTracker>();
                if (!Utilities.IsValid(bopTracker))
                    return;
            }

            if (Utilities.IsValid(bopTracker))
            {
                bopTracker.SendCustomEvent("_AddBop");
            }
        }

    }
}