using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZGenesis.Events {
    public class DragItemEvent : Event {
        Item item;
        Sylladex sylladex;
        public DragItemEvent(Item item, Sylladex sylladex) {
            this.item = item;
            this.sylladex = sylladex;
        }
        public override string ToString() => $"{GetType().Name}:\n\tSylladex:{sylladex}\n\tItem:{item}";
    }
}
