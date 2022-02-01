using UnityEngine;
using UnityEngine.UI;

namespace RedMountMedia
{
    public class SSButton : MonoBehaviour
    {
        #region Public Variables
        public delegate void SendEventImage(Texture img, int index);
        /// <summary>
        /// Event sent from SSButton to GameManager Script;
        /// This event enlarges image panel and sets the texture of the image you clicked on;
        /// </summary>
        public static event SendEventImage OnImageEnlarge;
        [HideInInspector] public int ssIndex;
        #endregion

        #region Private Variables
        private RawImage _image;
        #endregion

        #region Unity Callbacks
        void Start() => _image = GetComponent<RawImage>();
        #endregion

        #region My Functions
        /// <summary>
        /// Button tied to Inventory_SS_Button Button;
        /// This just enarlges the image;
        /// </summary>
        public void OnClick_EnlargeImage() => OnImageEnlarge?.Invoke(_image.texture, ssIndex);
        #endregion
    }
}