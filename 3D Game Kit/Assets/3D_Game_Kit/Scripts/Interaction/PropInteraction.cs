using UnityEngine;

namespace RedMountMedia
{
    public class PropInteraction : MonoBehaviour
    {
        #region Serialized Variables
        [Space, Header("Data")]
        [SerializeField]
        [Tooltip("GameManager Scriptable Object")]
        private GameMangerData gmData = default;

        [SerializeField]
        [Tooltip("Can it depend on the GameManager Scriptable Object? Set this to true if you want to use the GmData Scriptable Object")]
        private bool isUsingScriptableObject = default;

        [Space, Header("Door Raycast")]
        [SerializeField]
        [Tooltip("Which key to press when interacting")]
        private KeyCode interactKey = KeyCode.E;

        [SerializeField]
        [Tooltip("Ray distance for interaction")]
        private float rayDistance = default;

        [SerializeField]
        [Tooltip("Which layer(s) is used to interact?")]
        private LayerMask propLayer = default;

        [SerializeField]
        [Tooltip("Flashlight Tag")]
        private string flashlightTag = "Flashlight";

        #region Events
        public delegate void SendEvents();
        public static event SendEvents OnTorchPick;
        #endregion

        #endregion

        #region Private Variables
        private Camera _cam = default;

        [Header("Door Raycast")]
        private bool _isInteracting = default;
        private RaycastHit _hit = default;
        #endregion

        #region Unity Callbacks
        void Start() => _cam = Camera.main;

        void Update()
        {
            if (isUsingScriptableObject)
            {
                if (gmData.currState != GameMangerData.GameState.Inventory)
                    RaycastCheck();
            }
            else
                RaycastCheck();
        }
        #endregion

        #region My Functions
        void RaycastCheck()
        {
            Ray ray = new Ray(_cam.transform.position, _cam.transform.forward);
            _isInteracting = Physics.Raycast(ray.origin, ray.direction, out _hit, rayDistance, propLayer);
            Debug.DrawRay(ray.origin, ray.direction * rayDistance, _isInteracting ? Color.black : Color.red);

            if (_isInteracting)
            {
                if (Input.GetKeyDown(interactKey))
                {
                    if (_hit.collider.GetComponentInParent<SwingController>() != null)
                        _hit.collider.GetComponentInParent<SwingController>().InteractDoorRaycast();

                    if (_hit.collider.GetComponent<WorldInteraction>() != null)
                        _hit.collider.GetComponent<WorldInteraction>().InteractPropRaycast();

                    if (_hit.collider.CompareTag(flashlightTag))
                    {
                        OnTorchPick?.Invoke();
                        Destroy(_hit.collider.gameObject);
                    }
                }
            }
        }
        #endregion
    }
}