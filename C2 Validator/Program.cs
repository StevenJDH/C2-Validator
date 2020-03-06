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

using C2_Validator.Classes;
using C2_Validator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace C2_Validator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(GetLogo());

            try
            {
                var rootCert = GetRootCaCert(StoreName.My, StoreLocation.LocalMachine, GetPortBoundCertThumbprint());
                X509BasicConstraintsExtension basicConstraints = null;

                foreach (var extension in rootCert.Extensions)
                {
                    if (extension.Oid.FriendlyName != "Basic Constraints") continue;

                    basicConstraints = extension as X509BasicConstraintsExtension;
                    break;
                }

                var certDetails = GetCertDetails(rootCert, basicConstraints);

                Console.WriteLine(certDetails);

                if (basicConstraints == null || basicConstraints.CertificateAuthority == false || basicConstraints.Critical == false)
                {
                    Console.WriteLine("--ISSUE DETECTED--");
                    Console.WriteLine("You will need to regenerate the certificate above before upgrading.\n");
                }
                else
                {
                    Console.WriteLine("--CHECK PASSED--");
                    Console.WriteLine("It is safe to upgrade to Qlik Sense February 2020 or newer.\n");
                }
            }
            catch (NullReferenceException)
            {
                Console.WriteLine("Error: Certificate bound to port 4242 was not found in the 'Local Machine/Personal' store.\n");
            }
            catch (Exception ex) when(ex is CryptographicException || ex is StandardErrorException)
            {
                Console.WriteLine($"Error: {ex.Message}\n");
            }

            PauseConsoleForExit();
        }

        /// <summary>
        /// Gets a logo that is generated with author and version information.
        /// </summary>
        /// <returns>Text-based application logo.</returns>
        private static string GetLogo()
        {
            var v = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

            return $@"
  ____ ____   __     __    _ _     _       _             
 / ___|___ \  \ \   / /_ _| (_) __| | __ _| |_ ___  _ __ 
| |     __) |  \ \ / / _` | | |/ _` |/ _` | __/ _ \| '__|
| |___ / __/    \ V / (_| | | | (_| | (_| | || (_) | |   
 \____|_____|    \_/ \__,_|_|_|\__,_|\__,_|\__\___/|_|   
                           Steven Jenkins De Haro v{v.Major}.{v.Minor}.{v.Build}
                    ".Substring(2); // Removes the initial newline.
        }

        /// <summary>
        /// Gets the thumbprint of the Qlik Sense server certificate bound to port 4242.
        /// </summary>
        /// <returns>Certificate thumbprint.</returns>
        private static string GetPortBoundCertThumbprint()
        {
            ICommandable shell = new Shell();
            var cmdOutput = shell.Run("netsh http show sslcert ipport=0.0.0.0:4242");
            var outputLines = cmdOutput.StdOut.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            if (cmdOutput.ExitCode > 0 || outputLines.Length < 4)
            {
                throw new StandardErrorException(String.IsNullOrWhiteSpace(cmdOutput.StdErr) ? 
                    "No certificate binding found on port 4242. Make sure you are running this on your Qlik Sense server." : cmdOutput.StdErr);
            }

            return outputLines[3].Split(':')[1].Trim();
        }

        /// <summary>
        /// Gets the Root CA certificate of a certificate in the chain of trust.
        /// </summary>
        /// <param name="storeName">Certificate store such as Personal (My).</param>
        /// <param name="storeLocation">Certificate location such as Local Computer (Local Machine).</param>
        /// <param name="childThumbprint">Thumbprint of an intermediary certificate in the chain of trust.</param>
        /// <returns>Returns the Root CA certificate in the targeted chain of trust if found.</returns>
        private static X509Certificate2 GetRootCaCert(StoreName storeName, StoreLocation storeLocation, string childThumbprint)
        {
            // Not using 'Using' statements here to support .NET 4.5.x Framework.
            var store = new X509Store(storeName, storeLocation);
            var chain = new X509Chain();

            store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

            var results = store.Certificates.Find(X509FindType.FindByThumbprint, childThumbprint, false);

            if (results.Count == 0)
            {
                return null;
            }

            chain.Build(results[0]);
            store.Close();

            return chain.ChainElements[chain.ChainElements.Count - 1].Certificate;
        }

        /// <summary>
        /// Gets basic information about a Root certificate and its CA and Critical constraints.
        /// </summary>
        /// <param name="rootCert">Root certificate.</param>
        /// <param name="basicConstraints">Certificate's Basic Constraints.</param>
        /// <returns>Certificate information for display.</returns>
        private static string GetCertDetails(X509Certificate2 rootCert, X509BasicConstraintsExtension basicConstraints)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"      Name: {rootCert.GetNameInfo(X509NameType.SimpleName, forIssuer: false)}");
            sb.AppendLine($"Thumbprint: {rootCert.Thumbprint}");
            sb.AppendLine($"        CA: {(basicConstraints?.CertificateAuthority ?? false ? "Set" : "Missing")}");
            sb.AppendLine($"  Critical: {(basicConstraints?.Critical ?? false ? "Set" : "Missing")}");

            return sb.ToString();
        }

        /// <summary>
        /// Pauses the console execution before exiting until a key is pressed.
        /// </summary>
        private static void PauseConsoleForExit()
        {
            Console.Write("Press any key to exit . . .");
            Console.ReadKey(intercept: true); //Pause before closing workaround.
            Console.WriteLine("\n");
        }
    }
}
