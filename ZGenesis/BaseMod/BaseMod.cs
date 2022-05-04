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
        private List<Card> items = new List<Card>();
        public override void Load(ModusData data) => throw new System.NotImplementedException();
        public override ModusData Save() => throw new System.NotImplementedException();

        protected static readonly string description;
        protected static readonly Sprite sprite;
        static TestModus() {
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(File.ReadAllBytes("test.png"));
            sprite = Sprite.Create(tex, new Rect(0,0,tex.width,tex.height), new Vector2(.5f,.5f));
            description = "A Test Modus.";
        }
        public TestModus() {
            itemCapacity = 8;
            separation = new Vector2(-complexcardsize.x / 4f, complexcardsize.y / 4f);
            SetIcon("Queue");
            SetColor(new Color(0, 255f, 0));
        }
        protected override bool AddItemToModus(Item toAdd) {
            if(items.Count >= itemCapacity) return false;
            items.Add(MakeCard(toAdd, 0, -1));
            return true;
        }
        protected override bool IsRetrievable(Card item) {
            if(items.Count == 0) return false;
            return item == items.First() || item == items.Last();
        }
        protected override bool RemoveItemFromModus(Card item) {
            if(!IsRetrievable(item)) return false;
            items.Remove(item);
            return true;
        }
        protected override IEnumerable<Card> GetItemList() => items;
        protected override void Load(Item[] itemsIn) {
            items = new List<Card>();

            for(int i = 0; i < itemsIn.Length; i++) {
                items.Add(MakeCard(itemsIn[i], i));
            }
        }
        public override int GetAmount() {
            return items.Count;
        }
    }
}
