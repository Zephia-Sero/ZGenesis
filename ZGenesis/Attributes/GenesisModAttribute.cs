using System;

namespace ZGenesis.Attributes {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class GenesisModAttribute : Attribute {
        public GenesisModAttribute() { }
    }
}
