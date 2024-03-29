﻿using System;
using System.Collections.Generic;
using ZGenesis.Attributes;
using ZGenesis.Events;
using HarmonyLib;
using System.Reflection;
using System.Reflection.Emit;

namespace ZGenesis.BaseMod {
    [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Reflection will call these when patched.")]
    internal static class CameraPatches {
        // Camera patches
        private readonly static ConstructorInfo c_CameraRotateEvent = typeof(CameraRotateEvent).GetConstructor(new Type[] { typeof(MSPAOrthoController), typeof(float) });
        private readonly static FieldInfo f_CameraRotationControls__cameraController = typeof(CameraRotationControls).GetField("cameraController", BindingFlags.NonPublic | BindingFlags.Instance);
        [ModPatch("transpiler", "Assembly-CSharp", "CameraRotationControls.Rotate")]
        private static IEnumerable<CodeInstruction> OnCameraRotate(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
            generator.DeclareLocal(typeof(float));
            foreach(CodeInstruction instruction in instructions) {
                if(instruction.opcode == OpCodes.Sub) {
                    yield return new CodeInstruction(OpCodes.Dup);
                    yield return new CodeInstruction(OpCodes.Stloc_0);
                } else if(instruction.opcode == OpCodes.Ret) {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld,  f_CameraRotationControls__cameraController);
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    yield return new CodeInstruction(OpCodes.Newobj, c_CameraRotateEvent);
                    yield return new CodeInstruction(OpCodes.Pop);
                }
                yield return instruction;
            }
        }
        [ModPatch("postfix", "Assembly-CSharp", "CameraRotationControls.RotateLeft")]
        private static void OnCameraRotateLeft(ref MSPAOrthoController ___cameraController) {
            new CameraRotateEvent(___cameraController, -180f);
        }
        [ModPatch("postfix", "Assembly-CSharp", "CameraRotationControls.RotateRight")]
        private static void OnCameraRotateRight(MSPAOrthoController ___cameraController) {
            new CameraRotateEvent(___cameraController, 180f);
        }
    }
}
