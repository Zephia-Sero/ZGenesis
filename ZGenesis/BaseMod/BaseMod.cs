using System.IO;
using ZGenesis.Attributes;
using ZGenesis.Mod;
using ZGenesis.Configuration;
using ZGenesis.Registry;
using ZGenesis.Objects;
using UnityEngine;

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
            patchers.Add(new DependentPatcher(this, "event.misc", typeof(MiscPatches)));

            patchers.ForEach(patcher => {
                // All BaseMod patches should be patched after any existing pre-basemod patches run.
                patcher.AddDependency("com.prebasemod.*");
            });
        }
        public override void PostPatch() {
            ModusRegistry.RegisterModus(Name, "Test Modus", typeof(TestModus));
        }
    }
    public class TestModus : ModdedModus {
        public override void Load(ModusData data) => throw new System.NotImplementedException();
        public override ModusData Save() => throw new System.NotImplementedException();
        public static new string Description => description;
        private static readonly string description;
        public static new Sprite Sprite => sprite;
        private static readonly Sprite sprite;
        static TestModus() {
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(File.ReadAllBytes("test.png"));
            sprite = Sprite.Create(tex, new Rect(0,0,tex.width,tex.height), new Vector2(.5f,.5f));
            description = "A Test Modus.";
        }
    }
}
