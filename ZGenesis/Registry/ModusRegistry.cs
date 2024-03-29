﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace ZGenesis.Registry {
    public static class ModusRegistry {
        private static readonly Dictionary<string, Type> modusRegistry = new Dictionary<string, Type>();
        public static void RegisterModus(string modName, string name, Type type) {
            if(!modusRegistry.ContainsKey(name)) {
                modusRegistry[name] = type;
            } else {
                Logger.Log(Logger.LogLevel.ERROR, modName, "Modus named '{0}' already registered!", name);
            }
        }
        public static bool HasModus(string modusName) => modusRegistry.ContainsKey(modusName);
        public static string[] ModusNames => modusRegistry.Keys.ToArray();
        public static Type GetModus(string modusName) {
            if(HasModus(modusName)) {
                return modusRegistry[modusName];
            }
            Logger.Log(Logger.LogLevel.ERROR, "ZGenesis", "Modus named '{0}' has not been registered!", modusName);
            return null;
        }
        
        internal static object GetFieldFromModus(string modusName, string fieldName) {
            if(!modusRegistry.ContainsKey(modusName)) {
                Logger.Log(Logger.LogLevel.ERROR, "ZGenesis", "Modus named '{0}' has not been registered and thus cannot get the value of field '{1}'!", modusName, fieldName);
                return null;
            }
            FieldInfo field = modusRegistry[modusName].GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            if(field == null) {
                Logger.Log(Logger.LogLevel.ERROR, "ZGenesis", "Cannot find field '{0}' in modus '{1}'", fieldName, modusName);
                return null;
            }
            return field.GetValue(null); // Get static
        }
    }
}
