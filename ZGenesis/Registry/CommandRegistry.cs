using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZGenesis.Objects;

namespace ZGenesis.Registry {
    public static class CommandRegistry {
        private static readonly Dictionary<string, ModdedCommand> commands = new Dictionary<string, ModdedCommand>();
        public static void Register(string modname, string alias, ModdedCommand command) {
            if(!commands.ContainsKey(alias)) {
                commands.Add(alias, command);
            } else {
                Logger.Log(Logger.LogLevel.WARNING, modname, "Command with alias '{0}' already added!", alias);
            }
        }
        public static bool TryRun(string command) {
            return false;
        }
    }
}
