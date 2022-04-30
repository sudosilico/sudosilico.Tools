using UnityEngine;

namespace sudosilico.Tools
{
    public class DestroyAfterSeconds : MonoBehaviour
    {
        [SerializeField] private float _duration;
        
        private float _spawnTime;
        
        private void Awake()
        {
            _spawnTime = Time.time;
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