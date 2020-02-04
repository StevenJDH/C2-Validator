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
    /// Exception thrown when a <see cref="Shell"/> redirects a stream to the standard error output device.
    /// </summary>
    [Serializable]
    class StandardErrorException : Exception
    {
        /// <summary>
        /// Initializes a new instance of <see cref="StandardErrorException"/>.
        /// </summary>
        public StandardErrorException() : base() { }

        /// <summary>
        /// Initializes a new instance of <see cref="StandardErrorException"/>.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public StandardErrorException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of <see cref="StandardErrorException"/>.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="inner">The inner exception.</param>
        public StandardErrorException(string message, Exception inner) : base(message, inner) { }
    }
}
