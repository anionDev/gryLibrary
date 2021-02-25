using GRYLibrary.Core.OperatingSystem.ConcreteOperatingSystems;
using System.IO;
using System.Runtime.InteropServices;

namespace GRYLibrary.Core.OperatingSystem
{
    public abstract class OperatingSystem
    {
        public abstract void Accept(IOperatingSystemVisitor visitor);
        public abstract T Accept<T>(IOperatingSystemVisitor<T> visitor);
        public static OperatingSystem GetCurrentOperatingSystem()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return Windows.Instance;
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return Windows.Instance;
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return Windows.Instance;
            }
            throw new InvalidDataException("The current OS can not be determined");
        }
    }
    public interface IOperatingSystemVisitor
    {
        void Handle(OSX operatingSystem);
        void Handle(Windows operatingSystem);
        void Handle(Linux operatingSystem);
    }
    public interface IOperatingSystemVisitor<T>
    {
        T Handle(OSX operatingSystem);
        T Handle(Windows operatingSystem);
        T Handle(Linux operatingSystem);
    }
}
