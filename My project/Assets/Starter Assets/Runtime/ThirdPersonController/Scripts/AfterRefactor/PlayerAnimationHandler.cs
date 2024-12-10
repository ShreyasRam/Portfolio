// // 6. Animation Controller
using UnityEngine;

public class PlayerAnimationHandler : MonoBehaviour
    {
        private Animator _animator;
        private float _animationBlend;
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;

        private bool _hasAnimator;

        public void SetPlayerAnimation()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void Start()
        {
            _hasAnimator = TryGetComponent(out _animator);
            SetPlayerAnimation();
        }

        public void UpdateMovementAnimation(float speed, float motionSpeed)
        {
            if (!_hasAnimator) return;
            
            _animationBlend = Mathf.Lerp(_animationBlend, speed, Time.deltaTime * 10f);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            _animator.SetFloat(_animIDSpeed, _animationBlend);
            _animator.SetFloat(_animIDMotionSpeed, motionSpeed);
        }

        public void SetGroundedState(bool grounded)
        {
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, grounded);
            }
        }

        public void SetJumpState(bool jumping)
        {
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDJump, jumping);
            }
        }

        public void SetFreeFallState(bool freeFalling)
        {
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDFreeFall, freeFalling);
            }
        }
    }
