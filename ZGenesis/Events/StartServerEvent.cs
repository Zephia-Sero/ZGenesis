using Mirror;

namespace ZGenesis.Events {
    public class StartServerEvent : NetworkManagerEvent {
        public StartServerEvent(NetworkManager netman) : base(netman) {}
    }
}
