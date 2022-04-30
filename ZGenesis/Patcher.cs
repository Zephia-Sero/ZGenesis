using System.Collections.Generic;
using System;
using HarmonyLib;
using System.Threading;
using ZGenesis.Events;

namespace ZGenesis {
    public class Patcher {
        public const bool DEBUG_MODE = true;
        private static readonly Queue<Event> eventQueue = new Queue<Event>();
        private static readonly List<Pair<List<Type>, Action<Event>>> eventHandlers = new List<Pair<List<Type>, Action<Event>>>();
        private static Thread eventThread;
        private static bool eventThreadRunning = false;
        public Patcher() {
            Logger.Log("ZGenesis", "Patcher successfully instantiated.");
            if(DEBUG_MODE) {
                Logger.Log("ZGenesis", "DEBUG MODE ENABLED. Enabling Harmony debug mode.");
            }
            Harmony.DEBUG = DEBUG_MODE;
            Logger.Log("ZGenesis", "Instantiating Harmony");
            Harmony harmony = new Harmony("com.zgenesis.patch");
            EventPatches.PatchAll(harmony);
            Logger.Log("ZGenesis", "Harmony patching completed.");
            RegisterEventHandler(new List<Type> { typeof(Event) }, EventDebugger);
        }
        private static void EventDebugger(Event evt) {
            Logger.Log("ZGenesis DEBUG", evt.ToString());
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