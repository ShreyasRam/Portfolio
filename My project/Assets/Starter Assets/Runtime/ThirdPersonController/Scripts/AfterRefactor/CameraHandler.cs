// 5. Camera Controller
using UnityEngine;
namespace StarterAssets
{
    public class CameraHandler : MonoBehaviour
    {
        [Header("Cinemachine")]
        [SerializeField] private GameObject cinemachineCameraTarget;
        [SerializeField] private float topClamp = 70.0f;
        [SerializeField] private float bottomClamp = -30.0f;
        [SerializeField] private float cameraAngleOverride = 0.0f;
        [SerializeField] private bool lockCameraPosition = false;

        private PlayerInputHandler _inputHandler;
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;
        private const float _threshold = 0.01f;

        private void Awake()
        {
            _inputHandler = GetComponent<PlayerInputHandler>();
            _cinemachineTargetYaw = cinemachineCameraTarget.transform.rotation.eulerAngles.y;
        }

        public void CameraRotation()
        {
            // if there is an input and camera position is not fixed
            if (_inputHandler.LookInput.sqrMagnitude >= _threshold && !lockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = _inputHandler.IsUsingMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += _inputHandler.LookInput.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += _inputHandler.LookInput.y * deltaTimeMultiplier;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, bottomClamp, topClamp);

            // Cinemachine will follow this target
            cinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + cameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }
    }
}