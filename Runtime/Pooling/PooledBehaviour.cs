using System;
using UnityEngine;

namespace sudosilico.Tools.Pooling
{
    public abstract class PooledBehaviour<T> : MonoBehaviour
        where T : MonoBehaviour
    {
        public Action<T> ReturnToPoolAction { get; set; }
        
        public void ReturnToPool()
        {
            ReturnToPoolAction?.Invoke(this as T);
        }
    }
}