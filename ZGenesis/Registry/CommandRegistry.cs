using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZGenesis.Objects;

namespace ZGenesis.Registry {
    public static class CommandRegistry {
        private static readonly Dictionary<string, ModdedCommand> commands = new Dictionary<string, ModdedCommand>();
        private static ModdedCommand commandToConfirm = null;
        private static string[] argsToConfirm = null;
        private static string cmdNameToConfirm = "";
        public static void Register(string modname, string alias, ModdedCommand command) {
            if(!commands.ContainsKey(alias)) {
                commands.Add(alias, command);
            } else {
                Logger.Log(Logger.LogLevel.WARNING, modname, "Command with alias '{0}' already added!", alias);
            }
        }
        public static bool TryRun(string command) {
            if(command[0] == '/') {
                string[] args = command.Substring(1).Split(' ');
                if(args[0] == "help") {
                    if(args.Length > 1) {
                        if(commands.TryGetValue(args[1], out ModdedCommand other)) {
                            if(!other.safe) {
                                GlobalChat.WriteCommandMessage("<color=red>This is an extremely dangerous command. Use with caution, only when testing.</color>");
                            }
                            GlobalChat.WriteCommandMessage(other.help);
                        } else return false;
                    } else {
                        GlobalChat.WriteCommandMessage("This is a list of all MODDED commands. Those marked in red are dangerous, and can permanently make your session unplayable. Run /help commandname to get more information about what a command does.");
                        foreach(KeyValuePair<string, ModdedCommand> keyValuePair in commands) {
                            GlobalChat.WriteCommandMessage(
                                (keyValuePair.Value.safe ? "" : "<color=red>") +
                                "/" + keyValuePair.Key +
                                (keyValuePair.Value.safe ? "" : "</color>")
                            );
                        }
                        GlobalChat.WriteCommandMessage("");
                    }
                } else if(commands.TryGetValue(args[0], out ModdedCommand cmd)) {
                    if(cmd.safe) {
                        cmd.Execute(args);
                    } else {
                        GlobalChat.WriteCommandMessage("/" + args[0] + " is intended for testing and is very likely to break your session. Are you sure you want to do this? (y/n)");
                        commandToConfirm = cmd;
                        argsToConfirm = args;
                        cmdNameToConfirm = args[0];
                    }
                } else return false;
            } else if(commandToConfirm != null && argsToConfirm != null) {
                if(command == "n" || command == "y") {
                    GlobalChat.Pester(MultiplayerSettings.playerName + " decided to run the seriously dangerous command /" + cmdNameToConfirm, false);
                    commandToConfirm.Execute(argsToConfirm);
                } else {
                    GlobalChat.WriteCommandMessage("Cancelled command.");
                }
                commandToConfirm = null;
                argsToConfirm = null;
                cmdNameToConfirm = "";
            } else return false;
            return true;
        }
    }
}
