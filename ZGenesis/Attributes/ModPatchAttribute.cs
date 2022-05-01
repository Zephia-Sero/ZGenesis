using System;
using HarmonyLib;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;

namespace ZGenesis.Attributes {
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ModPatchAttribute : Attribute {
        public string patchType;
        public Assembly asm;
        public string originalMethod;
        public string[] dependencies;
        public ModPatchAttribute(string patchType, string asmName, string originalMethod, string[] dependencies) {
            this.patchType = patchType.ToLower();
            asm = AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(asm => asm.GetName().Name == asmName);
            this.originalMethod = originalMethod;
            this.dependencies = dependencies;
        }
        public ModPatchAttribute(string patchType, string asmName, string originalMethod) : this(patchType, asmName, originalMethod, new string[] { }) { }
        public override string ToString() {
            if(dependencies.Length == 0)
                return string.Format("[ModPatch(\"{0}\", \"{1}\", \"{2}\")]", patchType, asm.GetName().Name, originalMethod);
            return string.Format("[ModPatch(\"{0}\", \"{1}\", \"{2}\", {3})]", patchType, asm.GetName().Name, originalMethod, dependencies);
        }
        public void Patch(Harmony harmony, MethodInfo patchMethod) {
            try {
                Logger.Log("ZGD", "Finding things...");
                int splitidx = originalMethod.LastIndexOf('.');
                string className = originalMethod.Substring(0, splitidx);
                string methodName = originalMethod.Substring(1 + splitidx);
                Type type = asm.GetType(className) ?? throw new Exception($"Type {className} not found in assembly {asm}.");
                MethodInfo method = type.GetMethod(methodName) ?? throw new Exception($"Method {methodName} not found for type {className} in assembly {asm}.");
                MethodBase methodB = method.GetBaseDefinition();
                Logger.Log("ZGD", "Patching now!");
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
                Logger.Log(Logger.LogLevel.FATAL,"ZGenesis", "ERROR: Could not patch {0}:\n{1}", ToString(), e.ToString());
                Application.Quit(1);
            }
        }
    }
}
