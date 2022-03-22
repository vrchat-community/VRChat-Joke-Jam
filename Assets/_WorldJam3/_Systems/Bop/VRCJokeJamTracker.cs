using UnityEngine;
using VRC.SDKBase;

namespace VRC.SDK.Internal
{
    public class VRCJokeJamTracker : VRCCustomAction
    {
        public override void Execute(int value)
        {
            Debug.Log($"Bop received");
        }
    }
}