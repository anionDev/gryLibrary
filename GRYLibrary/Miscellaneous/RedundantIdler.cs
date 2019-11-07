using System;
using System.Diagnostics;

namespace GRYLibrary.Miscellaneous
{
    public class RedundantIdler
    {
        public static void WaitUntilExecutionIsRequired()
        {
            if (ThisProgramIsFirstInstance())
            {
                StartBackupInstance();
            }
            else
            {
                WaitUntilOtherProgramFinished();
            }
        }

        private static void WaitUntilOtherProgramFinished()
        {
            throw new NotImplementedException();
        }

        private static void StartBackupInstance()
        {
            throw new NotImplementedException();
        }

        private static bool ThisProgramIsFirstInstance()
        {
            return GetAmountOfProcessesWithNameOfCurrentProcess() == 1;
        }

        private static int GetAmountOfProcessesWithNameOfCurrentProcess()
        {
            string currentProcessName = Process.GetCurrentProcess().ProcessName;
            return Process.GetProcessesByName(currentProcessName).Length;
        }
    }
}
