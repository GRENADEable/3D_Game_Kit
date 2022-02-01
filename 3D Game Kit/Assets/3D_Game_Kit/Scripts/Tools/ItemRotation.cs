using UnityEngine;

public class ItemRotation : MonoBehaviour
{
    #region Serialized Variables
    [SerializeField]
    [Tooltip("Object Rotation speed")]
    private float speed = 5f;
    #endregion

    #region Private Variables
    [Tooltip("Bool for Rotating item")]
    [SerializeField] private bool _isRotating;
    #endregion

    #region Unity Callbacks
    void Update()
    {
        if (_isRotating)
            transform.RotateAround(transform.position, transform.up, speed * Time.deltaTime);
    }
    #endregion

    #region My Functions
    /// <summary>
    /// Tied to Button or Unity Event;
    /// This just toggls the booleon;
    /// </summary>
    public void OnToggleSpin() => _isRotating = !_isRotating;
    #endregion
}