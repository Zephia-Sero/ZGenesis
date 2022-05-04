﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZGenesis.Registry {
    public static class ModusRegistry {
        private static readonly Dictionary<string, Type> modusRegistry = new Dictionary<string, Type>();
        static ModusRegistry() {
            RegisterModus("ZGenesis", "Stack", typeof(FILOModus));
            RegisterModus("ZGenesis", "Queue", typeof(FIFOModus));
            RegisterModus("ZGenesis", "Tree", typeof(TreeModus));
            RegisterModus("ZGenesis", "Hashmap", typeof(HashmapModus));
            RegisterModus("ZGenesis", "Array", typeof(ArrayModus));
        }
        public static void RegisterModus(string modName, string name, Type type) {
            if(!modusRegistry.ContainsKey(name)) {
                modusRegistry[name] = type;
            } else {
                Logger.Log(Logger.LogLevel.ERROR, modName, "Modus named '{0}' already registered!", name);
            }
        }
        internal static object GetFieldFromModus(string modusName, string fieldName) {
            if(!modusRegistry.ContainsKey(modusName)) {
                Logger.Log(Logger.LogLevel.ERROR, "ZGenesis", "Modus named '{0}' as not been registered and thus cannot get the value of field '{1}'!", modusName, fieldName);
                return null;
            }
            FieldInfo field = modusRegistry[modusName].GetField(fieldName, BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            if(field == null) {
                Logger.Log(Logger.LogLevel.ERROR, "ZGenesis", "Cannot find field '{0}' in modus '{1}'", fieldName, modusName);
                return null;
            }
            return field.GetValue(null); // Get static
        }
    }
}
