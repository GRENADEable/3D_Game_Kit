using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedMountMedia
{
    public class SSTool : MonoBehaviour
    {
        #region Public Variables
        public static SSTool instance;
        [HideInInspector]public List<Texture2D> ssImages = new List<Texture2D>();
        #endregion

        #region Serialized Variables

        #region Datas
        [Space, Header("Data")]
        [SerializeField]
        [Tooltip("GameManager Scriptable Object")]
        private GameMangerData gmData;
        #endregion

        #region Inputs
        [Space, Header("Inputs")]
        [SerializeField]
        [Tooltip("Which key is pressed to open the Camera")]
        private KeyCode sSCamOpenKey = KeyCode.C;

        [SerializeField]
        [Tooltip("Which key is pressed to close the Camera")]
        private KeyCode sSCamCloseKey = KeyCode.Mouse1;

        [SerializeField]
        [Tooltip("Which key is pressed to take Screenshot from the Camera")]
        private KeyCode sSClickKey = KeyCode.Mouse0;
        #endregion

        #region Screenshot Camera
        [Space, Header("Screenshot Camera")]
        [SerializeField]
        [Tooltip("How many total number of screenshot the player can take?")]
        private int totalShots;
        #endregion

        #region Events

        #region Void Events
        public delegate void SendEvents();
        /// <summary>
        /// Event sent from SSTool to SSUI;
        /// This just makes changes to the UI when the Screenshot is taken;
        /// </summary>
        public static event SendEvents OnSSTaken;

        /// <summary>
        /// Event sent from SSTool to SSUI;
        /// /// This just rests the references to default in the SSUI Script;
        /// </summary>
        public static event SendEvents OnResetIndex;
        #endregion

        #region Bool Events
        public delegate void SendEventsBool(bool isCamOpened);
        /// <summary>
        /// Event sent from SSTool to SSUI Script;
        /// Enables and disables Camera;
        /// </summary>
        public static event SendEventsBool OnCamEnabled;

        /// <summary>
        /// Event sent from SSTool to SSUI Script;
        /// Enables and disables HUD panel;
        /// </summary>
        public static event SendEventsBool OnHudEnabled;
        #endregion

        #region Int Events
        public delegate void SendEventsInt(int index);
        /// <summary>
        /// Event sent from SSTool to SSUI;
        /// This just sets the total numbers of screenshots set from the SSTool Script;
        /// </summary>
        public static event SendEventsInt OnTotalShots;
        #endregion

        #endregion

        #endregion

        #region Private Variables
        private int _imageIndex;
        private bool _isCamOpen;
        private bool _isTakingSS;
        #endregion

        #region Unity Callbacks

        #region Singleton
        void Awake()
        {
            /// Made it a singleton to access some public variables;
            if (instance != null && instance != this)
                Destroy(this.gameObject);
            else
                instance = this;
        }
        #endregion

        #region Events
        void OnEnable()
        {
            SSUI.OnSSInvenEnabled += OnSSInvenEnabledEventReceived;
            SSUI.OnDeleteAllImages += OnDeleteAllImagesEventReceived;
            SSUI.OnDeleteCurrentImage += OnDeleteCurrentImageEventReceived;
        }

        void OnDisable()
        {
            SSUI.OnSSInvenEnabled -= OnSSInvenEnabledEventReceived;
            SSUI.OnDeleteAllImages -= OnDeleteAllImagesEventReceived;
            SSUI.OnDeleteCurrentImage -= OnDeleteCurrentImageEventReceived;
        }

        void OnDestroy()
        {
            SSUI.OnSSInvenEnabled -= OnSSInvenEnabledEventReceived;
            SSUI.OnDeleteAllImages -= OnDeleteAllImagesEventReceived;
            SSUI.OnDeleteCurrentImage -= OnDeleteCurrentImageEventReceived;
        }
        #endregion

        void Start() => OnTotalShots?.Invoke(totalShots);

        void Update()
        {
            if (gmData.currState == GameMangerData.GameState.Game)
            {
                ToolsHotKeysCheck();
                ScreenshotClickCheck();
            }
        }
        #endregion

        #region My Functions
        /// <summary>
        /// Function checks the keys pressed when opening and closing Camera;
        /// </summary>
        void ToolsHotKeysCheck()
        {
            if (Input.GetKeyDown(sSCamOpenKey) && !_isCamOpen)
            {
                OnCamEnabled?.Invoke(true);
                _isCamOpen = true;
            }

            if (Input.GetKeyDown(sSCamCloseKey) && _isCamOpen)
            {
                OnCamEnabled?.Invoke(false);
                _isCamOpen = false;
            }
        }

        /// <summary>
        /// Function checks the keys pressed when taking Screenshot;
        /// </summary>
        void ScreenshotClickCheck()
        {
            if (Input.GetKeyDown(sSClickKey) && !_isTakingSS
                && _isCamOpen && _imageIndex < totalShots)
                StartCoroutine(TakeSSDelay());
        }
        #endregion

        #region Coroutines
        /// <summary>
        /// Coroutine for taking Screenshot;
        /// Added two delays so that it can disable the UI, and process the screenshot properly;
        /// </summary>
        /// <returns> Float delay for coroutine </returns>
        IEnumerator TakeSSDelay()
        {
            int width = Screen.width;
            int height = Screen.height;

            OnHudEnabled?.Invoke(false);
            _isTakingSS = true;
            yield return new WaitForSeconds(0.1f);

            Texture2D SS = new Texture2D(width, height, TextureFormat.ARGB32, false);
            yield return new WaitForEndOfFrame();
            SS.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            SS.Apply();
            SS.name = $"Screenshot_{_imageIndex}";
            ssImages.Add(SS);
            _imageIndex++;
            Debug.Log("Taken Screenshot ");
            OnHudEnabled?.Invoke(true);
            OnSSTaken?.Invoke();
            yield return new WaitForSeconds(0.1f);
            _isTakingSS = false;
        }
        #endregion

        #region Events
        /// <summary>
        /// Subbed to OnSSInvenEnabled Event;
        /// Just disables camera usage when Screenshot inventory is open;
        /// </summary>
        void OnSSInvenEnabledEventReceived() => _isCamOpen = false;

        /// <summary>
        /// Subbed to OnDeleteAllImages Event;
        /// Resets references to default on SSUI Script;
        /// Sends Event back to SSUI Script to be resetted;
        /// </summary>
        void OnDeleteAllImagesEventReceived()
        {
            _imageIndex = 0;
            OnResetIndex?.Invoke();
        }

        /// <summary>
        /// Subbed to event from SSUI Script;
        /// </summary>
        void OnDeleteCurrentImageEventReceived() => _imageIndex = ssImages.Count;
        #endregion
    }
}