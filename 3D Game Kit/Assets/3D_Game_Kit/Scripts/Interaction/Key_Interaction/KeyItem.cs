using UnityEngine;

namespace RedMountMedia
{
    public class KeyItem : MonoBehaviour
    {
        #region Serialized Variables
        [SerializeField]
        [Tooltip("Key name. Has to be the same exact name as the DoorTrigger Script or else it won't work")]
        private int keyID = default;
        #endregion

        #region Private Variables
        //private const string _emissive = "_EMISSION";
        #endregion

        #region Unity Callbacks
        //void Start() => gameObject.GetComponent<MeshRenderer>().material.DisableKeyword(_emissive);
        #endregion

        #region My Functions
        /// <summary>
        /// Gets the key iD;
        /// </summary>
        /// <returns> Returns key iD; </returns>
        public int GetKeyTypeInt() => keyID;
        #endregion
    }
}