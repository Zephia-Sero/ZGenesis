using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using UnityEngine;
using ZGenesis.Events;
using ZGenesis.Mod;

namespace ZGenesis {
    public static class Patcher {
        public const bool DEBUG_MODE = true;
        private static readonly Queue<Event> eventQueue = new Queue<Event>();
        private static readonly List<Pair<List<Type>, Action<Event>>> eventHandlers = new List<Pair<List<Type>, Action<Event>>>();
        private static Thread eventThread;
        private static bool eventThreadRunning = false;
        public static readonly List<GenesisMod> loadedMods = new List<GenesisMod>();
        private const int MAX_DEPENDENCY_ATTEMPTS = 1000;
        static Patcher() {
            Logger.Log(Logger.LogLevel.ESSENTIAL, "ZGenesis", "Patcher successfully instantiated.");
            if(DEBUG_MODE) {
                Logger.Log(Logger.LogLevel.DEBUG, "ZGenesis", "DEBUG MODE ENABLED. Enabling Harmony debug mode.");
            }
            Harmony.DEBUG = DEBUG_MODE;
            Logger.Log(Logger.LogLevel.ESSENTIAL, "ZGenesis", "Loading ZGenesis BaseMod");
            GenesisMod basemod = new BaseMod.BaseMod();

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
            int i = 0;
            while(DependentPatcher.incompletePatches.Count > 0 && i < MAX_DEPENDENCY_ATTEMPTS) {
                loadedMods.ForEach(mod => {
                    if(!mod.PatchingFinished)
                        mod.TryPatch();
                });
                i++;
            }
            if(i == MAX_DEPENDENCY_ATTEMPTS) {
                Logger.Log(Logger.LogLevel.FATAL, "ZGenesis", "Mod patching took more than {0} cycles. Possible dependency cycle?", MAX_DEPENDENCY_ATTEMPTS);
                Application.Quit(1);
            }
            
            if(DEBUG_MODE)
                RegisterEventHandler(new List<Type> { typeof(Event) }, EventDebugger);
        }
        private static void EventDebugger(Event evt) {
            Logger.Log(Logger.LogLevel.DEBUG, "ZGenesis", evt.ToString());
        }
        private static void EventThreadFunc() {
            eventThreadRunning = true;
            while(eventQueue.Count > 0) {
                Event evt = eventQueue.Dequeue();
                foreach(Pair<List<Type>, Action<Event>> handler in eventHandlers) {
                    List<Type> types = handler.a;
                    foreach(Type type in types) {
                        if(evt.GetType().IsSubclassOf(type)) handler.b(evt);
                    }
                }
            }
            eventThreadRunning = false;
        }
        // EnqueueEvent method for transpilers.
        public readonly static MethodInfo m_Patcher__EnqueueEvent = typeof(Patcher).GetMethod(nameof(Patcher.EnqueueEvent));
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