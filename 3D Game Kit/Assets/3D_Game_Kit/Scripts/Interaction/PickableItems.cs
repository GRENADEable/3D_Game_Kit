using UnityEngine;

namespace RedMountMedia
{
    [RequireComponent(typeof(Rigidbody))]
    public class PickableItems : MonoBehaviour
    {
        #region Public Variables

        #region Datas
        [Space, Header("Examine Item")]
        [SerializeField]
        [Tooltip("Items Scriptable Object. Only use this if you want to store the item in the inventory")]
        public ItemsData itemsData;
        #endregion

        #region Examine Variables
        [Space, Header("Examine Variables")]
        [Tooltip("Will the item have free rotation? Horizontal? Vertical or No rotation?")]
        public ExamineTypes currExamine = ExamineTypes.Free;
        public enum ExamineTypes { Free, Horizontal, Vertical, None };

        [Tooltip("Can this item be stored in the inventory. If true then you have to set a Scriptable Object Asset with the script.")]
        public bool canBeStored = true;

        [Tooltip("Rotation speed")]
        public float rotationSpeed = 300f;

        [Tooltip("Vector3 position offset of the Examine GameObject")]
        public Vector3 examinePosOffset = Vector3.zero;

        [Tooltip("Vector3 rotation offset of the Examine GameObject")]
        public Vector3 examineRotOffset = Vector3.zero;

        [Tooltip("Vector3 scale of the Examine GameObject")]
        public Vector3 examineScale = Vector3.one;
        #endregion

        #region Examine Description
        [Space, Header("Examine Description")]
        [TextArea(5, 5)]
        [Tooltip("Item information")]
        public string itemDescription = "New Description";

        [Tooltip("Item information Text Color")]
        public Color itemDescriptionTextColor = Color.white;

        [Tooltip("Item information Text Size")]
        public float itemDescriptionTextSize = 50f;
        #endregion

        #endregion

        #region Private Variables
        [Header("Examine Variables")]
        private Rigidbody _rb = default;
        private Transform _rotateObjTransform = default;
        private Vector3 _intialObjPos = default;
        private Quaternion _intialObjRot = default;
        private Vector3 _intialObjScale = default;
        #endregion

        #region Unity Callbacks
        void OnApplicationQuit()
        {
            if (canBeStored)
            {
                itemsData.isIntialised = false;
                itemsData.intialPos = Vector3.zero;
                itemsData.intialRot = Quaternion.identity;
            }
        }

        void Start() => Intialise();

        void Update() => RotateObjectMouse();
        #endregion

        #region My Functions
        /// <summary>
        /// Gets references of the item this script is attached to;
        /// Stores this item's position and rotation in a scriptable object;
        /// </summary>
        void Intialise()
        {
            _rb = GetComponent<Rigidbody>();
            //gameObject.GetComponent<MeshRenderer>().material.DisableKeyword(_emissive);

            if (canBeStored)
            {
                if (!itemsData.isIntialised)
                {
                    itemsData.intialPos = gameObject.transform.position;
                    itemsData.intialRot = gameObject.transform.rotation;
                    itemsData.isIntialised = true;
                }
            }
        }

        /// <summary>
        /// Rotation with Mouse; Added different types of locks
        /// </summary>
        void RotateObjectMouse()
        {
            if (Input.GetMouseButton(0) && _rotateObjTransform != null)
            {
                if (currExamine == ExamineTypes.Free)
                {
                    RotateVertical();
                    RotateHorizontal();
                }

                if (currExamine == ExamineTypes.Horizontal)
                    RotateHorizontal();

                if (currExamine == ExamineTypes.Vertical)
                    RotateVertical();

                if (currExamine == ExamineTypes.None)
                    return;
            }
        }

        void RotateVertical()
        {
            float vertical = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
            _rotateObjTransform.RotateAround(_rotateObjTransform.position, Vector3.right, vertical);
        }

        void RotateHorizontal()
        {
            float horizontal = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            _rotateObjTransform.RotateAround(_rotateObjTransform.position, Vector3.down, horizontal);
        }

        public void StartInteraction()
        {
            _rb.isKinematic = true;

            _intialObjPos = transform.position;
            _intialObjRot = transform.rotation;
            _intialObjScale = transform.localScale;

            gameObject.layer = LayerMask.NameToLayer("InspectionLayer");
            _rotateObjTransform = transform;
            //Debug.Log("Examine Reference Started");
        }

        public void EndInteraction()
        {
            transform.SetPositionAndRotation(_intialObjPos, _intialObjRot);
            transform.localScale = _intialObjScale;
            _rotateObjTransform = null;
            _rb.isKinematic = false;
            //Debug.Log("Examine Reference Ended");
        }
        #endregion
    }
}