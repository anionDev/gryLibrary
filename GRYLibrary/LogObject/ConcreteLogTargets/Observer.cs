using GRYLibrary.Core.Miscellaneous;
using System;
using System.Collections.Generic;

namespace GRYLibrary.Core.LogObject.ConcreteLogTargets
{
    public sealed class Observer : GRYLogTarget
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
            Utilities.NoOperation();
        }
    }
}
