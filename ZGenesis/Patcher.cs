using HarmonyLib;
using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Threading;
using UnityEngine;
using ZGenesis.Events;
using ZGenesis.Mod;
using ZGenesis.BaseMod;
using ZGenesis.Configuration;

namespace ZGenesis {
    public static class Patcher {
        private static readonly Queue<Event> eventQueue = new Queue<Event>();
        private static readonly List<Pair<List<Type>, Action<Event>>> eventHandlers = new List<Pair<List<Type>, Action<Event>>>();
        private static Thread eventThread;
        private static bool eventThreadRunning = false;
        public static readonly List<GenesisMod> loadedMods = new List<GenesisMod>();

        static Patcher() {
            Logger.Log(Logger.LogLevel.ESSENTIAL, "ZGenesis", "Patcher successfully instantiated.");
            
            LoadMods();
            ConfigureMods();
            if(BaseMod.BaseMod.debugModeEnabled) {
                Logger.Log(Logger.LogLevel.DEBUG, "ZGenesis", "DEBUG MODE ENABLED. Enabling Harmony debug mode.");
                Harmony.DEBUG = true;
            }
            PatchMods();

            Logger.Log(Logger.LogLevel.ESSENTIAL, "ZGenesis", "Mod loading completed.");


            if(BaseMod.BaseMod.debugModeEnabled) {
                RegisterEventHandler(new List<Type> { typeof(Event) }, evt => {
                    Logger.Log(Logger.LogLevel.DEBUG, "ZGenesis", evt.ToString());
                });
            }
        }
        private static void LoadMods() {
            Logger.Log(Logger.LogLevel.ESSENTIAL, "ZGenesis", "Loading mod assemblies");
            LoadModAssemblies().ForEach(type => {
                type.GetConstructor(new Type[] { }).Invoke(new object[] { });
            });
            Logger.Log(Logger.LogLevel.ESSENTIAL, "ZGenesis", "Checking mod dependencies");
            bool satisfiedDependencies = true;
            loadedMods.ForEach(mod => {
                List<string> missing = mod.DependenciesUnavailable();
                if(missing.Count > 0) {
                    satisfiedDependencies = false;
                    missing.ForEach(dependency => {
                        Logger.Log(Logger.LogLevel.FATAL, mod.Name, "Could not find patcher dependency '" + dependency + "'!");
                    });
                }
            });
            if(!satisfiedDependencies) {
                Logger.Log(Logger.LogLevel.FATAL, "ZGenesis", "Could not load mods. Reason: Failed to find required patcher dependencies.");
                Application.Quit(1);
            }
        }
        private static void ConfigureMods() {
            Logger.Log(Logger.LogLevel.ESSENTIAL, "ZGenesis", "Stage: PreConfig");
            loadedMods.ForEach(mod => { mod.PreConfig(); });
            Logger.Log(Logger.LogLevel.ESSENTIAL, "ZGenesis", "Loading mod configs");
            loadedMods.ForEach(mod => {
                mod.LoadConfig();
            });
            Logger.Log(Logger.LogLevel.ESSENTIAL, "ZGenesis", "Stage: PostConfig");
            loadedMods.ForEach(mod => { mod.PostConfig(); });
        }
        private static void PatchMods() {
            Logger.Log(Logger.LogLevel.ESSENTIAL, "ZGenesis", "Stage: PrePatch");
            loadedMods.ForEach(mod => { mod.PostPatch(); });
            Logger.Log(Logger.LogLevel.ESSENTIAL, "ZGenesis", "Patching mods");
            int i = 0;
            while(DependentPatcher.incompletePatches.Count > 0 && i < BaseMod.BaseMod.MAX_PATCH_ATTEMPTS) {
                loadedMods.ForEach(mod => {
                    if(!mod.PatchingFinished)
                        mod.TryPatch();
                });
                i++;
            }
            if(i == BaseMod.BaseMod.MAX_PATCH_ATTEMPTS) {
                Logger.Log(Logger.LogLevel.FATAL, "ZGenesis", "Mod patching took more than {0} cycles. Possible dependency cycle?", BaseMod.BaseMod.MAX_PATCH_ATTEMPTS);
                Application.Quit(1);
            }
            Logger.Log(Logger.LogLevel.ESSENTIAL, "ZGenesis", "Stage: PostPatch");
            loadedMods.ForEach(mod => { mod.PostPatch(); });
        }
        private static List<Type> LoadModAssemblies() {
            List<Type> loadedTypes = new List<Type>();
            bool loadFailed = false;
            foreach(string filename in Directory.GetFiles("mods/", "*.dll").AddItem("ZGenesis.dll")) {
                try {
                    Assembly asm = Assembly.GetAssembly(typeof(Patcher));
                    if(filename != "ZGenesis") {
                        asm = Assembly.LoadFrom(filename);
                        if(asm == null) {
                            Logger.Log(Logger.LogLevel.FATAL, "ZGenesis", "Failed to load modfile '{0}'. Assembly could not be loaded for unknown reason.", filename);
                            loadFailed = true;
                        }
                    }
                    foreach(Type type in asm.ExportedTypes) {
                        if(type.IsSubclassOf(typeof(GenesisMod))) {
                            IEnumerable<Attributes.GenesisModAttribute> attrs = type.GetCustomAttributes<Attributes.GenesisModAttribute>();
                            int num = attrs.Count();
                            if(num == 0) {
                                Logger.Log(Logger.LogLevel.ERROR, "ZGenesis", "No GenesisMod attribute found. Modfile: '{0}', type: {1}", filename, type.ToString());
                                continue;
                            }
                            if(num > 1) {
                                Logger.Log(Logger.LogLevel.ERROR, "ZGenesis", "More than one GenesisMod attribute found. Modfile: '{0}', type: {1}", filename, type.ToString());
                                continue;
                            }
                            Logger.Log(Logger.LogLevel.INFO, "ZGenesis", "Found Mod class in '{0}': {1}", filename, type.ToString());
                            loadedTypes.Add(type);
                        }
                    }
                } catch(Exception e) {
                    Logger.Log(Logger.LogLevel.FATAL, "ZGenesis", "Failed to load modfile '{0}'. Assembly could not be loaded.\n{1}", filename, e);
                    loadFailed = true;
                }
            }
            if(loadFailed) {
                Logger.Log(Logger.LogLevel.FATAL, "ZGenesis", "Failed to load mods from mods/ directory for above reasons. Exiting");
                Application.Quit(1);
            }
            return loadedTypes;
        }
        private static void EventThreadFunc() {
            eventThreadRunning = true;
            while(eventQueue.Count > 0) {
                Event evt = eventQueue.Dequeue();
                foreach(Pair<List<Type>, Action<Event>> handler in eventHandlers) {
                    List<Type> types = handler.a;
                    foreach(Type type in types) {
                        if(evt.GetType().IsSubclassOf(type) || evt.GetType() == type) handler.b(evt);
                    }
                }
            }
            eventThreadRunning = false;
        }
        public static void EnqueueEvent(Event e) {
            eventQueue.Enqueue(e);
            if(!eventThreadRunning) {
                eventThread = new Thread(EventThreadFunc);
                eventThread.Start();
            }
        }
        public static void RegisterEventHandler(List<Type> eventTypes, Action<Event> handler) {
            eventHandlers.Add(new Pair<List<Type>, Action<Event>>(eventTypes, handler));
        }
    }
}