namespace GRYLibrary.Core.Log.ConcreteLogTargets
{
    public class Observer : GRYLogTarget
    {
        public Observer() { }
        protected override void ExecuteImplementation(LogItem logItem, GRYLog logObject)
        {
            logObject.InvokeObserver(logItem);
        }
    }
}
