using UnityEngine;

namespace sudosilico.Tools
{
    public class DestroyAfterParticleEffect : MonoBehaviour
    {
        private float _spawnTime;
        private float _duration;
        private ParticleSystem _particleSystem;

        private void Awake()
        {
            _spawnTime = Time.time;
            _particleSystem = GetComponent<ParticleSystem>();
            _duration = _particleSystem.main.duration;
        }

        private void Update()
        {
            if (Time.time > (_spawnTime + _duration))
            {
                Destroy(gameObject);
            }
        }
    }
}