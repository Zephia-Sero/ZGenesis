using Mirror;

namespace ZGenesis.Events {
    public class StopServerEvent : NetworkManagerEvent {
        public StopServerEvent(NetworkManager netman) : base(netman) {}
    }
}
