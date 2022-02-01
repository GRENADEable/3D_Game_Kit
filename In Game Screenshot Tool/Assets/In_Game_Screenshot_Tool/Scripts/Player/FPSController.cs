using UnityEngine;

namespace RedMountMedia
{
    public class FPSController : MonoBehaviour
    {
        #region Serialized Variables
        [Space, Header("Data")]
        [SerializeField]
        [Tooltip("GameManager Scriptable Object")]
        private GameMangerData gmData;

        [SerializeField]
        [Tooltip("Can it depend on the GameManager Scriptable Object? Set this to true if you want to use the GmData Scriptable Object")]
        private bool isUsingScriptableObject;

        #region Input
        [Space, Header("Player Keys")]
        [SerializeField]
        [Tooltip("Which key to press when running")]
        private KeyCode runKey = KeyCode.LeftShift;

        [SerializeField]
        [Tooltip("Which key to press when jumping")]
        private KeyCode jumpKey = KeyCode.Space;
        #endregion

        #region Player Movement
        [Space, Header("Player Variables")]
        [SerializeField]
        [Tooltip("Walk speed of the player")]
        private float playerWalkSpeed = 3f;

        [SerializeField]
        [Tooltip("Run speed of the player")]
        private float playerRunSpeed = 6f;

        [SerializeField]
        [Tooltip("Gravity of the player when falling")]
        private float gravity = -19.62f;
        #endregion

        #region Player Jump
        [Space, Header("Jump Variables")]
        [SerializeField]
        [Tooltip("Can the player Jump?")]
        private bool canJump = true;

        [SerializeField]
        [Tooltip("Power of how high the player can jump")]
        private float jumpPower = 2f;
        #endregion

        #region Player Grounding
        [Space, Header("Ground Check")]
        [SerializeField]
        [Tooltip("Transform Component for checking the ground")]
        private Transform groundCheck;

        [SerializeField]
        [Tooltip("Spherecast radius for the ground")]
        private float groundDistance = 0.4f;

        [SerializeField]
        [Tooltip("Which layer(s) is used for the ground")]
        private LayerMask groundMask;
        #endregion

        #endregion

        #region Private Variables

        #region Player Movement
        [Header("Player Variables")]
        private CharacterController _charControl;
        private Vector3 _moveDirection;
        private Vector3 _vel;
        private float _currSpeed;
        private bool _isRunning;
        #endregion

        #region Player Grounding
        [Header("Ground Check")]
        private bool _isGrounded;
        private bool _isJumping;
        #endregion

        #endregion

        #region Unity Callbacks
        void Start()
        {
            _charControl = GetComponent<CharacterController>();

            _currSpeed = playerWalkSpeed;
        }

        void Update()
        {
            GroundCheck();

            if (isUsingScriptableObject)
            {
                if (gmData.currState == GameMangerData.GameState.Game)
                {
                    InputChecks();
                    PlayerCurrStance();
                    PlayerMovement();
                }
            }
            else
            {
                InputChecks();
                PlayerCurrStance();
                PlayerMovement();
            }
        }
        #endregion

        #region My Functions

        #region Checks
        /// <summary>
        /// Checks what input is pressed;
        /// </summary>
        void InputChecks()
        {
            if (Input.GetKey(runKey))
                _isRunning = true;
            else
                _isRunning = false;

            if (Input.GetKeyDown(jumpKey) && canJump)
                PlayerJump();
        }

        /// <summary>
        /// Ground check for gavity and jumping;
        /// </summary>
        void GroundCheck()
        {
            _isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            if (_isGrounded && _vel.y < 0)
            {
                _isJumping = false;
                _vel.y = -2f;
            }

            _vel.y += gravity * Time.deltaTime;
            _charControl.Move(_vel * Time.deltaTime);
        }
        #endregion

        #region Player Movement
        /// <summary>
        /// This is where the player movement takes place;
        /// </summary>
        void PlayerMovement()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            _moveDirection = (transform.right * horizontal + transform.forward * vertical).normalized;
            _charControl.Move(_currSpeed * Time.deltaTime * _moveDirection);
        }

        /// <summary>
        /// Checks stance if the player is Running or Crouching;
        /// </summary>
        void PlayerCurrStance()
        {
            if (_isRunning) // Run
            {
                _currSpeed = playerRunSpeed;
                //Debug.Log("Running");
            }
            else // Walk
            {
                _currSpeed = playerWalkSpeed;
                //Debug.Log("Walking");
            }
        }

        /// <summary>
        /// Jump's player;
        /// </summary>
        void PlayerJump()
        {
            if (isUsingScriptableObject)
            {
                if (_isGrounded && !_isJumping && gmData.currState == GameMangerData.GameState.Game)
                {
                    float jumpForce = Mathf.Sqrt(jumpPower * Mathf.Abs(gravity) * 2);
                    _vel.y += jumpForce;
                    _isJumping = true;
                }
            }
            else
            {
                if (_isGrounded && !_isJumping)
                {
                    float jumpForce = Mathf.Sqrt(jumpPower * Mathf.Abs(gravity) * 2);
                    _vel.y += jumpForce;
                    _isJumping = true;
                }
            }
        }
        #endregion

        #endregion
    }
}