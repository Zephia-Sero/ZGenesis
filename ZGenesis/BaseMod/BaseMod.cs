using System.IO;
using System.Linq;
using ZGenesis.Attributes;
using ZGenesis.Mod;
using ZGenesis.Configuration;
using ZGenesis.Registry;
using ZGenesis.Objects;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ZGenesis.BaseMod {
    [GenesisMod]
    public class BaseMod : GenesisMod {
        public override string Name => "BaseMod";
        public override string ModNamespace => "com.zgenesis";
        public override string Description => "Base ZGenesis mod, including many event hooks for other mods to use.";
        public override string Version => "v0.2.0";
        public override string[] Requires => new string[] { };
        internal static int MAX_PATCH_ATTEMPTS = 500;
        internal static Logger.LogLevel logLevel = Logger.LogLevel.INFO;
        internal static bool debugModeEnabled = false;
        internal static BaseMod Instance;
        public BaseMod() {
            Instance = this;
        }
        
        public override void PreConfig() {
            config.Add("ZGenesis.zcfg");
            AddDefaultConfig("ZGenesis.zcfg", "debug_mode", new ConfigValue(Name, debugModeEnabled, EConfigValueType.Bool));
            AddDefaultConfig("ZGenesis.zcfg", "log_level", new ConfigValue(Name, (byte) logLevel, EConfigValueType.U8));
        }
        public override void PostConfig() {
            debugModeEnabled = ConfigFile.loadedConfigFiles["ZGenesis.zcfg"]["debug_mode"].GetValue<bool>();
            logLevel = (Logger.LogLevel) ConfigFile.loadedConfigFiles["ZGenesis.zcfg"]["log_level"].GetValue<byte>();
        }
        public override void PrePatch() {
            patchers.Add(new DependentPatcher(this, "event.network", typeof(NetworkPatches)));
            //patchers[0].AddDependency("com.example.dependency");
            patchers.Add(new DependentPatcher(this, "event.sylladex", typeof(SylladexPatches)));
            //patchers[1].AddDependency("com.example.optional.soft.dependencies.*");
            patchers.Add(new DependentPatcher(this, "event.camera", typeof(CameraPatches)));
            patchers.Add(new DependentPatcher(this, "event.chat", typeof(ChatPatches)));

            patchers.ForEach(patcher => {
                // All BaseMod patches should be patched after any existing pre-basemod patches run.
                patcher.AddDependency("com.prebasemod.*");
            });
        }
    }
}
