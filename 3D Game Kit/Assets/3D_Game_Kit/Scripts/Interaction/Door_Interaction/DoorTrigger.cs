using UnityEngine;

namespace RedMountMedia
{
    public class DoorTrigger : MonoBehaviour
    {
        #region Serialized Variables
        [SerializeField]
        [Tooltip("Use iD numbers above zero as zero is default number. This is used to distinguish which door it will send events to")]
        private int triggerID = default;

        [SerializeField]
        [Tooltip("Player tag to compare with?")]
        private string playerTag = "Player";

        [Space, Header("Keys")]
        [SerializeField]
        [Tooltip("Can this door use a key?")]
        private bool canUseKey = default;

        [SerializeField]
        [Tooltip("Remove key after use?")]
        private bool oneTimeOnly = default;

        [SerializeField]
        [Tooltip("The key this door accepts. Has to be the same exact KeyID as the keyItem Script or else it won't work")]
        private int keyTypeID = default;

        #region Events
        public delegate void SendEventsInt(int index);
        /// <summary>
        /// Event sent from DoorTrigger Script to DoorController Script;
        /// This just passes an int iD so that specific door with the same iD is opened;
        /// </summary>
        public static event SendEventsInt OnDoorTrigger;
        #endregion

        #endregion

        #region Unity Callbacks
        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(playerTag))
            {
                // Open Door
                if (canUseKey)
                    CompareKey(other.gameObject);
                else
                    OnDoorTrigger?.Invoke(triggerID);
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(playerTag))
            {
                // Close Door
                if (canUseKey)
                    CompareKey(other.gameObject);
                else
                    OnDoorTrigger?.Invoke(triggerID);
            }
        }
        #endregion

        #region My Functions
        public void InteractDoor(GameObject playerObj) => CompareKey(playerObj);

        public int GetKeyTypeInt() => keyTypeID;

        /// <summary>
        /// Checks players inventory if they have the right key ID;
        /// </summary>
        void CompareKey(GameObject playerObj)
        {
            Keyholder key = playerObj.GetComponent<Keyholder>();

            if (key.ContainKeyInt(GetKeyTypeInt()))
            {
                if (oneTimeOnly)
                {
                    OnDoorTrigger?.Invoke(triggerID);
                    key.RemoveKeyInt(GetKeyTypeInt());
                }
                else
                    OnDoorTrigger?.Invoke(triggerID);
            }
        }
        #endregion
    }
}