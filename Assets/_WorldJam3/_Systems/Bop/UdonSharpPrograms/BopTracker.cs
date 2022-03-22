using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using UnityEngine.UI;
using VRC.Examples.Helpers;

namespace VRC.Examples.Bops
{
    /// <summary>
    /// Adds BopReceivers to any existing and joining players and updates
    /// the total bop count of the world.
    /// </summary>
    public class BopTracker : UdonSharpBehaviour
    {

        public Text totalField;
        public VRCCustomAction bopTrackerAction;
        public LocalPool colliderPool;


        public int TotalBops
        {
            set
            {
                totalBops = value;
                totalField.text = totalBops.ToString();
            }
            get => totalBops;
        }


        [UdonSynced, FieldChangeCallback(nameof(TotalBops))]
        private int totalBops;


        /// <summary>
        /// Adds a BopReceiver to all players present when we join the world.
        /// </summary>
        public void Start()
        {
            VRCPlayerApi[] players = new VRCPlayerApi[VRCPlayerApi.GetPlayerCount()];
            VRCPlayerApi.GetPlayers(players);

            foreach (var player in players)
            {
                if (Utilities.IsValid(player) && !player.isLocal)
                {
                    AddBopReceiverTo(player);
                }
            }
        }

        /// <summary>
        /// Adds a BopReceiver to any players joining after we ourselves joined.
        /// </summary>
        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            AddBopReceiverTo(player);
        }

        /// <summary>
        /// Spawns a BopReceiver object from the object pool and connects it to 'player'
        /// NOTE: the BopReceiver is later despawned in 'BopReceiver.OnPlayerLeft'
        /// </summary>
        void AddBopReceiverTo(VRCPlayerApi player)
        {
            if (!player.IsValid() || player.isLocal)
            {
                return;
            }

            if (colliderPool == null)
            {
                colliderPool = GetComponentInChildren<LocalPool>();
            }

            GameObject go = colliderPool.Spawn();
            BopReceiver hpr = go.GetComponent<BopReceiver>();
            hpr.bopTracker = this;
            hpr.player = player;
        }

        /// <summary>
        /// Event target. Increments total bop count and executes CustomEvent to update world.
        /// </summary>
        public void _AddBop()
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            TotalBops++;
            RequestSerialization();
            if (Utilities.IsValid(bopTrackerAction))
            {
                bopTrackerAction.Execute(0);
            }
        }

    }
}