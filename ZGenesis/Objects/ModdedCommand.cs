using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZGenesis.Objects {
    public abstract class ModdedCommand {
        public bool safe = false;
        public string help = "Someone did a dum-dum and didn't add a command help text";
        public abstract void Execute(string[] args);
        protected void Message(string message) {
            GlobalChat.WriteCommandMessage(message);
        }
    }
}
