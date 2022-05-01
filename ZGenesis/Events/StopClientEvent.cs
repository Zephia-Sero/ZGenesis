using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZGenesis.Events {
    public class StopClientEvent : NetworkManagerEvent {
        public StopClientEvent(NetworkManager netman) : base(netman) {}
    }
}
