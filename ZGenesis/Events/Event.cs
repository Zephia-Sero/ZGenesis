using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZGenesis.Events {
    public class Event {
        public Event() { }
        public override string ToString() => $"{GetType().Name}";
    }
}
