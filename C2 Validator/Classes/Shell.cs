/**
 * This file is part of C2 Validator <https://github.com/StevenJDH/C2-Validator>.
 * Copyright (C) 2020 Steven Jenkins De Haro.
 *
 * C2 Validator is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * C2 Validator is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with C2 Validator.  If not, see <http://www.gnu.org/licenses/>.
 */

using C2_Validator.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C2_Validator.Classes
{
    /// <summary>
    /// Provides shell access to run commands on a host system.
    /// </summary>
    public class Shell : ICommandable
    {
        /// <summary>
        /// Runs a shell command on a host system.
        /// </summary>
        /// <param name="command">Command to run.</param>
        /// <returns>Output from the executed command.</returns>
        public CommandOutput Run(string command)
        {
            var cmdOutput = new CommandOutput() { ExitCode = 1 };

            var startInfo = new ProcessStartInfo()
            {
                FileName = "cmd.exe",
                Arguments = $"/c \"{command}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.UTF8,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            try
            {
                using (var process = Process.Start(startInfo))
                {
                    if (process == null)
                    {
                        throw new InvalidOperationException("Failed to execute command.");
                    }
                    cmdOutput.StdOut = process.StandardOutput.ReadToEnd().Trim();
                    cmdOutput.StdErr = process.StandardError.ReadToEnd().Trim();
                    process.WaitForExit();
                    cmdOutput.ExitCode = process.ExitCode;
                }

                return cmdOutput;
            }
            catch (Exception ex)
            {
                cmdOutput.StdErr = ex.Message;

                return cmdOutput;
            }
        }
    }
}
