using UdonSharp;
using UnityEngine;
using VRC.Examples.Helpers;
using VRC.SDKBase;

namespace VRC.Examples.JackInTheBox
{
    /// <summary>
    /// This script can be used to breathe life into scene objects by adding simple
    /// procedural eye motions. The eyes will alternate looking between players and
    /// objects of interest in the scene.
    /// </summary>
    public class SimpleEyeLook : UdonSharpBehaviour
    {
        public Transform leftEye;
        public Transform rightEye;
        public Transform lookTarget;
        public Transform[] objectsOfInterest;

        [Range(0, 180)] public float fieldOfView = 90f; // degrees
        [Range(0, 1000)] public float viewRange = 10f; // meters
        public float minLookInterval = 0.1f;    // seconds
        public float maxLookInterval = 5;   // seconds

        private float _nextAllowedLookTime;

        private VRCPlayerApi _currentTargetPlayer;
        private Transform _currentTargetTransform;
        private VRCPlayerApi[] _allPlayers = new VRCPlayerApi[50];



        /// <summary>
        /// 
        /// </summary>
        void Update()
        {
            // alternate between random look targets within a certain rough interval
            if (Time.time > _nextAllowedLookTime)
            {
                PickNewLookTarget();
                _nextAllowedLookTime = (Time.time + Random.Range(minLookInterval, maxLookInterval));
            }

            UpdateLookPosition();

            // refresh eyes
            VisionHelpers.LookAt(lookTarget.position, leftEye, 300, false);
            VisionHelpers.LookAt(lookTarget.position, rightEye, 300, false);
        }


        /// <summary>
        /// Sets position of the 'lookTarget' transform to the currently targeted
        /// player or object
        /// </summary>
        void UpdateLookPosition()
        {
            if (Utilities.IsValid(_currentTargetPlayer))
            {
                lookTarget.position = _currentTargetPlayer.GetBonePosition(HumanBodyBones.Head);
            }
            else if (Utilities.IsValid(_currentTargetTransform))
            {
                lookTarget.position = _currentTargetTransform.position;
            }
        }


        /// <summary>
        /// Chooses a new random target player or object to look at
        /// </summary>
        void PickNewLookTarget()
        {

            VRCPlayerApi.GetPlayers(_allPlayers);

            float playerPriority = (_allPlayers.Length / (_allPlayers.Length + objectsOfInterest.Length));

            if (Random.value < playerPriority)
            {
                _currentTargetPlayer = GetRandomVisiblePlayer();
                if (_currentTargetPlayer == null)
                {
                    _currentTargetTransform = GetRandomVisibleObject();
                }
                else
                {
                    _currentTargetTransform = null;
                }
            }
            else
            {
                _currentTargetTransform = GetRandomVisibleObject();
                if (_currentTargetTransform == null)
                {
                    _currentTargetPlayer = GetRandomVisiblePlayer();
                }
                else
                {
                    _currentTargetPlayer = null;
                }
            }

            if ((_currentTargetPlayer == null) && (_currentTargetTransform == null))
            {
                SetRandomWorldPosition();
            }

        }

        /// <summary>
        /// Chooses a random player among all those currently in our line of sight
        /// </summary>
        public VRCPlayerApi GetRandomVisiblePlayer()
        {

            VRCPlayerApi[] visiblePlayers = new VRCPlayerApi[_allPlayers.Length];
            int visibleCount = 0;

            foreach (var player in _allPlayers)
            {
                if (player == null)
                    continue;
                if (!player.IsValid())
                    continue;
                if (player == _currentTargetPlayer)
                    continue;
                if (!VisionHelpers.CanSee(transform.position + transform.forward, transform.forward,
                    player.GetBonePosition(HumanBodyBones.Head), fieldOfView, viewRange))
                    continue;

                // Success! Add this player to visible players.
                visiblePlayers[visibleCount] = player;
                visibleCount++;
            }

            if (visibleCount == 0)
            {
                return null;
            }

            return visiblePlayers[Random.Range(0, visibleCount)];

        }

        /// <summary>
        /// Wraps the GetRandomVisiblePlayer function so it can be called from the Graph via SendCustomEvent, and the result retrieved from GetRandomVisiblePlayerResult
        /// </summary>
        public VRCPlayerApi GetRandomVisiblePlayerResult;

        public void GetRandomVisiblePlayerGraph()
        {
            GetRandomVisiblePlayerResult = GetRandomVisiblePlayer();
        }


        /// <summary>
        /// Chooses a random object among all the 'objectsOfInterest' currently in our line of sight
        /// </summary>
        Transform GetRandomVisibleObject()
        {
            if (objectsOfInterest.Length < 1)
                return null;

            int visibleCount = 0;

            Transform[] visibleObjects = new Transform[objectsOfInterest.Length];

            foreach (var obj in objectsOfInterest)
            {
                if (obj == null)
                    continue;
                if (!obj.gameObject.activeInHierarchy)
                    continue;
                if (obj == _currentTargetTransform)
                    continue;
                if (!VisionHelpers.CanSee(transform.position + transform.forward, transform.forward, obj.position, fieldOfView,
                    viewRange))
                    continue;

                // Success! Add this object to visible objects.
                visibleObjects[visibleCount] = obj;
                visibleCount++;
            }

            if (visibleCount == 0)
            {
                return null;
            }

            return visibleObjects[Random.Range(0, visibleCount)];
        }

        /// <summary>
        /// Just looks at a random position.
        /// </summary>
        void SetRandomWorldPosition()
        {
            Vector3 localLookPos = transform.InverseTransformPoint(transform.position + (Random.onUnitSphere * 10f));
            localLookPos.z = Mathf.Abs(localLookPos.z) + 5;
            localLookPos.y *= 0.5f;
            lookTarget.position = transform.TransformPoint(localLookPos);
        }

    }
}