
using UnityEngine;
namespace StarterAssets
{
    public class PlayerAudioController : MonoBehaviour
    {
        [SerializeField] private AudioClip landingAudioClip;
        [SerializeField] private AudioClip[] footstepAudioClips;
        [SerializeField] private float footstepAudioVolume = 0.5f;

        private CharacterController _controller;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
        }

        public void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (footstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, footstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(footstepAudioClips[index], 
                        transform.TransformPoint(_controller.center), footstepAudioVolume);
                }
            }
        }

        public void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(landingAudioClip, 
                    transform.TransformPoint(_controller.center), footstepAudioVolume);
            }
        }
    }
}