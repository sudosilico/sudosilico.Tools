using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace sudosilico.Tools
{
    [InitializeOnLoad]
    public static class SoundUtil
    {
        private static GameObject _audioSourcePool2D;
        private static Queue<AudioSource> _availableAudioSources = new();
        private static List<AudioSource> _usedAudioSources = new();

        private const float _timeBetweenRefresh = 1.0f;
        private static float _timeOfLastRefresh;

        static SoundUtil()
        {
            Clear();
        }

        public static void Clear()
        {
            _audioSourcePool2D = null;
            _availableAudioSources = new();
            _usedAudioSources = new();
            _timeOfLastRefresh = float.MinValue;
        }
        
        public static void Play(AudioClip clip, float pitch, float volume)
        {
            RefreshIfNeeded();
            
            var source = GetAvailableAudioSource();

            if (source == null)
            {
                RefreshIfNeeded(true);
                source = GetAvailableAudioSource();
            }

            source.transform.position = Vector3.zero;
            source.loop = false;
            source.clip = clip;
            source.pitch = pitch;
            source.volume = volume;
            source.Play();
            
            _usedAudioSources.Add(source);
        }

        public static void Play(AudioClip clip, float pitch, float pitchVariation, float amp, float ampVariation)
        {
            var effectivePitch = Random.Range(pitch - pitchVariation, pitch + pitchVariation);
            var effectiveAmp = Random.Range(amp - ampVariation, amp + ampVariation);

            Play(clip, effectivePitch, effectiveAmp);
        }

        private static AudioSource GetAvailableAudioSource()
        {
            if (_availableAudioSources.Count > 0)
            {
                return _availableAudioSources.Dequeue();
            }

            return CreateAudioSource();
        }
        
        private static void RefreshIfNeeded(bool forceRefresh = false)
        {
            if (forceRefresh || (Time.time > (_timeOfLastRefresh + _timeBetweenRefresh)))
            {
                _timeOfLastRefresh = Time.time;

                for (int i = _usedAudioSources.Count - 1; i >= 0; i--)
                {
                    var audioSource = _usedAudioSources[i];

                    if (audioSource == null)
                    {
                        _usedAudioSources.RemoveAt(i);
                    }
                    else if (!audioSource.isPlaying)
                    {
                        _availableAudioSources.Enqueue(audioSource);
                        _usedAudioSources.RemoveAt(i);
                    }
                }
            }
        }

        private static AudioSource CreateAudioSource()
        {
            var go = new GameObject("Pooled AudioSource");
            var source = go.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.transform.parent = GetAudioSourcePool2D().transform;
            return source;
        }
        
        private static GameObject GetAudioSourcePool2D()
        {
            if (_audioSourcePool2D == null)
            {
                _audioSourcePool2D = new GameObject("Audio Source Pool 2D");
                _audioSourcePool2D.transform.parent = null;
            }

            return _audioSourcePool2D;
        }
    }
}