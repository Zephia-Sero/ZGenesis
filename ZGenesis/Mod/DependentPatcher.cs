using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using ZGenesis.Attributes;

namespace ZGenesis.Mod {
    public class DependentPatcher {
        public static List<string> incompletePatches = new List<string>();
        public static List<string> completedPatches = new List<string>();
        
        public List<string> dependencies = new List<String>();
        public bool patchingFinished = false;

        public Harmony harmonyInstance;
        private readonly Type patchHolder;
        
        public DependentPatcher(GenesisMod mod, string id, Type patchHolder) {
            string patchID = mod.ModNamespace + "." + id;
            if(!incompletePatches.Contains(patchID) && !completedPatches.Contains(patchID)) {
                incompletePatches.Add(patchID);
            } else {
                Logger.Log(Logger.LogLevel.FATAL, mod.Name, "Patch by patch ID '" + patchID + "' has already been added!");
                Application.Quit(1);
            }
            harmonyInstance = new Harmony(patchID);
            this.patchHolder = patchHolder;
        }
        public bool TryPatch() {
            if(patchingFinished) return true;
            if(DependenciesSatisfied())
                ForcePatch();
            return patchingFinished;
        }
        public void ForcePatch() {
            Logger.Log("ZGenesis", "Patching '{0}'", harmonyInstance.Id);
            MethodInfo[] methods = patchHolder.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            foreach(MethodInfo method in methods) {
                Attribute[] attrs = Attribute.GetCustomAttributes(method);
                foreach(Attribute attr in attrs) {
                    if(attr is ModPatchAttribute modPatch) {
                        modPatch.Patch(harmonyInstance, method);
                    }
                }
            }
            incompletePatches.Remove(harmonyInstance.Id);
            completedPatches.Add(harmonyInstance.Id);
            patchingFinished = true; 
        }
        public bool DependenciesSatisfied() {
            return dependencies.All(dependency => {
                if(dependency.EndsWith("*")) {
                    string depModule = dependency.Substring(0, dependency.IndexOf('*')-1);
                    return !incompletePatches.Any(patchID => {
                        return patchID.StartsWith(depModule);
                    });
                } else {
                    return completedPatches.Contains(dependency);
                }
            });
        }
        public IEnumerable<string> DependenciesUnavailable() {
            return dependencies.Where(dependency => {
                if(dependency.EndsWith("*")) return false;
                else {
                    return !incompletePatches.Contains(dependency);
                }
            });
        }
        public bool AddDependency(string dependency) {
            if(!patchingFinished && !dependencies.Contains(dependency)) {
                dependencies.Add(dependency);
                return true;
            }
            return false;
        }
        public bool RemoveDependency(string dependency) {
            if(!patchingFinished && dependencies.Contains(dependency)) {
                dependencies.Remove(dependency);
                return true;
            }
            return false;
        }
    }
}
