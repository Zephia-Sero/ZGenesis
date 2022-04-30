using HarmonyLib;
using ZGenesis.Events;

namespace ZGenesis {
    public class EventPatches {
        public static void PatchAll(Harmony harmony) {
            Logger.Log("ZGenesis", "Patching event hooks.");
            var original = typeof(Sylladex).GetMethod(nameof(Sylladex.SetDragItem));
            var postfix = typeof(EventPatches).GetMethod(nameof(SylladexSetDragItemPostfix));
            harmony.Patch(original, null, new HarmonyMethod(postfix));
        }
        public static void SylladexSetDragItemPostfix(Sylladex __instance, ref Item item) {
            Patcher.EnqueueEvent(new DragItemEvent(item, __instance));
        }
    }
}