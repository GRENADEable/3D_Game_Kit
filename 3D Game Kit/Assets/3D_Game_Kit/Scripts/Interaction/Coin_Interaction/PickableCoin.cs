using UnityEngine;

namespace RedMountMedia
{
    [RequireComponent(typeof(Rigidbody))]
    public class PickableCoin : MonoBehaviour
    {
        #region Serialized Variables
        [Space, Header("Data")]
        [SerializeField]
        [Tooltip("Coin item Scriptable Object. You have to set the item as Coin type")]
        private ItemsData itemsData = default;

        [SerializeField]
        [Tooltip("What is the player Tag?")]
        private string triggerTag = default;

        public delegate void SendEventsInt(int value);
        /// <summary>
        /// Event sent from PickableCoin to FPSCoinUI Script;
        /// This event just sends an integer value on how much score should it increment;
        /// </summary>
        public static event SendEventsInt OnCoinIncrement;
        #endregion

        #region Unity Callbacks
        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag(triggerTag))
            {
                OnCoinIncrement?.Invoke(itemsData.coinIncrement);
                Destroy(gameObject);
            }
        }
        #endregion
    }
}