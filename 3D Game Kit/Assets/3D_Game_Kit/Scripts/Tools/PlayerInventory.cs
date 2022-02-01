using UnityEngine;

namespace RedMountMedia
{
    public class PlayerInventory : MonoBehaviour
    {
        #region Serialized Variables
        [Space, Header("Data")]
        [SerializeField]
        [Tooltip("GameManager Scriptable Object")]
        private GameMangerData gmData = default;

        [SerializeField]
        [Tooltip("Inventory Scriptable Object")]
        private InventoryData invenData = default;

        [Space, Header("Interaction References")]
        [SerializeField]
        [Tooltip("Which layer(s) is used to examine?")]
        private LayerMask examineLayer = default;

        [SerializeField]
        [Tooltip("Ray distance of the inventroy")]
        private float rayDistance = 3f;

        [SerializeField]
        [Tooltip("Examine Camera References. If using FPSManager, you must leave this empty")]
        private GameObject examineCam = default;

        #region Events
        public delegate void SendEvents();
        /// <summary>
        /// Event received from PlayerInventory and sent to FPSManager, ExamineSystem and FPSDefaultUI Scripts;
        /// This event just resets the parameters after the player keeps item in the bag after examination;
        /// </summary>
        public static event SendEvents OnItemAdded;
        #endregion

        #endregion

        #region Private Variables
        [Header("Interaction References")]
        private RaycastHit _hit;
        private bool _isInteracting;
        #endregion

        #region Unity Callbacks

        #region Events
        void OnEnable() => FPSManager.OnPlayerInvenIntialised += OnPlayerInvenIntialised;

        void OnDisable() => FPSManager.OnPlayerInvenIntialised -= OnPlayerInvenIntialised;

        void OnDestroy() => FPSManager.OnPlayerInvenIntialised -= OnPlayerInvenIntialised;
        #endregion

        void Update() => RaycastChecks();
        #endregion

        #region My Functions
        /// <summary>
        /// Raycast from Examine Cam to Bag Items;
        /// Checks the item is within the pickable range and that its storable in the bagpack;
        /// </summary>
        void RaycastChecks()
        {
            Ray ray = new Ray(examineCam.transform.position, examineCam.transform.forward);

            _isInteracting = Physics.Raycast(ray, out _hit, rayDistance, examineLayer);
            Debug.DrawRay(ray.origin, ray.direction * rayDistance, _isInteracting ? Color.black : Color.white);

            if (_isInteracting)
            {
                if (_hit.collider.GetComponent<PickableItems>() != null)
                {
                    if (_hit.collider.GetComponent<PickableItems>().canBeStored)
                    {
                        if (Input.GetKeyDown(KeyCode.E) && gmData.currState == GameMangerData.GameState.Examine)
                            Pickup();
                    }
                }
            }
        }

        /// <summary>
        /// Pickup Item;
        /// </summary>
        void Pickup()
        {
            var item = _hit.collider.gameObject.GetComponent<PickableItems>();

            if (item)
            {
                bool wasPickedUp = invenData.AddItem(item.itemsData);

                if (wasPickedUp)
                {
                    Destroy(item.gameObject);
                    OnItemAdded?.Invoke();
                }
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Subbed to event from FPSManager Script;
        /// </summary>
        /// <param name="obj"> Must have a gameobject reference; </param>
        void OnPlayerInvenIntialised(GameObject obj) => examineCam = obj;
        #endregion
    }
}