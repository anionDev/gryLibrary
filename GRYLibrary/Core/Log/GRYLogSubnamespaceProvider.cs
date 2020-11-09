using System;

namespace GRYLibrary.Core.Log
{
    public class GRYLogSubNamespaceProvider : IDisposable
    {
        private readonly GRYLog _LogObject;
        private readonly string _SubNamespace;
        public readonly string _OriginalNamespace;

        public GRYLogSubNamespaceProvider(GRYLog logObject, string subnamespace)
        {
            this._LogObject = logObject;
            subnamespace = subnamespace.Trim();
            this._SubNamespace = subnamespace;
            this._OriginalNamespace = this._LogObject.Configuration.Name;
            if (!string.IsNullOrEmpty(subnamespace))
            {
                string prefix;
                if (string.IsNullOrEmpty(this._LogObject.Configuration.Name))
                {
                    prefix = string.Empty;
                }
                else
                {
                    prefix = $"{this._LogObject.Configuration.Name}.";
                }
                this._LogObject.Configuration.Name = $"{prefix}{this._SubNamespace}";
            }
        }

        public void Dispose()
        {
            this._LogObject.Configuration.Name = this._OriginalNamespace;
        }
    }

}
