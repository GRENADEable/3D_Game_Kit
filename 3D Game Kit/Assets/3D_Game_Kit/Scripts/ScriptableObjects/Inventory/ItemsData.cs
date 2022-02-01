using UnityEngine;

namespace RedMountMedia
{
    public enum ItemType { Default, Coin };
    [CreateAssetMenu(fileName = "Item_Data", menuName = "Inventory/ItemData")]
    public class ItemsData : ScriptableObject
    {
        [Tooltip("What item catagory is this?")]
        public ItemType currItem = ItemType.Default;

        [Tooltip("Item Name")]
        public string itemName = "New Item";

        [Tooltip("Item's sprite Icon")]
        public Sprite icon = null;

        [Tooltip("Item GameObject")]
        public GameObject itemObj;

        [HideInInspector] public bool isIntialised;
        [HideInInspector] public Vector3 intialPos;
        [HideInInspector] public Quaternion intialRot;

        [Tooltip("Coin")]
        public int coinIncrement;

        /// <summary>
        /// Use the item the user clicks on;
        /// </summary>
        public virtual void UseItem()
        {
            if (currItem == ItemType.Default)
                Debug.Log("Using " + name);
        }
    }
}