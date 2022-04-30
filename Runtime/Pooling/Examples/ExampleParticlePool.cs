using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace sudosilico.Tools.Pooling
{
    public class ExampleParticlePool : MonoBehaviour
    {
        [Header("Pool")]
        public GameObject ParticlePrefab;
        public PoolType PoolType;
        public int Capacity = -1;

        private Stack<ExampleParticle> _pulled = new Stack<ExampleParticle>();
        private Pool<ExampleParticle> _particlePool;

        [Header("outputs")]
        public int AllCount;
        public int FreeCount;
        
        private void Update()
        {
            if (_particlePool != null)
            {
                AllCount = _particlePool.CountTotal;
                FreeCount = _particlePool.Count;
            }
        }

        [Button("Spawn")]
        private void SpawnParticle()
        {
            var free = _particlePool.Pull();

            if (free != null)
            {
                _pulled.Push(free);
                free.gameObject.SetActive(true);
            }
        }
        
        [Button("ClearOne")]
        private void PopParticle()
        {
            var pulled = _pulled.Pop();
            
            if (pulled != null)
            {
                pulled.ReturnToPoolAction?.Invoke(pulled);
            }
        }

        private void Awake()
        {
            _particlePool = new BehaviourPool<ExampleParticle>(ParticlePrefab, Capacity, transform)
            {
                PoolType = PoolType,
            };
            
            _particlePool.Initialize();
        }
    }
}