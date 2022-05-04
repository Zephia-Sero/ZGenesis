using HarmonyLib;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using ZGenesis.Attributes;
using ZGenesis.Events;
using ZGenesis.Registry;
using ZGenesis.Objects;
using UnityEngine;
using UnityEngine.UI;


namespace ZGenesis.BaseMod {
    [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Reflection will call these when patched.")]
    internal static class SylladexPatches {

        // Inventory Events
        
        [ModPatch("postfix", "Assembly-CSharp", "Sylladex.SetDragItem")]
        private static void OnDragItem(Sylladex __instance, ref Item item) {
            new DragItemEvent(__instance, item);
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
                    yield return new CodeInstruction(OpCodes.Pop);
                }
            }
        }

        // Custom modus patches
        
        [ModPatch("prefix", "Assembly-CSharp", "Sylladex.SetModus")]
        private static void CustomModusSetup_SylladexSetModus(Sylladex __instance, ref Modus ___captchaModus, ref GameObject ___modusObject, ref string ___modusName, AudioClip ___clipSwitch, string modus, bool playSound) {
            Logger.Log("TESTF", "{0}", modus);
            if(ModusRegistry.HasModus(modus)) {
                Logger.Log("TESTG", "{0}", modus);
                ___captchaModus = (Modus) ___modusObject.AddComponent(ModusRegistry.GetModus(modus));
                ___modusName = modus;
                if(playSound) {
                    Logger.Log("TESTH", "{0}", modus);
                    __instance.PlaySoundEffect(___clipSwitch);
                }
            }
        }

        [ModPatch("prefix", "Assembly-CSharp", "AddCaptchamodusAction.Start")]
        private static bool CustomModusSetup_AddCaptchaModusActionStart(AddCaptchamodusAction __instance, ref Sprite ___sprite) {
            Logger.Log("TESTE", "{0}", __instance.modus);
            if(ModusRegistry.HasModus(__instance.modus)) {
                ___sprite = (Sprite) ModusRegistry.GetFieldFromModus(__instance.modus, "sprite");
                Logger.Log("TESTD", "{0}", __instance.modus);
                return false;
            }
            return true;
        }

        [ModPatch("prefix", "Assembly-CSharp", "ModusPickerComponent.ModusChange")]
        private static bool CustomModusSetup_ModPickerComponentModusChange(ref Image ___image, string to) {
            Logger.Log("TESTC", "{0}", to);
            if(ModusRegistry.HasModus(to)) {
                Logger.Log("TESTB", "'{0}': '{1}'", to, (Sprite) ModusRegistry.GetFieldFromModus(to, "sprite"));
                ___image.sprite = (Sprite) ModusRegistry.GetFieldFromModus(to, "sprite");
                return false;
            }
            return true;
        }
        
        [ModPatch("prefix", "Assembly-CSharp", "ModusPickerComponent.Awake")]
        private static void CustomModusSetup_ModusPickerComponentAwake(ref string[] ___options) {
            if(___options != null) {
                foreach(string option in ModusRegistry.ModusNames) {
                    if(!___options.Contains(option)) {
                        ___options = ___options.AddItem(option).ToArray();
                    }
                }
            }
        }

        [ModPatch("prefix", "Assembly-CSharp", "Modus.SetIcon")]
        private static bool CustomModusSetup_ModusSetIcon(Modus __instance, string title) {
            Logger.Log("TEST??", "{0}", title);
            if(ModusRegistry.HasModus(title)) {
                __instance.sylladex.modusIcon.sprite = (Sprite) ModusRegistry.GetFieldFromModus(title, "sprite");
                Logger.Log("TEST", "{0}", title);
                return false;
            }
            return true;
        }

        [ModPatch("postfix", "Assembly-CSharp", "ModusPicker.ReadDescriptions")]
        private static void CustomModusSetup_ModusPickerReadDescriptions(ref Dictionary<string,string> ___modusDescription) {
            foreach(string modus in ModusRegistry.ModusNames) {
                ___modusDescription[modus] = (string) ModusRegistry.GetFieldFromModus(modus, "description");
                Logger.Log("READ", "{0}", modus);
            }
        }

        private readonly static MethodInfo m_Image__set_sprite = typeof(Image).GetMethod("set_sprite", BindingFlags.Public | BindingFlags.Instance);
        private readonly static MethodInfo m_ModusRegistry__GetFieldFromModus = typeof(ModusRegistry).GetMethod("GetFieldFromModus", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
        [ModPatch("transpiler", "Assembly-CSharp", "ModusPicker.AddModus")]
        private static IEnumerable<CodeInstruction> CustomModusSetup_ModusPickerAddModus(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
            LocalBuilder lb = generator.DeclareLocal(typeof(Sprite));
            foreach(CodeInstruction instruction in instructions) {
                yield return instruction;
                if(instruction.Calls(m_Image__set_sprite)) {
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    yield return new CodeInstruction(OpCodes.Ldstr, "sprite");
                    yield return new CodeInstruction(OpCodes.Call, m_ModusRegistry__GetFieldFromModus);
                    yield return new CodeInstruction(OpCodes.Castclass, typeof(Sprite));
                    yield return new CodeInstruction(OpCodes.Stloc_S, lb.LocalIndex);
                    yield return new CodeInstruction(OpCodes.Ldloc_1);
                    yield return new CodeInstruction(OpCodes.Ldloc_S, lb.LocalIndex);
                    yield return new CodeInstruction(OpCodes.Call, m_Image__set_sprite);
                }
            }
        }
    }
}
