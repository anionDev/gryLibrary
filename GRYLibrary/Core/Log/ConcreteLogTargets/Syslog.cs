using System;
using System.Collections.Generic;

namespace GRYLibrary.Core.Log.ConcreteLogTargets
{
    public class Syslog : GRYLogTarget
    {

        protected override void ExecuteImplementation(LogItem logItem, GRYLog logObject)
        {
            string messageId;
            if (string.IsNullOrWhiteSpace(logItem.MessageId))
            {
                messageId = string.Empty;
            }else
            {
                messageId = $"--rfc5424 --msgid {logItem.MessageId}";
            }

            ExternalProgramExecutor externalProgramExecutor = ExternalProgramExecutor.Create("Logger", $"--tag {Utilities.GetNameOfCurrentExecutable()} {messageId} -- [{logItem.LogLevel}] [{logObject.Configuration.Name}] {logItem.PlainMessage}", System.IO.Directory.GetCurrentDirectory(), string.Empty, false,(int)Math.Round( TimeSpan.FromSeconds(20).TotalMilliseconds));
            externalProgramExecutor.ThrowErrorIfExitCodeIsNotZero = true;
            externalProgramExecutor.StartConsoleApplicationInCurrentConsoleWindow();
        }

        public override ISet<Type> FurtherGetExtraTypesWhichAreRequiredForSerialization()
        {
            return new HashSet<Type>();
        }
    }
}
