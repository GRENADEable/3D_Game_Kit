using UnityEngine;

namespace RedMountMedia
{
    [CreateAssetMenu(fileName = "GameManager_Data", menuName = "Managers/GameManagerData")]
    public class GameMangerData : ScriptableObject
    {
        #region Public Variables
        [Space, Header("Enums")]
        public GameState currState = GameState.Game;
        public enum GameState { Intro, Game, Inventory };
        #endregion

        #region My Functions

        #region Cursor
        /// <summary>
        /// Locks the user's cusor;
        /// </summary>
        /// <param name="isLocked"> If true, lock the cursor in place, if false, free the cursor; </param>
        public void LockCursor(bool isLocked)
        {
            if (isLocked)
                Cursor.lockState = CursorLockMode.Locked;
            else
                Cursor.lockState = CursorLockMode.None;
        }

        /// <summary>
        /// User's cursor visibility;
        /// </summary>
        /// <param name="isVisible"> If true, lock show cursor, if false, hide cursor; </param>
        public void VisibleCursor(bool isVisible)
        {
            if (isVisible)
                Cursor.visible = true;
            else
                Cursor.visible = false;
        }
        #endregion

        #region Game States
        /// <summary>
        /// Changes the state the game is running on using enums;
        /// </summary>
        /// <param name="state"> Uses string to check which state to change to. The string has to be exact or else it won't work; </param>
        public void ChangeGameState(string state)
        {
            if (state.Contains("Intro"))
                currState = GameState.Intro;

            if (state.Contains("Game"))
                currState = GameState.Game;

            if (state.Contains("Inventory"))
                currState = GameState.Inventory;
        }
        #endregion

        #endregion
    }
}