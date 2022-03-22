using UdonSharp;
using UnityEngine;

namespace VRC.Examples.LaserCat
{
    /// <summary>
    /// This script raycasts to a point in front of the pen, and moves itself
    /// there, adapting to the face of the hit surface.
    /// </summary>
    public class LaserDot : UdonSharpBehaviour
    {
        public Transform penForward;

        /// <summary>
        /// Rather than syncing rotation, we snap rotation to the hit normal every frame
        /// (since rotation sync is a little too sluggish and would rotate slowly across
        /// corners).
        /// </summary>
        private void LateUpdate()
        {
            Vector3 dir, pos;

            dir = penForward.forward;
            pos = penForward.position;

            RaycastHit hit;
            if (Physics.Raycast(pos, dir, out hit))
            {
                transform.position = hit.point + (hit.normal * 0.01f);
                transform.LookAt(hit.point + hit.normal);
            }
        }

    }
}