using System;
using System.Reflection.Emit;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using ZGenesis.Events;
using System.Reflection;
using ZGenesis.Attributes;

namespace ZGenesis {
    public class EventPatches {
        public static void PatchAll() {
            Harmony eventHarmony = new Harmony("com.zgenesis.event");
            MethodInfo[] methods = typeof(EventPatches).GetMethods(BindingFlags.NonPublic|BindingFlags.Public|BindingFlags.Static);
            Logger.Log("ZGD", "Got all methods...");
            foreach(MethodInfo method in methods) {
                Logger.Log("ZGD", "Method: {0}", method.ToString());
                Attribute[] attrs = Attribute.GetCustomAttributes(method);
                foreach(Attribute attr in attrs) {
                    if(attr is ModPatchAttribute modPatch) {
                        Logger.Log("ZGD", "Attribute: {0}", modPatch.ToString());
                        modPatch.Patch(eventHarmony, method);
                    }
                }
            }
        }
        internal static MethodInfo m_Patcher__EnqueueEvent = typeof(Patcher).GetMethod(nameof(Patcher.EnqueueEvent));
        [ModPatch("postfix", "Assembly-CSharp", "Sylladex.SetDragItem")]
        private static void OnDragItem(Sylladex __instance, ref Item item) {
            Patcher.EnqueueEvent(new DragItemEvent(__instance, item));
        }
        [ModPatch("postfix", "Mirror", "Mirror.NetworkServer.AddConnection")]
        private static void OnServerAddConnection(ref Mirror.NetworkConnection conn) {
            Patcher.EnqueueEvent(new PlayerJoinLobbyEvent(conn));
        }
        private readonly static MethodInfo m_Modus__UpdatePositions = typeof(Modus).GetMethod("UpdatePositions",BindingFlags.NonPublic|BindingFlags.Instance);
        private readonly static FieldInfo f_Modus__Sylladex = typeof(Modus).GetField("sylladex",BindingFlags.Public|BindingFlags.Instance);
        private readonly static ConstructorInfo c_SylladexAddItemEvent = typeof(SylladexAddItemEvent).GetConstructor(new Type[] {typeof(Item), typeof(Sylladex)});
        [ModPatch("transpiler", "Assembly-CSharp", "Modus.AcceptItem")]
        private static IEnumerable<CodeInstruction> OnModusAcceptItem(IEnumerable<CodeInstruction> instructions) {
            foreach(CodeInstruction instruction in instructions) {
                yield return instruction;
                if(instruction.Calls(m_Modus__UpdatePositions)) {
                    List<CodeInstruction> injection = new List<CodeInstruction>() {
                        new CodeInstruction(OpCodes.Ldarg_1),
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, f_Modus__Sylladex),
                        new CodeInstruction(OpCodes.Newobj, c_SylladexAddItemEvent),
                        new CodeInstruction(OpCodes.Call, m_Patcher__EnqueueEvent),
                    };
                    foreach(CodeInstruction injected in injection) {
                        yield return injected;
                    }
                }
            }
        }
    }
}