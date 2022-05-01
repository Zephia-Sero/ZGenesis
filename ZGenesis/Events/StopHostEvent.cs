using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZGenesis.Events {
    public class StopHostEvent : NetworkManagerEvent {
        public StopHostEvent(NetworkManager netman) : base(netman) {}
    }
}
