using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RedMountMedia
{
    public class SSUI : MonoBehaviour
    {
        #region Serialized Variables

        #region Datas
        [Space, Header("Datas")]
        [SerializeField]
        [Tooltip("GameManager Scriptable Object")]
        private GameMangerData gmData;
        #endregion

        #region Screenshot Input
        [Space, Header("Screenshot Input")]
        [SerializeField]
        [Tooltip("Which key to press to open Screenshot inventory panel")]
        private KeyCode ssInvenKey = KeyCode.Tab;
        #endregion

        #region Screenshot UI
        [Space, Header("Screenshot UI")]
        [SerializeField]
        [Tooltip("HUD GameObject panel")]
        private GameObject hudPanel;

        [SerializeField]
        [Tooltip("Screenshot inventory GameObject panel")]
        private GameObject sSInventoryPanel;

        [SerializeField]
        [Tooltip("Screenshot enlarged GameObject panel")]
        private GameObject sSEnalrgedPanel;

        [SerializeField]
        [Tooltip("Screenshot RawImage Component reference")]
        private RawImage enlargeRawImage;
        #endregion

        #region Screenshot Button
        [Space, Header("Screenshot Button")]
        [SerializeField]
        [Tooltip("Screenshot Button GameObject")]
        private GameObject sSButtonPrefab;

        [SerializeField]
        [Tooltip("Screenshot Button Spawn Position")]
        private Transform sSButtonPos;
        #endregion

        #region Camera UI
        [Space, Header("Camera UI")]
        [SerializeField]
        [Tooltip("Camera GameObject panel")]
        private GameObject camPanel;

        [SerializeField]
        [Tooltip("Screenshot Counter Text")]
        private TextMeshProUGUI ssCounterText;
        #endregion

        #region Events

        #region Void Events
        public delegate void SendEvents();
        /// <summary>
        /// Event sent from SSUI to SSTool Script;
        /// Just disables taking Screenshot when in Screenshot inventory;
        /// </summary>
        public static event SendEvents OnSSInvenEnabled;

        /// <summary>
        /// Event sent from SSUI to SSTool Script;
        /// Resets references when all Screenshots are deleted;
        /// </summary>
        public static event SendEvents OnDeleteAllImages;

        /// <summary>
        /// Event sent from SSUI to SSTool Script;
        /// Removes current Screenshot and their references;
        /// </summary>
        public static event SendEvents OnDeleteCurrentImage;
        #endregion

        #region Bool Events
        public delegate void SendEventsBool(bool isHudEnabled);
        /// <summary>
        /// Event sent from SSUI to SSManager script;
        /// Enables and disables HUD panel;
        /// </summary>
        public static event SendEventsBool OnHudEnabled;
        #endregion

        #endregion

        #endregion

        #region Private Variables
        [Header("Screenshot")]
        private int _shotIndex;
        private int _totalShots;
        private int _index;
        private int _currIndex;
        private List<RawImage> _rawImageButtons = new List<RawImage>();
        #endregion

        #region Unity Callbacks

        #region Events
        void OnEnable()
        {
            SSTool.OnCamEnabled += OnCamEnabledEventReceived;
            SSTool.OnHudEnabled += OnHudEnabledEventReceived;
            SSTool.OnSSTaken += OnSSTakenEventReceived;
            SSTool.OnTotalShots += OnTotalShotsEventReceived;
            SSTool.OnResetIndex += OnResetIndexEventReceived;

            SSButton.OnImageEnlarge += OnImageEnlargeEventReceived;
        }

        void OnDisable()
        {
            SSTool.OnCamEnabled -= OnCamEnabledEventReceived;
            SSTool.OnHudEnabled -= OnHudEnabledEventReceived;
            SSTool.OnSSTaken -= OnSSTakenEventReceived;
            SSTool.OnTotalShots -= OnTotalShotsEventReceived;
            SSTool.OnResetIndex -= OnResetIndexEventReceived;

            SSButton.OnImageEnlarge -= OnImageEnlargeEventReceived;
        }

        void OnDestroy()
        {
            SSTool.OnCamEnabled -= OnCamEnabledEventReceived;
            SSTool.OnHudEnabled -= OnHudEnabledEventReceived;
            SSTool.OnSSTaken -= OnSSTakenEventReceived;
            SSTool.OnTotalShots -= OnTotalShotsEventReceived;
            SSTool.OnResetIndex -= OnResetIndexEventReceived;

            SSButton.OnImageEnlarge -= OnImageEnlargeEventReceived;
        }
        #endregion

        void Update()
        {
            if (Input.GetKeyDown(ssInvenKey))
                OpenSSInven();
        }
        #endregion

        #region My Functions

        #region Screenshot Inven Panel
        /// <summary>
        /// Tied to SS_Back_Button Button;
        /// Just disables the Screenshot inventory panel and enables the hud panel
        /// </summary>
        public void OnClick_SSBack()
        {
            gmData.ChangeGameState("Game");

            hudPanel.SetActive(true);
            sSInventoryPanel.SetActive(false);
            OnHudEnabled?.Invoke(true);
        }

        /// <summary>
        /// Button tied to Delete_All_SS_Button Button;
        /// Deletes all the screenshots in the inventory;
        /// </summary>
        public void OnClick_SSDeleteAll()
        {
            for (int i = 0; i < SSTool.instance.ssImages.Count; i++)
                Destroy(_rawImageButtons[i].gameObject);

            SSTool.instance.ssImages.Clear();
            _rawImageButtons.Clear();
            OnDeleteAllImages?.Invoke();
            _index = 0;
        }

        /// <summary>
        /// Enables Screenshot inventory on key press;
        /// Also sets the texture on the UI depending on how many Screenshot is taken;
        /// </summary>
        void OpenSSInven()
        {
            gmData.ChangeGameState("Inventory");

            hudPanel.SetActive(false);
            camPanel.SetActive(false);
            sSInventoryPanel.SetActive(true);

            OnHudEnabled?.Invoke(false);
            OnSSInvenEnabled?.Invoke();

            for (int i = 0; i < SSTool.instance.ssImages.Count; i++)
                _rawImageButtons[i].texture = SSTool.instance.ssImages[i];
        }
        #endregion

        #region Screenshot Enlarged Panel
        /// <summary>
        /// Button tied to Enlarge_Back_Button Button;
        /// Disables enalrged GameObject panel and enables Screenshot inventory GameObject panel;
        /// </summary>
        public void OnClick_SSEnlargeBack()
        {
            sSInventoryPanel.SetActive(true);
            sSEnalrgedPanel.SetActive(false);
        }

        /// <summary>
        /// Button tied to Delete_Current_SS_Button Button;
        /// Deletes the current photo according to the current index;
        /// Sends Event to SSTool Script;
        /// </summary>
        public void OnClick_SSDeleteCurrent()
        {
            SSTool.instance.ssImages.RemoveAt(_currIndex);
            Destroy(_rawImageButtons[_currIndex].gameObject);
            _rawImageButtons.RemoveAt(_currIndex);
            OnDeleteCurrentImage?.Invoke();

            _shotIndex = SSTool.instance.ssImages.Count;
            ssCounterText.text = $"{_shotIndex} / {_totalShots}";

            OnClick_SSEnlargeBack();
            ChangeImageIndexRuntime();
        }

        /// <summary>
        /// This changes the index of all the following buttons on runtime to avoid IndexOutOfRange Error;
        /// </summary>
        void ChangeImageIndexRuntime()
        {
            _index--;
            int index = 0;

            for (int i = 0; i < _rawImageButtons.Count; i++)
            {
                _rawImageButtons[i].GetComponent<SSButton>().ssIndex = index;
                index++;
            }
        }
        #endregion

        #endregion

        #region Events
        /// <summary>
        /// Subbed to event to OnCamEnabled Event;
        /// Just enables and disables Camera panel;
        /// </summary>
        /// <param name="isCamOpened"> If true, enable Camera panel, if false, disable Camera panel; </param>
        void OnCamEnabledEventReceived(bool isCamOpened)
        {
            if (isCamOpened)
                camPanel.SetActive(true);
            else
                camPanel.SetActive(false);
        }

        /// <summary>
        /// Subbed to event to OnHudEnabled Event;
        /// Just enables and disables HUD panel;
        /// </summary>
        /// <param name="isHudEnabled"> If true, enable HUD panel, if false, disable HUD panel; </param>
        void OnHudEnabledEventReceived(bool isHudEnabled)
        {
            if (isHudEnabled)
            {
                camPanel.SetActive(true);
                hudPanel.SetActive(true);
            }
            else
            {
                camPanel.SetActive(false);
                hudPanel.SetActive(false);
            }
        }

        /// <summary>
        /// Subbed to event to OnSSTaken Event;
        /// Updates the index and UI;
        /// </summary>
        void OnSSTakenEventReceived()
        {
            _shotIndex++;
            ssCounterText.text = $"{_shotIndex} / {_totalShots}";

            GameObject rawImgObj = Instantiate(sSButtonPrefab, sSButtonPos.position, Quaternion.identity, sSButtonPos);
            rawImgObj.GetComponent<SSButton>().ssIndex = _index;
            _index++;

            _rawImageButtons.Add(rawImgObj.GetComponent<RawImage>());
        }

        /// <summary>
        /// Subbed to event to OnTotalShots Event;
        /// Intialises the total shots variable set from the SSUI Script inspector;
        /// </summary>
        /// <param name="index"></param>
        void OnTotalShotsEventReceived(int index)
        {
            _totalShots = index;
            ssCounterText.text = $"{_shotIndex} / {_totalShots}";
        }

        /// <summary>
        /// Subbed to event to OnResetIndex Event;
        /// Resets references to default on SSTool Script;
        /// </summary>
        void OnResetIndexEventReceived()
        {
            _shotIndex = 0;
            ssCounterText.text = $"{_shotIndex} / {_totalShots}";
        }

        /// <summary>
        /// Function subbed to OnImageEnlarge Event;
        /// Shows enalrged version of the image the user clicks on;
        /// </summary>
        /// <param name="texure"> Needs a texture to show the image enalrged; </param>
        /// <param name="index"> Sets the current index so that the user can delete that image; </param>
        void OnImageEnlargeEventReceived(Texture texure, int index)
        {
            sSEnalrgedPanel.SetActive(true);
            sSInventoryPanel.SetActive(false);
            enlargeRawImage.texture = texure;
            _currIndex = index;
        }
        #endregion
    }
}