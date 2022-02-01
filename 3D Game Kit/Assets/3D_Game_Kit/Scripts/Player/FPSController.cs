using UnityEngine;

namespace RedMountMedia
{
    public class FPSController : MonoBehaviour
    {
        #region Serialized Variables
        [Space, Header("Data")]
        [SerializeField]
        [Tooltip("GameManager Scriptable Object")]
        private GameMangerData gmData = default;

        [SerializeField]
        [Tooltip("Can it depend on the GameManager Scriptable Object? Set this to true if you want to use the GmData Scriptable Object")]
        private bool isUsingScriptableObject = default;

        #region Input
        [Space, Header("Player Keys")]
        [SerializeField]
        [Tooltip("Which key to press when running")]
        private KeyCode runKey = KeyCode.LeftShift;

        [SerializeField]
        [Tooltip("Which key to press when jumping")]
        private KeyCode jumpKey = KeyCode.Space;

        [SerializeField]
        [Tooltip("Which key to press when crouching")]
        private KeyCode crouchKey = KeyCode.C;

        [SerializeField]
        [Tooltip("Which alternate key to press when crouching")]
        private KeyCode crouchAlternateKey = KeyCode.LeftControl;
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

        [SerializeField]
        [Tooltip("Can the player push the GameObjects")]
        private bool canPush = default;

        [SerializeField]
        [Tooltip("Power of how much rigidbody GameObjects are affected when pushed")]
        private float pushPower = 2f;

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

        #region Player Crouch
        [Space, Header("Crouch Variables")]
        [SerializeField]
        [Tooltip("Height of the crouch ray shooting upwards")]
        private Vector3 rayHeight = default;

        [SerializeField]
        [Tooltip("Ray Distance of the crouch")]
        private float rayRoofDistance = 1f;

        [SerializeField]
        [Tooltip("Crouch speed of the player")]
        private float crouchWalkSpeed = 0.5f;

        [SerializeField]
        [Tooltip("How much the CharacterController Collider shrinks when crouched")]
        private float crouchColShrinkValue = 0.7f; //Initial Value is 0.7f

        [SerializeField]
        [Tooltip("Where is the center of the CharacterController Collider")]
        private float crouchColCenterValue = 0.5f; //Initial Value is 0.5f

        #endregion

        #region Player Grounding
        [Space, Header("Ground Check")]
        [SerializeField]
        [Tooltip("Transform Component for checking the ground")]
        private Transform groundCheck = default;

        [SerializeField]
        [Tooltip("Transform Component for checking the ground when crouched")]
        private Transform groundCheckCrouch = default;

        [SerializeField]
        [Tooltip("Spherecast radius for the ground")]
        private float groundDistance = 0.4f;

        [SerializeField]
        [Tooltip("Which layer(s) is used for the ground")]
        private LayerMask groundMask = default;
        #endregion

        #endregion

        #region Private Variables

        #region Player Movement
        [Header("Player Variables")]
        private CharacterController _charControl = default;
        private Vector3 _moveDirection = default;
        private Vector3 _vel = default;
        private float _currSpeed = default;
        private bool _isRunning = default;
        #endregion

        #region Player Crouch
        [Header("Crouch Variables")]
        private bool _isCrouching = default;
        private float _playerHeight = default;
        private float _playerCenter = default;
        private bool _isHittingRoof = default;
        #endregion

        #region Player Grounding
        [Header("Ground Check")]
        private bool _isGrounded = default;
        private bool _isJumping = default;
        #endregion

        #endregion

        #region Unity Callbacks
        void Start()
        {
            _charControl = GetComponent<CharacterController>();

            _currSpeed = playerWalkSpeed;
            _playerHeight = _charControl.height;
            _playerCenter = _charControl.center.y;
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

        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (canPush)
            {
                Rigidbody rB = hit.collider.attachedRigidbody;

                if (rB == null || rB.isKinematic)
                    return;

                Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
                rB.velocity = pushDir * pushPower;
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
            if (Input.GetKey(runKey) && !_isCrouching)
                _isRunning = true;
            else
                _isRunning = false;

            if (Input.GetKey(crouchKey) || Input.GetKey(crouchAlternateKey) && !_isRunning)
                _isCrouching = true;
            else if (!_isHittingRoof)
                _isCrouching = false;

            if (Input.GetKeyDown(jumpKey) && canJump)
                PlayerJump();
        }

        /// <summary>
        /// Ground check for gavity and jumping;
        /// </summary>
        void GroundCheck()
        {
            if (!_isCrouching)
                _isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
            else
                _isGrounded = Physics.CheckSphere(groundCheckCrouch.position, groundDistance, groundMask);


            if (_isGrounded && _vel.y < 0)
            {
                _isJumping = false;
                _vel.y = -2f;
            }

            _vel.y += gravity * Time.deltaTime;
            _charControl.Move(_vel * Time.deltaTime);

            //if (_vel.y < 0)
            //    _vel.y += gravity * (fallMulti - 1) * Time.deltaTime;
        }

        /// <summary>
        /// Crouch check so the player doesn't get stuck when they stop crouching;
        /// </summary>
        void CheckCrouch()
        {
            Ray ray = new Ray(transform.position + rayHeight, transform.forward);

            _isHittingRoof = Physics.Raycast(ray.origin, Vector3.up, rayRoofDistance);
            Debug.DrawRay(ray.origin, Vector3.up * rayRoofDistance, _isHittingRoof ? Color.green : Color.red);

            if (_isHittingRoof)
                _isCrouching = true;
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

            //if (moveDirection != Vector3.zero)
            //    playerFootStepAnim.SetBool("isMoving", true);
            //else
            //    playerFootStepAnim.SetBool("isMoving", false);
        }

        /// <summary>
        /// Checks stance if the player is Running or Crouching;
        /// </summary>
        void PlayerCurrStance()
        {
            float localHeight = _playerHeight;
            float localCenter = _playerCenter;

            if (_isRunning) // Run
            {
                _currSpeed = playerRunSpeed;
                //Debug.Log("Running");
            }
            else if (_isCrouching) // Crouch
            {
                localHeight = _playerHeight * crouchColShrinkValue;
                localCenter = _playerCenter / crouchColCenterValue;
                _currSpeed = crouchWalkSpeed;
                //Debug.Log("Crouching");
                CheckCrouch();
            }
            else // Walk
            {
                _currSpeed = playerWalkSpeed;
                //Debug.Log("Walking");
            }

            _charControl.height = Mathf.Lerp(_charControl.height, localHeight, 5 * Time.deltaTime);
            _charControl.center = new Vector3(0, Mathf.Lerp(_charControl.center.y, localCenter, 5 * Time.deltaTime), 0);
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