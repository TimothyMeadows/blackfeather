using System;

namespace Blackfeather.Data
{
    [Serializable]
    public struct ManagedMemorySpace
    {
        public long Created;
        public long Accessed;
        public long Updated;
        public string Pointer;
        public string Name;
        public object Value;
    }
}
