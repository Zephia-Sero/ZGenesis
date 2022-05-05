using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZGenesis.Objects {
    public abstract class ModdedCommand {
        public abstract bool Safe { get; }
        public abstract bool IsCheat { get; }
        public abstract string Help { get; }
        public abstract void Execute(string[] args);
        protected void Message(string message) {
            GlobalChat.WriteCommandMessage(message);
        }
    }
}
