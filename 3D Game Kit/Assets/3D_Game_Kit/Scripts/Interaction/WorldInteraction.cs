using UnityEngine;
using UnityEngine.Events;

public class WorldInteraction : MonoBehaviour
{
    #region Serialized Variables
    [Space, Header("Event Variables")]
    [SerializeField]
    [Tooltip("How many times can it be triggered, Keep it 0 to be toggled multiple times. Can't be changed on runtime")]
    private int triggerCount = default;

    [SerializeField]
    [Tooltip("Keep this bool check according to what is the intial state of the prop")]
    private bool isPropOn = default;

    [Space, Header("Events")]
    [SerializeField]
    [Tooltip("Event for triggered on Prop")]
    private UnityEvent OnPropOnEvent = default;

    [SerializeField]
    [Tooltip("Event for triggered off Prop")]
    private UnityEvent OnPropOffEvent = default;
    #endregion

    #region Private Variables
    private int _currTrigger = default;
    #endregion

    #region My Functions
    public void InteractPropRaycast()
    {
        if (triggerCount == 0 || (_currTrigger < triggerCount))
        {
            isPropOn = !isPropOn;

            if (isPropOn)
                OnPropOnEvent?.Invoke();
            else
                OnPropOffEvent?.Invoke();

            _currTrigger++;
        }
    }
    #endregion
}