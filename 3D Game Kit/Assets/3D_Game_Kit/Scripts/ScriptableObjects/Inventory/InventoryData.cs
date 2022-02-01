using System.Collections.Generic;
using UnityEngine;

namespace RedMountMedia
{
    [CreateAssetMenu(fileName = "Inventory_Data", menuName = "Inventory/InventoryData")]
    public class InventoryData : ScriptableObject
    {
        [Tooltip("How much space do you want in the inventory? Update by adding more inventory slots in the UI")]
        public int inventorySpace = 14;

        [Tooltip("List of what items are there in the inventory")]
        public List<ItemsData> itemsList = new List<ItemsData>();

        #region Events
        public delegate void SendEvents();
        /// <summary>
        /// Event sent from InventoryData to GameManager Script;
        /// This event just updates the UI in the inventroy;
        /// </summary>
        public static event SendEvents OnItemChanged;
        #endregion

        #region My Functions
        /// <summary>
        /// Adds the item in the invenetory;
        /// </summary>
        /// <param name="_item"> Which item in the ItemData; </param>
        /// <returns> If true, add item in the inventory, if false, no space in inventory; </returns>
        public bool AddItem(ItemsData _item)
        {
            if (itemsList.Count >= inventorySpace)
            {
                Debug.Log("No Inventory Space");
                return false;
            }

            itemsList.Add(_item);
            OnItemChanged?.Invoke(); // Event sent to GameManager and GameManagerTest scripts

            //Debug.Log("Item Picked up " + _item.name);
            return true;
        }

        /// <summary>
        /// Removes the item in the invenetory;
        /// </summary>
        /// <param name="_item"> Which item in the ItemData; </param>
        public void RemoveItem(ItemsData _item)
        {
            itemsList.Remove(_item);

            OnItemChanged?.Invoke(); // Event sent to GameManager and GameManagerTest scripts
        }
        #endregion
    }
}