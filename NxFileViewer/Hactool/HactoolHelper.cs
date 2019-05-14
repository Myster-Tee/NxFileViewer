using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Emignatik.NxFileViewer.Logging;

namespace Emignatik.NxFileViewer.Hactool
{
    public class HactoolHelper
    {
        private const string HACTOOL_NAME = "Hactool";

        public string HactoolFilePath { get; set; }

        public string KeySetFilePath { get; set; }

        public void NcaDecrypt(string srcNcaFilePath, string dstNcaFilePath)
        {
            try
            {
                RunHactool(new List<string> { $"--plaintext={dstNcaFilePath}", srcNcaFilePath });
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to decrypt NCA \"{srcNcaFilePath}\".", ex);
            }
        }

        public void NcaDecryptSectionToDir(string ncaFilePath, NcaSection ncaSection, string targetDirectory)
        {
            var sectionIndex = (int)ncaSection;
            try
            {
                RunHactool(new List<string> { $"--section{sectionIndex}dir", targetDirectory, ncaFilePath });
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to decrypt NCA section \"{sectionIndex}\".", ex);
            }
        }

        private void RunHactool(List<string> args, bool setKey = true)
        {
            var hactoolFilePath = this.HactoolFilePath;
            if (string.IsNullOrWhiteSpace(hactoolFilePath))
                throw new Exception($"\"{HACTOOL_NAME}\" path not defined.");

            if (!File.Exists(hactoolFilePath))
                throw new Exception($"\"{HACTOOL_NAME}\" not found at location \"{hactoolFilePath}\".");

            var keySetFilePath = KeySetFilePath;
            if (keySetFilePath != null && setKey)
            {
                args.Insert(0, "-k");
                args.Insert(1, keySetFilePath);
            }

            var arguments = StringifyArgs(args);
            var processStartInfo = new ProcessStartInfo(hactoolFilePath, arguments)
            {
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                CreateNoWindow = true,
                WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
            };

            using (var process = Process.Start(processStartInfo))
            {
                if (process == null)
                    throw new Exception($"Failed to start {HACTOOL_NAME} process, null process returned.");

                var stdOut = "";
                var stdErr = "";
                process.OutputDataReceived += (sender, eventArgs) =>
                {
                    stdOut += eventArgs.Data + Environment.NewLine;
                };
                process.ErrorDataReceived += (sender, eventArgs) =>
                {
                    var message = eventArgs?.Data ?? "";
                    const string WARN = "[WARN]: ";
                    if (message.StartsWith(WARN))
                    {
                        var warning = message.Substring(WARN.Length);
                        Logger.LogWarning(warning);
                    }
                    else
                    {
                        stdErr += eventArgs.Data + Environment.NewLine;
                    }
                };

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();

                Logger.LogInfo(stdOut);
                Logger.LogError(stdErr);

                if (process.ExitCode != 0 || !string.IsNullOrWhiteSpace(stdErr))
                    throw new Exception(stdErr);
            }
        }

        private static string StringifyArgs(IReadOnlyList<string> args)
        {
            var argsQuoted = new string[args.Count];
            for (var i = 0; i < args.Count; i++)
            {
                var arg = args[i];
                argsQuoted[i] = "\"" + arg.Replace(@"\", @"\\").Replace(@"""", @"\""") + "\"";
            }

            return $"{string.Join(" ", argsQuoted)}";
        }
    }
}