using System;
using System.Collections.Generic;

namespace GRYLibrary.Core.Log.ConcreteLogTargets
{
    public class Observer : GRYLogTarget
    {
        public Observer() { }

        public override ISet<Type> FurtherGetExtraTypesWhichAreRequiredForSerialization()
        {
            return new HashSet<Type>();
        }

        protected override void ExecuteImplementation(LogItem logItem, GRYLog logObject)
        {
            logObject.InvokeObserver(logItem);
        }
        public override void Dispose()
        {
            GRYLibrary.Core.Utilities.NoOperation();
        }
    }
}
