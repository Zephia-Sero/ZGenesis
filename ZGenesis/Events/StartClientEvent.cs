using Mirror;

namespace ZGenesis.Events {
    public class StartClientEvent : NetworkManagerEvent {
        public StartClientEvent(NetworkManager netman) : base(netman) {}
    }
}
