using System.Collections.Generic;
using UnityEngine;

public static class AudioGeneratedClipFactory {
    private static readonly Dictionary<string, AudioClip> clipCache = new Dictionary<string, AudioClip>(16);
    private const int SAMPLE_RATE = 44100;

    public static AudioClip GetOrCreateClip(AudioConfig config) {
        if (config == null || string.IsNullOrEmpty(config.AudioId)) {
            return null;
        }

        if (clipCache.TryGetValue(config.AudioId, out AudioClip clip)) {
            return clip;
        }

        float duration = config.GeneratedDuration <= 0f ? 0.18f : config.GeneratedDuration;
        float frequency = config.GeneratedFrequency <= 0f ? 440f : config.GeneratedFrequency;
        int sampleCount = Mathf.Max(1, Mathf.RoundToInt(duration * SAMPLE_RATE));
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; ++i) {
            float time = i / (float)SAMPLE_RATE;
            float fadeIn = Mathf.Clamp01(i / (float)Mathf.Max(1, sampleCount / 20));
            float fadeOut = Mathf.Clamp01((sampleCount - i) / (float)Mathf.Max(1, sampleCount / 12));
            float envelope = Mathf.Min(fadeIn, fadeOut);
            samples[i] = Mathf.Sin(2f * Mathf.PI * frequency * time) * 0.24f * envelope;
        }

        clip = AudioClip.Create(config.AudioId + "_Generated", sampleCount, 1, SAMPLE_RATE, false);
        clip.SetData(samples, 0);
        clipCache[config.AudioId] = clip;
        return clip;
    }
}
