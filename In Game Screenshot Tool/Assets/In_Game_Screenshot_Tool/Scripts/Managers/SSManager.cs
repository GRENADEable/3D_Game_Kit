using System.Collections;
using UnityEngine;

namespace RedMountMedia
{
    public class SSManager : MonoBehaviour
    {
        #region Serialized Variables
        [Space, Header("Data")]
        [SerializeField]
        [Tooltip("GameManager Scriptable Object")]
        private GameMangerData gmData;

        #region Intialisation
        [Space, Header("Intialisation")]
        [SerializeField]
        [Tooltip("Must have player prefab for Player Movement")]
        private GameObject fpsPlayer;

        [SerializeField]
        [Tooltip("Must have canvas prefab for UI")]
        private GameObject fpsCanvas;
        #endregion

        [Space, Header("Transform References")]
        [SerializeField]
        [Tooltip("Where the player will start. Can be left empty")]
        private Transform fpsPlayerStart;
        #endregion

        #region Unity Callbacks

        #region Events
        void OnEnable() => SSUI.OnHudEnabled += OnHudEnabledEventReceived;

        void OnDisable() => SSUI.OnHudEnabled -= OnHudEnabledEventReceived;

        void OnDestroy() => SSUI.OnHudEnabled -= OnHudEnabledEventReceived;
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
            if (!fpsPlayer || !fpsCanvas)
            {
                Debug.LogError("Add missing prefabs in FPSUIManager");
                Debug.Break();
                return;
            }

            Instantiate(fpsPlayer, GetStartPos(), GetStartRot(), transform);
            Instantiate(fpsCanvas, transform.position, Quaternion.identity, transform);

            gmData.ChangeGameState("Game");
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
        /// Subbed to event from SSUI script;
        /// Event just enables and disables cursor;
        /// </summary>
        /// <param name="isHudEnabled"> If true, enable cursor, if false, disable cursor; </param>
        void OnHudEnabledEventReceived(bool isHudEnabled)
        {
            if (isHudEnabled)
                DisableCursor();
            else
                EnableCursor();
        }
        #endregion
    }
}