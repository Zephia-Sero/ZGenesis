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
            foreach(MethodInfo method in methods) {
                Attribute[] attrs = Attribute.GetCustomAttributes(method);
                foreach(Attribute attr in attrs) {
                    if(attr is ModPatchAttribute modPatch) {
                        modPatch.Patch(eventHarmony, method);
                    }
                }
            }
        }
        // EnqueueEvent method for transpilers.
        private readonly static MethodInfo m_Patcher__EnqueueEvent = typeof(Patcher).GetMethod(nameof(Patcher.EnqueueEvent));

        // Network Events
        [ModPatch("postfix", "Mirror", "Mirror.NetworkManager.OnStartClient")]
        private static void OnStartClient(Mirror.NetworkManager __instance) {
            Patcher.EnqueueEvent(new StartClientEvent(__instance));
        }
        
        [ModPatch("postfix", "Mirror", "Mirror.NetworkManager.OnStartServer")]
        private static void OnStartServer(Mirror.NetworkManager __instance) {
            Patcher.EnqueueEvent(new StartServerEvent(__instance));
        }
        
        [ModPatch("postfix", "Mirror", "Mirror.NetworkManager.OnStartHost")]
        private static void OnStartHost(Mirror.NetworkManager __instance) {
            Patcher.EnqueueEvent(new StartHostEvent(__instance));
        }
        
        [ModPatch("postfix", "Mirror", "Mirror.NetworkManager.OnStopClient")]
        private static void OnStopClient(Mirror.NetworkManager __instance) {
            Patcher.EnqueueEvent(new StopClientEvent(__instance));
        }
        
        [ModPatch("postfix", "Mirror", "Mirror.NetworkManager.OnStopServer")]
        private static void OnStopServer(Mirror.NetworkManager __instance) {
            Patcher.EnqueueEvent(new StopServerEvent(__instance));
        }
        
        [ModPatch("postfix", "Mirror", "Mirror.NetworkManager.OnStopHost")]
        private static void OnStopHost(Mirror.NetworkManager __instance) {
            Patcher.EnqueueEvent(new StopHostEvent(__instance));
        }
        
        // Fix for NetcodeManager.OnStartClient() not triggering base(NetworkManager).OnStartClient() for some reason
        // (OnStartServer does trigger this however which is why a similar patch is not needed)
        private readonly static FieldInfo f_MultiplayerSettings__hosting = typeof(MultiplayerSettings).GetField("hosting", BindingFlags.Public | BindingFlags.Static);
        private readonly static MethodInfo m_NetworkManager__OnStartClient = typeof(Mirror.NetworkManager).GetMethod("OnStartClient", BindingFlags.Public);
        
        [ModPatch("transpiler", "Assembly-CSharp", "NetcodeManager.OnStartClient")]
        private static IEnumerable<CodeInstruction> OnStartClientFix(IEnumerable<CodeInstruction> instructions) {
            foreach(CodeInstruction instruction in instructions) {
                yield return instruction;
                if(instruction.StoresField(f_MultiplayerSettings__hosting)) {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Call, m_NetworkManager__OnStartClient);
                }
            }
        }
        
        [ModPatch("postfix", "Mirror", "Mirror.NetworkServer.AddConnection")]
        private static void OnServerAddConnection(ref Mirror.NetworkConnection conn) {
            Patcher.EnqueueEvent(new PlayerJoinLobbyEvent(conn));
        }

        // Inventory Events
        [ModPatch("postfix", "Assembly-CSharp", "Sylladex.SetDragItem")]
        private static void OnDragItem(Sylladex __instance, ref Item item) {
            Patcher.EnqueueEvent(new DragItemEvent(__instance, item));
        }

        private readonly static MethodInfo m_Modus__UpdatePositions = typeof(Modus).GetMethod("UpdatePositions",BindingFlags.NonPublic|BindingFlags.Instance);
        private readonly static FieldInfo f_Modus__sylladex = typeof(Modus).GetField("sylladex",BindingFlags.Public|BindingFlags.Instance);
        private readonly static ConstructorInfo c_SylladexAddItemEvent = typeof(SylladexAddItemEvent).GetConstructor(new Type[] {typeof(Item), typeof(Sylladex)});
        
        [ModPatch("transpiler", "Assembly-CSharp", "Modus.AcceptItem")]
        private static IEnumerable<CodeInstruction> OnModusAcceptItem(IEnumerable<CodeInstruction> instructions) {
            foreach(CodeInstruction instruction in instructions) {
                yield return instruction;
                if(instruction.Calls(m_Modus__UpdatePositions)) {
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, f_Modus__sylladex);
                    yield return new CodeInstruction(OpCodes.Newobj, c_SylladexAddItemEvent);
                    yield return new CodeInstruction(OpCodes.Call, m_Patcher__EnqueueEvent);
                }
            }
        }
    }
}