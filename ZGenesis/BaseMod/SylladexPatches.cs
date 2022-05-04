using HarmonyLib;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using ZGenesis.Attributes;
using ZGenesis.Events;
using ZGenesis.Registry;
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
        private static bool CustomModusSetup_SylladexSetModus(Sylladex __instance, ref Modus ___captchaModus, ref GameObject ___modusObject, ref string ___modusName, AudioClip ___clipSwitch, string modus, bool playSound) {
            if(ModusRegistry.HasModus(modus)) {
                Type t = ModusRegistry.GetModus(modus);
                Component component = ___modusObject.AddComponent(t);
                ___captchaModus = (Modus) component;
                ___modusName = modus;
                if(playSound) {
                    __instance.PlaySoundEffect(___clipSwitch);
                }
                return false;
            }
            return true;
        }

        [ModPatch("prefix", "Assembly-CSharp", "Sylladex.Start")]
        private static void CustomModusSetup_SylladexStart(ref List<string> ___modi) {
            foreach(string modus in ModusRegistry.ModusNames) {
                if(!___modi.Contains(modus)) {
                    ___modi.Add(modus);
                }
            }
        }

        [ModPatch("prefix", "Assembly-CSharp", "AddCaptchamodusAction.Start")]
        private static bool CustomModusSetup_AddCaptchaModusActionStart(AddCaptchamodusAction __instance, ref Sprite ___sprite, ref string ___desc) {
            if(ModusRegistry.HasModus(__instance.modus)) {
                ___sprite = (Sprite) ModusRegistry.GetFieldFromModus(__instance.modus, "sprite");
                ___desc = "Get " + __instance.modus + "Modus";
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
            if(ModusRegistry.HasModus(title)) {
                __instance.sylladex.modusIcon.sprite = (Sprite) ModusRegistry.GetFieldFromModus(title, "sprite");
                return false;
            }
            return true;
        }

        [ModPatch("postfix", "Assembly-CSharp", "ModusPicker.ReadDescriptions")]
        private static void CustomModusSetup_ModusPickerReadDescriptions(ref Dictionary<string,string> ___modusDescription) {
            foreach(string modus in ModusRegistry.ModusNames) {
                ___modusDescription[modus] = (string) ModusRegistry.GetFieldFromModus(modus, "description");
            }
        }

        private readonly static MethodInfo m_ModusPicker__ReadDescriptions = typeof(ModusPicker).GetMethod("ReadDescriptions", BindingFlags.NonPublic | BindingFlags.Static);
        private readonly static FieldInfo f_ModusPicker__modusOption = typeof(ModusPicker).GetField("modusOption", BindingFlags.NonPublic | BindingFlags.Instance);
        private readonly static FieldInfo f_ModusPicker__modusDescription = typeof(ModusPicker).GetField("modusDescription", BindingFlags.NonPublic | BindingFlags.Static);
        private readonly static FieldInfo f_ModusPicker__OnPickModus = typeof(ModusPicker).GetField("OnPickModus", BindingFlags.NonPublic | BindingFlags.Instance);
        [ModPatch("prefix", "Assembly-CSharp", "ModusPicker.AddModus")]
        private static bool CustomModusSetup_ModusPickerAddModus(ModusPicker __instance, string modus) {
            m_ModusPicker__ReadDescriptions.Invoke(null, new object[] { });
            Image modusOption = (Image) f_ModusPicker__modusOption.GetValue(__instance);
            Image image = (modusOption.sprite == null) ? modusOption : UnityEngine.Object.Instantiate<Image>(modusOption, modusOption.transform.parent);
            image.sprite = Resources.Load<Sprite>("Modi/" + modus + "Modus");
            image.name = modus + "Modus";
            if(ModusRegistry.HasModus(modus)) {
                image.sprite = (Sprite) ModusRegistry.GetFieldFromModus(modus, "sprite");
            }
            Button.ButtonClickedEvent onClick = image.GetComponent<Button>().onClick;
            onClick.RemoveAllListeners();
            onClick.AddListener(delegate {
                Action<string> OnPickModus = (Action<string>) f_ModusPicker__OnPickModus.GetValue(__instance);
                OnPickModus(modus);
            });
            image.transform.GetChild(0).GetComponent<Text>().text = modus;
            if(((Dictionary<string, string>) f_ModusPicker__modusDescription.GetValue(null)).TryGetValue(modus, out string text)) {
                image.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = text;
                return false;
            }
            image.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
            return false;
        }
        /*
        private readonly static MethodInfo m_Image__set_sprite = typeof(Image).GetMethod("set_sprite", BindingFlags.Public | BindingFlags.Instance);
        private readonly static MethodInfo m_ModusRegistry__GetFieldFromModus = typeof(ModusRegistry).GetMethod("GetFieldFromModus", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
        [ModPatch("transpiler", "Assembly-CSharp", "ModusPicker.AddModus")]
        private static IEnumerable<CodeInstruction> CustomModusSetup_ModusPickerAddModus(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
            LocalBuilder lb = generator.DeclareLocal(typeof(Sprite));
            Label label =  generator.DefineLabel();
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
                    generator.MarkLabel(label);
                }
            }
        }*/
    }
}
