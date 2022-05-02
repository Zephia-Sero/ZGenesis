namespace ZGenesis.Events {
    public class Event {
        public Event() {
            Patcher.EnqueueEvent(this);
        }
        public override string ToString() => $"{GetType().Name}";
    }
}
