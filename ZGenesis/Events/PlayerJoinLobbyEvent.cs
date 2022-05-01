using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZGenesis.Events {
    public class PlayerJoinLobbyEvent : Event {
        /// <summary>
        /// Player ID
        /// </summary>
        public Mirror.NetworkConnection PlayerConnection { get; }
        /// <summary>
        /// Create a new event
        /// </summary>
        /// <param name="id">Player ID</param>
        public PlayerJoinLobbyEvent(Mirror.NetworkConnection conn) {
            PlayerConnection = conn;
        }
        /// <summary>
        /// Get string version of this object
        /// </summary>
        /// <returns>A string</returns>
        public override string ToString() {
            return $"{GetType().Name}:\n\tPlayerConnection:{PlayerConnection}";
        }
    }
}
