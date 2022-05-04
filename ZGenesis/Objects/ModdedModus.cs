using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ZGenesis.Objects {
    public abstract class ModdedModus : Modus {
        public static string Description { get; private set; }
        public static Sprite Sprite { get; private set; }
    }
}
