using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using ZGenesis.Attributes;
using ZGenesis.Events;

namespace ZGenesis.BaseMod {
    [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Reflection will call these when patched.")]
    internal static class NetworkPatches {
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
    }
}
