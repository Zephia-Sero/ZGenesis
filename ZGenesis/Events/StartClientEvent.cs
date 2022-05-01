using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZGenesis.Events {
    public class StartClientEvent : NetworkManagerEvent {
        public StartClientEvent(NetworkManager netman) : base(netman) {}
    }
}
