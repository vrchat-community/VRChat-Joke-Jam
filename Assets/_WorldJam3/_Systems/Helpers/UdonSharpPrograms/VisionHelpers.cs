using UnityEngine;

namespace VRC.Examples.Helpers
{
    /// <summary>
    /// Helper methods to facilitate scripting entities with vision
    /// </summary>
    public static class VisionHelpers
    {

        /// <summary>
        /// Call this every frame to rotate the 'source' transform to look at 'targetPosition' at
        /// a constant speed. If 'billboard' is true, rotation will only occur around the Y axis.
        /// </summary>
        public static void LookAt(Vector3 targetPosition, Transform source, float speed, bool billboard)
        {
            Vector3 targetDir = (targetPosition - source.position).normalized;
            if (billboard)
                targetDir.y = 0;
            source.rotation = Quaternion.RotateTowards(source.rotation, Quaternion.LookRotation(targetDir),
                (speed * Time.deltaTime));
        }


        /// <summary>
        /// Checks for line-of-sight (LOS) between 'from' and 'to'.
        /// </summary>
        public static bool CanSee(Vector3 from, Vector3 forward, Vector3 to, float fieldOfViewInDegrees,
            float viewRange)
        {

            // Direction to target
            Vector3 dir = (to - from).normalized;

            // Field of View

            // Remap from degrees into a 1-0 range to work with 'Vector3.Dot'
            float dotFOV = Mathf.Clamp(fieldOfViewInDegrees, 0, 180);
            dotFOV /= 180;
            dotFOV = (1 - dotFOV);

            if (Vector3.Dot(forward, dir) < dotFOV)
                return false;

            // View Range
            if (Vector3.Distance(from, to) > viewRange)
                return false;

            // Line of Sight
            // TODO: layer mask
            float dist = Vector3.Distance(from, to);
            return (Physics.Raycast(to, -dir, dist - 0.5f) == false);

        }
        
    }
}