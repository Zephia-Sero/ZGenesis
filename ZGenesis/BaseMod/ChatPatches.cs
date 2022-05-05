using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using ZGenesis.Attributes;
using System.Reflection;
using System.Reflection.Emit;
using Mirror;
using UnityEngine.UI;
using ZGenesis.Registry;

namespace ZGenesis.BaseMod {
    [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Reflection will call these when patched.")]
    internal static class ChatPatches {
        [ModPatch("prefix", "Assembly-CSharp", "GlobalChat.DoCommand")]
        private static bool CustomCommandSetup_GlobalChatDoCommand(string text) {
            if(CommandRegistry.TryRun(text)) return false;
            return true;
        }
    }
}
