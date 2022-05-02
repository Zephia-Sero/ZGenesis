using System;
using System.Collections.Generic;
using ZGenesis.Attributes;
using ZGenesis.Events;
using HarmonyLib;
using System.Reflection;
using System.Reflection.Emit;

namespace ZGenesis.BaseMod {
    [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Reflection will call these when patched.")]
    internal static class MiscPatches {
        // Miscellaneous patches
        private readonly static ConstructorInfo c_CaneraRotateEvent = typeof(CameraRotateEvent).GetConstructor(new Type[] { });
        [ModPatch("transpiler", "Assembly-CSharp", "CameraRotationControls.Rotate")]
        private static IEnumerable<CodeInstruction> OnCameraRotate(IEnumerable<CodeInstruction> instructions) {
            foreach(CodeInstruction instruction in instructions) {
                if(instruction.opcode == OpCodes.Ret) {
                    yield return new CodeInstruction(OpCodes.Newobj, c_CaneraRotateEvent);
                    yield return new CodeInstruction(OpCodes.Pop);
                }
                yield return instruction;
            }
        }
    }
}
