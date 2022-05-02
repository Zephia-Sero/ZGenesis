namespace ZGenesis.Events {
    public abstract class NetworkManagerEvent : Event {
        public Mirror.NetworkManager NetManager { get; }
        public NetworkManagerEvent(Mirror.NetworkManager netman) {
            NetManager = netman;
        }
        public override string ToString() => $"{GetType().Name}\n\tNetwork Manager:{NetManager}";
    }
}
