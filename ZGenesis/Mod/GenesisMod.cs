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
                new ConfigFile(this, cfgfile);
            });
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
    }
}
