using System;

namespace GRYLibrary.Core.Log
{
    public class LoggedMessageTypeConfiguration
    {
        public ConsoleColor ConsoleColor { get; set; }
        public string CustomText { get; set; }

         public override int GetHashCode()
        {
            return Utilities.GenericGetHashCode(this);
        }
        public override bool Equals(object obj)
        {
            return Utilities.GenericEquals(this, obj);
        }
    }

}
