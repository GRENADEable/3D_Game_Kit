using UnityEngine;

namespace RedMountMedia
{
    public class FPSInventoryUI : MonoBehaviour
    {
        #region Serialized Variables

        #region Datas
        [Space, Header("Data")]
        [SerializeField]
        [Tooltip("GameManager Scriptable Object")]
        private GameMangerData gmData = default;

        [SerializeField]
        [Tooltip("Inventory Scriptable Object")]
        private InventoryData invenData = default;
        #endregion

        [SerializeField]
        [Tooltip("Is this using FPSManager Prefab or Not?")]
        private bool _isUsingFPSManager = default;

        #region Inventory Bag
        [Space, Header("Inventory Bag")]
        [SerializeField]
        [Tooltip("Which key when opening inventory panel")]
        private KeyCode inventoryKey = KeyCode.Tab;

        [SerializeField]
        [Tooltip("Prefab for inventory Button")]
        private GameObject inventoryButtonPrefab = default;

        [SerializeField]
        [Tooltip("Inventory panel GameObject")]
        private GameObject inventoryPanel = default;

        [SerializeField]
        [Tooltip("Rect transform GameObject where the the Slots variable will take refernce from")]
        private GameObject itemsParent = default;

        [SerializeField]
        [Tooltip("How strong the throw is when you drop the item from the inventory")]
        private float dropItemForce = 2;

        [SerializeField]
        [Tooltip("Transform parent Position for returninng item. If using FPSManager, you must leave this empty")]
        private Transform pickPropReturnParentPos = default;

        [SerializeField]
        [Tooltip("Transform parent Position for dropping item. If using FPSManager, you must leave this empty")]
        private Transform itemDropPos = default;
        #endregion

        #region Events
        public delegate void SendEventsBool(bool isOpen);

        /// <summary>
        /// Event received from FPSInventoryUI and sent to FPSManager and FPSDefaultUI Scripts;
        /// This just enables and disabled the HUD;
        /// </summary>
        public static event SendEventsBool OnHudEnabled;
        #endregion

        #endregion

        #region Private Variables
        [Header("Inventory Bag")]
        private InventorySlot[] _slots;
        #endregion

        #region Unity Callbacks

        #region Events
        void OnEnable()
        {
            FPSManager.OnUIPanelIntialised += OnUIPanelIntialised;

            InventorySlot.OnItemRemoved += OnItemRemovedEventReceived;

            InventoryData.OnItemChanged += OnItemChangedEventReceived;
        }

        void OnDisable()
        {
            FPSManager.OnUIPanelIntialised -= OnUIPanelIntialised;

            InventorySlot.OnItemRemoved -= OnItemRemovedEventReceived;

            InventoryData.OnItemChanged -= OnItemChangedEventReceived;
        }

        void OnDestroy()
        {
            FPSManager.OnUIPanelIntialised -= OnUIPanelIntialised;

            InventorySlot.OnItemRemoved -= OnItemRemovedEventReceived;

            InventoryData.OnItemChanged -= OnItemChangedEventReceived;
        }
        #endregion

        void OnApplicationQuit() => invenData.itemsList.Clear();

        void Start()
        {
            if (!_isUsingFPSManager)
            {
                for (int i = 0; i < invenData.inventorySpace; i++)
                    Instantiate(inventoryButtonPrefab, itemsParent.transform.position, Quaternion.identity, itemsParent.transform);

                _slots = itemsParent.GetComponentsInChildren<InventorySlot>();
            }
        }

        void Update() => OpenInventory();
        #endregion

        #region My Functions
        /// <summary>
        /// Added to Back Button on Inventory Button UI;
        /// </summary>
        public void OnClick_InventoryClose()
        {
            inventoryPanel.SetActive(false);
            OnHudEnabled?.Invoke(true);
            gmData.ChangeGameState("Game");
        }

        /// <summary>
        /// Opens Inventory Panel when Button is pressed
        /// </summary>
        void OpenInventory()
        {
            if (Input.GetKeyDown(inventoryKey) && gmData.currState == GameMangerData.GameState.Game)
            {
                inventoryPanel.SetActive(true);
                OnHudEnabled?.Invoke(false);
                gmData.ChangeGameState("Inventory");
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Subbed to event from FPSManager Sccript. This event just intialses if the FPS_Manager prefab is present;
        /// </summary>
        void OnUIPanelIntialised()
        {
            // GameObjectFind is slow but will be changed later on;
            if (!GameObject.FindGameObjectWithTag("Item_Drop_Pos"))
            {
                Debug.LogError("Missing item drop position transform. Is the Player missing in the scene?");
                //Debug.Break();
                return;
            }
            else if (!GameObject.FindGameObjectWithTag("Item_Drop_Parent"))
            {
                Debug.LogError("Add missing tag for FPSUIManager. Add empty Gameobject Item_Drop_Parent and add the respective tag to the Gameobject");
                Debug.Break();
                return;
            }
            else
            {
                itemDropPos = GameObject.FindGameObjectWithTag("Item_Drop_Pos").transform;
                pickPropReturnParentPos = GameObject.FindGameObjectWithTag("Item_Drop_Parent").transform;
            }

            for (int i = 0; i < invenData.inventorySpace; i++)
                Instantiate(inventoryButtonPrefab, itemsParent.transform.position, Quaternion.identity, itemsParent.transform);

            _slots = itemsParent.GetComponentsInChildren<InventorySlot>();
        }

        /// <summary>
        /// Subbed to Event from InventoryData. This event updates the UI of the inventory;
        /// </summary>
        void OnItemChangedEventReceived()
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                if (i < invenData.itemsList.Count)
                    _slots[i].AddItem(invenData.itemsList[i]);
                else
                    _slots[i].ClearSlot();
            }
        }

        /// <summary>
        /// Subbed to event from InventorySlot. This event recieves the prefab the player removed from the inventory;
        /// </summary>
        /// <param name="obj"> Must have a gameobject parameter so that it can spawn the item; </param>
        void OnItemRemovedEventReceived(GameObject obj)
        {
            GameObject invenObj = Instantiate(obj, itemDropPos.position, Quaternion.identity, pickPropReturnParentPos);
            invenObj.GetComponent<Rigidbody>().AddForce(itemDropPos.forward * dropItemForce, ForceMode.Impulse);
        }
        #endregion
    }
}