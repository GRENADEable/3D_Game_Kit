using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTransition : MonoBehaviour
{
    #region Serialized Variables
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private int sceneIndex = default;

    public delegate void SendEventsInt(int index);
    public static event SendEventsInt OnSceneTrigger;
    #endregion

    #region Unity Callbacks
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
            OnSceneTrigger?.Invoke(sceneIndex);
    }
    #endregion

}