using UnityEngine;

namespace RedMountMedia
{
    public class SwingController : MonoBehaviour
    {
        #region Serialized Variables
        [SerializeField]
        [Tooltip("This iD is being used for door Trigger. Has to be the same id as the DoorTrigger Script which is attached to the parent of this object.")]
        private int triggerID = default;

        [Space, Header("Door Gamobjects")]
        [SerializeField]
        [Tooltip("Place the Door Gameobject which will be visible in the scene")]
        private GameObject doorClose = default;

        [SerializeField]
        [Tooltip("Place the opened Door Gameobject")]
        private GameObject doorOpen = default;

        [Space, Header("Door Type")]
        [SerializeField]
        [Tooltip("Current door type. Will it rotate? Move or Both?")]
        DoorType currType = DoorType.Rotating;
        public enum DoorType { Moving, Rotating, Both };

        [Space, Header("Door Variables")]
        [SerializeField]
        [Tooltip("Door movement speed")]
        private float moveSpeed = 3f;

        [SerializeField]
        [Tooltip("Door rotation speed")]
        private float rotationSpeed = 90f;

        [SerializeField]
        [Tooltip("Is the door Opened? Or Closed?")]
        private bool _isOpened = false;
        public bool _isOnHold = false;
        #endregion

        #region Private Variables
        private GameObject _doorCloseObj = default;
        #endregion

        #region Unity Callbacks

        #region Events
        void OnEnable() => DoorTrigger.OnDoorTrigger += OnDoorTriggerEventReceived;

        void OnDisable() => DoorTrigger.OnDoorTrigger -= OnDoorTriggerEventReceived;

        void OnDestroy() => DoorTrigger.OnDoorTrigger -= OnDoorTriggerEventReceived;
        #endregion

        void Start() => IntializeDoor();

        void Update() => MoveAndRotateDoor();
        #endregion

        #region My Functions
        /// <summary>
        /// Intialises the door. Spawns a closed door and disables it. It also disables the dopen door debug;
        /// </summary>
        void IntializeDoor()
        {
            _doorCloseObj = Instantiate(doorClose, doorClose.transform.position, doorClose.transform.rotation, transform);
            _doorCloseObj.SetActive(false);
            doorOpen.SetActive(false);
        }

        /// <summary>
        /// This just flips the bool depending on the player interaction;
        /// </summary>
        public void InteractDoorRaycast() => _isOpened = !_isOpened;

        /// <summary>
        /// The door moves and rotates according to the enum chosen;
        /// </summary>
        void MoveAndRotateDoor()
        {
            if (!_isOnHold)
            {
                var target = _isOpened ? doorOpen : _doorCloseObj;

                if (currType == DoorType.Both)
                {
                    doorClose.transform.SetPositionAndRotation(Vector3.MoveTowards(doorClose.transform.position, target.transform.position, moveSpeed * Time.deltaTime),
                        Quaternion.RotateTowards(doorClose.transform.rotation, target.transform.rotation, rotationSpeed * Time.deltaTime));
                }

                if (currType == DoorType.Rotating)
                    doorClose.transform.rotation = Quaternion.RotateTowards(doorClose.transform.rotation, target.transform.rotation, rotationSpeed * Time.deltaTime);
                if (currType == DoorType.Moving)
                    doorClose.transform.position = Vector3.MoveTowards(doorClose.transform.position, target.transform.position, moveSpeed * Time.deltaTime);
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Subbed to event from DoorTrigger Script. This event just checks which door to control according to what trigger ID the player passed from;
        /// </summary>
        /// <param name="id"> ID from the trigger script must be the same as this script; </param>
        void OnDoorTriggerEventReceived(int id)
        {
            if (id == this.triggerID)
                _isOpened = !_isOpened;
        }
        #endregion
    }
}