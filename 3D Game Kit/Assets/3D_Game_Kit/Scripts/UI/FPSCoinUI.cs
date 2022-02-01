using UnityEngine;
using UnityEngine.Events;
using TMPro;

namespace RedMountMedia
{
    public class FPSCoinUI : MonoBehaviour
    {
        #region Serialized Variables
        [Space, Header("Coin Texts")]
        [SerializeField]
        [Tooltip("TextMeshPro for showing coin text")]
        private TextMeshProUGUI coinText = default;

        [SerializeField]
        [Tooltip("TextMeshPro for showing coin text")]
        private int maxCoinCount = default;

        [Space]
        [SerializeField]
        [Tooltip("Event on when coin reaches max Count;")]
        public UnityEvent OnMaxCountReached = default;
        #endregion

        #region Private Variables
        private int _currCoinCount = default;
        #endregion

        #region Unity Callbacks

        #region Events
        void OnEnable() => PickableCoin.OnCoinIncrement += OnCoinIncrementEventReceived;

        void OnDisable() => PickableCoin.OnCoinIncrement -= OnCoinIncrementEventReceived;

        void OnDestroy() => PickableCoin.OnCoinIncrement -= OnCoinIncrementEventReceived;
        #endregion

        void Start() => coinText.text = $"{_currCoinCount} / {maxCoinCount}";
        #endregion

        #region Events
        void OnCoinIncrementEventReceived(int value)
        {
            _currCoinCount += value;
            coinText.text = $"{_currCoinCount} / {maxCoinCount}";

            if (_currCoinCount >= maxCoinCount)
                OnMaxCountReached?.Invoke();
        }
        #endregion
    }
}