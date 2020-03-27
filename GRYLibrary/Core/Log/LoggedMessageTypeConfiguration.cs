using System;

namespace GRYLibrary.Core.Log
{
    public class LoggedMessageTypeConfiguration
    {
        public ConsoleColor ConsoleColor { get; set; }
        public string CustomText { get; set; }
        public override bool Equals(object obj)
        {
            LoggedMessageTypeConfiguration typedObject = obj as LoggedMessageTypeConfiguration;
            if (typedObject == null)
            {
                return false;
            }
            if (this.ConsoleColor != typedObject.ConsoleColor)
            {
                return false;
            }
            if (this.CustomText != typedObject.CustomText)
            {
                return false;
            }
            return true;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(this.CustomText);
        }
    }

}
