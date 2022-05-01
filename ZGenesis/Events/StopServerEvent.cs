using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZGenesis.Events {
    public class StopServerEvent : NetworkManagerEvent {
        public StopServerEvent(NetworkManager netman) : base(netman) {}
    }
}
