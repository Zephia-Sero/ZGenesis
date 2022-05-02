using System.Collections.Generic;
using System.Linq;

namespace ZGenesis.Mod {
    public abstract class GenesisMod {
        public abstract string Name { get; }
        public abstract string ModNamespace { get; }
        public abstract string Description { get; }
        public abstract string Version { get; }
        protected List<DependentPatcher> patchers = new List<DependentPatcher>();
        public bool PatchingFinished { get; private set; }

        public GenesisMod() {
            // Verify that mod hasn't already been loaded.
            if(Patcher.loadedMods.All(mod => { return mod.ModNamespace != ModNamespace; })) {
                Patcher.loadedMods.Add(this);
            }
        }
        public abstract void RegisterEventHandlers();
        public virtual bool TryPatch() {
            if(PatchingFinished) return true;
            PatchingFinished = true;
            patchers.ForEach(patcher => {
                PatchingFinished &= patcher.TryPatch(); 
            });
            return PatchingFinished;
        }
        public virtual List<string> DependenciesUnavailable() {
            List<string> missingDependencies = new List<string>();
            patchers.ForEach(patcher => {
                missingDependencies.AddRange(patcher.DependenciesUnavailable());
            });
            return missingDependencies;
        }
    }
}
