using System;
using UnityEngine;

namespace sudosilico.Tools.GameVariables
{
    public abstract class GameVariable<T, TVariable> : ValueGameEvent<T, TVariable>
        where TVariable : GameVariable<T, TVariable>
    {
        public virtual T Value
        {
            get => _value;
            set => SetValue(value);
        }
        
        [SerializeField]
        protected T _value;

        private bool _alwaysRaiseOnSet = false;
        
        public void SetValue(T value)
        {
            var newValue = ValidateValue(value);
            var oldValue = _value;
            _value = newValue;
            
            if (_alwaysRaiseOnSet || !AreValuesEqual(newValue, oldValue))
            {
                Raise(newValue);
            }
        }

        public override string ToString()
        {
            return _value == null ? "null" : _value.ToString();
        }

        public static implicit operator T(GameVariable<T, TVariable> variable)
        {
            return variable._value;
        }

        public void OnValidate()
        {
            SetValue(_value);
        }

        protected virtual bool AreValuesEqual(T lhs, T rhs)
        {
            if (lhs != null)
            {
                return lhs.Equals(rhs);
            }

            return rhs == null;
        }

        protected virtual T ValidateValue(T value)
        {
            return value;
        }
    }
}