using UdonSharp;
using UnityEngine;

namespace VRC.Examples.JackInTheBox
{
    /// <summary>
    /// This is a very simple spring physics solution that can be used to attach a
    /// rigidbody object to a position in world space with a spring-like motion.
    /// </summary>
    public class Spring : UdonSharpBehaviour
    {
        public Transform anchor; // The point in world space where our target 'wants to be'

        public new Rigidbody rigidbody; // The target object which will be moved by spring physics

        [Range(0, 100)]
        public float
            Force; // Together with Rigidbody Mass and Drag parameters (!) determines the strength and motion of the spring


        /// <summary>
        /// Moves our target object towards the anchor point every frame. If we overshoot, the
        /// Rigidbody will start going backwards to the anchor point, but will be slowed by drag
        /// which results in the illusion of spring physics
        /// </summary>
        void FixedUpdate()
        {
            Vector3 dir = (anchor.position - rigidbody.position).normalized;
            float dist = Vector3.Distance(rigidbody.position, anchor.position);
            Vector3 currentForce = (dir * dist * Force);

            rigidbody.AddForce(currentForce, ForceMode.VelocityChange);
        }
    }
}