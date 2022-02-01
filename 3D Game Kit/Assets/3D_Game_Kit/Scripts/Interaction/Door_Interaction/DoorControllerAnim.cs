using UnityEngine;

namespace RedMountMedia
{
    public class DoorControllerAnim : MonoBehaviour
    {
        #region Public Variables
        [Space, Header("Door Enums")]
        public DoorState currDoorState = DoorState.Closed;
        public enum DoorState { Open, Closed }

        [Space, Header("Animations")]
        public string propOpenAnim;
        public string propCloseAnim;
        public Animator propAnim;

        [Space, Header("Audio")]
        public AudioSource interactionAud;
        public AudioClip openPropSfx;
        public AudioClip closePropSfx;
        #endregion

        #region Private Variables
        [Header("Animations")]
        private bool _isOpen = default;
        private bool _isClosed = default;
        private bool _isOpening = default;
        private bool _isClosing = default;
        #endregion

        #region Unity Callbacks
        void Start() => SetDoorState();
        #endregion

        #region My Function
        public void InteractDoor()
        {
            if (_isClosed && !_isOpening) // Door Opening Animation
            {
                propAnim.Play(propOpenAnim);
                _isOpening = true;

                if (openPropSfx != null)
                    interactionAud.PlayOneShot(openPropSfx);

                Debug.Log("Door Open");
            }

            if (_isOpen && !_isClosing) // Door Closing Animation
            {
                propAnim.Play(propCloseAnim);
                _isClosing = true;

                Debug.Log("Door Close");
            }
        }

        void SetDoorState()
        {
            if (currDoorState == DoorState.Closed)
            {
                _isOpen = true;
                InteractDoor();
                Debug.Log("Closed Door Set");
            }
            else
            {
                _isClosed = true;
                InteractDoor();
                Debug.Log("Opened Door Set");
            }
        }
        #endregion

        #region Events
        void ReceivedPropOpenAnimEvent()
        {
            _isOpen = true;
            _isOpening = false;
            _isClosed = false;
        }

        void ReceivedPropCloseAnimEvent()
        {
            _isClosed = true;
            _isClosing = false;
            _isOpen = false;

            if (closePropSfx != null)
                interactionAud.PlayOneShot(closePropSfx);
        }
        #endregion
    }
}