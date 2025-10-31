using System.Collections.Generic;

using UnityEngine;

namespace Game.Audio
{
    /// <summary>
    /// Random Player for a limited amount of preselected AudioClips
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class RandomAudioClipPlayer : MonoBehaviour, IAudioRandomClipPlayer, IGameLoadProgress
    {
        [SerializeField] private List<AudioClip> clips;

        [Tooltip("Überschreibt die Lautstärke / Volume an AudioSource!")]
        [Range(0, 1)]
        [SerializeField] private float volume = 1;

        private AudioSource player;

        private void Awake()
        {
            this.player = GetComponent<AudioSource>();
            this.volume = player.volume;
            this.player.spread = 1; // 1 = 3D Sound
            this.player.playOnAwake = false;
            this.player.mute = false;
            this.player.loop = false;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (this.player == null)
                Awake();

            if (this.player == null)
                return;

            this.player.volume = volume;
        }
#endif

        /// <summary>
        /// Spielt einen aus der Liste zufällig gefällten Clip
        /// </summary>
        [ContextMenu("Play Random")]
        public void Play()
        {
            if (clips == null || clips.Count == 0)
                return;

            int clipIndex = Random.Range(0, clips.Count);
            AudioClip clip = clips[clipIndex];
            player.PlayOneShot(clip);
        }

        internal void SetMute(bool muted)
        {
            if (this.player == null)
                return;

            this.player.mute = muted;
        }
    }
}