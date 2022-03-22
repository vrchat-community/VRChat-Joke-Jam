using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace VRC.Examples.JackInTheBox
{
    /// <summary>
    /// This script simulates a crazy Jack in the Box which triggers when you interact with it.
    /// It is designed to work hand in hand with the 'Spring' script to make the head pop out.
    /// Once out, the head can be dragged around, stretching the spring.
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class JackInTheBox : UdonSharpBehaviour
    {
        public Transform body;
        public Transform neck;
        public Transform neckPosOpen;
        public Transform neckPosClosed;

        public Spring spring;
        public VRC_Pickup pickup;
        public new Rigidbody rigidbody;
        public new ParticleSystem particleSystem;
        public AudioSource audioSource;
        public Animator animator;

        private float _restSpringLength;

        [UdonSynced(UdonSyncMode.Smooth)] private Vector3 _grabbedPos;

        [UdonSynced(UdonSyncMode.Linear)] private bool _isGrabbed;

        [UdonSynced, FieldChangeCallback(nameof(IsOpen))]
        private bool _isOpen;


        public bool IsOpen
        {
            set
            {
                _isOpen = value;
                if (value == true)
                    Open();
                else
                    Close();
            }
            get => _isOpen;
        }


        /// <summary>
        /// 
        /// </summary>
        void Start()
        {

            // Calculate spring rest length which is used for updating spring in FixedUpdate
            _restSpringLength = Mathf.Abs(neckPosOpen.position.y - transform.root.position.y);

            IsOpen = false;

            if (Utilities.IsValid(animator))
                animator.StopPlayback();
        }

        /// <summary>
        /// 
        /// </summary>
        private void FixedUpdate()
        {
            // When box is closed, always smoothly slide back down into it
            if (!IsOpen)
                rigidbody.transform.position = Vector3.Lerp(rigidbody.transform.position, neckPosClosed.position,
                    Time.deltaTime * 1);

            // Calculate how much the spring is extended
            float currentSpringLength = Vector3.Distance(body.transform.position, transform.root.position);
            float springFactor = 1f - (currentSpringLength / _restSpringLength);

            // Calculate spring scale depending on stretch
            float thickness = Mathf.Max(-0.7f, (springFactor * 0.2f));
            float stretch = -springFactor;
            spring.transform.localScale = Vector3.one + new Vector3(thickness, thickness, stretch);

            // Also stretch out the head along with spring
            neck.transform.localScale = _isGrabbed ? Vector3.one : Vector3.one + (Vector3.forward * -springFactor);

        }

        /// <summary>
        /// 
        /// </summary>
        void LateUpdate()
        {
            // Handle interaction sync
            if (Networking.IsOwner(pickup.gameObject))
            {
                if (!Networking.IsOwner(gameObject))
                    Networking.SetOwner(Networking.LocalPlayer, gameObject);

                if (pickup.IsHeld)
                {
                    if (!_isGrabbed)
                    {
                        _isGrabbed = true;
                        RequestSerialization();
                    }
                }
                else
                {
                    if (_isGrabbed)
                    {
                        _isGrabbed = false;
                        RequestSerialization();
                    }
                }

                if (_isGrabbed)
                {
                    _grabbedPos = body.transform.position;
                    RequestSerialization();
                }

            }
            else if (_isGrabbed)
            {
                body.transform.position = Vector3.Lerp(body.transform.position, _grabbedPos, Time.deltaTime * 20f);
            }

            // Disable spring while head is grabbed
            if (_isGrabbed)
            {
                spring.enabled = false;
            }
            else
            {
                spring.enabled = true;
            }

            // Always fixate spring towards head, and face head perpendicular to direction of box root
            spring.transform.LookAt(neck.transform, -transform.root.forward);
            Vector3 headLookPos = transform.root.position + ((neck.transform.position - transform.root.position) * 2);
            neck.transform.LookAt(headLookPos, -transform.root.forward);
        }

        /// <summary>
        /// Toggles open/close state of the box upon player interaction
        /// </summary>
        public override void Interact()
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            IsOpen = !IsOpen;
            RequestSerialization();
        }

        /// <summary>
        /// On opening the box, enables the spring physics, makes the head grabbable
        /// and triggers particle, sound and animation effects on the box
        /// </summary>
        private void Open()
        {

            spring.enabled = true;
            rigidbody.isKinematic = false;
            pickup.pickupable = true;

            if (Utilities.IsValid(animator))
                animator.SetTrigger("Open");
            if (Utilities.IsValid(particleSystem))
            {
                particleSystem.Stop();
                particleSystem.Play();
            }

            if (Utilities.IsValid(audioSource) && Utilities.IsValid(audioSource.clip))
            {
                audioSource.Play();
            }
        }

        /// <summary>
        /// On closing the box, disables the spring physics, makes the head
        /// non-grabbable and triggers a close animation on the box
        /// </summary>
        void Close()
        {
            spring.enabled = false;
            rigidbody.isKinematic = true;
            pickup.pickupable = false;

            if (Utilities.IsValid(animator))
                animator.SetTrigger("Close");
        }

    }
}