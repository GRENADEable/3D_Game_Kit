using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace RedMountMedia
{
    public class FPSDefaultUI : MonoBehaviour
    {
        #region Serialized Variables
        [Space, Header("HUD Panel")]
        [SerializeField]
        [Tooltip("Fade panel Animation Component")]
        private Animator fadeBG = default;

        [SerializeField]
        [Tooltip("HUD panel GameObject Component")]
        private GameObject hudPanel = default;

        [Space, Header("Reticle")]
        [SerializeField]
        [Tooltip("Reticle Image")]
        private Image reticleImg = default;

        [SerializeField]
        [Tooltip("Reticle Sprite when zoomed out. This can be left empty")]
        private Sprite reticleZoomOutSprite = default;

        [SerializeField]
        [Tooltip("Reticle Sprite when zoomed in. This can be left empty")]
        private Sprite reticleZoomInSprite = default;

        [SerializeField]
        [Tooltip("Reticle Color when zoomed out")]
        private Color reticleZoomOutColor = Color.white;

        [SerializeField]
        [Tooltip("Reticle Color when zoomed in")]
        private Color reticleZoomInColor = Color.white;
        #endregion

        #region Unity Callbacks

        #region Events
        void OnEnable()
        {
            FPSManager.OnFadePanel += OnFadePanelEventReceived;

            ExamineSystem.OnHudEnabled += OnHudEnabledEventRecieved;

            FPSInventoryUI.OnHudEnabled += OnHudEnabledEventRecieved;

            PlayerInventory.OnItemAdded += OnItemAddedEventReceived;

            PlayerZoom.OnZoomInCam += OnZoomInCamEventReceieved;

            MapTransition.OnSceneTrigger += OnSceneTriggerEventReceived;
        }

        void OnDisable()
        {
            FPSManager.OnFadePanel -= OnFadePanelEventReceived;

            ExamineSystem.OnHudEnabled -= OnHudEnabledEventRecieved;

            FPSInventoryUI.OnHudEnabled -= OnHudEnabledEventRecieved;

            PlayerInventory.OnItemAdded -= OnItemAddedEventReceived;

            PlayerZoom.OnZoomInCam -= OnZoomInCamEventReceieved;

            MapTransition.OnSceneTrigger -= OnSceneTriggerEventReceived;
        }

        void OnDestroy()
        {
            FPSManager.OnFadePanel -= OnFadePanelEventReceived;

            ExamineSystem.OnHudEnabled -= OnHudEnabledEventRecieved;

            FPSInventoryUI.OnHudEnabled -= OnHudEnabledEventRecieved;

            PlayerInventory.OnItemAdded -= OnItemAddedEventReceived;

            PlayerZoom.OnZoomInCam -= OnZoomInCamEventReceieved;

            MapTransition.OnSceneTrigger -= OnSceneTriggerEventReceived;
        }
        #endregion

        #endregion

        #region My Functions
        /// <summary>
        /// Changes reticle according to bool;
        /// </summary>
        /// <param name="zoom"> If true, change to zoom in reticle settings. If false, chagne to zoom out reticle settings; </param>
        void IsZoomingReticle(bool zoom)
        {
            if (zoom)
            {
                reticleImg.color = reticleZoomInColor;

                if (reticleZoomInSprite != null)
                    reticleImg.sprite = reticleZoomInSprite;
            }
            else
            {
                reticleImg.color = reticleZoomOutColor;

                if (reticleZoomOutSprite != null)
                    reticleImg.sprite = reticleZoomOutSprite;
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Subbed to event from FPSManager. This event enabled the fade panel and plays the FadeIn Animation;
        /// </summary>
        void OnFadePanelEventReceived()
        {
            fadeBG.gameObject.SetActive(true);
            fadeBG.Play("FadeIn");
        }

        /// <summary>
        /// Subbed to event from ExamineSystem and FPSInventoryUI. This event enables and disables the hudPanel;
        /// </summary>
        /// <param name="isHudEnabled"> True to enable HUD, false to disable HUD; </param>
        void OnHudEnabledEventRecieved(bool isHudEnabled)
        {
            if (isHudEnabled)
                hudPanel.SetActive(true);
            else
                hudPanel.SetActive(false);
        }

        /// <summary>
        /// Subbed to event from the InventoryData. This event sets the HUD to true when player puts item in the bag;
        /// </summary>
        void OnItemAddedEventReceived() => hudPanel.SetActive(true);

        /// <summary>
        /// Subbed to event from 
        /// </summary>
        void OnZoomInCamEventReceieved(bool isZooming)
        {
            if (isZooming)
                IsZoomingReticle(true);
            else
                IsZoomingReticle(false);
        }

        /// <summary>
        /// Subbed to event from MapTransition. This event just fades out and starts the given map;
        /// </summary>
        /// <param name="index"> Integer value for the scene index; </param>
        void OnSceneTriggerEventReceived(int index) => StartCoroutine(TrasnitionDelay(index));
        #endregion

        #region Coroutines
        IEnumerator TrasnitionDelay(int sceneIndex)
        {
            fadeBG.Play("FadeOut");
            yield return new WaitForSeconds(1f);
            Application.LoadLevel(sceneIndex);
        }
        #endregion
    }
}