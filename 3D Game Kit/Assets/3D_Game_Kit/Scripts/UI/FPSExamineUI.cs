using UnityEngine;
using TMPro;

namespace RedMountMedia
{
    public class FPSExamineUI : MonoBehaviour
    {
        #region Serialized Variables
        [SerializeField]
        [Tooltip("Examine panel GameObject")]
        private GameObject examinePanel = default;

        [SerializeField]
        [Tooltip("Text to show item description")]
        private TextMeshProUGUI itemText = default;
        #endregion

        #region Unity Callbacks

        #region Events
        void OnEnable()
        {
            ExamineSystem.OnExamineHudEnabled += OnExamineHudEnabledEventRecieved;
            ExamineSystem.OnItemDescription += OnItemDescriptionEventReceived;

            PlayerInventory.OnItemAdded += OnItemAddedEventReceieved;
        }

        void OnDisable()
        {
            ExamineSystem.OnExamineHudEnabled -= OnExamineHudEnabledEventRecieved;
            ExamineSystem.OnItemDescription -= OnItemDescriptionEventReceived;

            PlayerInventory.OnItemAdded -= OnItemAddedEventReceieved;
        }

        void OnDestroy()
        {
            ExamineSystem.OnExamineHudEnabled -= OnExamineHudEnabledEventRecieved;
            ExamineSystem.OnItemDescription -= OnItemDescriptionEventReceived;

            PlayerInventory.OnItemAdded -= OnItemAddedEventReceieved;
        }
        #endregion

        #endregion

        #region Events
        /// <summary>
        /// Subbed to event from ExamineSyste. This event just enabled and disabled the examine panel GameObject;
        /// </summary>
        /// <param name="isExamineEnabled"></param>
        void OnExamineHudEnabledEventRecieved(bool isExamineEnabled)
        {
            if (isExamineEnabled)
                examinePanel.SetActive(true);
            else
                examinePanel.SetActive(false);
        }

        /// <summary>
        /// Subbed to event from ExamineSystem. This event just intialises the references on runtime;
        /// </summary>
        /// <param name="text"> Must have string value; </param>
        void OnItemDescriptionEventReceived(PickableItems pick)
        {
            itemText.text = pick.itemDescription;
            itemText.color = pick.itemDescriptionTextColor;
            itemText.fontSize = pick.itemDescriptionTextSize;
        }

        void OnItemAddedEventReceieved() => examinePanel.SetActive(false);
        #endregion
    }
}