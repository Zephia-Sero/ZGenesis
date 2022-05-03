using System.Collections.Generic;
using System.Linq;
using ZGenesis.Configuration;

namespace ZGenesis.Mod {
    public abstract class GenesisMod {
        public bool PatchingFinished { get; private set; }
        public GenesisMod() {
            // Verify that mod hasn't already been loaded.
            if(Patcher.loadedMods.All(mod => { return mod.ModNamespace != ModNamespace; })) {
                Patcher.loadedMods.Add(this);
            } else {
                Logger.Log(Logger.LogLevel.WARNING, Name, "Mod already loaded!");
            }
        }

        public bool TryPatch() {
            if(PatchingFinished) return true;
            PatchingFinished = true;
            patchers.ForEach(patcher => {
                PatchingFinished &= patcher.TryPatch();
            });
            return PatchingFinished;
        }
        public List<string> DependenciesUnavailable() {
            List<string> missingDependencies = new List<string>();
            patchers.ForEach(patcher => {
                missingDependencies.AddRange(patcher.DependenciesUnavailable());
            });
            return missingDependencies;
        }
        public void LoadConfig() {
            config.ForEach(cfgfile => {
                if(defaults.ContainsKey(cfgfile))
                    new ConfigFile(this, cfgfile, defaults[cfgfile]);
                else
                    new ConfigFile(this, cfgfile);
            });
        }
        protected void AddDefaultConfig(string filename, string key, ConfigValue value) {
            if(!config.Contains(filename)) Logger.Log(Logger.LogLevel.WARNING, Name, "Cannot add default config setting '{1} = {2}' for unloaded file '{0}'.", filename, key, value);
            else {
                if(!defaults.ContainsKey(filename)) defaults.Add(filename, new Dictionary<string, ConfigValue>());
                defaults[filename].Add(key, value);
            }
        }
        protected void AddDefaultConfig(int fileIndex, string key, ConfigValue value) {
            if(fileIndex >= config.Count) Logger.Log(Logger.LogLevel.WARNING, Name, "Cannot add default config setting '{1} = {2}'. Index {0} exceeds # of loaded config files.", fileIndex, key, value);
            else {
                string filename = config[fileIndex];
                if(!defaults.ContainsKey(filename)) defaults.Add(filename, new Dictionary<string, ConfigValue>());
                defaults[filename].Add(key, value);
            }
        }

        // Derive these!

        public abstract string Name { get; }
        public abstract string ModNamespace { get; }
        public abstract string Description { get; }
        public abstract string Version { get; }
        public virtual void PreConfig() { }
        public virtual void PostConfig() { }
        public virtual void PrePatch() { }
        public virtual void PostPatch() { }

        protected List<DependentPatcher> patchers = new List<DependentPatcher>();
        protected List<string> config = new List<string>();
        protected Dictionary<string, Dictionary<string, ConfigValue>> defaults = new Dictionary<string, Dictionary<string, ConfigValue>>();
    }
}
