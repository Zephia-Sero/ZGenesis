using Mirror;

namespace ZGenesis.Events {
    public class StartHostEvent : NetworkManagerEvent {
        public StartHostEvent(NetworkManager netman) : base(netman) {}
    }
}
