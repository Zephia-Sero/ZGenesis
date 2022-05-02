using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZGenesis.Attributes {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class GenesisModAttribute : Attribute {
        public GenesisModAttribute() { }
    }
}
