﻿using System.Diagnostics;
using vps.Constants;
using vps.Interfaces;

namespace vps.Operations
{
    public class ProcessOperation : IProcessOperation
    {
        public bool ExecuteCommand(string command, out int exitCode, out string output, out string error)
        {
            using var process = Process.Start(new ProcessStartInfo
            {
                FileName = CommonConstants.CommandShell,
                Arguments = command,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            });

            process.WaitForExit();

            output = process.StandardOutput.ReadToEnd();
            error = process.StandardError.ReadToEnd();
            exitCode = process.ExitCode;

            return process.ExitCode.Equals(CommonConstants.ProcessSuccessfulExitCode);
        }
    }
}
