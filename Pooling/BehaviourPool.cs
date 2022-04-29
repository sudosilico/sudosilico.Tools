using UnityEngine;
using Object = UnityEngine.Object;

namespace sudosilico.Tools.Pooling
{
    public class BehaviourPool<T> : Pool<T>
        where T : PooledBehaviour<T>
    {
        public BehaviourPool(GameObject prefab, int initialCapacity = 0, Transform parent = null)
            : base(prefab, initialCapacity, parent)
        {
        }

        protected override T CreateInstance()
        {
            GameObject gameObject = Object.Instantiate(Prefab, Parent);
            gameObject.SetActive(false);
                
            T component = gameObject.GetComponent<T>();
            
            if (component == null)
            {
                Debug.LogError($"Error: Pool<{typeof(T).Name}> spawned a prefab that did not contain a {typeof(T).Name}.");
                return null;
            }
            
            component.ReturnToPoolAction = PushFunc;
            
            return component;
        }

        protected override void SetActive(T instance, bool active) => instance.gameObject.SetActive(active);
    }
}