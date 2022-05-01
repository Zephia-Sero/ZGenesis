using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZGenesis.Events {
    public class DragItemEvent : Event {
        public Item Item {get;}
        public Sylladex Sylladex {get;}
        public DragItemEvent(Sylladex sylladex, Item item) {
            Item = item;
            Sylladex = sylladex;
        }
        public override string ToString() => $"{GetType().Name}:\n\tSylladex:{Sylladex}\n\tItem:{Item}";
    }
}
