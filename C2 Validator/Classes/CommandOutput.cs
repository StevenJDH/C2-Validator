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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C2_Validator.Classes
{
    /// <summary>
    /// Contains the output and status information for an executed command.
    /// </summary>
    public class CommandOutput
    {
        /// <summary>
        /// Contains the output from the standard error output device.
        /// </summary>
        public string StdOut { get; set; }

        /// <summary>
        /// Contains the output from the standard error output device. Check <see cref="ExitCode"/> to see
        /// if the output stream has been redirected here.
        /// </summary>
        public string StdErr { get; set; }

        /// <summary>
        /// Contains the status of the executed command with 0 meaning successful and 1 or more meaning it failed.
        /// </summary>
        public int ExitCode { get; set; }
    }
}
