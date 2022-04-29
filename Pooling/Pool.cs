using System;
using System.Collections.Generic;
using UnityEngine;

// https://www.youtube.com/watch?v=x6jFZvvOGgk

namespace sudosilico.Tools.Pooling
{
    public abstract class Pool<T>
        where T : class
    {
        public readonly GameObject Prefab;
        public readonly Transform Parent;
        
        public PoolType PoolType = PoolType.Cycle;

        protected List<T> PooledObjects;
        
        private bool _initialized = false;
        private int _initialCapacity;
        private Stack<T> _freeObjects;
        private int _cycleIndex = 0;

        public int CountTotal => PooledObjects.Count;
        public int Count => _freeObjects.Count;

        protected Pool(GameObject prefab, int initialCapacity, Transform parent)
        {
            Prefab = prefab;
            Parent = parent;

            _initialCapacity = initialCapacity;
            
            if (_initialCapacity < 0)
            {
                Debug.LogError("Pool initialCapacity should not be negative. Resetting to zero.");
                _initialCapacity = 0;
            }
        }

        public void Initialize()
        {
            if (_initialized)
            {
                return;
            }
            
            _freeObjects = new Stack<T>(_initialCapacity);
            PooledObjects = new List<T>(_initialCapacity);

            for (int i = 0; i < _initialCapacity; i++)
            {
                var instance = CreateInstance();
                if (instance != null)
                {
                    PooledObjects.Add(instance);
                    _freeObjects.Push(instance);
                }
            }

            _initialized = true;
        }
        
        public T Pull()
        {
            if (!_initialized)
            {
                Initialize();
            }
            
            switch (PoolType)
            {
                case PoolType.NoMaximum:
                {
                    if (_freeObjects.Count > 0)
                    {
                        var component = _freeObjects.Pop();
                        SetActive(component, true);
                        return component;
                    }

                    var instance = CreateInstance();
                    if (instance != null)
                    {
                        PooledObjects.Add(instance);
                    }

                    return instance;
                }
                case PoolType.Cycle:
                {
                    var component = PooledObjects[_cycleIndex];
                    _cycleIndex = (_cycleIndex + 1) % PooledObjects.Count;
                    SetActive(component, true);
                    return component;
                }
                case PoolType.WaitForReturn:
                {
                    if (_freeObjects.Count > 0)
                    {
                        var component = _freeObjects.Pop();
                        SetActive(component, true);
                        return component;
                    }

                    return null;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected void PushFunc(T component)
        {
            SetActive(component, false);
            _freeObjects.Push(component);
        }
        
        protected abstract T CreateInstance();

        protected virtual void SetActive(T instance, bool active)
        {
        }
    }
}