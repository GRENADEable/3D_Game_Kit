using UnityEngine;
using UnityEngine.UI;

namespace RedMountMedia
{
    public class InventorySlot : MonoBehaviour
    {
        #region Public Variables
        [Tooltip("InventoryData Scriptable Object")]
        public InventoryData invenData;

        [Tooltip("Icon reference on the Button")]
        public Image icon;

        [Tooltip("Remove button reference on the Button")]
        public Button removeButton;

        #region Events
        public delegate void SendEventsObj(GameObject obj);
        /// <summary>
        /// Event sent from InventorySlot to FPSInventoryUI Script;
        /// This event just sends the prefab that the player removes when pressing the remove button from the inventory UI;
        /// </summary>
        public static event SendEventsObj OnItemRemoved;
        #endregion

        #endregion

        #region Private Variables
        private ItemsData item = default;
        #endregion

        #region My Functions
        /// <summary>
        /// Added to Use Button on Inventory Button UI;
        /// </summary>
        public void OnClick_UseButton()
        {
            if (item != null)
                item.UseItem();
        }

        /// <summary>
        /// Added to Remove Button on Inventory Button UI;
        /// </summary>
        public void OnClick_RemoveButton()
        {
            OnItemRemoved?.Invoke(item.itemObj);
            invenData.RemoveItem(item);
            //Debug.Log("Remove Button Pressed");
        }

        /// <summary>
        /// Updates UI Button wit the appropiate sprite picked up from the ground;
        /// </summary>
        /// <param name="newItem"> Needs picked up ItemData; </param>
        public void AddItem(ItemsData newItem)
        {
            item = newItem;

            icon.sprite = item.icon;
            icon.enabled = true;
            removeButton.interactable = true;
            //Debug.Log("Item Added");
        }

        /// <summary>
        /// Clears all the referene in the UI;
        /// </summary>
        public void ClearSlot()
        {
            item = null;

            icon.sprite = null;
            icon.enabled = false;
            removeButton.interactable = false;

            //Debug.Log("Slot Cleared");
        }
        #endregion
    }
}