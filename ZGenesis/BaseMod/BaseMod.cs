using ZGenesis.Attributes;
using ZGenesis.Mod;
using ZGenesis.Configuration;

namespace ZGenesis.BaseMod {
    [GenesisMod]
    public class BaseMod : GenesisMod {
        public override string Name => "BaseMod";
        public override string ModNamespace => "com.zgenesis";
        public override string Description => "Base ZGenesis mod, including many event hooks for other mods to use.";
        public override string Version => "v0.1.0";
        internal static int MAX_PATCH_ATTEMPTS = 500;
        internal static Logger.LogLevel logLevel = Logger.LogLevel.INFO;
        internal static bool debugModeEnabled = false;
        public BaseMod() { }
        public override void PreConfig() {
            config.Add("ZGenesis.zcfg");
            AddDefaultConfig("ZGenesis.zcfg", "com.zgenesis.debug_mode", new ConfigValue(Name, false, EConfigValueType.Bool));
        }
        public override void PostConfig() {
            // Should call ConfigFile.GetValue("com.zgenesis.debug_mode"); and stuff -- todo
        }
        public override void PrePatch() {
            patchers.Add(new DependentPatcher(this, "event.network", typeof(NetworkPatches)));
            //patchers[0].AddDependency("com.example.dependency");
            patchers.Add(new DependentPatcher(this, "event.sylladex", typeof(SylladexPatches)));
            //patchers[1].AddDependency("com.example.optional.soft.dependencies.*");
            patchers.Add(new DependentPatcher(this, "event.misc", typeof(MiscPatches)));

            patchers.ForEach(patcher => {
                // All BaseMod patches should be patched after any existing pre-basemod patches run.
                patcher.AddDependency("com.prebasemod.*");
            });
        }
    }
}
