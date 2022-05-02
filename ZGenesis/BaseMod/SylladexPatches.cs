using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using ZGenesis.Attributes;
using ZGenesis.Events;

namespace ZGenesis.BaseMod {
    [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Reflection will call these when patched.")]
    internal static class SylladexPatches {
        // Inventory Events
        [ModPatch("postfix", "Assembly-CSharp", "Sylladex.SetDragItem")]
        private static void OnDragItem(Sylladex __instance, ref Item item) {
            Patcher.EnqueueEvent(new DragItemEvent(__instance, item));
        }

        private readonly static MethodInfo m_Modus__UpdatePositions = typeof(Modus).GetMethod("UpdatePositions", BindingFlags.NonPublic | BindingFlags.Instance);
        private readonly static FieldInfo f_Modus__sylladex = typeof(Modus).GetField("sylladex", BindingFlags.Public | BindingFlags.Instance);
        private readonly static ConstructorInfo c_SylladexAddItemEvent = typeof(SylladexAddItemEvent).GetConstructor(new Type[] { typeof(Item), typeof(Sylladex) });

        [ModPatch("transpiler", "Assembly-CSharp", "Modus.AcceptItem")]
        private static IEnumerable<CodeInstruction> OnModusAcceptItem(IEnumerable<CodeInstruction> instructions) {
            foreach(CodeInstruction instruction in instructions) {
                yield return instruction;
                if(instruction.Calls(m_Modus__UpdatePositions)) {
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, f_Modus__sylladex);
                    yield return new CodeInstruction(OpCodes.Newobj, c_SylladexAddItemEvent);
                    yield return new CodeInstruction(OpCodes.Call, Patcher.m_Patcher__EnqueueEvent);
                }
            }
        }
    }
}
