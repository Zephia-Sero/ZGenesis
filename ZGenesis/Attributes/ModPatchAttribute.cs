using HarmonyLib;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ZGenesis.Attributes {
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ModPatchAttribute : Attribute {
        public string patchType;
        public Assembly asm;
        public string originalMethod;
        public ModPatchAttribute(string patchType, string asmName, string originalMethod) {
            this.patchType = patchType.ToLower();
            asm = AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(asm => asm.GetName().Name == asmName);
            this.originalMethod = originalMethod;
        }
        public override string ToString() {
            return string.Format("[ModPatch(\"{0}\", \"{1}\", \"{2}\")]", patchType, asm.GetName().Name, originalMethod);
        }
        public void Patch(Harmony harmony, MethodInfo patchMethod) {
            try {
                int splitidx = originalMethod.LastIndexOf('.');
                string className = originalMethod.Substring(0, splitidx);
                string methodName = originalMethod.Substring(1 + splitidx);
                Type type = asm.GetType(className) ?? throw new Exception($"Type {className} not found in assembly {asm}.");
                MethodInfo method = type.GetMethod(methodName,
                                    BindingFlags.Instance | BindingFlags.Static |
                                    BindingFlags.NonPublic | BindingFlags.Public)
                                    ?? throw new Exception($"Method {methodName} not found for type {className} in assembly {asm}.");               MethodBase methodB = method.GetBaseDefinition();
                switch(patchType) {
                case "prefix":
                    harmony.Patch(methodB, new HarmonyMethod(patchMethod));
                    break;
                case "postfix":
                    harmony.Patch(methodB, null, new HarmonyMethod(patchMethod));
                    break;
                case "transpiler":
                    harmony.Patch(methodB, null, null, new HarmonyMethod(patchMethod));
                    break;
                case "finalizer":
                    harmony.Patch(methodB, null, null, null, new HarmonyMethod(patchMethod));
                    break;
                default:
                    throw new Exception(string.Format("Invalid patch type: \"{0}\"", patchType));
                }
            } catch(Exception e) {
                Logger.Log(Logger.LogLevel.FATAL, "ZGenesis", "ERROR: Could not patch {0}:\n{1}", ToString(), e.ToString());
                Application.Quit(1);
            }
        }
    }
}
