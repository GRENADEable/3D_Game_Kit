using System.Collections;
using UnityEngine;

namespace RedMountMedia
{
    public class FPSManager : MonoBehaviour
    {
        #region Serialized Variables
        [Space, Header("Data")]
        [SerializeField]
        [Tooltip("GameManager Scriptable Object")]
        private GameMangerData gmData = default;

        #region Intialisation
        [Space, Header("Intialisation")]
        [SerializeField]
        [Tooltip("Must have player prefab for Player Movement")]
        private GameObject fpsPlayer = default;

        [SerializeField]
        [Tooltip("Must have interaction prefab for Player Interaction")]
        private GameObject fpsInteraction = default;

        [SerializeField]
        [Tooltip("Must have examine cam prefab for Object Examination")]
        private GameObject fpsExamineCam = default;

        [SerializeField]
        [Tooltip("Must have canvas prefab for UI")]
        private GameObject fpsCanvas = default;

        [SerializeField]
        [Tooltip("Must have event system prefab for Input")]
        private GameObject fpsEventSystem = default;

        [SerializeField]
        [Tooltip("Do you want to start the game with a delay?")]
        private bool delayStart = default;

        [SerializeField]
        [Tooltip("Total delay time")]
        private float delayTime = 1f;
        #endregion

        [Space, Header("Transform References")]
        [SerializeField]
        [Tooltip("Where the player will start. Can be left empty")]
        private Transform fpsPlayerStart = default;

        #region Events

        #region Empty Events
        public delegate void SendEvents();
        /// <summary>
        /// Event received from FPSManager and sent to FPSInventoryUI Scripts;
        /// This event controls if we want to fade in and start or not through delayStart bool;
        /// </summary>
        public static event SendEvents OnFadePanel;

        /// <summary>
        /// Event received from FPSManager and sent to FPSInventoryUI Scripts;
        /// This event intialises the UI in the FPSUIManager;
        /// </summary>
        public static event SendEvents OnUIPanelIntialised;

        /// <summary>
        /// Event received from FPSManager and sent to ExamineSystem Script;
        /// This event intialises the Examine System in the FPSInventoryUI;
        /// </summary>
        public static event SendEvents OnPlayerIntialised;
        #endregion

        #region GameObject Events
        public delegate void SendEevntsGameObject(GameObject obj);
        /// <summary>
        /// Event receievd from FPSManager and sent to PlayerInventory Scripts;
        /// This event intialises the inventory variables in PlayerInventory
        /// </summary>
        public static event SendEevntsGameObject OnPlayerInvenIntialised;
        #endregion

        #endregion

        #endregion

        #region Unity Callbacks

        #region Events
        void OnEnable()
        {
            FPSInventoryUI.OnHudEnabled += OnHudEnabledEventReceived;

            ExamineSystem.OnHudEnabled += OnHudEnabledEventReceived;

            PlayerInventory.OnItemAdded += OnItemAddedEventReceived;
        }

        void OnDisable()
        {
            FPSInventoryUI.OnHudEnabled -= OnHudEnabledEventReceived;

            ExamineSystem.OnHudEnabled -= OnHudEnabledEventReceived;

            PlayerInventory.OnItemAdded -= OnItemAddedEventReceived;
        }

        void OnDestroy()
        {
            FPSInventoryUI.OnHudEnabled -= OnHudEnabledEventReceived;

            ExamineSystem.OnHudEnabled -= OnHudEnabledEventReceived;

            PlayerInventory.OnItemAdded -= OnItemAddedEventReceived;
        }
        #endregion

        void Start()
        {
            DisableCursor();
            Intialise();
        }
        #endregion

        #region My Functions

        #region Intialisation
        /// <summary>
        /// Spawning player prefab and player canvas in the scene on runtime;
        /// </summary>
        void Intialise()
        {
            if (!fpsPlayer || !fpsInteraction || !fpsEventSystem || !fpsCanvas || !fpsExamineCam)
            {
                Debug.LogError("Add missing prefabs in FPSUIManager");
                Debug.Break();
                return;
            }

            Instantiate(fpsPlayer, GetStartPos(), GetStartRot(), transform);
            GameObject camObj = Instantiate(fpsExamineCam, transform.position, Quaternion.identity, transform);
            Instantiate(fpsInteraction, transform.position, Quaternion.identity, transform);
            Instantiate(fpsCanvas, transform.position, Quaternion.identity, transform);
            Instantiate(fpsEventSystem, transform.position, Quaternion.identity, transform);

            if (delayStart)
            {
                StartCoroutine(StartDelay());
                OnFadePanel?.Invoke();
            }
            else
                gmData.ChangeGameState("Game");

            // Intialising spawned prefabs via Events;
            OnUIPanelIntialised?.Invoke();
            OnPlayerInvenIntialised?.Invoke(camObj);
            OnPlayerIntialised?.Invoke();
        }

        /// <summary>
        /// Starting position of the player. The Trasnform component can be left empty;
        /// </summary>
        /// <returns> Returns Vector3; </returns>
        Vector3 GetStartPos()
        {
            if (fpsPlayerStart != null)
                return fpsPlayerStart.position;
            else
                return Vector3.zero;
        }

        /// <summary>
        /// Starting rotation of the player. The Rotation component can be left empty;
        /// </summary>
        /// <returns> Returns Quaternion; </returns>
        Quaternion GetStartRot()
        {
            if (fpsPlayerStart != null)
                return fpsPlayerStart.rotation;
            else
                return Quaternion.identity;
        }
        #endregion

        #region Cursor
        /// <summary>
        /// Enables the user's Cursor;
        /// </summary>
        void EnableCursor()
        {
            gmData.VisibleCursor(true);
            gmData.LockCursor(false);
        }

        /// <summary>
        /// Disables the user's Cursor;
        /// </summary>
        void DisableCursor()
        {
            gmData.VisibleCursor(false);
            gmData.LockCursor(true);
        }
        #endregion

        #endregion

        #region Events
        /// <summary>
        /// Function called when examine is started;
        /// </summary>
        /// <param name="isHudEnabled"> Bool for enabling and disabling cursor; </param>
        void OnHudEnabledEventReceived(bool isHudEnabled)
        {
            if (isHudEnabled)
                DisableCursor();
            else
                EnableCursor();
        }

        /// <summary>
        /// Function called when item is in inventory;
        /// </summary>
        void OnItemAddedEventReceived() => DisableCursor();
        #endregion

        #region Coroutines
        /// <summary>
        /// Start is delayed as it has to fade in the fadePanel component, spawn in the dress and start the arrow spinning;
        /// </summary>
        /// <returns> Float delay; </returns>
        IEnumerator StartDelay()
        {
            gmData.ChangeGameState("Intro");
            yield return new WaitForSeconds(delayTime);
            gmData.ChangeGameState("Game");
        }
        #endregion
    }
}