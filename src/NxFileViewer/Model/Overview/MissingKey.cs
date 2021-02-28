using System;
using LibHac;

namespace Emignatik.NxFileViewer.Model.Overview
{
    public class MissingKey : IEquatable<MissingKey>
    {
        public MissingKey(string keyName, KeyType keyType)
        {
            KeyName = keyName ?? throw new ArgumentNullException(nameof(keyName));
            KeyType = keyType;
        }

        public string KeyName { get; }

        public KeyType KeyType { get; }

        public bool Equals(MissingKey? other)
        {
            if (other == null)
                return false;
            
            return KeyName == other.KeyName && KeyType == other.KeyType;
        }

        public override bool Equals(object? obj)
        {
            return obj is MissingKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(KeyName, (int) KeyType);
        }
    }
}