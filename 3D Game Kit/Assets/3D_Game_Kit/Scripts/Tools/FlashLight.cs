using UnityEngine;

namespace RedMountMedia
{
    public class FlashLight : MonoBehaviour
    {
        #region Serialized Variables
        [Space, Header("Flashlight")]
        [SerializeField]
        [Tooltip("Flashlight toggle, true to make the falshlight interact with the key, false to not make it interactable with the key")]
        private bool isEnabled = false;

        [SerializeField]
        [Tooltip("Which key to press when toggling Flashlight")]
        private KeyCode lightKey = KeyCode.F;

        [SerializeField]
        [Tooltip("Flashlight Prefab, This can be left empty if you are referencing from the scene")]
        private GameObject flashLightPrefabObj = default;

        [SerializeField]
        [Tooltip("Flashlight root GameObject. Reference from the scene itself")]
        private GameObject flashLightSceneObj = default;

        [SerializeField]
        [Tooltip("Flashlight movment speed, higher the number, the faster it is")]
        private float speed = 3f;
        #endregion

        #region Private Variables
        private Vector3 _offset = default;
        private GameObject _objFollow = default;
        #endregion

        #region Unity Callbacks

        #region Events
        void OnEnable() => PropInteraction.OnTorchPick += OnTorchPickEventReceived;

        void OnDisable() => PropInteraction.OnTorchPick -= OnTorchPickEventReceived;

        void OnDestroy() => PropInteraction.OnTorchPick -= OnTorchPickEventReceived;
        #endregion

        void Start()
        {
            if (flashLightSceneObj)
            {
                _objFollow = Camera.main.gameObject;
                _offset = flashLightSceneObj.transform.position - _objFollow.transform.position;
            }
            else
            {
                _objFollow = Camera.main.gameObject;
                GameObject lightObj = Instantiate(flashLightPrefabObj, _objFollow.transform.position, Quaternion.identity);
                _offset = lightObj.transform.position - _objFollow.transform.position;
                flashLightSceneObj = lightObj;
            }
        }

        void Update()
        {
            if (flashLightSceneObj && isEnabled)
            {
                ToggleFlashLight();
                OffsetFlashLight();
            }
        }
        #endregion

        #region My Functions
        /// <summary>
        /// Turn on and off the flashlight GameObject;
        /// </summary>
        void ToggleFlashLight()
        {
            if (Input.GetKeyDown(lightKey))
                flashLightSceneObj.SetActive(!flashLightSceneObj.activeSelf);
        }

        /// <summary>
        /// Smooth lerp the flashlight GameObject;
        /// </summary>
        void OffsetFlashLight()
        {
            flashLightSceneObj.transform.position = _objFollow.transform.position + _offset;
            flashLightSceneObj.transform.rotation = Quaternion.Slerp(flashLightSceneObj.transform.rotation, _objFollow.transform.rotation, speed * Time.deltaTime);
        }
        #endregion

        #region Event
        void OnTorchPickEventReceived()
        {
            isEnabled = true;
            flashLightSceneObj.SetActive(true);
        }
        #endregion
    }
}