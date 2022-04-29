using System;
using System.Collections.Generic;
using UnityEngine;

namespace sudosilico.Tools
{
    [Serializable]
    public class UniqueID : ISerializationCallbackReceiver, IComparable<UniqueID>
    {
        [SerializeField] 
        private byte[] _serializedGuid;
        
        [NonSerialized] 
        private Guid _guid;

        public UniqueID()
        {
            _guid = new Guid();
            _serializedGuid = _guid.ToByteArray();
        }

        public UniqueID(Guid guid)
        {
            _guid = guid;
            _serializedGuid = _guid.ToByteArray();
        }
        
        public UniqueID(byte[] guidBytes)
        {
            _guid = new Guid(guidBytes);
            _serializedGuid = guidBytes;
        }
        
        public bool IsEmpty()
        {
            return _guid == Guid.Empty;
        }

#if UNITY_EDITOR
        public byte[] GetBytes() => _serializedGuid;
#endif
        
        public Guid GetGuid() => _guid;
        
        public static UniqueID Parse(string stringID)
        {
            return new(Guid.Parse(stringID));
        }
        
        public static UniqueID Generate()
        {
            var guid = Guid.NewGuid();
            
            return new UniqueID
            {
                _guid = guid, 
                _serializedGuid = guid.ToByteArray()
            };
        }

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            _guid = new Guid(_serializedGuid);
        }

        public override string ToString()
        {
            return _guid.ToString();
        }

        protected bool Equals(UniqueID other)
        {
            return _guid.Equals(other._guid);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            
            if (obj.GetType() != this.GetType()) return false;
            
            return Equals((UniqueID)obj);
        }

        public int CompareTo(UniqueID other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            
            return _guid.CompareTo(other._guid);
        }

        public static bool operator ==(UniqueID lhs, UniqueID rhs)
        {
            if (lhs is null && rhs is null)
                return true;
            
            if (lhs is null || rhs is null)
                return false;
            
            return lhs._guid == rhs._guid;
        }

        public static bool operator !=(UniqueID lhs, UniqueID rhs)
        {
            return !(lhs == rhs);
        }

        private sealed class GUIDEqualityComparer : IEqualityComparer<UniqueID>
        {
            public bool Equals(UniqueID x, UniqueID y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                
                if (x.GetType() != y.GetType()) return false;
                
                return x._guid.Equals(y._guid);
            }

            public int GetHashCode(UniqueID obj)
            {
                return obj._guid.GetHashCode();
            }
        }

        public static IEqualityComparer<UniqueID> GUIDComparer { get; } = new GUIDEqualityComparer();

        public override int GetHashCode()
        {
            return _guid.GetHashCode();
        }
    }
}