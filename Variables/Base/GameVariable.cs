using sudosilico.Tools.Events;
using UnityEngine;

namespace sudosilico.Tools
{
    public abstract class GameVariable : GameEvent
    {
        public abstract object BaseValue { get; set; }
    }
    
    public abstract class GameVariable<T> : GameVariable
    {
#if UNITY_EDITOR
        [Multiline]
        [SerializeField]
        // ReSharper disable once NotAccessedField.Local
        private string _description = "";
#endif

        public virtual T Value
        {
            get => _value;
            set => SetValue(value);
        }

        public override object BaseValue
        {
            get => _value;
            set => SetValue((T)value);
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
                Raise();
            }
        }

        public override string ToString()
        {
            return _value == null ? "null" : _value.ToString();
        }

        public static implicit operator T(GameVariable<T> variable)
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