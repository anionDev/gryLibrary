using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Core.Exceptions
{
    public class UnexpectedExitCodeException : Exception
    {
        public ExternalProgramExecutor ExecutedProgram { get; }
        public UnexpectedExitCodeException(ExternalProgramExecutor externalProgramExecutor) : base(GetMessage(externalProgramExecutor))
        {
            this.ExecutedProgram = externalProgramExecutor;
        }

        private static string GetMessage(ExternalProgramExecutor externalProgramExecutor)
        {
            return $"'{externalProgramExecutor.Title}' had exitcode {externalProgramExecutor.ExitCode}.{Environment.NewLine}{Environment.NewLine}{externalProgramExecutor.GetResult()}";
        }
    }
}
