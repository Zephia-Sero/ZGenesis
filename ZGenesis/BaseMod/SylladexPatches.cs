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
        private static void CustomModusSetup_SylladexSetModus(Sylladex __instance, ref Modus ___captchaModus, ref GameObject ___modusObject, ref string ___modusName, AudioClip __clipSwitch, string modus, bool playSound) {
            if(ModusRegistry.HasModus(modus)) {
                ___captchaModus = (Modus) ___modusObject.AddComponent(ModusRegistry.GetModus(modus));
                ___modusName = modus;
                if(playSound) {
                    __instance.PlaySoundEffect(__clipSwitch);
                }
            }
        }

        [ModPatch("prefix", "Assembly-CSharp", "AddCaptchamodusAction.Start")]
        private static bool CustomModusSetup_AddCaptchaModusActionStart(AddCaptchamodusAction __instance, ref Sprite ___sprite) {
            if(ModusRegistry.HasModus(__instance.modus)) {
                ___sprite = (Sprite) ModusRegistry.GetFieldFromModus(__instance.modus, "sprite");
                return false;
            }
            return true;
        }

        [ModPatch("prefix", "Assembly-CSharp", "ModusPickerComponent.ModusChange")]
        private static bool CustomModusSetup_ModPickerComponentModusChange(ref Image ___image, string to) {
            if(ModusRegistry.HasModus(to)) {
                ___image.sprite = (Sprite) ModusRegistry.GetFieldFromModus(to, "sprite");
                return false;
            }
            return true;
        }

        [ModPatch("prefix", "Assembly-CSharp", "ModusPickerComponent.Awake")]
        private static void CustomModusSetup_ModusPickerComponentAwake(string[] ___options) {
            if(___options != null) {
                foreach(string option in ModusRegistry.ModusNames) {
                    if(!___options.Contains(option)) {
                        ___options = (string[]) ___options.AddItem(option);
                    }
                }
            }
        }

        [ModPatch("prefix", "Assembly-CSharp", "Modus.SetIcon")]
        private static bool CustomModusSetup_ModusSetIcon(Modus __instance, string title) {
            if(ModusRegistry.HasModus(title)) {
                __instance.sylladex.modusIcon.sprite = (Sprite) ModusRegistry.GetFieldFromModus(title, "sprite");
                return false;
            }
            return true;
        }

        [ModPatch("postfix", "Assembly-CSharp", "ModusPicker.ReadDescriptions")]
        private static void CustomModusSetup_ModusPickerReadDescriptions(ref Dictionary<string,string> __modusDescription) {
            foreach(string modus in ModusRegistry.ModusNames) {
                __modusDescription[modus] = (string) ModusRegistry.GetFieldFromModus(modus, "description");
            }
        }

        [ModPatch("postfix", "Assembly-CSharp", "ModusPicker.AddModus")]
        private static void CustomModusSetup_ModusPickerAddModus(string modus, ref Image image) {
            if(ModusRegistry.HasModus(modus)) {
                image.sprite = (Sprite) ModusRegistry.GetFieldFromModus(modus, "sprite");
            }
        }
    }
}
