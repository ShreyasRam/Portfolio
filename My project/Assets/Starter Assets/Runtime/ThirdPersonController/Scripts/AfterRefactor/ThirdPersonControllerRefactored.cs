// 8. Main Controller (Orchestrator)
using UnityEngine;
namespace StarterAssets
{
    public class ThirdPersonControllerRefactored : MonoBehaviour
    {
        private PlayerInputHandler _inputHandler;
        private PlayerMovementHandler _movementHandler;
        private CameraHandler _cameraController;

        private void Awake()
        {
            _inputHandler = GetComponent<PlayerInputHandler>();
            _movementHandler = GetComponent<PlayerMovementHandler>();
            _cameraController = GetComponent<CameraHandler>();

            // // Ensure required components are present
            // if (_inputHandler == null || _movementHandler == null || 
            //     _jumpController == null || _groundCheck == null || 
            //     _cameraController == null)
            // {
            //     Debug.LogError("Missing required components on ThirdPersonController!");
            // }
        }

        private void Start()
        {
            // Lock and hide cursor for game
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            _movementHandler.ProcessMovement();
        }
        
        void LateUpdate() 
        {
            _cameraController.CameraRotation();
        }

        private void OnEnable()
        {
            // Enable cursor lock when object is enabled
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void OnDisable()
        {
            // Free cursor when object is disabled
            Cursor.lockState = CursorLockMode.None;
        }
    }
    
}