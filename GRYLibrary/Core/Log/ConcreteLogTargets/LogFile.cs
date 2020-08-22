using System;
using System.Collections.Generic;
using System.Text;

namespace GRYLibrary.Core.Log.ConcreteLogTargets
{
    public class LogFile : GRYLogTarget
    {
        public LogFile() { }
        public string File { get; set; }
        public string Encoding { get; set; } = "utf-8";

        protected override void ExecuteImplementation(LogItem logItem, GRYLog logObject)
        {
            if (string.IsNullOrWhiteSpace(this.File))
            {
                throw new NullReferenceException($"LogFile is not defined");
            }
            string file = Utilities.ResolveToFullPath(this.File);
            Utilities.EnsureFileExists(file, true);
            logItem.Format(logObject.Configuration, out string formattedMessage, out int _, out int _, out ConsoleColor _, GRYLogLogFormat.OnlyMessage);
            if (!Utilities.FileIsEmpty(file))
            {
                formattedMessage = Environment.NewLine + formattedMessage;
            }
            Encoding encoding;
            if (this.Encoding.Equals("utf-8"))
            {
                encoding = new UTF8Encoding(false);
            }
            else
            {
                encoding = System.Text.Encoding.GetEncoding(this.Encoding);
            }
            System.IO.File.AppendAllText(file, formattedMessage, encoding);
        }
        public override ISet<Type> FurtherGetExtraTypesWhichAreRequiredForSerialization()
        {
            return new HashSet<Type>();
        }
    }
}
