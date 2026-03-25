using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(AudioSource))]
public sealed class AudioEmitter : MonoBehaviour {
    public string AudioId = "demo_emitter_loop";
    public bool PlayOnEnable = true;
    public bool StopOnDisable = true;
    public bool PlayOnVisible;
    public bool StopOnInvisible;

    private AudioSource cachedAudioSource;

    private void Awake() {
        cachedAudioSource = GetComponent<AudioSource>();
        if (cachedAudioSource != null) {
            cachedAudioSource.playOnAwake = false;
        }
    }

    private void OnEnable() {
        if (PlayOnEnable) {
            PlayAudio();
        }
    }

    private void OnDisable() {
        if (StopOnDisable) {
            StopAudio();
        }
    }

    private void OnBecameVisible() {
        if (PlayOnVisible) {
            PlayAudio();
        }
    }

    private void OnBecameInvisible() {
        if (StopOnInvisible) {
            StopAudio();
        }
    }

    public void PlayAudio() {
        if (cachedAudioSource == null || string.IsNullOrEmpty(AudioId)) {
            return;
        }

        if (cachedAudioSource.isPlaying) {
            return;
        }

        AudioManager.Instance.PlayOnSource(AudioId, cachedAudioSource);
    }

    public void StopAudio() {
        if (cachedAudioSource == null) {
            return;
        }

        AudioManager.Instance.Stop(cachedAudioSource);
    }
}
