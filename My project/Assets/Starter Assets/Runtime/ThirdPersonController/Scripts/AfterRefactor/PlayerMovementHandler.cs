using UnityEngine;
namespace StarterAssets
{
    public class PlayerMovementHandler : MonoBehaviour
    {

        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 2.0f;
        [SerializeField] private float sprintSpeed = 5.335f;
        [SerializeField] private float rotationSmoothTime = 0.12f;
        [SerializeField] private float speedChangeRate = 10.0f;
        [Header("Jump and Gravity")]
        [SerializeField] private float jumpHeight = 1.2f;
        [SerializeField] private float gravity = -15.0f;
        [SerializeField] private float jumpTimeout = 0.50f;
        [SerializeField] private float fallTimeout = 0.15f;
        [SerializeField] private float terminalVelocity = 53.0f;

        private PlayerInputHandler _inputHandler;
        private CharacterController _controller;

        private float _verticalVelocity;
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;
        private Transform _cameraTransform;
        private PlayerAnimationHandler _animationHandler;

        private float _speed;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;

        [SerializeField] private float groundedOffset = -0.14f;
        [SerializeField] private float groundedRadius = 0.28f;
        [SerializeField] private LayerMask groundLayers;
        public bool IsGrounded { get; private set; }

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _inputHandler = GetComponent<PlayerInputHandler>();
            _animationHandler = GetComponent<PlayerAnimationHandler>();
            _cameraTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;

            ResetTimeouts();
        }

        public void ProcessMovement()
        {
            GroundedCheck();
            JumpAndGravity();
            Move();
        }

        private void Move()
        {
            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = _inputHandler.SprintInput ? sprintSpeed : moveSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_inputHandler.MoveInput == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _inputHandler.IsAnalogMovement ? _inputHandler.MoveInput.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * speedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            // normalise input direction
            Vector3 inputDirection = new Vector3(_inputHandler.MoveInput.x, 0.0f, _inputHandler.MoveInput.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_inputHandler.MoveInput != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _cameraTransform.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    rotationSmoothTime);

                // rotate to face input direction relative to camera position
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }


            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            // move the player
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            _animationHandler?.UpdateMovementAnimation(_speed, inputMagnitude);
        }

        private void ResetTimeouts()
        {
            _jumpTimeoutDelta = jumpTimeout;
            _fallTimeoutDelta = fallTimeout;
        }
        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset,
                transform.position.z);
            IsGrounded = Physics.CheckSphere(spherePosition, groundedRadius, groundLayers,
                QueryTriggerInteraction.Ignore);

            _animationHandler.SetGroundedState(IsGrounded);
        }

        private void JumpAndGravity()
        {
            if (IsGrounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = fallTimeout;
                _animationHandler.SetJumpState(false);
                _animationHandler.SetFreeFallState(false);
                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (_inputHandler.JumpInput && _jumpTimeoutDelta <= 0.0f)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

                    _animationHandler.SetJumpState(true);
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = jumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    _animationHandler.SetFreeFallState(true);
                }
                _inputHandler.JumpComplete();
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < terminalVelocity)
            {
                _verticalVelocity += gravity * Time.deltaTime;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            Gizmos.color = IsGrounded ? transparentGreen : transparentRed;

            Vector3 position = transform.position;
            Gizmos.DrawSphere(
                new Vector3(position.x, position.y - groundedOffset, position.z),
                groundedRadius);
        }
    }
}
