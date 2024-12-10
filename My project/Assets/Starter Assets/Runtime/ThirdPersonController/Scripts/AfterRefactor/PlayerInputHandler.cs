// 1. Input Handler
using UnityEngine;
using UnityEngine.InputSystem;

namespace StarterAssets
{
    public class PlayerInputHandler : MonoBehaviour
    {
        private PlayerInput _playerInput;
        private StarterAssetsInputs _input;

        public Vector2 MoveInput => _input.move;
        public Vector2 LookInput => _input.look;
        public bool SprintInput => _input.sprint;
        public bool JumpInput => _input.jump;
        public bool IsAnalogMovement => _input.analogMovement;

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
            _input = GetComponent<StarterAssetsInputs>();
        }

        public bool IsUsingMouse => _playerInput.currentControlScheme == "KeyboardMouse";

        public void JumpComplete()
        {
            _input.jump = false;
        }
    }

}

    // // 8. Main Controller (Orchestrator)
    // public class ThirdPersonController : MonoBehaviour
    // {
    //     private PlayerInputHandler _inputHandler;
    //     private PlayerMovementController _movementController;
    //     private PlayerJumpController _jumpController;
    //     private PlayerGroundCheck _groundCheck;
    //     private PlayerCameraController _cameraController;

    //     private void Awake()
    //     {
    //         _inputHandler = GetComponent<PlayerInputHandler>();
    //         _