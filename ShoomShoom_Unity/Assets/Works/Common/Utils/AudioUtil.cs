using UnityEngine;

namespace SleepyUtil
{
    public static class AudioUtil
    {
        public static void SetRandomClip(this AudioSource source, ref AudioClip[] clips, float baseVolume, float volumeRange, float pitchRange)
        {
            AudioClip clip = clips[Random.Range(0, clips.Length)];
            source.SetRandomClip(ref clip, baseVolume, volumeRange, pitchRange);
        }
        public static void SetRandomClip(this AudioSource source, ref AudioClip[] clips, float volumeRange, float pitchRange)
        {
            source.SetRandomClip(ref clips, 1, volumeRange, pitchRange);
        }
        public static void SetRandomClip(this AudioSource source, ref AudioClip clip, float baseVolume, float volumeRange, float pitchRange)
        {
            source.clip = clip;
            source.volume = Random.Range(baseVolume - volumeRange, baseVolume);
            source.pitch = Random.Range(1 - pitchRange, 1 + pitchRange);

            source.PlayOneShot(source.clip);
        }
        public static void SetRandomClip(this AudioSource source, ref AudioClip clip, float volumeRange, float pitchRange)
        {
            source.SetRandomClip(ref clip, 1, volumeRange, pitchRange);
        }
        public static void SetRandomClipWithFixedVolume(this AudioSource source, ref AudioClip[] clips, float baseVolume, float pitchRange)
        {
            AudioClip clip = clips[Random.Range(0, clips.Length)];
            source.clip = clip;
            source.volume = baseVolume;
            source.pitch = Random.Range(1 - pitchRange, 1 + pitchRange);

            source.PlayOneShot(source.clip);
        }
    }

}