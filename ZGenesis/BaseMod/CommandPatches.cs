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
    internal static class CommandPatches {
        private readonly static MethodInfo m_NetworkServer__SendToAll = typeof(NetworkServer).GetMethod("SendToAll", BindingFlags.Public | BindingFlags.Static);
        private readonly static FieldInfo f_Chat__inputField = typeof(Chat).GetField("inputField", BindingFlags.NonPublic | BindingFlags.Instance);
        private readonly static MethodInfo m_InputField__get_text = typeof(InputField).GetMethod("get_text", BindingFlags.Public | BindingFlags.Instance);
        private readonly static MethodInfo m_CommandRegistry__TryRun = typeof(CommandRegistry).GetMethod("TryRun", BindingFlags.NonPublic | BindingFlags.Static);
        [ModPatch("transpiler", "Assembly-CSharp", "Chat.SendMsg")]
        private static IEnumerable<CodeInstruction> CustomCommandSetup_ChatSendMsg(IEnumerable<CodeInstruction> instructions) {
            List<CodeInstruction> result = new List<CodeInstruction>();
            int bridx = 0;
            bool incr = true;
            foreach(CodeInstruction instruction in instructions) {
                result.Add(instruction);
                if(incr) bridx++;
                if(instruction.Calls(m_NetworkServer__SendToAll)) {
                    incr = false;
                    bridx += 4;
                    result.Add(new CodeInstruction(OpCodes.Ldarg_0));
                    result.Add(new CodeInstruction(OpCodes.Ldfld, f_Chat__inputField));
                    result.Add(new CodeInstruction(OpCodes.Callvirt, m_InputField__get_text));
                    result.Add(new CodeInstruction(OpCodes.Call, m_CommandRegistry__TryRun));
                    result.Add(new CodeInstruction(OpCodes.Brtrue_S));
                }
            }
            for(int i = result.Count-1; i >= 0; i--) {
                CodeInstruction instruction = result[i];
                if(instruction.IsLdarg(0)) {
                    result[bridx].operand = instruction.labels[0];
                }
            }
            return result;
        }
    }
}
