using UnityEngine;

namespace RedMountMedia
{
    public class ExamineSystem : MonoBehaviour
    {
        #region Serialized Variables
        [Space, Header("Data")]
        [SerializeField]
        [Tooltip("GameManager Scriptable Object")]
        private GameMangerData gmData = default;

        [Space, Header("Key Inputs")]
        [SerializeField]
        [Tooltip("Which key to press picking")]
        private KeyCode pickupKey = KeyCode.E;

        [SerializeField]
        [Tooltip("Which key to press when dropping")]
        private KeyCode dropKey = KeyCode.Mouse1;

        [Space, Header("Examine References")]
        [SerializeField]
        [Tooltip("Which layer(s) is used to examine?")]
        private LayerMask examineLayer = default;

        [SerializeField]
        [Tooltip("Ray distance for examine")]
        private float rayDistance = 2f;

        [SerializeField]
        [Tooltip("Transform Position for examination. If using FPSManager, you must leave this empty")]
        private Transform examinePointPos = default;

        [SerializeField]
        [Tooltip("Transform parent Position for returninng item. If using FPSManager, you must leave this empty")]
        private Transform pickPropReturnParentPos = default;

        #region Events

        #region Bool Events
        public delegate void SendEventsBool(bool isHudEnabled);
        /// <summary>
        /// Event received from ExamineSystem and sent to FPSUIManager and FPSManager Scripts;
        /// This event just enables and disables cursor from the FPSManager and also disables and enables hud from FPSUIManager;
        /// </summary>
        public static event SendEventsBool OnHudEnabled;

        /// <summary>
        /// Event received from ExamineSystem and sent to FPSUIManager Scripts;
        /// This event just enables and disables the examine panel;
        /// </summary>
        public static event SendEventsBool OnExamineHudEnabled;
        #endregion

        public delegate void SendEventsPickItem(PickableItems pick);
        public static event SendEventsPickItem OnItemDescription;
        #endregion

        #endregion

        #region Private Variables
        [Header("Examine Variables")]
        private Camera _cam = default;
        private RaycastHit _hit = default;
        private GameObject _tempObjReference = default;
        private PickableItems _tempPickItem = default;
        private bool _isInteracting = default;
        private bool _isExamine = default;

        //[Header("Shader")]
        //private bool _isHighlighted;
        //private const string _emissive = "_EMISSION";
        //private Renderer _highlightObjRenderer;
        #endregion

        #region Unity Callbacks

        #region Events
        void OnEnable()
        {
            FPSManager.OnPlayerIntialised += OnPlayerIntialisedEventRecieved;

            PlayerInventory.OnItemAdded += OnItemAddedEventReceived;
        }

        void OnDisable()
        {
            FPSManager.OnPlayerIntialised -= OnPlayerIntialisedEventRecieved;

            PlayerInventory.OnItemAdded -= OnItemAddedEventReceived;
        }

        void OnDestroy()
        {
            FPSManager.OnPlayerIntialised -= OnPlayerIntialisedEventRecieved;

            PlayerInventory.OnItemAdded -= OnItemAddedEventReceived;
        }
        #endregion

        void Start() => _cam = Camera.main;

        void Update() => RaycastChecks();
        #endregion

        #region My Functions

        #region Examine Checks
        /// <summary>
        /// Exmaine System check for pickup and drop;
        /// </summary>
        void RaycastChecks()
        {
            if (gmData.currState != GameMangerData.GameState.Inventory)
            {
                Ray ray = new Ray(_cam.transform.position, _cam.transform.forward);

                _isInteracting = Physics.Raycast(ray, out _hit, rayDistance, examineLayer);
                Debug.DrawRay(ray.origin, ray.direction * rayDistance, _isInteracting ? Color.red : Color.white);

                if (_isInteracting)
                {
                    //// For highlighting object;
                    //if (!_isHighlighted && _hit.collider.GetComponentInChildren<Renderer>() != null)
                    //{
                    //    _highlightObjRenderer = _hit.collider.GetComponent<Renderer>();
                    //    _highlightObjRenderer.material.EnableKeyword(_emissive);
                    //}

                    //_isHighlighted = true;

                    // Pickup;
                    if (Input.GetKeyDown(pickupKey) && !_isExamine && _tempObjReference == null)
                    {
                        if (_hit.collider.GetComponent<PickableItems>() != null)
                        {
                            _tempPickItem = _hit.collider.GetComponent<PickableItems>();
                            _tempPickItem.StartInteraction();
                            ExamineStarted(_hit.collider.gameObject);
                            OnHudEnabled?.Invoke(false);
                            OnExamineHudEnabled?.Invoke(true);
                            _isExamine = true;
                        }
                    }
                }
                //else
                //{
                //    // For un-highlighting object;
                //    if (_isHighlighted && _highlightObjRenderer != null)
                //        _highlightObjRenderer.material.DisableKeyword(_emissive);

                //    _isHighlighted = false;
                //}

                // Drop;
                if (Input.GetKeyDown(dropKey) && _isExamine)
                {
                    _tempPickItem.EndInteraction();
                    ExamineEnded();
                    OnHudEnabled?.Invoke(true);
                    OnExamineHudEnabled?.Invoke(false);
                    _isExamine = false;
                }
            }
        }
        #endregion

        #region Examine Object References
        /// <summary>
        /// Start Examine System. Moves the object to the examine camera;
        /// </summary>
        /// <param name="obj"> Needs gameobject to hold temp references; </param>
        void ExamineStarted(GameObject obj)
        {
            _tempObjReference = obj;
            _tempObjReference.GetComponent<Rigidbody>().isKinematic = true;
            _tempObjReference.transform.parent = examinePointPos;
            _tempObjReference.transform.SetPositionAndRotation(examinePointPos.position + _tempPickItem.examinePosOffset, examinePointPos.rotation);
            _tempObjReference.transform.localScale = _tempPickItem.examineScale;
            _tempObjReference.transform.Rotate(_tempPickItem.examineRotOffset);

            gmData.ChangeGameState("Examine");

            PickableItems tempPickItems = _tempObjReference.GetComponent<PickableItems>();
            if (tempPickItems.itemDescription != null)
                OnItemDescription?.Invoke(tempPickItems);
            //Debug.Log("Examine Reference Started");
        }

        /// <summary>
        /// Ends Examine System. Moves object back to intial state and removes references;
        /// </summary>
        void ExamineEnded()
        {
            _tempObjReference.layer = LayerMask.NameToLayer("ExamineLayer");
            _tempObjReference.transform.parent = pickPropReturnParentPos;
            _tempObjReference = null;

            gmData.ChangeGameState("Game");
            //Debug.Log("Examine Reference Ended");
        }
        #endregion

        #endregion

        #region Events
        /// <summary>
        /// Subbed to event from FPSManager. This event just intialises the references on runtime;
        /// </summary>
        void OnPlayerIntialisedEventRecieved()
        {
            if (!GameObject.FindGameObjectWithTag("Examine_Pos"))
            {
                Debug.LogError("Missing examine position transform. Is the Examine_Cam missing in the scene?");
                Debug.Break();
                return;
            }
            else if (!GameObject.FindGameObjectWithTag("Item_Drop_Parent"))
            {
                Debug.LogError("Add missing tag for ExamineSystem. Add empty Gameobject Item_Drop_Parent and add the respective tag to the Gameobject");
                Debug.Break();
                return;
            }
            else
            {
                examinePointPos = GameObject.FindGameObjectWithTag("Examine_Pos").transform;
                pickPropReturnParentPos = GameObject.FindGameObjectWithTag("Item_Drop_Parent").transform;
            }
        }

        /// <summary>
        /// Subbed to event from PlayerInventory. This event just removes references after examination;
        /// </summary>
        void OnItemAddedEventReceived()
        {
            _tempPickItem = null;
            _tempObjReference = null;
            //_highlightObjRenderer = null;
            _isExamine = false;

            gmData.ChangeGameState("Game");
        }
        #endregion
    }
}