using System.Collections.Generic;
using sudosilico.Tools.GameEvents;
using UnityEngine;

namespace sudosilico.Tools.RuntimeSets
{
    public abstract class RuntimeSet : ScriptableObject
    {
        [Tooltip("A GameEvent to raise whenever the set is modified.")]
        public GameEvent OnChange;
    }
    
    public abstract class RuntimeSet<T> : RuntimeSet
    {
        public List<T> Items = new();
        
        public void Add(T item)
        {
            if (!Items.Contains(item))
            {
                Items.Add(item);

                if (OnChange != null)
                {
                    OnChange.Raise();
                }
            }
        }

        public void Remove(T item)
        {
            if (Items.Contains(item))
            {
                Items.Remove(item);
                
                if (OnChange != null)
                {
                    OnChange.Raise();
                }
            }
        }
    }
}