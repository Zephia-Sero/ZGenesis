namespace ZGenesis.Events {
    public class Event {
        public Event() { }
        public override string ToString() => $"{GetType().Name}";
    }
}
