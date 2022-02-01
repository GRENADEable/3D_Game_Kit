using UnityEngine;
using UnityEditor;

namespace RedMountMedia
{
    [CustomEditor(typeof(ItemsData))]
    public class ItemEditor : Editor
    {
        #region Unity Callbacks
        public override void OnInspectorGUI()
        {
            ItemsData item = (ItemsData)target;

            item.currItem = (ItemType)EditorGUILayout.EnumPopup("Select Item", item.currItem);

            if (item.currItem == ItemType.Default)
            {
                item.itemName = EditorGUILayout.TextField("Item Name", item.itemName);
                item.icon = (Sprite)EditorGUILayout.ObjectField("Item Icon ", item.icon, typeof(Sprite), true);
                item.itemObj = (GameObject)EditorGUILayout.ObjectField("Item Object", item.itemObj, typeof(GameObject), true);
            }

            if (item.currItem == ItemType.Coin)
                item.coinIncrement = EditorGUILayout.IntField("Coin Increment", item.coinIncrement);    

            if (GUI.changed)
            {
                Undo.RecordObject(target, "ItemSave");
                EditorUtility.SetDirty(item);
            }
        }
        #endregion
    }
}