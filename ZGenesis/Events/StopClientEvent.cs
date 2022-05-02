using Mirror;

namespace ZGenesis.Events {
    public class StopClientEvent : NetworkManagerEvent {
        public StopClientEvent(NetworkManager netman) : base(netman) {}
    }
}
