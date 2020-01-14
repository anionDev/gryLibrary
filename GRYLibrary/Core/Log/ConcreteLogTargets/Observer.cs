namespace GRYLibrary.Core.Log.ConcreteLogTargets
{
    public class Observer : GRYLogTarget
    {
        private Observer() { }
        public static Observer Instance { get; } = new Observer();
        protected override void ExecuteImplementation(LogItem logItem, GRYLog logObject)
        {
            logObject.InvokeObserver(logItem);
        }
    }
}
