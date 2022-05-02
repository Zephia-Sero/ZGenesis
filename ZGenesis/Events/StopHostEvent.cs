using Mirror;

namespace ZGenesis.Events {
    public class StopHostEvent : NetworkManagerEvent {
        public StopHostEvent(NetworkManager netman) : base(netman) {}
    }
}
