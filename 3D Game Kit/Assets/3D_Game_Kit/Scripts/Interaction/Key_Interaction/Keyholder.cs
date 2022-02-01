using System.Collections.Generic;
using UnityEngine;

namespace RedMountMedia
{
    public class Keyholder : MonoBehaviour
    {
        #region Serialized Variables
        [Space, Header("Data")]
        [SerializeField]
        [Tooltip("GameManager Scriptable Object")]
        private GameMangerData gmData = default;

        [SerializeField]
        [Tooltip("Can it depend on the GameManager Scriptable Object? Set this to true if you want to use the GmData Scriptable Object")]
        private bool isUsingScriptableObject = default;

        [Space, Header("Key Inputs")]
        [SerializeField]
        [Tooltip("Which key to press when interacting with KeyF ")]
        private KeyCode interactKey = KeyCode.E;

        [Space, Header("Key References")]
        [SerializeField]
        [Tooltip("Which key to press when running")]
        private LayerMask keyLayer = default;

        [SerializeField]
        [Tooltip("Which layer(s) is the door?")]
        private LayerMask doorLayer = default;

        [SerializeField]
        [Tooltip("Raycast distance from the player camera")]
        private float rayDistance = 2f;
        #endregion

        #region Private Variables
        private List<int> _keyItemsInt = new List<int>();
        private Camera _cam = default;
        private bool _isInteracting = default;
        private bool _isInteractingDoor = default;
        private Ray _ray = default;
        private RaycastHit _hit = default;

        //[Header("Shader")]
        //private bool _isHighlighted;
        //private Renderer _highlightObjRenderer;
        //private const string _emissive = "_EMISSION";
        #endregion

        #region Unity Callbacks
        void Start() => _cam = Camera.main;

        void Update()
        {
            if (isUsingScriptableObject)
            {
                if (gmData.currState != GameMangerData.GameState.Inventory)
                {
                    _ray = new Ray(_cam.transform.position, _cam.transform.forward);
                    RaycastChecksKeyItem();
                    RaycastCheckDoor();
                }
            }
            else
            {
                _ray = new Ray(_cam.transform.position, _cam.transform.forward);
                RaycastChecksKeyItem();
                RaycastCheckDoor();
            }
        }
        #endregion

        #region My Functions

        #region Keys
        /// <summary>
        /// Adds key iD to the list;
        /// </summary>
        /// <param name="keyInt"> Key iD parameter; </param>
        public void AddKeyInt(int keyInt) => _keyItemsInt.Add(keyInt);

        /// <summary>
        /// Removes key iD from the list;
        /// </summary>
        /// <param name="keyInt"> Key iD parameter; </param>
        public void RemoveKeyInt(int keyInt) => _keyItemsInt.Remove(keyInt);

        /// <summary>
        /// Checks key iD from the list;
        /// </summary>
        /// <param name="keyInt"> Key iD parameter; </param>
        public bool ContainKeyInt(int keyInt) => _keyItemsInt.Contains(keyInt);
        #endregion

        #region Raycast Checks
        /// <summary>
        /// Check for Key Pickup;
        /// </summary>
        void RaycastChecksKeyItem()
        {
            _isInteracting = Physics.Raycast(_ray, out _hit, rayDistance, keyLayer);
            //Debug.DrawRay(_ray.origin, _ray.direction * rayDistance, _isInteracting ? Color.red : Color.white);

            if (_isInteracting)
            {
                // For highlighting object;
                //if (!_isHighlighted)
                //{
                //    _highlightObjRenderer = _hit.collider.GetComponent<Renderer>();
                //    _highlightObjRenderer.material.EnableKeyword(_emissive);
                //}

                //_isHighlighted = true;

                // Pickup;
                if (Input.GetKeyDown(interactKey))
                {
                    if (_hit.collider.GetComponent<KeyItem>() != null)
                    {
                        AddKeyInt(_hit.collider.GetComponent<KeyItem>().GetKeyTypeInt());
                        //RemoveRefernces();
                        Destroy(_hit.collider.gameObject);
                    }
                }
            }
            //else
            //{
            //    // For un-highlighting object;
            //    if (_isHighlighted)
            //    {
            //        _highlightObjRenderer.material.DisableKeyword(_emissive);
            //        _isHighlighted = false;
            //    }
            //}
        }

        /// <summary>
        /// Check for Door;
        /// </summary>
        void RaycastCheckDoor()
        {
            _isInteractingDoor = Physics.Raycast(_ray, out _hit, rayDistance, doorLayer);

            if (_isInteractingDoor)
            {
                if (Input.GetKeyDown(interactKey))
                {
                    if (_hit.collider.GetComponentInParent<DoorTrigger>() != null)
                        _hit.collider.GetComponentInParent<DoorTrigger>().InteractDoor(gameObject);
                }
            }
        }

        //void RemoveRefernces()
        //{
        //    _highlightObjRenderer = null;
        //    _isHighlighted = false;
        //}
        #endregion

        #endregion
    }
}